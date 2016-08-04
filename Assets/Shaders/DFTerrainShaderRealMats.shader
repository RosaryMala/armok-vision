Shader "Custom/DFTerrainShaderRealMats" 
{
    Properties{
        _MainTex("Albedo (RGB)", 2D) = "white" {}
        _ElevationGradient("Elevation Gradient (RGB)", 2D) = "white" {}
        _BiomeMap("Biome Map (RGB)", 2D) = "white" {}
        _Glossiness("Smoothness", Range(0,1)) = 0.5
        _Metallic("Metallic", Range(0,1)) = 0.0
        _SeaLevel("Sea Level (Unscaled)", Float) = 100
        _Scale("Terrain Scale", Float) = 1.0
		_GraySample("Gray Sample", 2D) = "gray" {}
    }
        SubShader
        {
            Tags { "RenderType" = "Opaque" }
            LOD 200

            //Cull Front
            //CGPROGRAM
            //#pragma surface surf Standard fullforwardshadows

            //struct Input {
            //    float2 uv_MainTex;
            //};

            //// Flip normal for back faces
            //void vert(inout appdata_full v) {
            //    v.normal *= -1;
            //}
            //// Red back faces
            //void surf(Input IN, inout SurfaceOutputStandard o) {
            //    o.Albedo = fixed3(0,0,0);
            //    o.Alpha = 1;
            //}
            //ENDCG

            Cull Back
            CGPROGRAM
            // Physically based Standard lighting model, and enable shadows on all light types
            #pragma surface surf Standard fullforwardshadows

            // Use shader model 3.0 target, to get nicer looking lighting
            #pragma target 3.0

            sampler2D _MainTex;
            sampler2D _ElevationGradient;
			sampler2D _BiomeMap;
			sampler2D _GraySample;

            struct Input
            {
                float2 uv_MainTex;
                float2 uv2_BiomeMap;
                float4 color: Color; // Vertex color
                float3 worldPos;
            };

            half _Glossiness;
            half _Metallic;
            half _Scale;
            half _SeaLevel;

            void surf(Input IN, inout SurfaceOutputStandard o)
            {
                half correctedElevation = ((IN.worldPos.y / _Scale) - _SeaLevel) / 3;
                // Albedo comes from a texture tinted by color
				fixed4 gray = tex2D(_GraySample, float2(0.5,0.5));
				fixed3 terrainColor = gray.rgb < 0.5 ? (2.0 * gray.rgb * IN.color.rgb) : (1.0 - 2.0 * (1.0 - gray.rgb) * (1.0 - IN.color.rgb));
                fixed4 c = tex2D(_MainTex, IN.uv_MainTex);
                fixed4 g = tex2D(_ElevationGradient, ((((IN.worldPos / _Scale) - _SeaLevel) / 3) + 100) / 280);
                fixed4 b = tex2D(_BiomeMap, IN.uv2_BiomeMap);
                fixed3 falseColor = lerp(b.rgb, g.rgb, g.a);
				o.Albedo = lerp(falseColor, terrainColor, IN.color.a);
                // Metallic and smoothness come from slider variables
                o.Metallic = _Metallic;
                o.Smoothness = _Glossiness;
                o.Alpha = c.a;
            }
                ENDCG
        }
            FallBack "Diffuse"
}
