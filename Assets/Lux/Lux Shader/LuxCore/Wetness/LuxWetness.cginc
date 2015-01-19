#ifndef LuxWetness_CG_INCLUDED
#define LuxWetness_CG_INCLUDED

// Material specific Wetness Properties
float _WetnessWorldNormalDamp;
float _WetnessHeightMapInfluence;


// Properties needed by heightmap based wetness shaders
#if defined (Lux_WaterRipples) || defined (Lux_WaterFlow)
    sampler2D _HeightWetness;
    float4 _MainTex_ST;
#endif
#if defined (LUX_AO_ON) || !defined (WetnessMaskInputVertexColors)
    #if defined (Lux_WaterRipples) || defined (Lux_WaterFlow)
        float4 _AO_ST;
    #endif
#endif

// Properties only needed by flow based wetness shaders
#ifdef Lux_WaterFlow
    sampler2D _WaterBumpMap;
    float _WaterBumpScale;
    float _FlowSpeed;
    float _FlowHeightScale;
    float _FlowRefraction;
#endif


// Global Rain Properties passed in by Script
float2 _Lux_WaterFloodlevel;
float _Lux_RainIntensity;
sampler2D _Lux_RainRipples;
float4 _Lux_RippleWindSpeed;
float _Lux_RippleAnimSpeed;
float _Lux_RippleTiling;
float _Lux_WaterBumpDistance;



//	Based on the work of Sébastien Lagarde
//  http://seblagarde.wordpress.com/2013/04/14/water-drop-3b-physically-based-wet-surfaces/


inline float2 ComputeRipple(float4 UV, float CurrentTime, float Weight)
{
    float4 Ripple = tex2Dlod(_Lux_RainRipples, UV);
    // We use multi sampling here in order to improve Sharpness due to the lack of Anisotropic Filtering when using tex2Dlod
    Ripple += tex2Dlod(_Lux_RainRipples, float4(UV.xy, UV.zw * 0.5) );
    Ripple *= 0.5;

    Ripple.yz = Ripple.yz * 2 - 1; // Decompress Normal
    float DropFrac = frac(Ripple.w + CurrentTime); // Apply time shift
    float TimeFrac = DropFrac - 1.0f + Ripple.x;
    float DropFactor = saturate(0.2f + Weight * 0.8f - DropFrac);
    float FinalFactor = DropFactor * Ripple.x * sin( clamp(TimeFrac * 9.0f, 0.0f, 3.0f) * 3.141592653589793);
    return Ripple.yz * FinalFactor * 0.35f;
}

//	Accumulate water in cracks(x) and puddles(y)
inline float2 ComputeWaterAccumulation(float i_puddlemask, float2 i_heightmask, float i_worlnormalface_y )
{
	float2 AccumulatedWaters;
	// Get the "size" of the accumulated water in cracks
    AccumulatedWaters.x = min(_Lux_WaterFloodlevel.x, 1.0 - i_heightmask.x * _WetnessHeightMapInfluence ); // = Heightmap);
	// Get the size of the accumlated water in puddles taking into account the marging (0.4 constant here).
    // AccumulatedWaters.y = saturate((_Wetness.y - PuddleMask ) / 0.4);
    // First we shrink the Puddle by i_worlnormalface_y
    // In order to improve the edges we also take the Heightmap and PuddleNoise into account.
    AccumulatedWaters.y = saturate((_Lux_WaterFloodlevel.y * i_worlnormalface_y  - i_puddlemask - i_heightmask.x - i_heightmask.y * (1-_Lux_WaterFloodlevel.y) ) / 0.4)  ;
    // Damp overall WaterAccumulation according to the worldNormal.y Component
    float worldNormalDamp = saturate( saturate(i_worlnormalface_y) + _WetnessWorldNormalDamp);    
	// return float2( max(AccumulatedWaters.x, AccumulatedWaters.y) );
    return float2(max(AccumulatedWaters.x, AccumulatedWaters.y), max(AccumulatedWaters.x, AccumulatedWaters.y * 2) * 0.5) * worldNormalDamp.xx;
}

