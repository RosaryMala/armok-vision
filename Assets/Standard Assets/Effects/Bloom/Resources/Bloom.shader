Shader "Hidden/Image Effects/Cinematic/Bloom"
{
    Properties
    {
        _MainTex("", 2D) = "" {}
        _BaseTex("", 2D) = "" {}
    }
    SubShader
    {
        // 0: Prefilter
        Pass
        {
            ZTest Always Cull Off ZWrite Off
            CGPROGRAM
            #pragma multi_compile _ UNITY_COLORSPACE_GAMMA
            #include "Bloom.cginc"
            #pragma vertex vert
            #pragma fragment frag_prefilter
            #pragma target 3.0
            ENDCG
        }
        // 1: Prefilter with anti-flicker
        Pass
        {
            ZTest Always Cull Off ZWrite Off
            CGPROGRAM
            #define ANTI_FLICKER 1
            #pragma multi_compile _ UNITY_COLORSPACE_GAMMA
            #include "Bloom.cginc"
            #pragma vertex vert
            #pragma fragment frag_prefilter
            #pragma target 3.0
            ENDCG
        }
        // 2: First level downsampler
        Pass
        {
            ZTest Always Cull Off ZWrite Off
            CGPROGRAM
            #include "Bloom.cginc"
            #pragma vertex vert
            #pragma fragment frag_downsample1
            #pragma target 3.0
            ENDCG
        }
        // 3: First level downsampler with anti-flicker
        Pass
        {
            ZTest Always Cull Off ZWrite Off
            CGPROGRAM
            #define ANTI_FLICKER 1
            #include "Bloom.cginc"
            #pragma vertex vert
            #pragma fragment frag_downsample1
            #pragma target 3.0
            ENDCG
        }
        // 4: Second level downsampler
        Pass
        {
            ZTest Always Cull Off ZWrite Off
            CGPROGRAM
            #include "Bloom.cginc"
            #pragma vertex vert
            #pragma fragment frag_downsample2
            #pragma target 3.0
            ENDCG
        }
        // 5: Upsampler
        Pass
        {
            ZTest Always Cull Off ZWrite Off
            CGPROGRAM
            #include "Bloom.cginc"
            #pragma vertex vert_multitex
            #pragma fragment frag_upsample
            #pragma target 3.0
            ENDCG
        }
        // 6: High quality upsampler
        Pass
        {
            ZTest Always Cull Off ZWrite Off
            CGPROGRAM
            #define HIGH_QUALITY 1
            #include "Bloom.cginc"
            #pragma vertex vert_multitex
            #pragma fragment frag_upsample
            #pragma target 3.0
            ENDCG
        }
        // 7: Combiner
        Pass
        {
            ZTest Always Cull Off ZWrite Off
            CGPROGRAM
            #pragma multi_compile _ UNITY_COLORSPACE_GAMMA
            #include "Bloom.cginc"
            #pragma vertex vert_multitex
            #pragma fragment frag_upsample_final
            #pragma target 3.0
            ENDCG
        }
        // 8: High quality combiner
        Pass
        {
            ZTest Always Cull Off ZWrite Off
            CGPROGRAM
            #define HIGH_QUALITY 1
            #pragma multi_compile _ UNITY_COLORSPACE_GAMMA
            #include "Bloom.cginc"
            #pragma vertex vert_multitex
            #pragma fragment frag_upsample_final
            #pragma target 3.0
            ENDCG
        }
    }
}
