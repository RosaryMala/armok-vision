Shader "Effects/Additive" {
	Properties
	{
		_MainTex("Render Input", 2D) = "white" {}
		_BlendTex("Blending Texture", 2D) = "white" {}
	}
		SubShader
	{
		ZTest Always Cull Off ZWrite Off Fog{ Mode Off }
		Pass
		{
			CGPROGRAM
#pragma vertex vert_img
#pragma fragment frag
#include "UnityCG.cginc"

			sampler2D _MainTex;
			sampler2D _BlendTex;

			float4 frag(v2f_img IN) : COLOR
			{
				half4 c = tex2D(_MainTex, IN.uv);
				half4 d = tex2D(_BlendTex, IN.uv);
				return d;
			}
		ENDCG
		}
	}
}