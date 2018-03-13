Shader "Custom/Water" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
        _BumpScale("Scale", Float) = 1.0
        _BumpMap("Normal Map", 2D) = "bump" {}
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
		_Metallic ("Metallic", Range(0,1)) = 0.0
        _DetailNormalMapScale("Scale", Float) = 1.0
        _DetailNormalMap("Normal Map", 2D) = "bump" {}
        _FLowSpeed("Flow Speed", Vector) = (0,0,0,0)

	}
	SubShader {
        Tags { "RenderType" = "Transparent" "Queue" = "Transparent" }
		LOD 200
		    // extra pass that renders to depth buffer only

        Pass {
            ZWrite On
            ColorMask 0
        }
		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Standard fullforwardshadows alpha

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		sampler2D _MainTex;
        sampler2D _BumpMap;
        sampler2D _DetailNormalMap;

		struct Input {
            float2 uv_MainTex;
            float3 worldPos;
        };

		half _Glossiness;
		half _Metallic;
        half _BumpScale;
        half _DetailNormalMapScale;
		fixed4 _Color;
        float4 _FLowSpeed;
        float4 _BumpMap_ST;
        float4 _DetailNormalMap_ST;

		void surf (Input IN, inout SurfaceOutputStandard o) {
			// Albedo comes from a texture tinted by color
			fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
            half3 normalTangent = UnpackScaleNormal(tex2D(_BumpMap, IN.worldPos.xz * _BumpMap_ST.xy + _BumpMap_ST.zw + frac(_Time.y * _FLowSpeed.xy)), _BumpScale);
            half3 detailNormalTangent = UnpackScaleNormal(tex2D(_DetailNormalMap, IN.worldPos.xz * _DetailNormalMap_ST.xy + _DetailNormalMap_ST.zw), _DetailNormalMapScale);


			o.Albedo = c.rgb;
			// Metallic and smoothness come from slider variables
			o.Metallic = _Metallic;
			o.Smoothness = _Glossiness;
			o.Alpha = c.a;

            o.Normal = BlendNormals(normalTangent, detailNormalTangent);
		}
		ENDCG
	}
	FallBack "Diffuse"
}
