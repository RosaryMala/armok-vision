Shader "Hidden/Image Effects/Cinematic/AmbientOcclusion"
{
    Properties
    {
        _MainTex("", 2D) = ""{}
        _OcclusionTexture("", 2D) = ""{}
    }
    SubShader
    {
        // 0: Occlusion estimation with CameraDepthTexture
        Pass
        {
            ZTest Always Cull Off ZWrite Off
            CGPROGRAM
            #define SOURCE_DEPTH 1
            #include "AmbientOcclusion.cginc"
            #pragma vertex vert_img
            #pragma fragment frag_ao
            #pragma target 3.0
            ENDCG
        }
        // 1: Occlusion estimation with CameraDepthNormalsTexture
        Pass
        {
            ZTest Always Cull Off ZWrite Off
            CGPROGRAM
            #define SOURCE_DEPTHNORMALS 1
            #include "AmbientOcclusion.cginc"
            #pragma vertex vert_img
            #pragma fragment frag_ao
            #pragma target 3.0
            ENDCG
        }
        // 2: Occlusion estimation with G-Buffer
        Pass
        {
            ZTest Always Cull Off ZWrite Off
            CGPROGRAM
            #define SOURCE_GBUFFER 1
            #include "AmbientOcclusion.cginc"
            #pragma vertex vert_img
            #pragma fragment frag_ao
            #pragma target 3.0
            ENDCG
        }
        // 3: Noise reduction (first pass) with CameraDepthNormalsTexture
        Pass
        {
            ZTest Always Cull Off ZWrite Off
            CGPROGRAM
            #define SOURCE_DEPTHNORMALS 1
            #include "AmbientOcclusion.cginc"
            #pragma vertex vert_img
            #pragma fragment frag_blur1
            #pragma target 3.0
            ENDCG
        }
        // 4: Noise reduction (first pass) with G Buffer
        Pass
        {
            ZTest Always Cull Off ZWrite Off
            CGPROGRAM
            #define SOURCE_GBUFFER 1
            #include "AmbientOcclusion.cginc"
            #pragma vertex vert_img
            #pragma fragment frag_blur1
            #pragma target 3.0
            ENDCG
        }
        // 5: Noise reduction (second pass) with CameraDepthNormalsTexture
        Pass
        {
            ZTest Always Cull Off ZWrite Off
            CGPROGRAM
            #define SOURCE_DEPTHNORMALS 1
            #include "AmbientOcclusion.cginc"
            #pragma vertex vert_img
            #pragma fragment frag_blur2
            #pragma target 3.0
            ENDCG
        }
        // 6: Noise reduction (second pass) with G Buffer
        Pass
        {
            ZTest Always Cull Off ZWrite Off
            CGPROGRAM
            #define SOURCE_GBUFFER 1
            #include "AmbientOcclusion.cginc"
            #pragma vertex vert_img
            #pragma fragment frag_blur2
            #pragma target 3.0
            ENDCG
        }
        // 7: Occlusion combiner
        Pass
        {
            ZTest Always Cull Off ZWrite Off
            CGPROGRAM
            #include "AmbientOcclusion.cginc"
            #pragma vertex vert_multitex
            #pragma fragment frag_combine
            #pragma target 3.0
            ENDCG
        }
        // 8: Occlusion combiner for the ambient-only mode
        Pass
        {
            Blend Zero OneMinusSrcColor, Zero OneMinusSrcAlpha
            ZTest Always Cull Off ZWrite Off
            CGPROGRAM
            #include "AmbientOcclusion.cginc"
            #pragma vertex vert_gbuffer
            #pragma fragment frag_gbuffer_combine
            #pragma target 3.0
            ENDCG
        }
        // 9: Debug blit
        Pass
        {
            ZTest Always Cull Off ZWrite Off
            CGPROGRAM
            #include "AmbientOcclusion.cginc"
            #pragma vertex vert_multitex
            #pragma fragment frag_blit_ao
            #pragma target 3.0
            ENDCG
        }
    }
}
