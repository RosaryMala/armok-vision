﻿Shader "Custom/ArmokStandardPBRNoSpatter" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGB)", 2D) = "grey" {}
		_BumpMap ("Normalmap (RGB) Occlusion (A)", 2D) = "bump" {}
        _MetalGlossMap("Metallic", 2D) = "white" {}
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 300
		
		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Standard

		#include "ArmokStandardSharedNoSpatter.cginc"

		ENDCG
	} 
	FallBack "Diffuse"
}
