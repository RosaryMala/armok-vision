Shader "Custom/BuildingShader" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}

        _Cutoff("Alpha Cutoff", Range(0.0, 1.0)) = 0.5

        _Glossiness("Smoothness", Range(0.0, 1.0)) = 0.5
        _GlossMapScale("Smoothness Scale", Range(0.0, 1.0)) = 1.0
        [Enum(Metallic Alpha,0,Albedo Alpha,1)] _SmoothnessTextureChannel("Smoothness texture channel", Float) = 0
            
        [Gamma] _Metallic("Metallic", Range(0.0, 1.0)) = 0.0
        _MetallicGlossMap("Metallic", 2D) = "white" {}

        _BumpScale("Scale", Float) = 1.0
        _BumpMap("Normal Map", 2D) = "bump" {}

        _OcclusionStrength("Strength", Range(0.0, 1.0)) = 1.0
        _OcclusionMap("Occlusion", 2D) = "white" {}

        _EmissionColor("Color", Color) = (0,0,0)
        _EmissionMap("Emission", 2D) = "white" {}

        _DFMask("DF Material Splat", 2D) = "white" {}
  
        [Enum(UV0,0,UV1,1)] _UVSec("UV Set for secondary textures", Float) = 0

        _MatTex("DF Material Texture", 2DArray) = "grey" {}
        [PerRendererData] _MatColor("DF Material Color", Color) = (0.5,0.5,0.5,1)
        [PerRendererData] _MatIndex("DF Material Array Index", int) = 0

        // Blending state
        [HideInInspector] _Mode("__mode", Float) = 0.0
        [HideInInspector] _SrcBlend("__src", Float) = 1.0
        [HideInInspector] _DstBlend("__dst", Float) = 0.0
        [HideInInspector] _ZWrite("__zw", Float) = 1.0
    }
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Standard fullforwardshadows

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 4.0

        #pragma shader_feature _NORMALMAP
        #pragma shader_feature _TEXTURE_MASK

        half4       _Color;
        half        _Cutoff;

        sampler2D   _MainTex;

        sampler2D   _BumpMap;
        half        _BumpScale;

        sampler2D _DFMask;
        UNITY_DECLARE_TEX2DARRAY(_MatTex);

		struct Input {
            float2 uv_MainTex;
            float2 uv_BumpMap;
            float2 uv_DFMask;
        };

		half _Glossiness;
		half _Metallic;
        UNITY_INSTANCING_CBUFFER_START(MyProperties)
        UNITY_DEFINE_INSTANCED_PROP(fixed4, _MatColor)
        UNITY_DEFINE_INSTANCED_PROP(int, _MatIndex)
        UNITY_INSTANCING_CBUFFER_END

#include "blend.cginc"

		void surf (Input IN, inout SurfaceOutputStandard o) {
			// Albedo comes from a texture tinted by color
			fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
            fixed4 dfTex = UNITY_SAMPLE_TEX2DARRAY(_MatTex, float3(IN.uv_DFMask.xy, UNITY_ACCESS_INSTANCED_PROP(_MatIndex)));
            fixed3 df = overlay(dfTex.rgb, UNITY_ACCESS_INSTANCED_PROP(_MatColor).rgb);
            fixed4 splat = tex2D(_DFMask, IN.uv_DFMask);
            o.Albedo = lerp(c.rgb, df.rgb, splat.r);
			// Metallic and smoothness come from slider variables
            o.Metallic = lerp(_Metallic, 1 - UNITY_ACCESS_INSTANCED_PROP(_MatColor).a, splat.r);
			o.Smoothness = lerp(_Glossiness, dfTex.a, splat.r);
			o.Alpha = c.a;

#ifdef _NORMALMAP
            o.Normal = UnpackScaleNormal(tex2D(_BumpMap, IN.uv_BumpMap), _BumpScale);
#endif
        }
		ENDCG
	}
	FallBack "Diffuse"
    CustomEditor "BuildingMaterialEditor"

}
