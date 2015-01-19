Shader "Lux/Human/Eye AO" {

Properties {
	_Color ("Diffuse Color", Color) = (1,1,1,1)
	_MainTex ("Base (RGB) Alpha (A)", 2D) = "white" {}	
	
	// _Shininess property is needed by the lightmapper - otherwise it throws errors
	[HideInInspector] _Shininess ("Shininess (only for Lightmapper)", Float) = 0.5
}

SubShader { 
	Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}
	LOD 200
	Offset -1,-1
	
	CGPROGRAM
	#pragma surface surf Lambert noambient alpha decal:blend 

	float4 _Color;
	sampler2D _MainTex;

	struct Input {
		float2 uv_MainTex;
	};

	void surf (Input IN, inout SurfaceOutput o) {
		fixed4 c = tex2D(_MainTex, IN.uv_MainTex);
		c *= _Color;
		o.Albedo = c.rgb;
		o.Alpha = c.a;
			
	}
ENDCG
}
Fallback "Transparent/VertexLit"
}
