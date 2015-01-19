#ifndef LuxCustomFog_CG_INCLUDED
#define LuxCustomFog_CG_INCLUDED

half4 unity_FogColor;
half4 unity_FogDensity;
half4 unity_FogStart;
half4 unity_FogEnd;

// Fog linear
void customFogLinear (Input IN, SurfaceOutputLux o, inout fixed4 color)
{
	#if !defined (UNITY_PASS_FORWARDADD)
		float fogFactor = saturate((unity_FogEnd.x - LUX_CAMERADISTANCE) / (unity_FogEnd.x - unity_FogStart.x));
		color.rgb = lerp(unity_FogColor, color.rgb, fogFactor);
	#endif
}
// Fog Exp
void customFogExp (Input IN, SurfaceOutputLux o, inout fixed4 color)
{
	#if !defined (UNITY_PASS_FORWARDADD)
		float f = LUX_CAMERADISTANCE * unity_FogDensity;
		float fogFactor = saturate(1 / pow(2.71828,  f));
		color.rgb = lerp(unity_FogColor, color.rgb, fogFactor);
	#endif
}
// Fog Exp2
void customFogExp2 (Input IN, SurfaceOutputLux o, inout fixed4 color)
{
	#if !defined (UNITY_PASS_FORWARDADD)
		float f = LUX_CAMERADISTANCE * unity_FogDensity;
		float fogFactor = saturate(1 / pow(2.71828,  f * f));
		color.rgb = lerp(unity_FogColor, color.rgb, fogFactor);
	#endif
}
#endif