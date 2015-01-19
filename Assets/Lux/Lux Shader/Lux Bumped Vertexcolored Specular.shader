Shader "Lux/Bumped Vertex Colored Specular" {

Properties {
	_MainTex ("Base (RGB) Alpha (A)", 2D) = "white" {}
	_Shapetex ("Specular Roughness (R) Ambient Occlusion (B)", 2D) = "black" {}
	_BumpMap ("Normalmap", 2D) = "bump" {}
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
	#pragma multi_compile LUX_AO_OFF LUX_AO_ON

//	#define LUX_LIGHTING_BP
//	#define LUX_LINEAR
//	#define DIFFCUBE_ON
//	#define SPECCUBE_ON
//	#define LUX_AO_OFF
	
	// include should be called after all defines
	#include "LuxCore/LuxLightingDirect.cginc"

	float4 _Color;
	sampler2D _MainTex;
	sampler2D _Shapetex;
	sampler2D _BumpMap;
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
		float2 uv2_BumpMap ;
		float4 color: Color; //Vertex Color
		float3 viewDir;
		float3 worldNormal;
		float3 worldRefl;
		INTERNAL_DATA
	};
	
	void surf (Input IN, inout SurfaceOutputLux o) {
		fixed4 diff_albedo = tex2D(_MainTex, IN.uv_MainTex);
		fixed4 spec_albedo = tex2D(_Shapetex, IN.uv2_BumpMap);
		// Diffuse Albedo
		o.Albedo = diff_albedo.rgb * IN.color.rgb * IN.color.a;
		o.Alpha = diff_albedo.a;
		o.Normal = UnpackNormal(tex2D(_BumpMap, IN.uv2_BumpMap));
		// Specular Color
		o.SpecularColor = (diff_albedo.rgb * IN.color.rgb * (1.0 - IN.color.a)) + (0.3 * IN.color.a);
		// Roughness – gamma for BlinnPhong / linear for CookTorrence
		o.Specular = LuxAdjustSpecular(spec_albedo.r);

		#include "LuxCore/LuxLightingAmbientBumpVert.cginc"
	}
ENDCG
}
FallBack "Specular"
CustomEditor "LuxMaterialInspector"
}
