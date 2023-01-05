Shader "Art/EngravingVertex" 
{
    Properties 
    {
        _Color ("Color", Color) = (1,1,1,1)
        _SpecColor("Standard Specular Color", Color) = (0.220916301, 0.220916301, 0.220916301, 0.779083699)
        _ContributionAlbedo ("Contribution / Albedo", Range(0,1)) = 0.0
        _ContributionSpecSmoothness ("Contribution / Smoothness", Range(0,1)) = 0.0
        _ContributionNormal ("Contribution / Normal", Range(0,1)) = 1.0
        _ContributionEmission ("Contribution / Emission", Range(0,1)) = 1.0
        _BumpScale("Scale", Range(0,1)) = 1.0

    }
    SubShader 
    {
        Tags {"Queue"="AlphaTest" "IgnoreProjector"="True" "RenderType"="Opaque" "ForceNoShadowCasting"="True"}
        LOD 300
        Offset -1, -1
        Blend SrcAlpha OneMinusSrcAlpha

        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf Standard vertex:vert finalgbuffer:DecalFinalGBuffer keepalpha exclude_path:forward exclude_path:prepass noshadow noforwardadd

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 4.0
        #pragma multi_compile _ _BOUNDING_BOX_ENABLED

        UNITY_DECLARE_TEX2DARRAY(_ImageAtlas);
        UNITY_DECLARE_TEX2DARRAY(_ImageBumpAtlas);

        struct Input 
        {
            float2 uv_ImageAtlas;
            float3 worldPos;
            float imageAtlasIndex;
         };

        float3      _ViewMin = float3(-99999, -99999, -99999);
        float3      _ViewMax = float3(99999, 99999, 99999);
        float _BumpScale;
        float4 _Color;

UNITY_INSTANCING_BUFFER_START(MyProperties)
UNITY_DEFINE_INSTANCED_PROP(fixed4, _MatColor)
#define _MatColor_arr MyProperties
UNITY_DEFINE_INSTANCED_PROP(int, _MatIndex)
#define _MatIndex_arr MyProperties
UNITY_INSTANCING_BUFFER_END(MyProperties)

#include "CustomMetallic.cginc"

        half _ContributionAlbedo;
        half _ContributionSpecSmoothness;
        half _ContributionNormal;
        half _ContributionEmission;

        void vert (inout appdata_full v, out Input o) 
        {
            UNITY_INITIALIZE_OUTPUT(Input,o);
            o.imageAtlasIndex = v.texcoord3.x;
        }

        void surf (Input IN, inout SurfaceOutputStandard o) 
        {
            #ifdef _BOUNDING_BOX_ENABLED
                clip(IN.worldPos - _ViewMin);
                clip(_ViewMax - IN.worldPos);
            #endif
            float3 uv = float3(IN.uv_ImageAtlas, IN.imageAtlasIndex);

            fixed4 matColor = UNITY_SAMPLE_TEX2DARRAY (_ImageAtlas, uv) * _Color;
            clip(matColor.a - 0.5);

            o.Albedo = matColor.rgb;
            o.Alpha = matColor.a;
            o.Normal = UnpackScaleNormal(1-UNITY_SAMPLE_TEX2DARRAY(_ImageBumpAtlas, uv), _BumpScale);
        }

        void DecalFinalGBuffer (Input IN, SurfaceOutputStandard o, inout half4 diffuse, inout half4 specSmoothness, inout half4 normal, inout half4 emission)
        {
#ifdef _BOUNDING_BOX_ENABLED
            clip(IN.worldPos - _ViewMin);
            clip(_ViewMax - IN.worldPos);
#endif

            diffuse *= o.Alpha * _ContributionAlbedo; 
            specSmoothness *= o.Alpha * _ContributionSpecSmoothness; 
            normal *= o.Alpha * _ContributionNormal; 
            emission *= o.Alpha * _ContributionEmission; 
        }
        ENDCG
    }
    FallBack "Diffuse"
}
