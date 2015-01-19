Shader "Lux/Self-Illumin/Bumped Specular Metalness" {

Properties {
	_Color ("Diffuse Color", Color) = (1,1,1,1)
	_MainTex ("Base (RGB) Alpha (A)", 2D) = "white" {}
	_SpecTex ("Metalness (R) AO (G) Spec (B) Roughness (A)", 2D) = "black" {}
	_BumpMap ("Normalmap", 2D) = "bump" {}

	_Illum ("Illumin (RGB) Alpha (A)", 2D) = "black" {}
	_EmissionLM ("Emission (Lightmapper)", Float) = 1

	_DiffCubeIBL ("Custom Diffuse Cube", Cube) = "black" {}
	_SpecCubeIBL ("Custom Specular Cube", Cube) = "black" {}
	
	// _Shininess property is needed by the lightmapper - otherwise it throws errors
	[HideInInspector] _Shininess ("Shininess (only for Lightmapper)", Float) = 0.5
}

SubShader { 
	Tags { "RenderType"="Opaque" }
	LOD 400
	
	CGPROGRAM
	#pragma surface surf LuxDirect noambient
	#pragma glsl
	#pragma target 3.0

	// #pragma debug

	#pragma multi_compile LUX_LIGHTING_BP LUX_LIGHTING_CT
	#pragma multi_compile LUX_LINEAR LUX_GAMMA
	#pragma multi_compile DIFFCUBE_ON DIFFCUBE_OFF
	#pragma multi_compile SPECCUBE_ON SPECCUBE_OFF
	// AO is not needed here as it is stored in _SpecTex
	#define LUX_METALNESS


//	#define LUX_LIGHTING_CT
//	#define LUX_LINEAR
//	#define DIFFCUBE_ON
//	#define SPECCUBE_ON
	
	// include should be called after all defines
	#include "../LuxCore/LuxLightingDirect.cginc"

	float4 _Color;
	sampler2D _MainTex;
	sampler2D _SpecTex;
	sampler2D _BumpMap;
	sampler2D _Illum;
	float _EmissionLM;
	#ifdef DIFFCUBE_ON
		samplerCUBE _DiffCubeIBL;
	#endif
	#ifdef SPECCUBE_ON
		samplerCUBE _SpecCubeIBL;
	#endif
	
	// Is set by script
	float4 ExposureIBL;

	struct Input {
		float2 uv_MainTex;
		float2 uv_BumpMap;
		float3 viewDir;
		float3 worldNormal;
		float3 worldRefl;
		INTERNAL_DATA
	};


	void surf (Input IN, inout SurfaceOutputLux o) {
		fixed4 diff_albedo = tex2D(_MainTex, IN.uv_MainTex);
		// Metal (R) AO (G) Spec (B) Roughness (A)
		fixed4 spec_albedo = tex2D(_SpecTex, IN.uv_MainTex);
		fixed4 illumination = tex2D(_Illum, IN.uv_MainTex);
	
	//	Diffuse Albedo
		// We have to "darken" diffuse albedo by metalness as it controls ambient diffuse lighting
		o.Albedo = diff_albedo.rgb * _Color.rgb * (1.0 - spec_albedo.r);
		
		o.Alpha = diff_albedo.a * _Color.a;
		o.Normal = UnpackNormal(tex2D(_BumpMap, IN.uv_BumpMap));
	
	//	Specular Color
		// Lerp between specular color (defined as shades of gray for dielectric parts in the blue channel )
		// and the diffuse albedo color based on "Metalness"
		o.SpecularColor = lerp(spec_albedo.bbb, diff_albedo.rgb, spec_albedo.r);
		
		// Roughness – gamma for BlinnPhong / linear for CookTorrence
		o.Specular = LuxAdjustSpecular(spec_albedo.a);
		
		#include "../LuxCore/LuxLightingAmbient.cginc"

		o.Emission += illumination.rgb * _EmissionLM * illumination.a;
	}
ENDCG
}
FallBack "Specular"
CustomEditor "LuxMaterialInspector"
}
