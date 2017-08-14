Shader "Custom/AVSprite" {
	Properties {
        [PerRendererData] _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _Color("Tint", Color) = (1,1,1,1)
        [MaterialToggle] PixelSnap("Pixel snap", Float) = 0
    }
	SubShader {
        Tags
    {
        "Queue" = "Transparent"
        "IgnoreProjector" = "True"
        "RenderType" = "Transparent"
        "PreviewType" = "Plane"
        "CanUseSpriteAtlas" = "True"
    }
        LOD 200
		
		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Standard fullforwardshadows alpha vertex:vert
        //#pragma multi_compile _ ETC1_EXTERNAL_ALPHA

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		sampler2D _MainTex;
        sampler2D _AlphaTex;
        fixed4 _Color;

		struct Input {
			float2 uv_MainTex;
            float4 color: Color; // Vertex color
        };

        void vert(inout appdata_full v, out Input o)
        {
#if defined(PIXELSNAP_ON)
            v.vertex = UnityPixelSnap(v.vertex);
#endif

            UNITY_INITIALIZE_OUTPUT(Input, o);
            o.color = v.color * _Color;
        }


		// Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
		// See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
		// #pragma instancing_options assumeuniformscaling
		UNITY_INSTANCING_CBUFFER_START(Props)
			// put more per-instance properties here
		UNITY_INSTANCING_CBUFFER_END

#include "blend.cginc"

            void surf(Input IN, inout SurfaceOutputStandard o) {
            fixed4 c = tex2D(_MainTex, IN.uv_MainTex);
            clip(c.a - 0.5);
//#if ETC1_EXTERNAL_ALPHA
//            // get the color from an external texture (usecase: Alpha support for ETC1 on android)
//            //c.a = tex2D(_AlphaTex, IN.uv_MainTex).r;
//#endif //ETC1_EXTERNAL_ALPHA
            o.Albedo = overlay(c.rgb, IN.color.rgb);
            o.Metallic = max((IN.color.a * 2) - 1, 0);
            o.Alpha = c.a * min((IN.color.a * 2), 1);
        }
		ENDCG
	}
	FallBack "Diffuse"
}
