#ifndef ARMOK_STANDARD_SHARED
#define ARMOK_STANDARD_SHARED

sampler2D _MainTex;
sampler2D _BumpMap;
sampler2D _RoughGlossMap;
sampler2D _MetalGlossMap;
sampler2D _SpatterTex;
sampler2D _SpatterNoise;
float4 _SpatterDirection;
float _SpatterSmoothness;
float4 _WorldBounds;
float4 _SpatterNoise_ST;
float _Rough;
float _Metal;

struct Input {
	float2 uv_MainTex;
	float2 uv2_BumpMap;
	float2 uv3_MetalGlossMap;
	float4 color: Color; // Vertex color
	float3 worldNormal;
	float3 worldPos;
	INTERNAL_DATA
};

half _Glossiness;
fixed4 _Color;

void surf(Input IN, inout SurfaceOutputStandard o) {
	// Albedo comes from a texture tinted by color
	fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
	fixed4 bump = tex2D(_BumpMap, IN.uv2_BumpMap);
	fixed4 metal = tex2D(_MetalGlossMap, IN.uv3_MetalGlossMap);
	fixed4 spatter = tex2D(_SpatterTex, (IN.worldPos.xz - _WorldBounds.xy) / (_WorldBounds.zw - _WorldBounds.xy));
	fixed4 noise = tex2D(_SpatterNoise, TRANSFORM_TEX(IN.worldPos.xz, _SpatterNoise));
	//o.Albedo = c.rgb * IN.color.rgb;
	o.Normal = UnpackNormal(fixed4(1, bump.g, 1, bump.a));
	o.Alpha = bump.b;
	if (dot(WorldNormalVector(IN, o.Normal), _SpatterDirection.xyz) >= lerp(1, -1, (spatter.a - noise.r)))
	{
		o.Albedo = (spatter.rgb / spatter.a);
		o.Smoothness = _SpatterSmoothness * _Rough;
#ifdef TRANS
		o.Alpha = 1;
#endif
	}
	else
	{
        fixed3 albedo = c.rgb * IN.color.rgb;
		o.Albedo = albedo *(1 - metal.g);
		o.Metallic = (1.0 - IN.color.a) + metal.r * _Metal;
		o.Smoothness = tex2D(_RoughGlossMap, IN.uv_MainTex).r * _Rough;
		o.Emission = albedo * metal.g;
	}
	o.Occlusion = bump.r;
}

#endif