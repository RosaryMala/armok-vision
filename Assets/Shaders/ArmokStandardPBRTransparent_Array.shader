Shader "Custom/ArmokStandardPBRTransparent_Array" {
	Properties {
		_BumpMap ("Normalmap (RGB) Occlusion (A)", 2DArray) = "bump" {}
        _MetalGlossMap("Metallic", 2DArray) = "white" {}
		[PerRendererData]_SpatterTex("Spatter", 2D) = "" {}
		_SpatterDirection("Spatter Direction", Vector) = (0,1,0)
		_SpatterSmoothness("Spatter Smoothness", Range(0,1)) = 0
		_WorldBounds("World Bounds", Vector) = (0,0,1,1)
		_SpatterNoise("Spatter Noise", 2D) = "white" {}
        _TexArrayCount("Texture array count", Vector) = (0,0,0,0)
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
        #pragma target 3.5
        #define TRANS
        #pragma multi_compile __ CONTAMINANTS

		#include "ArmokStandardShared_Array.cginc"

		ENDCG
	} 
	FallBack "Unlit/TerrainSplatShadowFallback"
    CustomEditor "GroundSplatEditor"
}
