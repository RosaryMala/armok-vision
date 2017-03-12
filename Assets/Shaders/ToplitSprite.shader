// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'

Shader "Custom/ToplitSprite" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
        _SpriteArray("Sprite Array", 2DArray) = "white" {}
        _NormalArray("Normal Array", 2DArray) = "bump" {}
        _SpriteIndex("Sprite Index", Int) = 1
		_Glossiness ("Smoothness", Range(0,1)) = 0.5
		_Metallic ("Metallic", Range(0,1)) = 0.0

		_Cutoff("Alpha cutoff", Range(0,1)) = 0.5
	}
	SubShader {
		Tags { "RenderType"="TransparentCutout" }
		LOD 200
		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Standard addshadow alphatest:_Cutoff

        #pragma target 3.5

        UNITY_DECLARE_TEX2DARRAY(_SpriteArray);
        UNITY_DECLARE_TEX2DARRAY(_NormalArray);

		struct Input {
			float2 uv_MainTex;
		};

		half _Glossiness;
		half _Metallic;
		fixed4 _Color;
        int _SpriteIndex;

		void surf (Input IN, inout SurfaceOutputStandard o) {
			// Albedo comes from a texture tinted by color
            fixed4 c = _Color * UNITY_SAMPLE_TEX2DARRAY(_SpriteArray, float3(IN.uv_MainTex, _SpriteIndex));
            o.Albedo = c.rgb;
			// Metallic and smoothness come from slider variables
			o.Metallic = _Metallic;
			o.Smoothness = _Glossiness;
            o.Normal = UnpackNormal(UNITY_SAMPLE_TEX2DARRAY(_NormalArray, float3(IN.uv_MainTex, _SpriteIndex)));
			o.Alpha = c.a;
		}
		ENDCG
	} 
	FallBack "Diffuse"
}
