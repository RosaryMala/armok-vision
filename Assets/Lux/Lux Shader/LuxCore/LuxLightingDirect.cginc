#ifndef LuxLightingDirect_CG_INCLUDED
#define LuxLightingDirect_CG_INCLUDED

// Lux Lighting Functions

#define OneOnLN2_x6 8.656170
#define Pi 3.14159265358979323846

struct SurfaceOutputLux {
	half3 Albedo;
	half3 Normal;
	half3 Emission;
	half Specular;
	half3 SpecularColor;
	half Alpha;
	half DeferredFresnel;
};


// for Cook Torrence spec or roughness has to be in linear space
half LuxAdjustSpecular(half spec) {
	#if defined(LUX_LIGHTING_CT)
		return clamp(pow(spec, 1/2.2), 0.0, 0.996);
	#else
		return spec;
	#endif
}


/////////////////////////////// deferred lighting / uses faked fresnel

inline fixed4 LightingLuxDirect_PrePass (SurfaceOutputLux s, half4 light)
{
	// light.a is "compressed" to fit into the 0-1 range using log2(x + 1) which is the best compromise i have found
	fixed spec = exp2(light.a) - 1;
	fixed4 c;
//	Diffuse
	c.rgb = s.Albedo * light.rgb;
//	Specular
	//s.DeferredFresnel based on dot N V (faked fresnel as it should be dot H V)
	#if !defined (LUX_DIFFUSE)
		c.rgb += (s.SpecularColor.rgb + ( 1.0 - s.SpecularColor.rgb) * s.DeferredFresnel.x) * spec * light.rgb;
//	#else
//		c.rgb += s.SpecularColor.rgb * spec * light.rgb;
	#endif
	// this here is not really worth it:
	// do not use light.rgb but only the cromatic part of it as we have stores luninance already in the lighting pass
	// half3 luminanceSensivity = half3(0.299,0.587, 0.114); 
	// half crominanceSpecLight = spec / (dot(light.rgb, luminanceSensivity) + 0.0001);
	// see: http://www.realtimerendering.com/blog/deferred-lighting-approaches/
	// c.rgb += (s.SpecularColor.rgb + ( 1.0 - s.SpecularColor.rgb) * s.DeferredFresnel) * s.SpecularColor.rgb * light.rgb * crominanceSpecLight;
	c.a = s.Alpha; // + spec;
	return c;
}

/////////////////////////////// forward lighting

float4 LightingLuxDirect (SurfaceOutputLux s, fixed3 lightDir, half3 viewDir, fixed atten){
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
	c.rgb = (s.Albedo + fresnel) * _LightColor0.rgb * dotNL * (atten * 2);
	c.a = s.Alpha; // + _LightColor0.a * fresnel * atten;
	return c;
}

//////////////////////////////// directional lightmaps

inline half4 LightingLuxDirect_DirLightmap (SurfaceOutputLux s, fixed4 color, fixed4 scale, half3 viewDir, bool surfFuncWritesNormal, out half3 specColor)
{
	UNITY_DIRBASIS
	half3 scalePerBasisVector;
	
	half3 lm = DirLightmapDiffuse (unity_DirBasis, color, scale, s.Normal, surfFuncWritesNormal, scalePerBasisVector);
	
	half3 lightDir = normalize (scalePerBasisVector.x * unity_DirBasis[0] + scalePerBasisVector.y * unity_DirBasis[1] + scalePerBasisVector.z * unity_DirBasis[2]);
	half3 h = normalize (lightDir + viewDir);
	
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
	// in order to make deferred fit forward lighting better we have to tweak roughness here
	// roughness = pow(roughness, .25);
	float V_ONE = dotNL + sqrt(alpha2 + (1 - alpha2) * dotNL * dotNL );
	float V_TWO = dotNV + sqrt(alpha2 + (1 - alpha2) * dotNV * dotNV );
	spec /= V_ONE * V_TWO;
	#endif
	
//	Fresnel: Schlick
	// fixed3 fresnel = s.SpecularColor.rgb + ( 1.0 - s.SpecularColor.rgb) * pow(1.0f - saturate(dot(h, lightDir)), 5);
	// fast fresnel approximation:
	fixed3 fresnel = s.SpecularColor.rgb + ( 1.0 - s.SpecularColor.rgb) * exp2(-OneOnLN2_x6 * dot(h, lightDir));

	// from here on we use fresnel (in forward) instead of spec as it is fixed3 = color
	fresnel *= spec;
	// or spec for deferred (no fresnel term applied)

	// specColor used outside in the forward path, compiled out in prepass
	// here we drop spec and go with fresnel instead as it is float3
//	forward
	//specColor = lm * _SpecColor.rgb * s.Gloss * spec;
	specColor = lm * fresnel;
//	deferred	
	// spec from the alpha component is used to calculate specular
	// in the Lighting*_Prepass function, it's not used in forward
	// we have to compress spec like we do in the "Intrenal-PrepassLighting" shader
	return half4(lm, log2(spec + 1));
}
#endif
