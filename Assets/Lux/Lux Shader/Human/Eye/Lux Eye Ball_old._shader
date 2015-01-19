Shader "Lux/Human/Eye Ball" {
	Properties {
		_MainTex ("Base (RGB) Heightmap (A)", 2D) = "white" {}
		_SpecTex ("Specular Color (RGB) Roughness (A)", 2D) = "black" {}
		_BumpMap ("Normalmap", 2D) = "bump" {}
		_DiffCubeIBL ("Custom Diffuse Cube", Cube) = "black" {}
		_SpecCubeIBL ("Custom Specular Cube", Cube) = "black" {}

		_PupilSize ("PupilSize", Range (0.01, 1)) = 0.5
		_Parallax ("Height", Range (0.005, 0.08)) = 0.08

		// _Shininess property is needed by the lightmapper - otherwise it throws errors
		[HideInInspector] _Shininess ("Shininess (only for Lightmapper)", Float) = 0.5

	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200

		CGPROGRAM
		#pragma surface surf LuxDirect noambient
		#pragma glsl
		#pragma target 3.0

		#pragma multi_compile LUX_LIGHTING_BP LUX_LIGHTING_CT
		#pragma multi_compile LUX_LINEAR LUX_GAMMA
		#pragma multi_compile DIFFCUBE_ON DIFFCUBE_OFF
		#pragma multi_compile SPECCUBE_ON SPECCUBE_OFF

//		#define LUX_LIGHTING_BP
//		#define LUX_LINEAR
//		#define DIFFCUBE_ON
//		#define SPECCUBE_ON

		// We cant use AO here as the eye ball tends to move...
		#define LUX_AO_OFF

		// include should be called after all defines
		#include "../../LuxCore/LuxLightingDirect.cginc"

		sampler2D _MainTex;
		sampler2D _SpecTex;
		sampler2D _BumpMap;
		#ifdef DIFFCUBE_ON
			samplerCUBE _DiffCubeIBL;
		#endif
		#ifdef SPECCUBE_ON
			samplerCUBE _SpecCubeIBL;
		#endif

		// shader specific inputs
		float _PupilSize;
		float _Parallax;

		// Is set by script
		float4 ExposureIBL;

		struct Input {
			float2 uv_MainTex;
			float3 viewDir;
			float3 worldNormal;
			float3 worldRefl;
			INTERNAL_DATA
		};

		void surf (Input IN, inout SurfaceOutputLux o) {

		//	Calculate Parallax
			half h = tex2D (_MainTex, IN.uv_MainTex).a;
			float2 offset = ParallaxOffset (h, _Parallax, IN.viewDir);

		//	Apply Pupil Size
			float2 UVs = IN.uv_MainTex;
			float2 delta = float2(0.5, 0.5) - UVs;
			// Calculate pow(distance,2) to center (pythagoras...)
			float factor = (delta.x*delta.x + delta.y*delta.y); 
			// Clamp it in order to mask our pixels, then bring it back into 0 - 1 range
			// Max distance = 0.15 --> pow(max,2) = 0.0225
			factor = saturate(0.0225 - factor) * 44.444;
			UVs += delta * factor * _PupilSize;

		//	Now sample albedo and spec maps according to the pupil’s size and parallax
			o.Albedo = tex2D(_MainTex, UVs + offset).rgb;
			o.Alpha = 1;
			fixed4 spec_albedo = tex2D(_SpecTex, UVs + offset);
		//	Normal map
			o.Normal = UnpackNormal(tex2D(_BumpMap, IN.uv_MainTex - offset));
		//	Specular Color
			o.SpecularColor = spec_albedo.rgb;
		//	Roughness – gamma for BlinnPhong / linear for CookTorrence
			o.Specular = LuxAdjustSpecular(spec_albedo.a);

			#include "../../LuxCore/LuxLightingAmbient.cginc"

		}
		ENDCG
	} 
	FallBack "Diffuse"
	CustomEditor "LuxMaterialInspector"
}