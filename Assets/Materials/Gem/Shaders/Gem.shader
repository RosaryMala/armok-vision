// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "FX/Gem"
{
    Properties {
        _Color ("Color", Color) = (1,1,1,1)
        _ReflectionStrength ("Reflection Strength", Range(0.0,2.0)) = 1.0
        _EnvironmentLight ("Environment Light", Range(0.0,2.0)) = 1.0
        _Emission ("Emission", Range(0.0,2.0)) = 0.0
        [NoScaleOffset] _RefractTex ("Refraction Texture", Cube) = "" {}
    }
    SubShader {
        Tags {
            "Queue" = "Transparent"
        }
        // First pass - here we render the backfaces of the diamonds. Since those diamonds are more-or-less
        // convex objects, this is effectively rendering the inside of them.
        Pass {

            Cull Front
            ZWrite Off

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
        
            struct v2f {
                float4 pos : SV_POSITION;
                float3 uv : TEXCOORD0;
            };

            v2f vert (float4 v : POSITION, float3 n : NORMAL)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v);

                // TexGen CubeReflect:
                // reflect view direction along the normal, in view space.
                float3 viewDir = normalize(ObjSpaceViewDir(v));
                o.uv = -reflect(viewDir, n);
                o.uv = mul(unity_ObjectToWorld, float4(o.uv,0));
                return o;
            }

            fixed4 _Color;
            samplerCUBE _RefractTex;
            half _EnvironmentLight;
            half _Emission;
            half4 frag (v2f i) : SV_Target
            {
                half3 refraction = texCUBE(_RefractTex, i.uv).rgb * _Color.rgb;
                half4 reflection = UNITY_SAMPLE_TEXCUBE(unity_SpecCube0, i.uv);
                reflection.rgb = DecodeHDR (reflection, unity_SpecCube0_HDR);
                half3 multiplier = reflection.rgb * _EnvironmentLight + _Emission;
                return half4(refraction.rgb * multiplier.rgb, 1.0f);
            }
            ENDCG 
        }

        // Second pass - here we render the front faces of the diamonds.
        Pass {
            ZWrite On
            Blend One One
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
        
            struct v2f {
                float4 pos : SV_POSITION;
                float3 uv : TEXCOORD0;
                half fresnel : TEXCOORD1;
            };

            v2f vert (float4 v : POSITION, float3 n : NORMAL)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v);

                // TexGen CubeReflect:
                // reflect view direction along the normal, in view space.
                float3 viewDir = normalize(ObjSpaceViewDir(v));
                o.uv = -reflect(viewDir, n);
                o.uv = mul(unity_ObjectToWorld, float4(o.uv,0));
                o.fresnel = 1.0 - saturate(dot(n,viewDir));
                return o;
            }

            fixed4 _Color;
            samplerCUBE _RefractTex;
            half _ReflectionStrength;
            half _EnvironmentLight;
            half _Emission;
            half4 frag (v2f i) : SV_Target
            {
                half3 refraction = texCUBE(_RefractTex, i.uv).rgb * _Color.rgb;
                half4 reflection = UNITY_SAMPLE_TEXCUBE(unity_SpecCube0, i.uv);
                reflection.rgb = DecodeHDR (reflection, unity_SpecCube0_HDR);
                half3 reflection2 = reflection * _ReflectionStrength * i.fresnel;
                half3 multiplier = reflection.rgb * _EnvironmentLight + _Emission;
                return fixed4(reflection2 + refraction.rgb * multiplier, 1.0f);
            }
            ENDCG
        }

        // Shadow casting & depth texture support -- so that gems can
        // cast shadows
        UsePass "VertexLit/SHADOWCASTER"
    }
}
