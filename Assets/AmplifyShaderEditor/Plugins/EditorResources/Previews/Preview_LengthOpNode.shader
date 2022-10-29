Shader "Hidden/LengthOpNode"
{
	Properties
	{
		_A ("_A", 2D) = "white" {}
	}
		SubShader
	{
		Pass
		{
			CGPROGRAM
			#include "UnityCG.cginc"
			#pragma vertex vert_img
			#pragma fragment frag

			sampler2D _A;

			float4 frag (v2f_img i) : SV_Target
			{
				return length( tex2D (_A, i.uv).x );
			}
			ENDCG
		}

		Pass
		{
			CGPROGRAM
			#include "UnityCG.cginc"
			#pragma vertex vert_img
			#pragma fragment frag

			sampler2D _A;

			float4 frag (v2f_img i) : SV_Target
			{
				return length( tex2D(_A, i.uv).xy );
			}
			ENDCG
		}

		Pass
		{
			CGPROGRAM
			#include "UnityCG.cginc"
			#pragma vertex vert_img
			#pragma fragment frag

			sampler2D _A;

			float4 frag (v2f_img i) : SV_Target
			{
				return length( tex2D(_A, i.uv).xyz );
			}
			ENDCG
		}

		Pass
		{
			CGPROGRAM
			#include "UnityCG.cginc"
			#pragma vertex vert_img
			#pragma fragment frag

			sampler2D _A;

			float4 frag (v2f_img i) : SV_Target
			{
				return length( tex2D(_A, i.uv));
			}
			ENDCG
		}
	}
}
