#ifdef _BOUNDING_BOX_ENABLED
	clip(IN.worldPos - _ViewMin);
	clip(_ViewMax - IN.worldPos);
#endif

float4 texcoords = TexCoords(IN);
//get the mask 
fixed4 dfTex = UNITY_SAMPLE_TEX2DARRAY(_MatTexArray, float3(texcoords.zw, UNITY_ACCESS_INSTANCED_PROP(_MatIndex_arr, _MatIndex)));
fixed4 matColor = UNITY_ACCESS_INSTANCED_PROP(_MatColor_arr, _MatColor);
fixed3 albedo = dfTex.rgb * matColor.rgb;
half smoothness = dfTex.a;
half metallic = max((matColor.a * 2) - 1, 0);
fixed alpha = min(matColor.a * 2, 1);

fixed4 c = tex2D(_MainTex, texcoords.xy) * _Color;

#ifdef _TEXTURE_MASK
	fixed4 mask = tex2D(_DFMask, texcoords.xy);
	fixed4 m = tex2D(_MetallicGlossMap, texcoords.xy);
	albedo = lerp(albedo, c.rgb, mask.r);
	albedo = lerp(albedo, c.rgb * matColor.rgb, max(mask.g - mask.r, 0));
	albedo = lerp(albedo, UNITY_ACCESS_INSTANCED_PROP(_JobColor_arr, _JobColor), 1 - mask.a);
	alpha = lerp(c.a * alpha, c.a, mask.r);
	#ifdef _METALLICGLOSSMAP
		metallic = lerp(metallic, m.r, mask.r);
		smoothness = lerp(smoothness, m.a * _GlossMapScale, mask.b);
	#else
		metallic = lerp(metallic, _Metallic, mask.r);
		smoothness = lerp(smoothness, _Glossiness, mask.b);
	#endif
#else
alpha = c.a * alpha;
#endif

