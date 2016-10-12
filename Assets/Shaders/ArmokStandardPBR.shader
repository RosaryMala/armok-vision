Shader "Custom/ArmokStandardPBR" {
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
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Standard
        #pragma target 3.5

		#include "ArmokStandardShared.cginc"

		ENDCG
	} 
	FallBack "Custom/ArmokStandardPBRNoSpatter"
}
