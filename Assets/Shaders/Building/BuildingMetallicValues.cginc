//Actual output definitions
o.Albedo = albedo;
#ifdef _NORMALMAP
o.Normal = UnpackScaleNormal(tex2D(_BumpMap, texcoords.xy), _BumpScale);
#endif
#ifdef _EMISSION
o.Emission = tex2D(_EmissionMap, texcoords.xy) * _EmissionColor;
#endif
o.Metallic = metallic;
o.Smoothness = smoothness;
o.Occlusion = lerp(1, tex2D(_OcclusionMap, texcoords.xy), _OcclusionStrength);
o.Alpha = alpha;
