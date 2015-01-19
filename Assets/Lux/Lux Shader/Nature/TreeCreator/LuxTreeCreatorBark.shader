Shader "Lux/Nature/Tree Creator Bark" {
Properties {
	_Color ("Main Color", Color) = (1,1,1,1)
	_Shininess ("Shininess", Range (0.01, 1)) = 0.078125
	_MainTex ("Base (RGB) Alpha (A)", 2D) = "white" {}
	_BumpMap ("Normalmap", 2D) = "bump" {}
	_GlossMap ("Gloss (A)", 2D) = "black" {}
	
	// These are here only to provide default values
	_SpecColor ("Specular Color", Color) = (0.5, 0.5, 0.5, 1)
	_Scale ("Scale", Vector) = (1,1,1,1)
	_SquashAmount ("Squash", Float) = 1
}

SubShader { 
	Tags { "RenderType"="TreeBark" }
	LOD 200
		
CGPROGRAM
#pragma surface surf LuxDirect vertex:TreeVertBark addshadow nolightmap nodirlightmap noambient
#pragma exclude_renderers flash
#pragma glsl_no_auto_normalization
#include "Tree.cginc"
#pragma glsl
#pragma target 3.0

#pragma multi_compile LUX_LIGHTING_BP LUX_LIGHTING_CT
#pragma multi_compile LUX_LINEAR LUX_GAMMA
#pragma multi_compile DIFFCUBE_ON DIFFCUBE_OFF

// include should be called after all defines
#include "../../LuxCore/LuxLightingDirect.cginc"

sampler2D _MainTex;
sampler2D _BumpMap;
sampler2D _GlossMap;
half _Shininess;

#ifdef DIFFCUBE_ON
	samplerCUBE _DiffCubeIBL;
#endif

// Is set by script
float4 ExposureIBL;

struct Input {
	float2 uv_MainTex;
	fixed4 color : COLOR;
	float3 viewDir;
	float3 worldNormal;
	INTERNAL_DATA
};

void surf (Input IN, inout SurfaceOutputLux o) {
	fixed4 c = tex2D(_MainTex, IN.uv_MainTex);
	o.Albedo = c.rgb * _Color.rgb * IN.color.a;
	//Roughness
	o.Specular = tex2D(_GlossMap, IN.uv_MainTex).a;
	o.Alpha = c.a;
	// Specular Color
	o.SpecularColor = _Shininess.xxx;
	o.Normal = UnpackNormal(tex2D(_BumpMap, IN.uv_MainTex));
	#if defined(UNITY_PASS_PREPASSFINAL)	
		// Fake Fresnel effect using N dot V / only needed by deferred lighting	
		o.DeferredFresnel = exp2(-OneOnLN2_x6 * max(0, dot(o.Normal, normalize(IN.viewDir) )));	
	#endif
	#include "../../LuxCore/LuxLightingAmbient.cginc"
	// Ambient Occlusion
	o.Emission *= IN.color.a;
}
ENDCG
}

Dependency "OptimizedShader" = "Hidden/Lux/Nature/Tree Creator Bark Optimized"
FallBack "Diffuse"
}
