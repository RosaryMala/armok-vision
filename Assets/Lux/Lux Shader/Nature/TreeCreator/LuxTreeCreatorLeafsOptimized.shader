Shader "Hidden/Lux/Nature/Tree Creator Leaves Optimized" {
Properties {
	_Color ("Main Color", Color) = (1,1,1,1)
	_TranslucencyColor ("Translucency Color", Color) = (0.73,0.85,0.41,1) // (187,219,106,255)
	_Cutoff ("Alpha cutoff", Range(0,1)) = 0.3
	_TranslucencyViewDependency ("View dependency", Range(0,1)) = 0.7
	_ShadowStrength("Shadow Strength", Range(0,1)) = 0.8
	_ShadowOffsetScale ("Shadow Offset Scale", Float) = 1
	
	_MainTex ("Base (RGB) Alpha (A)", 2D) = "white" {}
	_ShadowTex ("Shadow (RGB)", 2D) = "white" {}
	_BumpSpecMap ("Normalmap (GA) Spec (R) Shadow Offset (B)", 2D) = "bump" {}
	_TranslucencyMap ("Trans (B) Gloss(A)", 2D) = "white" {}

	// These are here only to provide default values
	_Scale ("Scale", Vector) = (1,1,1,1)
	_SquashAmount ("Squash", Float) = 1
}

SubShader { 
	Tags {
		"IgnoreProjector"="True"
		"RenderType"="TreeLeaf"
	}
	LOD 200
	Fog { Mode Off }
	
CGPROGRAM
#pragma surface surf TreeLeaf alphatest:_Cutoff vertex:LuxTreeVertLeaf nolightmap nodirlightmap noambient finalcolor:customFogExp2
#pragma exclude_renderers flash
#pragma glsl_no_auto_normalization
#include "Lighting.cginc"
#include "Tree.cginc"
//#pragma glsl // this ruins the shadow collector pass!
#pragma target 3.0

#pragma multi_compile LUX_LINEAR LUX_GAMMA
#pragma multi_compile LUX_LLFIX_MESHTREES_ON LUX_LLFIX_MESHTREES_OFF
#pragma multi_compile GLDIFFCUBE_ON GLDIFFCUBE_OFF
// enable global switch between diff cube ibl and SH for this shader
#define USE_GLOBAL_DIFFIBL_SETTINGS


//#define LUX_LINEAR
//#define LUX_LLFIX_MESHTREES_ON
//#define GLDIFFCUBE_ON 


// important: needed as we do not include lux lighting
#define OneOnLN2_x6 8.656170

sampler2D _MainTex;
float4 _MainTex_ST;
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
	float3 myuv_MainTex;	//here we need float3 in order to store distance
	fixed4 color : COLOR; // color.a = AO
	float3 viewDir;
	float3 worldNormal;
	INTERNAL_DATA
};

// CustomFog must only be applied in Forward Pass Base (#if !defined ...)

// Fog linear
void customFogLinear (Input IN, LeafSurfaceOutput o, inout fixed4 color)
{
	#if !defined (UNITY_PASS_FORWARDADD)
		float fogFactor = saturate((unity_FogEnd.x - IN.myuv_MainTex.z) / (unity_FogEnd.x - unity_FogStart.x));
		color.rgb = lerp(unity_FogColor, color.rgb, fogFactor);
	#endif
}
// Fog Exp
void customFogExp (Input IN, LeafSurfaceOutput o, inout fixed4 color)
{
	#if !defined (UNITY_PASS_FORWARDADD)
		float f = IN.myuv_MainTex.z * unity_FogDensity;
		float fogFactor = saturate(1 / pow(2.71828,  f));
		color.rgb = lerp(unity_FogColor, color.rgb, fogFactor);
	#endif
}
// Fog Exp2
void customFogExp2 (Input IN, LeafSurfaceOutput o, inout fixed4 color)
{
	#if !defined (UNITY_PASS_FORWARDADD)
		float f = IN.myuv_MainTex.z * unity_FogDensity;
		float fogFactor = saturate(1 / pow(2.71828,  f * f));
		color.rgb = lerp(unity_FogColor, color.rgb, fogFactor);
	#endif
}

void LuxTreeVertLeaf (inout appdata_full v, out Input o) 
{
	UNITY_INITIALIZE_OUTPUT(Input,o);
	o.myuv_MainTex.xy = TRANSFORM_TEX(v.texcoord, _MainTex);
	ExpandBillboard (UNITY_MATRIX_IT_MV, v.vertex, v.normal, v.tangent);
	v.vertex.xyz *= _Scale.xyz;
	v.vertex = AnimateVertex (v.vertex,v.normal, float4(v.color.xy, v.texcoord1.xy));
	v.vertex = Squash(v.vertex);
	v.color = float4 (1, 1, 1, v.color.a);
	v.normal = normalize(v.normal);
	v.tangent.xyz = normalize(v.tangent.xyz);

	// tree color may be passed in by the terrain engine using the wrong colorspace
	#if defined (LUX_LINEAR) && defined (LUX_LLFIX_MESHTREES_ON)
		v.color.rgb = pow(_Color.rgb,0.45454545); // pow(color/1/2.2)
	#else
		v.color.rgb = _Color.rgba;
	#endif
	o.myuv_MainTex.z = length(mul(UNITY_MATRIX_MV, v.vertex).xyz);
}


void surf (Input IN, inout LeafSurfaceOutput o) {
	fixed4 c = tex2D(_MainTex, IN.myuv_MainTex.xy);

	// tree’s color gets corrected into the right color space in the vertex function, so we use In.color.rgb here / ambient occlusion is added to emission, so no * In.color.a here
	o.Albedo = c.rgb * IN.color.rgb; 
	
	fixed4 trngls = tex2D (_TranslucencyMap, IN.myuv_MainTex.xy);
	o.Translucency = trngls.b;
	// Roughness
//	o.Specular = trngls.a;
	o.Gloss = trngls.a;
	o.Alpha = c.a;
	
	half4 norspc = tex2D (_BumpSpecMap, IN.myuv_MainTex.xy);
//	o.SpecularColor = norspc.rrr;
	o.Specular = norspc.r;


	o.Normal = UnpackNormalDXT5nm(norspc);

	#include "../../LuxCore/LuxLightingAmbient.cginc"
	
	// Ambient Occlusion
	o.Emission *= IN.color.a;
}
ENDCG

	// Pass to render object as a shadow caster
	Pass {
		Name "ShadowCaster"
		Tags { "LightMode" = "ShadowCaster" }
		
		Fog {Mode Off}
		ZWrite On ZTest LEqual Cull Off
		Offset 1, 1

		CGPROGRAM
		#pragma vertex vert_surf
		#pragma fragment frag_surf
		#pragma exclude_renderers noshadows flash
		#pragma glsl_no_auto_normalization
		#pragma multi_compile_shadowcaster
		#include "HLSLSupport.cginc"
		#include "UnityCG.cginc"
		#include "Lighting.cginc"

		#define INTERNAL_DATA
		#define WorldReflectionVector(data,normal) data.worldRefl

		#include "Tree.cginc"

		sampler2D _ShadowTex;

		struct Input {
			float2 uv_MainTex;
		};

		struct v2f_surf {
			V2F_SHADOW_CASTER;
			float2 hip_pack0 : TEXCOORD1;
		};
		float4 _ShadowTex_ST;
		v2f_surf vert_surf (appdata_full v) {
			v2f_surf o;
			TreeVertLeaf (v);
			o.hip_pack0.xy = TRANSFORM_TEX(v.texcoord, _ShadowTex);
			TRANSFER_SHADOW_CASTER(o)
			return o;
		}
		fixed _Cutoff;
		float4 frag_surf (v2f_surf IN) : COLOR {
			half alpha = tex2D(_ShadowTex, IN.hip_pack0.xy).r;
			clip (alpha - _Cutoff);
			SHADOW_CASTER_FRAGMENT(IN)
		}
		ENDCG
	}
	
	// Pass to render object as a shadow collector
	Pass {
		Name "ShadowCollector"
		Tags { "LightMode" = "ShadowCollector" }
		
		Fog {Mode Off}
		ZWrite On ZTest LEqual

		CGPROGRAM
		#pragma vertex vert_surf
		#pragma fragment frag_surf
		#pragma exclude_renderers noshadows flash
		#pragma multi_compile_shadowcollector
		#pragma glsl_no_auto_normalization
		#include "HLSLSupport.cginc"
		#define SHADOW_COLLECTOR_PASS
		#include "UnityCG.cginc"
		#include "Lighting.cginc"

		#define INTERNAL_DATA
		#define WorldReflectionVector(data,normal) data.worldRefl

		#include "Tree.cginc"

		sampler2D _MainTex;
		sampler2D _BumpSpecMap;
		sampler2D _TranslucencyMap;
		float _ShadowOffsetScale;

		struct Input {
			float2 uv_MainTex;
		};

		struct v2f_surf {
			V2F_SHADOW_COLLECTOR;
			float2 hip_pack0 : TEXCOORD5;
			float3 normal : TEXCOORD6;
		};
		
		float4 _MainTex_ST;
		
		v2f_surf vert_surf (appdata_full v) {
			v2f_surf o;
			TreeVertLeaf (v);
			o.hip_pack0.xy = TRANSFORM_TEX(v.texcoord, _MainTex);
			
			float3 worldN = mul((float3x3)_Object2World, SCALED_NORMAL);
			o.normal = mul(_World2Shadow, half4(worldN, 0)).xyz;

			TRANSFER_SHADOW_COLLECTOR(o)
			return o;
		}
		
		fixed _Cutoff;
		
		half4 frag_surf (v2f_surf IN) : COLOR {
			half alpha = tex2D(_MainTex, IN.hip_pack0.xy).a;

			float3 shadowOffset = _ShadowOffsetScale * IN.normal * tex2D (_BumpSpecMap, IN.hip_pack0.xy).b;
			clip (alpha - _Cutoff);

			IN._ShadowCoord0 += shadowOffset;
			IN._ShadowCoord1 += shadowOffset;
			IN._ShadowCoord2 += shadowOffset;
			IN._ShadowCoord3 += shadowOffset;

			SHADOW_COLLECTOR_FRAGMENT(IN)
		}
		ENDCG
	}
}

