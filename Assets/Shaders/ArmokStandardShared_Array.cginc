#ifndef ARMOK_STANDARD_SHARED_ARRAY
#define ARMOK_STANDARD_SHARED_ARRAY

UNITY_DECLARE_TEX2DARRAY(_MainTex);
UNITY_DECLARE_TEX2DARRAY(_BumpMap);
UNITY_DECLARE_TEX2DARRAY(_SpecialTex);
sampler2D _SpatterTex;
sampler2D _SpatterNoise;
float4 _SpatterDirection;
float _SpatterSmoothness;
float4 _WorldBounds;
float4 _SpatterNoise_ST;
float4 _TexArrayCount;

struct Input {
	float2 uv_MainTex;
	float2 uv2_BumpMap;
	float2 uv3_SpecialTex;
	float4 color: Color; // Vertex color
	float3 worldNormal;
	float3 worldPos;
	INTERNAL_DATA
};

half _Glossiness;
fixed4 _Color;

void surf(Input IN, inout SurfaceOutputStandard o) {
	// Albedo comes from a texture tinted by color
	fixed4 c = UNITY_SAMPLE_TEX2DARRAY(_MainTex, float3(IN.uv_MainTex.xy, IN.uv2_BumpMap.x * _TexArrayCount.x)) * _Color;
	fixed4 bump = UNITY_SAMPLE_TEX2DARRAY(_BumpMap, float3(IN.uv_MainTex.xy, IN.uv2_BumpMap.y * _TexArrayCount.y));
	fixed4 special = UNITY_SAMPLE_TEX2DARRAY(_SpecialTex, float3(IN.uv_MainTex.xy, IN.uv3_SpecialTex.x * _TexArrayCount.z));
	fixed4 spatter = tex2D(_SpatterTex, (IN.worldPos.xz - _WorldBounds.xy) / (_WorldBounds.zw - _WorldBounds.xy));
	fixed4 noise = tex2D(_SpatterNoise, TRANSFORM_TEX(IN.worldPos.xz, _SpatterNoise));
	//o.Albedo = c.rgb * IN.color.rgb;
	o.Normal = UnpackNormal(bump.ggga);
	o.Alpha = bump.b;
	if (dot(WorldNormalVector(IN, o.Normal), _SpatterDirection.xyz) >= lerp(1, -1, (spatter.a - noise.r)))
	{
		o.Albedo = (spatter.rgb / spatter.a);
		o.Smoothness = _SpatterSmoothness;
#ifdef TRANS
		o.Alpha = 1;
#endif
	}
	else
	{
		fixed3 albedo = c.rgb < 0.5 ? (2.0 * c.rgb * IN.color.rgb) : (1.0 - 2.0 * (1.0 - c.rgb) * (1.0 - IN.color.rgb));
        o.Albedo = albedo *(1 - special.g);
		o.Metallic = (1.0 - IN.color.a) + special.r;
		o.Smoothness = c.a;
		o.Emission = albedo * special.g;
	}
	o.Occlusion = bump.r;
}

#endif