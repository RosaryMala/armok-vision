#ifndef BLEND_INC
#define BLEND_INC

fixed overlay(fixed a, fixed b)
{
#if UNITY_COLORSPACE_GAMMA
    return a < 0.5 ? (2 * a * b) : (1 - 2 * (1 - a) * (1 - b));
#else
    return a < 0.217638 ? (4.59479 * a * b) : (1 - 1.27818 * (1 - a) * (1 - b));
#endif
}

fixed3 overlay(fixed3 a, fixed3 b)
{
    return fixed3(overlay(a.r, b.r), overlay(a.g, b.g), overlay(a.b, b.b));
}

fixed4 overlay(fixed4 a, fixed3 b)
{
    return fixed4(overlay(a.r, b.r), overlay(a.g, b.g), overlay(a.b, b.b), a.a);
}

#endif