#ifdef _BOUNDING_BOX_ENABLED
	clip(IN.worldPos - _ViewMin);
	clip(_ViewMax - IN.worldPos);
#endif

float4 texcoords = TexCoords(IN);
//get the mask 
fixed4 dfTex = UNITY_SAMPLE_TEX2DARRAY(_MatTexArray, float3(texcoords.zw, UNITY_ACCESS_INSTANCED_PROP(_MatIndex_arr, _MatIndex)));
fixed4 matColor = UNITY_ACCESS_INSTANCED_PROP(_MatColor_arr, _MatColor);
fixed3 albedo = matColor.rgb;
fixed4 shape = UNITY_SAMPLE_TEX2DARRAY(_ShapeMap, float3(texcoords.zw, UNITY_ACCESS_INSTANCED_PROP(_ShapeIndex_arr, _ShapeIndex)));

fixed3 matNormal = UnpackNormal(float4(1, shape.g, 1, shape.a));
fixed3 normal = matNormal;

fixed occlusion = lerp(1, tex2D(_OcclusionMap, texcoords.xy), _OcclusionStrength);

#ifdef _PATTERN_MASK
fixed4 pattern_mask = tex2D(_PatternMask, texcoords.zw);
albedo = lerp(albedo, _Color1.rgb, pattern_mask.r);
albedo = lerp(albedo, _Color2.rgb, pattern_mask.g);
albedo = lerp(albedo, _Color3.rgb, pattern_mask.b);
#endif

albedo = dfTex.rgb * albedo;
half smoothness = dfTex.a;
half metallic = max((matColor.a * 2) - 1, 0);
fixed alpha = min(matColor.a * 2, 1);

fixed4 c = tex2D(_MainTex, texcoords.xy) * _Color;

#ifdef _NORMALMAP
    fixed3 customNormal = UnpackScaleNormal(tex2D(_BumpMap, texcoords.xy), _BumpScale);
#endif
#ifdef _TEXTURE_MASK
	fixed4 mask = tex2D(_DFMask, texcoords.xy);
#ifdef _NORMALMAP
    normal = lerp(BlendNormals(matNormal, customNormal), customNormal, mask.b);
#else
    normal = lerp(matNormal, fixed3(0, 1, 0), mask.b);
#endif
    albedo = lerp(albedo, c.rgb, mask.r);
	albedo = lerp(albedo, c.rgb * dfTex.rgb * matColor.rgb, max(mask.g - mask.r, 0));
	albedo = lerp(albedo, UNITY_ACCESS_INSTANCED_PROP(_JobColor_arr, _JobColor), 1 - mask.a);
	alpha = lerp(c.a * alpha, c.a, mask.r);
	#ifdef _METALLICGLOSSMAP
        fixed4 m = tex2D(_MetallicGlossMap, texcoords.xy);
        metallic = lerp(metallic, m.r, mask.r);
		smoothness = lerp(smoothness, m.a * _GlossMapScale, mask.b);
	#else
		metallic = lerp(metallic, _Metallic, mask.r);
		smoothness = lerp(smoothness, _Glossiness, mask.b);
	#endif
        occlusion = lerp(occlusion * shape.r, occlusion, mask.b);
#else
    alpha = c.a * alpha;
    #ifdef _NORMALMAP
        normal = BlendNormals(matNormal, customNormal);
    #endif
        occlusion *= shape.r;
#endif
        normal = matNormal;
