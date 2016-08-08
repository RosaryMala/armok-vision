Shader "Hidden/DepthOfField/DepthOfField"
{
    Properties
    {
        _MainTex ("-", 2D) = "black"
        _SecondTex ("-", 2D) = "black"
        _ThirdTex ("-", 2D) = "black"
    }

    CGINCLUDE

        #pragma target 3.0
        #pragma fragmentoption ARB_precision_hint_fastest
        #include "UnityCG.cginc"

        // Undef USE_LOCAL_TONEMAPPING if you dont want to use local tonemapping.
        // Tweaking these values down will trade stability for less bokeh (see Tonemap/TonemapInvert methods below).
        #ifndef SHADER_API_MOBILE
            #define USE_LOCAL_TONEMAPPING
        #endif
        #define LOCAL_TONEMAP_START_LUMA 1.0
        #define LOCAL_TONEMAP_RANGE_LUMA 5.0

        sampler2D _SecondTex;
        sampler2D _ThirdTex;

        uniform half4 _MainTex_TexelSize;
        uniform half4 _BlurCoe;
        uniform half4 _BlurParams;
        uniform half4 _Convolved_TexelSize;
        uniform float4 _Offsets;

        uniform half4 _MainTex_ST;
        uniform half4 _SecondTex_ST;
        uniform half4 _ThirdTex_ST;

        #if (SHADER_TARGET >= 50 && !defined(SHADER_API_PSSL))
            #define USE_TEX2DOBJECT_FOR_COC
        #endif

        #if defined(USE_TEX2DOBJECT_FOR_COC)
            Texture2D _CameraDepthTexture;
            SamplerState sampler_CameraDepthTexture;
            Texture2D _MainTex;
            SamplerState sampler_MainTex;
        #else
            sampler2D _MainTex;
            sampler2D _CameraDepthTexture;
        #endif

        ///////////////////////////////////////////////////////////////////////////////
        // Verter Shaders and declaration
        ///////////////////////////////////////////////////////////////////////////////

        struct v2f
        {
            float4 pos : SV_POSITION;
            float2 uv  : TEXCOORD0;
            float2 uv1 : TEXCOORD1;
        };

        struct v2fDepth
        {
            half4 pos  : SV_POSITION;
            half2 uv   : TEXCOORD0;
        };

        struct v2fBlur
        {
            float4 pos  : SV_POSITION;
            float2 uv   : TEXCOORD0;
            float4 uv01 : TEXCOORD1;
            float4 uv23 : TEXCOORD2;
            float4 uv45 : TEXCOORD3;
            float4 uv67 : TEXCOORD4;
            float4 uv89 : TEXCOORD5;
        };

        v2fDepth vert(appdata_img v)
        {
            v2fDepth o;
            o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
            o.uv = v.texcoord.xy;

        #if UNITY_UV_STARTS_AT_TOP
            if (_MainTex_TexelSize.y < 0.0)
                o.uv.y = 1.0 - o.uv.y;
        #endif

            return o;
        }

        v2fDepth vertNoFlip(appdata_img v)
        {
            v2fDepth o;
            o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
            o.uv = v.texcoord.xy;
            return o;
        }

        v2f vert_d(appdata_img v)
        {
            v2f o;
            o.pos = mul (UNITY_MATRIX_MVP, v.vertex);
            o.uv1.xy = v.texcoord.xy;
            o.uv.xy = v.texcoord.xy;

        #if UNITY_UV_STARTS_AT_TOP
            if (_MainTex_TexelSize.y < 0.0)
                o.uv.y = 1.0 - o.uv.y;
        #endif

            return o;
        }

        v2f vertFlip(appdata_img v)
        {
            v2f o;
            o.pos = mul (UNITY_MATRIX_MVP, v.vertex);
            o.uv1.xy = v.texcoord.xy;
            o.uv.xy = v.texcoord.xy;

        #if UNITY_UV_STARTS_AT_TOP
            if (_MainTex_TexelSize.y < 0.0)
            {
                o.uv.y = 1.0 - o.uv.y;
                o.uv1.y = 1.0 - o.uv1.y;
            }
        #endif

            return o;
        }

        v2fBlur vertBlurPlusMinus(appdata_img v)
        {
            v2fBlur o;
            o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
            o.uv.xy = v.texcoord.xy;
            o.uv01 =  v.texcoord.xyxy + _Offsets.xyxy * float4(1,1, -1,-1) * _MainTex_TexelSize.xyxy / 6.0;
            o.uv23 =  v.texcoord.xyxy + _Offsets.xyxy * float4(2,2, -2,-2) * _MainTex_TexelSize.xyxy / 6.0;
            o.uv45 =  v.texcoord.xyxy + _Offsets.xyxy * float4(3,3, -3,-3) * _MainTex_TexelSize.xyxy / 6.0;
            o.uv67 =  v.texcoord.xyxy + _Offsets.xyxy * float4(4,4, -4,-4) * _MainTex_TexelSize.xyxy / 6.0;
            o.uv89 =  v.texcoord.xyxy + _Offsets.xyxy * float4(5,5, -5,-5) * _MainTex_TexelSize.xyxy / 6.0;
            return o;
        }

        ///////////////////////////////////////////////////////////////////////////////
        // Helpers
        ///////////////////////////////////////////////////////////////////////////////

        inline half4 FetchMainTex(float2 uv)
        {
        #if defined(USE_TEX2DOBJECT_FOR_COC)
            return _MainTex.SampleLevel(sampler_MainTex, uv, 0.0);
        #else
            return tex2Dlod(_MainTex, float4(uv, 0.0, 0.0));
        #endif
        }

        inline half2 GetBilinearFetchTexOffsetFromAbsCoc(half4 absCoc)
        {
            half4 cocWeights = absCoc * absCoc * absCoc;

            half2 offset = 0.0;
            offset += cocWeights.r * float2(-1.0, 1.0);
            offset += cocWeights.g * float2( 1.0, 1.0);
            offset += cocWeights.b * float2( 1.0,-1.0);
            offset += cocWeights.a * float2(-1.0,-1.0);
            offset = clamp((half2)-1.0,(half2)1.0, offset);
            offset *= 0.5;
            return offset;
        }

        inline half4 FetchColorAndCocFromMainTex(float2 uv, float2 offsetFromKernelCenter)
        {
            // Bilinear
            half4 fetch =  FetchMainTex(uv);

            // CoC can't be linearly interpolated while doing "scatter and gather" or we will have haloing where coc vary sharply.
            #if defined(USE_TEX2DOBJECT_FOR_COC)
                half4 allCoc = _MainTex.GatherAlpha(sampler_MainTex, uv);
                half cocAB = (abs(allCoc.r) < abs(allCoc.g)) ? allCoc.r : allCoc.g;
                half cocCD = (abs(allCoc.b) < abs(allCoc.a)) ? allCoc.b : allCoc.a;
                half coc = (abs(cocAB) < abs(cocCD)) ? cocAB : cocCD;
            #else
                // No gather available -> instead point sample the coc from the fartest away texel (not as good).
                half2 bilinearCenter = floor(uv * _MainTex_TexelSize.zw - 0.5) + 1.0;
                half2 cocUV = bilinearCenter + 0.5 * sign(offsetFromKernelCenter);
                half coc = tex2Dlod(_MainTex, float4(cocUV * _MainTex_TexelSize.xy, 0.0, 0.0)).a;
            #endif

            fetch.a = coc;
            return fetch;
        }

        inline half GetCocFromZValue(half d)
        {
            d = Linear01Depth(d);
            half coc = d < _BlurCoe.z
                ? clamp((_BlurParams.x * d + _BlurParams.y), -1.0, 0.0)
                : saturate((_BlurParams.z * d + _BlurParams.w));
            return coc;
        }

        inline half GetCocFromDepth(half2 uv)
        {
        #if defined(USE_TEX2DOBJECT_FOR_COC)
            half d = _CameraDepthTexture.SampleLevel(sampler_CameraDepthTexture, uv, 0.0);
        #else
            half d = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, uv);
        #endif
            return GetCocFromZValue(d);
        }

    #if (SHADER_TARGET < 50 && !defined(SHADER_API_PSSL))
        half rcp(half x)
        {
            return 1.0 / x;
        }
    #endif

        // From http://graphicrants.blogspot.dk/2013/12/tone-mapping.html
        inline half3 Tonemap(half3 color)
        {
        #ifdef USE_LOCAL_TONEMAPPING
            half a = LOCAL_TONEMAP_START_LUMA;
            half b = LOCAL_TONEMAP_RANGE_LUMA;

            half luma = max(color.r, max(color.g, color.b));
            if (luma <= a)
                return color;

            return color * rcp(luma) * (a * a - b * luma) / (2.0 * a - b - luma);
        #else
            return color;
        #endif
        }

        inline half3 TonemapInvert(half3 color)
        {
        #ifdef USE_LOCAL_TONEMAPPING
            half a = LOCAL_TONEMAP_START_LUMA;
            half b = LOCAL_TONEMAP_RANGE_LUMA;

            half luma = max(color.r, max(color.g, color.b));
            if (luma <= a)
                return color;

            return color * rcp(luma) * (a * a - (2.0 * a - b) * luma) / (b - luma);
        #else
            return color;
        #endif
        }


        ///////////////////////////////////////////////////////////////////////////////
        // Directional (hexagonal/octogonal) bokeh
        ///////////////////////////////////////////////////////////////////////////////

        #define SAMPLE_NUM_L    6
        #define SAMPLE_NUM_M    11
        #define SAMPLE_NUM_H    16

        inline half4 shapeDirectionalBlur(half2 uv, bool mergePass, int numSample, bool sampleDilatedFG)
        {
            half4 centerTap = FetchMainTex(uv);
            half  fgCoc = centerTap.a;
            half  fgBlendFromPreviousPass = centerTap.a * _Offsets.z;

            if (sampleDilatedFG)
            {
                half2 cocs = tex2Dlod(_SecondTex, half4(uv, 0.0, 0.0)).rg;
                fgCoc = min(cocs.r, cocs.g);
                centerTap.a = cocs.g;
            }

            half bgRadius = smoothstep(0.0, 0.85, centerTap.a) * _BlurCoe.y;
            half fgRadius = smoothstep(0.0, 0.85, -fgCoc) * _BlurCoe.x;
            half radius = max(bgRadius, fgRadius);

            if (radius < 1e-2)
                return half4(centerTap.rgb, (sampleDilatedFG || mergePass) ? fgBlendFromPreviousPass : centerTap.a);

            half radOtherFgRad = radius / (fgRadius + 1e-2);
            half radOtherBgRad = radius / (bgRadius + 1e-2);
            half2 range = radius * _MainTex_TexelSize.xy;

            half fgWeight = 1e-3;
            half bgWeight = 1e-3;
            half3 fgSum = half3(0.0, 0.0, 0.0);
            half3 bgSum = half3(0.0, 0.0, 0.0);

            for (int k = 0; k < numSample; k++)
            {
                half t = (half)k / half(numSample - 1.0);
                half2 kVal = lerp(_Offsets.xy, -_Offsets.xy, t);
                half2 offset = kVal * range;
                half2 texCoord = uv + offset;
                half4 sample0 = FetchColorAndCocFromMainTex(texCoord, offset);

                if (sampleDilatedFG)
                    sample0.a = tex2Dlod(_SecondTex, half4(texCoord, 0.0, 0.0)).g;

                half dist = abs(2.0 * t - 1.0);
                half distanceFactor = saturate(-0.5 * abs(sample0.a - centerTap.a) * dist + 1.0);
                half isNear = max(0.0, -sample0.a);
                half isFar  = max(0.0, sample0.a)  * distanceFactor;
                isNear *= 1.0 - smoothstep(1.0, 2.0, dist * radOtherFgRad);
                isFar  *= 1.0 - smoothstep(1.0, 2.0, dist * radOtherBgRad);

                fgWeight += isNear;
                fgSum += sample0.rgb * isNear;
                bgWeight += isFar;
                bgSum += sample0.rgb * isFar;
            }

            half3 fgColor = fgSum / (fgWeight + 1e-4);
            half3 bgColor = bgSum / (bgWeight + 1e-4);
            half bgBlend = saturate(2.0 * bgWeight / numSample);
            half fgBlend = saturate(2.0 * fgWeight / numSample);

            half3 finalBg = lerp(centerTap.rgb, bgColor, bgBlend);
            half3 finalColor = lerp(finalBg, fgColor, max(max(0.0 , -centerTap.a), fgBlend));

            if (mergePass)
                finalColor = min(finalColor, tex2Dlod(_ThirdTex, half4(uv, 0.0, 0.0)).rgb);

            finalColor = lerp(centerTap.rgb, finalColor, saturate(bgBlend + fgBlend));
            fgBlend = max(fgBlendFromPreviousPass, fgBlend);
            return half4(finalColor, (sampleDilatedFG || mergePass) ? fgBlend : centerTap.a);
        }

        half4 fragShapeLowQuality(v2fDepth i) : SV_Target
        {
            return shapeDirectionalBlur(i.uv, false, SAMPLE_NUM_L, false);
        }

        half4 fragShapeLowQualityDilateFg(v2fDepth i) : SV_Target
        {
            return shapeDirectionalBlur(i.uv, false, SAMPLE_NUM_L, true);
        }

        half4 fragShapeLowQualityMerge(v2fDepth i) : SV_Target
        {
            return shapeDirectionalBlur(i.uv, true, SAMPLE_NUM_L, false);
        }

        half4 fragShapeLowQualityMergeDilateFg(v2fDepth i) : SV_Target
        {
            return shapeDirectionalBlur(i.uv, true, SAMPLE_NUM_L, true);
        }

        half4 fragShapeMediumQuality(v2fDepth i) : SV_Target
        {
            return shapeDirectionalBlur(i.uv, false, SAMPLE_NUM_M, false);
        }

        half4 fragShapeMediumQualityDilateFg(v2fDepth i) : SV_Target
        {
            return shapeDirectionalBlur(i.uv, false, SAMPLE_NUM_M, true);
        }

        half4 fragShapeMediumQualityMerge(v2fDepth i) : SV_Target
        {
            return shapeDirectionalBlur(i.uv, true, SAMPLE_NUM_M, false);
        }

        half4 fragShapeMediumQualityMergeDilateFg(v2fDepth i) : SV_Target
        {
            return shapeDirectionalBlur(i.uv, true, SAMPLE_NUM_M, true);
        }

        half4 fragShapeHighQuality(v2fDepth i) : SV_Target
        {
            return shapeDirectionalBlur(i.uv, false, SAMPLE_NUM_H, false);
        }

        half4 fragShapeHighQualityDilateFg(v2fDepth i) : SV_Target
        {
            return shapeDirectionalBlur(i.uv, false, SAMPLE_NUM_H, true);
        }

        half4 fragShapeHighQualityMerge(v2fDepth i) : SV_Target
        {
            return shapeDirectionalBlur(i.uv, true, SAMPLE_NUM_H, false);
        }

        half4 fragShapeHighQualityMergeDilateFg(v2fDepth i) : SV_Target
        {
            return shapeDirectionalBlur(i.uv, true, SAMPLE_NUM_H, true);
        }

        ///////////////////////////////////////////////////////////////////////////////
        // Disk Bokeh
        ///////////////////////////////////////////////////////////////////////////////

        static const half3 DiscBokeh48[48] =
        {
            // 48 tap regularly spaced circular kernel (x,y, length)
            // Fill free to change the shape to try other bokehs style :)
            half3( 0.99144h, 0.13053h, 1.0h),
            half3( 0.92388h, 0.38268h, 1.0h),
            half3( 0.79335h, 0.60876h, 1.0h),
            half3( 0.60876h, 0.79335h, 1.0h),
            half3( 0.38268h, 0.92388h, 1.0h),
            half3( 0.13053h, 0.99144h, 1.0h),
            half3(-0.13053h, 0.99144h, 1.0h),
            half3(-0.38268h, 0.92388h, 1.0h),
            half3(-0.60876h, 0.79335h, 1.0h),
            half3(-0.79335h, 0.60876h, 1.0h),
            half3(-0.92388h, 0.38268h, 1.0h),
            half3(-0.99144h, 0.13053h, 1.0h),
            half3(-0.99144h,-0.13053h, 1.0h),
            half3(-0.92388h,-0.38268h, 1.0h),
            half3(-0.79335h,-0.60876h, 1.0h),
            half3(-0.60876h,-0.79335h, 1.0h),
            half3(-0.38268h,-0.92388h, 1.0h),
            half3(-0.13053h,-0.99144h, 1.0h),
            half3( 0.13053h,-0.99144h, 1.0h),
            half3( 0.38268h,-0.92388h, 1.0h),
            half3( 0.60876h,-0.79335h, 1.0h),
            half3( 0.79335h,-0.60876h, 1.0h),
            half3( 0.92388h,-0.38268h, 1.0h),
            half3( 0.99144h,-0.13053h, 1.0h),
            half3( 0.64732h, 0.12876h, 0.66h),
            half3( 0.54877h, 0.36668h, 0.66h),
            half3( 0.36668h, 0.54877h, 0.66h),
            half3( 0.12876h, 0.64732h, 0.66h),
            half3(-0.12876h, 0.64732h, 0.66h),
            half3(-0.36668h, 0.54877h, 0.66h),
            half3(-0.54877h, 0.36668h, 0.66h),
            half3(-0.64732h, 0.12876h, 0.66h),
            half3(-0.64732h,-0.12876h, 0.66h),
            half3(-0.54877h,-0.36668h, 0.66h),
            half3(-0.36668h,-0.54877h, 0.66h),
            half3(-0.12876h,-0.64732h, 0.66h),
            half3( 0.12876h,-0.64732h, 0.66h),
            half3( 0.36668h,-0.54877h, 0.66h),
            half3( 0.54877h,-0.36668h, 0.66h),
            half3( 0.64732h,-0.12876h, 0.66h),
            half3( 0.30488h, 0.12629h, 0.33h),
            half3( 0.12629h, 0.30488h, 0.33h),
            half3(-0.12629h, 0.30488h, 0.33h),
            half3(-0.30488h, 0.12629h, 0.33h),
            half3(-0.30488h,-0.12629h, 0.33h),
            half3(-0.12629h,-0.30488h, 0.33h),
            half3( 0.12629h,-0.30488h, 0.33h),
            half3( 0.30488h,-0.12629h, 0.33h)
        };

        inline half4 circleCocBokeh(half2 uv, bool sampleDilatedFG, int increment)
        {
            half4 centerTap = FetchMainTex(uv);
            half  fgCoc = centerTap.a;

            if (sampleDilatedFG)
                fgCoc = min(tex2Dlod(_SecondTex, half4(uv, 0.0, 0.0)).r, fgCoc);

            half bgRadius = 0.5 * smoothstep(0.0, 0.85, centerTap.a)  * _BlurCoe.y;
            half fgRadius = 0.5 * smoothstep(0.0, 0.85, -fgCoc) * _BlurCoe.x;
            half radius = max(bgRadius, fgRadius);

            if (radius < 1e-2)
                return half4(centerTap.rgb, 0.0);

            half2 poissonScale = radius * _MainTex_TexelSize.xy;
            half fgWeight = max(0.0,-centerTap.a);
            half bgWeight = max(0.0, centerTap.a);
            half3 fgSum = centerTap.rgb * fgWeight;
            half3 bgSum = centerTap.rgb * bgWeight;

            half radOtherFgRad = radius / (fgRadius + 1e-2);
            half radOtherBgRad = radius / (bgRadius + 1e-2);

            for (int l = 0; l < 48; l += increment)
            {
                half2 sampleUV = uv + DiscBokeh48[l].xy * poissonScale.xy;
                half4 sample0  = FetchColorAndCocFromMainTex(sampleUV, DiscBokeh48[l].xy);

                half isNear = max(0.0, -sample0.a);
                half distanceFactor = saturate(-0.5 * abs(sample0.a - centerTap.a) * DiscBokeh48[l].z + 1.0);
                half isFar = max(0.0, sample0.a)  * distanceFactor;
                isNear *= 1.0 - smoothstep(1.0, 2.0, DiscBokeh48[l].z * radOtherFgRad);
                isFar  *= 1.0 - smoothstep(1.0, 2.0, DiscBokeh48[l].z * radOtherBgRad);

                fgWeight += isNear;
                fgSum += sample0.rgb * isNear;
                bgWeight += isFar;
                bgSum += sample0.rgb * isFar;
            }

            half3 fgColor = fgSum / (fgWeight + 1e-4);
            half3 bgColor = bgSum / (bgWeight + 1e-4);
            half bgBlend = saturate(2.0 * bgWeight / 49.0);
            half fgBlend = saturate(2.0 * fgWeight / 49.0);

            half3 finalBg = lerp(centerTap.rgb, bgColor, bgBlend);
            half3 finalColor = lerp(finalBg, fgColor, max(max(0.0 , -centerTap.a), fgBlend));
            half4 returnValue = half4(finalColor, fgBlend);

            return returnValue;
        }

        half4 fragCircleBlurWithDilatedFg(v2fDepth i) : SV_Target
        {
            return circleCocBokeh(i.uv, true, 1);
        }

        half4 fragCircleBlur(v2fDepth i) : SV_Target
        {
            return circleCocBokeh(i.uv, false, 1);
        }

        half4 fragCircleBlurWithDilatedFgLowQuality(v2fDepth i) : SV_Target
        {
            return circleCocBokeh(i.uv, true, 2);
        }

        half4 fragCircleBlurLowQuality(v2fDepth i) : SV_Target
        {
            return circleCocBokeh(i.uv, false, 2);
        }

        ///////////////////////////////////////////////////////////////////////////////
        // Prefilter blur
        ///////////////////////////////////////////////////////////////////////////////

        #define DISC_PREFILTER_SAMPLE   9
        static const half2 DiscPrefilter[DISC_PREFILTER_SAMPLE] =
        {
            half2(0.01288369,  0.5416069),
            half2(-0.9192798, -0.09529364),
            half2( 0.7596578,  0.1922738),
            half2(  -0.14132, -0.2880242),
            half2(-0.5249333,  0.7777638),
            half2(-0.5871695, -0.7403569),
            half2( 0.3202196, -0.6442268),
            half2( 0.8553214, -0.3920982),
            half2( 0.5827708,  0.7599297)
        };

        half4 fragCocPrefilter(v2fDepth i) : SV_Target
        {
            half4 centerTap = FetchMainTex(i.uv);
            half  radius = 0.33 * 0.5 * (centerTap.a < 0.0
                ? -(centerTap.a * _BlurCoe.x)
                :  (centerTap.a * _BlurCoe.y));
            half2 poissonScale = radius * _MainTex_TexelSize.xy;

            if (radius < 1e-2)
                return centerTap;

            half  sampleCount = 1.0;
            half3 sum = centerTap.rgb;

            for (int l = 0; l < DISC_PREFILTER_SAMPLE; l++)
            {
                half2 sampleUV = i.uv + DiscPrefilter[l].xy * poissonScale.xy;
                half4 sample0 = FetchColorAndCocFromMainTex(sampleUV, DiscPrefilter[l].xy);
                half weight = max(sample0.a * centerTap.a, 0.0);
                sum += sample0.rgb * weight;
                sampleCount += weight;
            }

            half4 returnValue = half4(sum / sampleCount, centerTap.a);
            return returnValue;
        }

        ///////////////////////////////////////////////////////////////////////////////
        // Final merge and upsample
        ///////////////////////////////////////////////////////////////////////////////

        inline half4 upSampleConvolved(half2 uv)
        {
            half2 convolvedTexelPos    = uv * _Convolved_TexelSize.xy;
            half2 convolvedTexelCenter = floor( convolvedTexelPos ) + 0.5;
            half2 convolvedTexelOffsetFromCenter = convolvedTexelPos - convolvedTexelCenter;
            half2 offsetFromCoc = half2(0.0, 0.0);

        #if defined(USE_TEX2DOBJECT_FOR_COC)
            half2 cocUV = (convolvedTexelOffsetFromCenter * _Convolved_TexelSize.zw) + uv;
            half4 coc = _CameraDepthTexture.GatherRed(sampler_CameraDepthTexture, cocUV);
            coc.r = GetCocFromZValue(coc.r);
            coc.g = GetCocFromZValue(coc.g);
            coc.b = GetCocFromZValue(coc.b);
            coc.a = GetCocFromZValue(coc.a);

            half4 absCoc = abs(coc);
            offsetFromCoc = GetBilinearFetchTexOffsetFromAbsCoc(absCoc) * 0.5;
            uv += offsetFromCoc * _Convolved_TexelSize.zw;
        #endif

            /*
            // Bicubic upsampling (B-spline) - Uncomment me for higher quality upsampling
            // Adding offsetFromCoc "antialias" haloing from bright in focus region on dark out of focus region.
            // However its a hack as we should consider all the COC of the bicubic region and kill the bicubic
            // interpolation to avoid in any leaking but that would be too expensive, so when this is a problem
            // one should rather disable bicubic interpolation.
            half2 f  = convolvedTexelOffsetFromCenter + offsetFromCoc;
            half2 f2 = f * f;
            half2 f3 = f * f2;

            half2 w0 = -0.166 * f3 + 0.5 * f2 - 0.5 * f + 0.166;
            half2 w1 =  0.5   * f3 - f2 + 0.666;
            half2 w3 =  0.166 * f3;
            half2 w2 =  1.0 - w0 - w1 - w3;

            half2 s0 = w0 + w1;
            half2 s1 = w2 + w3;
            half2 f0 = w1 / s0;
            half2 f1 = w3 / s1;

            half2 t0 = _Convolved_TexelSize.zw * (convolvedTexelCenter - 1.0 + f0);
            half2 t1 = _Convolved_TexelSize.zw * (convolvedTexelCenter + 1.0 + f1);

            return tex2Dlod(_SecondTex, half4(t0.x, t0.y, 0.0, 0.0)) * s0.x * s0.y +
                    tex2Dlod(_SecondTex, half4(t1.x, t0.y, 0.0, 0.0)) * s1.x * s0.y +
                    tex2Dlod(_SecondTex, half4(t0.x, t1.y, 0.0, 0.0)) * s0.x * s1.y +
                    tex2Dlod(_SecondTex, half4(t1.x, t1.y, 0.0, 0.0)) * s1.x * s1.y;
            */

            return tex2Dlod(_SecondTex, half4(uv, 0.0, 0.0));
        }

        inline half4 dofMerge(half2 uv)
        {
            half4 convolvedTap = upSampleConvolved(uv);
            convolvedTap.rgb = TonemapInvert(convolvedTap.rgb);

            half4 sourceTap = FetchMainTex(uv);
            half  coc       = GetCocFromDepth(uv);

            coc = (coc * _BlurCoe.y > 1.0) ? coc : 0.0;
            half blendValue = smoothstep(0.0, 0.33, max(coc, convolvedTap.a));
            half3 returnValue = lerp(sourceTap.rgb, convolvedTap.rgb, blendValue);
            return (blendValue < 1e-2) ? sourceTap : half4(returnValue.rgb, sourceTap.a);
        }

        half4 fragMergeExplicit(v2fDepth i) : SV_Target
        {
            return dofMerge(i.uv);
        }

        ///////////////////////////////////////////////////////////////////////////////
        // Downsampling and COC computation
        ///////////////////////////////////////////////////////////////////////////////

        inline half4 captureCoc(half2 uvColor, half2 uvDepth)
        {
            /*****************/
            /* coc.a | coc.b */
            /* coc.r | coc.g */
            /*****************/
        #if defined(USE_TEX2DOBJECT_FOR_COC)
            half4 coc = _CameraDepthTexture.GatherRed(sampler_CameraDepthTexture, uvDepth);
            coc.r = GetCocFromZValue(coc.r);
            coc.g = GetCocFromZValue(coc.g);
            coc.b = GetCocFromZValue(coc.b);
            coc.a = GetCocFromZValue(coc.a);
        #else
            half4 coc;
            coc.r = GetCocFromDepth(uvDepth + _MainTex_TexelSize.xy * half2(-0.25, 0.25));
            coc.g = GetCocFromDepth(uvDepth + _MainTex_TexelSize.xy * half2( 0.25, 0.25));
            coc.b = GetCocFromDepth(uvDepth + _MainTex_TexelSize.xy * half2( 0.25,-0.25));
            coc.a = GetCocFromDepth(uvDepth + _MainTex_TexelSize.xy * half2(-0.25,-0.25));
        #endif

            half4 absCoc = abs(coc);
            half2 offset = GetBilinearFetchTexOffsetFromAbsCoc(absCoc) * _MainTex_TexelSize.xy;
            half4 color = FetchMainTex(uvColor + offset);

            half cocRG = (absCoc.r<absCoc.g)?coc.r:coc.g;
            half cocBA = (absCoc.b<absCoc.a)?coc.b:coc.a;
            color.a    = (abs(cocRG)<abs(cocBA))?cocRG:cocBA;

            color.rgb = Tonemap(color);

            return color;
        }

        half4 fragCaptureCocExplicit(v2f i) : SV_Target
        {
            return captureCoc(i.uv, i.uv1);
        }

        ///////////////////////////////////////////////////////////////////////////////
        // Coc visualisation
        ///////////////////////////////////////////////////////////////////////////////

        inline half4 visualizeCoc(half2 uv)
        {
            half coc = GetCocFromDepth(uv);
            return (coc < 0.0)
                ? half4(-coc, -coc, 0.0, 1.0)
                : half4(0.0, coc, coc, 1.0);
        }

        half4 fragVisualizeCocExplicit(v2fDepth i) : SV_Target
        {
            return visualizeCoc(i.uv);
        }

        ///////////////////////////////////////////////////////////////////////////////
        // Foreground blur dilatation
        ///////////////////////////////////////////////////////////////////////////////

        inline half2 fgCocSourceChannel(half2 uv, bool fromAlpha)
        {
            if (fromAlpha)
                return FetchMainTex(uv).aa;
            else
                return FetchMainTex(uv).rg;
        }

        inline half2 weigthedFGCocBlur(v2fBlur i, bool fromAlpha)
        {
            half2 fgCoc00 = fgCocSourceChannel(i.uv.xy  , fromAlpha);
            half2 fgCoc01 = fgCocSourceChannel(i.uv01.zw, fromAlpha) * 1.0;
            half2 fgCoc02 = fgCocSourceChannel(i.uv01.xy, fromAlpha) * 1.0;
            half2 fgCoc03 = fgCocSourceChannel(i.uv23.xy, fromAlpha) * 0.8;
            half2 fgCoc04 = fgCocSourceChannel(i.uv23.zw, fromAlpha) * 0.8;
            half2 fgCoc05 = fgCocSourceChannel(i.uv45.xy, fromAlpha) * 0.6;
            half2 fgCoc06 = fgCocSourceChannel(i.uv45.zw, fromAlpha) * 0.6;
            half2 fgCoc07 = fgCocSourceChannel(i.uv67.xy, fromAlpha) * 0.4;
            half2 fgCoc08 = fgCocSourceChannel(i.uv67.zw, fromAlpha) * 0.4;
            half2 fgCoc09 = fgCocSourceChannel(i.uv89.xy, fromAlpha) * 0.25;
            half2 fgCoc10 = fgCocSourceChannel(i.uv89.zw, fromAlpha) * 0.25;

            half fgCoc;
            fgCoc = min(  0.0, fgCoc00.r);
            fgCoc = min(fgCoc, fgCoc01.r);
            fgCoc = min(fgCoc, fgCoc02.r);
            fgCoc = min(fgCoc, fgCoc03.r);
            fgCoc = min(fgCoc, fgCoc04.r);
            fgCoc = min(fgCoc, fgCoc05.r);
            fgCoc = min(fgCoc, fgCoc06.r);
            fgCoc = min(fgCoc, fgCoc07.r);
            fgCoc = min(fgCoc, fgCoc08.r);
            fgCoc = min(fgCoc, fgCoc09.r);
            fgCoc = min(fgCoc, fgCoc10.r);

            return half2(fgCoc,fgCoc00.g);
        }

        half4 fragDilateFgCocFromColor(v2fBlur i) : SV_Target
        {
            return weigthedFGCocBlur(i, true).xyxy;
        }

        half4 fragDilateFgCoc(v2fBlur i) : SV_Target
        {
            return weigthedFGCocBlur(i, false).xyxy;
        }

        ///////////////////////////////////////////////////////////////////////////////
        // Texture Bokeh related
        ///////////////////////////////////////////////////////////////////////////////

        half4 fragBlurAlphaWeighted(v2fBlur i) : SV_Target
        {
            const half ALPHA_WEIGHT = 2.0;
            half4 sum = half4(0.0, 0.0, 0.0, 0.0);
            half w = 0.0;
            half weights = 0.0;
            const half G_WEIGHTS[6] = { 1.0, 0.8, 0.675, 0.5, 0.2, 0.075 };

            half4 sampleA = FetchMainTex(i.uv.xy);
            half4 sampleB = FetchMainTex(i.uv01.xy);
            half4 sampleC = FetchMainTex(i.uv01.zw);
            half4 sampleD = FetchMainTex(i.uv23.xy);
            half4 sampleE = FetchMainTex(i.uv23.zw);
            half4 sampleF = FetchMainTex(i.uv45.xy);
            half4 sampleG = FetchMainTex(i.uv45.zw);
            half4 sampleH = FetchMainTex(i.uv67.xy);
            half4 sampleI = FetchMainTex(i.uv67.zw);
            half4 sampleJ = FetchMainTex(i.uv89.xy);
            half4 sampleK = FetchMainTex(i.uv89.zw);

            w = sampleA.a * G_WEIGHTS[0]; sum += sampleA * w; weights += w;
            w = saturate(ALPHA_WEIGHT * sampleB.a) * G_WEIGHTS[1]; sum += sampleB * w; weights += w;
            w = saturate(ALPHA_WEIGHT * sampleC.a) * G_WEIGHTS[1]; sum += sampleC * w; weights += w;
            w = saturate(ALPHA_WEIGHT * sampleD.a) * G_WEIGHTS[2]; sum += sampleD * w; weights += w;
            w = saturate(ALPHA_WEIGHT * sampleE.a) * G_WEIGHTS[2]; sum += sampleE * w; weights += w;
            w = saturate(ALPHA_WEIGHT * sampleF.a) * G_WEIGHTS[3]; sum += sampleF * w; weights += w;
            w = saturate(ALPHA_WEIGHT * sampleG.a) * G_WEIGHTS[3]; sum += sampleG * w; weights += w;
            w = saturate(ALPHA_WEIGHT * sampleH.a) * G_WEIGHTS[4]; sum += sampleH * w; weights += w;
            w = saturate(ALPHA_WEIGHT * sampleI.a) * G_WEIGHTS[4]; sum += sampleI * w; weights += w;
            w = saturate(ALPHA_WEIGHT * sampleJ.a) * G_WEIGHTS[5]; sum += sampleJ * w; weights += w;
            w = saturate(ALPHA_WEIGHT * sampleK.a) * G_WEIGHTS[5]; sum += sampleK * w; weights += w;

            sum /= weights + 1e-4;

            sum.a = sampleA.a;
            if(sampleA.a < 1e-2)
                sum.rgb = sampleA.rgb;

            return sum;
        }

        half4 fragBoxBlur(v2f i) : SV_Target
        {
            half4 returnValue = FetchMainTex(i.uv1.xy + 0.75 * _MainTex_TexelSize.xy);
            returnValue += FetchMainTex(i.uv1.xy - 0.75 * _MainTex_TexelSize.xy);
            returnValue += FetchMainTex(i.uv1.xy + 0.75 * _MainTex_TexelSize.xy * half2(1.0, -1.0));
            returnValue += FetchMainTex(i.uv1.xy - 0.75 * _MainTex_TexelSize.xy * half2(1.0, -1.0));
            return returnValue / 4.0;
        }

    ENDCG

    ///////////////////////////////////////////////////////////////////////////////

    SubShader
    {
        Tags { "Name" = "MainSubShader_SM5" }
        // If adding or removing a pass please also update the fallback subshader below

        ZTest Always Cull Off ZWrite Off Fog { Mode Off } Lighting Off Blend Off

        Pass
        {
            CGPROGRAM
                #pragma vertex vertBlurPlusMinus
                #pragma fragment fragBlurAlphaWeighted
            ENDCG
        }

        Pass
        {
            CGPROGRAM
                #pragma vertex vert_d
                #pragma fragment fragBoxBlur
            ENDCG
        }

        Pass
        {
            CGPROGRAM
                #pragma vertex vertBlurPlusMinus
                #pragma fragment fragDilateFgCocFromColor
            ENDCG
        }

        Pass
        {
            CGPROGRAM
                #pragma vertex vertBlurPlusMinus
                #pragma fragment fragDilateFgCoc
            ENDCG
        }

        Pass
        {
            CGPROGRAM
                #pragma vertex vert_d
                #pragma fragment fragCaptureCocExplicit
                #pragma target 5.0
            ENDCG
        }

        Pass
        {
            CGPROGRAM
                #pragma vertex vert
                #pragma fragment fragVisualizeCocExplicit
            ENDCG
        }

        Pass
        {
            CGPROGRAM
                #pragma vertex vert
                #pragma fragment fragCocPrefilter
                #pragma target 5.0
            ENDCG
        }

        Pass
        {
            CGPROGRAM
                #pragma vertex vert
                #pragma fragment fragCircleBlur
                #pragma target 5.0
            ENDCG
        }

        Pass
        {
            CGPROGRAM
                #pragma vertex vert
                #pragma fragment fragCircleBlurWithDilatedFg
                #pragma target 5.0
            ENDCG
        }

        Pass
        {
            CGPROGRAM
                #pragma vertex vert
                #pragma fragment fragCircleBlurLowQuality
                #pragma target 5.0
            ENDCG
        }

        Pass
        {
            CGPROGRAM
                #pragma vertex vert
                #pragma fragment fragCircleBlurWithDilatedFgLowQuality
                #pragma target 5.0
            ENDCG
        }

        Pass
        {
            CGPROGRAM
                #pragma vertex vertNoFlip
                #pragma fragment fragMergeExplicit
                #pragma target 5.0
            ENDCG
        }

        Pass
        {
            CGPROGRAM
                #pragma vertex vert
                #pragma fragment fragShapeLowQuality
            ENDCG
        }

        Pass
        {
            CGPROGRAM
                #pragma vertex vert
                #pragma fragment fragShapeLowQualityDilateFg
            ENDCG
        }

        Pass
        {
            CGPROGRAM
                #pragma vertex vert
                #pragma fragment fragShapeLowQualityMerge
            ENDCG
        }

        Pass
        {
            CGPROGRAM
                #pragma vertex vert
                #pragma fragment fragShapeLowQualityMergeDilateFg
            ENDCG
        }

        Pass
        {
            CGPROGRAM
                #pragma vertex vert
                #pragma fragment fragShapeMediumQuality
            ENDCG
        }

        Pass
        {
            CGPROGRAM
                #pragma vertex vert
                #pragma fragment fragShapeMediumQualityDilateFg
            ENDCG
        }

        Pass
        {
            CGPROGRAM
                #pragma vertex vert
                #pragma fragment fragShapeMediumQualityMerge
            ENDCG
        }

        Pass
        {
            CGPROGRAM
                #pragma vertex vert
                #pragma fragment fragShapeMediumQualityMergeDilateFg
            ENDCG
        }

        Pass
        {
            CGPROGRAM
                #pragma vertex vert
                #pragma fragment fragShapeHighQuality
            ENDCG
        }

        Pass
        {
            CGPROGRAM
                #pragma vertex vert
                #pragma fragment fragShapeHighQualityDilateFg
            ENDCG
        }

        Pass
        {
            CGPROGRAM
                #pragma vertex vert
                #pragma fragment fragShapeHighQualityMerge
            ENDCG
        }

        Pass
        {
            CGPROGRAM
                #pragma vertex vert
                #pragma fragment fragShapeHighQualityMergeDilateFg
            ENDCG
        }
    }

    SubShader
    {
        Tags { "Name" = "FallbackSubShader_SM3" }
        // If adding or removing a pass please also update the main subshader above

        ZTest Always Cull Off ZWrite Off Fog { Mode Off } Lighting Off Blend Off

        Pass
        {
            CGPROGRAM
                #pragma vertex vertBlurPlusMinus
                #pragma fragment fragBlurAlphaWeighted
            ENDCG
        }

        Pass
        {
            CGPROGRAM
                #pragma vertex vert_d
                #pragma fragment fragBoxBlur
            ENDCG
        }

        Pass
        {
            CGPROGRAM
                #pragma vertex vertBlurPlusMinus
                #pragma fragment fragDilateFgCocFromColor
            ENDCG
        }

        Pass
        {
            CGPROGRAM
                #pragma vertex vertBlurPlusMinus
                #pragma fragment fragDilateFgCoc
            ENDCG
        }

        Pass
        {
            CGPROGRAM
                #pragma vertex vert_d
                #pragma fragment fragCaptureCocExplicit
            ENDCG
        }

        Pass
        {
            CGPROGRAM
                #pragma vertex vert
                #pragma fragment fragVisualizeCocExplicit
            ENDCG
        }

        Pass
        {
            CGPROGRAM
                #pragma vertex vert
                #pragma fragment fragCocPrefilter
            ENDCG
        }

        Pass
        {
            CGPROGRAM
                #pragma vertex vert
                #pragma fragment fragCircleBlur
            ENDCG
        }

        Pass
        {
            CGPROGRAM
                #pragma vertex vert
                #pragma fragment fragCircleBlurWithDilatedFg
            ENDCG
        }

        Pass
        {
            CGPROGRAM
                #pragma vertex vert
                #pragma fragment fragCircleBlurLowQuality
            ENDCG
        }

        Pass
        {
            CGPROGRAM
                #pragma vertex vert
                #pragma fragment fragCircleBlurWithDilatedFgLowQuality
            ENDCG
        }

        Pass
        {
            CGPROGRAM
                #pragma vertex vertNoFlip
                #pragma fragment fragMergeExplicit
            ENDCG
        }

        Pass
        {
            CGPROGRAM
                #pragma vertex vert
                #pragma fragment fragShapeLowQuality
            ENDCG
        }

        Pass
        {
            CGPROGRAM
                #pragma vertex vert
                #pragma fragment fragShapeLowQualityDilateFg
            ENDCG
        }

        Pass
        {
            CGPROGRAM
                #pragma vertex vert
                #pragma fragment fragShapeLowQualityMerge
            ENDCG
        }

        Pass
        {
            CGPROGRAM
                #pragma vertex vert
                #pragma fragment fragShapeLowQualityMergeDilateFg
            ENDCG
        }

        Pass
        {
            CGPROGRAM
                #pragma vertex vert
                #pragma fragment fragShapeMediumQuality
            ENDCG
        }

        Pass
        {
            CGPROGRAM
                #pragma vertex vert
                #pragma fragment fragShapeMediumQualityDilateFg
            ENDCG
        }

        Pass
        {
            CGPROGRAM
                #pragma vertex vert
                #pragma fragment fragShapeMediumQualityMerge
            ENDCG
        }

        Pass
        {
            CGPROGRAM
                #pragma vertex vert
                #pragma fragment fragShapeMediumQualityMergeDilateFg
            ENDCG
        }

        Pass
        {
            CGPROGRAM
                #pragma vertex vert
                #pragma fragment fragShapeHighQuality
            ENDCG
        }

        Pass
        {
            CGPROGRAM
                #pragma vertex vert
                #pragma fragment fragShapeHighQualityDilateFg
            ENDCG
        }

        Pass
        {
            CGPROGRAM
                #pragma vertex vert
                #pragma fragment fragShapeHighQualityMerge
            ENDCG
        }

        Pass
        {
            CGPROGRAM
                #pragma vertex vert
                #pragma fragment fragShapeHighQualityMergeDilateFg
            ENDCG
        }
    }

    FallBack Off
}
