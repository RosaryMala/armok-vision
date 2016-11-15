Shader "Custom/Grass Shader" {
    Properties{
        _Color("Color", Color) = (1,1,1,1)
        _MainTex("Albedo (RGB)", 2D) = "white" {}
    _BumpMap("Normalmap (RGB) Occlusion (A)", 2D) = "bump" {}
    _Depth("Depth map", 2D) = "black" {}
        _Alpha("Depth map", 2D) = "black" {}
        _Glossiness("Smoothness", Range(0,1)) = 0.5
        _Metallic("Metallic", Range(0,1)) = 0.0

        _Cutoff("Alpha cutoff", Range(0,1)) = 0.5
    }
        SubShader{
            Tags { "RenderType" = "Opaque" }
            LOD 200

            CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf Standard addshadow alphatest:_Cutoff

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0

        sampler2D _MainTex;
        sampler2D _Depth;
        sampler2D _Alpha;
        sampler2D _BumpMap;

        struct Input {
            float2 uv_MainTex;
        };

        half _Glossiness;
        half _Metallic;
        fixed4 _Color;

        void surf(Input IN, inout SurfaceOutputStandard o) {
            // Albedo comes from a texture tinted by color
            fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
            fixed4 a = tex2D(_Depth, IN.uv_MainTex) + tex2D(_Alpha, IN.uv_MainTex) - 0.5;
            fixed4 bump = tex2D(_BumpMap, IN.uv_MainTex);
            o.Albedo = c.rgb;
            // Metallic and smoothness come from slider variables
            o.Metallic = _Metallic;
            o.Smoothness = _Glossiness;
            o.Alpha = a.r;
            o.Normal = UnpackNormal(bump.ggga);
        }
        ENDCG
    }
        FallBack "Diffuse"
}
