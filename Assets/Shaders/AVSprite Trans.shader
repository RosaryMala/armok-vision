// Upgrade NOTE: upgraded instancing buffer 'Props' to new syntax.

Shader "Custom/AVSprite Trans" {
	Properties {
        [PerRendererData] _MainTex("Albedo (RGB)", 2D) = "white" {}
        _Color("Tint", Color) = (1,1,1,1)
        _Cutoff("Alpha cutoff", Range(0,1)) = 0.5
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
		#pragma surface surf Standard fullforwardshadows alpha:fade vertex:vert
        //#pragma multi_compile _ ETC1_EXTERNAL_ALPHA

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

        sampler2D _MainTex;
        float4 _MainTex_TexelSize;
        sampler2D _AlphaTex;
        fixed4 _Color;
        float _Cutoff;

		struct Input {
			float2 uv_MainTex;
            float4 color: Color; // Vertex color
            float3 viewDir;
        };

        void vert(inout appdata_full v, out Input o)
        {
            UNITY_INITIALIZE_OUTPUT(Input, o);
            o.color = v.color * _Color;
        }


		// Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
		// See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
		// #pragma instancing_options assumeuniformscaling
		UNITY_INSTANCING_BUFFER_START(Props)
			// put more per-instance properties here
		UNITY_INSTANCING_BUFFER_END(Props)

#include "blend.cginc"

        void surf(Input IN, inout SurfaceOutputStandard o) {
            fixed4 c = tex2D(_MainTex, IN.uv_MainTex);
            clip(c.a - _Cutoff + 1 - IN.color.a);
            float2 alphaCoords = IN.uv_MainTex * _MainTex_TexelSize.zw;

            float distance = 8;
            float blur = 3;

            float n = tex2Dlod(_MainTex, float4((alphaCoords + float2(0, distance)) * _MainTex_TexelSize.xy, 0, blur)).a;
            float s = tex2Dlod(_MainTex, float4((alphaCoords + float2(0, -distance)) * _MainTex_TexelSize.xy, 0, blur)).a;
            float e = tex2Dlod(_MainTex, float4((alphaCoords + float2(-distance, 0)) * _MainTex_TexelSize.xy, 0, blur)).a;
            float w = tex2Dlod(_MainTex, float4((alphaCoords + float2(distance, 0)) * _MainTex_TexelSize.xy, 0, blur)).a;

            float3 normal = normalize(float3(e - w, s - n, 0.5));
            //#if ETC1_EXTERNAL_ALPHA
            //            // get the color from an external texture (usecase: Alpha support for ETC1 on android)
            //            //c.a = tex2D(_AlphaTex, IN.uv_MainTex).r;
            //#endif //ETC1_EXTERNAL_ALPHA
            float metal = max((IN.color.a * 2) - 1, 0);
            o.Normal = normal;
            c.rgb = overlay(c.rgb, IN.color.rgb);
            o.Albedo = c.rgb;
            o.Metallic = metal;
            o.Alpha = c.a; // *min((IN.color.a * 2), 1);
            o.Smoothness = lerp(0.1, 0.8, metal);
        }
		ENDCG
	}
	FallBack "Unlit/AVSprite Shadow"
}
