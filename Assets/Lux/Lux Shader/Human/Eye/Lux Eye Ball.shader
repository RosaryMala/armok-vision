Shader "Lux/Human/Eye Ball" {
	Properties {
		_MainTex ("Base (RGB) Heightmap (A)", 2D) = "white" {}
		_SpecTex ("Specular Color (RGB) Roughness (A)", 2D) = "black" {}
		_BumpMap ("Normalmap", 2D) = "bump" {}
		_DiffCubeIBL ("Custom Diffuse Cube", Cube) = "black" {}
		_SpecCubeIBL ("Custom Specular Cube", Cube) = "black" {}

		_PupilSize ("PupilSize", Range (0.01, 1)) = 0.5
		_Parallax ("Height", Range (0.005, 0.08)) = 0.08

		// _Shininess property is needed by the lightmapper - otherwise it throws errors
		[HideInInspector] _Shininess ("Shininess (only for Lightmapper)", Float) = 0.5

	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200

		CGPROGRAM
		#pragma surface surf LuxDirectEye fullforwardshadows exclude_path:prepass nolightmap nodirlightmap noambient 
		#pragma glsl
		#pragma target 3.0

		#pragma multi_compile LUX_LIGHTING_BP LUX_LIGHTING_CT
		#pragma multi_compile LUX_LINEAR LUX_GAMMA
		#pragma multi_compile DIFFCUBE_ON DIFFCUBE_OFF
		#pragma multi_compile SPECCUBE_ON SPECCUBE_OFF

//		#define LUX_LIGHTING_BP
//		#define LUX_LINEAR
//		#define DIFFCUBE_ON
//		#define SPECCUBE_ON

		// We cant use AO here as the eye ball tends to move...
		#define LUX_AO_OFF

		// include should be called after all defines
		#include "../../LuxCore/LuxLightingDirect.cginc"

		sampler2D _MainTex;
		sampler2D _SpecTex;
		sampler2D _BumpMap;
		#ifdef DIFFCUBE_ON
			samplerCUBE _DiffCubeIBL;
		#endif
		#ifdef SPECCUBE_ON
			samplerCUBE _SpecCubeIBL;
		#endif

		// shader specific inputs
		float _PupilSize;
		float _Parallax;

		// Is set by script
		float4 ExposureIBL;

		struct Input {
			float2 uv_MainTex;
			float3 viewDir;
			float3 worldNormal;
			float3 worldRefl;
			INTERNAL_DATA
		};

		void surf (Input IN, inout SurfaceOutputLux o) {

		//	Calculate Parallax
			half h = tex2D (_MainTex, IN.uv_MainTex).a;
			float2 offset = ParallaxOffset (h, _Parallax, IN.viewDir);

		//	Apply Pupil Size
			float2 UVs = IN.uv_MainTex;
			float2 delta = float2(0.5, 0.5) - UVs;
			// Calculate pow(distance,2) to center (pythagoras...)
			float factor = (delta.x*delta.x + delta.y*delta.y); 
			// Clamp it in order to mask our pixels, then bring it back into 0 - 1 range
			// Max distance = 0.15 --> pow(max,2) = 0.0225
			factor = saturate(0.0225 - factor) * 44.444;
			UVs += delta * factor * _PupilSize;

		//	Now sample albedo and spec maps according to the pupil’s size and parallax
			o.Albedo = tex2D(_MainTex, UVs + offset).rgb;
			o.Alpha = 1;
			fixed4 spec_albedo = tex2D(_SpecTex, UVs + offset);
		//	Normal map
			o.Normal = UnpackNormal(tex2D(_BumpMap, IN.uv_MainTex - offset));
		//	Specular Color
			o.SpecularColor = spec_albedo.rgb;
		//	Roughness – gamma for BlinnPhong / linear for CookTorrence
			o.Specular = LuxAdjustSpecular(spec_albedo.a);

			//#include "../../LuxCore/LuxLightingAmbient.cginc"



		//	///////////////////////////////////////
		// 	Further functions to keep the surf function rather simple
			
			#ifdef SPECCUBE_ON
				float NdotV = max(0, dot(o.Normal, normalize(IN.viewDir.xyz)));
			#endif

		//	///////////////////////////////////////
		// 	Lux IBL / ambient lighting
			
			// set o.Emission = 0.0 to make diffuse shaders work correctly
			o.Emission = 0.0;
			float3 worldNormal = WorldNormalVector(IN, float3( -o.Normal.x, -o.Normal.y, o.Normal.z));

		//	add diffuse IBL
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
				o.Emission = ShadeSH9 ( float4(worldNormal.xyz, 1.0)) * o.Albedo;
			#endif
			
		//	add specular IBL		
			#ifdef SPECCUBE_ON
				half3 worldRefl = WorldReflectionVector (IN, o.Normal);	

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


		}


		/////////////////////////////// forward lighting

		float4 LightingLuxDirectEye (SurfaceOutputLux s, fixed3 lightDir, half3 viewDir, fixed atten){
		  	// get base variables

		  	// normalizing lightDir makes fresnel smoother
			lightDir = normalize(lightDir);
			// normalizing viewDir does not help here, so we skip it
			half3 h = normalize (lightDir + viewDir);
			// dotNL has to have max
			float dotNL = max (0, dot (s.Normal, lightDir));
			float dotNH = max (0, dot (s.Normal, h));
			
			#if !defined (LUX_LIGHTING_BP) && !defined (LUX_LIGHTING_CT)
				#define LUX_LIGHTING_BP
			#endif

		//	////////////////////////////////////////////////////////////
		//	Blinn Phong	
			#if defined (LUX_LIGHTING_BP)
			// bring specPower into a range of 0.25 – 2048
			float specPower = exp2(10 * s.Specular + 1) - 1.75;

		//	Normalized Lighting Model:
			// L = (c_diff * dotNL + F_Schlick(c_spec, l_c, h) * ( (spec + 2)/8) * dotNH˄spec * dotNL) * c_light
			
		//	Specular: Phong lobe normal distribution function
			//float spec = ((specPower + 2.0) * 0.125 ) * pow(dotNH, specPower) * dotNL; // would be the correct term
			// we use late * dotNL to get rid of any artifacts on the backsides
			float spec = specPower * 0.125 * pow(dotNH, specPower);

		//	Visibility: Schlick-Smith
			float alpha = 2.0 / sqrt( Pi * (specPower + 2) );
			float visibility = 1.0 / ( (dotNL * (1 - alpha) + alpha) * ( saturate(dot(s.Normal, viewDir)) * (1 - alpha) + alpha) ); 
			spec *= visibility;
			#endif
			
		//	////////////////////////////////////////////////////////////
		//	Cook Torrrence like
		//	from The Order 1886 // http://blog.selfshadow.com/publications/s2013-shading-course/rad/s2013_pbs_rad_notes.pdf

			#if defined (LUX_LIGHTING_CT)
			float dotNV = max(0, dot(s.Normal, normalize(viewDir) ) );

		//	Please note: s.Specular must be linear
			float alpha = (1.0 - s.Specular); // alpha is roughness
			alpha *= alpha;
			float alpha2 = alpha * alpha;

		//	Specular Normal Distribution Function: GGX Trowbridge Reitz
			float denominator = (dotNH * dotNH) * (alpha2 - 1) + 1;
			denominator = Pi * denominator * denominator;
			float spec = alpha2 / denominator;

		//	Geometric Shadowing: Smith
			float V_ONE = dotNL + sqrt(alpha2 + (1 - alpha2) * dotNL * dotNL );
			float V_TWO = dotNV + sqrt(alpha2 + (1 - alpha2) * dotNV * dotNV );
			spec /= V_ONE * V_TWO;
			#endif

		//	Fresnel: Schlick
			// fast fresnel approximation:
			fixed3 fresnel = s.SpecularColor.rgb + ( 1.0 - s.SpecularColor.rgb) * exp2(-OneOnLN2_x6 * dot(h, lightDir));
			// from here on we use fresnel instead of spec as it is fixed3 = color
			fresnel *= spec;
			
		// Final Composition
			fixed4 c;
			// we only use fresnel here / and apply late dotNL

			// we have to use the "concave" normal for diffuse here 
			//s.Albedo
			c.rgb = ( s.Albedo * _LightColor0.rgb * max (0, dot ( float3( -s.Normal.x, -s.Normal.y, s.Normal.z), lightDir)) + fresnel * _LightColor0.rgb * dotNL) * (atten * 2);

			c.a = s.Alpha; // + _LightColor0.a * fresnel * atten;
			return c;
		}


		ENDCG
	} 
	FallBack "Diffuse"
	CustomEditor "LuxMaterialInspector"
}