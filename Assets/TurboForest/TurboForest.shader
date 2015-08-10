Shader "Custom/TurboForest" 
{
	Properties 
	{
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_Bump ("Bump (RGB)", 2D) = "white" {}
	}
	SubShader 
	{
      Tags {"Queue"="AlphaTest" "IgnoreProjector"="True"}
      LOD 20
      Pass 
      {   
		
		CGPROGRAM
		#pragma vertex vert  
		#pragma fragment frag 
		#pragma exclude_renderers xbox360
		
		uniform sampler2D _MainTex;        
		uniform sampler2D _Bump;
		fixed4 _LightColor0;
		
		struct vertexInput  
		{
			float4 vertex : POSITION;
			float4 tex : TEXCOORD0;
			float4 tex1 : TEXCOORD1;
			
			float4 pos : NORMAL;
		};
		struct vertexOutput 
		{
			float4 pos : SV_POSITION;
			float4 tex : TEXCOORD0;
			float4 lightDirection : TEXCOORD1;
			float tint: TEXCOORD2;
		};
 
		vertexOutput vert(vertexInput input) 
		{
			vertexOutput output;

			output.pos = mul(UNITY_MATRIX_P, mul(UNITY_MATRIX_MV, float4(input.pos.x, input.pos.y, input.pos.z, 1.0) ) - float4(input.vertex.x, -input.vertex.y, 0.0, 0.0));
			output.tex = input.tex;
			output.tint = input.vertex.z;

			float3 dir=normalize(_WorldSpaceCameraPos.xyz - input.pos.xyz);
			float d=saturate(dot(dir,float3(0,1,0)));

			output.tex.y+=1.0-(1.0/8)*((int)(d*4))-1.0/8;
			output.tex.x+=(1.0/8)*((int)((d*4-(int)(d*4))*4));
			
			output.tex.x+=input.tex1.x;
			output.tex.y+=input.tex1.y;
			
			float4 lightDir=_WorldSpaceLightPos0;

			output.lightDirection.xyz = normalize( mul(UNITY_MATRIX_VP,lightDir).xyz );

			return output;
		}

		float4 frag(vertexOutput input) : COLOR
		{
			fixed4 c=tex2D(_MainTex, input.tex.xy);
			if(c.a<.5) discard;

			//float3 bump=tex2D(_Bump, input.tex.xy).xyz;

			//bump.xy=0.5-bump.xy;

			//float att= (dot( normalize(bump*2-0.5) ,normalize(-input.lightDirection.xyz))+1)/2 ;

			//c.rgb*=input.tint;
			//c.rgb=float3(UNITY_LIGHTMODEL_AMBIENT.rgb)*c.rgb+c.rgb*att*_LightColor0.rgb;
			
			return c;
		}
		ENDCG

		}

		// Pass to render object as a shadow caster
		
		Pass 
	    {
			Name "ShadowCaster"
			Tags { "LightMode" = "ShadowCaster" }
			Fog {Mode Off}
			ZWrite On ZTest LEqual Cull Off
			Offset 1, 1
			
			
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_shadowcaster
			#pragma fragmentoption ARB_precision_hint_fastest
			#include "UnityCG.cginc"
			#pragma exclude_renderers xbox360
			
			uniform sampler2D _MainTex; 
			uniform float4x4 _LightMatrix0;
			
			struct v2f 
			{
				float4 tex : TEXCOORD1;
				V2F_SHADOW_CASTER;
			};

			v2f vert( appdata_full v )
			{
				v2f o;
				
				o.pos=mul(UNITY_MATRIX_P, mul(UNITY_MATRIX_MV, float4(v.normal.x, v.normal.y, v.normal.z, 1.0)) - float4(v.vertex.x, -v.vertex.y, 0.0, 0.0));
			    o.pos.z += unity_LightShadowBias.x;
			    float clamped = max(o.pos.z, o.pos.w*UNITY_NEAR_CLIP_VALUE); 
			    o.pos.z = lerp(o.pos.z, clamped, unity_LightShadowBias.y);
			
				o.tex.xy = v.texcoord.xy+v.texcoord1.xy;
				o.tex.y+=0.5;

			    return o;
			}

			float4 frag( v2f i ) : COLOR
			{
				if(tex2D(_MainTex, i.tex.xy).a<0.5) discard;
				
				SHADOW_CASTER_FRAGMENT(i)
				
			}
			ENDCG
		}
	}
	
}
