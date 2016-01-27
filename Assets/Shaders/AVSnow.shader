Shader "Custom/AVSnow" {
        Properties{
        _Color("Color", Color) = (1,1,1,1)
        _MainTex("Albedo (RGB)", 2D) = "grey" {}
        _BumpMap("Normalmap (RGB) Occlusion (A)", 2D) = "bump" {}
        _Snow("Snow Level", Range(0,1)) = 0
        _SnowColor("Snow Color", Color) = (1.0,1.0,1.0,1.0)
        _SnowDirection("Snow Direction", Vector) = (0,1,0)
        _SnowSmoothness("Snow Smoothness", Range(0,1)) = 0

    }
        SubShader{
        Tags{ "RenderType" = "Opaque" }
        LOD 200

        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
#pragma surface surf Standard

        // Use shader model 3.0 target, to get nicer looking lighting
#pragma target 3.0

        sampler2D _MainTex;
    sampler2D _Shapetex;
    sampler2D _BumpMap;
    float _Snow;
    float4 _SnowColor;
    float4 _SnowDirection;
    float _SnowSmoothness;


    struct Input {
        float2 uv_MainTex;
        float2 uv2_BumpMap;
        float4 color: Color; // Vertex color
        float3 worldNormal;
        INTERNAL_DATA
    };

    half _Glossiness;
    fixed4 _Color;

    void surf(Input IN, inout SurfaceOutputStandard o) {
        // Albedo comes from a texture tinted by color
        fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
        fixed4 b = tex2D(_BumpMap, IN.uv2_BumpMap);
        o.Normal = UnpackNormal(b.ggga);
        if (dot(WorldNormalVector(IN, o.Normal), _SnowDirection.xyz) >= lerp(1, -1, _Snow))
        {
            o.Albedo = _SnowColor.rgb;
            o.Smoothness = _SnowSmoothness;
        }
        else 
        {
            o.Albedo = c.rgb < 0.5 ? (2.0 * c.rgb * IN.color.rgb) : (1.0 - 2.0 * (1.0 - c.rgb) * (1.0 - IN.color.rgb));
            o.Smoothness = c.a;
        }
        o.Metallic = 1.0 - IN.color.a;
        o.Occlusion = b.r;
        o.Alpha = b.b;
    }
    ENDCG
    }
        FallBack "Diffuse"
    }
