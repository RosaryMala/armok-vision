Shader "Custom/Ground Splat" {
	Properties {
        _MainTex("Albedo (RGB)", 2DArray) = "grey" {}
        _BumpMap("Shape Texture Splat", 2DArray) = "bump" {}
        [PerRendererData]_Tint("Tint (RGBA)", 2D) = "black" {}
        [PerRendererData]_Control("Control (RG)", 2D) = "black" {}
 
        [PerRendererData]_GrassTint("Tint (RGBA)", 2D) = "black" {}
        [PerRendererData]_GrassControl("Control (RG)", 2D) = "black" {}

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

#pragma multi_compile __ CONTAMINANTS
#pragma multi_compile __ GRASS

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.5

        sampler2D _Control;
        sampler2D _GrassControl;
        float4 _Control_TexelSize;
        sampler2D _Tint;
        sampler2D _GrassTint;
        UNITY_DECLARE_TEX2DARRAY(_MainTex);
        UNITY_DECLARE_TEX2DARRAY(_BumpMap);

#ifdef CONTAMINANTS
        sampler2D _SpatterTex;
        sampler2D _SpatterNoise;
        float4 _SpatterDirection;
        float _SpatterSmoothness;
        float4 _SpatterNoise_ST;
#endif
        float4 _WorldBounds;

		struct Input {
			float2 uv_MainTex;
            float4 color: Color; // Vertex color
            float3 worldNormal;
            float3 worldPos;
            INTERNAL_DATA
        };


#include "blend.cginc"
        float4 MixColor(float4 texture1, float height1, float a1, float4 texture2, float height2, float a2)
        {
            float depth = 0.2;
            float ma = max(height1 + a1, height2 + a2) - depth;

            float b1 = max(height1 + a1 - ma, 0);
            float b2 = max(height2 + a2 - ma, 0);

            return (texture1 * b1 + texture2 * b2) / (b1 + b2);
        }

        float4 MixColorSharp(float4 texture1, float height1, float a1, float4 texture2, float height2, float a2)
        {
            return height1 + a1 > height2 + a2 ? texture1 : texture2;
        }



        void surf(Input IN, inout SurfaceOutputStandard o) {
            float2 controlUV = (IN.worldPos.xz - _WorldBounds.xy) / (_WorldBounds.zw - _WorldBounds.xy);

#ifdef CONTAMINANTS
            fixed4 spatter = tex2D(_SpatterTex, controlUV);
            fixed4 noise = tex2D(_SpatterNoise, TRANSFORM_TEX(IN.worldPos.xz, _SpatterNoise));
#endif
            
            float2 controlCoords = controlUV * _Control_TexelSize.zw;
            float2 controlCoordsBase = floor(controlCoords + float2(0.5, 0.5)) - float2(0.5, 0.5);
            float2 controlFraction = controlCoords - controlCoordsBase;

            float2 a_cont = tex2D(_Control, (controlCoordsBase + float2(0, 0)) * _Control_TexelSize.xy);
            float2 b_cont = tex2D(_Control, (controlCoordsBase + float2(1, 0)) * _Control_TexelSize.xy);
            float2 c_cont = tex2D(_Control, (controlCoordsBase + float2(0, 1)) * _Control_TexelSize.xy);
            float2 d_cont = tex2D(_Control, (controlCoordsBase + float2(1, 1)) * _Control_TexelSize.xy);

            float4 a_tint = tex2D(_Tint, (controlCoordsBase + float2(0, 0)) * _Control_TexelSize.xy);
            float4 b_tint = tex2D(_Tint, (controlCoordsBase + float2(1, 0)) * _Control_TexelSize.xy);
            float4 c_tint = tex2D(_Tint, (controlCoordsBase + float2(0, 1)) * _Control_TexelSize.xy);
            float4 d_tint = tex2D(_Tint, (controlCoordsBase + float2(1, 1)) * _Control_TexelSize.xy);

            float4 a_c = UNITY_SAMPLE_TEX2DARRAY(_MainTex, float3(IN.uv_MainTex, a_cont.x));
            float4 b_c = UNITY_SAMPLE_TEX2DARRAY(_MainTex, float3(IN.uv_MainTex, b_cont.x));
            float4 c_c = UNITY_SAMPLE_TEX2DARRAY(_MainTex, float3(IN.uv_MainTex, c_cont.x));
            float4 d_c = UNITY_SAMPLE_TEX2DARRAY(_MainTex, float3(IN.uv_MainTex, d_cont.x));

            float4 a_n = UNITY_SAMPLE_TEX2DARRAY(_BumpMap, float3(IN.uv_MainTex, a_cont.y));
            float4 b_n = UNITY_SAMPLE_TEX2DARRAY(_BumpMap, float3(IN.uv_MainTex, b_cont.y));
            float4 c_n = UNITY_SAMPLE_TEX2DARRAY(_BumpMap, float3(IN.uv_MainTex, c_cont.y));
            float4 d_n = UNITY_SAMPLE_TEX2DARRAY(_BumpMap, float3(IN.uv_MainTex, d_cont.y));

#ifdef GRASS
            float2 a_grass_cont = tex2D(_GrassControl, (controlCoordsBase + float2(0, 0)) * _Control_TexelSize.xy);
            float2 b_grass_cont = tex2D(_GrassControl, (controlCoordsBase + float2(1, 0)) * _Control_TexelSize.xy);
            float2 c_grass_cont = tex2D(_GrassControl, (controlCoordsBase + float2(0, 1)) * _Control_TexelSize.xy);
            float2 d_grass_cont = tex2D(_GrassControl, (controlCoordsBase + float2(1, 1)) * _Control_TexelSize.xy);

            float4 a_grass_tint = tex2D(_GrassTint, (controlCoordsBase + float2(0, 0)) * _Control_TexelSize.xy);
            float4 b_grass_tint = tex2D(_GrassTint, (controlCoordsBase + float2(1, 0)) * _Control_TexelSize.xy);
            float4 c_grass_tint = tex2D(_GrassTint, (controlCoordsBase + float2(0, 1)) * _Control_TexelSize.xy);
            float4 d_grass_tint = tex2D(_GrassTint, (controlCoordsBase + float2(1, 1)) * _Control_TexelSize.xy);

            float4 a_grass_c = UNITY_SAMPLE_TEX2DARRAY(_MainTex, float3(IN.uv_MainTex, a_grass_cont.x));
            float4 b_grass_c = UNITY_SAMPLE_TEX2DARRAY(_MainTex, float3(IN.uv_MainTex, b_grass_cont.x));
            float4 c_grass_c = UNITY_SAMPLE_TEX2DARRAY(_MainTex, float3(IN.uv_MainTex, c_grass_cont.x));
            float4 d_grass_c = UNITY_SAMPLE_TEX2DARRAY(_MainTex, float3(IN.uv_MainTex, d_grass_cont.x));

            float4 a_grass_n = UNITY_SAMPLE_TEX2DARRAY(_BumpMap, float3(IN.uv_MainTex, a_grass_cont.y));
            float4 b_grass_n = UNITY_SAMPLE_TEX2DARRAY(_BumpMap, float3(IN.uv_MainTex, b_grass_cont.y));
            float4 c_grass_n = UNITY_SAMPLE_TEX2DARRAY(_BumpMap, float3(IN.uv_MainTex, c_grass_cont.y));
            float4 d_grass_n = UNITY_SAMPLE_TEX2DARRAY(_BumpMap, float3(IN.uv_MainTex, d_grass_cont.y));

            if (a_grass_tint.a > 0)
            {
                a_c = MixColor(a_grass_c, a_grass_n.b, a_grass_tint.a, a_c, a_n.b, 1 - a_grass_n.b);
                a_tint = MixColor(a_grass_tint, a_grass_n.b, a_grass_tint.a, a_tint, a_n.b, 1 - a_grass_n.b);
                a_n = MixColor(a_grass_n, a_grass_n.b, a_grass_tint.a, a_n, a_n.b, 1 - a_grass_n.b);
            }
            if (b_grass_tint.a > 0)
            {
                b_c = MixColor(b_grass_c, b_grass_n.b, b_grass_tint.a, b_c, b_n.b, 1 - b_grass_n.b);
                b_tint = MixColor(b_grass_tint, b_grass_n.b, b_grass_tint.a, b_tint, b_n.b, 1 - b_grass_n.b);
                b_n = MixColor(b_grass_n, b_grass_n.b, b_grass_tint.a, b_n, b_n.b, 1 - b_grass_n.b);
            }
            if (c_grass_tint.a > 0)
            {
                c_c = MixColor(c_grass_c, c_grass_n.b, c_grass_tint.a, c_c, c_n.b, 1 - c_grass_n.b);
                c_tint = MixColor(c_grass_tint, c_grass_n.b, c_grass_tint.a, c_tint, c_n.b, 1 - c_grass_n.b);
                c_n = MixColor(c_grass_n, c_grass_n.b, c_grass_tint.a, c_n, c_n.b, 1 - c_grass_n.b);
            }
            if (d_grass_tint.a > 0)
            {
                d_c = MixColor(d_grass_c, d_grass_n.b, d_grass_tint.a, d_c, d_n.b, 1 - d_grass_n.b);
                d_tint = MixColor(d_grass_tint, d_grass_n.b, d_grass_tint.a, d_tint, d_n.b, 1 - d_grass_n.b);
                d_n = MixColor(d_grass_n, d_grass_n.b, d_grass_tint.a, d_n, d_n.b, 1 - d_grass_n.b);
            }
#endif
            float4 ab_c = MixColor(a_c, a_n.b, 1 - controlFraction.x, b_c, b_n.b, controlFraction.x);
            float4 ab_n = MixColor(a_n, a_n.b, 1 - controlFraction.x, b_n, b_n.b, controlFraction.x);
            float4 ab_tint = MixColor(a_tint, a_n.b, 1 - controlFraction.x, b_tint, b_n.b, controlFraction.x);

            float4 cd_c = MixColor(c_c, c_n.b, 1 - controlFraction.x, d_c, d_n.b, controlFraction.x);
            float4 cd_n = MixColor(c_n, c_n.b, 1 - controlFraction.x, d_n, d_n.b, controlFraction.x);
            float4 cd_tint = MixColor(c_tint, c_n.b, 1 - controlFraction.x, d_tint, d_n.b, controlFraction.x);

            float4 abcd_c = MixColor(ab_c, ab_n.b, 1 - controlFraction.y, cd_c, cd_n.b, controlFraction.y);
            float4 abcd_n = MixColor(ab_n, ab_n.b, 1 - controlFraction.y, cd_n, cd_n.b, controlFraction.y);
            float4 abcd_tint = MixColor(ab_tint, ab_n.b, 1 - controlFraction.y, cd_tint, cd_n.b, controlFraction.y);

            o.Normal = UnpackNormal(abcd_n.ggga);

#ifdef CONTAMINANTS
            if (dot(WorldNormalVector(IN, o.Normal), _SpatterDirection.xyz) >= lerp(1, -1, (spatter.a - noise.r)))
            {
                o.Albedo = (spatter.rgb / spatter.a);
                o.Smoothness = _SpatterSmoothness;
                o.Metallic = 0;
            }
            else
#endif
            {
                o.Albedo = overlay(abcd_c.rgb, abcd_tint.rgb);
                o.Smoothness = abcd_c.a;
                o.Metallic = 1 - abcd_tint.a;
            }
            o.Occlusion = abcd_n.r;
        }
		ENDCG
	}
	FallBack "Diffuse"
    CustomEditor "GroundSplatEditor"
}
