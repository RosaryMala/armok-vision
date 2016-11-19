Shader "Custom/2D Flat" {
    Properties{
        _Color("Color", Color) = (1,1,1,1)
    }
        SubShader{
        Tags{ "RenderType" = "Opaque" }
        LOD 200

        CGPROGRAM
        #pragma surface surf Lambert vertex:vert

        fixed4 _Color;

    struct Input {
        float dummy;
    };

    void vert(inout appdata_full v) {
        v.normal = float3(0, 1, 0);
    }

    void surf(Input IN, inout SurfaceOutput o) {
        o.Albedo = _Color.rgb;
    }
    ENDCG
    }
        FallBack "Diffuse"
}