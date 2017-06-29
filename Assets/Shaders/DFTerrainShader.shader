Shader "Custom/DFTerrainShader" 
{
    Properties{
        _MainTex("Albedo (RGB)", 2D) = "white" {}
        _ElevationGradient("Elevation Gradient (RGB)", 2D) = "white" {}
        _BiomeMap("Biome Map (RGB)", 2D) = "white" {}
        _Glossiness("Smoothness", Range(0,1)) = 0.5
        _Metallic("Metallic", Range(0,1)) = 0.0
        _SeaLevel("Sea Level (Unscaled)", Float) = 100
        _Scale("Terrain Scale", Float) = 1.0
            _Curvature("Curvature", Float) = 0.0001
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
            #pragma surface surf Standard fullforwardshadows vertex:vert

            // Use shader model 3.0 target, to get nicer looking lighting
            #pragma target 3.0

            sampler2D _MainTex;
            sampler2D _ElevationGradient;
            sampler2D _BiomeMap;

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

            float _Curvature;
            // This is where the curvature is applied
            void vert(inout appdata_full v)
            {
                // Transform the vertex coordinates from model space into world space
                float4 vv = mul(unity_ObjectToWorld, v.vertex);

                // Now adjust the coordinates to be relative to the camera position
                vv.xyz -= _WorldSpaceCameraPos.xyz;

                // Reduce the y coordinate (i.e. lower the "height") of each vertex based
                // on the square of the distance from the camera in the z axis, multiplied
                // by the chosen curvature factor
                vv = float4(0.0f, ((vv.z * vv.z) + (vv.x * vv.x)) * -_Curvature, 0.0f, 0.0f);

                // Now apply the offset back to the vertices in model space
                v.vertex += mul(unity_WorldToObject, vv);
            }

            void surf(Input IN, inout SurfaceOutputStandard o)
            {
                half correctedElevation = ((IN.worldPos.y / _Scale) - _SeaLevel) / 3;
                // Albedo comes from a texture tinted by color
                fixed4 c = tex2D(_MainTex, IN.uv_MainTex);
                fixed4 g = tex2D(_ElevationGradient, ((((IN.worldPos / _Scale) - _SeaLevel) / 3) + 100) / 280);
                fixed4 b = tex2D(_BiomeMap, IN.uv2_BiomeMap);
                o.Albedo = lerp(b.rgb, g.rgb, g.a);
                // Metallic and smoothness come from slider variables
                o.Metallic = _Metallic;
                o.Smoothness = _Glossiness;
                o.Alpha = c.a;
            }
                ENDCG
        }
            FallBack "Diffuse"
}
