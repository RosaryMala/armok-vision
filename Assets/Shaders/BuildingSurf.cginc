#ifdef _BOUNDING_BOX_ENABLED
clip(IN.worldPos - _ViewMin);
clip(_ViewMax - IN.worldPos);
#endif

float4 texcoords = TexCoords(IN);
//get the mask 
fixed4 dfTex = UNITY_SAMPLE_TEX2DARRAY(_MatTexArray, float3(texcoords.zw, UNITY_ACCESS_INSTANCED_PROP(_MatIndex)));
fixed4 matColor = UNITY_ACCESS_INSTANCED_PROP(_MatColor);
fixed3 albedo = overlay(dfTex.rgb, matColor.rgb);
half smoothness = dfTex.a;
half metallic = max((matColor.a * 2) - 1, 0);
fixed alpha = 1;

#ifdef _TEXTURE_MASK
fixed4 mask = tex2D(_DFMask, texcoords.xy);
fixed4 c = tex2D(_MainTex, texcoords.xy) * _Color;
fixed4 m = tex2D(_MetallicGlossMap, texcoords.xy);
albedo = lerp(albedo, c.rgb, mask.r);
albedo = lerp(albedo, overlay(c.rgb, matColor.rgb), max(mask.g - mask.r, 0));
alpha = lerp(c.a * min(matColor.a * 2, 1), c.a, mask.r);
#ifdef _METALLICGLOSSMAP
metallic = lerp(metallic, m.r, mask.r);
smoothness = lerp(smoothness, m.a * _GlossMapScale, mask.b);
#else
metallic = lerp(metallic, _Metallic, mask.r);
smoothness = lerp(smoothness, _Glossiness, mask.b);
#endif
//#else
//            albedo = overlay(albedo, matColor.rgb);
#endif


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
