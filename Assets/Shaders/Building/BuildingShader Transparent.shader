Shader "Building/Transparent" {
    Properties {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}

        //_Cutoff("Alpha Cutoff", Range(0.0, 1.0)) = 0.5

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

        [PerRendererData] _MatColor("DF Material Color", Color) = (0.5,0.5,0.5,1)
        [PerRendererData] _MatIndex("DF Material Array Index", int) = 0

        // Blending state
        [HideInInspector] _Mode("__mode", Float) = 0.0

        _SpecColor("Standard Specular Color", Color) = (0.220916301, 0.220916301, 0.220916301, 0.779083699)
    }
    SubShader{
        Tags { "Queue" = "Transparent" "RenderType" = "Opaque" }
        LOD 200

        Cull Front
        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf StandardSpecular alpha vertex:vert

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 4.0

        #pragma shader_feature _NORMALMAP
        #pragma shader_feature _TEXTURE_MASK
        #pragma shader_feature _SECOND_UV
        #pragma shader_feature _EMISSION
        #pragma shader_feature _METALLICGLOSSMAP
#pragma multi_compile _ _BOUNDING_BOX_ENABLED

#include "buildingInputs.cginc"

#include "CustomMetallic.cginc"

        // Flip normal for back faces
        void vert (inout appdata_full v) 
        {
            v.normal *= -1;
        }
        void surf (Input IN, inout SurfaceOutputStandardSpecular o)
        {
#include "BuildingSurf.cginc"
#include "BuildingSpecularValues.cginc"
        }
        ENDCG

        Cull Back
        // extra pass that renders to depth buffer only
        Pass 
        {
            ZWrite On
            ColorMask 0
        }
        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf StandardSpecular alpha

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 4.0

        #pragma shader_feature _NORMALMAP
        #pragma shader_feature _TEXTURE_MASK
        #pragma shader_feature _SECOND_UV
        #pragma shader_feature _EMISSION
        #pragma shader_feature _METALLICGLOSSMAP
#pragma multi_compile _ _BOUNDING_BOX_ENABLED

#include "buildingInputs.cginc"

#include "CustomMetallic.cginc"

        void surf (Input IN, inout SurfaceOutputStandardSpecular o)
        {
#include "BuildingSurf.cginc"
#include "BuildingSpecularValues.cginc"
        }
        ENDCG
    }
    FallBack "Unlit/BuildingShadowFallback"
    CustomEditor "BuildingMaterialEditor"

}
