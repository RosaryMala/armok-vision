Shader "Hidden/Lux/Nature/Tree Creator Bark Rendertex" {
Properties {
	_MainTex ("Base (RGB) Alpha (A)", 2D) = "white" {}
	_BumpSpecMap ("Normalmap (GA) Spec (R)", 2D) = "bump" {}
	_TranslucencyMap ("Trans (RGB) Gloss(A)", 2D) = "white" {}
	
	// These are here only to provide default values
	_SpecColor ("Specular Color", Color) = (0.5, 0.5, 0.5, 1)
	_Scale ("Scale", Vector) = (1,1,1,1)
	_SquashAmount ("Squash", Float) = 1
}

SubShader {  
	Fog { Mode Off }	
	Pass {
CGPROGRAM
#pragma vertex vert
#pragma fragment frag
#include "UnityCG.cginc"
#pragma target 3.0

#pragma multi_compile LUX_LINEAR LUX_GAMMA
#pragma multi_compile LUX_LLFIX_BILLBOARDS_ON LUX_LLFIX_BILLBOARDS_OFF
#pragma multi_compile GLDIFFCUBE_ON GLDIFFCUBE_OFF

struct v2f {
	float4 pos : SV_POSITION;
	float2 uv : TEXCOORD0;
	float3 color : TEXCOORD1;
	float2 params[3]: TEXCOORD2;
	// normal equals worldnormal
	float3 normal : TEXCOORD5;
};

CBUFFER_START(UnityTerrainImposter)
	float3 _TerrainTreeLightDirections[4];
	float4 _TerrainTreeLightColors[4];
CBUFFER_END

v2f vert (appdata_full v) {
	v2f o;
	o.pos = mul (UNITY_MATRIX_MVP, v.vertex);
	o.uv = v.texcoord.xy;
	float3 viewDir = normalize(ObjSpaceViewDir(v.vertex));
	
	for (int j = 0; j < 3; j++)
	{
		float3 lightDir = _TerrainTreeLightDirections[j];
	
		half nl = dot (v.normal, lightDir);
		o.params[j].r = max (0, nl);
		
		half3 h = normalize (lightDir + viewDir);
		float nh = max (0, dot (v.normal, h));
		o.params[j].g = nh;
	}
	o.color = v.color.a;
	o.normal = v.normal;
	return o;
}

sampler2D _MainTex;
sampler2D _BumpSpecMap;
sampler2D _TranslucencyMap;
fixed4 _SpecColor;

#ifdef GLDIFFCUBE_ON
	samplerCUBE _DiffCubeIBL;
#endif

// Is set by script
float4 ExposureIBL;

fixed4 frag (v2f i) : COLOR
{

	fixed3 albedo = tex2D (_MainTex, i.uv).rgb;
	half gloss = tex2D(_TranslucencyMap, i.uv).a;
	float specular = exp2(10 * tex2D (_BumpSpecMap, i.uv).r + 1) - 1.75;

	half3 ambient;
	//	add diffuse IBL / always globally controlled
	#ifdef GLDIFFCUBE_ON
		half4 diff_ibl = texCUBE (_DiffCubeIBL, i.normal);
		#ifdef LUX_LINEAR
			// if colorspace = linear alpha has to be brought to linear too (rgb already is): alpha = pow(alpha,2.233333333).
			// approximation taken from http://chilliant.blogspot.de/2012/08/srgb-approximations-for-hlsl.html
			diff_ibl.a *= diff_ibl.a * (diff_ibl.a * 0.305306011 + 0.682171111) + 0.012522878;
		#endif
		diff_ibl.rgb = diff_ibl.rgb * diff_ibl.a;
		ambient = diff_ibl.rgb * ExposureIBL.x * i.color;
	#else
//		otherwise add ambient light
		ambient = UNITY_LIGHTMODEL_AMBIENT * i.color * 2;
	#endif

	half3 light = 0;
	float3 spec = 0;
	for (int j = 0; j < 3; j++)
	{
		half3 lightColor = _TerrainTreeLightColors[j].rgb;
/// lightcolors are passed using the wrong colorspace
		#if defined (LUX_LINEAR) && defined (LUX_LLFIX_BILLBOARDS_ON)
			lightColor = pow(lightColor,1/2.2);
		#endif
		
		half nl = i.params[j].r;
		// diffuse
		light += lightColor * nl;
		float nh = i.params[j].g;
		spec += specular * 0.125 * pow(nh, specular) * gloss * lightColor;
	}
	fixed4 c;
	c.rgb = (albedo * light + spec) * 2 + ambient * albedo;
	c.a = 1.0;
	return c;
}
ENDCG
	}
}

FallBack Off
}
