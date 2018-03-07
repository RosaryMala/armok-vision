using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class DfColor
{
    static Color[] _colors =
    {
        new Color(0.0f, 0.0f, 0.0f),
        new Color(0.0f, 0.0f, 0.5f),
        new Color(0.0f, 0.5f, 0.0f),
        new Color(0.0f, 0.5f, 0.5f),
        new Color(0.5f, 0.0f, 0.0f),
        new Color(0.5f, 0.0f, 0.5f),
        new Color(0.5f, 0.5f, 0.0f),
        new Color(0.75f, 0.75f, 0.75f),
        new Color(0.5f, 0.5f, 0.5f),
        new Color(0.0f, 0.0f, 1.0f),
        new Color(0.0f, 1.0f, 0.0f),
        new Color(0.0f, 1.0f, 1.0f),
        new Color(1.0f, 0.0f, 0.0f),
        new Color(1.0f, 0.0f, 1.0f),
        new Color(1.0f, 1.0f, 0.0f),
        new Color(1.0f, 1.0f, 1.0f),
    };

    /// <summary>
    /// convert a DF color index to unity color.
    /// </summary>
    /// <param name="col">index from 0 to 15</param>
    /// <returns></returns>
    public static Color GetColor(int col)
    {
        return _colors[col];
    }

    public static Color MorphColor(Color color, int source, int dest)
    {
        if (source == dest)
            return color;
        float H, S, V, Hs, Ss, Vs, Hd, Sd, Vd;
        Color.RGBToHSV(color, out H, out S, out V);
        Color.RGBToHSV(GetColor(source), out Hs, out Ss, out Vs);
        Color.RGBToHSV(GetColor(dest), out Hd, out Sd, out Vd);
        H += (Hd - Hs);
        if (H > 1)
            H -= 1;
        if (H < 0)
            H += 1;
        if(Vs > Vd)
        {
            V *= (Vd / Vs);
        }
        else
        {
            V = 1 - ((1 - V) * ((1 - Vd) / (1 - Vs)));
        }
        return Color.HSVToRGB(H, S, V);
    }
}