//	Add animated Ripples to areas where the Water Accumulation is high enough
//  Returns the tweaked and adjusted Ripple Normal
inline float3 AddWaterRipples(float2 i_wetFactor, float3 i_worldPos, float2 lambda, float fadeOutWaterBumps )
{
    float4 Weights = _Lux_RainIntensity - float4(0, 0.25, 0.5, 0.75);
    Weights = saturate(Weights * 4);
    float animSpeed =  _Time.y * _Lux_RippleAnimSpeed;
    float2 Ripple1 = ComputeRipple( float4(i_worldPos.xz * _Lux_RippleTiling + float2( 0.25f,0.0f) + _Lux_RippleWindSpeed.xy * _Time.y, lambda ), animSpeed, Weights.x);
    float2 Ripple2 = ComputeRipple( float4(i_worldPos.xz * _Lux_RippleTiling + float2(-0.55f,0.3f) + _Lux_RippleWindSpeed.zw * _Time.y, lambda ), animSpeed * 0.71, Weights.y); 
    float3 rippleNormal = float3( Weights.x * Ripple1.xy + Weights.y * Ripple2.xy, 1);
    // Blend and fade out Ripples
    return lerp( float3(0,0,1), rippleNormal, i_wetFactor.y * i_wetFactor.y * fadeOutWaterBumps );
}


//  Water flow specific functions
#ifdef Lux_WaterFlow
    // Add water flow based on slope

    // http://www.heathershrewsbury.com/dreu2010/wp-content/uploads/2010/07/FlowVisualizationUsingMovingTextures.pdf
    // During the animation of the texture advection, this distortion continues to build up, so that eventually the visualization will become useless.
    // Therefore we periodically restart the texture coordinates back at their original positions in the regular grid. --> frac
    // To avoid the sudden jump this would cause in the animation, we gradually fade up the new texture and fade down the old one

    inline fixed3 AddWaterFlow(float2 MainUV, float2 flowDirection, float worlNormalY, float overallWetness, float2 lambda, float fadeOutWaterBumps)
    {
        float2 flowUV = MainUV * _WaterBumpScale;
        float2 fracTime = frac(float2(_Time.y, _Time.y + 0.5));
        float fade = abs(fracTime.x * 2 - 1);
        flowDirection *= _FlowSpeed;
        float4 flowBump = tex2Dlod(_WaterBumpMap, float4(flowUV + fracTime.xx * flowDirection, lambda)  );
        flowBump = lerp(flowBump, tex2Dlod(_WaterBumpMap, float4(flowUV + fracTime.yy * flowDirection, lambda) ), fade);
        fixed3 finalflowBump = UnpackNormal(flowBump);
        // Mask water flow according to overallWetness and scale down flow normal according to speed (worldNormalFace.y)
        worlNormalY = saturate(worlNormalY);
        return lerp(float3(0,0,1), finalflowBump, (1 - worlNormalY * worlNormalY) * overallWetness ) * float3( _FlowHeightScale, _FlowHeightScale, 1) * fadeOutWaterBumps;
    } 

    //  Add Water Ripples to Waterflow
    inline float3 AddWaterFlowRipples(float2 i_wetFactor, float3 i_worldPos, float2 lambda, float i_worldNormalFaceY, float fadeOutWaterBumps)
    {
        float4 Weights = _Lux_RainIntensity - float4(0, 0.25, 0.5, 0.75);
        Weights = saturate(Weights * 4);
        float animSpeed =  _Time.y * _Lux_RippleAnimSpeed;
        float2 Ripple1 = ComputeRipple( float4(i_worldPos.xz * _Lux_RippleTiling + float2( 0.25f,0.0f), lambda ), animSpeed, Weights.x);
        float2 Ripple2 = ComputeRipple( float4(i_worldPos.xz * _Lux_RippleTiling + float2(-0.55f,0.3f), lambda ), animSpeed * 0.71, Weights.y); 
        float3 rippleNormal = float3( Weights.x * Ripple1.xy + Weights.y * Ripple2.xy, 1);
        // Blend and fade out Ripples
        return lerp( float3(0,0,1), rippleNormal, i_wetFactor.y * i_wetFactor.y * fadeOutWaterBumps * i_worldNormalFaceY*i_worldNormalFaceY );
    } 
#endif

//	Tweak all BRDF Inputs according to calculated Wetness/Porosity
inline fixed WaterBRDF (inout fixed3 Albedo, fixed Specular, fixed3 SpecularColor, fixed wetFactor) {
	float porosity = saturate((( (1 - Specular) - 0.5)) / 0.4 );
    // Materials (like metal) which are not porose should not be darkened, so we have to find the metal parts:
    // As metals have high specular color values (>0.33) we can use SpecularColor to identify those
    float metalness = saturate((dot(SpecularColor, 0.33) * 1000 - 500) );
    float factor = lerp(1, 0.2, (1 - metalness) * porosity);
	// Attenuate Albedo
	Albedo *= lerp(1.0, factor, wetFactor);	
	// Tweak SpecularColor towards Water / Water F0 specular is 0.02 (based on IOR of 1.33)
	SpecularColor = lerp(SpecularColor, 0.02, wetFactor);
	// Calc Roughness
	return lerp(1.0, Specular, lerp(1.0, factor, wetFactor) );
}

#endif