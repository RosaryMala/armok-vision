// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "Unlit/TerrainSplatShadowFallback"
{
    Properties
    {
        _BumpMap("Normalmap (RGB) Occlusion (A)", 2DArray) = "bump" {}
        _SpecialTex("Metallic (R)", 2DArray) = "black" {}
        [PerRendererData]_SpatterTex("Spatter", 2D) = "" {}
        _SpatterDirection("Spatter Direction", Vector) = (0,1,0)
        _SpatterSmoothness("Spatter Smoothness", Range(0,1)) = 0
        _WorldBounds("World Bounds", Vector) = (0,0,1,1)
        _SpatterNoise("Spatter Noise", 2D) = "white" {}
        _TexArrayCount("Texture array count", Vector) = (0,0,0,0)
    }
        SubShader
    {
        Tags{ "RenderType" = "Opaque" }
        LOD 100

        // Pass to render object as a shadow caster
        Pass{
        Name "Caster"
        Tags{ "LightMode" = "ShadowCaster" }

        CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 3.5
            #pragma multi_compile_shadowcaster
            #pragma multi_compile_instancing // allow instanced shadow pass for most of the shaders
            #pragma multi_compile __ CONTAMINANTS
            #include "UnityCG.cginc"
            #include "UnityStandardUtils.cginc"

            UNITY_DECLARE_TEX2DARRAY(_BumpMap);
            UNITY_DECLARE_TEX2DARRAY(_SpecialTex);
            sampler2D _SpatterTex;
            sampler2D _SpatterNoise;
            float4 _SpatterDirection;
            float _SpatterSmoothness;
            float4 _WorldBounds;
            float4 _SpatterNoise_ST;
            float4 _TexArrayCount;

            sampler3D _DitherMaskLOD;

            struct v2f {
                V2F_SHADOW_CASTER_NOPOS
                // these three vectors will hold a 3x3 rotation matrix
                // that transforms from tangent to world space
                float4 tspace0 : TEXCOORD1; // tangent.x, bitangent.x, normal.x
                float4 tspace1 : TEXCOORD2; // tangent.y, bitangent.y, normal.y
                float4 tspace2 : TEXCOORD3; // tangent.z, bitangent.z, normal.z
                // texture coordinate for the normal map
                float2 uv : TEXCOORD4;
                float2 uv2 : TEXCOORD5;
                fixed4 color : COLOR;
                UNITY_VERTEX_OUTPUT_STEREO
            };

            uniform float4 _MainTex_ST;

            v2f vert(appdata_full v, out float4 opos : SV_POSITION)
            {
                v2f o;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
                TRANSFER_SHADOW_CASTER_NOPOS(o, opos)
                o.uv = v.texcoord;
                o.uv2 = v.texcoord2;
                half3 wNormal = UnityObjectToWorldNormal(v.normal);
                half3 wTangent = UnityObjectToWorldDir(v.tangent.xyz);
                // compute bitangent from cross product of normal and tangent
                half tangentSign = v.tangent.w * unity_WorldTransformParams.w;
                half3 wBitangent = cross(wNormal, wTangent) * tangentSign;
                // output the tangent space matrix
                float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                o.tspace0 = float4(wTangent.x, wBitangent.x, wNormal.x, worldPos.x);
                o.tspace1 = float4(wTangent.y, wBitangent.y, wNormal.y, worldPos.y);
                o.tspace2 = float4(wTangent.z, wBitangent.z, wNormal.z, worldPos.z);
                o.color = v.color;
                return o;
            }

            float4 frag(v2f i, UNITY_VPOS_TYPE vpos : VPOS) : SV_Target
            {
                fixed4 bump = UNITY_SAMPLE_TEX2DARRAY(_BumpMap, float3(i.uv.xy, i.uv2.y * _TexArrayCount.y));
                fixed4 spatter = tex2D(_SpatterTex, (float2(i.tspace0.w, i.tspace1.w) - _WorldBounds.xy) / (_WorldBounds.zw - _WorldBounds.xy));
                fixed4 noise = tex2D(_SpatterNoise, TRANSFORM_TEX(float2(i.tspace0.w, i.tspace1.w), _SpatterNoise));
                
                // sample the normal map, and decode from the Unity encoding
                half3 tnormal = UnpackNormal(bump.ggga);
                // transform normal from tangent to world space
                half3 worldNormal;
                worldNormal.x = dot(i.tspace0, tnormal);
                worldNormal.y = dot(i.tspace1, tnormal);
                worldNormal.z = dot(i.tspace2, tnormal);

                float alpha = min((i.color.a * 2), 1) *bump.b;
                half metallic = max((i.color.a * 2) - 1, 0);
#ifdef CONTAMINANTS
                if (dot(float3(worldNormal), _SpatterDirection.xyz) >= lerp(1, -1, (spatter.a - noise.r)))
                {
                    alpha = 1;
                    metallic = 0;
                }
#endif

                //following use the alpha and metallic variables only.
                half outModifiedAlpha;
                PreMultiplyAlpha(half3(0, 0, 0), alpha, OneMinusReflectivityFromMetallic(metallic), outModifiedAlpha);
                alpha = outModifiedAlpha;

                half alphaRef = tex3D(_DitherMaskLOD, float3(vpos.xy*0.25, alpha*0.9375)).a;
                clip(alphaRef - 0.01);

                SHADOW_CASTER_FRAGMENT(i)
            }
        ENDCG
        }
    }
}
