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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace hqx
{
    /// <summary>
    /// Contains the color-blending operations used internally by hqx.
    /// </summary>
    internal static class Interpolation
    {
        const uint MaskAlpha = 0xff000000;
        const uint MaskGreen = 0x0000ff00;
        const uint MaskRedBlue = 0x00ff00ff;

        const int AlphaShift = 24;

        public static uint Mix3To1(uint c1, uint c2)
        {
            return MixColours(3, 1, c1, c2);
        }

        public static uint Mix2To1To1(uint c1, uint c2, uint c3)
        {
            return MixColours(2, 1, 1, c1, c2, c3);
        }

        public static uint Mix7To1(uint c1, uint c2)
        {
            return MixColours(7, 1, c1, c2);
        }

        public static uint Mix2To7To7(uint c1, uint c2, uint c3)
        {
            return MixColours(2, 7, 7, c1, c2, c3);
        }

        public static uint MixEven(uint c1, uint c2)
        {
            return MixColours(1, 1, c1, c2);
        }

        public static uint Mix5To2To1(uint c1, uint c2, uint c3)
        {
            return MixColours(5, 2, 1, c1, c2, c3);
        }

        public static uint Mix6To1To1(uint c1, uint c2, uint c3)
        {
            return MixColours(6, 1, 1, c1, c2, c3);
        }

        public static uint Mix5To3(uint c1, uint c2)
        {
            return MixColours(5, 3, c1, c2);
        }

        public static uint Mix2To3To3(uint c1, uint c2, uint c3)
        {
            return MixColours(2, 3, 3, c1, c2, c3);
        }

        public static uint Mix14To1To1(uint c1, uint c2, uint c3)
        {
            return MixColours(14, 1, 1, c1, c2, c3);
        }

        // This method can overflow between blue and red and from red to nothing when the sum of all weightings is higher than 255.
        // It only works for weightings with a sum that is a power of two, otherwise the blue value is corrupted.
        // Parameters: weighting0, weighting1[, ...], colour0, colour1[, ...]
        public static uint MixColours(params uint[] weightingsAndColours)
        {
            uint totalPartsColour = 0;
            uint totalPartsAlpha = 0;

            uint totalGreen = 0;
            uint totalRedBlue = 0;
            uint totalAlpha = 0;

            for (int i = 0; i < weightingsAndColours.Length / 2; i++)
            {
                var weighting = weightingsAndColours[i];
                var colour = weightingsAndColours[weightingsAndColours.Length / 2 + i];

                if (weighting > 0)
                {

                    var alpha = (colour >> AlphaShift) * weighting;

                    totalPartsAlpha += weighting;
                    if (alpha != 0)
                    {
                        totalAlpha += alpha;

                        totalPartsColour += weighting;
                        totalGreen += (colour & MaskGreen) * weighting;
                        totalRedBlue += (colour & MaskRedBlue) * weighting;
                    }
                }
            }

            totalAlpha /= totalPartsAlpha;
            totalAlpha <<= AlphaShift;

            if (totalPartsColour > 0)
            {
                totalGreen /= totalPartsColour;
                totalGreen &= MaskGreen;

                totalRedBlue /= totalPartsColour;
                totalRedBlue &= MaskRedBlue;
            }

            return totalAlpha | totalGreen | totalRedBlue;
        }
    }
}
