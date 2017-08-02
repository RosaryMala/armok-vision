Shader "Unlit/BuildingShadowFallback"
{
	Properties
	{
        _Color("Color", Color) = (1,1,1,1)
        _MainTex("Albedo (RGB)", 2D) = "white" {}

    _Cutoff("Alpha Cutoff", Range(0.0, 1.0)) = 0.5

        _Glossiness("Smoothness", Range(0.0, 1.0)) = 0.5
        _GlossMapScale("Smoothness Scale", Range(0.0, 1.0)) = 1.0

        [Gamma] _Metallic("Metallic", Range(0.0, 1.0)) = 0.0
        _MetallicGlossMap("Metallic", 2D) = "white" {}

    _BumpScale("Scale", Float) = 1.0
        _BumpMap("Normal Map", 2D) = "bump" {}

    _OcclusionStrength("Strength", Range(0.0, 1.0)) = 1.0
        _OcclusionMap("Occlusion", 2D) = "white" {}

    _EmissionColor("Color", Color) = (0,0,0)
        _EmissionMap("Emission", 2D) = "white" {}

    _DFMask("DF Material Splat", 2D) = "black" {}

    [Enum(UV0,0,UV1,1)] _UVSec("UV Set for secondary textures", Float) = 0

        _MatTex("DF Material Texture", 2DArray) = "grey" {}
    [PerRendererData] _MatColor("DF Material Color", Color) = (0.5,0.5,0.5,1)
        [PerRendererData] _MatIndex("DF Material Array Index", int) = 0

        // Blending state
        [HideInInspector] _Mode("__mode", Float) = 0.0
    }
	SubShader
	{
		Tags { "RenderType"="Opaque" }
		LOD 100

        // Pass to render object as a shadow caster
                Pass{
                Name "Caster"
                Tags{ "LightMode" = "ShadowCaster" }

                CGPROGRAM
                #pragma vertex vert
                #pragma fragment frag
                #pragma target 2.0
                #pragma multi_compile_shadowcaster
                #pragma multi_compile_instancing // allow instanced shadow pass for most of the shaders
                #pragma shader_feature _TEXTURE_MASK
                #include "UnityCG.cginc"
                #include "buildingInputs.cginc"
                #include "UnityStandardUtils.cginc"

        sampler3D _DitherMaskLOD;

        struct v2f {
                V2F_SHADOW_CASTER_NOPOS
                float2  uv : TEXCOORD0;
                UNITY_VERTEX_OUTPUT_STEREO
                };

            uniform float4 _MainTex_ST;

            v2f vert(appdata_base v, out float4 opos : SV_POSITION)
            {
                v2f o;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
                TRANSFER_SHADOW_CASTER_NOPOS(o, opos)
                o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
                return o;
            }

            float4 frag(v2f i, UNITY_VPOS_TYPE vpos : VPOS) : SV_Target
            {
            fixed4 matColor = UNITY_ACCESS_INSTANCED_PROP(_MatColor);
            fixed alpha = min((matColor.a * 2), 1);
            half metallic = max((matColor.a * 2) - 1, 0);

#ifdef _TEXTURE_MASK
            fixed4 mask = tex2D(_DFMask, i.uv);
            fixed4 c = tex2D(_MainTex, i.uv) * _Color;
            alpha = lerp(alpha, c.a, mask.r);
#ifdef _METALLICGLOSSMAP
            metallic = lerp(metallic, m.r, mask.r);
#else
            metallic = lerp(metallic, _Metallic, mask.r);
#endif
#endif
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
        FallBack "Diffuse"
        CustomEditor "BuildingMaterialEditor"

}
