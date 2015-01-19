// Lux Grass Billboard Shader: overwrites the built in one

Shader "Hidden/TerrainEngine/Details/BillboardWavingDoublePass" {
	Properties {
		_WavingTint ("Fade Color", Color) = (.7,.6,.5, 0)
		_MainTex ("Base (RGB) Alpha (A)", 2D) = "white" {}
		_WaveAndDistance ("Wave and distance", Vector) = (12, 3.6, 1, 1)
		_Cutoff ("Cutoff", float) = 0.5
	}
	
CGINCLUDE
#include "UnityCG.cginc"
#include "TerrainEngine.cginc"
#pragma glsl_no_auto_normalization

struct v2f {
	float4 pos : POSITION;
	fixed4 color : COLOR;
	float4 uv : TEXCOORD0;
};
v2f BillboardVert (appdata_full v) {
	v2f o;
	WavingGrassBillboardVert (v);
	o.color = v.color;
	
	o.color.rgb *= ShadeVertexLights (v.vertex, v.normal);
		
	o.pos = mul (UNITY_MATRIX_MVP, v.vertex);	
	o.uv = v.texcoord;
	return o;
}
ENDCG

	SubShader {
		Tags {
			"Queue" = "Geometry+200"
			"IgnoreProjector"="True"
			"RenderType"="GrassBillboard"
		}
		Cull Off
		LOD 200
//		ColorMask RGB // We must not use color mask as it causes flickering in forward
				
CGPROGRAM
#pragma surface surf Lambert vertex:LuxWavingGrassBillboardVert addshadow noambient
#pragma target 3.0

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

void LuxWavingGrassBillboardVert (inout appdata_full v, out Input o)
{
	UNITY_INITIALIZE_OUTPUT(Input,o);
	TerrainBillboardGrass (v.vertex, v.tangent.xy);
	// wave amount defined by the grass height
	float waveAmount = v.tangent.y;
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
			"RenderType"="GrassBillboard"
		}

		ColorMask RGB
		Cull Off
		Lighting On
		
		Pass {
			CGPROGRAM
			#pragma vertex BillboardVert
			#pragma exclude_renderers shaderonly
			ENDCG

			AlphaTest Greater [_Cutoff]

			SetTexture [_MainTex] { combine texture * primary DOUBLE, texture * primary }
		}
	} 
	
	Fallback Off
}
