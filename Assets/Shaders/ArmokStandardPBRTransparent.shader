Shader "Custom/ArmokStandardPBRTransparent" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGB)", 2D) = "grey" {}
		_BumpMap ("Normalmap (RGB) Occlusion (A)", 2D) = "bump" {}
        _SpecialTex("Metallic (R)", 2D) = "black" {}
		[PerRendererData]_SpatterTex("Spatter", 2D) = "" {}
		_SpatterDirection("Spatter Direction", Vector) = (0,1,0)
		_SpatterSmoothness("Spatter Smoothness", Range(0,1)) = 0
		_WorldBounds("World Bounds", Vector) = (0,0,1,1)
		_SpatterNoise("Spatter Noise", 2D) = "white" {}
	}
	SubShader {
		Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}
		LOD 200	

        // extra pass that renders to depth buffer only
        Pass{
            ZWrite On
            ColorMask 0
        }
			
		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Standard alpha
		#define TRANS
		#include "ArmokStandardShared.cginc"

		ENDCG
	} 
	FallBack "Custom/ArmokStandardPBRTransparentNoSpatter"
}
