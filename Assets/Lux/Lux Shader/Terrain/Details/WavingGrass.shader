// Lux Grass Shader: overwrites the built in one

Shader "Hidden/TerrainEngine/Details/WavingDoublePass" {
Properties {
	_WavingTint ("Fade Color", Color) = (.7,.6,.5, 0)
	_MainTex ("Base (RGB) Alpha (A)", 2D) = "white" {}
	_WaveAndDistance ("Wave and distance", Vector) = (12, 3.6, 1, 1)
	_Cutoff ("Cutoff", float) = 0.5
}

SubShader {
	Tags {
		"Queue" = "Geometry+200"
		"IgnoreProjector"="True"
		"RenderType"="Grass"
	}
	Cull Off
	LOD 200
//	ColorMask RGB // We must not use color mask as it causes flickering in forward
		
CGPROGRAM
#pragma surface surf Lambert vertex:LuxWavingGrassVert addshadow noambient
#pragma target 3.0
#include "TerrainEngine.cginc"


#pragma multi_compile LUX_LINEAR LUX_GAMMA
#pragma multi_compile GLDIFFCUBE_ON GLDIFFCUBE_OFF
// enable global switch between diff cube ibl and SH for this shader
#define USE_GLOBAL_DIFFIBL_SETTINGS
// as vertex.normal = worldNormal we do not have to use worldNormal here (expensive)
#define NORMAL_IS_WORLDNORMAL

sampler2D _MainTex;
fixed _Cutoff;

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

void LuxWavingGrassVert (inout appdata_full v, out Input o)
{
	UNITY_INITIALIZE_OUTPUT(Input,o);
	float waveAmount = v.color.a * _WaveAndDistance.z;
	v.color = TerrainWaveGrass (v.vertex, waveAmount, v.color);
	o.normal = v.normal.xyz;
}

void surf (Input IN, inout SurfaceOutput o) {
	fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * IN.color;
	o.Albedo = c.rgb;
	o.Alpha = c.a;
	clip (o.Alpha - _Cutoff);
	o.Alpha *= IN.color.a;

	#include "../../LuxCore/LuxLightingAmbient.cginc"
}
ENDCG
}
	
	SubShader {
		Tags {
			"Queue" = "Geometry+200"
			"IgnoreProjector"="True"
			"RenderType"="Grass"	
		}
		Cull Off
		LOD 200
		ColorMask RGB
		
		Pass {
			Tags { "LightMode" = "Vertex" }
			Material {
				Diffuse (1,1,1,1)
				Ambient (1,1,1,1)
			}
			Lighting On
			ColorMaterial AmbientAndDiffuse
			AlphaTest Greater [_Cutoff]
			SetTexture [_MainTex] { combine texture * primary DOUBLE, texture }
		}
		Pass {
			Tags { "LightMode" = "VertexLMRGBM" }
			AlphaTest Greater [_Cutoff]
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
	
	Fallback Off
}
