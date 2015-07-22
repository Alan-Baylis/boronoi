// Shader created with Shader Forge v1.16 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.16;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,lico:1,lgpr:1,limd:1,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,rpth:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,culm:0,bsrc:0,bdst:1,dpts:2,wrdp:True,dith:0,rfrpo:True,rfrpn:Refraction,ufog:False,aust:True,igpj:False,qofs:0,qpre:1,rntp:1,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,ofsf:0,ofsu:0,f2p0:False;n:type:ShaderForge.SFN_Final,id:634,x:32778,y:32848,varname:node_634,prsc:2|diff-3105-OUT;n:type:ShaderForge.SFN_Tex2d,id:6219,x:31862,y:32518,ptovrint:False,ptlb:t1,ptin:_t1,varname:node_6219,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:9d2885fa3c2ba4e4f9b8b0ffa32259a2,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Lerp,id:5952,x:32464,y:32626,varname:node_5952,prsc:2|A-6219-RGB,B-7783-RGB,T-9924-R;n:type:ShaderForge.SFN_Tex2d,id:7783,x:31862,y:32728,ptovrint:False,ptlb:t2,ptin:_t2,varname:node_7783,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:fdf730f95ce033e4f840529d329ef289,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Tex2d,id:4824,x:31859,y:32905,ptovrint:False,ptlb:t3,ptin:_t3,varname:node_4824,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:cfd39afd1c27c5d48a4d145ac6f24568,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Tex2d,id:910,x:31859,y:33106,ptovrint:False,ptlb:t4,ptin:_t4,varname:node_910,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:02e7b1ec10ec1514db73b7e2f1e2f50a,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Lerp,id:5355,x:32464,y:32793,varname:node_5355,prsc:2|A-5952-OUT,B-4824-RGB,T-9924-G;n:type:ShaderForge.SFN_Lerp,id:3600,x:32464,y:32958,varname:node_3600,prsc:2|A-5355-OUT,B-910-RGB,T-9924-B;n:type:ShaderForge.SFN_Lerp,id:3105,x:32464,y:33138,varname:node_3105,prsc:2|A-3600-OUT,B-9423-RGB,T-9924-A;n:type:ShaderForge.SFN_Tex2d,id:9423,x:31859,y:33317,ptovrint:False,ptlb:t5,ptin:_t5,varname:node_9423,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:9efed271a5bfd0c4485fb05548f54332,ntxv:0,isnm:False;n:type:ShaderForge.SFN_VertexColor,id:9924,x:32074,y:32396,varname:node_9924,prsc:2;n:type:ShaderForge.SFN_Tex2d,id:6998,x:32207,y:32209,ptovrint:False,ptlb:node_6998,ptin:_node_6998,varname:node_6998,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:0e64cd744429828468bed72e94aa027e,ntxv:0,isnm:False;proporder:6219-7783-4824-910-9423-6998;pass:END;sub:END;*/

