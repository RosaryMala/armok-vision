// Upgrade NOTE: upgraded instancing buffer 'Props' to new syntax.

Shader "Instanced/Items" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Cutoff("Alpha cutoff", Range(0,1)) = 0.5
    }
	SubShader {
		Tags { "Queue" = "AlphaTest" "IgnoreProjector" = "True" "RenderType" = "TransparentCutout" }
		LOD 200
		
		CGPROGRAM

		// Physically based Standard lighting model, and enable shadows on all light types
		// And generate the shadow pass with instancing support
		#pragma surface surf Standard fullforwardshadows addshadow alphatest:_Cutoff

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		// Enable instancing for this shader
		#pragma multi_compile_instancing

		// Config maxcount. See manual page.
		// #pragma instancing_options

		sampler2D _MainTex;

		struct Input {
			float2 uv_MainTex;
		};

		half _Glossiness;
		half _Metallic;

		// Declare instanced properties inside a cbuffer.
		// Each instanced property is an array of by default 500(D3D)/128(GL) elements. Since D3D and GL imposes a certain limitation
		// of 64KB and 16KB respectively on the size of a cubffer, the default array size thus allows two matrix arrays in one cbuffer.
		// Use maxcount option on #pragma instancing_options directive to specify array size other than default (divided by 4 when used
		// for GL).
		UNITY_INSTANCING_BUFFER_START(Props)
			UNITY_DEFINE_INSTANCED_PROP(fixed4, _Color)	// Make _Color an instanced property (i.e. an array)
#define _Color_arr Props
		UNITY_INSTANCING_BUFFER_END(Props)

		void surf (Input IN, inout SurfaceOutputStandard o) {
			// Albedo comes from a texture tinted by color
            fixed4 tex = tex2D(_MainTex, IN.uv_MainTex);
            fixed4 col = UNITY_ACCESS_INSTANCED_PROP(_Color_arr, _Color);
            o.Albedo = tex.rgb < 0.5 ? (2.0 * tex.rgb * col.rgb) : (1.0 - 2.0 * (1.0 - tex.rgb) * (1.0 - col.rgb));
			// Metallic and smoothness come from slider variables
			o.Metallic = (1.0 - col.a);
			o.Smoothness = _Glossiness;
			o.Alpha = tex.a;
		}
		ENDCG
	}
	FallBack "Diffuse"
}
