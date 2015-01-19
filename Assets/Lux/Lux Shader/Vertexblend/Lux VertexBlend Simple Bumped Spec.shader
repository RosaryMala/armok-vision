Shader "Lux/VertexBlend/Simple Bumped Specular" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_SpecTex ("Specular Color (RGB) Roughness (A)", 2D) = "black" {}
		_BumpMap ("Normalmap", 2D) = "bump" {}

		_MainTex1 ("Base Tex1(RGB)", 2D) = "white" {}
		_SpecTex1 ("Specular Color Tex1 (RGB) Roughness (A)", 2D) = "black" {}
		_BumpMap1 ("Normalmap Tex1", 2D) = "bump" {}

		_Tex1TilingX ("Tex1 Tiling Factor X", Float) = 1.0
		_Tex1TilingY ("Tex1 Tiling Factor Y", Float) = 1.0

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

	//	#define LUX_LIGHTING_CT
	//	#define LUX_LINEAR
	//	#define DIFFCUBE_ON
	//	#define SPECCUBE_ON
	//	#define LUX_AO_OFF

		// include should be called after all defines
		#include "../LuxCore/LuxLightingDirect.cginc"

		sampler2D _MainTex;
		sampler2D _SpecTex;
		sampler2D _BumpMap;
		sampler2D _MainTex1;
		sampler2D _SpecTex1;
		sampler2D _BumpMap1;

		float _Tex1TilingX, _Tex1TilingY;


		#ifdef DIFFCUBE_ON
		samplerCUBE _DiffCubeIBL;
		#endif
		#ifdef SPECCUBE_ON
			samplerCUBE _SpecCubeIBL;
		#endif
		#ifdef LUX_AO_ON
			sampler2D _AO;
		#endif
		
		// Is set by script
		float4 ExposureIBL;


		struct Input {
			float2 uv_MainTex;

			fixed4 color : COLOR;
			#ifdef LUX_AO_ON
				float2 uv_AO;
			#endif
			float3 viewDir;
			float3 worldNormal;
			float3 worldRefl;
			INTERNAL_DATA
		};

		void surf (Input IN, inout SurfaceOutputLux o) {
			// sample all textures
			float2 uv_tex1 = IN.uv_MainTex * float2(_Tex1TilingX, _Tex1TilingY);
			half4 base = tex2D (_MainTex, IN.uv_MainTex);
			half4 base1 = tex2D (_MainTex1, uv_tex1);
			fixed4 spec_albedo = tex2D(_SpecTex, IN.uv_MainTex);
			fixed4 spec_albedo1 = tex2D(_SpecTex1, uv_tex1) ;
			fixed4 normal = tex2D(_BumpMap, IN.uv_MainTex);
			fixed4 normal1 = tex2D(_BumpMap1, uv_tex1);

			// calculate blendvalue
			fixed2 blendval = fixed2(1.0-IN.color.r, IN.color.r);
			// combine Albedo
			o.Albedo = base.rgb*blendval.x + base1.rgb*blendval.y;
			// combine normal
			o.Normal = UnpackNormal(normal*blendval.x + normal1*blendval.y);
			// combine Specular Color
			o.SpecularColor = spec_albedo.rgb*blendval.x + spec_albedo1.rgb*blendval.y;
			// combine Roughness – gamma for BlinnPhong / linear for CookTorrence
			o.Specular = LuxAdjustSpecular(spec_albedo.a*blendval.x + spec_albedo1.a*blendval.y);
			// combine alpha
			o.Alpha = base.a*blendval.x + base1.a*blendval.y;

			#include "../LuxCore/LuxLightingAmbient.cginc"

		}
		ENDCG
	} 
	FallBack "Diffuse"
	CustomEditor "LuxMaterialInspector"
}
