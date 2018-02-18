/*
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

using System.Collections.Generic;
using UnityEngine;

namespace hqx
{
    struct ColorYUV
    {
        private static ColorYUV[] lookupTable;

        private readonly byte y;
        private readonly byte u;
        private readonly byte v;

        public byte Y
        {
            get
            {
                return y;
            }
        }
        public byte U
        {
            get
            {
                return u;
            }
        }
        public byte V
        {
            get
            {
                return v;
            }
        }

        public ColorYUV(byte Y, byte U, byte V)
        {
            y = Y;
            u = U;
            v = V;
        }

        public ColorYUV(Color32 color)
        {
            if(lookupTable == null)
            {
                Init();
            }
            this = lookupTable[ColorToInt(color)];
        }

        private static int ColorToInt(Color32 color)
        {
            return color.r + (color.g << 8) + (color.b << 16);
        }

        static void Init()
        {
            lookupTable = new ColorYUV[0x1000000];
            for (byte r = 0; r < 255; r++)
                for (byte g = 0; g < 255; g++)
                    for (byte b = 0; b < 255; b++)
                    {
                        Color32 color = new Color32(r, g, b, 255);
                        lookupTable[ColorToInt(color)] = GenNewColor(color);
                    }
        }

        static ColorYUV GenNewColor(Color32 color32)
        {
            byte y = (byte)(0.299 * color32.r + 0.587 * color32.g + 0.114 * color32.b);
            byte u = (byte)((int)(-0.169 * color32.r - 0.331 * color32.g + .5 * color32.b) + 128);
            byte v = (byte)((int)(0.5 * color32.r - 0.419 * color32.g - 0.081 * color32.b) + 128);

            return new ColorYUV(y, u, v);
        }

        public override bool Equals(object obj)
        {
            if (!(obj is ColorYUV))
                return false;
            ColorYUV col = (ColorYUV)obj;
            return col.y == y && col.u == u && col.v == v;
        }
        public override int GetHashCode()
        {
           return y | (u << 8) | (v << 16);
        }
        public override string ToString()
        {
            return string.Format("[{0}, {1}, {2}]", y, u - 128, v - 128);
        }
    }
}
