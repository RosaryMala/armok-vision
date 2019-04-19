// Upgrade NOTE: upgraded instancing buffer 'MyProperties' to new syntax.

float _Amount;

half4       _Color;
half4 _Color1;
half4 _Color2;
half4 _Color3;
sampler2D   _PatternMask;

sampler2D   _MainTex;

sampler2D   _BumpMap;
half        _BumpScale;
sampler2D _DFMask;
UNITY_DECLARE_TEX2DARRAY(_MatTexArray);
UNITY_DECLARE_TEX2DARRAY(_ShapeMap);

sampler2D   _SpecGlossMap;
sampler2D   _MetallicGlossMap;
half        _Metallic;
half        _Glossiness;
half        _GlossMapScale;

sampler2D   _OcclusionMap;
half        _OcclusionStrength;

half4       _EmissionColor;
sampler2D   _EmissionMap;

float3      _ViewMin = float3(-99999, -99999, -99999);
float3      _ViewMax = float3(99999, 99999, 99999);

UNITY_INSTANCING_BUFFER_START(MyProperties)
UNITY_DEFINE_INSTANCED_PROP(fixed4, _MatColor)
#define _MatColor_arr MyProperties
UNITY_DEFINE_INSTANCED_PROP(int, _MatIndex)
#define _MatIndex_arr MyProperties
UNITY_DEFINE_INSTANCED_PROP(int, _ShapeIndex)
#define _ShapeIndex_arr MyProperties
UNITY_DEFINE_INSTANCED_PROP(fixed4, _JobColor)
#define _JobColor_arr MyProperties
UNITY_INSTANCING_BUFFER_END(MyProperties)

struct Input {
	float2 uv_MainTex;
#ifdef _SECOND_UV
	float2 uv2_MatTexArray;
#else
	float2 uv_MatTexArray;
#endif
	float3 worldPos;
};

float4 TexCoords(Input IN)
{
	float4 texcoord;
	texcoord.xy = IN.uv_MainTex; // Always source from uv0
#ifdef _SECOND_UV
	texcoord.zw = IN.uv2_MatTexArray;
#else
	texcoord.zw = IN.uv_MatTexArray;
#endif
	return texcoord;
}