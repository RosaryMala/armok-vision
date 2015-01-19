#ifndef TREE_CG_INCLUDED
#define TREE_CG_INCLUDED

#include "TerrainEngine.cginc"

fixed4 _Color;
fixed3 _TranslucencyColor;
fixed _TranslucencyViewDependency;
half _ShadowStrength;

struct LeafSurfaceOutput {
	fixed3 Albedo;
	fixed3 Normal;
	fixed3 Emission;
	fixed Translucency;
	half Specular;
	fixed Gloss;
	//fixed3 SpecularColor;
	//fixed Specular;
	fixed Alpha;
};

// important
#define X_OneOnLN2_x6 8.656170


inline half4 LightingTreeLeaf (LeafSurfaceOutput s, half3 lightDir, half3 viewDir, half atten)
{
	
	// Tree leafs always us BlinnPhong
	half3 h = normalize (lightDir + viewDir);
	half dotNL = dot (s.Normal, lightDir);
	half dotNH = max (0, dot (s.Normal, h));

	//float specPower = exp2(10 * s.Specular + 1) - 1.75;
	//float spec = specPower * 0.125 * pow(dotNH, specPower);
	half spec = pow (dotNH, s.Specular * 128.0) * s.Gloss;

	// view dependent back contribution for translucency
	fixed backContrib = saturate(dot(viewDir, -lightDir));

	// normally translucency is more like -nl, but looks better when it's view dependent
	backContrib = lerp(saturate(-dotNL), backContrib, _TranslucencyViewDependency);
	fixed3 translucencyColor = backContrib * s.Translucency * _TranslucencyColor;
	
//	Visibility: Schlick-Smith
	// we skip this here...

//	Fresnel: Schlick
//	fixed3 fresnel = s.SpecularColor.rgb + ( 1.0 - s.SpecularColor.rgb) * exp2(-X_OneOnLN2_x6 * dot(h, lightDir));
	// from here on we use fresnel instead of spec as it is fixed3 = color
//	fresnel *= spec;

//	spec *= s.SpecularColor.r;

	// wrap-around diffuse
	dotNL = max(0, dotNL * 0.6 + 0.4);
	
	fixed4 c;
	c.rgb = s.Albedo * (translucencyColor * 2 + dotNL);
//	c.rgb += fresnel * max (0, dotNL);
//	c.rgb += spec * max (0, dotNL);
//	c.rgb *= _LightColor0.rgb;
c.rgb = (c.rgb + spec) * _LightColor0.rgb;
	
	// For directional lights, apply less shadow attenuation
	// based on shadow strength parameter.
	#if defined(DIRECTIONAL) || defined(DIRECTIONAL_COOKIE)
	c.rgb *= lerp(2, atten * 2, _ShadowStrength);
	#else
	c.rgb *= 2*atten;
	#endif
	
	return c;
}

#endif