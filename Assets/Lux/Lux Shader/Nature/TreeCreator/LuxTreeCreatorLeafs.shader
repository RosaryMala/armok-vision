Shader "Lux/Nature/Tree Creator Leaves" {
Properties {
	_Color ("Main Color", Color) = (1,1,1,1)
	_Shininess ("Shininess", Range (0.01, 1)) = 0.078125
	_MainTex ("Base (RGB) Alpha (A)", 2D) = "white" {}
	_BumpMap ("Normalmap", 2D) = "bump" {}
	_GlossMap ("Gloss (A)", 2D) = "black" {}
	_TranslucencyMap ("Translucency (A)", 2D) = "white" {}
	_ShadowOffset ("Shadow Offset (A)", 2D) = "black" {}
	
	// These are here only to provide default values
	_Cutoff ("Alpha cutoff", Range(0,1)) = 0.3
	_Scale ("Scale", Vector) = (1,1,1,1)
	_SquashAmount ("Squash", Float) = 1
}

SubShader { 
	Tags { "IgnoreProjector"="True" "RenderType"="TreeLeaf" }
	LOD 200
		
CGPROGRAM
#pragma surface surf TreeLeaf alphatest:_Cutoff vertex:TreeVertLeaf addshadow nolightmap nodirlightmap noambient
#pragma exclude_renderers flash
#pragma glsl_no_auto_normalization
#include "Tree.cginc"
#pragma target 3.0

#pragma multi_compile LUX_LINEAR LUX_GAMMA
#pragma multi_compile GLDIFFCUBE_ON GLDIFFCUBE_OFF
// enable global switch between diff cube ibl and SH for this shader
#define USE_GLOBAL_DIFFIBL_SETTINGS

// important
#define OneOnLN2_x6 8.656170

sampler2D _MainTex;
sampler2D _BumpMap;
sampler2D _GlossMap;
sampler2D _TranslucencyMap;
half _Shininess;

#if defined(USE_GLOBAL_DIFFIBL_SETTINGS) && defined(GLDIFFCUBE_ON)
	samplerCUBE _DiffCubeIBL;
#endif

// Is set by script
float4 ExposureIBL;

struct Input {
	float2 uv_MainTex;
	fixed4 color : COLOR; // color.a = AO
	float3 viewDir;
	float3 worldNormal;
	float3 worldRefl;
	INTERNAL_DATA
};

void surf (Input IN, inout LeafSurfaceOutput o) {
	fixed4 c = tex2D(_MainTex, IN.uv_MainTex);
	o.Albedo = c.rgb * _Color.rgb; // do not add ambient occlusion here * IN.color.a;
	o.Translucency = tex2D(_TranslucencyMap, IN.uv_MainTex).rgb;
	// Roughness
	//o.Specular = tex2D(_GlossMap, IN.uv_MainTex).a; // * _Color.r;
	
	o.Gloss = tex2D(_GlossMap, IN.uv_MainTex).a;

	o.Alpha = c.a;
	//o.SpecularColor = _Shininess.xxx;
	o.Specular = _Shininess;

	o.Normal = UnpackNormal(tex2D(_BumpMap, IN.uv_MainTex));

	#include "../../LuxCore/LuxLightingAmbient.cginc"
	
	// Ambient Occlusion
	o.Emission *= IN.color.a;
}
ENDCG
}

Dependency "OptimizedShader" = "Hidden/Lux/Nature/Tree Creator Leaves Optimized"
FallBack "Diffuse"
}
