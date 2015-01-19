Shader "Lux/Wetness/Puddles Bumped Specular" {

Properties {
	_Color ("Diffuse Color", Color) = (1,1,1,1)
	_MainTex ("Base (RGB) Alpha (A)", 2D) = "white" {}
	_SpecTex ("Specular Color (RGB) Roughness (A)", 2D) = "black" {}
	_BumpMap ("Normalmap", 2D) = "bump" {}
	
	// Special Properties needed by Heightmap based wetness shaders
	_HeightWetness("Heightmap(A) Puddle Noise(R)", 2D) = "white" {}
    _Parallax ("Height", Range (0.005, 0.08)) = 0.02

    // Special Properties needed by wetness shaders using tex2Dlod
    _TextureSize ("Texture Size", Float) = 1024
	_MipBias ("Mip Bias", Float) = 0.75

	// Special Properties needed by all wetness shaders
	_WetnessWorldNormalDamp ("Wetness WorldNormal Damp", Range(0,1)) = 0.5
	_WetnessHeightMapInfluence  ("Wetness HeightMap Influence", Range(0,1)) = 1

	// General Lux Properties
	_DiffCubeIBL ("Custom Diffuse Cube", Cube) = "black" {}
	_SpecCubeIBL ("Custom Specular Cube", Cube) = "black" {}

	// Properties needed if you used Boxprojection
	// _CubemapSize ("Cube Size", Vector) = (1,1,1,0)

	// _Shininess property is needed by the lightmapper - otherwise it throws errors
	[HideInInspector] _Shininess ("Shininess (only for Lightmapper)", Float) = 0.5
	[HideInInspector] _AO ("Ambient Occlusion Alpha (A)", 2D) = "white" {}
}

SubShader { 
	Tags { "RenderType"="Opaque" }
	LOD 400
	
	CGPROGRAM
	#pragma surface surf LuxDirect vertex:LuxVertWet fullforwardshadows noambient
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
//	#define LUX_AO_ON

//	///////////////////////////////////////
//	Config Wetness
	#define WetnessMaskInputVertexColors // PuddleMask is stored in vertex.color.g / comment this if you want to use a texture instead
    
    #ifdef WetnessMaskInputVertexColors
        #define PuddleMask IN.color.g
        #define WetMask IN.color.b
    #else
        // If You do not want to use vertex colors to define wet vs. dry or puddles you could use a texture instead:
        // Do not forget that you have to declare the texture using sampler2D and sample it in the surface function
        // In this example we take both values from the ambient occlusion texture
        #define PuddleMask ambientOcclusion.g
        #define WetMask ambientOcclusion.b
    #endif

    //	Use this define to enable water ripples (needed by LuxWetness.cginc )
    #define Lux_WaterRipples

//	///////////////////////////////////////	
	
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
	#if defined (LUX_AO_ON) || !defined (WetnessMaskInputVertexColors)
		sampler2D _AO;
	#endif
	
	// Is set by script
	float4 ExposureIBL;

	float _Parallax;

	float _TextureSize;
	float _MipBias;

//	Wetness specific Properties are defined in the include file


//	Nevertheless we have to slightly adjust the Input Structure
	struct Input {
		// We can not use uv_MainTex and uv_BumpMap here as we might run out of Texture Interpolators
		// We can not use uv_AO here as we might run out of Texture Interpolators
		// So we combine uv_MainTex and uv_AO into LuxUV_MainAOTex
		float4 LuxUV_MainAOTex;
		float3 viewDir;
		float3 worldNormal;
		float3 worldRefl;
		float3 worldPos;		// Needed by Wetness
		#ifdef WetnessMaskInputVertexColors
            fixed4 color : COLOR;
        #endif
		INTERNAL_DATA
	};

	void LuxVertWet (inout appdata_full v, out Input o) {
    	UNITY_INITIALIZE_OUTPUT(Input,o);
    	// We have to combine the texture coords
    	o.LuxUV_MainAOTex.xy = TRANSFORM_TEX(v.texcoord, _MainTex);
    	#if defined (LUX_AO_ON) || !defined (WetnessMaskInputVertexColors)
			o.LuxUV_MainAOTex.zw = TRANSFORM_TEX(v.texcoord1, _AO);
		#endif
	}
	
	void surf (Input IN, inout SurfaceOutputLux o) {

	//	Sample combined Height and Wetness Map first as we need both to calculate Wetness: A = height / R = Puddle Noise
        fixed4 HeightWetness = tex2D(_HeightWetness, IN.LuxUV_MainAOTex.xy);

    //	If we do not use vertex colors to control puddle distribution and overall wetness we have to sample the ao tex already here
        #if !defined (WetnessMaskInputVertexColors)
        	fixed4 ambientOcclusion = tex2D(_AO, IN.LuxUV_MainAOTex.zw);
        	#define LUX_AO_SAMPLED // Do not let LuxLightingAmbient.cginc sample AO twice 
        #endif

	//	Calculate miplevel (needed by tex2Dlod)
		// Calculate componentwise max derivatives
		float2 dx1 = ddx( IN.LuxUV_MainAOTex.xy * _TextureSize * _MipBias );
		float2 dy1 = ddy( IN.LuxUV_MainAOTex.xy * _TextureSize * _MipBias );
		float d = max( dot( dx1, dx1 ), dot( dy1, dy1 ) );
		float2 lambda = 0.5 * log2(d);

	//	//////////////////
	//	Wetness specific

		float3 rippleNormal = float3(0,0,1);
		float2 wetFactor = float2(0,0);

		if (_Lux_WaterFloodlevel.x + _Lux_WaterFloodlevel.y > 0 && WetMask > 0 ) {
	        // Calculate worldNormal of Face
	        float3 worldNormalFace = WorldNormalVector(IN, float3(0,0,1));
	        // Claculate Wetness / wetFactor.x = overall Wetness Factor / wetFactor.y = special Wetness Factor for Raindrops
	    	wetFactor = ComputeWaterAccumulation(PuddleMask, HeightWetness.ar, worldNormalFace.y ) * WetMask;
	    	// Add Water Ripples
	    	// Calc Ripple Distance
    		float fadeOutWaterBumps = saturate( ( _Lux_WaterBumpDistance - distance(_WorldSpaceCameraPos, IN.worldPos)) / 5);
    		// Limit Ripples to Distance
    		if (fadeOutWaterBumps > 0 && _Lux_RainIntensity > 0) {
	    		rippleNormal = AddWaterRipples(wetFactor, IN.worldPos, lambda, fadeOutWaterBumps);
	    	}
	    }

    //	Now we can apply the Parallax Extrusion
    	float2 offset = ParallaxOffset (HeightWetness.a, _Parallax, IN.viewDir);
    	// Add Height and Refraction
    	IN.LuxUV_MainAOTex.xy += offset + rippleNormal.xy;

    //	//////////////////
    //	Standard functions
    	// Sample the Base Textures
		fixed4 diff_albedo = tex2D(_MainTex, IN.LuxUV_MainAOTex.xy);
		fixed4 spec_albedo = tex2D(_SpecTex, IN.LuxUV_MainAOTex.xy);
		// Diffuse Albedo
		o.Albedo = diff_albedo.rgb * _Color.rgb;
		o.Alpha = diff_albedo.a * _Color.a;
		o.Normal = UnpackNormal(tex2D(_BumpMap, IN.LuxUV_MainAOTex.xy));
		// Specular Color
		o.SpecularColor = spec_albedo.rgb;
		// Roughness
		o.Specular = spec_albedo.a;
		
	//	//////////////////
	//	Wetness specific
		if (_Lux_WaterFloodlevel.x + _Lux_WaterFloodlevel.y > 0 && WetMask > 0 ) {
			// Calling "o.Specular = WaterBRDF()" will tweak o.Albedo, o.Specular and o.SpecularColor according to the overall wetness (wetFactor.x)
			o.Specular = WaterBRDF(o.Albedo, o.Specular, o.SpecularColor, wetFactor.x);
			// Finally tweak o.Normal based on the overall Wetness Factor
			o.Normal = lerp(o.Normal, rippleNormal, wetFactor.x );
		}
		// We have to remap the _AO uv coords before we can call the include
		#define uv_AO LuxUV_MainAOTex.zw
	//	//////////////////

		// Convert Roughness for BlinnPhong / CookTorrence as in other Lux shaders
		o.Specular = LuxAdjustSpecular(o.Specular);

		#include "../LuxCore/LuxLightingAmbient.cginc"
	}
ENDCG
}
FallBack "Specular"
CustomEditor "LuxMaterialInspector"
}

