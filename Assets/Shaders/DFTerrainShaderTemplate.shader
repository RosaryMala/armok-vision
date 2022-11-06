Shader /*ase_name*/ "ASETemplateShaders/DFTerrainTemplate" /*end*/
{
	Properties
	{
        _MainTex("Albedo (RGB)", 2D) = "white" {}
        _ElevationGradient("Elevation Gradient (RGB)", 2D) = "white" {}
        _BiomeMap("Biome Map (RGB)", 2D) = "white" {}
		/*ase_props*/
	}
	
	SubShader
	{
		Tags { "RenderType"="Opaque" }
		LOD 200
		Cull Back
		/*ase_pass*/

		Pass
		{
			CGPROGRAM
			#pragma target 3.0 
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"
			/*ase_pragma*/

			struct appdata
			{
				float4 vertex : POSITION;
				float2 texcoord : TEXCOORD0;
				float2 texcoord1 : TEXCOORD1;
				float3 worldPos : NORMAL;
				/*ase_vdata:p=p;uv0=tc0.xy;uv1=tc1.xy;n=n*/
			};
			
			struct v2f
			{
				float4 vertex : SV_POSITION;
				/*ase_interp(1,7):sp=sp.xyzw;uv0=tc0.xy;uv1=tc0.zw;n=n*/
			};

            uniform sampler2D _MainTex;
            uniform sampler2D _ElevationGradient;
            uniform sampler2D _BiomeMap;
            uniform half _Scale;
            uniform half _SeaLevel;
			/*ase_globals*/
			
			v2f vert ( appdata v /*ase_vert_input*/)
			{
				v2f o;
				// ase common template code
				/*ase_vert_code:v=appdata;o=v2f*/
				o.vertex.xyz += /*ase_vert_out:Local Vertex;Float3*/ float3(0,0,0) /*end*/;
				o.vertex = UnityObjectToClipPos(v.vertex);
				return o;
			}
			
			fixed4 frag (v2f i /*ase_frag_input*/) : SV_Target
			{
				float4 Albedo; 
				float Alpha;
				// ase common template code
				/*ase_frag_code:i=v2f*/
				Albedo = /*ase_frag_out:Albedo;Float4*/float4(1,1,1,1)/*end*/;
				Alpha = /*ase_frag_out:Alpha;Float*/1/*end*/;
				
				return Albedo;
			}
			ENDCG
		}
	}
}