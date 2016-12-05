Shader "Custom/Ground Splat" {
	Properties {
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _Control("Control (RGBA)", 2D) = "red" {}

        _Color4("Color 4", Color) = (0.5,0.5,0.5,1)
        _Color3("Color 3", Color) = (0.5,0.5,0.5,1)
        _Color2("Color 2", Color) = (0.5,0.5,0.5,1)
        _Color1("Color 1", Color) = (0.5,0.5,0.5,1)
        _Color0("Color 0", Color) = (0.5,0.5,0.5,1)


        _Splat4("Layer 4 (A)", 2D) = "red" {}
        _Splat3("Layer 3 (B)", 2D) = "red" {}
        _Splat2("Layer 2 (G)", 2D) = "red" {}
        _Splat1("Layer 1 (R)", 2D) = "red" {}
        _Splat0("Layer 0 (Base)", 2D) = "red" {}
        _Normal4("Normal 4 (A)", 2D) = "red" {}
        _Normal3("Normal 3 (B)", 2D) = "red" {}
        _Normal2("Normal 2 (G)", 2D) = "red" {}
        _Normal1("Normal 1 (R)", 2D) = "red" {}
        _Normal0("Normal 0 (Base)", 2D) = "red" {}

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

        sampler2D _Control;

        sampler2D _Splat0, _Splat1, _Splat2, _Splat3, _Splat4;
        sampler2D _Normal0, _Normal1, _Normal2, _Normal3, _Normal4;
        fixed4 _Color0, _Color1, _Color2, _Color3, _Color4;

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


#include "blend.cginc"

        fixed4 MixColor(fixed4 bottom, inout fixed4 bottom_c, fixed4 top, fixed4 top_c, fixed top_alpha)
        {
            top = fixed4(top.rg, max(top.b + top_alpha - 1, 0), top.a);
            //bottom = fixed4(bottom.rg, max(bottom.b - top_alpha, 0), bottom.a);
            //crappy blending to test
            bottom_c = (top.b) > (bottom.b) ? top_c : bottom_c;
            return (top.b) > (bottom.b) ? top : bottom;
        }

        void surf(Input IN, inout SurfaceOutputStandard o) {

            fixed4 spatter = tex2D(_SpatterTex, (IN.worldPos.xz - _WorldBounds.xy) / (_WorldBounds.zw - _WorldBounds.xy));
            fixed4 noise = tex2D(_SpatterNoise, TRANSFORM_TEX(IN.worldPos.xz, _SpatterNoise));
            fixed4 control = tex2D(_Control, (IN.worldPos.xz - _WorldBounds.xy) / (_WorldBounds.zw - _WorldBounds.xy));

            // Albedo comes from a texture tinted by color
            fixed4 c = overlay(tex2D(_Splat0, IN.uv_MainTex), _Color0.rgb);
            fixed4 n = tex2D(_Normal0, IN.uv_MainTex);
            n.b = max(n.b - (control.r + control.g + control.b + control.a), 0);

            n = MixColor(n, c, tex2D(_Normal1, IN.uv_MainTex), overlay(tex2D(_Splat1, IN.uv_MainTex), _Color1.rgb), control.r);
            n = MixColor(n, c, tex2D(_Normal2, IN.uv_MainTex), overlay(tex2D(_Splat2, IN.uv_MainTex), _Color2.rgb), control.g);
            n = MixColor(n, c, tex2D(_Normal3, IN.uv_MainTex), overlay(tex2D(_Splat3, IN.uv_MainTex), _Color3.rgb), control.b);
            n = MixColor(n, c, tex2D(_Normal4, IN.uv_MainTex), overlay(tex2D(_Splat4, IN.uv_MainTex), _Color4.rgb), control.a);

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
                o.Smoothness = 0.5;// c.a;

            }
            o.Occlusion = n.r;

		}
		ENDCG
	}
	FallBack "Diffuse"
}
