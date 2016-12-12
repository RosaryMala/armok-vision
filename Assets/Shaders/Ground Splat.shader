Shader "Custom/Ground Splat" {
	Properties {
        _MainTex("Albedo (RGB)", 2DArray) = "grey" {}
        _BumpMap("Shape Texture Splat", 2DArray) = "bump" {}
        [PerRendererData]_Tint("Tint (RGBA)", 2D) = "black" {}
        [PerRendererData]_Control("Control (RG)", 2D) = "black" {}

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

        #pragma shader_feature CONTAMINANTS

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.5

        sampler2D _Control;
        float4 _Control_TexelSize;
        sampler2D _Tint;
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

            float4 a_c = overlay(UNITY_SAMPLE_TEX2DARRAY(_MainTex, float3(IN.uv_MainTex, a_cont.x)), tex2D(_Tint, (controlCoordsBase + float2(0, 0)) * _Control_TexelSize.xy));
            float4 b_c = overlay(UNITY_SAMPLE_TEX2DARRAY(_MainTex, float3(IN.uv_MainTex, b_cont.x)), tex2D(_Tint, (controlCoordsBase + float2(1, 0)) * _Control_TexelSize.xy));
            float4 c_c = overlay(UNITY_SAMPLE_TEX2DARRAY(_MainTex, float3(IN.uv_MainTex, c_cont.x)), tex2D(_Tint, (controlCoordsBase + float2(0, 1)) * _Control_TexelSize.xy));
            float4 d_c = overlay(UNITY_SAMPLE_TEX2DARRAY(_MainTex, float3(IN.uv_MainTex, d_cont.x)), tex2D(_Tint, (controlCoordsBase + float2(1, 1)) * _Control_TexelSize.xy));

            float4 a_n = UNITY_SAMPLE_TEX2DARRAY(_BumpMap, float3(IN.uv_MainTex, a_cont.y));
            float4 b_n = UNITY_SAMPLE_TEX2DARRAY(_BumpMap, float3(IN.uv_MainTex, b_cont.y));
            float4 c_n = UNITY_SAMPLE_TEX2DARRAY(_BumpMap, float3(IN.uv_MainTex, c_cont.y));
            float4 d_n = UNITY_SAMPLE_TEX2DARRAY(_BumpMap, float3(IN.uv_MainTex, d_cont.y));

            float4 ab_c = MixColor(a_c, a_n.b, 1 - controlFraction.x, b_c, b_n.b, controlFraction.x);
            float4 ab_n = MixColor(a_n, a_n.b, 1 - controlFraction.x, b_n, b_n.b, controlFraction.x);
 
            float4 cd_c = MixColor(c_c, c_n.b, 1 - controlFraction.x, d_c, d_n.b, controlFraction.x);
            float4 cd_n = MixColor(c_n, c_n.b, 1 - controlFraction.x, d_n, d_n.b, controlFraction.x);

            float4 abcd_c = MixColor(ab_c, ab_n.b, 1 - controlFraction.y, cd_c, cd_n.b, controlFraction.y);
            float4 abcd_n = MixColor(ab_n, ab_n.b, 1 - controlFraction.y, cd_n, cd_n.b, controlFraction.y);


            o.Normal = UnpackNormal(abcd_n.ggga);

#ifdef CONTAMINANTS
            if (dot(WorldNormalVector(IN, o.Normal), _SpatterDirection.xyz) >= lerp(1, -1, (spatter.a - noise.r)))
            {
                o.Albedo = (spatter.rgb / spatter.a);
                o.Smoothness = _SpatterSmoothness;
                o.Metallic = 0;
            }
            else
            {
#endif
                o.Albedo = abcd_c.rgb;
                o.Smoothness = abcd_c.a;
#ifdef CONTAMINANTS
            }
#endif
            o.Occlusion = abcd_n.r;
		}
		ENDCG
	}
	FallBack "Diffuse"
    CustomEditor "GroundSplatEditor"
}
