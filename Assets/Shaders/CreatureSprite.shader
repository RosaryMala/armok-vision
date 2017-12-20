// Upgrade NOTE: upgraded instancing buffer 'Props' to new syntax.

Shader "Custom/CreatureSprite" {
	Properties {
        _MatTex("Albedo (RGB)", 2DArray) = "gray" {}
        _BumpMap("BumpMap", 2DArray) = "bump" {}
        _Cutoff("Alpha cutoff", Range(0,1)) = 0.5
    }
	SubShader {
		Tags { "RenderType"="TransparentCutout" }
		LOD 200
		
		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Standard addshadow alphatest:_Cutoff

		#pragma target 3.5

        UNITY_DECLARE_TEX2DARRAY(_MatTex);
        UNITY_DECLARE_TEX2DARRAY(_BumpMap);

		struct Input {
			float2 uv_MatTex;
		};

		// Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
		// See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
		// #pragma instancing_options assumeuniformscaling
        UNITY_INSTANCING_BUFFER_START(Props)
            UNITY_DEFINE_INSTANCED_PROP(fixed4, _LayerColor)
#define _LayerColor_arr Props
            UNITY_DEFINE_INSTANCED_PROP(float, _LayerIndex)
#define _LayerIndex_arr Props
        UNITY_INSTANCING_BUFFER_END(Props)

		void surf (Input IN, inout SurfaceOutputStandard o) {
            float layerIndex = UNITY_ACCESS_INSTANCED_PROP(_LayerIndex_arr, _LayerIndex);
            fixed4 layerPixel = UNITY_SAMPLE_TEX2DARRAY(_MatTex, float3(IN.uv_MatTex.xy, layerIndex));
            fixed4 layerColor = UNITY_ACCESS_INSTANCED_PROP(_LayerColor_arr, _LayerColor);

            o.Albedo = layerPixel.rgb * layerColor.rgb;
            o.Metallic = max((layerColor.a * 2) - 1, 0);
            o.Alpha = layerPixel.a;
            o.Normal = UnpackNormal(UNITY_SAMPLE_TEX2DARRAY(_BumpMap, float3(IN.uv_MatTex.xy, layerIndex)));
		}
		ENDCG
	}
	FallBack "Diffuse"
    //CustomEditor "CreatureSpriteEditor"

}
