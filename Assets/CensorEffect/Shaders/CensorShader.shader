Shader "FX/Censor"
{
	Properties
	{
		_Pixelation ("Pixelation", Range(0.001, 0.1)) = 0.01
	}
	SubShader
	{
		Tags { "RenderType"="Opaque"
		"Queue" = "Overlay" }
		LOD 100

		GrabPass
        {
			//Do a grab pass
        }

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
			};

			struct v2f
			{
				float4 vertex : SV_POSITION;
				float4 screenUV : TEXCOORD1;
			};

			sampler2D _GrabTexture;
			float _Pixelation;
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.screenUV = ComputeGrabScreenPos(o.vertex);
				return o;
			}

			
			fixed4 frag (v2f i) : SV_Target
			{
				float4 screenUV = i.screenUV / i.screenUV.w;
				float4 pixelUV = half4((int)(screenUV.x / _Pixelation) * _Pixelation, (int)((screenUV.y) / _Pixelation) * _Pixelation, screenUV.z, screenUV.w);
				half4 screenColorPixelized = tex2Dproj(_GrabTexture, pixelUV);

				return screenColorPixelized;
			}
			ENDCG
		}
	}
}
