Shader "Custom/ArmokStandardPBRCutout_Array" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGB)", 2DArray) = "grey" {}
		_BumpMap ("Normalmap (RGB) Occlusion (A)", 2DArray) = "bump" {}
        _SpecialTex("Metallic (R)", 2DArray) = "black" {}
		[PerRendererData]_SpatterTex("Spatter", 2D) = "" {}
		_SpatterDirection("Spatter Direction", Vector) = (0,1,0)
		_SpatterSmoothness("Spatter Smoothness", Range(0,1)) = 0
		_WorldBounds("World Bounds", Vector) = (0,0,1,1)
		_SpatterNoise("Spatter Noise", 2D) = "white" {}

		_Cutoff ("Alpha cutoff", Range(0,1)) = 0.5
            _TexArrayCount("Texture array count", Vector) = (0,0,0,0)
    }
	SubShader {
		Tags { "Queue"="AlphaTest" "IgnoreProjector"="True" "RenderType"="TransparentCutout" }
		LOD 200
		
		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Standard addshadow alphatest:_Cutoff
        #pragma target 3.5
        #pragma shader_feature CONTAMINANTS

		#include "ArmokStandardShared_Array.cginc"

		ENDCG
	} 
	FallBack "Custom/ArmokStandardPBRCutoutNoSpatter"
            CustomEditor "GroundSplatEditor"
}
