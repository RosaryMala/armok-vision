#ifndef ARMOK_STANDARD_SHARED
#define ARMOK_STANDARD_SHARED

// Use shader model 3.0 target, to get nicer looking lighting
#pragma target 3.0

sampler2D _MainTex;
sampler2D _Shapetex;
sampler2D _BumpMap;
sampler2D _SpecialTex;
sampler2D _SpatterTex;

struct Input {
	float2 uv_MainTex;
	float2 uv2_BumpMap;
	float2 uv3_SpecialTex;
	float4 color: Color; // Vertex color
	INTERNAL_DATA
};

half _Glossiness;
fixed4 _Color;

void surf(Input IN, inout SurfaceOutputStandard o) {
	// Albedo comes from a texture tinted by color
	fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
	fixed4 bump = tex2D(_BumpMap, IN.uv2_BumpMap);
	fixed4 special = tex2D(_SpecialTex, IN.uv3_SpecialTex);
	//o.Albedo = c.rgb * IN.color.rgb;
	o.Normal = UnpackNormal(bump.ggga);
	o.Alpha = bump.b;
	fixed3 albedo = c.rgb < 0.5 ? (2.0 * c.rgb * IN.color.rgb) : (1.0 - 2.0 * (1.0 - c.rgb) * (1.0 - IN.color.rgb));
	o.Albedo = albedo *(1 - special.g);
	o.Metallic = (1.0 - IN.color.a) + special.r;
	o.Smoothness = c.a;
	o.Emission = albedo * special.g;
	o.Occlusion = bump.r;
}

#endif