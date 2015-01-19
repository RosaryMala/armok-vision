Shader "Hidden/Terrain/Lux Terrain Far" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_ColorMap ("Custom Color Map (RGB)", 2D) = "white" {}
		_TerrainNormalMap ("Terrain Normal Map (RGB)", 2D) = "bump" {}
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		Fog { Mode Off }
		
		CGPROGRAM
		#pragma surface surf LuxDirect noambient finalcolor:customFogExp2 vertex:vert
		#pragma glsl
		#pragma target 3.0

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


	//	important here as we do not need it here
		#define NO_DEFERREDFRESNEL

		// include should be called after all defines
		#include "../LuxCore/LuxLightingDirect.cginc"

		#ifdef COLORMAP_OFF
			sampler2D _MainTex;
		#endif
		#ifdef COLORMAP_ON
			sampler2D _ColorMap;
		#endif
		sampler2D _TerrainNormalMap;
		
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
			#ifdef COLORMAP_OFF
				float2 uv_MainTex;
			#endif
			#ifdef COLORMAP_ON
				float2 uv_ColorMap;
			#endif
			float worlDistance;
			float3 worldNormal;
			INTERNAL_DATA
		};

		// CustomFog only has to be added in the first pass. Otherwise fog gets overbrightened.
		// Fog linear
		void customFogLinear (Input IN, SurfaceOutputLux o, inout fixed4 color)
		{
			float fogFactor = saturate((unity_FogEnd.x - IN.worlDistance) / (unity_FogEnd.x - unity_FogStart.x));
			color.rgb = lerp(unity_FogColor, color.rgb, fogFactor);
		}
		// Fog Exp
		void customFogExp (Input IN, SurfaceOutputLux o, inout fixed4 color)
		{
			float f = IN.worlDistance* unity_FogDensity;
			float fogFactor = saturate(1 / pow(2.71828,  f));
			color.rgb = lerp(unity_FogColor, color.rgb, fogFactor);
		}
		// Fog Exp2
		void customFogExp2 (Input IN, SurfaceOutputLux o, inout fixed4 color)
		{
			float f = IN.worlDistance * unity_FogDensity;
			float fogFactor = saturate(1 / pow(2.71828,  f * f));
			color.rgb = lerp(unity_FogColor, color.rgb, fogFactor);
		}
		
		void vert (inout appdata_full v, out Input o) {
			UNITY_INITIALIZE_OUTPUT(Input,o);
			// Supply the shader with tangents for the terrain
			v.tangent.xyz = cross(v.normal, float3(0,0,1));
			v.tangent.w = -1;
			o.worlDistance = distance(_WorldSpaceCameraPos, mul(_Object2World, v.vertex).xyz);
		}

		void surf (Input IN, inout SurfaceOutputLux o) {
			half4 c;
			#ifdef COLORMAP_OFF
				c = tex2D (_MainTex, IN.uv_MainTex);
				o.Normal = UnpackNormal( tex2D(_TerrainNormalMap, IN.uv_MainTex) );
			#endif
			#ifdef COLORMAP_ON
				c = tex2D (_ColorMap, IN.uv_ColorMap);
				o.Normal = UnpackNormal( tex2D(_TerrainNormalMap, IN.uv_ColorMap) );
			#endif
			// Spec color
			o.SpecularColor = 0.0;
			// Roughness
			o.Specular = 0.0;
			o.Albedo = c.rgb;
			// Set Alpha
			o.Alpha = 0.0;

			#include "../LuxCore/LuxLightingAmbient.cginc"

			// reduce ambient lighting according colorMap.a (ambient occlusion)
			#ifdef COLORMAP_ON
				o.Emission = o.Emission * c.aaa;
			#endif
		}
		ENDCG
	} 
	FallBack "Diffuse"
}
