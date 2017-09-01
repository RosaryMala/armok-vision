Shader "Custom/MaterialStore" {
    Properties{
        [HDR]_Color("Color", Color) = (0.5,0.5,0.5,1)
		_MainTex ("Albedo (RGB)", 2D) = "grey" {}
        _Specular("specular (R)", 2D) = "grey" {}
        _Normal("normal", 2D) = "bump" {}
        _Occlusion("occlusion", 2D) = "white" {}
        _Height("height/alpha", 2D) = "white" {}
        _HeightPreview("height preview cutoff", Range(0,1)) = 0
    }
	SubShader {
        Tags{ "Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent" }
        LOD 200
		
		CGPROGRAM
		// Physically based Standard lighting model, and enable transparency
		#pragma surface surf Standard alpha addshadow

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

        sampler2D _MainTex;
        sampler2D _Specular;
        sampler2D _Normal;
        sampler2D _Occlusion;
        sampler2D _Height;

		struct Input {
			float2 uv_MainTex;
		};

		float4 _Color;
        float _HeightPreview;

		// Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
		// See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
		// #pragma instancing_options assumeuniformscaling
		UNITY_INSTANCING_CBUFFER_START(Props)
			// put more per-instance properties here
		UNITY_INSTANCING_CBUFFER_END

#include "blend.cginc"

		void surf (Input IN, inout SurfaceOutputStandard o) {
            clip(tex2D(_Height, IN.uv_MainTex).r - _HeightPreview);
			// Albedo comes from a texture tinted by color
            float4 c = overlay(tex2D (_MainTex, IN.uv_MainTex), _Color.rgb);
            float isHDR = step(max(_Color.r, max(_Color.g, _Color.b)),1.001);
            fixed4 specular = tex2D(_Specular, IN.uv_MainTex);
			o.Albedo = c.rgb * (1 - isHDR);
            o.Normal = UnpackNormal(tex2D(_Normal, IN.uv_MainTex));
			// Metallic and smoothness come from slider variables
            o.Metallic = max((_Color.a * 2) - 1, 0);
			o.Smoothness = specular.r;
            o.Occlusion = tex2D(_Occlusion, IN.uv_MainTex).r;
			o.Alpha = min((_Color.a * 2), 1);
            o.Emission = c.rgb * isHDR;
		}
		ENDCG
	}
	FallBack "Diffuse"
}