Shader "Custom/Terrain" {
    Properties {
        _t1 ("t1", 2D) = "white" {}
        _t2 ("t2", 2D) = "white" {}
        _t3 ("t3", 2D) = "white" {}
        _t4 ("t4", 2D) = "white" {}
        _t5 ("t5", 2D) = "white" {}
        _node_6998 ("node_6998", 2D) = "white" {}
    }
    SubShader {
        Tags {
            "RenderType"="Opaque"
        }
        LOD 200
        Pass {
            Name "FORWARD"
            Tags {
                "LightMode"="ForwardBase"
            }
            
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDBASE
            #include "UnityCG.cginc"
            #include "AutoLight.cginc"
            #pragma multi_compile_fwdbase_fullshadows
            #pragma exclude_renderers gles3 metal d3d11_9x xbox360 xboxone ps3 ps4 psp2 
            #pragma target 3.0
            uniform float4 _LightColor0;
            uniform sampler2D _t1; uniform float4 _t1_ST;
            uniform sampler2D _t2; uniform float4 _t2_ST;
            uniform sampler2D _t3; uniform float4 _t3_ST;
            uniform sampler2D _t4; uniform float4 _t4_ST;
            uniform sampler2D _t5; uniform float4 _t5_ST;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 texcoord0 : TEXCOORD0;
                float4 vertexColor : COLOR;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float4 posWorld : TEXCOORD1;
                float3 normalDir : TEXCOORD2;
                float4 vertexColor : COLOR;
                LIGHTING_COORDS(3,4)
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.vertexColor = v.vertexColor;
                o.normalDir = UnityObjectToWorldNormal(v.normal);
                o.posWorld = mul(_Object2World, v.vertex);
                float3 lightColor = _LightColor0.rgb;
                o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
                TRANSFER_VERTEX_TO_FRAGMENT(o)
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
                i.normalDir = normalize(i.normalDir);
/////// Vectors:
                float3 normalDirection = i.normalDir;
                float3 lightDirection = normalize(_WorldSpaceLightPos0.xyz);
                float3 lightColor = _LightColor0.rgb;
////// Lighting:
                float attenuation = LIGHT_ATTENUATION(i);
                float3 attenColor = attenuation * _LightColor0.xyz;
/////// Diffuse:
                float NdotL = max(0.0,dot( normalDirection, lightDirection ));
                float3 directDiffuse = max( 0.0, NdotL) * attenColor;
                float3 indirectDiffuse = float3(0,0,0);
                indirectDiffuse += UNITY_LIGHTMODEL_AMBIENT.rgb; // Ambient Light
                float4 _t1_var = tex2D(_t1,TRANSFORM_TEX(i.uv0, _t1));
                float4 _t2_var = tex2D(_t2,TRANSFORM_TEX(i.uv0, _t2));
                float4 _t3_var = tex2D(_t3,TRANSFORM_TEX(i.uv0, _t3));
                float4 _t4_var = tex2D(_t4,TRANSFORM_TEX(i.uv0, _t4));
                float4 _t5_var = tex2D(_t5,TRANSFORM_TEX(i.uv0, _t5));
                float3 diffuseColor = lerp(lerp(lerp(lerp(_t1_var.rgb,_t2_var.rgb,i.vertexColor.r),_t3_var.rgb,i.vertexColor.g),_t4_var.rgb,i.vertexColor.b),_t5_var.rgb,i.vertexColor.a);
                float3 diffuse = (directDiffuse + indirectDiffuse) * diffuseColor;
/// Final Color:
                float3 finalColor = diffuse;
                return fixed4(finalColor,1);
            }
            ENDCG
        }
        Pass {
            Name "FORWARD_DELTA"
            Tags {
                "LightMode"="ForwardAdd"
            }
            Blend One One
            
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDADD
            #include "UnityCG.cginc"
            #include "AutoLight.cginc"
            #pragma multi_compile_fwdadd_fullshadows
            #pragma exclude_renderers gles3 metal d3d11_9x xbox360 xboxone ps3 ps4 psp2 
            #pragma target 3.0
            uniform float4 _LightColor0;
            uniform sampler2D _t1; uniform float4 _t1_ST;
            uniform sampler2D _t2; uniform float4 _t2_ST;
            uniform sampler2D _t3; uniform float4 _t3_ST;
            uniform sampler2D _t4; uniform float4 _t4_ST;
            uniform sampler2D _t5; uniform float4 _t5_ST;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 texcoord0 : TEXCOORD0;
                float4 vertexColor : COLOR;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float4 posWorld : TEXCOORD1;
                float3 normalDir : TEXCOORD2;
                float4 vertexColor : COLOR;
                LIGHTING_COORDS(3,4)
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.vertexColor = v.vertexColor;
                o.normalDir = UnityObjectToWorldNormal(v.normal);
                o.posWorld = mul(_Object2World, v.vertex);
                float3 lightColor = _LightColor0.rgb;
                o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
                TRANSFER_VERTEX_TO_FRAGMENT(o)
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
                i.normalDir = normalize(i.normalDir);
/////// Vectors:
                float3 normalDirection = i.normalDir;
                float3 lightDirection = normalize(lerp(_WorldSpaceLightPos0.xyz, _WorldSpaceLightPos0.xyz - i.posWorld.xyz,_WorldSpaceLightPos0.w));
                float3 lightColor = _LightColor0.rgb;
////// Lighting:
                float attenuation = LIGHT_ATTENUATION(i);
                float3 attenColor = attenuation * _LightColor0.xyz;
/////// Diffuse:
                float NdotL = max(0.0,dot( normalDirection, lightDirection ));
                float3 directDiffuse = max( 0.0, NdotL) * attenColor;
                float4 _t1_var = tex2D(_t1,TRANSFORM_TEX(i.uv0, _t1));
                float4 _t2_var = tex2D(_t2,TRANSFORM_TEX(i.uv0, _t2));
                float4 _t3_var = tex2D(_t3,TRANSFORM_TEX(i.uv0, _t3));
                float4 _t4_var = tex2D(_t4,TRANSFORM_TEX(i.uv0, _t4));
                float4 _t5_var = tex2D(_t5,TRANSFORM_TEX(i.uv0, _t5));
                float3 diffuseColor = lerp(lerp(lerp(lerp(_t1_var.rgb,_t2_var.rgb,i.vertexColor.r),_t3_var.rgb,i.vertexColor.g),_t4_var.rgb,i.vertexColor.b),_t5_var.rgb,i.vertexColor.a);
                float3 diffuse = directDiffuse * diffuseColor;
/// Final Color:
                float3 finalColor = diffuse;
                return fixed4(finalColor * 1,0);
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
