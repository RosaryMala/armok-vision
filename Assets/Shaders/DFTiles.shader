Shader "Unlit/DFTiles"
{
	Properties
	{
        _MainTex("Texture", 2D) = "white" {}
        _Pallet("Pallet", 2D) = "white" {}
        _Threshold("Threshold", Range(0,1)) = 0.5
    }
	SubShader
	{
		Tags { "RenderType"="Opaque" }
		LOD 100

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			// make fog work
			#pragma multi_compile_fog
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
                float4 color: Color; // Vertex color
            };

			struct v2f
			{
				float2 uv : TEXCOORD0;
				UNITY_FOG_COORDS(1)
				float4 vertex : SV_POSITION;
                float4 color: Color; // Vertex color
            };

            sampler2D _MainTex;
            sampler2D _Pallet;
            float4 _MainTex_ST;
            float _Threshold;
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				UNITY_TRANSFER_FOG(o,o.vertex);
                o.color = v.color;
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
                fixed4 fgCol = tex2D(_Pallet, float2(i.color.r, 0.5));
                fixed4 bgCol = tex2D(_Pallet, float2(i.color.g, 0.5));
                // sample the texture
				fixed4 texCol = tex2D(_MainTex, i.uv);
                fixed4 col = ((texCol * fgCol) * step(_Threshold, texCol.a)) + (step(texCol.a, _Threshold) * bgCol);
				// apply fog
				UNITY_APPLY_FOG(i.fogCoord, col);
				return col;
			}
			ENDCG
		}
	}
}
