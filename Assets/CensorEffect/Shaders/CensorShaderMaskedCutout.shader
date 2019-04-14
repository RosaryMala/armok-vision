Shader "FX/Censor (Masked Cutout)"
{
	Properties
	{
		[NoScaleOffset]_MainTex ("Mask (Alpha)", 2D) = "white" {}
		_Pixelation ("Pixelation", Range(0.001, 0.1)) = 0.01
	}
	SubShader
	{
		Tags { "RenderType"="Cutout"
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
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float4 vertex : SV_POSITION;
				float2 uv : TEXCOORD0;
				float4 screenUV : TEXCOORD1;
			};

			sampler2D _GrabTexture;
			sampler2D _MainTex;
			float _Pixelation;
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				#ifdef UNITY_SINGLE_PASS_STEREO
				o.uv = UnityStereoScreenSpaceUVAdjust(v.uv, _MainTex);
				#else
				o.uv = v.uv;
				#endif

				#if UNITY_UV_STARTS_AT_TOP
				o.uv.y = 1 - o.uv.y;
				#endif

				o.screenUV = ComputeGrabScreenPos(o.vertex);
				return o;
			}

			
			fixed4 frag (v2f i) : SV_Target
			{
				fixed mask = tex2D(_MainTex, i.uv).a;
				clip(mask - 0.5);

				float4 screenUV = i.screenUV / i.screenUV.w;
				//screenUV.z = ( UNITY_NEAR_CLIP_VALUE >= 0 ) ? screenUV.z : screenUV.z * 0.5 + 0.5;
				float4 pixelUV = half4((int)(screenUV.x / _Pixelation) * _Pixelation, (int)((screenUV.y) / _Pixelation) * _Pixelation, screenUV.z, screenUV.w);

				half4 screenColorPixelized = tex2Dproj(_GrabTexture, pixelUV);


				return screenColorPixelized;
			}
			ENDCG
		}
	}
}
