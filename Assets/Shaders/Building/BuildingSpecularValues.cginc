half3 specColor;
half oneMinusReflectivity;

albedo = DiffuseAndSpecularFromMetallicCustom(albedo, metallic, specColor, oneMinusReflectivity);

//Actual output definitions
o.Albedo = albedo;
o.Specular = specColor;
o.Normal = normal;
#ifdef _EMISSION
o.Emission = tex2D(_EmissionMap, texcoords.xy) * _EmissionColor;
#endif
o.Smoothness = smoothness;
o.Occlusion = lerp(1, tex2D(_OcclusionMap, texcoords.xy), _OcclusionStrength) * shape.r;
o.Alpha = alpha;
