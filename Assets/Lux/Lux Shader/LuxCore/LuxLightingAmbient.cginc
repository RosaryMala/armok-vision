#ifndef LuxIBL_CG_INCLUDED
#define LuxIBL_CG_INCLUDED

//		///////////////////////////////////////
// 		Further functions to keep the surf function rather simple
		
		#ifdef SPECCUBE_ON
			float NdotV = max(0, dot(o.Normal, normalize(IN.viewDir.xyz)));
		#endif

		// Fake Fresnel effect using N dot V / only needed by deferred lighting	
		#if defined(UNITY_PASS_PREPASSFINAL) && defined (SPECCUBE_ON)
			#ifdef NO_DEFERREDFRESNEL
				o.DeferredFresnel = 0;
			#else
				o.DeferredFresnel = exp2(-OneOnLN2_x6 * NdotV);
			#endif
		#endif

//		///////////////////////////////////////
// 		Lux IBL / ambient lighting
		
		// set o.Emission = 0.0 to make diffuse shaders work correctly
		o.Emission = 0.0;

		#ifdef NORMAL_IS_WORLDNORMAL
			float3 worldNormal = IN.normal;
		#else
			#ifdef USE_BLURREDNORMAL
				float3 worldNormal = WorldNormalVector(IN, o.NormalBlur);
			#else
				float3 worldNormal = WorldNormalVector(IN, o.Normal);
			#endif
		#endif

		#if defined(USE_GLOBAL_DIFFIBL_SETTINGS) && defined(GLDIFFCUBE_ON)
			#define DIFFCUBE_ON
		#endif

//		add diffuse IBL
		#ifdef DIFFCUBE_ON
			fixed4	diff_ibl = texCUBE (_DiffCubeIBL, worldNormal);
			#ifdef LUX_LINEAR
				// if colorspace = linear alpha has to be brought to linear too (rgb already is): alpha = pow(alpha,2.233333333).
				// approximation taken from http://chilliant.blogspot.de/2012/08/srgb-approximations-for-hlsl.html
				diff_ibl.a *= diff_ibl.a * (diff_ibl.a * 0.305306011 + 0.682171111) + 0.012522878;
			#endif
			diff_ibl.rgb = diff_ibl.rgb * diff_ibl.a;
			o.Emission = diff_ibl.rgb * ExposureIBL.x * o.Albedo;
		#else
			#if defined (LIGHTMAP_OFF) && defined (DIRLIGHTMAP_OFF)
//		otherwise add ambient light from Spherical Harmonics
				o.Emission = ShadeSH9 ( float4(worldNormal.xyz, 1.0)) * o.Albedo;
			#endif
			#if defined (LUX_LIGHTMAP_OFF)
//		otherwise add ambient light from Spherical Harmonics
				o.Emission = ShadeSH9 ( float4(worldNormal.xyz, 1.0)) * o.Albedo;
			#endif
		#endif
		
//		add specular IBL		
		#ifdef SPECCUBE_ON
			//#ifdef LUX_BOXPROJECTION
			//	half3 worldRefl;
			//#else
				half3 worldRefl = WorldReflectionVector (IN, o.Normal);	
			//#endif

		//	Boxprojection / Rotation
			#ifdef LUX_BOXPROJECTION
				// Bring worldRefl and worldPos into Cube Map Space
				worldRefl = mul(_CubeMatrix_Trans, float4(worldRefl,1)).xyz;
				float3 PosCS = mul(_CubeMatrix_Inv,float4(IN.worldPos,1)).xyz;
				float3 FirstPlaneIntersect = _CubemapSize - PosCS;
				float3 SecondPlaneIntersect = -_CubemapSize - PosCS;
				float3 FurthestPlane = (worldRefl > 0.0) ? FirstPlaneIntersect : SecondPlaneIntersect;
				FurthestPlane /= worldRefl;
				float Distance = min(FurthestPlane.x, min(FurthestPlane.y, FurthestPlane.z));
				worldRefl = PosCS + worldRefl * Distance;
			#endif

			#if defined (LUX_LIGHTING_CT)
				o.Specular *= o.Specular * (o.Specular * 0.305306011 + 0.682171111) + 0.012522878;
			#endif

			float mipSelect = 1.0f - o.Specular;
			mipSelect = mipSelect * mipSelect * 7; // but * 6 would look better...
			fixed4 spec_ibl = texCUBElod (_SpecCubeIBL, float4(worldRefl, mipSelect));
			#ifdef LUX_LINEAR
				// if colorspace = linear alpha has to be brought to linear too (rgb already is): alpha = pow(alpha,2.233333333) / approximation taken from http://chilliant.blogspot.de/2012/08/srgb-approximations-for-hlsl.html
				spec_ibl.a *= spec_ibl.a * (spec_ibl.a * 0.305306011 + 0.682171111) + 0.012522878;
			#endif
			spec_ibl.rgb = spec_ibl.rgb * spec_ibl.a;
			// fresnel based on spec_albedo.rgb and roughness (spec_albedo.a) / taken from: http://seblagarde.wordpress.com/2011/08/17/hello-world/
			float3 FresnelSchlickWithRoughness = o.SpecularColor + ( max(o.Specular, o.SpecularColor ) - o.SpecularColor) * exp2(-OneOnLN2_x6 * NdotV);
			// colorize fresnel highlights and make it look like marmoset:
			// float3 FresnelSchlickWithRoughness = o.SpecularColor + o.Specular.xxx * o.SpecularColor * exp2(-OneOnLN2_x6 * NdotV);	
			spec_ibl.rgb *= FresnelSchlickWithRoughness * ExposureIBL.y;
			// add diffuse and specular and conserve energy
			o.Emission = (1 - spec_ibl.rgb) * o.Emission + spec_ibl.rgb;

		#endif
		
		#ifdef LUX_AO_ON
			#if !defined(LUX_AO_SAMPLED)
				half ambientOcclusion = tex2D(_AO,IN.uv_AO).a;
				o.Emission *= ambientOcclusion;
			#else
				o.Emission *= ambientOcclusion.a;
			#endif
		#endif

		#ifdef LUX_METALNESS
			o.Emission *= spec_albedo.g;
		#endif

#endif
