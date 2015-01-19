Shader "Lux/Bumped Specular Materialtest" {

Properties {
	_Color ("Diffuse Color", Color) = (1,1,1,1)
	_MainTex ("Base (RGB) Alpha (A)", 2D) = "white" {}
	_SpecTex ("Specular Color (RGB)", 2D) = "white" {}
	_SpecColMult ("Specular Color Multiplier", Color) = (1,1,1,1)
	_RoughTex ("Roughness (Grayscale in Alpha)", 2D) = "white" {}
	_RoughnessFactor ("Roughness Factor", Range(0.0,1.0)) = 1


	_BumpMap ("Normalmap", 2D) = "bump" {}
	_DiffCubeIBL ("Custom Diffuse Cube", Cube) = "black" {}
	_SpecCubeIBL ("Custom Specular Cube", Cube) = "black" {}
	
	// _Shininess property is needed by the lightmapper - otherwise it throws errors
	[HideInInspector] _Shininess ("Shininess (only for Lightmapper)", Float) = 0.5
	[HideInInspector] _AO ("Ambient Occlusion Alpha (A)", 2D) = "white" {}
}

SubShader { 
	Tags { "RenderType"="Opaque" }
	LOD 400
	
	CGPROGRAM
	#pragma surface surf LuxDirect noambient fullforwardshadows
	#pragma glsl
	#pragma target 3.0

	// #pragma debug

	#pragma multi_compile LUX_LIGHTING_BP LUX_LIGHTING_CT
	#pragma multi_compile LUX_LINEAR LUX_GAMMA
	#pragma multi_compile DIFFCUBE_ON DIFFCUBE_OFF
	#pragma multi_compile SPECCUBE_ON SPECCUBE_OFF
	#pragma multi_compile LUX_AO_OFF LUX_AO_ON

//#define LUX_LIGHTING_CT
//#define LUX_LINEAR
//#define DIFFCUBE_ON
//#define SPECCUBE_ON
//#define LUX_AO_ON

	// include should be called after all defines
	#include "LuxCore/LuxLightingDirect.cginc"

	float4 _Color;
	sampler2D _MainTex;
	sampler2D _SpecTex;
	sampler2D _RoughTex;
	sampler2D _BumpMap;
	#ifdef DIFFCUBE_ON
		samplerCUBE _DiffCubeIBL;
	#endif
	#ifdef SPECCUBE_ON
		samplerCUBE _SpecCubeIBL;
	#endif
	#ifdef LUX_AO_ON
		sampler2D _AO;
	#endif

	 float _RoughnessFactor;
	 fixed4 _SpecColMult;
	
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
		float3 worldRefl;
		INTERNAL_DATA
	};
	
	void surf (Input IN, inout SurfaceOutputLux o) {
		fixed4 diff_albedo = tex2D(_MainTex, IN.uv_MainTex);
		fixed4 spec_albedo = tex2D(_SpecTex, IN.uv_MainTex);
		spec_albedo.rgb *= _SpecColMult;
		spec_albedo.a = tex2D(_RoughTex, IN.uv_MainTex).a * _RoughnessFactor;
		// Diffuse Albedo
		o.Albedo = diff_albedo.rgb * _Color.rgb;
		o.Alpha = diff_albedo.a * _Color.a;
		o.Normal = UnpackNormal(tex2D(_BumpMap, IN.uv_BumpMap));
		// Specular Color
		o.SpecularColor = spec_albedo.rgb;
		// Roughness – gamma for BlinnPhong / linear for CookTorrence
		o.Specular = LuxAdjustSpecular(spec_albedo.a);
		
		#include "LuxCore/LuxLightingAmbient.cginc"
	}
ENDCG
}
FallBack "Specular"
CustomEditor "LuxMaterialInspector"
}
