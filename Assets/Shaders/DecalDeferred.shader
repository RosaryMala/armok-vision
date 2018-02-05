Shader "Hardsurface/DecalDeferred" 
{
	Properties 
	{
		_MainTex ("Albedo (RGB), Alpha (A)", 2D) = "white" {}
		_BumpMap ("Normalmap", 2D) = "bump" {}
		_Smoothness ("Smoothness", Range(0,1)) = 0.5
		_Metalness ("Metalness", Range(0,1)) = 0.0
		_DisableAlpha ("Disable alpha (for testing)", Range(0,1)) = 0.0

		_ContributionAlbedo ("Contribution / Albedo", Range(0,1)) = 0.0
		_ContributionSpecSmoothness ("Contribution / Smoothness", Range(0,1)) = 0.0
		_ContributionNormal ("Contribution / Normal", Range(0,1)) = 1.0
		_ContributionEmission ("Contribution / Emission", Range(0,1)) = 1.0
	}
	SubShader 
	{
		Tags {"Queue"="AlphaTest" "IgnoreProjector"="True" "RenderType"="Opaque" "ForceNoShadowCasting"="True"}
		LOD 300
		Offset -1, -1
		Blend SrcAlpha OneMinusSrcAlpha
		
		CGPROGRAM

		#pragma surface surf Standard finalgbuffer:DecalFinalGBuffer keepalpha exclude_path:forward exclude_path:prepass noshadow noforwardadd
		#pragma target 3.0

		sampler2D _MainTex;
		sampler2D _BumpMap;

		struct Input 
		{
			float2 uv_MainTex;
			float2 uv_BumpMap;
			float4 screenPos;
		};

		half _Smoothness;
		half _Metalness;
		half _DisableAlpha;

		half _ContributionAlbedo;
		half _ContributionSpecSmoothness;
		half _ContributionNormal;
		half _ContributionEmission;

		void surf (Input IN, inout SurfaceOutputStandard o) 
		{
			fixed4 main = tex2D(_MainTex, IN.uv_MainTex);

			o.Albedo = main;
			o.Normal = UnpackNormal(tex2D(_BumpMap, IN.uv_BumpMap));
			o.Metallic = _Metalness;
			o.Smoothness = _Smoothness;
			o.Alpha = lerp (main.w, 1, _DisableAlpha);
		}

		void DecalFinalGBuffer (Input IN, SurfaceOutputStandard o, inout half4 diffuse, inout half4 specSmoothness, inout half4 normal, inout half4 emission)
		{
			diffuse *= o.Alpha * _ContributionAlbedo; 
			specSmoothness *= o.Alpha * _ContributionSpecSmoothness; 
			normal *= o.Alpha * _ContributionNormal; 
			emission *= o.Alpha * _ContributionEmission; 
		}

		ENDCG
	} 
	FallBack "Diffuse"
}