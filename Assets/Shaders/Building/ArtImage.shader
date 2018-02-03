Shader "Custom/ArtImage" 
{
    Properties 
    {
        _Color ("Color", Color) = (1,1,1,1)
        _Atlas ("Albedo (RGB)", 2DArray) = "white" {}
        _BumpAtlas ("Normal Map", 2DArray) = "bump" {}
        _SpecColor("Standard Specular Color", Color) = (0.220916301, 0.220916301, 0.220916301, 0.779083699)
        _TileIndex ("TileIndex (R)", 2D) = "gray" {}
        [PerRendererData] _MatColor("DF Material Color", Color) = (0.5,0.5,0.5,1)
        [PerRendererData] _MatIndex("DF Material Array Index", int) = 0
    }
    SubShader 
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf StandardSpecular alpha

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 4.0

        UNITY_DECLARE_TEX2DARRAY(_Atlas);
        UNITY_DECLARE_TEX2DARRAY(_BumpAtlas);
        UNITY_DECLARE_TEX2DARRAY(_MatTexArray);
        sampler2D _TileIndex;

        struct Input 
        {
            float2 uv_Atlas;
            float2 uv_TileIndex;
            float2 uv_MatTexArray;
            float3 worldPos;
        };

        half _Glossiness;
        half _Metallic;
        fixed4 _Color;
        float _Tile;

        float3      _ViewMin = float3(-99999, -99999, -99999);
        float3      _ViewMax = float3(99999, 99999, 99999);


UNITY_INSTANCING_BUFFER_START(MyProperties)
UNITY_DEFINE_INSTANCED_PROP(fixed4, _MatColor)
#define _MatColor_arr MyProperties
UNITY_DEFINE_INSTANCED_PROP(int, _MatIndex)
#define _MatIndex_arr MyProperties
UNITY_INSTANCING_BUFFER_END(MyProperties)

#include "blend.cginc"
#include "CustomMetallic.cginc"

        void surf (Input IN, inout SurfaceOutputStandardSpecular o) 
        {
            #ifdef _BOUNDING_BOX_ENABLED
                clip(IN.worldPos - _ViewMin);
                clip(_ViewMax - IN.worldPos);
            #endif
            float4 index = floor(tex2D(_TileIndex, IN.uv_TileIndex) * 255);
            float3 uv = float3(IN.uv_Atlas, index.r);
            uv.xy -= index.ba;
            uv.xy *= index.g;
            clip(UNITY_SAMPLE_TEX2DARRAY (_Atlas, uv).a - 0.5);

            fixed4 dfTex = UNITY_SAMPLE_TEX2DARRAY(_MatTexArray, float3(IN.uv_MatTexArray, UNITY_ACCESS_INSTANCED_PROP(_MatIndex_arr, _MatIndex)));
            fixed4 matColor = UNITY_ACCESS_INSTANCED_PROP(_MatColor_arr, _MatColor);
            fixed3 albedo = overlay(dfTex.rgb, matColor.rgb);
            half smoothness = dfTex.a;
            half metallic = max((matColor.a * 2) - 1, 0);
            fixed alpha = min(matColor.a * 2, 1);

            half3 specColor;
            half oneMinusReflectivity;

            albedo = DiffuseAndSpecularFromMetallicCustom(albedo, metallic, specColor, oneMinusReflectivity);


            o.Albedo = albedo;
            o.Specular = specColor;
            // Metallic and smoothness come from slider variables
            o.Alpha = alpha;
            o.Normal = UnpackNormal(UNITY_SAMPLE_TEX2DARRAY(_BumpAtlas, uv));

        }
        ENDCG
    }
    FallBack "Transparent/Diffuse"
}
