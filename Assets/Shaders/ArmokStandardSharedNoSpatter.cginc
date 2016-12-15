#ifndef ARMOK_STANDARD_SHARED
#define ARMOK_STANDARD_SHARED

// Use shader model 3.0 target, to get nicer looking lighting
#pragma target 3.0

sampler2D _MainTex;
sampler2D _BumpMap;

struct Input {
	float2 uv_MainTex;
	float2 uv2_BumpMap;
	float4 color: Color; // Vertex color
};

half _Glossiness;
fixed4 _Color;

void surf(Input IN, inout SurfaceOutputStandard o) {
	// Albedo comes from a texture tinted by color
	fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
	fixed4 bump = tex2D(_BumpMap, IN.uv2_BumpMap);
	//o.Albedo = c.rgb * IN.color.rgb;
	o.Normal = UnpackNormal(bump.ggga);
	o.Alpha = bump.b;
	fixed3 albedo = c.rgb < 0.5 ? (2.0 * c.rgb * IN.color.rgb) : (1.0 - 2.0 * (1.0 - c.rgb) * (1.0 - IN.color.rgb));
	o.Albedo = albedo;
	o.Metallic = (1.0 - IN.color.a);
	o.Smoothness = c.a;
	o.Occlusion = bump.r;
}

#endif