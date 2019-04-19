Shader "Custom/Grass Splat"
{
    Properties
    {
        _BaseControl("Base Layer Control (RGBA)", 2D) = "red" {}
        _Control("Control (RGBA)", 2D) = "red" {}
        _Splat4("Layer 4 (A)", 2D) = "red" {}
        _Splat3("Layer 3 (B)", 2D) = "red" {}
        _Splat2("Layer 2 (G)", 2D) = "red" {}
        _Splat1("Layer 1 (R)", 2D) = "red" {}
        _Splat0("Layer 0 (Base)", 2D) = "red" {}
        _Normal4("Normal 4 (A)", 2D) = "bump" {}
        _Normal3("Normal 3 (B)", 2D) = "bump" {}
        _Normal2("Normal 2 (G)", 2D) = "bump" {}
        _Normal1("Normal 1 (R)", 2D) = "bump" {}
        _Normal0("Normal 0 (Base)", 2D) = "bump" {}
        _WorldBounds("World Bounds", Vector) = (0,0,1,1)
        // used in fallback on old cards & base map
        _MainTex("BaseMap (RGB)", 2D) = "white" {}
        _Color("Main Color", Color) = (1,1,1,1)
        _Cutoff("Alpha cutoff", Range(0,1)) = 0.5
    }
    SubShader
    {
        Tags{ "Queue" = "AlphaTest-1" "IgnoreProjector" = "True" "RenderType" = "TransparentCutout" }
        LOD 200
        Offset -1, -1

        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf Standard alphatest:_Cutoff

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0

        sampler2D _BaseControl;
        sampler2D _Control;

        sampler2D _Splat0, _Splat1, _Splat2, _Splat3, _Splat4;
        sampler2D _Normal0, _Normal1, _Normal2, _Normal3, _Normal4;

        sampler2D _MainTex;

        struct Input {
            float2 uv_MainTex;
            float3 worldPos;
        };
        fixed4 _Color;
        float4 _WorldBounds;

        fixed4 MixColor(fixed4 bottom, inout fixed4 bottom_n, fixed4 top, fixed4 top_n, fixed top_alpha)
        {
            top = fixed4(top.rgb, max(top.a + top_alpha - 1, 0));
            //crappy blending to test
            bottom_n = (top.a) > (bottom.a) ? top_n : bottom_n;
            return (top.a) > (bottom.a) ? top: bottom;
        }



        void surf(Input IN, inout SurfaceOutputStandard o) {
            // Albedo comes from a texture tinted by color
            fixed4 baseControl = tex2D(_BaseControl, (IN.worldPos.xz - _WorldBounds.xy) / (_WorldBounds.zw - _WorldBounds.xy));
            fixed4 control = tex2D(_Control, (IN.worldPos.xz - _WorldBounds.xy) / (_WorldBounds.zw - _WorldBounds.xy));
            fixed4 c = tex2D(_Splat0, IN.uv_MainTex);
            fixed4 n = tex2D(_Normal0, IN.uv_MainTex);
            c = fixed4(c.rgb * baseControl.rgb, c.a + baseControl.a - 1);
            c = MixColor(c, n, tex2D(_Splat1, IN.uv_MainTex), tex2D(_Normal1, IN.uv_MainTex), control.r);
            c = MixColor(c, n, tex2D(_Splat2, IN.uv_MainTex), tex2D(_Normal2, IN.uv_MainTex), control.g);
            c = MixColor(c, n, tex2D(_Splat3, IN.uv_MainTex), tex2D(_Normal3, IN.uv_MainTex), control.b);
            c = MixColor(c, n, tex2D(_Splat4, IN.uv_MainTex), tex2D(_Normal4, IN.uv_MainTex), control.a);
            o.Albedo = c.rgb;
            // Metallic and smoothness come from slider variables
            //o.Metallic = _Metallic;
            //o.Smoothness = _Glossiness;
            o.Alpha = c.a > 0 ? 1 : 0;
            o.Normal = UnpackNormal(n);
            o.Occlusion = n.r;
        }
        ENDCG
    }
        FallBack "Diffuse"
}
