Shader "Lux/Wetness/Simple Bumped Specular" {

Properties {
	_Color ("Diffuse Color", Color) = (1,1,1,1)
	_MainTex ("Base (RGB) Alpha (A)", 2D) = "white" {}
	_SpecTex ("Specular Color (RGB) Roughness (A)", 2D) = "black" {}
	_BumpMap ("Normalmap", 2D) = "bump" {}
	_DiffCubeIBL ("Custom Diffuse Cube", Cube) = "black" {}
	_SpecCubeIBL ("Custom Specular Cube", Cube) = "black" {}

	// Special Properties needed by all wetness shaders
	_WetnessWorldNormalDamp ("Wetness WorldNormal Damp", Range(0,1)) = 0.5
	
	// _Shininess property is needed by the lightmapper - otherwise it throws errors
	[HideInInspector] _Shininess ("Shininess (only for Lightmapper)", Float) = 0.5
	[HideInInspector] _AO ("Ambient Occlusion Alpha (A)", 2D) = "white" {}
}

SubShader { 
	Tags { "RenderType"="Opaque" }
	LOD 400
	
	CGPROGRAM
	#pragma surface surf LuxDirect fullforwardshadows noambient
	#pragma glsl
	#pragma target 3.0


	// #pragma debug

	#pragma multi_compile LUX_LIGHTING_BP LUX_LIGHTING_CT
	#pragma multi_compile LUX_LINEAR LUX_GAMMA
	#pragma multi_compile DIFFCUBE_ON DIFFCUBE_OFF
	#pragma multi_compile SPECCUBE_ON SPECCUBE_OFF
	#pragma multi_compile LUX_AO_OFF LUX_AO_ON

	//#define LUX_LIGHTING_BP
	//#define LUX_LINEAR
	//#define DIFFCUBE_ON
	//#define SPECCUBE_ON
	//#define LUX_AO_ON

//	///////////////////////////////////////
//	Config Wetness
	#define WetnessMaskInputVertexColors // PuddleMask is stored in vertex.color.g / Dry/wet ist stored in vertex.color.b
    
    #ifdef WetnessMaskInputVertexColors
        #define PuddleMask IN.color.g
        #define WetMask IN.color.b
    #else
        // If You do not want to use vertex colors to define wet vs. dry or puddles you could use a ture instead:
        // Do not forget that you have to declare the texture using sampler2D and sample it in the surface function
        // #define PuddleMask _MyWetnessTexture.g
        // #define WetMask _MyWetnessTexture.a
    #endif	
	
	// include should be called after all defines
	#include "../LuxCore/LuxLightingDirect.cginc"

	// include wetness specific functions and properties
	#include "../LuxCore/Wetness/LuxWetness.cginc"

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
	
	// Is set by script
	float4 ExposureIBL;

	struct Input {
		float2 uv_MainTex; 		// We can not use uv_MainTex and uv_BumpMap here as we might run out of Texture Interpolators
		#ifdef LUX_AO_ON
			float2 uv_AO;
		#endif
		float3 viewDir;
		float3 worldNormal;
		float3 worldRefl;
		#ifdef WetnessMaskInputVertexColors
            fixed4 color : COLOR;
        #endif
		INTERNAL_DATA
	};
	
	void surf (Input IN, inout SurfaceOutputLux o) {

	//	//////////////////
    //	Standard functions
    	// Sample the Base Textures
		fixed4 diff_albedo = tex2D(_MainTex, IN.uv_MainTex);
		fixed4 spec_albedo = tex2D(_SpecTex, IN.uv_MainTex);
		// Diffuse Albedo
		o.Albedo = diff_albedo.rgb * _Color.rgb;
		o.Alpha = diff_albedo.a * _Color.a;
		o.Normal = UnpackNormal(tex2D(_BumpMap, IN.uv_MainTex));
		// Specular Color
		o.SpecularColor = spec_albedo.rgb;
		// Roughness
		o.Specular = spec_albedo.a;

	//	//////////////////
	//	Wetness specific

		if (_Lux_WaterFloodlevel.x > 0  && WetMask > 0 ) {
			// Calculate worldNormal of Face
	        // float3 worldNormalFace = WorldNormalVector(IN, float3(0,0,1));
	        
	        // Calculate worldNormal of Pixel
			float3 worldNormalFace = WorldNormalVector(IN, o.Normal);

	        // Damp overall WaterAccumulation according to the worldNormal.y Component
    		float worldNormalDamp = saturate( saturate(worldNormalFace.y) + _WetnessWorldNormalDamp); 
	        // Claculate Wetness / wetFactor.x = overall Wetness Factor / wetFactor.y = special Wetness Factor for Raindrops
	    	float wetFactor = _Lux_WaterFloodlevel.x * worldNormalDamp.xx * WetMask;

			// Calling "o.Specular = WaterBRDF()" will tweak o.Albedo, o.Specular and o.SpecularColor according to the overall wetness (wetFactor.x)
			o.Specular = WaterBRDF(o.Albedo, o.Specular, o.SpecularColor, wetFactor);
			// Finally tweak o.Normal based on the overall Wetness Factor
			o.Normal = normalize (lerp(o.Normal, fixed3(0,0,1), wetFactor));

		}

		// Roughness – gamma for BlinnPhong / linear for CookTorrence
		o.Specular = LuxAdjustSpecular(o.Specular);

		#include "../LuxCore/LuxLightingAmbient.cginc"
	}
ENDCG
}
FallBack "Specular"
CustomEditor "LuxMaterialInspector"
}