SubShader {
	Tags {
		"IgnoreProjector"="True"
		"RenderType"="TreeLeaf"
	}
	
	ColorMask RGB
	Lighting On
	
	Pass {
		CGPROGRAM
		#pragma vertex TreeVertLit
		#pragma exclude_renderers shaderonly
		
		#include "UnityCG.cginc"
		#include "Lighting.cginc"
		#include "Tree.cginc"
		
		struct v2f {
			float4 pos : SV_POSITION;
			fixed4 color : COLOR;
			float4 uv : TEXCOORD0;
		};
		
		v2f TreeVertLit (appdata_full v) {
			v2f o;
			TreeVertLeaf(v);

			o.color.rgb = ShadeVertexLights (v.vertex, v.normal);
				
			o.pos = mul (UNITY_MATRIX_MVP, v.vertex);	
			o.uv = v.texcoord;
			o.color.a = 1.0f;
			return o;
		}
		ENDCG

		AlphaTest Greater [_Cutoff]
		SetTexture [_MainTex] { combine texture * primary DOUBLE, texture }
		SetTexture [_MainTex] {
			ConstantColor [_Color]
			Combine previous * constant, previous
		} 
	}
}
Dependency "BillboardShader" = "Hidden/Lux/Nature/Tree Creator Leaves Rendertex"
}
