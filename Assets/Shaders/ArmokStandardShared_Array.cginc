#ifndef ARMOK_STANDARD_SHARED_ARRAY
#define ARMOK_STANDARD_SHARED_ARRAY

UNITY_DECLARE_TEX2DARRAY(_MatTexArray);
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
	float2 uv_MatTexArray;
	float2 uv2_BumpMap;
	float2 uv3_SpecialTex;
	float4 color: Color; // Vertex color
	float3 worldNormal;
	float3 worldPos;
	INTERNAL_DATA
};


void surf(Input IN, inout SurfaceOutputStandard o) {
	// Albedo comes from a texture tinted by color
	fixed4 c = UNITY_SAMPLE_TEX2DARRAY(_MatTexArray, float3(IN.uv_MatTexArray.xy, IN.uv2_BumpMap.x));
	fixed4 bump = UNITY_SAMPLE_TEX2DARRAY(_BumpMap, float3(IN.uv_MatTexArray.xy, IN.uv2_BumpMap.y));
	fixed4 spatter = tex2D(_SpatterTex, (IN.worldPos.xz - _WorldBounds.xy) / (_WorldBounds.zw - _WorldBounds.xy));
	fixed4 noise = tex2D(_SpatterNoise, TRANSFORM_TEX(IN.worldPos.xz, _SpatterNoise));
	//o.Albedo = c.rgb * IN.color.rgb;
	o.Normal = UnpackNormal(fixed4(1, bump.g, 1, bump.a));
	o.Alpha = min((IN.color.a * 2), 1) *bump.b;
    o.Metallic = max((IN.color.a * 2) - 1, 0);
#ifdef CONTAMINANTS
    if (dot(WorldNormalVector(IN, o.Normal), _SpatterDirection.xyz) >= lerp(1, -1, (spatter.a - noise.r)))
	{
		o.Albedo = (spatter.rgb / spatter.a);
		o.Smoothness = _SpatterSmoothness;
        o.Metallic = 0;
#ifdef TRANS
		o.Alpha = 1;
#endif
	}
	else
#endif
    {
		fixed3 albedo = c.rgb * IN.color.rgb;
        o.Albedo = albedo;
		o.Smoothness = c.a;
	}
	o.Occlusion = bump.r;
}

#endif