           struct Input
            {
                float2 uv_MainTex;
                float2 uv2_BiomeMap;
                float4 color: Color; // Vertex color
                float3 worldPos;
            };

			
            void surf(Input IN)
            {
                float correctedElevation = ((IN.worldPos.y / _Scale) - _SeaLevel) / 3;
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