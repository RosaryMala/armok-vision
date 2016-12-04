Shader "Custom/Ground Splat" {
	Properties {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0

        [PerRendererData]_SpatterTex("Spatter", 2D) = "" {}
        _SpatterDirection("Spatter Direction", Vector) = (0,1,0)
        _SpatterSmoothness("Spatter Smoothness", Range(0,1)) = 0
        _WorldBounds("World Bounds", Vector) = (0,0,1,1)
        _SpatterNoise("Spatter Noise", 2D) = "white" {}
    }
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Standard fullforwardshadows

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.5

		sampler2D _MainTex;

        sampler2D _SpatterTex;
        sampler2D _SpatterNoise;
        float4 _SpatterDirection;
        float _SpatterSmoothness;
        float4 _WorldBounds;
        float4 _SpatterNoise_ST;


		struct Input {
			float2 uv_MainTex;
            float4 color: Color; // Vertex color
            float3 worldNormal;
            float3 worldPos;
            INTERNAL_DATA
        };

		half _Glossiness;
		half _Metallic;
		fixed4 _Color;


        void surf(Input IN, inout SurfaceOutputStandard o) {

            fixed4 spatter = tex2D(_SpatterTex, (IN.worldPos.xz - _WorldBounds.xy) / (_WorldBounds.zw - _WorldBounds.xy));
            fixed4 noise = tex2D(_SpatterNoise, TRANSFORM_TEX(IN.worldPos.xz, _SpatterNoise));

            // Albedo comes from a texture tinted by color
            fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;

            o.Alpha = c.a;

            if (dot(WorldNormalVector(IN, o.Normal), _SpatterDirection.xyz) >= lerp(1, -1, (spatter.a - noise.r)))
            {
                o.Albedo = (spatter.rgb / spatter.a);
                o.Smoothness = _SpatterSmoothness;
                o.Metallic = 0;
            }
            else
            {
                o.Albedo = c.rgb;
                // Metallic and smoothness come from slider variables
                o.Metallic = _Metallic;
                o.Smoothness = _Glossiness;
            }
		}
		ENDCG
	}
	FallBack "Diffuse"
}
