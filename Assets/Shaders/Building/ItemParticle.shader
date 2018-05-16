// Unity built-in shader source. Copyright (c) 2016 Unity Technologies. MIT license (see license.txt)

Shader "Particles/ItemParticle" {
Properties {
    _InvFade ("Soft Particles Factor", Range(0.01,3.0)) = 1.0
}

Category {
    Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" "PreviewType"="Plane" }
    Blend SrcAlpha OneMinusSrcAlpha
    ColorMask RGB
    Cull Off Lighting Off ZWrite Off

    SubShader {
        Pass {

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 2.0
            #pragma multi_compile_particles
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            UNITY_DECLARE_TEX2DARRAY(_ImageAtlas);
            UNITY_DECLARE_TEX2DARRAY(_MatTexArray);

            struct appdata_t {
                float4 vertex : POSITION;
                fixed4 color : COLOR;
                float4 texcoord : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f {
                float4 vertex : SV_POSITION;
                fixed4 color : COLOR;
                float2 texcoord : TEXCOORD0;
                float2 atlasIndex : TEXCOORD1;
                UNITY_FOG_COORDS(2)
                #ifdef SOFTPARTICLES_ON
                float4 projPos : TEXCOORD4;
                #endif
                UNITY_VERTEX_OUTPUT_STEREO
            };

            float3      _ViewMin = float3(-99999, -99999, -99999);
            float3      _ViewMax = float3(99999, 99999, 99999);

            v2f vert (appdata_t v)
            {
                v2f o;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
                o.vertex = UnityObjectToClipPos(v.vertex);
                #ifdef SOFTPARTICLES_ON
                o.projPos = ComputeScreenPos (o.vertex);
                COMPUTE_EYEDEPTH(o.projPos.z);
                #endif
                o.color = v.color;
                o.texcoord = v.texcoord.xy;
                UNITY_TRANSFER_FOG(o,o.vertex);
                o.atlasIndex = v.texcoord.zw;
                return o;
            }

            UNITY_DECLARE_DEPTH_TEXTURE(_CameraDepthTexture);
            float _InvFade;

            fixed4 frag (v2f i) : SV_Target
            {
                #ifdef _BOUNDING_BOX_ENABLED
                    clip(IN.worldPos - _ViewMin);
                    clip(_ViewMax - IN.worldPos);
                #endif
                float3 uv = float3(i.texcoord, i.atlasIndex.x);
                float4 c = UNITY_SAMPLE_TEX2DARRAY (_ImageAtlas, uv);
                clip(c.a - 0.5);

                fixed4 dfTex = UNITY_SAMPLE_TEX2DARRAY(_MatTexArray, float3(i.texcoord, i.atlasIndex.y));
                fixed3 albedo = dfTex.rgb * i.color.rgb;
                c.a *= min(i.color.a * 2, 1);
                c.rgb =c.rgb * albedo;

                #ifdef SOFTPARTICLES_ON
                float sceneZ = LinearEyeDepth (SAMPLE_DEPTH_TEXTURE_PROJ(_CameraDepthTexture, UNITY_PROJ_COORD(i.projPos)));
                float partZ = i.projPos.z;
                float fade = saturate (_InvFade * (sceneZ-partZ));
                i.color.a *= fade;
                #endif

                fixed4 col = c;
                UNITY_APPLY_FOG(i.fogCoord, col);
                return col;
            }
            ENDCG
        }
    }
}
}
