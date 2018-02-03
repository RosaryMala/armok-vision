Shader "Custom/ArtImage" 
{
    Properties 
    {
        _Color ("Color", Color) = (1,1,1,1)
        _Atlas ("Albedo (RGB)", 2DArray) = "white" {}
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0
        _TileIndex ("TileIndex (R)", 2D) = "gray" {}
        _Tile ("Metallic", Range(0,255)) = 0.0
    }
    SubShader 
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf Standard fullforwardshadows

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0

        UNITY_DECLARE_TEX2DARRAY(_Atlas);
        sampler2D _TileIndex;

        struct Input 
        {
            float2 uv_Atlas;
            float2 uv_TileIndex;
        };

        half _Glossiness;
        half _Metallic;
        fixed4 _Color;
        float _Tile;

        // Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
        // See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
        // #pragma instancing_options assumeuniformscaling
        UNITY_INSTANCING_BUFFER_START(Props)
        // put more per-instance properties here
        UNITY_INSTANCING_BUFFER_END(Props)

        void surf (Input IN, inout SurfaceOutputStandard o) 
        {

            float4 index = floor(tex2D(_TileIndex, IN.uv_TileIndex) * 255);
            //float index = _Tile;
            // Albedo comes from a texture tinted by color
            float3 uv = float3(IN.uv_Atlas, index.r);
            uv.xy -= (index.ba / 16);
            uv.xy /= (index.g / 16);
            float4 c = UNITY_SAMPLE_TEX2DARRAY (_Atlas, uv) * _Color;
            o.Albedo = c.rgb*c.a;
            // Metallic and smoothness come from slider variables
            o.Metallic = _Metallic;
            o.Smoothness = _Glossiness;
            o.Alpha = c.a;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
