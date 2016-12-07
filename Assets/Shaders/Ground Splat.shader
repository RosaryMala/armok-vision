Shader "Custom/Ground Splat" {
	Properties {
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _Control("Control (RGBA)", 2DArray) = "black" {}
        _Splat("Albedo Splat", 2DArray) = "black" {}
        _Normal("Shape Texture Splat", 2DArray) = "bump" {}
        _LayerCount("texture splat layers", Int) = 1

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

        UNITY_DECLARE_TEX2DARRAY(_Control);
        UNITY_DECLARE_TEX2DARRAY(_Splat);
        UNITY_DECLARE_TEX2DARRAY(_Normal);

        sampler2D _MainTex;

        sampler2D _SpatterTex;
        sampler2D _SpatterNoise;
        float4 _SpatterDirection;
        float _SpatterSmoothness;
        float4 _WorldBounds;
        float4 _SpatterNoise_ST;
        float _LayerCount;
        float4 _ArrayIndices[32];

		struct Input {
			float2 uv_MainTex;
            float4 color: Color; // Vertex color
            float3 worldNormal;
            float3 worldPos;
            INTERNAL_DATA
        };


#include "blend.cginc"

        fixed4 MixColor(fixed4 bottom, inout fixed4 bottom_c, fixed4 top, fixed4 top_c, fixed top_alpha)
        {
            fixed depth = 0.2;
            fixed ma = max(top.b + top_alpha, bottom.b + 1 - top_alpha) - depth;

            fixed b1 = max(top.b + top_alpha - ma, 0);
            fixed b2 = max(bottom.b + 1 - top_alpha - ma, 0);

            bottom_c = (top_c * b1 + bottom_c * b2) / (b1 + b2);
            return (top * b1 + bottom * b2) / (b1 + b2);
        }

        void surf(Input IN, inout SurfaceOutputStandard o) {

            fixed4 spatter = tex2D(_SpatterTex, (IN.worldPos.xz - _WorldBounds.xy) / (_WorldBounds.zw - _WorldBounds.xy));
            fixed4 noise = tex2D(_SpatterNoise, TRANSFORM_TEX(IN.worldPos.xz, _SpatterNoise));
            fixed4 control = UNITY_SAMPLE_TEX2DARRAY(_Control, float3((IN.worldPos.xz - _WorldBounds.xy) / (_WorldBounds.zw - _WorldBounds.xy), 0));

            // Albedo comes from a texture tinted by color
            fixed4 c = overlay(UNITY_SAMPLE_TEX2DARRAY(_Splat, float3(IN.uv_MainTex, 0)), control.rgb);
            fixed4 n = UNITY_SAMPLE_TEX2DARRAY(_Normal, float3(IN.uv_MainTex, 0));
            n.b = max(n.b - control.a, 0);

            for (int i = 1; i < _LayerCount; i++)
            {
                fixed4 cont = UNITY_SAMPLE_TEX2DARRAY(_Control, float3((IN.worldPos.xz - _WorldBounds.xy) / (_WorldBounds.zw - _WorldBounds.xy), i));
                n = MixColor(n, c, UNITY_SAMPLE_TEX2DARRAY(_Normal, float3(IN.uv_MainTex, _ArrayIndices[i].y)), overlay(UNITY_SAMPLE_TEX2DARRAY(_Splat, float3(IN.uv_MainTex, _ArrayIndices[i].x)), cont.rgb), cont.a);
            }

            o.Normal = UnpackNormal(n.ggga);

            if (dot(WorldNormalVector(IN, o.Normal), _SpatterDirection.xyz) >= lerp(1, -1, (spatter.a - noise.r)))
            {
                o.Albedo = (spatter.rgb / spatter.a);
                o.Smoothness = _SpatterSmoothness;
                o.Metallic = 0;
            }
            else
            {
                o.Albedo = c.rgb;
                o.Smoothness = 0.25;// c.a;

            }
            o.Occlusion = n.r;

		}
		ENDCG
	}
	FallBack "Diffuse"
}
