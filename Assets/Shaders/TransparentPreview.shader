Shader "Hidden/TransparentPreview"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
        _Color("Sprite Color", Color) = (0.5,0.5,0.5,0.5)
        _Rect("Sprite Rect", Vector) = (0,0,1,1)
	}
	SubShader
	{
        Tags{ "Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha 
            // No culling or depth
		Cull Off ZWrite Off ZTest Always
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
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
			};

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				return o;
			}
			
			sampler2D _MainTex;
            fixed4 _Color;
            fixed4 _Rect;

#include "blend.cginc"

			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 col = tex2D(_MainTex, (i.uv* _Rect.zw) + _Rect.xy);
                //clip(col.a - 0.5);
				col.rgb = overlay(col.rgb, _Color.rgb);
                col.a = col.a * min((_Color.a * 2), 1);
                return col;
			}
			ENDCG
		}
	}
}
