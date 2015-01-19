Shader "Lux/Bumped Specular Detail" {

Properties {
	_Color ("Diffuse Color", Color) = (1,1,1,1)
	_MainTex ("Base (RGB) Alpha (A)", 2D) = "white" {}
	_SpecTex ("Specular Color (RGB) Roughness (A)", 2D) = "black" {}
	_BumpMap ("Normalmap", 2D) = "bump" {}

	_Detail ("Detail Diffuse(RGB) Roughness(A)", 2D) = "white" {}
	_DetailBumpMap ("Detail Normalmap", 2D) = "bump" {}
	_DetailTilingX ("Detail Tiling Factor X", Float) = 1.0
	_DetailTilingY ("Detail Tiling Factor Y", Float) = 1.0
	_DetailDiffuseStrength ("Detail Diffuse Strength", Range (0.0, 1.0)) = 1.0
	_DetailNormalStrength ("Detail Normal Strength", Range (0.0, 1.0)) = 1.0
	_OverallRoughness ("Overall Roughness ", Range (0.0, 1.0)) = 1.0


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
	#pragma surface surf LuxDirect noambient
	#pragma glsl
	#pragma target 3.0

	// #pragma debug
	
	#pragma multi_compile LUX_LIGHTING_BP LUX_LIGHTING_CT
	#pragma multi_compile LUX_LINEAR LUX_GAMMA
	#pragma multi_compile DIFFCUBE_ON DIFFCUBE_OFF
	#pragma multi_compile SPECCUBE_ON SPECCUBE_OFF
	#pragma multi_compile LUX_AO_OFF LUX_AO_ON

// #define LUX_LIGHTING_CT
// #define LUX_GAMMA
// #define DIFFCUBE_ON
// #define SPECCUBE_ON
// #define LUX_AO_ON

	// include should be called after all defines
	#include "../LuxCore/LuxLightingDirect.cginc"

	float4 _Color;
	sampler2D _MainTex;
	sampler2D _SpecTex;
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

	//Shader specific inputs
	sampler2D _Detail;
	sampler2D _DetailBumpMap;
	float _DetailTilingX, _DetailTilingY, _DetailDiffuseStrength, _DetailNormalStrength, _OverallRoughness;
	
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
		// Multiply by Detail Diffuse
		fixed4 diff_albedo_detail = tex2D(_Detail, IN.uv_MainTex * float2 (_DetailTilingX, _DetailTilingY));
		diff_albedo_detail = lerp(fixed4(1,1,1,0), diff_albedo_detail, _DetailDiffuseStrength);
		diff_albedo *= diff_albedo_detail;
		fixed4 spec_albedo = tex2D(_SpecTex, IN.uv_MainTex);
		// Diffuse Albedo
		o.Albedo = diff_albedo.rgb * _Color.rgb;
		o.Alpha = diff_albedo.a * _Color.a;

		// Sample and Combine Normals
		float3 normal1 = UnpackNormal(tex2D(_BumpMap, IN.uv_BumpMap));
		float3 normal2 = UnpackNormal(tex2D(_DetailBumpMap, IN.uv_MainTex * float2 (_DetailTilingX, _DetailTilingY)));
		// Using _DetailNormalStrength might not be a good idea for production
		// Simply adjust your DetailNormalMap instead an comment the following 2 lines:
		normal2.xy *= _DetailNormalStrength;
		normal2 = normalize (normal2);
		float3x3 nBasis = float3x3(
			float3 (normal1.z, normal1.y,-normal1.x),
			float3 (normal1.x, normal1.z,-normal1.y),
			float3 (normal1.x, normal1.y, normal1.z)
		);
		o.Normal = normalize ( normal2.x*nBasis[0] + normal2.y*nBasis[1] + normal2.z*nBasis[2] );
		
		// Specular Color
		o.SpecularColor = spec_albedo.rgb;
		// Roughness – gamma for BlinnPhong / linear for CookTorrence
		o.Specular = LuxAdjustSpecular(spec_albedo.a + diff_albedo_detail.a) * _OverallRoughness;
		
		#include "../LuxCore/LuxLightingAmbient.cginc"
	}
ENDCG
}
FallBack "Specular"
CustomEditor "LuxMaterialInspector"
}
