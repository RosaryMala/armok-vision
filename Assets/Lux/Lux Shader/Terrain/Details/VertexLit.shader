// Lux Vertex Lit Shader: overwrites the built in one

Shader "Hidden/TerrainEngine/Details/Vertexlit" {
Properties {
	_MainTex ("Main Texture", 2D) = "white" {  }
}
SubShader {
	Tags { "RenderType"="Opaque" }
	LOD 200

CGPROGRAM
#pragma surface surf Lambert vertex:LuxVertLit noambient
#pragma target 3.0

#pragma multi_compile LUX_LINEAR LUX_GAMMA
#pragma multi_compile GLDIFFCUBE_ON GLDIFFCUBE_OFF
// enable global switch between diff cube ibl and SH for this shader
#define USE_GLOBAL_DIFFIBL_SETTINGS
// as vertex.normal = worldNormal we do not have to use worldNormal here (expensive)
#define NORMAL_IS_WORLDNORMAL

sampler2D _MainTex;

#if defined(USE_GLOBAL_DIFFIBL_SETTINGS) && defined(GLDIFFCUBE_ON)
	samplerCUBE _DiffCubeIBL;
#endif
// Is set by script
float4 ExposureIBL;

struct Input {
	float2 uv_MainTex;
	fixed4 color : COLOR;
	float3 normal;
};

void LuxVertLit (inout appdata_full v, out Input o)
{
	UNITY_INITIALIZE_OUTPUT(Input,o);
	o.normal = v.normal.xyz;
}

void surf (Input IN, inout SurfaceOutput o) {
	fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * IN.color;
	o.Albedo = c.rgb;
	o.Alpha = c.a;

	#include "../../LuxCore/LuxLightingAmbient.cginc"
}

ENDCG
}
SubShader {
	Tags { "RenderType"="Opaque" }
	Pass {
		Tags { "LightMode" = "Vertex" }
		ColorMaterial AmbientAndDiffuse
		Lighting On
		SetTexture [_MainTex] {
			combine texture * primary DOUBLE, texture * primary
		} 
	}
	Pass {
		Tags { "LightMode" = "VertexLMRGBM" }
		ColorMaterial AmbientAndDiffuse
		BindChannels {
			Bind "Vertex", vertex
			Bind "texcoord1", texcoord0 // lightmap uses 2nd uv
			Bind "texcoord", texcoord1 // main uses 1st uv
		}
		SetTexture [unity_Lightmap] {
			matrix [unity_LightmapMatrix]
			combine texture * texture alpha DOUBLE
		}
		SetTexture [_MainTex] { combine texture * previous QUAD, texture }
	}
}

Fallback "VertexLit"
}
