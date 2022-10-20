Shader "Custom/ArmokStandardPBR_Array" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_BumpMap ("Normalmap (RGB) Occlusion (A)", 2DArray) = "bump" {}
		_Rough("Roughness", Range(0.0, 1.0)) = 0.5
		_Metal("Metallic", Range(0.0, 1.0)) = 0.0
        _MetalGlossMap("Metallic", 2DArray) = "white" {}
		_RoughnessMap("Roughness", 2DArray) = "white" {}
		[PerRendererData]_SpatterTex("Spatter", 2D) = "" {}
		_SpatterDirection("Spatter Direction", Vector) = (0,1,0)
		_SpatterSmoothness("Spatter Smoothness", Range(0,1)) = 0
		_WorldBounds("World Bounds", Vector) = (0,0,1,1)
		_SpatterNoise("Spatter Noise", 2D) = "white" {}
        _TexArrayCount("Texture array count", Vector) = (0,0,0,0)
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Standard
        #pragma target 3.5
        #pragma multi_compile __ CONTAMINANTS

		#include "ArmokStandardShared_Array.cginc"

		ENDCG
	} 
	FallBack "Custom/ArmokStandardPBRNoSpatter"
            CustomEditor "GroundSplatEditor"
}
