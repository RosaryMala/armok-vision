Shader "Hidden/Lux/Nature/Tree Creator Bark Optimized" {
Properties {
	_Color ("Main Color", Color) = (1,1,1,1)
	_MainTex ("Base (RGB) Alpha (A)", 2D) = "white" {}
	_BumpSpecMap ("Normalmap (GA) Spec (R)", 2D) = "bump" {}
	_TranslucencyMap ("Trans (RGB) Gloss(A)", 2D) = "white" {}

	// These are here only to provide default values
	_SpecColor ("Specular Color", Color) = (0.5, 0.5, 0.5, 1)
	_Scale ("Scale", Vector) = (1,1,1,1)
	_SquashAmount ("Squash", Float) = 1
}

SubShader { 
	Tags { "RenderType"="TreeBark" }
	LOD 200
	Fog { Mode Off }
	
CGPROGRAM

#pragma surface surf LuxDirect vertex:LuxTreeVertBark addshadow nolightmap nodirlightmap noambient finalcolor:customFogExp2
#pragma exclude_renderers flash
#pragma glsl_no_auto_normalization
#include "Tree.cginc"
#pragma target 3.0

#pragma glsl

#pragma multi_compile LUX_LIGHTING_BP LUX_LIGHTING_CT
#pragma multi_compile LUX_LINEAR LUX_GAMMA
#pragma multi_compile LUX_LLFIX_MESHTREES_ON LUX_LLFIX_MESHTREES_OFF
#pragma multi_compile GLDIFFCUBE_ON GLDIFFCUBE_OFF
// enable global switch between diff cube ibl and SH for this shader
#define USE_GLOBAL_DIFFIBL_SETTINGS

#define NO_DEFERREDFRESNEL

//#define LUX_LINEAR
//#define GLDIFFCUBE_ON

// include should be called after all defines
#include "../../LuxCore/LuxLightingDirect.cginc"


sampler2D _MainTex;
sampler2D _BumpSpecMap;
sampler2D _TranslucencyMap;

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
	float2 uv_MainTex;
	fixed4 color : COLOR;
	float worldDistance;
	float3 viewDir;
	float3 worldNormal;
	INTERNAL_DATA
};

// CustomFog must only be applied in Forward Pass Base (#if !defined ...)

// Fog linear
void customFogLinear (Input IN, SurfaceOutputLux o, inout fixed4 color)
{
	#if !defined (UNITY_PASS_FORWARDADD)
		float fogFactor = saturate((unity_FogEnd.x - IN.worldDistance) / (unity_FogEnd.x - unity_FogStart.x));
		color.rgb = lerp(unity_FogColor, color.rgb, fogFactor);
	#endif
}
// Fog Exp
void customFogExp (Input IN, SurfaceOutputLux o, inout fixed4 color)
{
	#if !defined (UNITY_PASS_FORWARDADD)
		float f = IN.worldDistance * unity_FogDensity;
		float fogFactor = saturate(1 / pow(2.71828,  f));
		color.rgb = lerp(unity_FogColor, color.rgb, fogFactor);
	#endif
}
// Fog Exp2
void customFogExp2 (Input IN, SurfaceOutputLux o, inout fixed4 color)
{
	#if !defined (UNITY_PASS_FORWARDADD)
		float f = IN.worldDistance * unity_FogDensity;
		float fogFactor = saturate(1 / pow(2.71828,  f * f));
		color.rgb = lerp(unity_FogColor, color.rgb, fogFactor);
	#endif
}

void LuxTreeVertBark (inout appdata_full v, out Input o) 
{
	UNITY_INITIALIZE_OUTPUT(Input,o);
	v.vertex.xyz *= _Scale.xyz;
	v.vertex = AnimateVertex(v.vertex, v.normal, float4(v.color.xy, v.texcoord1.xy));
	v.vertex = Squash(v.vertex);
	v.color = float4 (1, 1, 1, v.color.a);
	v.normal = normalize(v.normal);
	v.tangent.xyz = normalize(v.tangent.xyz);

	// tree color may be passed in by the terrain engine using the wrong colorspace
	#if defined (LUX_LINEAR) && defined (LUX_LLFIX_MESHTREES_ON)
		v.color.rgb = pow(_Color.rgb,0.45454545); // pow(color/1/2.2)
	#else
		v.color.rgb = _Color.rgb;
	#endif
	o.worldDistance = length(mul(UNITY_MATRIX_MV, v.vertex).xyz);
}

void surf (Input IN, inout SurfaceOutputLux o) {
	fixed4 c = tex2D(_MainTex, IN.uv_MainTex.xy);
	// tree’s color gets corrected into the right color space in the vertex function, so we use In.color.rgb here / ambient occlusion is added to emission, so no * In.color.a here
	o.Albedo = c.rgb * IN.color.rgb; 

	fixed4 trngls = tex2D (_TranslucencyMap, IN.uv_MainTex.xy);
	// Roughness – gamma for BlinnPhong / linear for CookTorrence
	o.Specular = LuxAdjustSpecular(trngls.a);
	o.Alpha = c.a;
	half4 norspc = tex2D (_BumpSpecMap, IN.uv_MainTex.xy);
	// Specular Color
	o.SpecularColor = norspc.rrr;
	o.Normal = UnpackNormalDXT5nm(norspc);
	
	#include "../../LuxCore/LuxLightingAmbient.cginc"

	// Ambient Occlusion
	o.Emission *= IN.color.a;
}
ENDCG
}
Dependency "BillboardShader" = "Hidden/Lux/Nature/Tree Creator Bark Rendertex"
}
