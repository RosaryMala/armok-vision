Shader "Lux/Terrain/Spec Bumped" {
Properties {

	_ColorMap ("Custom Color Map (RGB) Ambient Occlusion (A)", 2D) = "white" {}
	_TerrainNormalMap ("Terrain Normal Map (RGB)", 2D) = "bump" {}
	_FresnelStrength ("FresnelStrength", Range(0.0,1.0)) = 0.3


//_DetailMap ("Detail Map (RGB)", 2D) = "white" {}
_DetailNormalMap ("Detail Normal Map (RGB)", 2D) = "bump" {}
_DetailNormalMapScale ("Detail Normal Map Scale", Float) = 0.25

	_BasemapDistance ("BaseMap Distance", Float) = 1000
	_FadeLength ("Fade Lenght", Float) = 100

	_ColTex1 ("Avrg. Color Tex 1", Color) = (.5,.5,.5,1)
	_SpecTex1 ("Specular Color Tex 1", Color) = (0.5, 0.5, 0.5, 1)
	_ColTex2 ("Avrg. Color Tex 2", Color) = (.5,.5,.5,1)
	_SpecTex2 ("Specular Color Tex 2", Color) = (0.5, 0.5, 0.5, 1)
	_ColTex3 ("Avrg. Color Tex 3", Color) = (.5,.5,.5,1)
	_SpecTex3 ("Specular Color Tex 3", Color) = (0.5, 0.5, 0.5, 1)
	_ColTex4 ("Avrg. Color Tex 4", Color) = (.5,.5,.5,1)
	_SpecTex4 ("Specular Color Tex 4", Color) = (0.5, 0.5, 0.5, 1)

	_ColTex5 ("Avrg. Color Tex 5", Color) = (.5,.5,.5,1)
	_SpecTex5 ("Specular Color Tex 5", Color) = (0.5, 0.5, 0.5, 1)
	_ColTex6 ("Avrg. Color Tex 6", Color) = (.5,.5,.5,1)
	_SpecTex6 ("Specular Color Tex 6", Color) = (0.5, 0.5, 0.5, 1)
	_ColTex7 ("Avrg. Color Tex 7", Color) = (.5,.5,.5,1)
	_SpecTex7 ("Specular Color Tex 7", Color) = (0.5, 0.5, 0.5, 1)
	_ColTex8 ("Avrg. Color Tex 8", Color) = (.5,.5,.5,1)
	_SpecTex8 ("Specular Color Tex 8", Color) = (0.5, 0.5, 0.5, 1)


	_DiffCubeIBL ("Custom Diffuse Cube", Cube) = "black" {}

	_UVs0 ("Tiling Texture 0", Vector) = (100,100,0,0)
	_UVs1 ("Tiling Texture 1", Vector) = (100,100,0,0)
	_UVs2 ("Tiling Texture 2", Vector) = (100,100,0,0)
	_UVs3 ("Tiling Texture 3", Vector) = (100,100,0,0)

	_UVs4 ("Tiling Texture 4", Vector) = (100,100,0,0)
	_UVs5 ("Tiling Texture 5", Vector) = (100,100,0,0)
	_UVs6 ("Tiling Texture 6", Vector) = (100,100,0,0)
	_UVs7 ("Tiling Texture 7", Vector) = (100,100,0,0)

	// set by terrain engine
	[HideInInspector] _Control ("Control (RGBA)", 2D) = "red" {}
	[HideInInspector] _Splat3 ("Layer 3 (A)", 2D) = "white" {}
	[HideInInspector] _Splat2 ("Layer 2 (B)", 2D) = "white" {}
	[HideInInspector] _Splat1 ("Layer 1 (G)", 2D) = "white" {}
	[HideInInspector] _Splat0 ("Layer 0 (R)", 2D) = "white" {}
	[HideInInspector] _Normal3 ("Normal 3 (A)", 2D) = "bump" {}
	[HideInInspector] _Normal2 ("Normal 2 (B)", 2D) = "bump" {}
	[HideInInspector] _Normal1 ("Normal 1 (G)", 2D) = "bump" {}
	[HideInInspector] _Normal0 ("Normal 0 (R)", 2D) = "bump" {}
	// used in fallback on old cards & base map
	[HideInInspector] _MainTex ("BaseMap (RGB)", 2D) = "white" {}
	[HideInInspector] _Color ("Main Color", Color) = (1,1,1,1)
}
	
SubShader {
	Tags {
		"SplatCount" = "4"
		"Queue" = "Geometry-100"
		"RenderType" = "Opaque"
	}
	Fog { Mode Off }
CGPROGRAM
#pragma surface surf LuxDirect noambient finalcolor:customFogExp2 vertex:vert
#pragma glsl
#pragma target 3.0

// #pragma debug

#pragma multi_compile LUX_LIGHTING_BP LUX_LIGHTING_CT
#pragma multi_compile LUX_LINEAR LUX_GAMMA
#pragma multi_compile GLDIFFCUBE_ON GLDIFFCUBE_OFF
#pragma multi_compile COLORMAP_ON COLORMAP_OFF
// enable global switch between diff cube ibl and SH for this shader
#define USE_GLOBAL_DIFFIBL_SETTINGS

//#define LUX_LIGHTING_CT
//#define LUX_LINEAR
//#define GLDIFFCUBE_ON
//#define COLORMAP_ON
//#define COLORMAP_OFF

// important here as we use our own function in this shader
#define NO_DEFERREDFRESNEL

// include should be called after all defines
#include "../LuxCore/LuxLightingDirect.cginc"

sampler2D _Control;
sampler2D _Splat0,_Splat1,_Splat2,_Splat3;
sampler2D _Normal0,_Normal1,_Normal2,_Normal3;
#ifdef COLORMAP_ON
	sampler2D _ColorMap;
#endif 

sampler2D _TerrainNormalMap;
sampler2D _DetailNormalMap;
float _DetailNormalMapScale;

float4 _UVs0, _UVs1, _UVs2, _UVs3;
float _BasemapDistance, _FadeLength, _FresnelStrength;

fixed3 _ColTex1, _ColTex2, _ColTex3, _ColTex4;
fixed3 _SpecTex1, _SpecTex2, _SpecTex3, _SpecTex4;

#if defined(USE_GLOBAL_DIFFIBL_SETTINGS) && defined(GLDIFFCUBE_ON)
	samplerCUBE _DiffCubeIBL;
#endif
// Is set by script
float4 ExposureIBL;

half4 unity_FogColor;
half4 unity_FogDensity;
half4 unity_FogStart;
half4 unity_FogEnd;

struct Input {
	float2 uv_Control : TEXCOORD0;
	float4 TanViewDirection;
	float3 worldNormal;
	INTERNAL_DATA
};

// CustomFog only has to be added in the first pass. Otherwise fog will get overbrightened.
// It also must only be applied in Forward Pass Base (#if !defined ...)

// Fog linear
void customFogLinear (Input IN, SurfaceOutputLux o, inout fixed4 color)
{
	#if !defined (UNITY_PASS_FORWARDADD)
		float fogFactor = saturate((unity_FogEnd.x - IN.TanViewDirection.w) / (unity_FogEnd.x - unity_FogStart.x));
		color.rgb = lerp(unity_FogColor, color.rgb, fogFactor);
	#endif
}
// Fog Exp
void customFogExp (Input IN, SurfaceOutputLux o, inout fixed4 color)
{
	#if !defined (UNITY_PASS_FORWARDADD)
		float f = IN.TanViewDirection.w * unity_FogDensity;
		float fogFactor = saturate(1 / pow(2.71828,  f));
		color.rgb = lerp(unity_FogColor, color.rgb, fogFactor);
	#endif
}
// Fog Exp2
void customFogExp2 (Input IN, SurfaceOutputLux o, inout fixed4 color)
{
	#if !defined (UNITY_PASS_FORWARDADD)
		float f = IN.TanViewDirection.w * unity_FogDensity;
		float fogFactor = saturate(1 / pow(2.71828,  f * f));
		color.rgb = lerp(unity_FogColor, color.rgb, fogFactor);
	#endif
}

void vert (inout appdata_full v, out Input o) 
{
	UNITY_INITIALIZE_OUTPUT(Input,o);
	v.tangent.xyz = cross(v.normal, float3(0,0,1));
	v.tangent.w = -1;

	// create tangent space rotation
	float3 binormal = cross( v.normal, v.tangent.xyz ) * v.tangent.w;
	float3x3 rotation = float3x3( v.tangent.xyz, binormal, v.normal.xyz );
	
	// get view Direction
	float3 Direction = ObjSpaceViewDir(v.vertex);
	o.TanViewDirection.xyz = normalize( mul( rotation, Direction ) );

	// store distance
	o.TanViewDirection.w = distance(_WorldSpaceCameraPos, mul(_Object2World, v.vertex).xyz);	
}

void surf (Input IN, inout SurfaceOutputLux o) {

	float fadeout = saturate( ( _BasemapDistance - IN.TanViewDirection.w ) / _FadeLength );

	float2 uv_Splat0 = _UVs0.xy * IN.uv_Control;
	float2 uv_Splat1 = _UVs1.xy * IN.uv_Control;
	float2 uv_Splat2 = _UVs2.xy * IN.uv_Control;
	float2 uv_Splat3 = _UVs3.xy * IN.uv_Control;

	fixed4 splat_control = tex2D (_Control, IN.uv_Control);
	fixed4 col;
	col  = splat_control.r * tex2D (_Splat0, uv_Splat0);
	col += splat_control.g * tex2D (_Splat1, uv_Splat1);
	col += splat_control.b * tex2D (_Splat2, uv_Splat2);
	col += splat_control.a * tex2D (_Splat3, uv_Splat3);

	fixed splatSum = dot(splat_control, fixed4(1,1,1,1));

	#ifdef COLORMAP_ON
		fixed3 color_correction;
		fixed4 colorMap = tex2D (_ColorMap, IN.uv_Control) * splatSum;
		color_correction = splat_control.r*_ColTex1 + splat_control.g*_ColTex2 + splat_control.b*_ColTex3 + splat_control.a*_ColTex4;
		col.rgb *= (colorMap.rgb / (color_correction + 0.001f) );
		col.rgb = lerp(colorMap.rgb, col.rgb, fadeout);
	#endif

	o.Albedo = col.rgb;

	fixed4 nrm;
	nrm  = splat_control.r * tex2D (_Normal0, uv_Splat0);
	nrm += splat_control.g * tex2D (_Normal1, uv_Splat1);
	nrm += splat_control.b * tex2D (_Normal2, uv_Splat2);
	nrm += splat_control.a * tex2D (_Normal3, uv_Splat3);
	
	// Sum of our four splat weights might not sum up to 1, in
	// case of more than 4 total splat maps. Need to lerp towards
	// "flat normal" in that case.
	// fixed4 flatNormal = fixed4(0.5,0.5,1,0.5); // this is "flat normal" in both DXT5nm and xyz*2-1 cases
//	Hack: to reduce black borders - works for all sun rotations, forward and deferred
	float storedSplatSum = splatSum;
	splatSum *= splatSum;
	splatSum *= splatSum;
	splatSum *= splatSum;
	
	o.Normal = UnpackNormal(nrm);

	float3 normal1 = UnpackNormal (tex2D (_DetailNormalMap, uv_Splat2 * _DetailNormalMapScale));
	float3 tangentBase = normalize(cross(float3(0.0,1.0,0.0), normal1));
	float3 binormalBase = normalize(cross(normal1, tangentBase));
	normal1 = tangentBase * o.Normal.x + binormalBase * o.Normal.y + normal1 * o.Normal.z;
	o.Normal = lerp(o.Normal, normal1, splat_control.b);

	float3 globalnrm =  UnpackNormal( tex2D(_TerrainNormalMap, IN.uv_Control) );
	tangentBase = normalize(cross(float3(0.0,1.0,0.0), globalnrm));
	binormalBase = normalize(cross(globalnrm, tangentBase));

	o.Normal = tangentBase * o.Normal.x + binormalBase * o.Normal.y + globalnrm * o.Normal.z;
	o.Normal = lerp(globalnrm, o.Normal, fadeout);

//	lerp towards "flat normal"
	o.Normal = lerp (fixed3(0,0,1), o.Normal, splatSum);

//	Spec color
	o.SpecularColor = splat_control.r*_SpecTex1 + splat_control.g*_SpecTex2 + splat_control.b*_SpecTex3 + splat_control.a*_SpecTex4;

//	Roughness – gamma for BlinnPhong / linear for CookTorrence
	o.Specular = LuxAdjustSpecular(col.a * fadeout);

	// Set Alpha
	o.Alpha = 0.0;
	#if defined(UNITY_PASS_PREPASSFINAL)	
			// Fake Fresnel effect using N dot V / only needed by deferred lighting	
			o.DeferredFresnel = exp2(-OneOnLN2_x6 * max(0, dot(o.Normal, IN.TanViewDirection.xyz ))) * _FresnelStrength * storedSplatSum * fadeout;
	#endif

//	Important: We have to remap IN.viewDir here as it is expected by LuxLightingAmbient
	#define viewDir TanViewDirection
	#include "../LuxCore/LuxLightingAmbient.cginc"
	
	// reduce ambient lighting according colorMap.a (ambient occlusion) and splatsum
	#ifdef COLORMAP_ON
		o.Emission = o.Emission * storedSplatSum  * colorMap.aaa;
	#else
		o.Emission = o.Emission * storedSplatSum;
	#endif

}
ENDCG  
}

Dependency "AddPassShader" = "Hidden/Lux/Terrain/Spec Bumped AddPass"
Dependency "BaseMapShader" = "Hidden/Terrain/Lux Terrain Far"
Fallback "Nature/Terrain/Diffuse"
}
