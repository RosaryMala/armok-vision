Shader "Lux/Box projected/Self-Illumin Bumped Specular Boxprojected" {

Properties {
	_Color ("Diffuse Color", Color) = (1,1,1,1)
	_MainTex ("Base (RGB) Illumin Mask (A)", 2D) = "white" {}
	_SpecTex ("Specular Color (RGB) Roughness (A)", 2D) = "black" {}
	_BumpMap ("Normalmap", 2D) = "bump" {}

	_EmissionLM ("Emission (Lightmapper)", Float) = 1

	_DiffCubeIBL ("Custom Diffuse Cube", Cube) = "black" {}
	_SpecCubeIBL ("Custom Specular Cube", Cube) = "black" {}

	//_CubemapPositionWS ("Cube Position (Worldspace)", Vector) = (1,1,1,0)
	_CubemapSize ("Cube Size", Vector) = (1,1,1,0)
	
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

//	Does not make sense here...
//	#pragma multi_compile SPECCUBE_ON SPECCUBE_OFF

	#pragma multi_compile LUX_AO_OFF LUX_AO_ON

//	#define LUX_LIGHTING_BP
//	#define LUX_LINEAR
//	#define DIFFCUBE_ON

//	We should alway define SPECCUBE_ON
	#define SPECCUBE_ON

//	Activate Box Projection in LuxLightingAmbient
	#define LUX_BOXPROJECTION


	// include should be called after all defines
	#include "../LuxCore/LuxLightingDirect.cginc"

	float4 _Color;
	sampler2D _MainTex;
	sampler2D _SpecTex;
	sampler2D _BumpMap;
	float _EmissionLM;
	#ifdef DIFFCUBE_ON
		samplerCUBE _DiffCubeIBL;
	#endif
	samplerCUBE _SpecCubeIBL;
	#ifdef LUX_AO_ON
		sampler2D _AO;
	#endif

//	Needed by Box Projection
	//float3 _CubemapPositionWS;
	float3 _CubemapSize;
	float4x4 _CubeMatrix_Trans;
	float4x4 _CubeMatrix_Inv;
	
	// Is set by script
	float4 ExposureIBL;

	struct Input {
		float2 uv_MainTex;
		// float2 uv_BumpMap; // Bump and Main Tex have to share the same texcoords to safe texture interpolators
		#ifdef LUX_AO_ON
			float2 uv_AO;
		#endif
		float3 viewDir;
		float3 worldNormal;
		float3 worldRefl;
		// needed by Box Projection
		float3 worldPos;
		INTERNAL_DATA
	};

	void surf (Input IN, inout SurfaceOutputLux o) {
		fixed4 diff_albedo = tex2D(_MainTex, IN.uv_MainTex);
		fixed4 spec_albedo = tex2D(_SpecTex, IN.uv_MainTex);
		// Diffuse Albedo
		o.Albedo = diff_albedo.rgb * _Color.rgb;
		o.Alpha = _Color.a;
		// Bump and Main Tex have to share the same texcoords to safe texture interpolators
		o.Normal = UnpackNormal(tex2D(_BumpMap, IN.uv_MainTex));
		// Specular Color
		o.SpecularColor = spec_albedo.rgb;
		// Roughness – gamma for BlinnPhong / linear for CookTorrence
		o.Specular = LuxAdjustSpecular(spec_albedo.a);
		
		#include "../LuxCore/LuxLightingAmbient.cginc"

		o.Emission += diff_albedo.rgb * diff_albedo.a  * _EmissionLM;
	}
ENDCG
}
FallBack "Specular"
CustomEditor "LuxMaterialInspector"
}
