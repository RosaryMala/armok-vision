Shader "Custom/DFTilesSurface" {
	Properties {
        _Color("Color", Color) = (1,1,1,1)
        _Light("Emission", Color) = (0,0,0)
        _MainTex("Texture", 2D) = "white" {}
        _Pallet("Pallet", 2D) = "white" {}
        _Threshold("Threshold", Range(0,1)) = 0.5
		_Glossiness ("Smoothness", Range(0,1)) = 0.5
		_Metallic ("Metallic", Range(0,1)) = 0.0
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Standard fullforwardshadows

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		sampler2D _MainTex;
        sampler2D _Pallet;
        float _Threshold;

		struct Input {
			float2 uv_MainTex;
            float4 color: Color; // Vertex color
        };

		half _Glossiness;
		half _Metallic;
        fixed4 _Color;
        half3 _Light;

		void surf (Input IN, inout SurfaceOutputStandard o) {
            fixed4 fgCol = tex2D(_Pallet, float2(IN.color.r, 0.5));
            fixed4 bgCol = tex2D(_Pallet, float2(IN.color.g, 0.5));
            // sample the texture
            fixed4 texCol = tex2D(_MainTex, IN.uv_MainTex);
            fixed4 col = ((texCol * fgCol) * step(_Threshold, texCol.a)) + (step(texCol.a, _Threshold) * bgCol);
            o.Albedo = col.rgb * _Color.rgb;
			// Metallic and smoothness come from slider variables
			o.Metallic = _Metallic;
			o.Smoothness = _Glossiness;
			o.Alpha = _Color.a;
            o.Emission = col.rgb * _Light;
		}
		ENDCG
	}
	FallBack "Diffuse"
}
