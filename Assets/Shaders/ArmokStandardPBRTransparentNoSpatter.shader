Shader "Custom/ArmokStandardPBRTransparentNoSpatter" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGB)", 2D) = "grey" {}
		_BumpMap ("Normalmap (RGB) Occlusion (A)", 2D) = "bump" {}
        _SpecialTex("Metallic (R)", 2D) = "black" {}
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
		#include "ArmokStandardSharedNoSpatter.cginc"

		ENDCG
	} 
	FallBack "Transparent/Diffuse"
}
