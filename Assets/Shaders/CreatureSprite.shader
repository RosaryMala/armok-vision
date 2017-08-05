Shader "Custom/CreatureSprite" {
	Properties {
        _MatTex("Albedo (RGB)", 2DArray) = "gray" {}
        _BumpMap("BumpMap", 2DArray) = "bump" {}
        _Cutoff("Alpha cutoff", Range(0,1)) = 0.5
    }
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Standard addshadow alphatest:_Cutoff

		#pragma target 3.5

        #pragma instancing_options maxcount:512
        #define layer_count 1

        UNITY_DECLARE_TEX2DARRAY(_MatTex);
        UNITY_DECLARE_TEX2DARRAY(_BumpMap);

		struct Input {
			float2 uv_MatTex;
		};

		// Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
		// See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
		// #pragma instancing_options assumeuniformscaling
        UNITY_INSTANCING_CBUFFER_START(Props)
            UNITY_DEFINE_INSTANCED_PROP(fixed4, _LayerColor[layer_count])
            UNITY_DEFINE_INSTANCED_PROP(float, _LayerIndex[layer_count])
        UNITY_INSTANCING_CBUFFER_END

#include "blend.cginc"

		void surf (Input IN, inout SurfaceOutputStandard o) {
            fixed3 c = fixed3(0, 0, 0);
            fixed m = 0;
            fixed a = 0;
            fixed3 n = fixed3(0.5, 0.5, 1);
//#if defined(layer_1)
            int i = 0;
//#else
//            for (int i = 0; i < layer_count; i++)
//#endif
            {
                float layerIndex = UNITY_ACCESS_INSTANCED_PROP(_LayerIndex)[i];
                fixed4 layerPixel = UNITY_SAMPLE_TEX2DARRAY(_MatTex, float3(IN.uv_MatTex.xy, layerIndex));
                fixed4 normalPixel = UNITY_SAMPLE_TEX2DARRAY(_BumpMap, float3(IN.uv_MatTex.xy, layerIndex));
                fixed4 layerColor = UNITY_ACCESS_INSTANCED_PROP(_LayerColor)[i];
                if (layerIndex >= 0 && layerPixel.a >= 0.5)
                {
                    //currently just multiplying because the sprites aren't setup yet.
                    c = layerPixel.rgb * layerColor.rgb;
                    a = layerPixel.a;
                    m = max((layerColor.a * 2) - 1, 0);
                    n = UnpackNormal(normalPixel);
                }
            }

            o.Albedo = c;
            o.Metallic = m;
            o.Alpha = a;
            o.Normal = n;
		}
		ENDCG
	}
	FallBack "Diffuse"
    //CustomEditor "CreatureSpriteEditor"

}
