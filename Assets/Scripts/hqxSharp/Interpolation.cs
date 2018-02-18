/*
 * Copyright © 2003 Maxim Stepin (maxst@hiend3d.com)
 * 
 * Copyright © 2010 Cameron Zemek (grom@zeminvaders.net)
 * 
 * Copyright © 2011, 2012 Tamme Schichler (tamme.schichler@googlemail.com)
 * 
 * This file is part of hqxSharp.
 *
 * hqxSharp is free software: you can redistribute it and/or modify
 * it under the terms of the GNU Lesser General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * hqxSharp is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
 * GNU Lesser General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public License
 * along with hqxSharp. If not, see <http://www.gnu.org/licenses/>.
 */

using UnityEngine;

namespace hqx
{
    /// <summary>
    /// Contains the color-blending operations used internally by hqx.
    /// </summary>
    internal static class Interpolation
    {
        const int AlphaShift = 24;

        public static Color32 Mix3To1(Color32 c1, Color32 c2)
        {
            return MixColours(new WeightedColor(3, c1), new WeightedColor(1, c2));
        }

        public static Color32 Mix2To1To1(Color32 c1, Color32 c2, Color32 c3)
        {
            return MixColours(new WeightedColor(2, c1), new WeightedColor(1, c2), new WeightedColor(1, c3));
        }

        public static Color32 Mix7To1(Color32 c1, Color32 c2)
        {
            return MixColours(new WeightedColor(7, c1), new WeightedColor(1, c2));
        }

        public static Color32 Mix2To7To7(Color32 c1, Color32 c2, Color32 c3)
        {
            return MixColours(new WeightedColor(2, c1), new WeightedColor(7, c2), new WeightedColor(7, c3));
        }

        public static Color32 MixEven(Color32 c1, Color32 c2)
        {
            return MixColours(new WeightedColor(1, c1), new WeightedColor(1, c2));
        }

        public static Color32 Mix5To2To1(Color32 c1, Color32 c2, Color32 c3)
        {
            return MixColours(new WeightedColor(5, c1), new WeightedColor(2, c2), new WeightedColor(1, c3));
        }

        public static Color32 Mix6To1To1(Color32 c1, Color32 c2, Color32 c3)
        {
            return MixColours(new WeightedColor(6, c1), new WeightedColor(1, c2), new WeightedColor(1, c3));
        }

        public static Color32 Mix5To3(Color32 c1, Color32 c2)
        {
            return MixColours(new WeightedColor(5, c1), new WeightedColor(3, c2));
        }

        public static Color32 Mix2To3To3(Color32 c1, Color32 c2, Color32 c3)
        {
            return MixColours(new WeightedColor(2, c1), new WeightedColor(3, c2), new WeightedColor(3, c3));
        }

        public static Color32 Mix14To1To1(Color32 c1, Color32 c2, Color32 c3)
        {
            return MixColours(new WeightedColor(14, c1), new WeightedColor(1, c2), new WeightedColor(1, c3));
        }

        public struct WeightedColor
        {
            public readonly uint weight;
            public readonly Color32 color;
            public WeightedColor(uint weight, Color32 color)
            {
                this.weight = weight;
                this.color = color;
            }
        }

        public static Color32 MixColours(params WeightedColor[] weightingsAndColours)
        {
            uint totalPartsColour = 0;
            uint totalPartsAlpha = 0;

            uint totalGreen = 0;
            uint totalRed = 0;
            uint totalBlue = 0;
            uint totalAlpha = 0;

            for (int i = 0; i < weightingsAndColours.Length; i++)
            {
                var weighting = weightingsAndColours[i].weight;
                var colour = weightingsAndColours[i].color;

                if (weighting > 0)
                {

                    var alpha = colour.a * weighting;

                    totalPartsAlpha += weighting;
                    if (alpha != 0)
                    {
                        totalAlpha += alpha;

                        totalPartsColour += weighting;
                        totalRed += colour.r * weighting;
                        totalGreen += colour.g * weighting;
                        totalBlue += colour.b * weighting;
                    }
                }
            }

            totalAlpha /= totalPartsAlpha;

            if (totalPartsColour > 0)
            {
                totalRed /= totalPartsColour;
                totalGreen /= totalPartsColour;
                totalBlue /= totalPartsColour;
            }

            return new Color32((byte)totalRed, (byte)totalGreen, (byte)totalBlue, (byte)totalAlpha);
        }
    }
}
