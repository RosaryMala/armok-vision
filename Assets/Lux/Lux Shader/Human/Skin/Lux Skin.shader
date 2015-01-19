// //////////////////////////
// original code taken from: http://www.farfarer.com/blog/2013/02/11/pre-integrated-skin-shader-unity-3d/


Shader "Lux/Human/Skin" {

Properties {
	_MainTex ("Base (RGB) Alpha (A)", 2D) = "white" {}
	_SpecTex ("Specular (R) Roughness (G) SSS Mask (B), AO (A)", 2D) = "gray" {}
	_BumpMap ("Normalmap", 2D) = "bump" {}

	// BRDF Lookup texture, light direction on x and curvature on y.
	_BRDFTex ("BRDF Lookup (RGB)", 2D) = "gray" {}
	// Curvature scale. Multiplier for the curvature - best to keep this very low - between 0.02 and 0.002.
	_CurvatureScale ("Curvature Scale", Float) = 0.02
	// Which mip-map to use when calculating curvature. Best to keep this between 1 and 2.
	_BumpBias ("Normal Map Blur Bias", Float) = 1.5

	_DiffCubeIBL ("Custom Diffuse Cube", Cube) = "black" {}
	_SpecCubeIBL ("Custom Specular Cube", Cube) = "black" {}

	// _Shininess property is needed by the lightmapper - otherwise it throws errors
	[HideInInspector] _Shininess ("Shininess (only for Lightmapper)", Float) = 0.5
}

SubShader { 
	Tags { "Queue" = "Geometry" "RenderType" = "LuxOpaque" }
	LOD 400
//	Built in Fog breaks rendering using directX and only one pixel light
	Fog { Mode Off } 

//	Offset -1, -1 
//	Would be needed in deferred to get rid of z-fighting problems caused by:
//	#pragma glsl + real time shadows from directional lights
//	But using Offset -1, -1 produces some "pixel acne" on deltaWorldNormal
//	So we have to go with our own Depthpath: LuxOpaque.
//	See: LuxCore/Ressources/Camera-DepthTexture.shader / Camera-DepthNormalTexture.shader


	CGPROGRAM
	#pragma surface surf LuxSkin exclude_path:prepass fullforwardshadows nodirlightmap nolightmap noambient vertex:vert finalcolor:customFogExp2
//	#pragma glsl causes z-fighting issues but is needed... so we have to go the long way described above
	#pragma glsl
	#pragma target 3.0

//	#pragma multi_compile LUX_LIGHTING_BP LUX_LIGHTING_CT
	#pragma multi_compile LUX_LINEAR LUX_GAMMA
	#pragma multi_compile DIFFCUBE_ON DIFFCUBE_OFF
	#pragma multi_compile SPECCUBE_ON SPECCUBE_OFF

//	#pragma multi_compile LUX_AO_OFF LUX_AO_ON

//	CookTorrance does not look convincing so we skip it here
	#define LUX_LIGHTING_BP
//	#define LUX_LINEAR
//	#define DIFFCUBE_ON
//	#define SPECCUBE_ON

//	Important as we use alpha channel of _SpecTex here	
	#define LUX_AO_OFF

	// include should be called after all defines
	// we do not use the direct lighting functions but some other stuff, so let’s get it
	#include "../../LuxCore/LuxLightingDirect.cginc"

	sampler2D _MainTex;
	sampler2D _SpecTex;
	sampler2D _BumpMap;
	sampler2D _BRDFTex;
	#ifdef DIFFCUBE_ON
		samplerCUBE _DiffCubeIBL;
	#endif
	#ifdef SPECCUBE_ON
		samplerCUBE _SpecCubeIBL;
	#endif

	float _BumpBias;
	float _CurvatureScale;

	// Is set by script
	float4 ExposureIBL;

	struct SurfaceOutputLuxSkin {
		fixed3 Albedo;
		fixed3 Normal;
		fixed3 NormalBlur;
		fixed3 Emission;
		fixed Specular;
		fixed Alpha;
		float Curvature; 
		fixed SpecularColor; 
	};

	struct Input {
		float2 uv_MainTex;
		float3 viewDir;
		// needed by custom fog and curvature calc
		float4 worldPosLux;
		float3 worldNormal;
		float3 worldRefl;
		INTERNAL_DATA
	};
	
	// Overwrite SurfaceOutputLux by SurfaceOutputLuxSkin in customfog
	#define SurfaceOutputLux SurfaceOutputLuxSkin
	// Define LUX_CAMERADISTANCE as worldPosLux.w
	#define LUX_CAMERADISTANCE IN.worldPosLux.w
	#include "../../LuxCore/LuxCustomFog.cginc"
	
	void vert (inout appdata_full v, out Input o) 
	{
		UNITY_INITIALIZE_OUTPUT(Input,o);
		o.worldPosLux.xyz = mul(_Object2World, v.vertex).xyz;
		o.worldPosLux.w = length(mul(UNITY_MATRIX_MV, v.vertex).xyz);
	}

	void surf (Input IN, inout SurfaceOutputLuxSkin o) {
		fixed3 diff_albedo = tex2D(_MainTex, IN.uv_MainTex).rgb;
		// spec_albedo: r = specular color, g = roughness, b = sss, a = ao
		fixed4 spec_albedo = tex2D(_SpecTex, IN.uv_MainTex);
		// Diffuse Albedo
		o.Albedo = diff_albedo.rgb;
		o.Alpha = 1; //diff_albedo.a;
		o.Normal = UnpackNormal(tex2D(_BumpMap, IN.uv_MainTex));
		// Specular Color
		o.SpecularColor = spec_albedo.r;
		// Roughness – gamma for BlinnPhong / linear for CookTorrence
		o.Specular = LuxAdjustSpecular(spec_albedo.g);

	//	Calculate the curvature of the model dynamically
		o.NormalBlur = UnpackNormal( tex2Dlod ( _BumpMap, float4 ( IN.uv_MainTex, 0.0, _BumpBias ) ) );
		// Transform it back into a world normal so we can get good derivatives from it.
		float3 blurredWorldNormal = WorldNormalVector( IN, o.NormalBlur );
		// Get the scale of the derivatives of the blurred world normal and the world position.
		float deltaWorldNormal = length( fwidth( blurredWorldNormal ) );
		float deltaWorldPosition = length( fwidth ( IN.worldPosLux.xyz ) );		
		o.Curvature = (deltaWorldNormal / deltaWorldPosition) * _CurvatureScale * spec_albedo.b;

	//	In order to soften the diffuse ambient lighting we can define USE_BLURREDNORMAL
		#define USE_BLURREDNORMAL
		#include "../../LuxCore/LuxLightingAmbient.cginc"

	//	Add AO
		o.Emission *= spec_albedo.a;

	}


	inline fixed4 LightingLuxSkin ( SurfaceOutputLuxSkin s, fixed3 lightDir, fixed3 viewDir, fixed atten )
	{
		lightDir = normalize( lightDir );
		viewDir = normalize( viewDir );
		half3 h = normalize (lightDir + viewDir);
		float dotNL = max (0, dot (s.Normal, lightDir));
		// We must NOT max dotNLBlur due to Half-Lambert lighting
		float dotNLBlur = dot( s.NormalBlur, lightDir);
		float dotNH = max (0, dot (s.Normal, h));

	//	////////////////////////////////////////////////////////////
	//	Blinn Phong
		#if defined (LUX_LIGHTING_BP)
		// bring specPower into a range of 0.25 – 2048
		float specPower = exp2(10 * s.Specular + 1) - 1.75;
		float spec = specPower * 0.125 * pow(dotNH, specPower);

	//	Visibility: Schlick-Smith
		float alpha = 2.0 / sqrt( Pi * (specPower + 2) );
		float visibility = 1.0 / ( (dotNL * (1 - alpha) + alpha) * ( saturate(dot(s.Normal, viewDir)) * (1 - alpha) + alpha) ); 
		spec *= visibility;
		#endif

	//	////////////////////////////////////////////////////////////
	//	Cook Torrrence like
	//	from The Order 1886 // http://blog.selfshadow.com/publications/s2013-shading-course/rad/s2013_pbs_rad_notes.pdf

		#if defined (LUX_LIGHTING_CT)
		float dotNV = max(0, dot(s.Normal, normalize(viewDir) ) );

	//	Please note: s.Specular must be linear
		float alpha = (1.0 - s.Specular); // alpha is roughness
		alpha *= alpha;
		float alpha2 = alpha * alpha;

	//	Specular Normal Distribution Function: GGX Trowbridge Reitz
		float denominator = (dotNH * dotNH) * (alpha2 - 1) + 1;
		denominator = Pi * denominator * denominator;
		float spec = alpha2 / denominator;

	//	Geometric Shadowing: Smith
		float V_ONE = dotNL + sqrt(alpha2 + (1 - alpha2) * dotNL * dotNL );
		float V_TWO = dotNV + sqrt(alpha2 + (1 - alpha2) * dotNV * dotNV );
		spec /= V_ONE * V_TWO;
		#endif

	//	Fresnel: Schlick
		// fast fresnel approximation:
		float fresnel = s.SpecularColor + ( 1.0 - s.SpecularColor) * exp2(-OneOnLN2_x6 * dot(h, lightDir));
		spec *= fresnel;

		float2 brdfUV;
		// Half-Lambert lighting value based on blurred normals.
		brdfUV.x = dotNLBlur * 0.5 + 0.5;
		//Curvature amount. Multiplied by light's luminosity so brighter light = more scattering.
		brdfUV.y = s.Curvature * dot( _LightColor0.rgb, fixed3(0.22, 0.707, 0.071 ) );
		float3 brdf = tex2D( _BRDFTex, brdfUV ).rgb;

		float m = atten; // Multiplier for spec and brdf.
		#if !defined (SHADOWS_SCREEN) && !defined (SHADOWS_DEPTH) && !defined (SHADOWS_CUBE)
			// If shadows are off, we need to reduce the brightness
			// of the scattering on polys facing away from the light
			// as it won't get killed off by shadow value.
			// Same for the specular highlights.
			m *= saturate( ( (dotNLBlur * 0.5 + 0.5) * 2.0) * 2.0 - 1.0);
		#endif

		fixed4 c;
		// m replaces atten / apply late dotNL on spec
		c.rgb = (brdf * s.Albedo + spec * dotNL) * _LightColor0.rgb * m * 2;
		//c.a = s.Curvature; // Output the curvature to the frame alpha, just as a debug.
		c.a = s.Alpha;
		return c;
	}


ENDCG

}
FallBack "Diffuse"
CustomEditor "LuxMaterialInspector"
}