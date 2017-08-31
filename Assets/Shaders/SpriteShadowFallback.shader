Shader "Unlit/AVSprite Shadow"
{
	Properties
	{
        [PerRendererData] _MainTex("Albedo (RGB)", 2D) = "white" {}
    _Color("Tint", Color) = (1,1,1,1)
        _Cutoff("Alpha cutoff", Range(0,1)) = 0.5
    }
	SubShader
	{
		Tags 
    { 
        "RenderType"="Opaque"
        "PreviewType" = "Plane"
        "CanUseSpriteAtlas" = "True"
    }
		LOD 100

        // Pass to render object as a shadow caster
                Pass{
                Name "Caster"
                Tags{ "LightMode" = "ShadowCaster" }

                CGPROGRAM
                #pragma vertex vert
                #pragma fragment frag
                #pragma target 3.0
                #pragma multi_compile_shadowcaster
                #pragma multi_compile_instancing // allow instanced shadow pass for most of the shaders
                #pragma shader_feature _TEXTURE_MASK
                #include "UnityCG.cginc"
                #include "UnityStandardUtils.cginc"

        sampler2D _MainTex;
    float4 _MainTex_TexelSize;
    sampler2D _AlphaTex;
    fixed4 _Color;
    float _Cutoff;

        sampler3D _DitherMaskLOD;

        struct v2f {
                V2F_SHADOW_CASTER_NOPOS
                float2  uv : TEXCOORD1;
                float4 color    : COLOR;
                UNITY_VERTEX_OUTPUT_STEREO
                };

            uniform float4 _MainTex_ST;

            v2f vert(appdata_full v, out float4 opos : SV_POSITION)
            {
                v2f o;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
                TRANSFER_SHADOW_CASTER_NOPOS(o, opos)
                o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
                o.color = v.color * _Color;
                return o;
            }

            float4 frag(v2f i, UNITY_VPOS_TYPE vpos : VPOS) : SV_Target
            {
            fixed4 c = tex2D(_MainTex, i.uv);
            fixed alpha = c.a * min((i.color.a * 2), 1);
            half metallic = max((i.color.a * 2) - 1, 0);

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
