        void MixColor_float(float4 bottom, inout float4 bottom_n, float4 top, float4 top_n, float top_alpha)
        {
            top = float4(top.rgb, max(top.a + top_alpha - 1, 0));
            //crappy blending to test
            bottom_n = (top.a) > (bottom.a) ? top_n : bottom_n;
        }