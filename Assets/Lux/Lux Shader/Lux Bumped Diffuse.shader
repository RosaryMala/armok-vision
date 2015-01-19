Shader "Lux/Bumped Diffuse" {

Properties {
	_Color ("Diffuse Color", Color) = (1,1,1,1)
	_MainTex ("Base (RGB) Alpha (A)", 2D) = "white" {}
	_BumpMap ("Normalmap", 2D) = "bump" {}
	_DiffCubeIBL ("Custom Diffuse Cube", Cube) = "black" {}
	[HideInInspector] _AO ("Ambient Occlusion Alpha (A)", 2D) = "white" {}
}

SubShader { 
	Tags { "RenderType"="Opaque" }
	LOD 400
	
	CGPROGRAM
	#pragma surface surf Lambert noambient
	#pragma glsl
	#pragma target 3.0
	// we use the default lighting functions
	
	#pragma multi_compile LUX_LINEAR LUX_GAMMA
	#pragma multi_compile DIFFCUBE_ON DIFFCUBE_OFF
	#pragma multi_compile LUX_AO_OFF LUX_AO_ON

	#define LUX_DIFFUSE

	float4 _Color;
	sampler2D _MainTex;
	sampler2D _BumpMap;
	#ifdef DIFFCUBE_ON
		samplerCUBE _DiffCubeIBL;
	#endif
	#ifdef LUX_AO_ON
		sampler2D _AO;
	#endif
	
	// Is set by script
	float4 ExposureIBL;

	struct Input {
		float2 uv_MainTex;
		float2 uv_BumpMap;
		#ifdef LUX_AO_ON
			float2 uv_AO;
		#endif
		float3 viewDir;
		float3 worldNormal;
		INTERNAL_DATA
	};

	void surf (Input IN, inout SurfaceOutput o) {
		fixed4 diff_albedo = tex2D(_MainTex, IN.uv_MainTex);
		// Diffuse Albedo
		o.Albedo = diff_albedo.rgb * _Color.rgb;
		o.Alpha = diff_albedo.a * _Color.a;
		o.Normal = UnpackNormal(tex2D(_BumpMap, IN.uv_BumpMap));
		
		#include "LuxCore/LuxLightingAmbient.cginc"
		
	}
ENDCG
}
FallBack "Diffuse"
CustomEditor "LuxMaterialInspector"
}
