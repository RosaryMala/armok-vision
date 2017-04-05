Shader "Custom/BuildingShader" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_Glossiness ("Smoothness", Range(0,1)) = 0.5
		_Metallic ("Metallic", Range(0,1)) = 0.0
        _BumpMap("Normalmap ", 2D) = "bump" {}
        _DFMat("DF Material Splat", 2D) = "white" {}
        _MatTex("DF Material Texture", 2DArray) = "grey" {}
        [PerRendererData] _MatColor("DF Material Color", Color) = (0.5,0.5,0.5,1)
        [PerRendererData] _MatIndex("DF Material Array Index", int) = 0
    }
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Standard fullforwardshadows

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 4.0

        sampler2D _MainTex;
        sampler2D _BumpMap;
        sampler2D _DFMat;
        UNITY_DECLARE_TEX2DARRAY(_MatTex);

		struct Input {
            float2 uv_MainTex;
            float2 uv_BumpMap;
            float2 uv_DFMat;
        };

		half _Glossiness;
		half _Metallic;
		fixed4 _Color;
        UNITY_INSTANCING_CBUFFER_START(MyProperties)
        UNITY_DEFINE_INSTANCED_PROP(fixed4, _MatColor)
        UNITY_DEFINE_INSTANCED_PROP(int, _MatIndex)
        UNITY_INSTANCING_CBUFFER_END

#include "blend.cginc"

		void surf (Input IN, inout SurfaceOutputStandard o) {
			// Albedo comes from a texture tinted by color
			fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
            fixed4 dfTex = UNITY_SAMPLE_TEX2DARRAY(_MatTex, float3(IN.uv_DFMat.xy, UNITY_ACCESS_INSTANCED_PROP(_MatIndex)));
            fixed3 df = overlay(dfTex.rgb, UNITY_ACCESS_INSTANCED_PROP(_MatColor).rgb);
            fixed4 splat = tex2D(_DFMat, IN.uv_DFMat);
            o.Albedo = lerp(c.rgb, df.rgb, splat.r);
			// Metallic and smoothness come from slider variables
            o.Metallic = lerp(_Metallic, 1 - UNITY_ACCESS_INSTANCED_PROP(_MatColor).a, splat.r);
			o.Smoothness = lerp(_Glossiness, dfTex.a, splat.r);
			o.Alpha = c.a;
            o.Normal = UnpackNormal(tex2D(_BumpMap, IN.uv_BumpMap));
        }
		ENDCG
	}
	FallBack "Diffuse"
}
