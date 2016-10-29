Shader "Custom/ItemParticles" {
	Properties {
        _Color("Color", Color) = (1,1,1,1)
        _MainTex("Albedo (RGB)", 2D) = "grey" {}
        _BumpMap("Normalmap (RGB) Occlusion (A)", 2D) = "bump" {}
        _SpecialTex("Metallic (R)", 2D) = "black" {}

        _Cutoff("Alpha cutoff", Range(0,1)) = 0.5
    }
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Standard fullforwardshadows alphatest:_Cutoff

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

        sampler2D _MainTex;
        sampler2D _BumpMap;
        sampler2D _SpecialTex;

		struct Input {
            float2 uv_MainTex;
            float2 uv_BumpMap;
            float2 uv_SpecialTex;
            float4 color: Color; // Vertex color
        };

        fixed4 _Color;

		void surf (Input IN, inout SurfaceOutputStandard o) {
            // Albedo comes from a texture tinted by color
            fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
            fixed4 bump = tex2D(_BumpMap, IN.uv_BumpMap);
            fixed4 special = tex2D(_SpecialTex, IN.uv_SpecialTex);
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
		ENDCG
	}
	FallBack "Diffuse"
}
