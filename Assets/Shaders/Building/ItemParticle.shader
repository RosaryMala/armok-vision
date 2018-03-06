Shader "Custom/ItemParticle" {
    Properties {
        _SpecColor("Standard Specular Color", Color) = (0.220916301, 0.220916301, 0.220916301, 0.779083699)
        [PerRendererData] _MatColor("DF Material Color", Color) = (0.5,0.5,0.5,1)
        [PerRendererData] _MatIndex("DF Material Array Index", int) = 0
    }
    SubShader {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf Standard fullforwardshadows alpha vertex:vert

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0
        #pragma multi_compile _ _BOUNDING_BOX_ENABLED

        UNITY_DECLARE_TEX2DARRAY(_ImageAtlas);
        UNITY_DECLARE_TEX2DARRAY(_ImageBumpAtlas);
        UNITY_DECLARE_TEX2DARRAY(_MatTexArray);

        sampler2D _MainTex;

        struct appdata_particles {
            float4 vertex : POSITION;
            float3 normal : NORMAL;
            float4 color : COLOR;
            float4 texcoord : TEXCOORD0;
            float2 texcoordIndex : TEXCOORD1;
        };

        struct Input {
            float2 uv_ImageAtlas;
            float3 worldPos;
            float2 texcoord1; 
            float2 imageAtlasIndex;
            float4 color;
        };

        float3      _ViewMin = float3(-99999, -99999, -99999);
        float3      _ViewMax = float3(99999, 99999, 99999);
        float _ContributionAlbedo;

#include "blend.cginc"

        void vert(inout appdata_particles v, out Input o) {
            UNITY_INITIALIZE_OUTPUT(Input, o);
            o.uv_ImageAtlas = v.texcoord.xy;
            o.texcoord1 = v.texcoord.zw;
            o.imageAtlasIndex = v.texcoordIndex.xy;
            o.color = v.color;
        }

        // Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
        // See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
        // #pragma instancing_options assumeuniformscaling
        UNITY_INSTANCING_BUFFER_START(Props)
            // put more per-instance properties here
        UNITY_INSTANCING_BUFFER_END(Props)

        void surf (Input IN, inout SurfaceOutputStandard o) 
        {
        #ifdef _BOUNDING_BOX_ENABLED
            clip(IN.worldPos - _ViewMin);
            clip(_ViewMax - IN.worldPos);
        #endif

            float3 uv = float3(IN.uv_ImageAtlas, IN.imageAtlasIndex.x);
            float4 c = UNITY_SAMPLE_TEX2DARRAY(_ImageAtlas, uv);
            clip(c.a - 0.5);

            float4 dfTex = UNITY_SAMPLE_TEX2DARRAY(_MatTexArray, float3(IN.uv_ImageAtlas, IN.imageAtlasIndex.y));
            float3 albedo = overlay(dfTex.rgb, IN.color.rgb);
            float metallic = max((IN.color.a * 2) - 1, 0);
            float alpha = min(IN.color.a * 2, 1);

            o.Albedo = overlay(c.rgb, albedo);
            // Metallic and smoothness come from slider variables
            o.Metallic = metallic;
            o.Smoothness = dfTex.a;
            o.Alpha = alpha;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
