
Shader "Nature/Terrain/TerrainTexturingWithTileMapping" 
{
	Properties 
	{
		_ControlMap("Control map (RGBA)", 2D) = "red" {}
		_MappingIndex("Mapping index", 2D) = "black" {}
		_SplatTex3("Layer 3 (A)", 2D) = "white" {}
		_SplatTex2("Layer 2 (B)", 2D) = "white" {}
		_SplatTex1("Layer 1 (G)", 2D) = "white" {}
		_SplatTex0("Layer 0 (R)", 2D) = "white" {}
		// used in fallback on old cards
		_MainTex("BaseMap (RGB)", 2D) = "white" {}
		_Color("Main Color", Color) = (1,1,1,1)
		//if you use a tile map with two horizontal and two vertical colors this should be 4,4 (ie 2 squared)
		_TileScale("Tile scale", Vector) = (4,4,0,0) 
		//If you make your own mapping index and it is not 1024 by 1024 pixels you will need to change this
		_TileMappingScale("Tile mapping scale", Vector) = (1024,1024,0,0) 
		//If you add your own border to the control maps and it is not 2 pixesl you will need to change this.
		//If you use a control map with no edge borders set this to 0 to disable it 
		_BorderSize("Border size", Float) = 2
		_ControlMapSize("Control Map Size", Float) = 2048
	}
	
	SubShader 
	{
		Tags 
		{
			"SplatCount" = "4"
			"Queue" = "Geometry-100"
			"RenderType" = "Opaque"
		}
	
		CGPROGRAM
		#pragma surface surf Lambert vertex:vert
		#pragma target 3.0
		#pragma glsl
		#include "UnityCG.cginc"
		
		sampler2D _ControlMap, _MappingIndex;
		sampler2D _SplatTex0,_SplatTex1,_SplatTex2,_SplatTex3;
		float2 _TileScale, _TileMappingScale;
		float _BorderSize, _ControlMapSize;
		half3 _Color;
		
		struct Input 
		{
			float2 uv_ControlMap : TEXCOORD0;
			float2 uv_SplatTex0 : TEXCOORD1;
			//float2 uv_SplatTex1 : TEXCOORD2;
			//float2 uv_SplatTex2 : TEXCOORD3;
			//float2 uv_SplatTex3 : TEXCOORD4;
		};
		
		// Supply the shader with tangents for the terrain
		//There is no normal mapping but I have left in the tangent calc just in case you want to add it
		void vert (inout appdata_full v) 
		{
			// A general tangent estimation	
			float3 T1 = float3(1, 0, 1);
			float3 Bi = cross(T1, v.normal);
			float3 newTangent = cross(v.normal, Bi);
			normalize(newTangent);
			v.tangent.xyz = newTangent.xyz;
			if (dot(cross(v.normal,newTangent),Bi) < 0)
				v.tangent.w = -1.0f;
			else
				v.tangent.w = 1.0f;
		}
		
		void surf (Input IN, inout SurfaceOutput o) 
		{
			//Adjust the uvs for the control map to remove seams at terrain edges
			float u = 1.0/_ControlMapSize;
			float2 uv_ControlMap = IN.uv_ControlMap * (1.0-u*_BorderSize*2.0) + float2(u,u)*_BorderSize;
		
			half4 splatControl = tex2D(_ControlMap, uv_ControlMap);
			
			//Which tile to use is determined by the mapping index and a set of uv's.
			//I am using the uv's for splat0 for all the splat textures.
			//This means only one look up of the mapping index is needed but it also 
			//means that the uv's of splat0 also controls splat 1, 2, and 3.
			//If you want unique uv's for each splat tex you will need to calculate which tile for each set of uv's
        	float2 whichTile = tex2D(_MappingIndex, IN.uv_SplatTex0).xy * 255.0; 
        	
        	//If uncommented this will just use the top left tile in the splat texture and  
        	//is simiar to using a normal repeating texture
        	//whichTile = 0;  
	
        	float2 tileScaledTex = IN.uv_SplatTex0 * _TileMappingScale * (1.0/_TileScale);
        	float2 mappingAddress = IN.uv_SplatTex0 * _TileMappingScale;
        	
        	//using the derivatives of the tile scale removes some artifacts that are caused by the mip mapping of the tiles textures
        	float2 dd_x = ddx(tileScaledTex);
        	float2 dd_y = ddy(tileScaledTex);
        	float2 uv = (whichTile + frac(mappingAddress))/_TileScale;
        	
        	half4 splat0 = tex2D(_SplatTex0, uv, dd_x, dd_y);
        	half4 splat1 = tex2D(_SplatTex1, uv, dd_x, dd_y);
        	half4 splat2 = tex2D(_SplatTex2, uv, dd_x, dd_y);
        	half4 splat3 = tex2D(_SplatTex3, uv, dd_x, dd_y);

        	splat0 = lerp(splat0, splat1, splatControl.r);
        	splat0 = lerp(splat0, splat2, splatControl.g);
        	splat0 = lerp(splat0, splat3, splatControl.b);
        	
			o.Albedo = splat0.rgb;
			o.Alpha = 1.0;
		}
		ENDCG  
	}

}