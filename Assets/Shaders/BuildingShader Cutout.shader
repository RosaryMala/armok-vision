Shader "Building/Cutout" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}

        _Cutoff("Alpha Cutoff", Range(0.0, 1.0)) = 0.5

        _Glossiness("Smoothness", Range(0.0, 1.0)) = 0.5
        _GlossMapScale("Smoothness Scale", Range(0.0, 1.0)) = 1.0
            
        [Gamma] _Metallic("Metallic", Range(0.0, 1.0)) = 0.0
        _MetallicGlossMap("Metallic", 2D) = "white" {}

        _BumpScale("Scale", Float) = 1.0
        _BumpMap("Normal Map", 2D) = "bump" {}

        _OcclusionStrength("Strength", Range(0.0, 1.0)) = 1.0
        _OcclusionMap("Occlusion", 2D) = "white" {}

        _EmissionColor("Color", Color) = (0,0,0)
        _EmissionMap("Emission", 2D) = "white" {}

        _DFMask("DF Material Splat", 2D) = "black" {}
  
        [Enum(UV0,0,UV1,1)] _UVSec("UV Set for secondary textures", Float) = 0

        [PerRendererData] _MatColor("DF Material Color", Color) = (0.5,0.5,0.5,1)
        [PerRendererData] _MatIndex("DF Material Array Index", int) = 0

        // Blending state
        [HideInInspector] _Mode("__mode", Float) = 0.0
    }
	SubShader {
		Tags { "Queue" = "AlphaTest" "RenderType"="TransparentCutout" }
		LOD 200
		
		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Standard fullforwardshadows alphatest:_Cutoff

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 4.0

        #pragma shader_feature _NORMALMAP
        #pragma shader_feature _TEXTURE_MASK
        #pragma shader_feature _SECOND_UV
        #pragma shader_feature _EMISSION
        #pragma shader_feature _METALLICGLOSSMAP
#pragma multi_compile _ _BOUNDING_BOX_ENABLED

#include "buildingInputs.cginc"

		struct Input {
            float2 uv_MainTex;
#ifdef _SECOND_UV
            float2 uv2_MatTex;
#else
            float2 uv_MatTex;
#endif
            float3 worldPos;
      };

#include "blend.cginc"

        float4 TexCoords(Input IN)
        {
            float4 texcoord;
            texcoord.xy = IN.uv_MainTex; // Always source from uv0
#ifdef _SECOND_UV
            texcoord.zw = IN.uv2_MatTex;
#else
            texcoord.zw = IN.uv_MatTex;
#endif
            return texcoord;
        }

		void surf (Input IN, inout SurfaceOutputStandard o)
        {
#ifdef _BOUNDING_BOX_ENABLED
			clip(IN.worldPos - _ViewMin);
			clip(_ViewMax - IN.worldPos);
#endif

            float4 texcoords = TexCoords(IN);
            //get the mask 
            fixed4 dfTex = UNITY_SAMPLE_TEX2DARRAY(_MatTexArray, float3(texcoords.zw, UNITY_ACCESS_INSTANCED_PROP(_MatIndex)));
            fixed4 matColor = UNITY_ACCESS_INSTANCED_PROP(_MatColor);
            fixed3 albedo = overlay(dfTex.rgb, matColor.rgb);
            half smoothness = dfTex.a;
            half metallic = max((matColor.a * 2) - 1, 0);
            fixed alpha = 1;

#ifdef _TEXTURE_MASK
            fixed4 mask = tex2D(_DFMask, texcoords.xy);
            fixed4 c = tex2D(_MainTex, texcoords.xy) * _Color;
            fixed4 m = tex2D(_MetallicGlossMap, texcoords.xy);
            albedo = lerp(albedo, c.rgb, mask.r);
            albedo = lerp(albedo, overlay(c.rgb, matColor.rgb), max(mask.g - mask.r, 0));
            alpha = c.a;
#ifdef _METALLICGLOSSMAP
            metallic = lerp(metallic, m.r, mask.r);
            smoothness = lerp(smoothness, m.a * _GlossMapScale, mask.b);
#else
            metallic = lerp(metallic, _Metallic, mask.r);
            smoothness = lerp(smoothness, _Glossiness, mask.b);
#endif
#else
            albedo = overlay(albedo, matColor.rgb);
#endif


            //Actual output definitions
            o.Albedo = albedo;
#ifdef _NORMALMAP
            o.Normal = UnpackScaleNormal(tex2D(_BumpMap, texcoords.xy), _BumpScale);
#endif
#ifdef _EMISSION
            o.Emission = tex2D(_EmissionMap, texcoords.xy) * _EmissionColor;
#endif
            o.Metallic = metallic;
            o.Smoothness = smoothness;
            o.Occlusion = tex2D(_OcclusionMap, texcoords.xy) * _OcclusionStrength;
			o.Alpha = alpha;
        }
		ENDCG
	}
	FallBack "Diffuse"
    CustomEditor "BuildingMaterialEditor"

}
