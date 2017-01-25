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
    public static partial class HqxSharp
    {
        /// <summary>
        /// This is the extended C# port of the hq4x algorithm.
        /// <para>The image is scaled to four times its size.</para>
        /// </summary>
        /// <param name="bitmap">The source image.</param>
        /// <param name="trY">The Y (luminance) threshold.</param>
        /// <param name="trU">The U (chrominance) threshold.</param>
        /// <param name="trV">The V (chrominance) threshold.</param>
        /// <param name="trA">The A (transparency) threshold.</param>
        /// <param name="wrapX">Used for images that can be seamlessly repeated horizontally.</param>
        /// <param name="wrapY">Used for images that can be seamlessly repeated vertically.</param>
        /// <returns>A new Bitmap instance that contains the source imagage scaled to four times its size.</returns>
        public static Texture2D Scale4(Texture2D bitmap, uint trY = 48, uint trU = 7, uint trV = 6, uint trA = 0, bool wrapX = false, bool wrapY = false)
        {
            int Xres = bitmap.width;
            int Yres = bitmap.height;

            var dest = new Texture2D(bitmap.width * 4, bitmap.height * 4);

            var bmpData = bitmap.GetPixels32();
            Color32[] destData = new Color32[bitmap.width * 4 * bitmap.height * 4];

            Scale4(bmpData, destData, Xres, Yres, trY, trU, trV, trA, wrapX, wrapY);

            dest.SetPixels32(destData);
            dest.Apply();

            return dest;
        }

        /// <summary>
        /// This is the extended C# port of the hq4x algorithm.
        /// <para>The destination image must be exactly four times as large in both dimensions as the source image.</para>
        /// </summary>
        /// <param name="source">A pointer to the source image.</param>
        /// <param name="dest">A pointer to the destination image.</param>
        /// <param name="Xres">The horizontal resolution of the source image.</param>
        /// <param name="Yres">The vertical resolution of the source image.</param>
        /// <param name="trY">The Y (luminance) threshold.</param>
        /// <param name="trU">The U (chrominance) threshold.</param>
        /// <param name="trV">The V (chrominance) threshold.</param>
        /// <param name="trA">The A (transparency) threshold.</param>
        /// <param name="wrapX">Used for images that can be seamlessly repeated horizontally.</param>
        /// <param name="wrapY">Used for images that can be seamlessly repeated vertically.</param>
        public static void Scale4(Color32[] source, Color32[] dest, int Xres, int Yres, uint trY = 48, uint trU = 7, uint trV = 6, uint trA = 0, bool wrapX = false, bool wrapY = false)
        {
            int dpL = Xres * 4;

            int sourceIndex, destIndex, prevline, nextline;
            Color32[] patch = new Color32[9];
            sourceIndex = destIndex = 0;

            for (int yy = 0; yy < Yres; yy++)
            {
                if (yy > 0)
                {
                    prevline = -Xres;
                }
                else
                {
                    if (wrapY)
                    {
                        prevline = Xres * (Yres - 1);
                    }
                    else
                    {
                        prevline = 0;
                    }
                }
                if (yy < Yres - 1)
                {
                    nextline = Xres;
                }
                else
                {
                    if (wrapY)
                    {
                        nextline = -(Xres * (Yres - 1));
                    }
                    else
                    {
                        nextline = 0;
                    }
                }

                for (int xx = 0; xx < Xres; xx++)
                {
                    patch[1] = source[sourceIndex + prevline];
                    patch[4] = source[sourceIndex];
                    patch[7] = source[sourceIndex + nextline];

                    if (xx > 0)
                    {
                        patch[0] = source[sourceIndex + prevline - 1];
                        patch[3] = source[sourceIndex - 1];
                        patch[6] = source[sourceIndex + nextline - 1];
                    }
                    else
                    {
                        if (wrapX)
                        {
                            patch[0] = source[sourceIndex + prevline + Xres - 1];
                            patch[3] = source[sourceIndex + Xres - 1];
                            patch[6] = source[sourceIndex + nextline + Xres - 1];
                        }
                        else
                        {
                            patch[0] = patch[1];
                            patch[3] = patch[4];
                            patch[6] = patch[7];
                        }
                    }

                    if (xx < Xres - 1)
                    {
                        patch[2] = source[sourceIndex + prevline + 1];
                        patch[5] = source[sourceIndex + 1];
                        patch[8] = source[sourceIndex + nextline + 1];
                    }
                    else
                    {
                        if (wrapX)
                        {
                            patch[2] = source[sourceIndex + prevline - Xres + 1];
                            patch[5] = source[sourceIndex - Xres + 1];
                            patch[8] = source[sourceIndex + nextline - Xres + 1];
                        }
                        else
                        {
                            patch[2] = patch[1];
                            patch[5] = patch[4];
                            patch[8] = patch[7];
                        }
                    }

                    int pattern = 0;
                    int flag = 1;

                    for (int patchIndex = 0; patchIndex < 9; patchIndex++)
                    {
                        if (patchIndex == 4) continue;

                        if (!IsEqual(patch[patchIndex], patch[4]))
                        {
                            if (Diff(patch[4], patch[patchIndex], trY, trU, trV, trA))
                                pattern |= flag;
                        }
                        flag <<= 1;
                    }

                    switch (pattern)
                    {
                        case 0:
                        case 1:
                        case 4:
                        case 32:
                        case 128:
                        case 5:
                        case 132:
                        case 160:
                        case 33:
                        case 129:
                        case 36:
                        case 133:
                        case 164:
                        case 161:
                        case 37:
                        case 165:
                            {
                                dest[destIndex] = Interpolation.Mix2To1To1(patch[4], patch[1], patch[3]);
                                dest[destIndex + 1] = Interpolation.Mix5To2To1(patch[4], patch[1], patch[3]);
                                dest[destIndex + 2] = Interpolation.Mix5To2To1(patch[4], patch[1], patch[5]);
                                dest[destIndex + 3] = Interpolation.Mix2To1To1(patch[4], patch[1], patch[5]);
                                dest[destIndex + dpL] = Interpolation.Mix5To2To1(patch[4], patch[3], patch[1]);
                                dest[destIndex + dpL + 1] = Interpolation.Mix6To1To1(patch[4], patch[3], patch[1]);
                                dest[destIndex + dpL + 2] = Interpolation.Mix6To1To1(patch[4], patch[5], patch[1]);
                                dest[destIndex + dpL + 3] = Interpolation.Mix5To2To1(patch[4], patch[5], patch[1]);
                                dest[destIndex + dpL + dpL] = Interpolation.Mix5To2To1(patch[4], patch[3], patch[7]);
                                dest[destIndex + dpL + dpL + 1] = Interpolation.Mix6To1To1(patch[4], patch[3], patch[7]);
                                dest[destIndex + dpL + dpL + 2] = Interpolation.Mix6To1To1(patch[4], patch[5], patch[7]);
                                dest[destIndex + dpL + dpL + 3] = Interpolation.Mix5To2To1(patch[4], patch[5], patch[7]);
                                dest[destIndex + dpL + dpL + dpL] = Interpolation.Mix2To1To1(patch[4], patch[7], patch[3]);
                                dest[destIndex + dpL + dpL + dpL + 1] = Interpolation.Mix5To2To1(patch[4], patch[7], patch[3]);
                                dest[destIndex + dpL + dpL + dpL + 2] = Interpolation.Mix5To2To1(patch[4], patch[7], patch[5]);
                                dest[destIndex + dpL + dpL + dpL + 3] = Interpolation.Mix2To1To1(patch[4], patch[7], patch[5]);
                                break;
                            }
                        case 2:
                        case 34:
                        case 130:
                        case 162:
                            {
                                dest[destIndex] = Interpolation.Mix5To3(patch[4], patch[0]);
                                dest[destIndex + 1] = Interpolation.Mix3To1(patch[4], patch[0]);
                                dest[destIndex + 2] = Interpolation.Mix3To1(patch[4], patch[2]);
                                dest[destIndex + 3] = Interpolation.Mix5To3(patch[4], patch[2]);
                                dest[destIndex + dpL] = Interpolation.Mix5To2To1(patch[4], patch[3], patch[0]);
                                dest[destIndex + dpL + 1] = Interpolation.Mix7To1(patch[4], patch[0]);
                                dest[destIndex + dpL + 2] = Interpolation.Mix7To1(patch[4], patch[2]);
                                dest[destIndex + dpL + 3] = Interpolation.Mix5To2To1(patch[4], patch[5], patch[2]);
                                dest[destIndex + dpL + dpL] = Interpolation.Mix5To2To1(patch[4], patch[3], patch[7]);
                                dest[destIndex + dpL + dpL + 1] = Interpolation.Mix6To1To1(patch[4], patch[3], patch[7]);
                                dest[destIndex + dpL + dpL + 2] = Interpolation.Mix6To1To1(patch[4], patch[5], patch[7]);
                                dest[destIndex + dpL + dpL + 3] = Interpolation.Mix5To2To1(patch[4], patch[5], patch[7]);
                                dest[destIndex + dpL + dpL + dpL] = Interpolation.Mix2To1To1(patch[4], patch[7], patch[3]);
                                dest[destIndex + dpL + dpL + dpL + 1] = Interpolation.Mix5To2To1(patch[4], patch[7], patch[3]);
                                dest[destIndex + dpL + dpL + dpL + 2] = Interpolation.Mix5To2To1(patch[4], patch[7], patch[5]);
                                dest[destIndex + dpL + dpL + dpL + 3] = Interpolation.Mix2To1To1(patch[4], patch[7], patch[5]);
                                break;
                            }
                        case 16:
                        case 17:
                        case 48:
                        case 49:
                            {
                                dest[destIndex] = Interpolation.Mix2To1To1(patch[4], patch[1], patch[3]);
                                dest[destIndex + 1] = Interpolation.Mix5To2To1(patch[4], patch[1], patch[3]);
                                dest[destIndex + 2] = Interpolation.Mix5To2To1(patch[4], patch[1], patch[2]);
                                dest[destIndex + 3] = Interpolation.Mix5To3(patch[4], patch[2]);
                                dest[destIndex + dpL] = Interpolation.Mix5To2To1(patch[4], patch[3], patch[1]);
                                dest[destIndex + dpL + 1] = Interpolation.Mix6To1To1(patch[4], patch[3], patch[1]);
                                dest[destIndex + dpL + 2] = Interpolation.Mix7To1(patch[4], patch[2]);
                                dest[destIndex + dpL + 3] = Interpolation.Mix3To1(patch[4], patch[2]);
                                dest[destIndex + dpL + dpL] = Interpolation.Mix5To2To1(patch[4], patch[3], patch[7]);
                                dest[destIndex + dpL + dpL + 1] = Interpolation.Mix6To1To1(patch[4], patch[3], patch[7]);
                                dest[destIndex + dpL + dpL + 2] = Interpolation.Mix7To1(patch[4], patch[8]);
                                dest[destIndex + dpL + dpL + 3] = Interpolation.Mix3To1(patch[4], patch[8]);
                                dest[destIndex + dpL + dpL + dpL] = Interpolation.Mix2To1To1(patch[4], patch[7], patch[3]);
                                dest[destIndex + dpL + dpL + dpL + 1] = Interpolation.Mix5To2To1(patch[4], patch[7], patch[3]);
                                dest[destIndex + dpL + dpL + dpL + 2] = Interpolation.Mix5To2To1(patch[4], patch[7], patch[8]);
                                dest[destIndex + dpL + dpL + dpL + 3] = Interpolation.Mix5To3(patch[4], patch[8]);
                                break;
                            }
                        case 64:
                        case 65:
                        case 68:
                        case 69:
                            {
                                dest[destIndex] = Interpolation.Mix2To1To1(patch[4], patch[1], patch[3]);
                                dest[destIndex + 1] = Interpolation.Mix5To2To1(patch[4], patch[1], patch[3]);
                                dest[destIndex + 2] = Interpolation.Mix5To2To1(patch[4], patch[1], patch[5]);
                                dest[destIndex + 3] = Interpolation.Mix2To1To1(patch[4], patch[1], patch[5]);
                                dest[destIndex + dpL] = Interpolation.Mix5To2To1(patch[4], patch[3], patch[1]);
                                dest[destIndex + dpL + 1] = Interpolation.Mix6To1To1(patch[4], patch[3], patch[1]);
                                dest[destIndex + dpL + 2] = Interpolation.Mix6To1To1(patch[4], patch[5], patch[1]);
                                dest[destIndex + dpL + 3] = Interpolation.Mix5To2To1(patch[4], patch[5], patch[1]);
                                dest[destIndex + dpL + dpL] = Interpolation.Mix5To2To1(patch[4], patch[3], patch[6]);
                                dest[destIndex + dpL + dpL + 1] = Interpolation.Mix7To1(patch[4], patch[6]);
                                dest[destIndex + dpL + dpL + 2] = Interpolation.Mix7To1(patch[4], patch[8]);
                                dest[destIndex + dpL + dpL + 3] = Interpolation.Mix5To2To1(patch[4], patch[5], patch[8]);
                                dest[destIndex + dpL + dpL + dpL] = Interpolation.Mix5To3(patch[4], patch[6]);
                                dest[destIndex + dpL + dpL + dpL + 1] = Interpolation.Mix3To1(patch[4], patch[6]);
                                dest[destIndex + dpL + dpL + dpL + 2] = Interpolation.Mix3To1(patch[4], patch[8]);
                                dest[destIndex + dpL + dpL + dpL + 3] = Interpolation.Mix5To3(patch[4], patch[8]);
                                break;
                            }
                        case 8:
                        case 12:
                        case 136:
                        case 140:
                            {
                                dest[destIndex] = Interpolation.Mix5To3(patch[4], patch[0]);
                                dest[destIndex + 1] = Interpolation.Mix5To2To1(patch[4], patch[1], patch[0]);
                                dest[destIndex + 2] = Interpolation.Mix5To2To1(patch[4], patch[1], patch[5]);
                                dest[destIndex + 3] = Interpolation.Mix2To1To1(patch[4], patch[1], patch[5]);
                                dest[destIndex + dpL] = Interpolation.Mix3To1(patch[4], patch[0]);
                                dest[destIndex + dpL + 1] = Interpolation.Mix7To1(patch[4], patch[0]);
                                dest[destIndex + dpL + 2] = Interpolation.Mix6To1To1(patch[4], patch[5], patch[1]);
                                dest[destIndex + dpL + 3] = Interpolation.Mix5To2To1(patch[4], patch[5], patch[1]);
                                dest[destIndex + dpL + dpL] = Interpolation.Mix3To1(patch[4], patch[6]);
                                dest[destIndex + dpL + dpL + 1] = Interpolation.Mix7To1(patch[4], patch[6]);
                                dest[destIndex + dpL + dpL + 2] = Interpolation.Mix6To1To1(patch[4], patch[5], patch[7]);
                                dest[destIndex + dpL + dpL + 3] = Interpolation.Mix5To2To1(patch[4], patch[5], patch[7]);
                                dest[destIndex + dpL + dpL + dpL] = Interpolation.Mix5To3(patch[4], patch[6]);
                                dest[destIndex + dpL + dpL + dpL + 1] = Interpolation.Mix5To2To1(patch[4], patch[7], patch[6]);
                                dest[destIndex + dpL + dpL + dpL + 2] = Interpolation.Mix5To2To1(patch[4], patch[7], patch[5]);
                                dest[destIndex + dpL + dpL + dpL + 3] = Interpolation.Mix2To1To1(patch[4], patch[7], patch[5]);
                                break;
                            }
                        case 3:
                        case 35:
                        case 131:
                        case 163:
                            {
                                dest[destIndex] = Interpolation.Mix5To3(patch[4], patch[3]);
                                dest[destIndex + 1] = Interpolation.Mix7To1(patch[4], patch[3]);
                                dest[destIndex + 2] = Interpolation.Mix3To1(patch[4], patch[2]);
                                dest[destIndex + 3] = Interpolation.Mix5To3(patch[4], patch[2]);
                                dest[destIndex + dpL] = Interpolation.Mix5To3(patch[4], patch[3]);
                                dest[destIndex + dpL + 1] = Interpolation.Mix7To1(patch[4], patch[3]);
                                dest[destIndex + dpL + 2] = Interpolation.Mix7To1(patch[4], patch[2]);
                                dest[destIndex + dpL + 3] = Interpolation.Mix5To2To1(patch[4], patch[5], patch[2]);
                                dest[destIndex + dpL + dpL] = Interpolation.Mix5To2To1(patch[4], patch[3], patch[7]);
                                dest[destIndex + dpL + dpL + 1] = Interpolation.Mix6To1To1(patch[4], patch[3], patch[7]);
                                dest[destIndex + dpL + dpL + 2] = Interpolation.Mix6To1To1(patch[4], patch[5], patch[7]);
                                dest[destIndex + dpL + dpL + 3] = Interpolation.Mix5To2To1(patch[4], patch[5], patch[7]);
                                dest[destIndex + dpL + dpL + dpL] = Interpolation.Mix2To1To1(patch[4], patch[7], patch[3]);
                                dest[destIndex + dpL + dpL + dpL + 1] = Interpolation.Mix5To2To1(patch[4], patch[7], patch[3]);
                                dest[destIndex + dpL + dpL + dpL + 2] = Interpolation.Mix5To2To1(patch[4], patch[7], patch[5]);
                                dest[destIndex + dpL + dpL + dpL + 3] = Interpolation.Mix2To1To1(patch[4], patch[7], patch[5]);
                                break;
                            }
                        case 6:
                        case 38:
                        case 134:
                        case 166:
                            {
                                dest[destIndex] = Interpolation.Mix5To3(patch[4], patch[0]);
                                dest[destIndex + 1] = Interpolation.Mix3To1(patch[4], patch[0]);
                                dest[destIndex + 2] = Interpolation.Mix7To1(patch[4], patch[5]);
                                dest[destIndex + 3] = Interpolation.Mix5To3(patch[4], patch[5]);
                                dest[destIndex + dpL] = Interpolation.Mix5To2To1(patch[4], patch[3], patch[0]);
                                dest[destIndex + dpL + 1] = Interpolation.Mix7To1(patch[4], patch[0]);
                                dest[destIndex + dpL + 2] = Interpolation.Mix7To1(patch[4], patch[5]);
                                dest[destIndex + dpL + 3] = Interpolation.Mix5To3(patch[4], patch[5]);
                                dest[destIndex + dpL + dpL] = Interpolation.Mix5To2To1(patch[4], patch[3], patch[7]);
                                dest[destIndex + dpL + dpL + 1] = Interpolation.Mix6To1To1(patch[4], patch[3], patch[7]);
                                dest[destIndex + dpL + dpL + 2] = Interpolation.Mix6To1To1(patch[4], patch[5], patch[7]);
                                dest[destIndex + dpL + dpL + 3] = Interpolation.Mix5To2To1(patch[4], patch[5], patch[7]);
                                dest[destIndex + dpL + dpL + dpL] = Interpolation.Mix2To1To1(patch[4], patch[7], patch[3]);
                                dest[destIndex + dpL + dpL + dpL + 1] = Interpolation.Mix5To2To1(patch[4], patch[7], patch[3]);
                                dest[destIndex + dpL + dpL + dpL + 2] = Interpolation.Mix5To2To1(patch[4], patch[7], patch[5]);
                                dest[destIndex + dpL + dpL + dpL + 3] = Interpolation.Mix2To1To1(patch[4], patch[7], patch[5]);
                                break;
                            }
                        case 20:
                        case 21:
                        case 52:
                        case 53:
                            {
                                dest[destIndex] = Interpolation.Mix2To1To1(patch[4], patch[1], patch[3]);
                                dest[destIndex + 1] = Interpolation.Mix5To2To1(patch[4], patch[1], patch[3]);
                                dest[destIndex + 2] = Interpolation.Mix5To3(patch[4], patch[1]);
                                dest[destIndex + 3] = Interpolation.Mix5To3(patch[4], patch[1]);
                                dest[destIndex + dpL] = Interpolation.Mix5To2To1(patch[4], patch[3], patch[1]);
                                dest[destIndex + dpL + 1] = Interpolation.Mix6To1To1(patch[4], patch[3], patch[1]);
                                dest[destIndex + dpL + 2] = Interpolation.Mix7To1(patch[4], patch[1]);
                                dest[destIndex + dpL + 3] = Interpolation.Mix7To1(patch[4], patch[1]);
                                dest[destIndex + dpL + dpL] = Interpolation.Mix5To2To1(patch[4], patch[3], patch[7]);
                                dest[destIndex + dpL + dpL + 1] = Interpolation.Mix6To1To1(patch[4], patch[3], patch[7]);
                                dest[destIndex + dpL + dpL + 2] = Interpolation.Mix7To1(patch[4], patch[8]);
                                dest[destIndex + dpL + dpL + 3] = Interpolation.Mix3To1(patch[4], patch[8]);
                                dest[destIndex + dpL + dpL + dpL] = Interpolation.Mix2To1To1(patch[4], patch[7], patch[3]);
                                dest[destIndex + dpL + dpL + dpL + 1] = Interpolation.Mix5To2To1(patch[4], patch[7], patch[3]);
                                dest[destIndex + dpL + dpL + dpL + 2] = Interpolation.Mix5To2To1(patch[4], patch[7], patch[8]);
                                dest[destIndex + dpL + dpL + dpL + 3] = Interpolation.Mix5To3(patch[4], patch[8]);
                                break;
                            }
                        case 144:
                        case 145:
                        case 176:
                        case 177:
                            {
                                dest[destIndex] = Interpolation.Mix2To1To1(patch[4], patch[1], patch[3]);
                                dest[destIndex + 1] = Interpolation.Mix5To2To1(patch[4], patch[1], patch[3]);
                                dest[destIndex + 2] = Interpolation.Mix5To2To1(patch[4], patch[1], patch[2]);
                                dest[destIndex + 3] = Interpolation.Mix5To3(patch[4], patch[2]);
                                dest[destIndex + dpL] = Interpolation.Mix5To2To1(patch[4], patch[3], patch[1]);
                                dest[destIndex + dpL + 1] = Interpolation.Mix6To1To1(patch[4], patch[3], patch[1]);
                                dest[destIndex + dpL + 2] = Interpolation.Mix7To1(patch[4], patch[2]);
                                dest[destIndex + dpL + 3] = Interpolation.Mix3To1(patch[4], patch[2]);
                                dest[destIndex + dpL + dpL] = Interpolation.Mix5To2To1(patch[4], patch[3], patch[7]);
                                dest[destIndex + dpL + dpL + 1] = Interpolation.Mix6To1To1(patch[4], patch[3], patch[7]);
                                dest[destIndex + dpL + dpL + 2] = Interpolation.Mix7To1(patch[4], patch[7]);
                                dest[destIndex + dpL + dpL + 3] = Interpolation.Mix7To1(patch[4], patch[7]);
                                dest[destIndex + dpL + dpL + dpL] = Interpolation.Mix2To1To1(patch[4], patch[7], patch[3]);
                                dest[destIndex + dpL + dpL + dpL + 1] = Interpolation.Mix5To2To1(patch[4], patch[7], patch[3]);
                                dest[destIndex + dpL + dpL + dpL + 2] = Interpolation.Mix5To3(patch[4], patch[7]);
                                dest[destIndex + dpL + dpL + dpL + 3] = Interpolation.Mix5To3(patch[4], patch[7]);
                                break;
                            }
                        case 192:
                        case 193:
                        case 196:
                        case 197:
                            {
                                dest[destIndex] = Interpolation.Mix2To1To1(patch[4], patch[1], patch[3]);
                                dest[destIndex + 1] = Interpolation.Mix5To2To1(patch[4], patch[1], patch[3]);
                                dest[destIndex + 2] = Interpolation.Mix5To2To1(patch[4], patch[1], patch[5]);
                                dest[destIndex + 3] = Interpolation.Mix2To1To1(patch[4], patch[1], patch[5]);
                                dest[destIndex + dpL] = Interpolation.Mix5To2To1(patch[4], patch[3], patch[1]);
                                dest[destIndex + dpL + 1] = Interpolation.Mix6To1To1(patch[4], patch[3], patch[1]);
                                dest[destIndex + dpL + 2] = Interpolation.Mix6To1To1(patch[4], patch[5], patch[1]);
                                dest[destIndex + dpL + 3] = Interpolation.Mix5To2To1(patch[4], patch[5], patch[1]);
                                dest[destIndex + dpL + dpL] = Interpolation.Mix5To2To1(patch[4], patch[3], patch[6]);
                                dest[destIndex + dpL + dpL + 1] = Interpolation.Mix7To1(patch[4], patch[6]);
                                dest[destIndex + dpL + dpL + 2] = Interpolation.Mix7To1(patch[4], patch[5]);
                                dest[destIndex + dpL + dpL + 3] = Interpolation.Mix5To3(patch[4], patch[5]);
                                dest[destIndex + dpL + dpL + dpL] = Interpolation.Mix5To3(patch[4], patch[6]);
                                dest[destIndex + dpL + dpL + dpL + 1] = Interpolation.Mix3To1(patch[4], patch[6]);
                                dest[destIndex + dpL + dpL + dpL + 2] = Interpolation.Mix7To1(patch[4], patch[5]);
                                dest[destIndex + dpL + dpL + dpL + 3] = Interpolation.Mix5To3(patch[4], patch[5]);
                                break;
                            }
                        case 96:
                        case 97:
                        case 100:
                        case 101:
                            {
                                dest[destIndex] = Interpolation.Mix2To1To1(patch[4], patch[1], patch[3]);
                                dest[destIndex + 1] = Interpolation.Mix5To2To1(patch[4], patch[1], patch[3]);
                                dest[destIndex + 2] = Interpolation.Mix5To2To1(patch[4], patch[1], patch[5]);
                                dest[destIndex + 3] = Interpolation.Mix2To1To1(patch[4], patch[1], patch[5]);
                                dest[destIndex + dpL] = Interpolation.Mix5To2To1(patch[4], patch[3], patch[1]);
                                dest[destIndex + dpL + 1] = Interpolation.Mix6To1To1(patch[4], patch[3], patch[1]);
                                dest[destIndex + dpL + 2] = Interpolation.Mix6To1To1(patch[4], patch[5], patch[1]);
                                dest[destIndex + dpL + 3] = Interpolation.Mix5To2To1(patch[4], patch[5], patch[1]);
                                dest[destIndex + dpL + dpL] = Interpolation.Mix5To3(patch[4], patch[3]);
                                dest[destIndex + dpL + dpL + 1] = Interpolation.Mix7To1(patch[4], patch[3]);
                                dest[destIndex + dpL + dpL + 2] = Interpolation.Mix7To1(patch[4], patch[8]);
                                dest[destIndex + dpL + dpL + 3] = Interpolation.Mix5To2To1(patch[4], patch[5], patch[8]);
                                dest[destIndex + dpL + dpL + dpL] = Interpolation.Mix5To3(patch[4], patch[3]);
                                dest[destIndex + dpL + dpL + dpL + 1] = Interpolation.Mix7To1(patch[4], patch[3]);
                                dest[destIndex + dpL + dpL + dpL + 2] = Interpolation.Mix3To1(patch[4], patch[8]);
                                dest[destIndex + dpL + dpL + dpL + 3] = Interpolation.Mix5To3(patch[4], patch[8]);
                                break;
                            }
                        case 40:
                        case 44:
                        case 168:
                        case 172:
                            {
                                dest[destIndex] = Interpolation.Mix5To3(patch[4], patch[0]);
                                dest[destIndex + 1] = Interpolation.Mix5To2To1(patch[4], patch[1], patch[0]);
                                dest[destIndex + 2] = Interpolation.Mix5To2To1(patch[4], patch[1], patch[5]);
                                dest[destIndex + 3] = Interpolation.Mix2To1To1(patch[4], patch[1], patch[5]);
                                dest[destIndex + dpL] = Interpolation.Mix3To1(patch[4], patch[0]);
                                dest[destIndex + dpL + 1] = Interpolation.Mix7To1(patch[4], patch[0]);
                                dest[destIndex + dpL + 2] = Interpolation.Mix6To1To1(patch[4], patch[5], patch[1]);
                                dest[destIndex + dpL + 3] = Interpolation.Mix5To2To1(patch[4], patch[5], patch[1]);
                                dest[destIndex + dpL + dpL] = Interpolation.Mix7To1(patch[4], patch[7]);
                                dest[destIndex + dpL + dpL + 1] = Interpolation.Mix7To1(patch[4], patch[7]);
                                dest[destIndex + dpL + dpL + 2] = Interpolation.Mix6To1To1(patch[4], patch[5], patch[7]);
                                dest[destIndex + dpL + dpL + 3] = Interpolation.Mix5To2To1(patch[4], patch[5], patch[7]);
                                dest[destIndex + dpL + dpL + dpL] = Interpolation.Mix5To3(patch[4], patch[7]);
                                dest[destIndex + dpL + dpL + dpL + 1] = Interpolation.Mix5To3(patch[4], patch[7]);
                                dest[destIndex + dpL + dpL + dpL + 2] = Interpolation.Mix5To2To1(patch[4], patch[7], patch[5]);
                                dest[destIndex + dpL + dpL + dpL + 3] = Interpolation.Mix2To1To1(patch[4], patch[7], patch[5]);
                                break;
                            }
                        case 9:
                        case 13:
                        case 137:
                        case 141:
                            {
                                dest[destIndex] = Interpolation.Mix5To3(patch[4], patch[1]);
                                dest[destIndex + 1] = Interpolation.Mix5To3(patch[4], patch[1]);
                                dest[destIndex + 2] = Interpolation.Mix5To2To1(patch[4], patch[1], patch[5]);
                                dest[destIndex + 3] = Interpolation.Mix2To1To1(patch[4], patch[1], patch[5]);
                                dest[destIndex + dpL] = Interpolation.Mix7To1(patch[4], patch[1]);
                                dest[destIndex + dpL + 1] = Interpolation.Mix7To1(patch[4], patch[1]);
                                dest[destIndex + dpL + 2] = Interpolation.Mix6To1To1(patch[4], patch[5], patch[1]);
                                dest[destIndex + dpL + 3] = Interpolation.Mix5To2To1(patch[4], patch[5], patch[1]);
                                dest[destIndex + dpL + dpL] = Interpolation.Mix3To1(patch[4], patch[6]);
                                dest[destIndex + dpL + dpL + 1] = Interpolation.Mix7To1(patch[4], patch[6]);
                                dest[destIndex + dpL + dpL + 2] = Interpolation.Mix6To1To1(patch[4], patch[5], patch[7]);
                                dest[destIndex + dpL + dpL + 3] = Interpolation.Mix5To2To1(patch[4], patch[5], patch[7]);
                                dest[destIndex + dpL + dpL + dpL] = Interpolation.Mix5To3(patch[4], patch[6]);
                                dest[destIndex + dpL + dpL + dpL + 1] = Interpolation.Mix5To2To1(patch[4], patch[7], patch[6]);
                                dest[destIndex + dpL + dpL + dpL + 2] = Interpolation.Mix5To2To1(patch[4], patch[7], patch[5]);
                                dest[destIndex + dpL + dpL + dpL + 3] = Interpolation.Mix2To1To1(patch[4], patch[7], patch[5]);
                                break;
                            }
                        case 18:
                        case 50:
                            {
                                dest[destIndex] = Interpolation.Mix5To3(patch[4], patch[0]);
                                dest[destIndex + 1] = Interpolation.Mix3To1(patch[4], patch[0]);
                                if (Diff(patch[1], patch[5], trY, trU, trV, trA))
                                {
                                    dest[destIndex + 2] = Interpolation.Mix3To1(patch[4], patch[2]);
                                    dest[destIndex + 3] = Interpolation.Mix5To3(patch[4], patch[2]);
                                    dest[destIndex + dpL + 2] = Interpolation.Mix7To1(patch[4], patch[2]);
                                    dest[destIndex + dpL + 3] = Interpolation.Mix3To1(patch[4], patch[2]);
                                }
                                else
                                {
                                    dest[destIndex + 2] = Interpolation.MixEven(patch[1], patch[4]);
                                    dest[destIndex + 3] = Interpolation.MixEven(patch[1], patch[5]);
                                    dest[destIndex + dpL + 2] = patch[4];
                                    dest[destIndex + dpL + 3] = Interpolation.MixEven(patch[5], patch[4]);
                                }
                                dest[destIndex + dpL] = Interpolation.Mix5To2To1(patch[4], patch[3], patch[0]);
                                dest[destIndex + dpL + 1] = Interpolation.Mix7To1(patch[4], patch[0]);
                                dest[destIndex + dpL + dpL] = Interpolation.Mix5To2To1(patch[4], patch[3], patch[7]);
                                dest[destIndex + dpL + dpL + 1] = Interpolation.Mix6To1To1(patch[4], patch[3], patch[7]);
                                dest[destIndex + dpL + dpL + 2] = Interpolation.Mix7To1(patch[4], patch[8]);
                                dest[destIndex + dpL + dpL + 3] = Interpolation.Mix3To1(patch[4], patch[8]);
                                dest[destIndex + dpL + dpL + dpL] = Interpolation.Mix2To1To1(patch[4], patch[7], patch[3]);
                                dest[destIndex + dpL + dpL + dpL + 1] = Interpolation.Mix5To2To1(patch[4], patch[7], patch[3]);
                                dest[destIndex + dpL + dpL + dpL + 2] = Interpolation.Mix5To2To1(patch[4], patch[7], patch[8]);
                                dest[destIndex + dpL + dpL + dpL + 3] = Interpolation.Mix5To3(patch[4], patch[8]);
                                break;
                            }
                        case 80:
                        case 81:
                            {
                                dest[destIndex] = Interpolation.Mix2To1To1(patch[4], patch[1], patch[3]);
                                dest[destIndex + 1] = Interpolation.Mix5To2To1(patch[4], patch[1], patch[3]);
                                dest[destIndex + 2] = Interpolation.Mix5To2To1(patch[4], patch[1], patch[2]);
                                dest[destIndex + 3] = Interpolation.Mix5To3(patch[4], patch[2]);
                                dest[destIndex + dpL] = Interpolation.Mix5To2To1(patch[4], patch[3], patch[1]);
                                dest[destIndex + dpL + 1] = Interpolation.Mix6To1To1(patch[4], patch[3], patch[1]);
                                dest[destIndex + dpL + 2] = Interpolation.Mix7To1(patch[4], patch[2]);
                                dest[destIndex + dpL + 3] = Interpolation.Mix3To1(patch[4], patch[2]);
                                dest[destIndex + dpL + dpL] = Interpolation.Mix5To2To1(patch[4], patch[3], patch[6]);
                                dest[destIndex + dpL + dpL + 1] = Interpolation.Mix7To1(patch[4], patch[6]);
                                if (Diff(patch[5], patch[7], trY, trU, trV, trA))
                                {
                                    dest[destIndex + dpL + dpL + 2] = Interpolation.Mix7To1(patch[4], patch[8]);
                                    dest[destIndex + dpL + dpL + 3] = Interpolation.Mix3To1(patch[4], patch[8]);
                                    dest[destIndex + dpL + dpL + dpL + 2] = Interpolation.Mix3To1(patch[4], patch[8]);
                                    dest[destIndex + dpL + dpL + dpL + 3] = Interpolation.Mix5To3(patch[4], patch[8]);
                                }
                                else
                                {
                                    dest[destIndex + dpL + dpL + 2] = patch[4];
                                    dest[destIndex + dpL + dpL + 3] = Interpolation.MixEven(patch[5], patch[4]);
                                    dest[destIndex + dpL + dpL + dpL + 2] = Interpolation.MixEven(patch[7], patch[4]);
                                    dest[destIndex + dpL + dpL + dpL + 3] = Interpolation.MixEven(patch[7], patch[5]);
                                }
                                dest[destIndex + dpL + dpL + dpL] = Interpolation.Mix5To3(patch[4], patch[6]);
                                dest[destIndex + dpL + dpL + dpL + 1] = Interpolation.Mix3To1(patch[4], patch[6]);
                                break;
                            }
                        case 72:
                        case 76:
                            {
                                dest[destIndex] = Interpolation.Mix5To3(patch[4], patch[0]);
                                dest[destIndex + 1] = Interpolation.Mix5To2To1(patch[4], patch[1], patch[0]);
                                dest[destIndex + 2] = Interpolation.Mix5To2To1(patch[4], patch[1], patch[5]);
                                dest[destIndex + 3] = Interpolation.Mix2To1To1(patch[4], patch[1], patch[5]);
                                dest[destIndex + dpL] = Interpolation.Mix3To1(patch[4], patch[0]);
                                dest[destIndex + dpL + 1] = Interpolation.Mix7To1(patch[4], patch[0]);
                                dest[destIndex + dpL + 2] = Interpolation.Mix6To1To1(patch[4], patch[5], patch[1]);
                                dest[destIndex + dpL + 3] = Interpolation.Mix5To2To1(patch[4], patch[5], patch[1]);
                                if (Diff(patch[7], patch[3], trY, trU, trV, trA))
                                {
                                    dest[destIndex + dpL + dpL] = Interpolation.Mix3To1(patch[4], patch[6]);
                                    dest[destIndex + dpL + dpL + 1] = Interpolation.Mix7To1(patch[4], patch[6]);
                                    dest[destIndex + dpL + dpL + dpL] = Interpolation.Mix5To3(patch[4], patch[6]);
                                    dest[destIndex + dpL + dpL + dpL + 1] = Interpolation.Mix3To1(patch[4], patch[6]);
                                }
                                else
                                {
                                    dest[destIndex + dpL + dpL] = Interpolation.MixEven(patch[3], patch[4]);
                                    dest[destIndex + dpL + dpL + 1] = patch[4];
                                    dest[destIndex + dpL + dpL + dpL] = Interpolation.MixEven(patch[7], patch[3]);
                                    dest[destIndex + dpL + dpL + dpL + 1] = Interpolation.MixEven(patch[7], patch[4]);
                                }
                                dest[destIndex + dpL + dpL + 2] = Interpolation.Mix7To1(patch[4], patch[8]);
                                dest[destIndex + dpL + dpL + 3] = Interpolation.Mix5To2To1(patch[4], patch[5], patch[8]);
                                dest[destIndex + dpL + dpL + dpL + 2] = Interpolation.Mix3To1(patch[4], patch[8]);
                                dest[destIndex + dpL + dpL + dpL + 3] = Interpolation.Mix5To3(patch[4], patch[8]);
                                break;
                            }
                        case 10:
                        case 138:
                            {
                                if (Diff(patch[3], patch[1], trY, trU, trV, trA))
                                {
                                    dest[destIndex] = Interpolation.Mix5To3(patch[4], patch[0]);
                                    dest[destIndex + 1] = Interpolation.Mix3To1(patch[4], patch[0]);
                                    dest[destIndex + dpL] = Interpolation.Mix3To1(patch[4], patch[0]);
                                    dest[destIndex + dpL + 1] = Interpolation.Mix7To1(patch[4], patch[0]);
                                }
                                else
                                {
                                    dest[destIndex] = Interpolation.MixEven(patch[1], patch[3]);
                                    dest[destIndex + 1] = Interpolation.MixEven(patch[1], patch[4]);
                                    dest[destIndex + dpL] = Interpolation.MixEven(patch[3], patch[4]);
                                    dest[destIndex + dpL + 1] = patch[4];
                                }
                                dest[destIndex + 2] = Interpolation.Mix3To1(patch[4], patch[2]);
                                dest[destIndex + 3] = Interpolation.Mix5To3(patch[4], patch[2]);
                                dest[destIndex + dpL + 2] = Interpolation.Mix7To1(patch[4], patch[2]);
                                dest[destIndex + dpL + 3] = Interpolation.Mix5To2To1(patch[4], patch[5], patch[2]);
                                dest[destIndex + dpL + dpL] = Interpolation.Mix3To1(patch[4], patch[6]);
                                dest[destIndex + dpL + dpL + 1] = Interpolation.Mix7To1(patch[4], patch[6]);
                                dest[destIndex + dpL + dpL + 2] = Interpolation.Mix6To1To1(patch[4], patch[5], patch[7]);
                                dest[destIndex + dpL + dpL + 3] = Interpolation.Mix5To2To1(patch[4], patch[5], patch[7]);
                                dest[destIndex + dpL + dpL + dpL] = Interpolation.Mix5To3(patch[4], patch[6]);
                                dest[destIndex + dpL + dpL + dpL + 1] = Interpolation.Mix5To2To1(patch[4], patch[7], patch[6]);
                                dest[destIndex + dpL + dpL + dpL + 2] = Interpolation.Mix5To2To1(patch[4], patch[7], patch[5]);
                                dest[destIndex + dpL + dpL + dpL + 3] = Interpolation.Mix2To1To1(patch[4], patch[7], patch[5]);
                                break;
                            }
                        case 66:
                            {
                                dest[destIndex] = Interpolation.Mix5To3(patch[4], patch[0]);
                                dest[destIndex + 1] = Interpolation.Mix3To1(patch[4], patch[0]);
                                dest[destIndex + 2] = Interpolation.Mix3To1(patch[4], patch[2]);
                                dest[destIndex + 3] = Interpolation.Mix5To3(patch[4], patch[2]);
                                dest[destIndex + dpL] = Interpolation.Mix5To2To1(patch[4], patch[3], patch[0]);
                                dest[destIndex + dpL + 1] = Interpolation.Mix7To1(patch[4], patch[0]);
                                dest[destIndex + dpL + 2] = Interpolation.Mix7To1(patch[4], patch[2]);
                                dest[destIndex + dpL + 3] = Interpolation.Mix5To2To1(patch[4], patch[5], patch[2]);
                                dest[destIndex + dpL + dpL] = Interpolation.Mix5To2To1(patch[4], patch[3], patch[6]);
                                dest[destIndex + dpL + dpL + 1] = Interpolation.Mix7To1(patch[4], patch[6]);
                                dest[destIndex + dpL + dpL + 2] = Interpolation.Mix7To1(patch[4], patch[8]);
                                dest[destIndex + dpL + dpL + 3] = Interpolation.Mix5To2To1(patch[4], patch[5], patch[8]);
                                dest[destIndex + dpL + dpL + dpL] = Interpolation.Mix5To3(patch[4], patch[6]);
                                dest[destIndex + dpL + dpL + dpL + 1] = Interpolation.Mix3To1(patch[4], patch[6]);
                                dest[destIndex + dpL + dpL + dpL + 2] = Interpolation.Mix3To1(patch[4], patch[8]);
                                dest[destIndex + dpL + dpL + dpL + 3] = Interpolation.Mix5To3(patch[4], patch[8]);
                                break;
                            }
                        case 24:
                            {
                                dest[destIndex] = Interpolation.Mix5To3(patch[4], patch[0]);
                                dest[destIndex + 1] = Interpolation.Mix5To2To1(patch[4], patch[1], patch[0]);
                                dest[destIndex + 2] = Interpolation.Mix5To2To1(patch[4], patch[1], patch[2]);
                                dest[destIndex + 3] = Interpolation.Mix5To3(patch[4], patch[2]);
                                dest[destIndex + dpL] = Interpolation.Mix3To1(patch[4], patch[0]);
                                dest[destIndex + dpL + 1] = Interpolation.Mix7To1(patch[4], patch[0]);
                                dest[destIndex + dpL + 2] = Interpolation.Mix7To1(patch[4], patch[2]);
                                dest[destIndex + dpL + 3] = Interpolation.Mix3To1(patch[4], patch[2]);
                                dest[destIndex + dpL + dpL] = Interpolation.Mix3To1(patch[4], patch[6]);
                                dest[destIndex + dpL + dpL + 1] = Interpolation.Mix7To1(patch[4], patch[6]);
                                dest[destIndex + dpL + dpL + 2] = Interpolation.Mix7To1(patch[4], patch[8]);
                                dest[destIndex + dpL + dpL + 3] = Interpolation.Mix3To1(patch[4], patch[8]);
                                dest[destIndex + dpL + dpL + dpL] = Interpolation.Mix5To3(patch[4], patch[6]);
                                dest[destIndex + dpL + dpL + dpL + 1] = Interpolation.Mix5To2To1(patch[4], patch[7], patch[6]);
                                dest[destIndex + dpL + dpL + dpL + 2] = Interpolation.Mix5To2To1(patch[4], patch[7], patch[8]);
                                dest[destIndex + dpL + dpL + dpL + 3] = Interpolation.Mix5To3(patch[4], patch[8]);
                                break;
                            }
                        case 7:
                        case 39:
                        case 135:
                            {
                                dest[destIndex] = Interpolation.Mix5To3(patch[4], patch[3]);
                                dest[destIndex + 1] = Interpolation.Mix7To1(patch[4], patch[3]);
                                dest[destIndex + 2] = Interpolation.Mix7To1(patch[4], patch[5]);
                                dest[destIndex + 3] = Interpolation.Mix5To3(patch[4], patch[5]);
                                dest[destIndex + dpL] = Interpolation.Mix5To3(patch[4], patch[3]);
                                dest[destIndex + dpL + 1] = Interpolation.Mix7To1(patch[4], patch[3]);
                                dest[destIndex + dpL + 2] = Interpolation.Mix7To1(patch[4], patch[5]);
                                dest[destIndex + dpL + 3] = Interpolation.Mix5To3(patch[4], patch[5]);
                                dest[destIndex + dpL + dpL] = Interpolation.Mix5To2To1(patch[4], patch[3], patch[7]);
                                dest[destIndex + dpL + dpL + 1] = Interpolation.Mix6To1To1(patch[4], patch[3], patch[7]);
                                dest[destIndex + dpL + dpL + 2] = Interpolation.Mix6To1To1(patch[4], patch[5], patch[7]);
                                dest[destIndex + dpL + dpL + 3] = Interpolation.Mix5To2To1(patch[4], patch[5], patch[7]);
                                dest[destIndex + dpL + dpL + dpL] = Interpolation.Mix2To1To1(patch[4], patch[7], patch[3]);
                                dest[destIndex + dpL + dpL + dpL + 1] = Interpolation.Mix5To2To1(patch[4], patch[7], patch[3]);
                                dest[destIndex + dpL + dpL + dpL + 2] = Interpolation.Mix5To2To1(patch[4], patch[7], patch[5]);
                                dest[destIndex + dpL + dpL + dpL + 3] = Interpolation.Mix2To1To1(patch[4], patch[7], patch[5]);
                                break;
                            }
                        case 148:
                        case 149:
                        case 180:
                            {
                                dest[destIndex] = Interpolation.Mix2To1To1(patch[4], patch[1], patch[3]);
                                dest[destIndex + 1] = Interpolation.Mix5To2To1(patch[4], patch[1], patch[3]);
                                dest[destIndex + 2] = Interpolation.Mix5To3(patch[4], patch[1]);
                                dest[destIndex + 3] = Interpolation.Mix5To3(patch[4], patch[1]);
                                dest[destIndex + dpL] = Interpolation.Mix5To2To1(patch[4], patch[3], patch[1]);
                                dest[destIndex + dpL + 1] = Interpolation.Mix6To1To1(patch[4], patch[3], patch[1]);
                                dest[destIndex + dpL + 2] = Interpolation.Mix7To1(patch[4], patch[1]);
                                dest[destIndex + dpL + 3] = Interpolation.Mix7To1(patch[4], patch[1]);
                                dest[destIndex + dpL + dpL] = Interpolation.Mix5To2To1(patch[4], patch[3], patch[7]);
                                dest[destIndex + dpL + dpL + 1] = Interpolation.Mix6To1To1(patch[4], patch[3], patch[7]);
                                dest[destIndex + dpL + dpL + 2] = Interpolation.Mix7To1(patch[4], patch[7]);
                                dest[destIndex + dpL + dpL + 3] = Interpolation.Mix7To1(patch[4], patch[7]);
                                dest[destIndex + dpL + dpL + dpL] = Interpolation.Mix2To1To1(patch[4], patch[7], patch[3]);
                                dest[destIndex + dpL + dpL + dpL + 1] = Interpolation.Mix5To2To1(patch[4], patch[7], patch[3]);
                                dest[destIndex + dpL + dpL + dpL + 2] = Interpolation.Mix5To3(patch[4], patch[7]);
                                dest[destIndex + dpL + dpL + dpL + 3] = Interpolation.Mix5To3(patch[4], patch[7]);
                                break;
                            }
                        case 224:
                        case 228:
                        case 225:
                            {
                                dest[destIndex] = Interpolation.Mix2To1To1(patch[4], patch[1], patch[3]);
                                dest[destIndex + 1] = Interpolation.Mix5To2To1(patch[4], patch[1], patch[3]);
                                dest[destIndex + 2] = Interpolation.Mix5To2To1(patch[4], patch[1], patch[5]);
                                dest[destIndex + 3] = Interpolation.Mix2To1To1(patch[4], patch[1], patch[5]);
                                dest[destIndex + dpL] = Interpolation.Mix5To2To1(patch[4], patch[3], patch[1]);
                                dest[destIndex + dpL + 1] = Interpolation.Mix6To1To1(patch[4], patch[3], patch[1]);
                                dest[destIndex + dpL + 2] = Interpolation.Mix6To1To1(patch[4], patch[5], patch[1]);
                                dest[destIndex + dpL + 3] = Interpolation.Mix5To2To1(patch[4], patch[5], patch[1]);
                                dest[destIndex + dpL + dpL] = Interpolation.Mix5To3(patch[4], patch[3]);
                                dest[destIndex + dpL + dpL + 1] = Interpolation.Mix7To1(patch[4], patch[3]);
                                dest[destIndex + dpL + dpL + 2] = Interpolation.Mix7To1(patch[4], patch[5]);
                                dest[destIndex + dpL + dpL + 3] = Interpolation.Mix5To3(patch[4], patch[5]);
                                dest[destIndex + dpL + dpL + dpL] = Interpolation.Mix5To3(patch[4], patch[3]);
                                dest[destIndex + dpL + dpL + dpL + 1] = Interpolation.Mix7To1(patch[4], patch[3]);
                                dest[destIndex + dpL + dpL + dpL + 2] = Interpolation.Mix7To1(patch[4], patch[5]);
                                dest[destIndex + dpL + dpL + dpL + 3] = Interpolation.Mix5To3(patch[4], patch[5]);
                                break;
                            }
                        case 41:
                        case 169:
                        case 45:
                            {
                                dest[destIndex] = Interpolation.Mix5To3(patch[4], patch[1]);
                                dest[destIndex + 1] = Interpolation.Mix5To3(patch[4], patch[1]);
                                dest[destIndex + 2] = Interpolation.Mix5To2To1(patch[4], patch[1], patch[5]);
                                dest[destIndex + 3] = Interpolation.Mix2To1To1(patch[4], patch[1], patch[5]);
                                dest[destIndex + dpL] = Interpolation.Mix7To1(patch[4], patch[1]);
                                dest[destIndex + dpL + 1] = Interpolation.Mix7To1(patch[4], patch[1]);
                                dest[destIndex + dpL + 2] = Interpolation.Mix6To1To1(patch[4], patch[5], patch[1]);
                                dest[destIndex + dpL + 3] = Interpolation.Mix5To2To1(patch[4], patch[5], patch[1]);
                                dest[destIndex + dpL + dpL] = Interpolation.Mix7To1(patch[4], patch[7]);
                                dest[destIndex + dpL + dpL + 1] = Interpolation.Mix7To1(patch[4], patch[7]);
                                dest[destIndex + dpL + dpL + 2] = Interpolation.Mix6To1To1(patch[4], patch[5], patch[7]);
                                dest[destIndex + dpL + dpL + 3] = Interpolation.Mix5To2To1(patch[4], patch[5], patch[7]);
                                dest[destIndex + dpL + dpL + dpL] = Interpolation.Mix5To3(patch[4], patch[7]);
                                dest[destIndex + dpL + dpL + dpL + 1] = Interpolation.Mix5To3(patch[4], patch[7]);
                                dest[destIndex + dpL + dpL + dpL + 2] = Interpolation.Mix5To2To1(patch[4], patch[7], patch[5]);
                                dest[destIndex + dpL + dpL + dpL + 3] = Interpolation.Mix2To1To1(patch[4], patch[7], patch[5]);
                                break;
                            }
                        case 22:
                        case 54:
                            {
                                dest[destIndex] = Interpolation.Mix5To3(patch[4], patch[0]);
                                dest[destIndex + 1] = Interpolation.Mix3To1(patch[4], patch[0]);
                                if (Diff(patch[1], patch[5], trY, trU, trV, trA))
                                {
                                    dest[destIndex + 2] = patch[4];
                                    dest[destIndex + 3] = patch[4];
                                    dest[destIndex + dpL + 3] = patch[4];
                                }
                                else
                                {
                                    dest[destIndex + 2] = Interpolation.MixEven(patch[1], patch[4]);
                                    dest[destIndex + 3] = Interpolation.MixEven(patch[1], patch[5]);
                                    dest[destIndex + dpL + 3] = Interpolation.MixEven(patch[5], patch[4]);
                                }
                                dest[destIndex + dpL] = Interpolation.Mix5To2To1(patch[4], patch[3], patch[0]);
                                dest[destIndex + dpL + 1] = Interpolation.Mix7To1(patch[4], patch[0]);
                                dest[destIndex + dpL + 2] = patch[4];
                                dest[destIndex + dpL + dpL] = Interpolation.Mix5To2To1(patch[4], patch[3], patch[7]);
                                dest[destIndex + dpL + dpL + 1] = Interpolation.Mix6To1To1(patch[4], patch[3], patch[7]);
                                dest[destIndex + dpL + dpL + 2] = Interpolation.Mix7To1(patch[4], patch[8]);
                                dest[destIndex + dpL + dpL + 3] = Interpolation.Mix3To1(patch[4], patch[8]);
                                dest[destIndex + dpL + dpL + dpL] = Interpolation.Mix2To1To1(patch[4], patch[7], patch[3]);
                                dest[destIndex + dpL + dpL + dpL + 1] = Interpolation.Mix5To2To1(patch[4], patch[7], patch[3]);
                                dest[destIndex + dpL + dpL + dpL + 2] = Interpolation.Mix5To2To1(patch[4], patch[7], patch[8]);
                                dest[destIndex + dpL + dpL + dpL + 3] = Interpolation.Mix5To3(patch[4], patch[8]);
                                break;
                            }
                        case 208:
                        case 209:
                            {
                                dest[destIndex] = Interpolation.Mix2To1To1(patch[4], patch[1], patch[3]);
                                dest[destIndex + 1] = Interpolation.Mix5To2To1(patch[4], patch[1], patch[3]);
                                dest[destIndex + 2] = Interpolation.Mix5To2To1(patch[4], patch[1], patch[2]);
                                dest[destIndex + 3] = Interpolation.Mix5To3(patch[4], patch[2]);
                                dest[destIndex + dpL] = Interpolation.Mix5To2To1(patch[4], patch[3], patch[1]);
                                dest[destIndex + dpL + 1] = Interpolation.Mix6To1To1(patch[4], patch[3], patch[1]);
                                dest[destIndex + dpL + 2] = Interpolation.Mix7To1(patch[4], patch[2]);
                                dest[destIndex + dpL + 3] = Interpolation.Mix3To1(patch[4], patch[2]);
                                dest[destIndex + dpL + dpL] = Interpolation.Mix5To2To1(patch[4], patch[3], patch[6]);
                                dest[destIndex + dpL + dpL + 1] = Interpolation.Mix7To1(patch[4], patch[6]);
                                dest[destIndex + dpL + dpL + 2] = patch[4];
                                if (Diff(patch[5], patch[7], trY, trU, trV, trA))
                                {
                                    dest[destIndex + dpL + dpL + 3] = patch[4];
                                    dest[destIndex + dpL + dpL + dpL + 2] = patch[4];
                                    dest[destIndex + dpL + dpL + dpL + 3] = patch[4];
                                }
                                else
                                {
                                    dest[destIndex + dpL + dpL + 3] = Interpolation.MixEven(patch[5], patch[4]);
                                    dest[destIndex + dpL + dpL + dpL + 2] = Interpolation.MixEven(patch[7], patch[4]);
                                    dest[destIndex + dpL + dpL + dpL + 3] = Interpolation.MixEven(patch[7], patch[5]);
                                }
                                dest[destIndex + dpL + dpL + dpL] = Interpolation.Mix5To3(patch[4], patch[6]);
                                dest[destIndex + dpL + dpL + dpL + 1] = Interpolation.Mix3To1(patch[4], patch[6]);
                                break;
                            }
                        case 104:
                        case 108:
                            {
                                dest[destIndex] = Interpolation.Mix5To3(patch[4], patch[0]);
                                dest[destIndex + 1] = Interpolation.Mix5To2To1(patch[4], patch[1], patch[0]);
                                dest[destIndex + 2] = Interpolation.Mix5To2To1(patch[4], patch[1], patch[5]);
                                dest[destIndex + 3] = Interpolation.Mix2To1To1(patch[4], patch[1], patch[5]);
                                dest[destIndex + dpL] = Interpolation.Mix3To1(patch[4], patch[0]);
                                dest[destIndex + dpL + 1] = Interpolation.Mix7To1(patch[4], patch[0]);
                                dest[destIndex + dpL + 2] = Interpolation.Mix6To1To1(patch[4], patch[5], patch[1]);
                                dest[destIndex + dpL + 3] = Interpolation.Mix5To2To1(patch[4], patch[5], patch[1]);
                                if (Diff(patch[7], patch[3], trY, trU, trV, trA))
                                {
                                    dest[destIndex + dpL + dpL] = patch[4];
                                    dest[destIndex + dpL + dpL + dpL] = patch[4];
                                    dest[destIndex + dpL + dpL + dpL + 1] = patch[4];
                                }
                                else
                                {
                                    dest[destIndex + dpL + dpL] = Interpolation.MixEven(patch[3], patch[4]);
                                    dest[destIndex + dpL + dpL + dpL] = Interpolation.MixEven(patch[7], patch[3]);
                                    dest[destIndex + dpL + dpL + dpL + 1] = Interpolation.MixEven(patch[7], patch[4]);
                                }
                                dest[destIndex + dpL + dpL + 1] = patch[4];
                                dest[destIndex + dpL + dpL + 2] = Interpolation.Mix7To1(patch[4], patch[8]);
                                dest[destIndex + dpL + dpL + 3] = Interpolation.Mix5To2To1(patch[4], patch[5], patch[8]);
                                dest[destIndex + dpL + dpL + dpL + 2] = Interpolation.Mix3To1(patch[4], patch[8]);
                                dest[destIndex + dpL + dpL + dpL + 3] = Interpolation.Mix5To3(patch[4], patch[8]);
                                break;
                            }
                        case 11:
                        case 139:
                            {
                                if (Diff(patch[3], patch[1], trY, trU, trV, trA))
                                {
                                    dest[destIndex] = patch[4];
                                    dest[destIndex + 1] = patch[4];
                                    dest[destIndex + dpL] = patch[4];
                                }
                                else
                                {
                                    dest[destIndex] = Interpolation.MixEven(patch[1], patch[3]);
                                    dest[destIndex + 1] = Interpolation.MixEven(patch[1], patch[4]);
                                    dest[destIndex + dpL] = Interpolation.MixEven(patch[3], patch[4]);
                                }
                                dest[destIndex + 2] = Interpolation.Mix3To1(patch[4], patch[2]);
                                dest[destIndex + 3] = Interpolation.Mix5To3(patch[4], patch[2]);
                                dest[destIndex + dpL + 1] = patch[4];
                                dest[destIndex + dpL + 2] = Interpolation.Mix7To1(patch[4], patch[2]);
                                dest[destIndex + dpL + 3] = Interpolation.Mix5To2To1(patch[4], patch[5], patch[2]);
                                dest[destIndex + dpL + dpL] = Interpolation.Mix3To1(patch[4], patch[6]);
                                dest[destIndex + dpL + dpL + 1] = Interpolation.Mix7To1(patch[4], patch[6]);
                                dest[destIndex + dpL + dpL + 2] = Interpolation.Mix6To1To1(patch[4], patch[5], patch[7]);
                                dest[destIndex + dpL + dpL + 3] = Interpolation.Mix5To2To1(patch[4], patch[5], patch[7]);
                                dest[destIndex + dpL + dpL + dpL] = Interpolation.Mix5To3(patch[4], patch[6]);
                                dest[destIndex + dpL + dpL + dpL + 1] = Interpolation.Mix5To2To1(patch[4], patch[7], patch[6]);
                                dest[destIndex + dpL + dpL + dpL + 2] = Interpolation.Mix5To2To1(patch[4], patch[7], patch[5]);
                                dest[destIndex + dpL + dpL + dpL + 3] = Interpolation.Mix2To1To1(patch[4], patch[7], patch[5]);
                                break;
                            }
                        case 19:
                        case 51:
                            {
                                if (Diff(patch[1], patch[5], trY, trU, trV, trA))
                                {
                                    dest[destIndex] = Interpolation.Mix5To3(patch[4], patch[3]);
                                    dest[destIndex + 1] = Interpolation.Mix7To1(patch[4], patch[3]);
                                    dest[destIndex + 2] = Interpolation.Mix3To1(patch[4], patch[2]);
                                    dest[destIndex + 3] = Interpolation.Mix5To3(patch[4], patch[2]);
                                    dest[destIndex + dpL + 2] = Interpolation.Mix7To1(patch[4], patch[2]);
                                    dest[destIndex + dpL + 3] = Interpolation.Mix3To1(patch[4], patch[2]);
                                }
                                else
                                {
                                    dest[destIndex] = Interpolation.Mix3To1(patch[4], patch[1]);
                                    dest[destIndex + 1] = Interpolation.Mix3To1(patch[1], patch[4]);
                                    dest[destIndex + 2] = Interpolation.Mix5To3(patch[1], patch[5]);
                                    dest[destIndex + 3] = Interpolation.MixEven(patch[1], patch[5]);
                                    dest[destIndex + dpL + 2] = Interpolation.Mix6To1To1(patch[4], patch[5], patch[1]);
                                    dest[destIndex + dpL + 3] = Interpolation.Mix2To1To1(patch[5], patch[4], patch[1]);
                                }
                                dest[destIndex + dpL] = Interpolation.Mix5To3(patch[4], patch[3]);
                                dest[destIndex + dpL + 1] = Interpolation.Mix7To1(patch[4], patch[3]);
                                dest[destIndex + dpL + dpL] = Interpolation.Mix5To2To1(patch[4], patch[3], patch[7]);
                                dest[destIndex + dpL + dpL + 1] = Interpolation.Mix6To1To1(patch[4], patch[3], patch[7]);
                                dest[destIndex + dpL + dpL + 2] = Interpolation.Mix7To1(patch[4], patch[8]);
                                dest[destIndex + dpL + dpL + 3] = Interpolation.Mix3To1(patch[4], patch[8]);
                                dest[destIndex + dpL + dpL + dpL] = Interpolation.Mix2To1To1(patch[4], patch[7], patch[3]);
                                dest[destIndex + dpL + dpL + dpL + 1] = Interpolation.Mix5To2To1(patch[4], patch[7], patch[3]);
                                dest[destIndex + dpL + dpL + dpL + 2] = Interpolation.Mix5To2To1(patch[4], patch[7], patch[8]);
                                dest[destIndex + dpL + dpL + dpL + 3] = Interpolation.Mix5To3(patch[4], patch[8]);
                                break;
                            }
                        case 146:
                        case 178:
                            {
                                dest[destIndex] = Interpolation.Mix5To3(patch[4], patch[0]);
                                dest[destIndex + 1] = Interpolation.Mix3To1(patch[4], patch[0]);
                                if (Diff(patch[1], patch[5], trY, trU, trV, trA))
                                {
                                    dest[destIndex + 2] = Interpolation.Mix3To1(patch[4], patch[2]);
                                    dest[destIndex + 3] = Interpolation.Mix5To3(patch[4], patch[2]);
                                    dest[destIndex + dpL + 2] = Interpolation.Mix7To1(patch[4], patch[2]);
                                    dest[destIndex + dpL + 3] = Interpolation.Mix3To1(patch[4], patch[2]);
                                    dest[destIndex + dpL + dpL + 3] = Interpolation.Mix7To1(patch[4], patch[7]);
                                    dest[destIndex + dpL + dpL + dpL + 3] = Interpolation.Mix5To3(patch[4], patch[7]);
                                }
                                else
                                {
                                    dest[destIndex + 2] = Interpolation.Mix2To1To1(patch[1], patch[4], patch[5]);
                                    dest[destIndex + 3] = Interpolation.MixEven(patch[1], patch[5]);
                                    dest[destIndex + dpL + 2] = Interpolation.Mix6To1To1(patch[4], patch[5], patch[1]);
                                    dest[destIndex + dpL + 3] = Interpolation.Mix5To3(patch[5], patch[1]);
                                    dest[destIndex + dpL + dpL + 3] = Interpolation.Mix3To1(patch[5], patch[4]);
                                    dest[destIndex + dpL + dpL + dpL + 3] = Interpolation.Mix3To1(patch[4], patch[5]);
                                }
                                dest[destIndex + dpL] = Interpolation.Mix5To2To1(patch[4], patch[3], patch[0]);
                                dest[destIndex + dpL + 1] = Interpolation.Mix7To1(patch[4], patch[0]);
                                dest[destIndex + dpL + dpL] = Interpolation.Mix5To2To1(patch[4], patch[3], patch[7]);
                                dest[destIndex + dpL + dpL + 1] = Interpolation.Mix6To1To1(patch[4], patch[3], patch[7]);
                                dest[destIndex + dpL + dpL + 2] = Interpolation.Mix7To1(patch[4], patch[7]);
                                dest[destIndex + dpL + dpL + dpL] = Interpolation.Mix2To1To1(patch[4], patch[7], patch[3]);
                                dest[destIndex + dpL + dpL + dpL + 1] = Interpolation.Mix5To2To1(patch[4], patch[7], patch[3]);
                                dest[destIndex + dpL + dpL + dpL + 2] = Interpolation.Mix5To3(patch[4], patch[7]);
                                break;
                            }
                        case 84:
                        case 85:
                            {
                                dest[destIndex] = Interpolation.Mix2To1To1(patch[4], patch[1], patch[3]);
                                dest[destIndex + 1] = Interpolation.Mix5To2To1(patch[4], patch[1], patch[3]);
                                dest[destIndex + 2] = Interpolation.Mix5To3(patch[4], patch[1]);
                                if (Diff(patch[5], patch[7], trY, trU, trV, trA))
                                {
                                    dest[destIndex + 3] = Interpolation.Mix5To3(patch[4], patch[1]);
                                    dest[destIndex + dpL + 3] = Interpolation.Mix7To1(patch[4], patch[1]);
                                    dest[destIndex + dpL + dpL + 2] = Interpolation.Mix7To1(patch[4], patch[8]);
                                    dest[destIndex + dpL + dpL + 3] = Interpolation.Mix3To1(patch[4], patch[8]);
                                    dest[destIndex + dpL + dpL + dpL + 2] = Interpolation.Mix3To1(patch[4], patch[8]);
                                    dest[destIndex + dpL + dpL + dpL + 3] = Interpolation.Mix5To3(patch[4], patch[8]);
                                }
                                else
                                {
                                    dest[destIndex + 3] = Interpolation.Mix3To1(patch[4], patch[5]);
                                    dest[destIndex + dpL + 3] = Interpolation.Mix3To1(patch[5], patch[4]);
                                    dest[destIndex + dpL + dpL + 2] = Interpolation.Mix6To1To1(patch[4], patch[5], patch[7]);
                                    dest[destIndex + dpL + dpL + 3] = Interpolation.Mix5To3(patch[5], patch[7]);
                                    dest[destIndex + dpL + dpL + dpL + 2] = Interpolation.Mix2To1To1(patch[7], patch[4], patch[5]);
                                    dest[destIndex + dpL + dpL + dpL + 3] = Interpolation.MixEven(patch[7], patch[5]);
                                }
                                dest[destIndex + dpL] = Interpolation.Mix5To2To1(patch[4], patch[3], patch[1]);
                                dest[destIndex + dpL + 1] = Interpolation.Mix6To1To1(patch[4], patch[3], patch[1]);
                                dest[destIndex + dpL + 2] = Interpolation.Mix7To1(patch[4], patch[1]);
                                dest[destIndex + dpL + dpL] = Interpolation.Mix5To2To1(patch[4], patch[3], patch[6]);
                                dest[destIndex + dpL + dpL + 1] = Interpolation.Mix7To1(patch[4], patch[6]);
                                dest[destIndex + dpL + dpL + dpL] = Interpolation.Mix5To3(patch[4], patch[6]);
                                dest[destIndex + dpL + dpL + dpL + 1] = Interpolation.Mix3To1(patch[4], patch[6]);
                                break;
                            }
                        case 112:
                        case 113:
                            {
                                dest[destIndex] = Interpolation.Mix2To1To1(patch[4], patch[1], patch[3]);
                                dest[destIndex + 1] = Interpolation.Mix5To2To1(patch[4], patch[1], patch[3]);
                                dest[destIndex + 2] = Interpolation.Mix5To2To1(patch[4], patch[1], patch[2]);
                                dest[destIndex + 3] = Interpolation.Mix5To3(patch[4], patch[2]);
                                dest[destIndex + dpL] = Interpolation.Mix5To2To1(patch[4], patch[3], patch[1]);
                                dest[destIndex + dpL + 1] = Interpolation.Mix6To1To1(patch[4], patch[3], patch[1]);
                                dest[destIndex + dpL + 2] = Interpolation.Mix7To1(patch[4], patch[2]);
                                dest[destIndex + dpL + 3] = Interpolation.Mix3To1(patch[4], patch[2]);
                                dest[destIndex + dpL + dpL] = Interpolation.Mix5To3(patch[4], patch[3]);
                                dest[destIndex + dpL + dpL + 1] = Interpolation.Mix7To1(patch[4], patch[3]);
                                if (Diff(patch[5], patch[7], trY, trU, trV, trA))
                                {
                                    dest[destIndex + dpL + dpL + 2] = Interpolation.Mix7To1(patch[4], patch[8]);
                                    dest[destIndex + dpL + dpL + 3] = Interpolation.Mix3To1(patch[4], patch[8]);
                                    dest[destIndex + dpL + dpL + dpL] = Interpolation.Mix5To3(patch[4], patch[3]);
                                    dest[destIndex + dpL + dpL + dpL + 1] = Interpolation.Mix7To1(patch[4], patch[3]);
                                    dest[destIndex + dpL + dpL + dpL + 2] = Interpolation.Mix3To1(patch[4], patch[8]);
                                    dest[destIndex + dpL + dpL + dpL + 3] = Interpolation.Mix5To3(patch[4], patch[8]);
                                }
                                else
                                {
                                    dest[destIndex + dpL + dpL + 2] = Interpolation.Mix6To1To1(patch[4], patch[5], patch[7]);
                                    dest[destIndex + dpL + dpL + 3] = Interpolation.Mix2To1To1(patch[5], patch[4], patch[7]);
                                    dest[destIndex + dpL + dpL + dpL] = Interpolation.Mix3To1(patch[4], patch[7]);
                                    dest[destIndex + dpL + dpL + dpL + 1] = Interpolation.Mix3To1(patch[7], patch[4]);
                                    dest[destIndex + dpL + dpL + dpL + 2] = Interpolation.Mix5To3(patch[7], patch[5]);
                                    dest[destIndex + dpL + dpL + dpL + 3] = Interpolation.MixEven(patch[7], patch[5]);
                                }
                                break;
                            }
                        case 200:
                        case 204:
                            {
                                dest[destIndex] = Interpolation.Mix5To3(patch[4], patch[0]);
                                dest[destIndex + 1] = Interpolation.Mix5To2To1(patch[4], patch[1], patch[0]);
                                dest[destIndex + 2] = Interpolation.Mix5To2To1(patch[4], patch[1], patch[5]);
                                dest[destIndex + 3] = Interpolation.Mix2To1To1(patch[4], patch[1], patch[5]);
                                dest[destIndex + dpL] = Interpolation.Mix3To1(patch[4], patch[0]);
                                dest[destIndex + dpL + 1] = Interpolation.Mix7To1(patch[4], patch[0]);
                                dest[destIndex + dpL + 2] = Interpolation.Mix6To1To1(patch[4], patch[5], patch[1]);
                                dest[destIndex + dpL + 3] = Interpolation.Mix5To2To1(patch[4], patch[5], patch[1]);
                                if (Diff(patch[7], patch[3], trY, trU, trV, trA))
                                {
                                    dest[destIndex + dpL + dpL] = Interpolation.Mix3To1(patch[4], patch[6]);
                                    dest[destIndex + dpL + dpL + 1] = Interpolation.Mix7To1(patch[4], patch[6]);
                                    dest[destIndex + dpL + dpL + dpL] = Interpolation.Mix5To3(patch[4], patch[6]);
                                    dest[destIndex + dpL + dpL + dpL + 1] = Interpolation.Mix3To1(patch[4], patch[6]);
                                    dest[destIndex + dpL + dpL + dpL + 2] = Interpolation.Mix7To1(patch[4], patch[5]);
                                    dest[destIndex + dpL + dpL + dpL + 3] = Interpolation.Mix5To3(patch[4], patch[5]);
                                }
                                else
                                {
                                    dest[destIndex + dpL + dpL] = Interpolation.Mix2To1To1(patch[3], patch[4], patch[7]);
                                    dest[destIndex + dpL + dpL + 1] = Interpolation.Mix6To1To1(patch[4], patch[3], patch[7]);
                                    dest[destIndex + dpL + dpL + dpL] = Interpolation.MixEven(patch[7], patch[3]);
                                    dest[destIndex + dpL + dpL + dpL + 1] = Interpolation.Mix5To3(patch[7], patch[3]);
                                    dest[destIndex + dpL + dpL + dpL + 2] = Interpolation.Mix3To1(patch[7], patch[4]);
                                    dest[destIndex + dpL + dpL + dpL + 3] = Interpolation.Mix3To1(patch[4], patch[7]);
                                }
                                dest[destIndex + dpL + dpL + 2] = Interpolation.Mix7To1(patch[4], patch[5]);
                                dest[destIndex + dpL + dpL + 3] = Interpolation.Mix5To3(patch[4], patch[5]);
                                break;
                            }
                        case 73:
                        case 77:
                            {
                                if (Diff(patch[7], patch[3], trY, trU, trV, trA))
                                {
                                    dest[destIndex] = Interpolation.Mix5To3(patch[4], patch[1]);
                                    dest[destIndex + dpL] = Interpolation.Mix7To1(patch[4], patch[1]);
                                    dest[destIndex + dpL + dpL] = Interpolation.Mix3To1(patch[4], patch[6]);
                                    dest[destIndex + dpL + dpL + 1] = Interpolation.Mix7To1(patch[4], patch[6]);
                                    dest[destIndex + dpL + dpL + dpL] = Interpolation.Mix5To3(patch[4], patch[6]);
                                    dest[destIndex + dpL + dpL + dpL + 1] = Interpolation.Mix3To1(patch[4], patch[6]);
                                }
                                else
                                {
                                    dest[destIndex] = Interpolation.Mix3To1(patch[4], patch[3]);
                                    dest[destIndex + dpL] = Interpolation.Mix3To1(patch[3], patch[4]);
                                    dest[destIndex + dpL + dpL] = Interpolation.Mix5To3(patch[3], patch[7]);
                                    dest[destIndex + dpL + dpL + 1] = Interpolation.Mix6To1To1(patch[4], patch[3], patch[7]);
                                    dest[destIndex + dpL + dpL + dpL] = Interpolation.MixEven(patch[7], patch[3]);
                                    dest[destIndex + dpL + dpL + dpL + 1] = Interpolation.Mix2To1To1(patch[7], patch[4], patch[3]);
                                }
                                dest[destIndex + 1] = Interpolation.Mix5To3(patch[4], patch[1]);
                                dest[destIndex + 2] = Interpolation.Mix5To2To1(patch[4], patch[1], patch[5]);
                                dest[destIndex + 3] = Interpolation.Mix2To1To1(patch[4], patch[1], patch[5]);
                                dest[destIndex + dpL + 1] = Interpolation.Mix7To1(patch[4], patch[1]);
                                dest[destIndex + dpL + 2] = Interpolation.Mix6To1To1(patch[4], patch[5], patch[1]);
                                dest[destIndex + dpL + 3] = Interpolation.Mix5To2To1(patch[4], patch[5], patch[1]);
                                dest[destIndex + dpL + dpL + 2] = Interpolation.Mix7To1(patch[4], patch[8]);
                                dest[destIndex + dpL + dpL + 3] = Interpolation.Mix5To2To1(patch[4], patch[5], patch[8]);
                                dest[destIndex + dpL + dpL + dpL + 2] = Interpolation.Mix3To1(patch[4], patch[8]);
                                dest[destIndex + dpL + dpL + dpL + 3] = Interpolation.Mix5To3(patch[4], patch[8]);
                                break;
                            }
                        case 42:
                        case 170:
                            {
                                if (Diff(patch[3], patch[1], trY, trU, trV, trA))
                                {
                                    dest[destIndex] = Interpolation.Mix5To3(patch[4], patch[0]);
                                    dest[destIndex + 1] = Interpolation.Mix3To1(patch[4], patch[0]);
                                    dest[destIndex + dpL] = Interpolation.Mix3To1(patch[4], patch[0]);
                                    dest[destIndex + dpL + 1] = Interpolation.Mix7To1(patch[4], patch[0]);
                                    dest[destIndex + dpL + dpL] = Interpolation.Mix7To1(patch[4], patch[7]);
                                    dest[destIndex + dpL + dpL + dpL] = Interpolation.Mix5To3(patch[4], patch[7]);
                                }
                                else
                                {
                                    dest[destIndex] = Interpolation.MixEven(patch[1], patch[3]);
                                    dest[destIndex + 1] = Interpolation.Mix2To1To1(patch[1], patch[4], patch[3]);
                                    dest[destIndex + dpL] = Interpolation.Mix5To3(patch[3], patch[1]);
                                    dest[destIndex + dpL + 1] = Interpolation.Mix6To1To1(patch[4], patch[3], patch[1]);
                                    dest[destIndex + dpL + dpL] = Interpolation.Mix3To1(patch[3], patch[4]);
                                    dest[destIndex + dpL + dpL + dpL] = Interpolation.Mix3To1(patch[4], patch[3]);
                                }
                                dest[destIndex + 2] = Interpolation.Mix3To1(patch[4], patch[2]);
                                dest[destIndex + 3] = Interpolation.Mix5To3(patch[4], patch[2]);
                                dest[destIndex + dpL + 2] = Interpolation.Mix7To1(patch[4], patch[2]);
                                dest[destIndex + dpL + 3] = Interpolation.Mix5To2To1(patch[4], patch[5], patch[2]);
                                dest[destIndex + dpL + dpL + 1] = Interpolation.Mix7To1(patch[4], patch[7]);
                                dest[destIndex + dpL + dpL + 2] = Interpolation.Mix6To1To1(patch[4], patch[5], patch[7]);
                                dest[destIndex + dpL + dpL + 3] = Interpolation.Mix5To2To1(patch[4], patch[5], patch[7]);
                                dest[destIndex + dpL + dpL + dpL + 1] = Interpolation.Mix5To3(patch[4], patch[7]);
                                dest[destIndex + dpL + dpL + dpL + 2] = Interpolation.Mix5To2To1(patch[4], patch[7], patch[5]);
                                dest[destIndex + dpL + dpL + dpL + 3] = Interpolation.Mix2To1To1(patch[4], patch[7], patch[5]);
                                break;
                            }
                        case 14:
                        case 142:
                            {
                                if (Diff(patch[3], patch[1], trY, trU, trV, trA))
                                {
                                    dest[destIndex] = Interpolation.Mix5To3(patch[4], patch[0]);
                                    dest[destIndex + 1] = Interpolation.Mix3To1(patch[4], patch[0]);
                                    dest[destIndex + 2] = Interpolation.Mix7To1(patch[4], patch[5]);
                                    dest[destIndex + 3] = Interpolation.Mix5To3(patch[4], patch[5]);
                                    dest[destIndex + dpL] = Interpolation.Mix3To1(patch[4], patch[0]);
                                    dest[destIndex + dpL + 1] = Interpolation.Mix7To1(patch[4], patch[0]);
                                }
                                else
                                {
                                    dest[destIndex] = Interpolation.MixEven(patch[1], patch[3]);
                                    dest[destIndex + 1] = Interpolation.Mix5To3(patch[1], patch[3]);
                                    dest[destIndex + 2] = Interpolation.Mix3To1(patch[1], patch[4]);
                                    dest[destIndex + 3] = Interpolation.Mix3To1(patch[4], patch[1]);
                                    dest[destIndex + dpL] = Interpolation.Mix2To1To1(patch[3], patch[4], patch[1]);
                                    dest[destIndex + dpL + 1] = Interpolation.Mix6To1To1(patch[4], patch[3], patch[1]);
                                }
                                dest[destIndex + dpL + 2] = Interpolation.Mix7To1(patch[4], patch[5]);
                                dest[destIndex + dpL + 3] = Interpolation.Mix5To3(patch[4], patch[5]);
                                dest[destIndex + dpL + dpL] = Interpolation.Mix3To1(patch[4], patch[6]);
                                dest[destIndex + dpL + dpL + 1] = Interpolation.Mix7To1(patch[4], patch[6]);
                                dest[destIndex + dpL + dpL + 2] = Interpolation.Mix6To1To1(patch[4], patch[5], patch[7]);
                                dest[destIndex + dpL + dpL + 3] = Interpolation.Mix5To2To1(patch[4], patch[5], patch[7]);
                                dest[destIndex + dpL + dpL + dpL] = Interpolation.Mix5To3(patch[4], patch[6]);
                                dest[destIndex + dpL + dpL + dpL + 1] = Interpolation.Mix5To2To1(patch[4], patch[7], patch[6]);
                                dest[destIndex + dpL + dpL + dpL + 2] = Interpolation.Mix5To2To1(patch[4], patch[7], patch[5]);
                                dest[destIndex + dpL + dpL + dpL + 3] = Interpolation.Mix2To1To1(patch[4], patch[7], patch[5]);
                                break;
                            }
                        case 67:
                            {
                                dest[destIndex] = Interpolation.Mix5To3(patch[4], patch[3]);
                                dest[destIndex + 1] = Interpolation.Mix7To1(patch[4], patch[3]);
                                dest[destIndex + 2] = Interpolation.Mix3To1(patch[4], patch[2]);
                                dest[destIndex + 3] = Interpolation.Mix5To3(patch[4], patch[2]);
                                dest[destIndex + dpL] = Interpolation.Mix5To3(patch[4], patch[3]);
                                dest[destIndex + dpL + 1] = Interpolation.Mix7To1(patch[4], patch[3]);
                                dest[destIndex + dpL + 2] = Interpolation.Mix7To1(patch[4], patch[2]);
                                dest[destIndex + dpL + 3] = Interpolation.Mix5To2To1(patch[4], patch[5], patch[2]);
                                dest[destIndex + dpL + dpL] = Interpolation.Mix5To2To1(patch[4], patch[3], patch[6]);
                                dest[destIndex + dpL + dpL + 1] = Interpolation.Mix7To1(patch[4], patch[6]);
                                dest[destIndex + dpL + dpL + 2] = Interpolation.Mix7To1(patch[4], patch[8]);
                                dest[destIndex + dpL + dpL + 3] = Interpolation.Mix5To2To1(patch[4], patch[5], patch[8]);
                                dest[destIndex + dpL + dpL + dpL] = Interpolation.Mix5To3(patch[4], patch[6]);
                                dest[destIndex + dpL + dpL + dpL + 1] = Interpolation.Mix3To1(patch[4], patch[6]);
                                dest[destIndex + dpL + dpL + dpL + 2] = Interpolation.Mix3To1(patch[4], patch[8]);
                                dest[destIndex + dpL + dpL + dpL + 3] = Interpolation.Mix5To3(patch[4], patch[8]);
                                break;
                            }
                        case 70:
                            {
                                dest[destIndex] = Interpolation.Mix5To3(patch[4], patch[0]);
                                dest[destIndex + 1] = Interpolation.Mix3To1(patch[4], patch[0]);
                                dest[destIndex + 2] = Interpolation.Mix7To1(patch[4], patch[5]);
                                dest[destIndex + 3] = Interpolation.Mix5To3(patch[4], patch[5]);
                                dest[destIndex + dpL] = Interpolation.Mix5To2To1(patch[4], patch[3], patch[0]);
                                dest[destIndex + dpL + 1] = Interpolation.Mix7To1(patch[4], patch[0]);
                                dest[destIndex + dpL + 2] = Interpolation.Mix7To1(patch[4], patch[5]);
                                dest[destIndex + dpL + 3] = Interpolation.Mix5To3(patch[4], patch[5]);
                                dest[destIndex + dpL + dpL] = Interpolation.Mix5To2To1(patch[4], patch[3], patch[6]);
                                dest[destIndex + dpL + dpL + 1] = Interpolation.Mix7To1(patch[4], patch[6]);
                                dest[destIndex + dpL + dpL + 2] = Interpolation.Mix7To1(patch[4], patch[8]);
                                dest[destIndex + dpL + dpL + 3] = Interpolation.Mix5To2To1(patch[4], patch[5], patch[8]);
                                dest[destIndex + dpL + dpL + dpL] = Interpolation.Mix5To3(patch[4], patch[6]);
                                dest[destIndex + dpL + dpL + dpL + 1] = Interpolation.Mix3To1(patch[4], patch[6]);
                                dest[destIndex + dpL + dpL + dpL + 2] = Interpolation.Mix3To1(patch[4], patch[8]);
                                dest[destIndex + dpL + dpL + dpL + 3] = Interpolation.Mix5To3(patch[4], patch[8]);
                                break;
                            }
                        case 28:
                            {
                                dest[destIndex] = Interpolation.Mix5To3(patch[4], patch[0]);
                                dest[destIndex + 1] = Interpolation.Mix5To2To1(patch[4], patch[1], patch[0]);
                                dest[destIndex + 2] = Interpolation.Mix5To3(patch[4], patch[1]);
                                dest[destIndex + 3] = Interpolation.Mix5To3(patch[4], patch[1]);
                                dest[destIndex + dpL] = Interpolation.Mix3To1(patch[4], patch[0]);
                                dest[destIndex + dpL + 1] = Interpolation.Mix7To1(patch[4], patch[0]);
                                dest[destIndex + dpL + 2] = Interpolation.Mix7To1(patch[4], patch[1]);
                                dest[destIndex + dpL + 3] = Interpolation.Mix7To1(patch[4], patch[1]);
                                dest[destIndex + dpL + dpL] = Interpolation.Mix3To1(patch[4], patch[6]);
                                dest[destIndex + dpL + dpL + 1] = Interpolation.Mix7To1(patch[4], patch[6]);
                                dest[destIndex + dpL + dpL + 2] = Interpolation.Mix7To1(patch[4], patch[8]);
                                dest[destIndex + dpL + dpL + 3] = Interpolation.Mix3To1(patch[4], patch[8]);
                                dest[destIndex + dpL + dpL + dpL] = Interpolation.Mix5To3(patch[4], patch[6]);
                                dest[destIndex + dpL + dpL + dpL + 1] = Interpolation.Mix5To2To1(patch[4], patch[7], patch[6]);
                                dest[destIndex + dpL + dpL + dpL + 2] = Interpolation.Mix5To2To1(patch[4], patch[7], patch[8]);
                                dest[destIndex + dpL + dpL + dpL + 3] = Interpolation.Mix5To3(patch[4], patch[8]);
                                break;
                            }
                        case 152:
                            {
                                dest[destIndex] = Interpolation.Mix5To3(patch[4], patch[0]);
                                dest[destIndex + 1] = Interpolation.Mix5To2To1(patch[4], patch[1], patch[0]);
                                dest[destIndex + 2] = Interpolation.Mix5To2To1(patch[4], patch[1], patch[2]);
                                dest[destIndex + 3] = Interpolation.Mix5To3(patch[4], patch[2]);
                                dest[destIndex + dpL] = Interpolation.Mix3To1(patch[4], patch[0]);
                                dest[destIndex + dpL + 1] = Interpolation.Mix7To1(patch[4], patch[0]);
                                dest[destIndex + dpL + 2] = Interpolation.Mix7To1(patch[4], patch[2]);
                                dest[destIndex + dpL + 3] = Interpolation.Mix3To1(patch[4], patch[2]);
                                dest[destIndex + dpL + dpL] = Interpolation.Mix3To1(patch[4], patch[6]);
                                dest[destIndex + dpL + dpL + 1] = Interpolation.Mix7To1(patch[4], patch[6]);
                                dest[destIndex + dpL + dpL + 2] = Interpolation.Mix7To1(patch[4], patch[7]);
                                dest[destIndex + dpL + dpL + 3] = Interpolation.Mix7To1(patch[4], patch[7]);
                                dest[destIndex + dpL + dpL + dpL] = Interpolation.Mix5To3(patch[4], patch[6]);
                                dest[destIndex + dpL + dpL + dpL + 1] = Interpolation.Mix5To2To1(patch[4], patch[7], patch[6]);
                                dest[destIndex + dpL + dpL + dpL + 2] = Interpolation.Mix5To3(patch[4], patch[7]);
                                dest[destIndex + dpL + dpL + dpL + 3] = Interpolation.Mix5To3(patch[4], patch[7]);
                                break;
                            }
                        case 194:
                            {
                                dest[destIndex] = Interpolation.Mix5To3(patch[4], patch[0]);
                                dest[destIndex + 1] = Interpolation.Mix3To1(patch[4], patch[0]);
                                dest[destIndex + 2] = Interpolation.Mix3To1(patch[4], patch[2]);
                                dest[destIndex + 3] = Interpolation.Mix5To3(patch[4], patch[2]);
                                dest[destIndex + dpL] = Interpolation.Mix5To2To1(patch[4], patch[3], patch[0]);
                                dest[destIndex + dpL + 1] = Interpolation.Mix7To1(patch[4], patch[0]);
                                dest[destIndex + dpL + 2] = Interpolation.Mix7To1(patch[4], patch[2]);
                                dest[destIndex + dpL + 3] = Interpolation.Mix5To2To1(patch[4], patch[5], patch[2]);
                                dest[destIndex + dpL + dpL] = Interpolation.Mix5To2To1(patch[4], patch[3], patch[6]);
                                dest[destIndex + dpL + dpL + 1] = Interpolation.Mix7To1(patch[4], patch[6]);
                                dest[destIndex + dpL + dpL + 2] = Interpolation.Mix7To1(patch[4], patch[5]);
                                dest[destIndex + dpL + dpL + 3] = Interpolation.Mix5To3(patch[4], patch[5]);
                                dest[destIndex + dpL + dpL + dpL] = Interpolation.Mix5To3(patch[4], patch[6]);
                                dest[destIndex + dpL + dpL + dpL + 1] = Interpolation.Mix3To1(patch[4], patch[6]);
                                dest[destIndex + dpL + dpL + dpL + 2] = Interpolation.Mix7To1(patch[4], patch[5]);
                                dest[destIndex + dpL + dpL + dpL + 3] = Interpolation.Mix5To3(patch[4], patch[5]);
                                break;
                            }
                        case 98:
                            {
                                dest[destIndex] = Interpolation.Mix5To3(patch[4], patch[0]);
                                dest[destIndex + 1] = Interpolation.Mix3To1(patch[4], patch[0]);
                                dest[destIndex + 2] = Interpolation.Mix3To1(patch[4], patch[2]);
                                dest[destIndex + 3] = Interpolation.Mix5To3(patch[4], patch[2]);
                                dest[destIndex + dpL] = Interpolation.Mix5To2To1(patch[4], patch[3], patch[0]);
                                dest[destIndex + dpL + 1] = Interpolation.Mix7To1(patch[4], patch[0]);
                                dest[destIndex + dpL + 2] = Interpolation.Mix7To1(patch[4], patch[2]);
                                dest[destIndex + dpL + 3] = Interpolation.Mix5To2To1(patch[4], patch[5], patch[2]);
                                dest[destIndex + dpL + dpL] = Interpolation.Mix5To3(patch[4], patch[3]);
                                dest[destIndex + dpL + dpL + 1] = Interpolation.Mix7To1(patch[4], patch[3]);
                                dest[destIndex + dpL + dpL + 2] = Interpolation.Mix7To1(patch[4], patch[8]);
                                dest[destIndex + dpL + dpL + 3] = Interpolation.Mix5To2To1(patch[4], patch[5], patch[8]);
                                dest[destIndex + dpL + dpL + dpL] = Interpolation.Mix5To3(patch[4], patch[3]);
                                dest[destIndex + dpL + dpL + dpL + 1] = Interpolation.Mix7To1(patch[4], patch[3]);
                                dest[destIndex + dpL + dpL + dpL + 2] = Interpolation.Mix3To1(patch[4], patch[8]);
                                dest[destIndex + dpL + dpL + dpL + 3] = Interpolation.Mix5To3(patch[4], patch[8]);
                                break;
                            }
                        case 56:
                            {
                                dest[destIndex] = Interpolation.Mix5To3(patch[4], patch[0]);
                                dest[destIndex + 1] = Interpolation.Mix5To2To1(patch[4], patch[1], patch[0]);
                                dest[destIndex + 2] = Interpolation.Mix5To2To1(patch[4], patch[1], patch[2]);
                                dest[destIndex + 3] = Interpolation.Mix5To3(patch[4], patch[2]);
                                dest[destIndex + dpL] = Interpolation.Mix3To1(patch[4], patch[0]);
                                dest[destIndex + dpL + 1] = Interpolation.Mix7To1(patch[4], patch[0]);
                                dest[destIndex + dpL + 2] = Interpolation.Mix7To1(patch[4], patch[2]);
                                dest[destIndex + dpL + 3] = Interpolation.Mix3To1(patch[4], patch[2]);
                                dest[destIndex + dpL + dpL] = Interpolation.Mix7To1(patch[4], patch[7]);
                                dest[destIndex + dpL + dpL + 1] = Interpolation.Mix7To1(patch[4], patch[7]);
                                dest[destIndex + dpL + dpL + 2] = Interpolation.Mix7To1(patch[4], patch[8]);
                                dest[destIndex + dpL + dpL + 3] = Interpolation.Mix3To1(patch[4], patch[8]);
                                dest[destIndex + dpL + dpL + dpL] = Interpolation.Mix5To3(patch[4], patch[7]);
                                dest[destIndex + dpL + dpL + dpL + 1] = Interpolation.Mix5To3(patch[4], patch[7]);
                                dest[destIndex + dpL + dpL + dpL + 2] = Interpolation.Mix5To2To1(patch[4], patch[7], patch[8]);
                                dest[destIndex + dpL + dpL + dpL + 3] = Interpolation.Mix5To3(patch[4], patch[8]);
                                break;
                            }
                        case 25:
                            {
                                dest[destIndex] = Interpolation.Mix5To3(patch[4], patch[1]);
                                dest[destIndex + 1] = Interpolation.Mix5To3(patch[4], patch[1]);
                                dest[destIndex + 2] = Interpolation.Mix5To2To1(patch[4], patch[1], patch[2]);
                                dest[destIndex + 3] = Interpolation.Mix5To3(patch[4], patch[2]);
                                dest[destIndex + dpL] = Interpolation.Mix7To1(patch[4], patch[1]);
                                dest[destIndex + dpL + 1] = Interpolation.Mix7To1(patch[4], patch[1]);
                                dest[destIndex + dpL + 2] = Interpolation.Mix7To1(patch[4], patch[2]);
                                dest[destIndex + dpL + 3] = Interpolation.Mix3To1(patch[4], patch[2]);
                                dest[destIndex + dpL + dpL] = Interpolation.Mix3To1(patch[4], patch[6]);
                                dest[destIndex + dpL + dpL + 1] = Interpolation.Mix7To1(patch[4], patch[6]);
                                dest[destIndex + dpL + dpL + 2] = Interpolation.Mix7To1(patch[4], patch[8]);
                                dest[destIndex + dpL + dpL + 3] = Interpolation.Mix3To1(patch[4], patch[8]);
                                dest[destIndex + dpL + dpL + dpL] = Interpolation.Mix5To3(patch[4], patch[6]);
                                dest[destIndex + dpL + dpL + dpL + 1] = Interpolation.Mix5To2To1(patch[4], patch[7], patch[6]);
                                dest[destIndex + dpL + dpL + dpL + 2] = Interpolation.Mix5To2To1(patch[4], patch[7], patch[8]);
                                dest[destIndex + dpL + dpL + dpL + 3] = Interpolation.Mix5To3(patch[4], patch[8]);
                                break;
                            }
                        case 26:
                        case 31:
                            {
                                if (Diff(patch[3], patch[1], trY, trU, trV, trA))
                                {
                                    dest[destIndex] = patch[4];
                                    dest[destIndex + 1] = patch[4];
                                    dest[destIndex + dpL] = patch[4];
                                }
                                else
                                {
                                    dest[destIndex] = Interpolation.MixEven(patch[1], patch[3]);
                                    dest[destIndex + 1] = Interpolation.MixEven(patch[1], patch[4]);
                                    dest[destIndex + dpL] = Interpolation.MixEven(patch[3], patch[4]);
                                }
                                if (Diff(patch[1], patch[5], trY, trU, trV, trA))
                                {
                                    dest[destIndex + 2] = patch[4];
                                    dest[destIndex + 3] = patch[4];
                                    dest[destIndex + dpL + 3] = patch[4];
                                }
                                else
                                {
                                    dest[destIndex + 2] = Interpolation.MixEven(patch[1], patch[4]);
                                    dest[destIndex + 3] = Interpolation.MixEven(patch[1], patch[5]);
                                    dest[destIndex + dpL + 3] = Interpolation.MixEven(patch[5], patch[4]);
                                }
                                dest[destIndex + dpL + 1] = patch[4];
                                dest[destIndex + dpL + 2] = patch[4];
                                dest[destIndex + dpL + dpL] = Interpolation.Mix3To1(patch[4], patch[6]);
                                dest[destIndex + dpL + dpL + 1] = Interpolation.Mix7To1(patch[4], patch[6]);
                                dest[destIndex + dpL + dpL + 2] = Interpolation.Mix7To1(patch[4], patch[8]);
                                dest[destIndex + dpL + dpL + 3] = Interpolation.Mix3To1(patch[4], patch[8]);
                                dest[destIndex + dpL + dpL + dpL] = Interpolation.Mix5To3(patch[4], patch[6]);
                                dest[destIndex + dpL + dpL + dpL + 1] = Interpolation.Mix5To2To1(patch[4], patch[7], patch[6]);
                                dest[destIndex + dpL + dpL + dpL + 2] = Interpolation.Mix5To2To1(patch[4], patch[7], patch[8]);
                                dest[destIndex + dpL + dpL + dpL + 3] = Interpolation.Mix5To3(patch[4], patch[8]);
                                break;
                            }
                        case 82:
                        case 214:
                            {
                                dest[destIndex] = Interpolation.Mix5To3(patch[4], patch[0]);
                                dest[destIndex + 1] = Interpolation.Mix3To1(patch[4], patch[0]);
                                if (Diff(patch[1], patch[5], trY, trU, trV, trA))
                                {
                                    dest[destIndex + 2] = patch[4];
                                    dest[destIndex + 3] = patch[4];
                                    dest[destIndex + dpL + 3] = patch[4];
                                }
                                else
                                {
                                    dest[destIndex + 2] = Interpolation.MixEven(patch[1], patch[4]);
                                    dest[destIndex + 3] = Interpolation.MixEven(patch[1], patch[5]);
                                    dest[destIndex + dpL + 3] = Interpolation.MixEven(patch[5], patch[4]);
                                }
                                dest[destIndex + dpL] = Interpolation.Mix5To2To1(patch[4], patch[3], patch[0]);
                                dest[destIndex + dpL + 1] = Interpolation.Mix7To1(patch[4], patch[0]);
                                dest[destIndex + dpL + 2] = patch[4];
                                dest[destIndex + dpL + dpL] = Interpolation.Mix5To2To1(patch[4], patch[3], patch[6]);
                                dest[destIndex + dpL + dpL + 1] = Interpolation.Mix7To1(patch[4], patch[6]);
                                dest[destIndex + dpL + dpL + 2] = patch[4];
                                if (Diff(patch[5], patch[7], trY, trU, trV, trA))
                                {
                                    dest[destIndex + dpL + dpL + 3] = patch[4];
                                    dest[destIndex + dpL + dpL + dpL + 2] = patch[4];
                                    dest[destIndex + dpL + dpL + dpL + 3] = patch[4];
                                }
                                else
                                {
                                    dest[destIndex + dpL + dpL + 3] = Interpolation.MixEven(patch[5], patch[4]);
                                    dest[destIndex + dpL + dpL + dpL + 2] = Interpolation.MixEven(patch[7], patch[4]);
                                    dest[destIndex + dpL + dpL + dpL + 3] = Interpolation.MixEven(patch[7], patch[5]);
                                }
                                dest[destIndex + dpL + dpL + dpL] = Interpolation.Mix5To3(patch[4], patch[6]);
                                dest[destIndex + dpL + dpL + dpL + 1] = Interpolation.Mix3To1(patch[4], patch[6]);
                                break;
                            }
                        case 88:
                        case 248:
                            {
                                dest[destIndex] = Interpolation.Mix5To3(patch[4], patch[0]);
                                dest[destIndex + 1] = Interpolation.Mix5To2To1(patch[4], patch[1], patch[0]);
                                dest[destIndex + 2] = Interpolation.Mix5To2To1(patch[4], patch[1], patch[2]);
                                dest[destIndex + 3] = Interpolation.Mix5To3(patch[4], patch[2]);
                                dest[destIndex + dpL] = Interpolation.Mix3To1(patch[4], patch[0]);
                                dest[destIndex + dpL + 1] = Interpolation.Mix7To1(patch[4], patch[0]);
                                dest[destIndex + dpL + 2] = Interpolation.Mix7To1(patch[4], patch[2]);
                                dest[destIndex + dpL + 3] = Interpolation.Mix3To1(patch[4], patch[2]);
                                if (Diff(patch[7], patch[3], trY, trU, trV, trA))
                                {
                                    dest[destIndex + dpL + dpL] = patch[4];
                                    dest[destIndex + dpL + dpL + dpL] = patch[4];
                                    dest[destIndex + dpL + dpL + dpL + 1] = patch[4];
                                }
                                else
                                {
                                    dest[destIndex + dpL + dpL] = Interpolation.MixEven(patch[3], patch[4]);
                                    dest[destIndex + dpL + dpL + dpL] = Interpolation.MixEven(patch[7], patch[3]);
                                    dest[destIndex + dpL + dpL + dpL + 1] = Interpolation.MixEven(patch[7], patch[4]);
                                }
                                dest[destIndex + dpL + dpL + 1] = patch[4];
                                dest[destIndex + dpL + dpL + 2] = patch[4];
                                if (Diff(patch[5], patch[7], trY, trU, trV, trA))
                                {
                                    dest[destIndex + dpL + dpL + 3] = patch[4];
                                    dest[destIndex + dpL + dpL + dpL + 2] = patch[4];
                                    dest[destIndex + dpL + dpL + dpL + 3] = patch[4];
                                }
                                else
                                {
                                    dest[destIndex + dpL + dpL + 3] = Interpolation.MixEven(patch[5], patch[4]);
                                    dest[destIndex + dpL + dpL + dpL + 2] = Interpolation.MixEven(patch[7], patch[4]);
                                    dest[destIndex + dpL + dpL + dpL + 3] = Interpolation.MixEven(patch[7], patch[5]);
                                }
                                break;
                            }
                        case 74:
                        case 107:
                            {
                                if (Diff(patch[3], patch[1], trY, trU, trV, trA))
                                {
                                    dest[destIndex] = patch[4];
                                    dest[destIndex + 1] = patch[4];
                                    dest[destIndex + dpL] = patch[4];
                                }
                                else
                                {
                                    dest[destIndex] = Interpolation.MixEven(patch[1], patch[3]);
                                    dest[destIndex + 1] = Interpolation.MixEven(patch[1], patch[4]);
                                    dest[destIndex + dpL] = Interpolation.MixEven(patch[3], patch[4]);
                                }
                                dest[destIndex + 2] = Interpolation.Mix3To1(patch[4], patch[2]);
                                dest[destIndex + 3] = Interpolation.Mix5To3(patch[4], patch[2]);
                                dest[destIndex + dpL + 1] = patch[4];
                                dest[destIndex + dpL + 2] = Interpolation.Mix7To1(patch[4], patch[2]);
                                dest[destIndex + dpL + 3] = Interpolation.Mix5To2To1(patch[4], patch[5], patch[2]);
                                if (Diff(patch[7], patch[3], trY, trU, trV, trA))
                                {
                                    dest[destIndex + dpL + dpL] = patch[4];
                                    dest[destIndex + dpL + dpL + dpL] = patch[4];
                                    dest[destIndex + dpL + dpL + dpL + 1] = patch[4];
                                }
                                else
                                {
                                    dest[destIndex + dpL + dpL] = Interpolation.MixEven(patch[3], patch[4]);
                                    dest[destIndex + dpL + dpL + dpL] = Interpolation.MixEven(patch[7], patch[3]);
                                    dest[destIndex + dpL + dpL + dpL + 1] = Interpolation.MixEven(patch[7], patch[4]);
                                }
                                dest[destIndex + dpL + dpL + 1] = patch[4];
                                dest[destIndex + dpL + dpL + 2] = Interpolation.Mix7To1(patch[4], patch[8]);
                                dest[destIndex + dpL + dpL + 3] = Interpolation.Mix5To2To1(patch[4], patch[5], patch[8]);
                                dest[destIndex + dpL + dpL + dpL + 2] = Interpolation.Mix3To1(patch[4], patch[8]);
                                dest[destIndex + dpL + dpL + dpL + 3] = Interpolation.Mix5To3(patch[4], patch[8]);
                                break;
                            }
                        case 27:
                            {
                                if (Diff(patch[3], patch[1], trY, trU, trV, trA))
                                {
                                    dest[destIndex] = patch[4];
                                    dest[destIndex + 1] = patch[4];
                                    dest[destIndex + dpL] = patch[4];
                                }
                                else
                                {
                                    dest[destIndex] = Interpolation.MixEven(patch[1], patch[3]);
                                    dest[destIndex + 1] = Interpolation.MixEven(patch[1], patch[4]);
                                    dest[destIndex + dpL] = Interpolation.MixEven(patch[3], patch[4]);
                                }
                                dest[destIndex + 2] = Interpolation.Mix3To1(patch[4], patch[2]);
                                dest[destIndex + 3] = Interpolation.Mix5To3(patch[4], patch[2]);
                                dest[destIndex + dpL + 1] = patch[4];
                                dest[destIndex + dpL + 2] = Interpolation.Mix7To1(patch[4], patch[2]);
                                dest[destIndex + dpL + 3] = Interpolation.Mix3To1(patch[4], patch[2]);
                                dest[destIndex + dpL + dpL] = Interpolation.Mix3To1(patch[4], patch[6]);
                                dest[destIndex + dpL + dpL + 1] = Interpolation.Mix7To1(patch[4], patch[6]);
                                dest[destIndex + dpL + dpL + 2] = Interpolation.Mix7To1(patch[4], patch[8]);
                                dest[destIndex + dpL + dpL + 3] = Interpolation.Mix3To1(patch[4], patch[8]);
                                dest[destIndex + dpL + dpL + dpL] = Interpolation.Mix5To3(patch[4], patch[6]);
                                dest[destIndex + dpL + dpL + dpL + 1] = Interpolation.Mix5To2To1(patch[4], patch[7], patch[6]);
                                dest[destIndex + dpL + dpL + dpL + 2] = Interpolation.Mix5To2To1(patch[4], patch[7], patch[8]);
                                dest[destIndex + dpL + dpL + dpL + 3] = Interpolation.Mix5To3(patch[4], patch[8]);
                                break;
                            }
                        case 86:
                            {
                                dest[destIndex] = Interpolation.Mix5To3(patch[4], patch[0]);
                                dest[destIndex + 1] = Interpolation.Mix3To1(patch[4], patch[0]);
                                if (Diff(patch[1], patch[5], trY, trU, trV, trA))
                                {
                                    dest[destIndex + 2] = patch[4];
                                    dest[destIndex + 3] = patch[4];
                                    dest[destIndex + dpL + 3] = patch[4];
                                }
                                else
                                {
                                    dest[destIndex + 2] = Interpolation.MixEven(patch[1], patch[4]);
                                    dest[destIndex + 3] = Interpolation.MixEven(patch[1], patch[5]);
                                    dest[destIndex + dpL + 3] = Interpolation.MixEven(patch[5], patch[4]);
                                }
                                dest[destIndex + dpL] = Interpolation.Mix5To2To1(patch[4], patch[3], patch[0]);
                                dest[destIndex + dpL + 1] = Interpolation.Mix7To1(patch[4], patch[0]);
                                dest[destIndex + dpL + 2] = patch[4];
                                dest[destIndex + dpL + dpL] = Interpolation.Mix5To2To1(patch[4], patch[3], patch[6]);
                                dest[destIndex + dpL + dpL + 1] = Interpolation.Mix7To1(patch[4], patch[6]);
                                dest[destIndex + dpL + dpL + 2] = Interpolation.Mix7To1(patch[4], patch[8]);
                                dest[destIndex + dpL + dpL + 3] = Interpolation.Mix3To1(patch[4], patch[8]);
                                dest[destIndex + dpL + dpL + dpL] = Interpolation.Mix5To3(patch[4], patch[6]);
                                dest[destIndex + dpL + dpL + dpL + 1] = Interpolation.Mix3To1(patch[4], patch[6]);
                                dest[destIndex + dpL + dpL + dpL + 2] = Interpolation.Mix3To1(patch[4], patch[8]);
                                dest[destIndex + dpL + dpL + dpL + 3] = Interpolation.Mix5To3(patch[4], patch[8]);
                                break;
                            }
                        case 216:
                            {
                                dest[destIndex] = Interpolation.Mix5To3(patch[4], patch[0]);
                                dest[destIndex + 1] = Interpolation.Mix5To2To1(patch[4], patch[1], patch[0]);
                                dest[destIndex + 2] = Interpolation.Mix5To2To1(patch[4], patch[1], patch[2]);
                                dest[destIndex + 3] = Interpolation.Mix5To3(patch[4], patch[2]);
                                dest[destIndex + dpL] = Interpolation.Mix3To1(patch[4], patch[0]);
                                dest[destIndex + dpL + 1] = Interpolation.Mix7To1(patch[4], patch[0]);
                                dest[destIndex + dpL + 2] = Interpolation.Mix7To1(patch[4], patch[2]);
                                dest[destIndex + dpL + 3] = Interpolation.Mix3To1(patch[4], patch[2]);
                                dest[destIndex + dpL + dpL] = Interpolation.Mix3To1(patch[4], patch[6]);
                                dest[destIndex + dpL + dpL + 1] = Interpolation.Mix7To1(patch[4], patch[6]);
                                dest[destIndex + dpL + dpL + 2] = patch[4];
                                if (Diff(patch[5], patch[7], trY, trU, trV, trA))
                                {
                                    dest[destIndex + dpL + dpL + 3] = patch[4];
                                    dest[destIndex + dpL + dpL + dpL + 2] = patch[4];
                                    dest[destIndex + dpL + dpL + dpL + 3] = patch[4];
                                }
                                else
                                {
                                    dest[destIndex + dpL + dpL + 3] = Interpolation.MixEven(patch[5], patch[4]);
                                    dest[destIndex + dpL + dpL + dpL + 2] = Interpolation.MixEven(patch[7], patch[4]);
                                    dest[destIndex + dpL + dpL + dpL + 3] = Interpolation.MixEven(patch[7], patch[5]);
                                }
                                dest[destIndex + dpL + dpL + dpL] = Interpolation.Mix5To3(patch[4], patch[6]);
                                dest[destIndex + dpL + dpL + dpL + 1] = Interpolation.Mix3To1(patch[4], patch[6]);
                                break;
                            }
                        case 106:
                            {
                                dest[destIndex] = Interpolation.Mix5To3(patch[4], patch[0]);
                                dest[destIndex + 1] = Interpolation.Mix3To1(patch[4], patch[0]);
                                dest[destIndex + 2] = Interpolation.Mix3To1(patch[4], patch[2]);
                                dest[destIndex + 3] = Interpolation.Mix5To3(patch[4], patch[2]);
                                dest[destIndex + dpL] = Interpolation.Mix3To1(patch[4], patch[0]);
                                dest[destIndex + dpL + 1] = Interpolation.Mix7To1(patch[4], patch[0]);
                                dest[destIndex + dpL + 2] = Interpolation.Mix7To1(patch[4], patch[2]);
                                dest[destIndex + dpL + 3] = Interpolation.Mix5To2To1(patch[4], patch[5], patch[2]);
                                if (Diff(patch[7], patch[3], trY, trU, trV, trA))
                                {
                                    dest[destIndex + dpL + dpL] = patch[4];
                                    dest[destIndex + dpL + dpL + dpL] = patch[4];
                                    dest[destIndex + dpL + dpL + dpL + 1] = patch[4];
                                }
                                else
                                {
                                    dest[destIndex + dpL + dpL] = Interpolation.MixEven(patch[3], patch[4]);
                                    dest[destIndex + dpL + dpL + dpL] = Interpolation.MixEven(patch[7], patch[3]);
                                    dest[destIndex + dpL + dpL + dpL + 1] = Interpolation.MixEven(patch[7], patch[4]);
                                }
                                dest[destIndex + dpL + dpL + 1] = patch[4];
                                dest[destIndex + dpL + dpL + 2] = Interpolation.Mix7To1(patch[4], patch[8]);
                                dest[destIndex + dpL + dpL + 3] = Interpolation.Mix5To2To1(patch[4], patch[5], patch[8]);
                                dest[destIndex + dpL + dpL + dpL + 2] = Interpolation.Mix3To1(patch[4], patch[8]);
                                dest[destIndex + dpL + dpL + dpL + 3] = Interpolation.Mix5To3(patch[4], patch[8]);
                                break;
                            }
                        case 30:
                            {
                                dest[destIndex] = Interpolation.Mix5To3(patch[4], patch[0]);
                                dest[destIndex + 1] = Interpolation.Mix3To1(patch[4], patch[0]);
                                if (Diff(patch[1], patch[5], trY, trU, trV, trA))
                                {
                                    dest[destIndex + 2] = patch[4];
                                    dest[destIndex + 3] = patch[4];
                                    dest[destIndex + dpL + 3] = patch[4];
                                }
                                else
                                {
                                    dest[destIndex + 2] = Interpolation.MixEven(patch[1], patch[4]);
                                    dest[destIndex + 3] = Interpolation.MixEven(patch[1], patch[5]);
                                    dest[destIndex + dpL + 3] = Interpolation.MixEven(patch[5], patch[4]);
                                }
                                dest[destIndex + dpL] = Interpolation.Mix3To1(patch[4], patch[0]);
                                dest[destIndex + dpL + 1] = Interpolation.Mix7To1(patch[4], patch[0]);
                                dest[destIndex + dpL + 2] = patch[4];
                                dest[destIndex + dpL + dpL] = Interpolation.Mix3To1(patch[4], patch[6]);
                                dest[destIndex + dpL + dpL + 1] = Interpolation.Mix7To1(patch[4], patch[6]);
                                dest[destIndex + dpL + dpL + 2] = Interpolation.Mix7To1(patch[4], patch[8]);
                                dest[destIndex + dpL + dpL + 3] = Interpolation.Mix3To1(patch[4], patch[8]);
                                dest[destIndex + dpL + dpL + dpL] = Interpolation.Mix5To3(patch[4], patch[6]);
                                dest[destIndex + dpL + dpL + dpL + 1] = Interpolation.Mix5To2To1(patch[4], patch[7], patch[6]);
                                dest[destIndex + dpL + dpL + dpL + 2] = Interpolation.Mix5To2To1(patch[4], patch[7], patch[8]);
                                dest[destIndex + dpL + dpL + dpL + 3] = Interpolation.Mix5To3(patch[4], patch[8]);
                                break;
                            }
                        case 210:
                            {
                                dest[destIndex] = Interpolation.Mix5To3(patch[4], patch[0]);
                                dest[destIndex + 1] = Interpolation.Mix3To1(patch[4], patch[0]);
                                dest[destIndex + 2] = Interpolation.Mix3To1(patch[4], patch[2]);
                                dest[destIndex + 3] = Interpolation.Mix5To3(patch[4], patch[2]);
                                dest[destIndex + dpL] = Interpolation.Mix5To2To1(patch[4], patch[3], patch[0]);
                                dest[destIndex + dpL + 1] = Interpolation.Mix7To1(patch[4], patch[0]);
                                dest[destIndex + dpL + 2] = Interpolation.Mix7To1(patch[4], patch[2]);
                                dest[destIndex + dpL + 3] = Interpolation.Mix3To1(patch[4], patch[2]);
                                dest[destIndex + dpL + dpL] = Interpolation.Mix5To2To1(patch[4], patch[3], patch[6]);
                                dest[destIndex + dpL + dpL + 1] = Interpolation.Mix7To1(patch[4], patch[6]);
                                dest[destIndex + dpL + dpL + 2] = patch[4];
                                if (Diff(patch[5], patch[7], trY, trU, trV, trA))
                                {
                                    dest[destIndex + dpL + dpL + 3] = patch[4];
                                    dest[destIndex + dpL + dpL + dpL + 2] = patch[4];
                                    dest[destIndex + dpL + dpL + dpL + 3] = patch[4];
                                }
                                else
                                {
                                    dest[destIndex + dpL + dpL + 3] = Interpolation.MixEven(patch[5], patch[4]);
                                    dest[destIndex + dpL + dpL + dpL + 2] = Interpolation.MixEven(patch[7], patch[4]);
                                    dest[destIndex + dpL + dpL + dpL + 3] = Interpolation.MixEven(patch[7], patch[5]);
                                }
                                dest[destIndex + dpL + dpL + dpL] = Interpolation.Mix5To3(patch[4], patch[6]);
                                dest[destIndex + dpL + dpL + dpL + 1] = Interpolation.Mix3To1(patch[4], patch[6]);
                                break;
                            }
                        case 120:
                            {
                                dest[destIndex] = Interpolation.Mix5To3(patch[4], patch[0]);
                                dest[destIndex + 1] = Interpolation.Mix5To2To1(patch[4], patch[1], patch[0]);
                                dest[destIndex + 2] = Interpolation.Mix5To2To1(patch[4], patch[1], patch[2]);
                                dest[destIndex + 3] = Interpolation.Mix5To3(patch[4], patch[2]);
                                dest[destIndex + dpL] = Interpolation.Mix3To1(patch[4], patch[0]);
                                dest[destIndex + dpL + 1] = Interpolation.Mix7To1(patch[4], patch[0]);
                                dest[destIndex + dpL + 2] = Interpolation.Mix7To1(patch[4], patch[2]);
                                dest[destIndex + dpL + 3] = Interpolation.Mix3To1(patch[4], patch[2]);
                                if (Diff(patch[7], patch[3], trY, trU, trV, trA))
                                {
                                    dest[destIndex + dpL + dpL] = patch[4];
                                    dest[destIndex + dpL + dpL + dpL] = patch[4];
                                    dest[destIndex + dpL + dpL + dpL + 1] = patch[4];
                                }
                                else
                                {
                                    dest[destIndex + dpL + dpL] = Interpolation.MixEven(patch[3], patch[4]);
                                    dest[destIndex + dpL + dpL + dpL] = Interpolation.MixEven(patch[7], patch[3]);
                                    dest[destIndex + dpL + dpL + dpL + 1] = Interpolation.MixEven(patch[7], patch[4]);
                                }
                                dest[destIndex + dpL + dpL + 1] = patch[4];
                                dest[destIndex + dpL + dpL + 2] = Interpolation.Mix7To1(patch[4], patch[8]);
                                dest[destIndex + dpL + dpL + 3] = Interpolation.Mix3To1(patch[4], patch[8]);
                                dest[destIndex + dpL + dpL + dpL + 2] = Interpolation.Mix3To1(patch[4], patch[8]);
                                dest[destIndex + dpL + dpL + dpL + 3] = Interpolation.Mix5To3(patch[4], patch[8]);
                                break;
                            }
                        case 75:
                            {
                                if (Diff(patch[3], patch[1], trY, trU, trV, trA))
                                {
                                    dest[destIndex] = patch[4];
                                    dest[destIndex + 1] = patch[4];
                                    dest[destIndex + dpL] = patch[4];
                                }
                                else
                                {
                                    dest[destIndex] = Interpolation.MixEven(patch[1], patch[3]);
                                    dest[destIndex + 1] = Interpolation.MixEven(patch[1], patch[4]);
                                    dest[destIndex + dpL] = Interpolation.MixEven(patch[3], patch[4]);
                                }
                                dest[destIndex + 2] = Interpolation.Mix3To1(patch[4], patch[2]);
                                dest[destIndex + 3] = Interpolation.Mix5To3(patch[4], patch[2]);
                                dest[destIndex + dpL + 1] = patch[4];
                                dest[destIndex + dpL + 2] = Interpolation.Mix7To1(patch[4], patch[2]);
                                dest[destIndex + dpL + 3] = Interpolation.Mix5To2To1(patch[4], patch[5], patch[2]);
                                dest[destIndex + dpL + dpL] = Interpolation.Mix3To1(patch[4], patch[6]);
                                dest[destIndex + dpL + dpL + 1] = Interpolation.Mix7To1(patch[4], patch[6]);
                                dest[destIndex + dpL + dpL + 2] = Interpolation.Mix7To1(patch[4], patch[8]);
                                dest[destIndex + dpL + dpL + 3] = Interpolation.Mix5To2To1(patch[4], patch[5], patch[8]);
                                dest[destIndex + dpL + dpL + dpL] = Interpolation.Mix5To3(patch[4], patch[6]);
                                dest[destIndex + dpL + dpL + dpL + 1] = Interpolation.Mix3To1(patch[4], patch[6]);
                                dest[destIndex + dpL + dpL + dpL + 2] = Interpolation.Mix3To1(patch[4], patch[8]);
                                dest[destIndex + dpL + dpL + dpL + 3] = Interpolation.Mix5To3(patch[4], patch[8]);
                                break;
                            }
                        case 29:
                            {
                                dest[destIndex] = Interpolation.Mix5To3(patch[4], patch[1]);
                                dest[destIndex + 1] = Interpolation.Mix5To3(patch[4], patch[1]);
                                dest[destIndex + 2] = Interpolation.Mix5To3(patch[4], patch[1]);
                                dest[destIndex + 3] = Interpolation.Mix5To3(patch[4], patch[1]);
                                dest[destIndex + dpL] = Interpolation.Mix7To1(patch[4], patch[1]);
                                dest[destIndex + dpL + 1] = Interpolation.Mix7To1(patch[4], patch[1]);
                                dest[destIndex + dpL + 2] = Interpolation.Mix7To1(patch[4], patch[1]);
                                dest[destIndex + dpL + 3] = Interpolation.Mix7To1(patch[4], patch[1]);
                                dest[destIndex + dpL + dpL] = Interpolation.Mix3To1(patch[4], patch[6]);
                                dest[destIndex + dpL + dpL + 1] = Interpolation.Mix7To1(patch[4], patch[6]);
                                dest[destIndex + dpL + dpL + 2] = Interpolation.Mix7To1(patch[4], patch[8]);
                                dest[destIndex + dpL + dpL + 3] = Interpolation.Mix3To1(patch[4], patch[8]);
                                dest[destIndex + dpL + dpL + dpL] = Interpolation.Mix5To3(patch[4], patch[6]);
                                dest[destIndex + dpL + dpL + dpL + 1] = Interpolation.Mix5To2To1(patch[4], patch[7], patch[6]);
                                dest[destIndex + dpL + dpL + dpL + 2] = Interpolation.Mix5To2To1(patch[4], patch[7], patch[8]);
                                dest[destIndex + dpL + dpL + dpL + 3] = Interpolation.Mix5To3(patch[4], patch[8]);
                                break;
                            }
                        case 198:
                            {
                                dest[destIndex] = Interpolation.Mix5To3(patch[4], patch[0]);
                                dest[destIndex + 1] = Interpolation.Mix3To1(patch[4], patch[0]);
                                dest[destIndex + 2] = Interpolation.Mix7To1(patch[4], patch[5]);
                                dest[destIndex + 3] = Interpolation.Mix5To3(patch[4], patch[5]);
                                dest[destIndex + dpL] = Interpolation.Mix5To2To1(patch[4], patch[3], patch[0]);
                                dest[destIndex + dpL + 1] = Interpolation.Mix7To1(patch[4], patch[0]);
                                dest[destIndex + dpL + 2] = Interpolation.Mix7To1(patch[4], patch[5]);
                                dest[destIndex + dpL + 3] = Interpolation.Mix5To3(patch[4], patch[5]);
                                dest[destIndex + dpL + dpL] = Interpolation.Mix5To2To1(patch[4], patch[3], patch[6]);
                                dest[destIndex + dpL + dpL + 1] = Interpolation.Mix7To1(patch[4], patch[6]);
                                dest[destIndex + dpL + dpL + 2] = Interpolation.Mix7To1(patch[4], patch[5]);
                                dest[destIndex + dpL + dpL + 3] = Interpolation.Mix5To3(patch[4], patch[5]);
                                dest[destIndex + dpL + dpL + dpL] = Interpolation.Mix5To3(patch[4], patch[6]);
                                dest[destIndex + dpL + dpL + dpL + 1] = Interpolation.Mix3To1(patch[4], patch[6]);
                                dest[destIndex + dpL + dpL + dpL + 2] = Interpolation.Mix7To1(patch[4], patch[5]);
                                dest[destIndex + dpL + dpL + dpL + 3] = Interpolation.Mix5To3(patch[4], patch[5]);
                                break;
                            }
                        case 184:
                            {
                                dest[destIndex] = Interpolation.Mix5To3(patch[4], patch[0]);
                                dest[destIndex + 1] = Interpolation.Mix5To2To1(patch[4], patch[1], patch[0]);
                                dest[destIndex + 2] = Interpolation.Mix5To2To1(patch[4], patch[1], patch[2]);
                                dest[destIndex + 3] = Interpolation.Mix5To3(patch[4], patch[2]);
                                dest[destIndex + dpL] = Interpolation.Mix3To1(patch[4], patch[0]);
                                dest[destIndex + dpL + 1] = Interpolation.Mix7To1(patch[4], patch[0]);
                                dest[destIndex + dpL + 2] = Interpolation.Mix7To1(patch[4], patch[2]);
                                dest[destIndex + dpL + 3] = Interpolation.Mix3To1(patch[4], patch[2]);
                                dest[destIndex + dpL + dpL] = Interpolation.Mix7To1(patch[4], patch[7]);
                                dest[destIndex + dpL + dpL + 1] = Interpolation.Mix7To1(patch[4], patch[7]);
                                dest[destIndex + dpL + dpL + 2] = Interpolation.Mix7To1(patch[4], patch[7]);
                                dest[destIndex + dpL + dpL + 3] = Interpolation.Mix7To1(patch[4], patch[7]);
                                dest[destIndex + dpL + dpL + dpL] = Interpolation.Mix5To3(patch[4], patch[7]);
                                dest[destIndex + dpL + dpL + dpL + 1] = Interpolation.Mix5To3(patch[4], patch[7]);
                                dest[destIndex + dpL + dpL + dpL + 2] = Interpolation.Mix5To3(patch[4], patch[7]);
                                dest[destIndex + dpL + dpL + dpL + 3] = Interpolation.Mix5To3(patch[4], patch[7]);
                                break;
                            }
                        case 99:
                            {
                                dest[destIndex] = Interpolation.Mix5To3(patch[4], patch[3]);
                                dest[destIndex + 1] = Interpolation.Mix7To1(patch[4], patch[3]);
                                dest[destIndex + 2] = Interpolation.Mix3To1(patch[4], patch[2]);
                                dest[destIndex + 3] = Interpolation.Mix5To3(patch[4], patch[2]);
                                dest[destIndex + dpL] = Interpolation.Mix5To3(patch[4], patch[3]);
                                dest[destIndex + dpL + 1] = Interpolation.Mix7To1(patch[4], patch[3]);
                                dest[destIndex + dpL + 2] = Interpolation.Mix7To1(patch[4], patch[2]);
                                dest[destIndex + dpL + 3] = Interpolation.Mix5To2To1(patch[4], patch[5], patch[2]);
                                dest[destIndex + dpL + dpL] = Interpolation.Mix5To3(patch[4], patch[3]);
                                dest[destIndex + dpL + dpL + 1] = Interpolation.Mix7To1(patch[4], patch[3]);
                                dest[destIndex + dpL + dpL + 2] = Interpolation.Mix7To1(patch[4], patch[8]);
                                dest[destIndex + dpL + dpL + 3] = Interpolation.Mix5To2To1(patch[4], patch[5], patch[8]);
                                dest[destIndex + dpL + dpL + dpL] = Interpolation.Mix5To3(patch[4], patch[3]);
                                dest[destIndex + dpL + dpL + dpL + 1] = Interpolation.Mix7To1(patch[4], patch[3]);
                                dest[destIndex + dpL + dpL + dpL + 2] = Interpolation.Mix3To1(patch[4], patch[8]);
                                dest[destIndex + dpL + dpL + dpL + 3] = Interpolation.Mix5To3(patch[4], patch[8]);
                                break;
                            }
                        case 57:
                            {
                                dest[destIndex] = Interpolation.Mix5To3(patch[4], patch[1]);
                                dest[destIndex + 1] = Interpolation.Mix5To3(patch[4], patch[1]);
                                dest[destIndex + 2] = Interpolation.Mix5To2To1(patch[4], patch[1], patch[2]);
                                dest[destIndex + 3] = Interpolation.Mix5To3(patch[4], patch[2]);
                                dest[destIndex + dpL] = Interpolation.Mix7To1(patch[4], patch[1]);
                                dest[destIndex + dpL + 1] = Interpolation.Mix7To1(patch[4], patch[1]);
                                dest[destIndex + dpL + 2] = Interpolation.Mix7To1(patch[4], patch[2]);
                                dest[destIndex + dpL + 3] = Interpolation.Mix3To1(patch[4], patch[2]);
                                dest[destIndex + dpL + dpL] = Interpolation.Mix7To1(patch[4], patch[7]);
                                dest[destIndex + dpL + dpL + 1] = Interpolation.Mix7To1(patch[4], patch[7]);
                                dest[destIndex + dpL + dpL + 2] = Interpolation.Mix7To1(patch[4], patch[8]);
                                dest[destIndex + dpL + dpL + 3] = Interpolation.Mix3To1(patch[4], patch[8]);
                                dest[destIndex + dpL + dpL + dpL] = Interpolation.Mix5To3(patch[4], patch[7]);
                                dest[destIndex + dpL + dpL + dpL + 1] = Interpolation.Mix5To3(patch[4], patch[7]);
                                dest[destIndex + dpL + dpL + dpL + 2] = Interpolation.Mix5To2To1(patch[4], patch[7], patch[8]);
                                dest[destIndex + dpL + dpL + dpL + 3] = Interpolation.Mix5To3(patch[4], patch[8]);
                                break;
                            }
                        case 71:
                            {
                                dest[destIndex] = Interpolation.Mix5To3(patch[4], patch[3]);
                                dest[destIndex + 1] = Interpolation.Mix7To1(patch[4], patch[3]);
                                dest[destIndex + 2] = Interpolation.Mix7To1(patch[4], patch[5]);
                                dest[destIndex + 3] = Interpolation.Mix5To3(patch[4], patch[5]);
                                dest[destIndex + dpL] = Interpolation.Mix5To3(patch[4], patch[3]);
                                dest[destIndex + dpL + 1] = Interpolation.Mix7To1(patch[4], patch[3]);
                                dest[destIndex + dpL + 2] = Interpolation.Mix7To1(patch[4], patch[5]);
                                dest[destIndex + dpL + 3] = Interpolation.Mix5To3(patch[4], patch[5]);
                                dest[destIndex + dpL + dpL] = Interpolation.Mix5To2To1(patch[4], patch[3], patch[6]);
                                dest[destIndex + dpL + dpL + 1] = Interpolation.Mix7To1(patch[4], patch[6]);
                                dest[destIndex + dpL + dpL + 2] = Interpolation.Mix7To1(patch[4], patch[8]);
                                dest[destIndex + dpL + dpL + 3] = Interpolation.Mix5To2To1(patch[4], patch[5], patch[8]);
                                dest[destIndex + dpL + dpL + dpL] = Interpolation.Mix5To3(patch[4], patch[6]);
                                dest[destIndex + dpL + dpL + dpL + 1] = Interpolation.Mix3To1(patch[4], patch[6]);
                                dest[destIndex + dpL + dpL + dpL + 2] = Interpolation.Mix3To1(patch[4], patch[8]);
                                dest[destIndex + dpL + dpL + dpL + 3] = Interpolation.Mix5To3(patch[4], patch[8]);
                                break;
                            }
                        case 156:
                            {
                                dest[destIndex] = Interpolation.Mix5To3(patch[4], patch[0]);
                                dest[destIndex + 1] = Interpolation.Mix5To2To1(patch[4], patch[1], patch[0]);
                                dest[destIndex + 2] = Interpolation.Mix5To3(patch[4], patch[1]);
                                dest[destIndex + 3] = Interpolation.Mix5To3(patch[4], patch[1]);
                                dest[destIndex + dpL] = Interpolation.Mix3To1(patch[4], patch[0]);
                                dest[destIndex + dpL + 1] = Interpolation.Mix7To1(patch[4], patch[0]);
                                dest[destIndex + dpL + 2] = Interpolation.Mix7To1(patch[4], patch[1]);
                                dest[destIndex + dpL + 3] = Interpolation.Mix7To1(patch[4], patch[1]);
                                dest[destIndex + dpL + dpL] = Interpolation.Mix3To1(patch[4], patch[6]);
                                dest[destIndex + dpL + dpL + 1] = Interpolation.Mix7To1(patch[4], patch[6]);
                                dest[destIndex + dpL + dpL + 2] = Interpolation.Mix7To1(patch[4], patch[7]);
                                dest[destIndex + dpL + dpL + 3] = Interpolation.Mix7To1(patch[4], patch[7]);
                                dest[destIndex + dpL + dpL + dpL] = Interpolation.Mix5To3(patch[4], patch[6]);
                                dest[destIndex + dpL + dpL + dpL + 1] = Interpolation.Mix5To2To1(patch[4], patch[7], patch[6]);
                                dest[destIndex + dpL + dpL + dpL + 2] = Interpolation.Mix5To3(patch[4], patch[7]);
                                dest[destIndex + dpL + dpL + dpL + 3] = Interpolation.Mix5To3(patch[4], patch[7]);
                                break;
                            }
                        case 226:
                            {
                                dest[destIndex] = Interpolation.Mix5To3(patch[4], patch[0]);
                                dest[destIndex + 1] = Interpolation.Mix3To1(patch[4], patch[0]);
                                dest[destIndex + 2] = Interpolation.Mix3To1(patch[4], patch[2]);
                                dest[destIndex + 3] = Interpolation.Mix5To3(patch[4], patch[2]);
                                dest[destIndex + dpL] = Interpolation.Mix5To2To1(patch[4], patch[3], patch[0]);
                                dest[destIndex + dpL + 1] = Interpolation.Mix7To1(patch[4], patch[0]);
                                dest[destIndex + dpL + 2] = Interpolation.Mix7To1(patch[4], patch[2]);
                                dest[destIndex + dpL + 3] = Interpolation.Mix5To2To1(patch[4], patch[5], patch[2]);
                                dest[destIndex + dpL + dpL] = Interpolation.Mix5To3(patch[4], patch[3]);
                                dest[destIndex + dpL + dpL + 1] = Interpolation.Mix7To1(patch[4], patch[3]);
                                dest[destIndex + dpL + dpL + 2] = Interpolation.Mix7To1(patch[4], patch[5]);
                                dest[destIndex + dpL + dpL + 3] = Interpolation.Mix5To3(patch[4], patch[5]);
                                dest[destIndex + dpL + dpL + dpL] = Interpolation.Mix5To3(patch[4], patch[3]);
                                dest[destIndex + dpL + dpL + dpL + 1] = Interpolation.Mix7To1(patch[4], patch[3]);
                                dest[destIndex + dpL + dpL + dpL + 2] = Interpolation.Mix7To1(patch[4], patch[5]);
                                dest[destIndex + dpL + dpL + dpL + 3] = Interpolation.Mix5To3(patch[4], patch[5]);
                                break;
                            }
                        case 60:
                            {
                                dest[destIndex] = Interpolation.Mix5To3(patch[4], patch[0]);
                                dest[destIndex + 1] = Interpolation.Mix5To2To1(patch[4], patch[1], patch[0]);
                                dest[destIndex + 2] = Interpolation.Mix5To3(patch[4], patch[1]);
                                dest[destIndex + 3] = Interpolation.Mix5To3(patch[4], patch[1]);
                                dest[destIndex + dpL] = Interpolation.Mix3To1(patch[4], patch[0]);
                                dest[destIndex + dpL + 1] = Interpolation.Mix7To1(patch[4], patch[0]);
                                dest[destIndex + dpL + 2] = Interpolation.Mix7To1(patch[4], patch[1]);
                                dest[destIndex + dpL + 3] = Interpolation.Mix7To1(patch[4], patch[1]);
                                dest[destIndex + dpL + dpL] = Interpolation.Mix7To1(patch[4], patch[7]);
                                dest[destIndex + dpL + dpL + 1] = Interpolation.Mix7To1(patch[4], patch[7]);
                                dest[destIndex + dpL + dpL + 2] = Interpolation.Mix7To1(patch[4], patch[8]);
                                dest[destIndex + dpL + dpL + 3] = Interpolation.Mix3To1(patch[4], patch[8]);
                                dest[destIndex + dpL + dpL + dpL] = Interpolation.Mix5To3(patch[4], patch[7]);
                                dest[destIndex + dpL + dpL + dpL + 1] = Interpolation.Mix5To3(patch[4], patch[7]);
                                dest[destIndex + dpL + dpL + dpL + 2] = Interpolation.Mix5To2To1(patch[4], patch[7], patch[8]);
                                dest[destIndex + dpL + dpL + dpL + 3] = Interpolation.Mix5To3(patch[4], patch[8]);
                                break;
                            }
                        case 195:
                            {
                                dest[destIndex] = Interpolation.Mix5To3(patch[4], patch[3]);
                                dest[destIndex + 1] = Interpolation.Mix7To1(patch[4], patch[3]);
                                dest[destIndex + 2] = Interpolation.Mix3To1(patch[4], patch[2]);
                                dest[destIndex + 3] = Interpolation.Mix5To3(patch[4], patch[2]);
                                dest[destIndex + dpL] = Interpolation.Mix5To3(patch[4], patch[3]);
                                dest[destIndex + dpL + 1] = Interpolation.Mix7To1(patch[4], patch[3]);
                                dest[destIndex + dpL + 2] = Interpolation.Mix7To1(patch[4], patch[2]);
                                dest[destIndex + dpL + 3] = Interpolation.Mix5To2To1(patch[4], patch[5], patch[2]);
                                dest[destIndex + dpL + dpL] = Interpolation.Mix5To2To1(patch[4], patch[3], patch[6]);
                                dest[destIndex + dpL + dpL + 1] = Interpolation.Mix7To1(patch[4], patch[6]);
                                dest[destIndex + dpL + dpL + 2] = Interpolation.Mix7To1(patch[4], patch[5]);
                                dest[destIndex + dpL + dpL + 3] = Interpolation.Mix5To3(patch[4], patch[5]);
                                dest[destIndex + dpL + dpL + dpL] = Interpolation.Mix5To3(patch[4], patch[6]);
                                dest[destIndex + dpL + dpL + dpL + 1] = Interpolation.Mix3To1(patch[4], patch[6]);
                                dest[destIndex + dpL + dpL + dpL + 2] = Interpolation.Mix7To1(patch[4], patch[5]);
                                dest[destIndex + dpL + dpL + dpL + 3] = Interpolation.Mix5To3(patch[4], patch[5]);
                                break;
                            }
                        case 102:
                            {
                                dest[destIndex] = Interpolation.Mix5To3(patch[4], patch[0]);
                                dest[destIndex + 1] = Interpolation.Mix3To1(patch[4], patch[0]);
                                dest[destIndex + 2] = Interpolation.Mix7To1(patch[4], patch[5]);
                                dest[destIndex + 3] = Interpolation.Mix5To3(patch[4], patch[5]);
                                dest[destIndex + dpL] = Interpolation.Mix5To2To1(patch[4], patch[3], patch[0]);
                                dest[destIndex + dpL + 1] = Interpolation.Mix7To1(patch[4], patch[0]);
                                dest[destIndex + dpL + 2] = Interpolation.Mix7To1(patch[4], patch[5]);
                                dest[destIndex + dpL + 3] = Interpolation.Mix5To3(patch[4], patch[5]);
                                dest[destIndex + dpL + dpL] = Interpolation.Mix5To3(patch[4], patch[3]);
                                dest[destIndex + dpL + dpL + 1] = Interpolation.Mix7To1(patch[4], patch[3]);
                                dest[destIndex + dpL + dpL + 2] = Interpolation.Mix7To1(patch[4], patch[8]);
                                dest[destIndex + dpL + dpL + 3] = Interpolation.Mix5To2To1(patch[4], patch[5], patch[8]);
                                dest[destIndex + dpL + dpL + dpL] = Interpolation.Mix5To3(patch[4], patch[3]);
                                dest[destIndex + dpL + dpL + dpL + 1] = Interpolation.Mix7To1(patch[4], patch[3]);
                                dest[destIndex + dpL + dpL + dpL + 2] = Interpolation.Mix3To1(patch[4], patch[8]);
                                dest[destIndex + dpL + dpL + dpL + 3] = Interpolation.Mix5To3(patch[4], patch[8]);
                                break;
                            }
                        case 153:
                            {
                                dest[destIndex] = Interpolation.Mix5To3(patch[4], patch[1]);
                                dest[destIndex + 1] = Interpolation.Mix5To3(patch[4], patch[1]);
                                dest[destIndex + 2] = Interpolation.Mix5To2To1(patch[4], patch[1], patch[2]);
                                dest[destIndex + 3] = Interpolation.Mix5To3(patch[4], patch[2]);
                                dest[destIndex + dpL] = Interpolation.Mix7To1(patch[4], patch[1]);
                                dest[destIndex + dpL + 1] = Interpolation.Mix7To1(patch[4], patch[1]);
                                dest[destIndex + dpL + 2] = Interpolation.Mix7To1(patch[4], patch[2]);
                                dest[destIndex + dpL + 3] = Interpolation.Mix3To1(patch[4], patch[2]);
                                dest[destIndex + dpL + dpL] = Interpolation.Mix3To1(patch[4], patch[6]);
                                dest[destIndex + dpL + dpL + 1] = Interpolation.Mix7To1(patch[4], patch[6]);
                                dest[destIndex + dpL + dpL + 2] = Interpolation.Mix7To1(patch[4], patch[7]);
                                dest[destIndex + dpL + dpL + 3] = Interpolation.Mix7To1(patch[4], patch[7]);
                                dest[destIndex + dpL + dpL + dpL] = Interpolation.Mix5To3(patch[4], patch[6]);
                                dest[destIndex + dpL + dpL + dpL + 1] = Interpolation.Mix5To2To1(patch[4], patch[7], patch[6]);
                                dest[destIndex + dpL + dpL + dpL + 2] = Interpolation.Mix5To3(patch[4], patch[7]);
                                dest[destIndex + dpL + dpL + dpL + 3] = Interpolation.Mix5To3(patch[4], patch[7]);
                                break;
                            }
                        case 58:
                            {
                                if (Diff(patch[3], patch[1], trY, trU, trV, trA))
                                {
                                    dest[destIndex] = Interpolation.Mix5To3(patch[4], patch[0]);
                                    dest[destIndex + 1] = Interpolation.Mix3To1(patch[4], patch[0]);
                                    dest[destIndex + dpL] = Interpolation.Mix3To1(patch[4], patch[0]);
                                    dest[destIndex + dpL + 1] = Interpolation.Mix7To1(patch[4], patch[0]);
                                }
                                else
                                {
                                    dest[destIndex] = Interpolation.Mix2To1To1(patch[4], patch[1], patch[3]);
                                    dest[destIndex + 1] = Interpolation.Mix3To1(patch[4], patch[1]);
                                    dest[destIndex + dpL] = Interpolation.Mix3To1(patch[4], patch[3]);
                                    dest[destIndex + dpL + 1] = patch[4];
                                }
                                if (Diff(patch[1], patch[5], trY, trU, trV, trA))
                                {
                                    dest[destIndex + 2] = Interpolation.Mix3To1(patch[4], patch[2]);
                                    dest[destIndex + 3] = Interpolation.Mix5To3(patch[4], patch[2]);
                                    dest[destIndex + dpL + 2] = Interpolation.Mix7To1(patch[4], patch[2]);
                                    dest[destIndex + dpL + 3] = Interpolation.Mix3To1(patch[4], patch[2]);
                                }
                                else
                                {
                                    dest[destIndex + 2] = Interpolation.Mix3To1(patch[4], patch[1]);
                                    dest[destIndex + 3] = Interpolation.Mix2To1To1(patch[4], patch[1], patch[5]);
                                    dest[destIndex + dpL + 2] = patch[4];
                                    dest[destIndex + dpL + 3] = Interpolation.Mix3To1(patch[4], patch[5]);
                                }
                                dest[destIndex + dpL + dpL] = Interpolation.Mix7To1(patch[4], patch[7]);
                                dest[destIndex + dpL + dpL + 1] = Interpolation.Mix7To1(patch[4], patch[7]);
                                dest[destIndex + dpL + dpL + 2] = Interpolation.Mix7To1(patch[4], patch[8]);
                                dest[destIndex + dpL + dpL + 3] = Interpolation.Mix3To1(patch[4], patch[8]);
                                dest[destIndex + dpL + dpL + dpL] = Interpolation.Mix5To3(patch[4], patch[7]);
                                dest[destIndex + dpL + dpL + dpL + 1] = Interpolation.Mix5To3(patch[4], patch[7]);
                                dest[destIndex + dpL + dpL + dpL + 2] = Interpolation.Mix5To2To1(patch[4], patch[7], patch[8]);
                                dest[destIndex + dpL + dpL + dpL + 3] = Interpolation.Mix5To3(patch[4], patch[8]);
                                break;
                            }
                        case 83:
                            {
                                dest[destIndex] = Interpolation.Mix5To3(patch[4], patch[3]);
                                dest[destIndex + 1] = Interpolation.Mix7To1(patch[4], patch[3]);
                                if (Diff(patch[1], patch[5], trY, trU, trV, trA))
                                {
                                    dest[destIndex + 2] = Interpolation.Mix3To1(patch[4], patch[2]);
                                    dest[destIndex + 3] = Interpolation.Mix5To3(patch[4], patch[2]);
                                    dest[destIndex + dpL + 2] = Interpolation.Mix7To1(patch[4], patch[2]);
                                    dest[destIndex + dpL + 3] = Interpolation.Mix3To1(patch[4], patch[2]);
                                }
                                else
                                {
                                    dest[destIndex + 2] = Interpolation.Mix3To1(patch[4], patch[1]);
                                    dest[destIndex + 3] = Interpolation.Mix2To1To1(patch[4], patch[1], patch[5]);
                                    dest[destIndex + dpL + 2] = patch[4];
                                    dest[destIndex + dpL + 3] = Interpolation.Mix3To1(patch[4], patch[5]);
                                }
                                dest[destIndex + dpL] = Interpolation.Mix5To3(patch[4], patch[3]);
                                dest[destIndex + dpL + 1] = Interpolation.Mix7To1(patch[4], patch[3]);
                                dest[destIndex + dpL + dpL] = Interpolation.Mix5To2To1(patch[4], patch[3], patch[6]);
                                dest[destIndex + dpL + dpL + 1] = Interpolation.Mix7To1(patch[4], patch[6]);
                                if (Diff(patch[5], patch[7], trY, trU, trV, trA))
                                {
                                    dest[destIndex + dpL + dpL + 2] = Interpolation.Mix7To1(patch[4], patch[8]);
                                    dest[destIndex + dpL + dpL + 3] = Interpolation.Mix3To1(patch[4], patch[8]);
                                    dest[destIndex + dpL + dpL + dpL + 2] = Interpolation.Mix3To1(patch[4], patch[8]);
                                    dest[destIndex + dpL + dpL + dpL + 3] = Interpolation.Mix5To3(patch[4], patch[8]);
                                }
                                else
                                {
                                    dest[destIndex + dpL + dpL + 2] = patch[4];
                                    dest[destIndex + dpL + dpL + 3] = Interpolation.Mix3To1(patch[4], patch[5]);
                                    dest[destIndex + dpL + dpL + dpL + 2] = Interpolation.Mix3To1(patch[4], patch[7]);
                                    dest[destIndex + dpL + dpL + dpL + 3] = Interpolation.Mix2To1To1(patch[4], patch[7], patch[5]);
                                }
                                dest[destIndex + dpL + dpL + dpL] = Interpolation.Mix5To3(patch[4], patch[6]);
                                dest[destIndex + dpL + dpL + dpL + 1] = Interpolation.Mix3To1(patch[4], patch[6]);
                                break;
                            }
                        case 92:
                            {
                                dest[destIndex] = Interpolation.Mix5To3(patch[4], patch[0]);
                                dest[destIndex + 1] = Interpolation.Mix5To2To1(patch[4], patch[1], patch[0]);
                                dest[destIndex + 2] = Interpolation.Mix5To3(patch[4], patch[1]);
                                dest[destIndex + 3] = Interpolation.Mix5To3(patch[4], patch[1]);
                                dest[destIndex + dpL] = Interpolation.Mix3To1(patch[4], patch[0]);
                                dest[destIndex + dpL + 1] = Interpolation.Mix7To1(patch[4], patch[0]);
                                dest[destIndex + dpL + 2] = Interpolation.Mix7To1(patch[4], patch[1]);
                                dest[destIndex + dpL + 3] = Interpolation.Mix7To1(patch[4], patch[1]);
                                if (Diff(patch[7], patch[3], trY, trU, trV, trA))
                                {
                                    dest[destIndex + dpL + dpL] = Interpolation.Mix3To1(patch[4], patch[6]);
                                    dest[destIndex + dpL + dpL + 1] = Interpolation.Mix7To1(patch[4], patch[6]);
                                    dest[destIndex + dpL + dpL + dpL] = Interpolation.Mix5To3(patch[4], patch[6]);
                                    dest[destIndex + dpL + dpL + dpL + 1] = Interpolation.Mix3To1(patch[4], patch[6]);
                                }
                                else
                                {
                                    dest[destIndex + dpL + dpL] = Interpolation.Mix3To1(patch[4], patch[3]);
                                    dest[destIndex + dpL + dpL + 1] = patch[4];
                                    dest[destIndex + dpL + dpL + dpL] = Interpolation.Mix2To1To1(patch[4], patch[7], patch[3]);
                                    dest[destIndex + dpL + dpL + dpL + 1] = Interpolation.Mix3To1(patch[4], patch[7]);
                                }
                                if (Diff(patch[5], patch[7], trY, trU, trV, trA))
                                {
                                    dest[destIndex + dpL + dpL + 2] = Interpolation.Mix7To1(patch[4], patch[8]);
                                    dest[destIndex + dpL + dpL + 3] = Interpolation.Mix3To1(patch[4], patch[8]);
                                    dest[destIndex + dpL + dpL + dpL + 2] = Interpolation.Mix3To1(patch[4], patch[8]);
                                    dest[destIndex + dpL + dpL + dpL + 3] = Interpolation.Mix5To3(patch[4], patch[8]);
                                }
                                else
                                {
                                    dest[destIndex + dpL + dpL + 2] = patch[4];
                                    dest[destIndex + dpL + dpL + 3] = Interpolation.Mix3To1(patch[4], patch[5]);
                                    dest[destIndex + dpL + dpL + dpL + 2] = Interpolation.Mix3To1(patch[4], patch[7]);
                                    dest[destIndex + dpL + dpL + dpL + 3] = Interpolation.Mix2To1To1(patch[4], patch[7], patch[5]);
                                }
                                break;
                            }
                        case 202:
                            {
                                if (Diff(patch[3], patch[1], trY, trU, trV, trA))
                                {
                                    dest[destIndex] = Interpolation.Mix5To3(patch[4], patch[0]);
                                    dest[destIndex + 1] = Interpolation.Mix3To1(patch[4], patch[0]);
                                    dest[destIndex + dpL] = Interpolation.Mix3To1(patch[4], patch[0]);
                                    dest[destIndex + dpL + 1] = Interpolation.Mix7To1(patch[4], patch[0]);
                                }
                                else
                                {
                                    dest[destIndex] = Interpolation.Mix2To1To1(patch[4], patch[1], patch[3]);
                                    dest[destIndex + 1] = Interpolation.Mix3To1(patch[4], patch[1]);
                                    dest[destIndex + dpL] = Interpolation.Mix3To1(patch[4], patch[3]);
                                    dest[destIndex + dpL + 1] = patch[4];
                                }
                                dest[destIndex + 2] = Interpolation.Mix3To1(patch[4], patch[2]);
                                dest[destIndex + 3] = Interpolation.Mix5To3(patch[4], patch[2]);
                                dest[destIndex + dpL + 2] = Interpolation.Mix7To1(patch[4], patch[2]);
                                dest[destIndex + dpL + 3] = Interpolation.Mix5To2To1(patch[4], patch[5], patch[2]);
                                if (Diff(patch[7], patch[3], trY, trU, trV, trA))
                                {
                                    dest[destIndex + dpL + dpL] = Interpolation.Mix3To1(patch[4], patch[6]);
                                    dest[destIndex + dpL + dpL + 1] = Interpolation.Mix7To1(patch[4], patch[6]);
                                    dest[destIndex + dpL + dpL + dpL] = Interpolation.Mix5To3(patch[4], patch[6]);
                                    dest[destIndex + dpL + dpL + dpL + 1] = Interpolation.Mix3To1(patch[4], patch[6]);
                                }
                                else
                                {
                                    dest[destIndex + dpL + dpL] = Interpolation.Mix3To1(patch[4], patch[3]);
                                    dest[destIndex + dpL + dpL + 1] = patch[4];
                                    dest[destIndex + dpL + dpL + dpL] = Interpolation.Mix2To1To1(patch[4], patch[7], patch[3]);
                                    dest[destIndex + dpL + dpL + dpL + 1] = Interpolation.Mix3To1(patch[4], patch[7]);
                                }
                                dest[destIndex + dpL + dpL + 2] = Interpolation.Mix7To1(patch[4], patch[5]);
                                dest[destIndex + dpL + dpL + 3] = Interpolation.Mix5To3(patch[4], patch[5]);
                                dest[destIndex + dpL + dpL + dpL + 2] = Interpolation.Mix7To1(patch[4], patch[5]);
                                dest[destIndex + dpL + dpL + dpL + 3] = Interpolation.Mix5To3(patch[4], patch[5]);
                                break;
                            }
                        case 78:
                            {
                                if (Diff(patch[3], patch[1], trY, trU, trV, trA))
                                {
                                    dest[destIndex] = Interpolation.Mix5To3(patch[4], patch[0]);
                                    dest[destIndex + 1] = Interpolation.Mix3To1(patch[4], patch[0]);
                                    dest[destIndex + dpL] = Interpolation.Mix3To1(patch[4], patch[0]);
                                    dest[destIndex + dpL + 1] = Interpolation.Mix7To1(patch[4], patch[0]);
                                }
                                else
                                {
                                    dest[destIndex] = Interpolation.Mix2To1To1(patch[4], patch[1], patch[3]);
                                    dest[destIndex + 1] = Interpolation.Mix3To1(patch[4], patch[1]);
                                    dest[destIndex + dpL] = Interpolation.Mix3To1(patch[4], patch[3]);
                                    dest[destIndex + dpL + 1] = patch[4];
                                }
                                dest[destIndex + 2] = Interpolation.Mix7To1(patch[4], patch[5]);
                                dest[destIndex + 3] = Interpolation.Mix5To3(patch[4], patch[5]);
                                dest[destIndex + dpL + 2] = Interpolation.Mix7To1(patch[4], patch[5]);
                                dest[destIndex + dpL + 3] = Interpolation.Mix5To3(patch[4], patch[5]);
                                if (Diff(patch[7], patch[3], trY, trU, trV, trA))
                                {
                                    dest[destIndex + dpL + dpL] = Interpolation.Mix3To1(patch[4], patch[6]);
                                    dest[destIndex + dpL + dpL + 1] = Interpolation.Mix7To1(patch[4], patch[6]);
                                    dest[destIndex + dpL + dpL + dpL] = Interpolation.Mix5To3(patch[4], patch[6]);
                                    dest[destIndex + dpL + dpL + dpL + 1] = Interpolation.Mix3To1(patch[4], patch[6]);
                                }
                                else
                                {
                                    dest[destIndex + dpL + dpL] = Interpolation.Mix3To1(patch[4], patch[3]);
                                    dest[destIndex + dpL + dpL + 1] = patch[4];
                                    dest[destIndex + dpL + dpL + dpL] = Interpolation.Mix2To1To1(patch[4], patch[7], patch[3]);
                                    dest[destIndex + dpL + dpL + dpL + 1] = Interpolation.Mix3To1(patch[4], patch[7]);
                                }
                                dest[destIndex + dpL + dpL + 2] = Interpolation.Mix7To1(patch[4], patch[8]);
                                dest[destIndex + dpL + dpL + 3] = Interpolation.Mix5To2To1(patch[4], patch[5], patch[8]);
                                dest[destIndex + dpL + dpL + dpL + 2] = Interpolation.Mix3To1(patch[4], patch[8]);
                                dest[destIndex + dpL + dpL + dpL + 3] = Interpolation.Mix5To3(patch[4], patch[8]);
                                break;
                            }
                        case 154:
                            {
                                if (Diff(patch[3], patch[1], trY, trU, trV, trA))
                                {
                                    dest[destIndex] = Interpolation.Mix5To3(patch[4], patch[0]);
                                    dest[destIndex + 1] = Interpolation.Mix3To1(patch[4], patch[0]);
                                    dest[destIndex + dpL] = Interpolation.Mix3To1(patch[4], patch[0]);
                                    dest[destIndex + dpL + 1] = Interpolation.Mix7To1(patch[4], patch[0]);
                                }
                                else
                                {
                                    dest[destIndex] = Interpolation.Mix2To1To1(patch[4], patch[1], patch[3]);
                                    dest[destIndex + 1] = Interpolation.Mix3To1(patch[4], patch[1]);
                                    dest[destIndex + dpL] = Interpolation.Mix3To1(patch[4], patch[3]);
                                    dest[destIndex + dpL + 1] = patch[4];
                                }
                                if (Diff(patch[1], patch[5], trY, trU, trV, trA))
                                {
                                    dest[destIndex + 2] = Interpolation.Mix3To1(patch[4], patch[2]);
                                    dest[destIndex + 3] = Interpolation.Mix5To3(patch[4], patch[2]);
                                    dest[destIndex + dpL + 2] = Interpolation.Mix7To1(patch[4], patch[2]);
                                    dest[destIndex + dpL + 3] = Interpolation.Mix3To1(patch[4], patch[2]);
                                }
                                else
                                {
                                    dest[destIndex + 2] = Interpolation.Mix3To1(patch[4], patch[1]);
                                    dest[destIndex + 3] = Interpolation.Mix2To1To1(patch[4], patch[1], patch[5]);
                                    dest[destIndex + dpL + 2] = patch[4];
                                    dest[destIndex + dpL + 3] = Interpolation.Mix3To1(patch[4], patch[5]);
                                }
                                dest[destIndex + dpL + dpL] = Interpolation.Mix3To1(patch[4], patch[6]);
                                dest[destIndex + dpL + dpL + 1] = Interpolation.Mix7To1(patch[4], patch[6]);
                                dest[destIndex + dpL + dpL + 2] = Interpolation.Mix7To1(patch[4], patch[7]);
                                dest[destIndex + dpL + dpL + 3] = Interpolation.Mix7To1(patch[4], patch[7]);
                                dest[destIndex + dpL + dpL + dpL] = Interpolation.Mix5To3(patch[4], patch[6]);
                                dest[destIndex + dpL + dpL + dpL + 1] = Interpolation.Mix5To2To1(patch[4], patch[7], patch[6]);
                                dest[destIndex + dpL + dpL + dpL + 2] = Interpolation.Mix5To3(patch[4], patch[7]);
                                dest[destIndex + dpL + dpL + dpL + 3] = Interpolation.Mix5To3(patch[4], patch[7]);
                                break;
                            }
                        case 114:
                            {
                                dest[destIndex] = Interpolation.Mix5To3(patch[4], patch[0]);
                                dest[destIndex + 1] = Interpolation.Mix3To1(patch[4], patch[0]);
                                if (Diff(patch[1], patch[5], trY, trU, trV, trA))
                                {
                                    dest[destIndex + 2] = Interpolation.Mix3To1(patch[4], patch[2]);
                                    dest[destIndex + 3] = Interpolation.Mix5To3(patch[4], patch[2]);
                                    dest[destIndex + dpL + 2] = Interpolation.Mix7To1(patch[4], patch[2]);
                                    dest[destIndex + dpL + 3] = Interpolation.Mix3To1(patch[4], patch[2]);
                                }
                                else
                                {
                                    dest[destIndex + 2] = Interpolation.Mix3To1(patch[4], patch[1]);
                                    dest[destIndex + 3] = Interpolation.Mix2To1To1(patch[4], patch[1], patch[5]);
                                    dest[destIndex + dpL + 2] = patch[4];
                                    dest[destIndex + dpL + 3] = Interpolation.Mix3To1(patch[4], patch[5]);
                                }
                                dest[destIndex + dpL] = Interpolation.Mix5To2To1(patch[4], patch[3], patch[0]);
                                dest[destIndex + dpL + 1] = Interpolation.Mix7To1(patch[4], patch[0]);
                                dest[destIndex + dpL + dpL] = Interpolation.Mix5To3(patch[4], patch[3]);
                                dest[destIndex + dpL + dpL + 1] = Interpolation.Mix7To1(patch[4], patch[3]);
                                if (Diff(patch[5], patch[7], trY, trU, trV, trA))
                                {
                                    dest[destIndex + dpL + dpL + 2] = Interpolation.Mix7To1(patch[4], patch[8]);
                                    dest[destIndex + dpL + dpL + 3] = Interpolation.Mix3To1(patch[4], patch[8]);
                                    dest[destIndex + dpL + dpL + dpL + 2] = Interpolation.Mix3To1(patch[4], patch[8]);
                                    dest[destIndex + dpL + dpL + dpL + 3] = Interpolation.Mix5To3(patch[4], patch[8]);
                                }
                                else
                                {
                                    dest[destIndex + dpL + dpL + 2] = patch[4];
                                    dest[destIndex + dpL + dpL + 3] = Interpolation.Mix3To1(patch[4], patch[5]);
                                    dest[destIndex + dpL + dpL + dpL + 2] = Interpolation.Mix3To1(patch[4], patch[7]);
                                    dest[destIndex + dpL + dpL + dpL + 3] = Interpolation.Mix2To1To1(patch[4], patch[7], patch[5]);
                                }
                                dest[destIndex + dpL + dpL + dpL] = Interpolation.Mix5To3(patch[4], patch[3]);
                                dest[destIndex + dpL + dpL + dpL + 1] = Interpolation.Mix7To1(patch[4], patch[3]);
                                break;
                            }
                        case 89:
                            {
                                dest[destIndex] = Interpolation.Mix5To3(patch[4], patch[1]);
                                dest[destIndex + 1] = Interpolation.Mix5To3(patch[4], patch[1]);
                                dest[destIndex + 2] = Interpolation.Mix5To2To1(patch[4], patch[1], patch[2]);
                                dest[destIndex + 3] = Interpolation.Mix5To3(patch[4], patch[2]);
                                dest[destIndex + dpL] = Interpolation.Mix7To1(patch[4], patch[1]);
                                dest[destIndex + dpL + 1] = Interpolation.Mix7To1(patch[4], patch[1]);
                                dest[destIndex + dpL + 2] = Interpolation.Mix7To1(patch[4], patch[2]);
                                dest[destIndex + dpL + 3] = Interpolation.Mix3To1(patch[4], patch[2]);
                                if (Diff(patch[7], patch[3], trY, trU, trV, trA))
                                {
                                    dest[destIndex + dpL + dpL] = Interpolation.Mix3To1(patch[4], patch[6]);
                                    dest[destIndex + dpL + dpL + 1] = Interpolation.Mix7To1(patch[4], patch[6]);
                                    dest[destIndex + dpL + dpL + dpL] = Interpolation.Mix5To3(patch[4], patch[6]);
                                    dest[destIndex + dpL + dpL + dpL + 1] = Interpolation.Mix3To1(patch[4], patch[6]);
                                }
                                else
                                {
                                    dest[destIndex + dpL + dpL] = Interpolation.Mix3To1(patch[4], patch[3]);
                                    dest[destIndex + dpL + dpL + 1] = patch[4];
                                    dest[destIndex + dpL + dpL + dpL] = Interpolation.Mix2To1To1(patch[4], patch[7], patch[3]);
                                    dest[destIndex + dpL + dpL + dpL + 1] = Interpolation.Mix3To1(patch[4], patch[7]);
                                }
                                if (Diff(patch[5], patch[7], trY, trU, trV, trA))
                                {
                                    dest[destIndex + dpL + dpL + 2] = Interpolation.Mix7To1(patch[4], patch[8]);
                                    dest[destIndex + dpL + dpL + 3] = Interpolation.Mix3To1(patch[4], patch[8]);
                                    dest[destIndex + dpL + dpL + dpL + 2] = Interpolation.Mix3To1(patch[4], patch[8]);
                                    dest[destIndex + dpL + dpL + dpL + 3] = Interpolation.Mix5To3(patch[4], patch[8]);
                                }
                                else
                                {
                                    dest[destIndex + dpL + dpL + 2] = patch[4];
                                    dest[destIndex + dpL + dpL + 3] = Interpolation.Mix3To1(patch[4], patch[5]);
                                    dest[destIndex + dpL + dpL + dpL + 2] = Interpolation.Mix3To1(patch[4], patch[7]);
                                    dest[destIndex + dpL + dpL + dpL + 3] = Interpolation.Mix2To1To1(patch[4], patch[7], patch[5]);
                                }
                                break;
                            }
                        case 90:
                            {
                                if (Diff(patch[3], patch[1], trY, trU, trV, trA))
                                {
                                    dest[destIndex] = Interpolation.Mix5To3(patch[4], patch[0]);
                                    dest[destIndex + 1] = Interpolation.Mix3To1(patch[4], patch[0]);
                                    dest[destIndex + dpL] = Interpolation.Mix3To1(patch[4], patch[0]);
                                    dest[destIndex + dpL + 1] = Interpolation.Mix7To1(patch[4], patch[0]);
                                }
                                else
                                {
                                    dest[destIndex] = Interpolation.Mix2To1To1(patch[4], patch[1], patch[3]);
                                    dest[destIndex + 1] = Interpolation.Mix3To1(patch[4], patch[1]);
                                    dest[destIndex + dpL] = Interpolation.Mix3To1(patch[4], patch[3]);
                                    dest[destIndex + dpL + 1] = patch[4];
                                }
                                if (Diff(patch[1], patch[5], trY, trU, trV, trA))
                                {
                                    dest[destIndex + 2] = Interpolation.Mix3To1(patch[4], patch[2]);
                                    dest[destIndex + 3] = Interpolation.Mix5To3(patch[4], patch[2]);
                                    dest[destIndex + dpL + 2] = Interpolation.Mix7To1(patch[4], patch[2]);
                                    dest[destIndex + dpL + 3] = Interpolation.Mix3To1(patch[4], patch[2]);
                                }
                                else
                                {
                                    dest[destIndex + 2] = Interpolation.Mix3To1(patch[4], patch[1]);
                                    dest[destIndex + 3] = Interpolation.Mix2To1To1(patch[4], patch[1], patch[5]);
                                    dest[destIndex + dpL + 2] = patch[4];
                                    dest[destIndex + dpL + 3] = Interpolation.Mix3To1(patch[4], patch[5]);
                                }
                                if (Diff(patch[7], patch[3], trY, trU, trV, trA))
                                {
                                    dest[destIndex + dpL + dpL] = Interpolation.Mix3To1(patch[4], patch[6]);
                                    dest[destIndex + dpL + dpL + 1] = Interpolation.Mix7To1(patch[4], patch[6]);
                                    dest[destIndex + dpL + dpL + dpL] = Interpolation.Mix5To3(patch[4], patch[6]);
                                    dest[destIndex + dpL + dpL + dpL + 1] = Interpolation.Mix3To1(patch[4], patch[6]);
                                }
                                else
                                {
                                    dest[destIndex + dpL + dpL] = Interpolation.Mix3To1(patch[4], patch[3]);
                                    dest[destIndex + dpL + dpL + 1] = patch[4];
                                    dest[destIndex + dpL + dpL + dpL] = Interpolation.Mix2To1To1(patch[4], patch[7], patch[3]);
                                    dest[destIndex + dpL + dpL + dpL + 1] = Interpolation.Mix3To1(patch[4], patch[7]);
                                }
                                if (Diff(patch[5], patch[7], trY, trU, trV, trA))
                                {
                                    dest[destIndex + dpL + dpL + 2] = Interpolation.Mix7To1(patch[4], patch[8]);
                                    dest[destIndex + dpL + dpL + 3] = Interpolation.Mix3To1(patch[4], patch[8]);
                                    dest[destIndex + dpL + dpL + dpL + 2] = Interpolation.Mix3To1(patch[4], patch[8]);
                                    dest[destIndex + dpL + dpL + dpL + 3] = Interpolation.Mix5To3(patch[4], patch[8]);
                                }
                                else
                                {
                                    dest[destIndex + dpL + dpL + 2] = patch[4];
                                    dest[destIndex + dpL + dpL + 3] = Interpolation.Mix3To1(patch[4], patch[5]);
                                    dest[destIndex + dpL + dpL + dpL + 2] = Interpolation.Mix3To1(patch[4], patch[7]);
                                    dest[destIndex + dpL + dpL + dpL + 3] = Interpolation.Mix2To1To1(patch[4], patch[7], patch[5]);
                                }
                                break;
                            }
                        case 55:
                        case 23:
                            {
                                if (Diff(patch[1], patch[5], trY, trU, trV, trA))
                                {
                                    dest[destIndex] = Interpolation.Mix5To3(patch[4], patch[3]);
                                    dest[destIndex + 1] = Interpolation.Mix7To1(patch[4], patch[3]);
                                    dest[destIndex + 2] = patch[4];
                                    dest[destIndex + 3] = patch[4];
                                    dest[destIndex + dpL + 2] = patch[4];
                                    dest[destIndex + dpL + 3] = patch[4];
                                }
                                else
                                {
                                    dest[destIndex] = Interpolation.Mix3To1(patch[4], patch[1]);
                                    dest[destIndex + 1] = Interpolation.Mix3To1(patch[1], patch[4]);
                                    dest[destIndex + 2] = Interpolation.Mix5To3(patch[1], patch[5]);
                                    dest[destIndex + 3] = Interpolation.MixEven(patch[1], patch[5]);
                                    dest[destIndex + dpL + 2] = Interpolation.Mix6To1To1(patch[4], patch[5], patch[1]);
                                    dest[destIndex + dpL + 3] = Interpolation.Mix2To1To1(patch[5], patch[4], patch[1]);
                                }
                                dest[destIndex + dpL] = Interpolation.Mix5To3(patch[4], patch[3]);
                                dest[destIndex + dpL + 1] = Interpolation.Mix7To1(patch[4], patch[3]);
                                dest[destIndex + dpL + dpL] = Interpolation.Mix5To2To1(patch[4], patch[3], patch[7]);
                                dest[destIndex + dpL + dpL + 1] = Interpolation.Mix6To1To1(patch[4], patch[3], patch[7]);
                                dest[destIndex + dpL + dpL + 2] = Interpolation.Mix7To1(patch[4], patch[8]);
                                dest[destIndex + dpL + dpL + 3] = Interpolation.Mix3To1(patch[4], patch[8]);
                                dest[destIndex + dpL + dpL + dpL] = Interpolation.Mix2To1To1(patch[4], patch[7], patch[3]);
                                dest[destIndex + dpL + dpL + dpL + 1] = Interpolation.Mix5To2To1(patch[4], patch[7], patch[3]);
                                dest[destIndex + dpL + dpL + dpL + 2] = Interpolation.Mix5To2To1(patch[4], patch[7], patch[8]);
                                dest[destIndex + dpL + dpL + dpL + 3] = Interpolation.Mix5To3(patch[4], patch[8]);
                                break;
                            }
                        case 182:
                        case 150:
                            {
                                dest[destIndex] = Interpolation.Mix5To3(patch[4], patch[0]);
                                dest[destIndex + 1] = Interpolation.Mix3To1(patch[4], patch[0]);
                                if (Diff(patch[1], patch[5], trY, trU, trV, trA))
                                {
                                    dest[destIndex + 2] = patch[4];
                                    dest[destIndex + 3] = patch[4];
                                    dest[destIndex + dpL + 2] = patch[4];
                                    dest[destIndex + dpL + 3] = patch[4];
                                    dest[destIndex + dpL + dpL + 3] = Interpolation.Mix7To1(patch[4], patch[7]);
                                    dest[destIndex + dpL + dpL + dpL + 3] = Interpolation.Mix5To3(patch[4], patch[7]);
                                }
                                else
                                {
                                    dest[destIndex + 2] = Interpolation.Mix2To1To1(patch[1], patch[4], patch[5]);
                                    dest[destIndex + 3] = Interpolation.MixEven(patch[1], patch[5]);
                                    dest[destIndex + dpL + 2] = Interpolation.Mix6To1To1(patch[4], patch[5], patch[1]);
                                    dest[destIndex + dpL + 3] = Interpolation.Mix5To3(patch[5], patch[1]);
                                    dest[destIndex + dpL + dpL + 3] = Interpolation.Mix3To1(patch[5], patch[4]);
                                    dest[destIndex + dpL + dpL + dpL + 3] = Interpolation.Mix3To1(patch[4], patch[5]);
                                }
                                dest[destIndex + dpL] = Interpolation.Mix5To2To1(patch[4], patch[3], patch[0]);
                                dest[destIndex + dpL + 1] = Interpolation.Mix7To1(patch[4], patch[0]);
                                dest[destIndex + dpL + dpL] = Interpolation.Mix5To2To1(patch[4], patch[3], patch[7]);
                                dest[destIndex + dpL + dpL + 1] = Interpolation.Mix6To1To1(patch[4], patch[3], patch[7]);
                                dest[destIndex + dpL + dpL + 2] = Interpolation.Mix7To1(patch[4], patch[7]);
                                dest[destIndex + dpL + dpL + dpL] = Interpolation.Mix2To1To1(patch[4], patch[7], patch[3]);
                                dest[destIndex + dpL + dpL + dpL + 1] = Interpolation.Mix5To2To1(patch[4], patch[7], patch[3]);
                                dest[destIndex + dpL + dpL + dpL + 2] = Interpolation.Mix5To3(patch[4], patch[7]);
                                break;
                            }
                        case 213:
                        case 212:
                            {
                                dest[destIndex] = Interpolation.Mix2To1To1(patch[4], patch[1], patch[3]);
                                dest[destIndex + 1] = Interpolation.Mix5To2To1(patch[4], patch[1], patch[3]);
                                dest[destIndex + 2] = Interpolation.Mix5To3(patch[4], patch[1]);
                                if (Diff(patch[5], patch[7], trY, trU, trV, trA))
                                {
                                    dest[destIndex + 3] = Interpolation.Mix5To3(patch[4], patch[1]);
                                    dest[destIndex + dpL + 3] = Interpolation.Mix7To1(patch[4], patch[1]);
                                    dest[destIndex + dpL + dpL + 2] = patch[4];
                                    dest[destIndex + dpL + dpL + 3] = patch[4];
                                    dest[destIndex + dpL + dpL + dpL + 2] = patch[4];
                                    dest[destIndex + dpL + dpL + dpL + 3] = patch[4];
                                }
                                else
                                {
                                    dest[destIndex + 3] = Interpolation.Mix3To1(patch[4], patch[5]);
                                    dest[destIndex + dpL + 3] = Interpolation.Mix3To1(patch[5], patch[4]);
                                    dest[destIndex + dpL + dpL + 2] = Interpolation.Mix6To1To1(patch[4], patch[5], patch[7]);
                                    dest[destIndex + dpL + dpL + 3] = Interpolation.Mix5To3(patch[5], patch[7]);
                                    dest[destIndex + dpL + dpL + dpL + 2] = Interpolation.Mix2To1To1(patch[7], patch[4], patch[5]);
                                    dest[destIndex + dpL + dpL + dpL + 3] = Interpolation.MixEven(patch[7], patch[5]);
                                }
                                dest[destIndex + dpL] = Interpolation.Mix5To2To1(patch[4], patch[3], patch[1]);
                                dest[destIndex + dpL + 1] = Interpolation.Mix6To1To1(patch[4], patch[3], patch[1]);
                                dest[destIndex + dpL + 2] = Interpolation.Mix7To1(patch[4], patch[1]);
                                dest[destIndex + dpL + dpL] = Interpolation.Mix5To2To1(patch[4], patch[3], patch[6]);
                                dest[destIndex + dpL + dpL + 1] = Interpolation.Mix7To1(patch[4], patch[6]);
                                dest[destIndex + dpL + dpL + dpL] = Interpolation.Mix5To3(patch[4], patch[6]);
                                dest[destIndex + dpL + dpL + dpL + 1] = Interpolation.Mix3To1(patch[4], patch[6]);
                                break;
                            }
                        case 241:
                        case 240:
                            {
                                dest[destIndex] = Interpolation.Mix2To1To1(patch[4], patch[1], patch[3]);
                                dest[destIndex + 1] = Interpolation.Mix5To2To1(patch[4], patch[1], patch[3]);
                                dest[destIndex + 2] = Interpolation.Mix5To2To1(patch[4], patch[1], patch[2]);
                                dest[destIndex + 3] = Interpolation.Mix5To3(patch[4], patch[2]);
                                dest[destIndex + dpL] = Interpolation.Mix5To2To1(patch[4], patch[3], patch[1]);
                                dest[destIndex + dpL + 1] = Interpolation.Mix6To1To1(patch[4], patch[3], patch[1]);
                                dest[destIndex + dpL + 2] = Interpolation.Mix7To1(patch[4], patch[2]);
                                dest[destIndex + dpL + 3] = Interpolation.Mix3To1(patch[4], patch[2]);
                                dest[destIndex + dpL + dpL] = Interpolation.Mix5To3(patch[4], patch[3]);
                                dest[destIndex + dpL + dpL + 1] = Interpolation.Mix7To1(patch[4], patch[3]);
                                if (Diff(patch[5], patch[7], trY, trU, trV, trA))
                                {
                                    dest[destIndex + dpL + dpL + 2] = patch[4];
                                    dest[destIndex + dpL + dpL + 3] = patch[4];
                                    dest[destIndex + dpL + dpL + dpL] = Interpolation.Mix5To3(patch[4], patch[3]);
                                    dest[destIndex + dpL + dpL + dpL + 1] = Interpolation.Mix7To1(patch[4], patch[3]);
                                    dest[destIndex + dpL + dpL + dpL + 2] = patch[4];
                                    dest[destIndex + dpL + dpL + dpL + 3] = patch[4];
                                }
                                else
                                {
                                    dest[destIndex + dpL + dpL + 2] = Interpolation.Mix6To1To1(patch[4], patch[5], patch[7]);
                                    dest[destIndex + dpL + dpL + 3] = Interpolation.Mix2To1To1(patch[5], patch[4], patch[7]);
                                    dest[destIndex + dpL + dpL + dpL] = Interpolation.Mix3To1(patch[4], patch[7]);
                                    dest[destIndex + dpL + dpL + dpL + 1] = Interpolation.Mix3To1(patch[7], patch[4]);
                                    dest[destIndex + dpL + dpL + dpL + 2] = Interpolation.Mix5To3(patch[7], patch[5]);
                                    dest[destIndex + dpL + dpL + dpL + 3] = Interpolation.MixEven(patch[7], patch[5]);
                                }
                                break;
                            }
                        case 236:
                        case 232:
                            {
                                dest[destIndex] = Interpolation.Mix5To3(patch[4], patch[0]);
                                dest[destIndex + 1] = Interpolation.Mix5To2To1(patch[4], patch[1], patch[0]);
                                dest[destIndex + 2] = Interpolation.Mix5To2To1(patch[4], patch[1], patch[5]);
                                dest[destIndex + 3] = Interpolation.Mix2To1To1(patch[4], patch[1], patch[5]);
                                dest[destIndex + dpL] = Interpolation.Mix3To1(patch[4], patch[0]);
                                dest[destIndex + dpL + 1] = Interpolation.Mix7To1(patch[4], patch[0]);
                                dest[destIndex + dpL + 2] = Interpolation.Mix6To1To1(patch[4], patch[5], patch[1]);
                                dest[destIndex + dpL + 3] = Interpolation.Mix5To2To1(patch[4], patch[5], patch[1]);
                                if (Diff(patch[7], patch[3], trY, trU, trV, trA))
                                {
                                    dest[destIndex + dpL + dpL] = patch[4];
                                    dest[destIndex + dpL + dpL + 1] = patch[4];
                                    dest[destIndex + dpL + dpL + dpL] = patch[4];
                                    dest[destIndex + dpL + dpL + dpL + 1] = patch[4];
                                    dest[destIndex + dpL + dpL + dpL + 2] = Interpolation.Mix7To1(patch[4], patch[5]);
                                    dest[destIndex + dpL + dpL + dpL + 3] = Interpolation.Mix5To3(patch[4], patch[5]);
                                }
                                else
                                {
                                    dest[destIndex + dpL + dpL] = Interpolation.Mix2To1To1(patch[3], patch[4], patch[7]);
                                    dest[destIndex + dpL + dpL + 1] = Interpolation.Mix6To1To1(patch[4], patch[3], patch[7]);
                                    dest[destIndex + dpL + dpL + dpL] = Interpolation.MixEven(patch[7], patch[3]);
                                    dest[destIndex + dpL + dpL + dpL + 1] = Interpolation.Mix5To3(patch[7], patch[3]);
                                    dest[destIndex + dpL + dpL + dpL + 2] = Interpolation.Mix3To1(patch[7], patch[4]);
                                    dest[destIndex + dpL + dpL + dpL + 3] = Interpolation.Mix3To1(patch[4], patch[7]);
                                }
                                dest[destIndex + dpL + dpL + 2] = Interpolation.Mix7To1(patch[4], patch[5]);
                                dest[destIndex + dpL + dpL + 3] = Interpolation.Mix5To3(patch[4], patch[5]);
                                break;
                            }
                        case 109:
                        case 105:
                            {
                                if (Diff(patch[7], patch[3], trY, trU, trV, trA))
                                {
                                    dest[destIndex] = Interpolation.Mix5To3(patch[4], patch[1]);
                                    dest[destIndex + dpL] = Interpolation.Mix7To1(patch[4], patch[1]);
                                    dest[destIndex + dpL + dpL] = patch[4];
                                    dest[destIndex + dpL + dpL + 1] = patch[4];
                                    dest[destIndex + dpL + dpL + dpL] = patch[4];
                                    dest[destIndex + dpL + dpL + dpL + 1] = patch[4];
                                }
                                else
                                {
                                    dest[destIndex] = Interpolation.Mix3To1(patch[4], patch[3]);
                                    dest[destIndex + dpL] = Interpolation.Mix3To1(patch[3], patch[4]);
                                    dest[destIndex + dpL + dpL] = Interpolation.Mix5To3(patch[3], patch[7]);
                                    dest[destIndex + dpL + dpL + 1] = Interpolation.Mix6To1To1(patch[4], patch[3], patch[7]);
                                    dest[destIndex + dpL + dpL + dpL] = Interpolation.MixEven(patch[7], patch[3]);
                                    dest[destIndex + dpL + dpL + dpL + 1] = Interpolation.Mix2To1To1(patch[7], patch[4], patch[3]);
                                }
                                dest[destIndex + 1] = Interpolation.Mix5To3(patch[4], patch[1]);
                                dest[destIndex + 2] = Interpolation.Mix5To2To1(patch[4], patch[1], patch[5]);
                                dest[destIndex + 3] = Interpolation.Mix2To1To1(patch[4], patch[1], patch[5]);
                                dest[destIndex + dpL + 1] = Interpolation.Mix7To1(patch[4], patch[1]);
                                dest[destIndex + dpL + 2] = Interpolation.Mix6To1To1(patch[4], patch[5], patch[1]);
                                dest[destIndex + dpL + 3] = Interpolation.Mix5To2To1(patch[4], patch[5], patch[1]);
                                dest[destIndex + dpL + dpL + 2] = Interpolation.Mix7To1(patch[4], patch[8]);
                                dest[destIndex + dpL + dpL + 3] = Interpolation.Mix5To2To1(patch[4], patch[5], patch[8]);
                                dest[destIndex + dpL + dpL + dpL + 2] = Interpolation.Mix3To1(patch[4], patch[8]);
                                dest[destIndex + dpL + dpL + dpL + 3] = Interpolation.Mix5To3(patch[4], patch[8]);
                                break;
                            }
                        case 171:
                        case 43:
                            {
                                if (Diff(patch[3], patch[1], trY, trU, trV, trA))
                                {
                                    dest[destIndex] = patch[4];
                                    dest[destIndex + 1] = patch[4];
                                    dest[destIndex + dpL] = patch[4];
                                    dest[destIndex + dpL + 1] = patch[4];
                                    dest[destIndex + dpL + dpL] = Interpolation.Mix7To1(patch[4], patch[7]);
                                    dest[destIndex + dpL + dpL + dpL] = Interpolation.Mix5To3(patch[4], patch[7]);
                                }
                                else
                                {
                                    dest[destIndex] = Interpolation.MixEven(patch[1], patch[3]);
                                    dest[destIndex + 1] = Interpolation.Mix2To1To1(patch[1], patch[4], patch[3]);
                                    dest[destIndex + dpL] = Interpolation.Mix5To3(patch[3], patch[1]);
                                    dest[destIndex + dpL + 1] = Interpolation.Mix6To1To1(patch[4], patch[3], patch[1]);
                                    dest[destIndex + dpL + dpL] = Interpolation.Mix3To1(patch[3], patch[4]);
                                    dest[destIndex + dpL + dpL + dpL] = Interpolation.Mix3To1(patch[4], patch[3]);
                                }
                                dest[destIndex + 2] = Interpolation.Mix3To1(patch[4], patch[2]);
                                dest[destIndex + 3] = Interpolation.Mix5To3(patch[4], patch[2]);
                                dest[destIndex + dpL + 2] = Interpolation.Mix7To1(patch[4], patch[2]);
                                dest[destIndex + dpL + 3] = Interpolation.Mix5To2To1(patch[4], patch[5], patch[2]);
                                dest[destIndex + dpL + dpL + 1] = Interpolation.Mix7To1(patch[4], patch[7]);
                                dest[destIndex + dpL + dpL + 2] = Interpolation.Mix6To1To1(patch[4], patch[5], patch[7]);
                                dest[destIndex + dpL + dpL + 3] = Interpolation.Mix5To2To1(patch[4], patch[5], patch[7]);
                                dest[destIndex + dpL + dpL + dpL + 1] = Interpolation.Mix5To3(patch[4], patch[7]);
                                dest[destIndex + dpL + dpL + dpL + 2] = Interpolation.Mix5To2To1(patch[4], patch[7], patch[5]);
                                dest[destIndex + dpL + dpL + dpL + 3] = Interpolation.Mix2To1To1(patch[4], patch[7], patch[5]);
                                break;
                            }
                        case 143:
                        case 15:
                            {
                                if (Diff(patch[3], patch[1], trY, trU, trV, trA))
                                {
                                    dest[destIndex] = patch[4];
                                    dest[destIndex + 1] = patch[4];
                                    dest[destIndex + 2] = Interpolation.Mix7To1(patch[4], patch[5]);
                                    dest[destIndex + 3] = Interpolation.Mix5To3(patch[4], patch[5]);
                                    dest[destIndex + dpL] = patch[4];
                                    dest[destIndex + dpL + 1] = patch[4];
                                }
                                else
                                {
                                    dest[destIndex] = Interpolation.MixEven(patch[1], patch[3]);
                                    dest[destIndex + 1] = Interpolation.Mix5To3(patch[1], patch[3]);
                                    dest[destIndex + 2] = Interpolation.Mix3To1(patch[1], patch[4]);
                                    dest[destIndex + 3] = Interpolation.Mix3To1(patch[4], patch[1]);
                                    dest[destIndex + dpL] = Interpolation.Mix2To1To1(patch[3], patch[4], patch[1]);
                                    dest[destIndex + dpL + 1] = Interpolation.Mix6To1To1(patch[4], patch[3], patch[1]);
                                }
                                dest[destIndex + dpL + 2] = Interpolation.Mix7To1(patch[4], patch[5]);
                                dest[destIndex + dpL + 3] = Interpolation.Mix5To3(patch[4], patch[5]);
                                dest[destIndex + dpL + dpL] = Interpolation.Mix3To1(patch[4], patch[6]);
                                dest[destIndex + dpL + dpL + 1] = Interpolation.Mix7To1(patch[4], patch[6]);
                                dest[destIndex + dpL + dpL + 2] = Interpolation.Mix6To1To1(patch[4], patch[5], patch[7]);
                                dest[destIndex + dpL + dpL + 3] = Interpolation.Mix5To2To1(patch[4], patch[5], patch[7]);
                                dest[destIndex + dpL + dpL + dpL] = Interpolation.Mix5To3(patch[4], patch[6]);
                                dest[destIndex + dpL + dpL + dpL + 1] = Interpolation.Mix5To2To1(patch[4], patch[7], patch[6]);
                                dest[destIndex + dpL + dpL + dpL + 2] = Interpolation.Mix5To2To1(patch[4], patch[7], patch[5]);
                                dest[destIndex + dpL + dpL + dpL + 3] = Interpolation.Mix2To1To1(patch[4], patch[7], patch[5]);
                                break;
                            }
                        case 124:
                            {
                                dest[destIndex] = Interpolation.Mix5To3(patch[4], patch[0]);
                                dest[destIndex + 1] = Interpolation.Mix5To2To1(patch[4], patch[1], patch[0]);
                                dest[destIndex + 2] = Interpolation.Mix5To3(patch[4], patch[1]);
                                dest[destIndex + 3] = Interpolation.Mix5To3(patch[4], patch[1]);
                                dest[destIndex + dpL] = Interpolation.Mix3To1(patch[4], patch[0]);
                                dest[destIndex + dpL + 1] = Interpolation.Mix7To1(patch[4], patch[0]);
                                dest[destIndex + dpL + 2] = Interpolation.Mix7To1(patch[4], patch[1]);
                                dest[destIndex + dpL + 3] = Interpolation.Mix7To1(patch[4], patch[1]);
                                if (Diff(patch[7], patch[3], trY, trU, trV, trA))
                                {
                                    dest[destIndex + dpL + dpL] = patch[4];
                                    dest[destIndex + dpL + dpL + dpL] = patch[4];
                                    dest[destIndex + dpL + dpL + dpL + 1] = patch[4];
                                }
                                else
                                {
                                    dest[destIndex + dpL + dpL] = Interpolation.MixEven(patch[3], patch[4]);
                                    dest[destIndex + dpL + dpL + dpL] = Interpolation.MixEven(patch[7], patch[3]);
                                    dest[destIndex + dpL + dpL + dpL + 1] = Interpolation.MixEven(patch[7], patch[4]);
                                }
                                dest[destIndex + dpL + dpL + 1] = patch[4];
                                dest[destIndex + dpL + dpL + 2] = Interpolation.Mix7To1(patch[4], patch[8]);
                                dest[destIndex + dpL + dpL + 3] = Interpolation.Mix3To1(patch[4], patch[8]);
                                dest[destIndex + dpL + dpL + dpL + 2] = Interpolation.Mix3To1(patch[4], patch[8]);
                                dest[destIndex + dpL + dpL + dpL + 3] = Interpolation.Mix5To3(patch[4], patch[8]);
                                break;
                            }
                        case 203:
                            {
                                if (Diff(patch[3], patch[1], trY, trU, trV, trA))
                                {
                                    dest[destIndex] = patch[4];
                                    dest[destIndex + 1] = patch[4];
                                    dest[destIndex + dpL] = patch[4];
                                }
                                else
                                {
                                    dest[destIndex] = Interpolation.MixEven(patch[1], patch[3]);
                                    dest[destIndex + 1] = Interpolation.MixEven(patch[1], patch[4]);
                                    dest[destIndex + dpL] = Interpolation.MixEven(patch[3], patch[4]);
                                }
                                dest[destIndex + 2] = Interpolation.Mix3To1(patch[4], patch[2]);
                                dest[destIndex + 3] = Interpolation.Mix5To3(patch[4], patch[2]);
                                dest[destIndex + dpL + 1] = patch[4];
                                dest[destIndex + dpL + 2] = Interpolation.Mix7To1(patch[4], patch[2]);
                                dest[destIndex + dpL + 3] = Interpolation.Mix5To2To1(patch[4], patch[5], patch[2]);
                                dest[destIndex + dpL + dpL] = Interpolation.Mix3To1(patch[4], patch[6]);
                                dest[destIndex + dpL + dpL + 1] = Interpolation.Mix7To1(patch[4], patch[6]);
                                dest[destIndex + dpL + dpL + 2] = Interpolation.Mix7To1(patch[4], patch[5]);
                                dest[destIndex + dpL + dpL + 3] = Interpolation.Mix5To3(patch[4], patch[5]);
                                dest[destIndex + dpL + dpL + dpL] = Interpolation.Mix5To3(patch[4], patch[6]);
                                dest[destIndex + dpL + dpL + dpL + 1] = Interpolation.Mix3To1(patch[4], patch[6]);
                                dest[destIndex + dpL + dpL + dpL + 2] = Interpolation.Mix7To1(patch[4], patch[5]);
                                dest[destIndex + dpL + dpL + dpL + 3] = Interpolation.Mix5To3(patch[4], patch[5]);
                                break;
                            }
                        case 62:
                            {
                                dest[destIndex] = Interpolation.Mix5To3(patch[4], patch[0]);
                                dest[destIndex + 1] = Interpolation.Mix3To1(patch[4], patch[0]);
                                if (Diff(patch[1], patch[5], trY, trU, trV, trA))
                                {
                                    dest[destIndex + 2] = patch[4];
                                    dest[destIndex + 3] = patch[4];
                                    dest[destIndex + dpL + 3] = patch[4];
                                }
                                else
                                {
                                    dest[destIndex + 2] = Interpolation.MixEven(patch[1], patch[4]);
                                    dest[destIndex + 3] = Interpolation.MixEven(patch[1], patch[5]);
                                    dest[destIndex + dpL + 3] = Interpolation.MixEven(patch[5], patch[4]);
                                }
                                dest[destIndex + dpL] = Interpolation.Mix3To1(patch[4], patch[0]);
                                dest[destIndex + dpL + 1] = Interpolation.Mix7To1(patch[4], patch[0]);
                                dest[destIndex + dpL + 2] = patch[4];
                                dest[destIndex + dpL + dpL] = Interpolation.Mix7To1(patch[4], patch[7]);
                                dest[destIndex + dpL + dpL + 1] = Interpolation.Mix7To1(patch[4], patch[7]);
                                dest[destIndex + dpL + dpL + 2] = Interpolation.Mix7To1(patch[4], patch[8]);
                                dest[destIndex + dpL + dpL + 3] = Interpolation.Mix3To1(patch[4], patch[8]);
                                dest[destIndex + dpL + dpL + dpL] = Interpolation.Mix5To3(patch[4], patch[7]);
                                dest[destIndex + dpL + dpL + dpL + 1] = Interpolation.Mix5To3(patch[4], patch[7]);
                                dest[destIndex + dpL + dpL + dpL + 2] = Interpolation.Mix5To2To1(patch[4], patch[7], patch[8]);
                                dest[destIndex + dpL + dpL + dpL + 3] = Interpolation.Mix5To3(patch[4], patch[8]);
                                break;
                            }
                        case 211:
                            {
                                dest[destIndex] = Interpolation.Mix5To3(patch[4], patch[3]);
                                dest[destIndex + 1] = Interpolation.Mix7To1(patch[4], patch[3]);
                                dest[destIndex + 2] = Interpolation.Mix3To1(patch[4], patch[2]);
                                dest[destIndex + 3] = Interpolation.Mix5To3(patch[4], patch[2]);
                                dest[destIndex + dpL] = Interpolation.Mix5To3(patch[4], patch[3]);
                                dest[destIndex + dpL + 1] = Interpolation.Mix7To1(patch[4], patch[3]);
                                dest[destIndex + dpL + 2] = Interpolation.Mix7To1(patch[4], patch[2]);
                                dest[destIndex + dpL + 3] = Interpolation.Mix3To1(patch[4], patch[2]);
                                dest[destIndex + dpL + dpL] = Interpolation.Mix5To2To1(patch[4], patch[3], patch[6]);
                                dest[destIndex + dpL + dpL + 1] = Interpolation.Mix7To1(patch[4], patch[6]);
                                dest[destIndex + dpL + dpL + 2] = patch[4];
                                if (Diff(patch[5], patch[7], trY, trU, trV, trA))
                                {
                                    dest[destIndex + dpL + dpL + 3] = patch[4];
                                    dest[destIndex + dpL + dpL + dpL + 2] = patch[4];
                                    dest[destIndex + dpL + dpL + dpL + 3] = patch[4];
                                }
                                else
                                {
                                    dest[destIndex + dpL + dpL + 3] = Interpolation.MixEven(patch[5], patch[4]);
                                    dest[destIndex + dpL + dpL + dpL + 2] = Interpolation.MixEven(patch[7], patch[4]);
                                    dest[destIndex + dpL + dpL + dpL + 3] = Interpolation.MixEven(patch[7], patch[5]);
                                }
                                dest[destIndex + dpL + dpL + dpL] = Interpolation.Mix5To3(patch[4], patch[6]);
                                dest[destIndex + dpL + dpL + dpL + 1] = Interpolation.Mix3To1(patch[4], patch[6]);
                                break;
                            }
                        case 118:
                            {
                                dest[destIndex] = Interpolation.Mix5To3(patch[4], patch[0]);
                                dest[destIndex + 1] = Interpolation.Mix3To1(patch[4], patch[0]);
                                if (Diff(patch[1], patch[5], trY, trU, trV, trA))
                                {
                                    dest[destIndex + 2] = patch[4];
                                    dest[destIndex + 3] = patch[4];
                                    dest[destIndex + dpL + 3] = patch[4];
                                }
                                else
                                {
                                    dest[destIndex + 2] = Interpolation.MixEven(patch[1], patch[4]);
                                    dest[destIndex + 3] = Interpolation.MixEven(patch[1], patch[5]);
                                    dest[destIndex + dpL + 3] = Interpolation.MixEven(patch[5], patch[4]);
                                }
                                dest[destIndex + dpL] = Interpolation.Mix5To2To1(patch[4], patch[3], patch[0]);
                                dest[destIndex + dpL + 1] = Interpolation.Mix7To1(patch[4], patch[0]);
                                dest[destIndex + dpL + 2] = patch[4];
                                dest[destIndex + dpL + dpL] = Interpolation.Mix5To3(patch[4], patch[3]);
                                dest[destIndex + dpL + dpL + 1] = Interpolation.Mix7To1(patch[4], patch[3]);
                                dest[destIndex + dpL + dpL + 2] = Interpolation.Mix7To1(patch[4], patch[8]);
                                dest[destIndex + dpL + dpL + 3] = Interpolation.Mix3To1(patch[4], patch[8]);
                                dest[destIndex + dpL + dpL + dpL] = Interpolation.Mix5To3(patch[4], patch[3]);
                                dest[destIndex + dpL + dpL + dpL + 1] = Interpolation.Mix7To1(patch[4], patch[3]);
                                dest[destIndex + dpL + dpL + dpL + 2] = Interpolation.Mix3To1(patch[4], patch[8]);
                                dest[destIndex + dpL + dpL + dpL + 3] = Interpolation.Mix5To3(patch[4], patch[8]);
                                break;
                            }
                        case 217:
                            {
                                dest[destIndex] = Interpolation.Mix5To3(patch[4], patch[1]);
                                dest[destIndex + 1] = Interpolation.Mix5To3(patch[4], patch[1]);
                                dest[destIndex + 2] = Interpolation.Mix5To2To1(patch[4], patch[1], patch[2]);
                                dest[destIndex + 3] = Interpolation.Mix5To3(patch[4], patch[2]);
                                dest[destIndex + dpL] = Interpolation.Mix7To1(patch[4], patch[1]);
                                dest[destIndex + dpL + 1] = Interpolation.Mix7To1(patch[4], patch[1]);
                                dest[destIndex + dpL + 2] = Interpolation.Mix7To1(patch[4], patch[2]);
                                dest[destIndex + dpL + 3] = Interpolation.Mix3To1(patch[4], patch[2]);
                                dest[destIndex + dpL + dpL] = Interpolation.Mix3To1(patch[4], patch[6]);
                                dest[destIndex + dpL + dpL + 1] = Interpolation.Mix7To1(patch[4], patch[6]);
                                dest[destIndex + dpL + dpL + 2] = patch[4];
                                if (Diff(patch[5], patch[7], trY, trU, trV, trA))
                                {
                                    dest[destIndex + dpL + dpL + 3] = patch[4];
                                    dest[destIndex + dpL + dpL + dpL + 2] = patch[4];
                                    dest[destIndex + dpL + dpL + dpL + 3] = patch[4];
                                }
                                else
                                {
                                    dest[destIndex + dpL + dpL + 3] = Interpolation.MixEven(patch[5], patch[4]);
                                    dest[destIndex + dpL + dpL + dpL + 2] = Interpolation.MixEven(patch[7], patch[4]);
                                    dest[destIndex + dpL + dpL + dpL + 3] = Interpolation.MixEven(patch[7], patch[5]);
                                }
                                dest[destIndex + dpL + dpL + dpL] = Interpolation.Mix5To3(patch[4], patch[6]);
                                dest[destIndex + dpL + dpL + dpL + 1] = Interpolation.Mix3To1(patch[4], patch[6]);
                                break;
                            }
                        case 110:
                            {
                                dest[destIndex] = Interpolation.Mix5To3(patch[4], patch[0]);
                                dest[destIndex + 1] = Interpolation.Mix3To1(patch[4], patch[0]);
                                dest[destIndex + 2] = Interpolation.Mix7To1(patch[4], patch[5]);
                                dest[destIndex + 3] = Interpolation.Mix5To3(patch[4], patch[5]);
                                dest[destIndex + dpL] = Interpolation.Mix3To1(patch[4], patch[0]);
                                dest[destIndex + dpL + 1] = Interpolation.Mix7To1(patch[4], patch[0]);
                                dest[destIndex + dpL + 2] = Interpolation.Mix7To1(patch[4], patch[5]);
                                dest[destIndex + dpL + 3] = Interpolation.Mix5To3(patch[4], patch[5]);
                                if (Diff(patch[7], patch[3], trY, trU, trV, trA))
                                {
                                    dest[destIndex + dpL + dpL] = patch[4];
                                    dest[destIndex + dpL + dpL + dpL] = patch[4];
                                    dest[destIndex + dpL + dpL + dpL + 1] = patch[4];
                                }
                                else
                                {
                                    dest[destIndex + dpL + dpL] = Interpolation.MixEven(patch[3], patch[4]);
                                    dest[destIndex + dpL + dpL + dpL] = Interpolation.MixEven(patch[7], patch[3]);
                                    dest[destIndex + dpL + dpL + dpL + 1] = Interpolation.MixEven(patch[7], patch[4]);
                                }
                                dest[destIndex + dpL + dpL + 1] = patch[4];
                                dest[destIndex + dpL + dpL + 2] = Interpolation.Mix7To1(patch[4], patch[8]);
                                dest[destIndex + dpL + dpL + 3] = Interpolation.Mix5To2To1(patch[4], patch[5], patch[8]);
                                dest[destIndex + dpL + dpL + dpL + 2] = Interpolation.Mix3To1(patch[4], patch[8]);
                                dest[destIndex + dpL + dpL + dpL + 3] = Interpolation.Mix5To3(patch[4], patch[8]);
                                break;
                            }
                        case 155:
                            {
                                if (Diff(patch[3], patch[1], trY, trU, trV, trA))
                                {
                                    dest[destIndex] = patch[4];
                                    dest[destIndex + 1] = patch[4];
                                    dest[destIndex + dpL] = patch[4];
                                }
                                else
                                {
                                    dest[destIndex] = Interpolation.MixEven(patch[1], patch[3]);
                                    dest[destIndex + 1] = Interpolation.MixEven(patch[1], patch[4]);
                                    dest[destIndex + dpL] = Interpolation.MixEven(patch[3], patch[4]);
                                }
                                dest[destIndex + 2] = Interpolation.Mix3To1(patch[4], patch[2]);
                                dest[destIndex + 3] = Interpolation.Mix5To3(patch[4], patch[2]);
                                dest[destIndex + dpL + 1] = patch[4];
                                dest[destIndex + dpL + 2] = Interpolation.Mix7To1(patch[4], patch[2]);
                                dest[destIndex + dpL + 3] = Interpolation.Mix3To1(patch[4], patch[2]);
                                dest[destIndex + dpL + dpL] = Interpolation.Mix3To1(patch[4], patch[6]);
                                dest[destIndex + dpL + dpL + 1] = Interpolation.Mix7To1(patch[4], patch[6]);
                                dest[destIndex + dpL + dpL + 2] = Interpolation.Mix7To1(patch[4], patch[7]);
                                dest[destIndex + dpL + dpL + 3] = Interpolation.Mix7To1(patch[4], patch[7]);
                                dest[destIndex + dpL + dpL + dpL] = Interpolation.Mix5To3(patch[4], patch[6]);
                                dest[destIndex + dpL + dpL + dpL + 1] = Interpolation.Mix5To2To1(patch[4], patch[7], patch[6]);
                                dest[destIndex + dpL + dpL + dpL + 2] = Interpolation.Mix5To3(patch[4], patch[7]);
                                dest[destIndex + dpL + dpL + dpL + 3] = Interpolation.Mix5To3(patch[4], patch[7]);
                                break;
                            }
                        case 188:
                            {
                                dest[destIndex] = Interpolation.Mix5To3(patch[4], patch[0]);
                                dest[destIndex + 1] = Interpolation.Mix5To2To1(patch[4], patch[1], patch[0]);
                                dest[destIndex + 2] = Interpolation.Mix5To3(patch[4], patch[1]);
                                dest[destIndex + 3] = Interpolation.Mix5To3(patch[4], patch[1]);
                                dest[destIndex + dpL] = Interpolation.Mix3To1(patch[4], patch[0]);
                                dest[destIndex + dpL + 1] = Interpolation.Mix7To1(patch[4], patch[0]);
                                dest[destIndex + dpL + 2] = Interpolation.Mix7To1(patch[4], patch[1]);
                                dest[destIndex + dpL + 3] = Interpolation.Mix7To1(patch[4], patch[1]);
                                dest[destIndex + dpL + dpL] = Interpolation.Mix7To1(patch[4], patch[7]);
                                dest[destIndex + dpL + dpL + 1] = Interpolation.Mix7To1(patch[4], patch[7]);
                                dest[destIndex + dpL + dpL + 2] = Interpolation.Mix7To1(patch[4], patch[7]);
                                dest[destIndex + dpL + dpL + 3] = Interpolation.Mix7To1(patch[4], patch[7]);
                                dest[destIndex + dpL + dpL + dpL] = Interpolation.Mix5To3(patch[4], patch[7]);
                                dest[destIndex + dpL + dpL + dpL + 1] = Interpolation.Mix5To3(patch[4], patch[7]);
                                dest[destIndex + dpL + dpL + dpL + 2] = Interpolation.Mix5To3(patch[4], patch[7]);
                                dest[destIndex + dpL + dpL + dpL + 3] = Interpolation.Mix5To3(patch[4], patch[7]);
                                break;
                            }
                        case 185:
                            {
                                dest[destIndex] = Interpolation.Mix5To3(patch[4], patch[1]);
                                dest[destIndex + 1] = Interpolation.Mix5To3(patch[4], patch[1]);
                                dest[destIndex + 2] = Interpolation.Mix5To2To1(patch[4], patch[1], patch[2]);
                                dest[destIndex + 3] = Interpolation.Mix5To3(patch[4], patch[2]);
                                dest[destIndex + dpL] = Interpolation.Mix7To1(patch[4], patch[1]);
                                dest[destIndex + dpL + 1] = Interpolation.Mix7To1(patch[4], patch[1]);
                                dest[destIndex + dpL + 2] = Interpolation.Mix7To1(patch[4], patch[2]);
                                dest[destIndex + dpL + 3] = Interpolation.Mix3To1(patch[4], patch[2]);
                                dest[destIndex + dpL + dpL] = Interpolation.Mix7To1(patch[4], patch[7]);
                                dest[destIndex + dpL + dpL + 1] = Interpolation.Mix7To1(patch[4], patch[7]);
                                dest[destIndex + dpL + dpL + 2] = Interpolation.Mix7To1(patch[4], patch[7]);
                                dest[destIndex + dpL + dpL + 3] = Interpolation.Mix7To1(patch[4], patch[7]);
                                dest[destIndex + dpL + dpL + dpL] = Interpolation.Mix5To3(patch[4], patch[7]);
                                dest[destIndex + dpL + dpL + dpL + 1] = Interpolation.Mix5To3(patch[4], patch[7]);
                                dest[destIndex + dpL + dpL + dpL + 2] = Interpolation.Mix5To3(patch[4], patch[7]);
                                dest[destIndex + dpL + dpL + dpL + 3] = Interpolation.Mix5To3(patch[4], patch[7]);
                                break;
                            }
                        case 61:
                            {
                                dest[destIndex] = Interpolation.Mix5To3(patch[4], patch[1]);
                                dest[destIndex + 1] = Interpolation.Mix5To3(patch[4], patch[1]);
                                dest[destIndex + 2] = Interpolation.Mix5To3(patch[4], patch[1]);
                                dest[destIndex + 3] = Interpolation.Mix5To3(patch[4], patch[1]);
                                dest[destIndex + dpL] = Interpolation.Mix7To1(patch[4], patch[1]);
                                dest[destIndex + dpL + 1] = Interpolation.Mix7To1(patch[4], patch[1]);
                                dest[destIndex + dpL + 2] = Interpolation.Mix7To1(patch[4], patch[1]);
                                dest[destIndex + dpL + 3] = Interpolation.Mix7To1(patch[4], patch[1]);
                                dest[destIndex + dpL + dpL] = Interpolation.Mix7To1(patch[4], patch[7]);
                                dest[destIndex + dpL + dpL + 1] = Interpolation.Mix7To1(patch[4], patch[7]);
                                dest[destIndex + dpL + dpL + 2] = Interpolation.Mix7To1(patch[4], patch[8]);
                                dest[destIndex + dpL + dpL + 3] = Interpolation.Mix3To1(patch[4], patch[8]);
                                dest[destIndex + dpL + dpL + dpL] = Interpolation.Mix5To3(patch[4], patch[7]);
                                dest[destIndex + dpL + dpL + dpL + 1] = Interpolation.Mix5To3(patch[4], patch[7]);
                                dest[destIndex + dpL + dpL + dpL + 2] = Interpolation.Mix5To2To1(patch[4], patch[7], patch[8]);
                                dest[destIndex + dpL + dpL + dpL + 3] = Interpolation.Mix5To3(patch[4], patch[8]);
                                break;
                            }
                        case 157:
                            {
                                dest[destIndex] = Interpolation.Mix5To3(patch[4], patch[1]);
                                dest[destIndex + 1] = Interpolation.Mix5To3(patch[4], patch[1]);
                                dest[destIndex + 2] = Interpolation.Mix5To3(patch[4], patch[1]);
                                dest[destIndex + 3] = Interpolation.Mix5To3(patch[4], patch[1]);
                                dest[destIndex + dpL] = Interpolation.Mix7To1(patch[4], patch[1]);
                                dest[destIndex + dpL + 1] = Interpolation.Mix7To1(patch[4], patch[1]);
                                dest[destIndex + dpL + 2] = Interpolation.Mix7To1(patch[4], patch[1]);
                                dest[destIndex + dpL + 3] = Interpolation.Mix7To1(patch[4], patch[1]);
                                dest[destIndex + dpL + dpL] = Interpolation.Mix3To1(patch[4], patch[6]);
                                dest[destIndex + dpL + dpL + 1] = Interpolation.Mix7To1(patch[4], patch[6]);
                                dest[destIndex + dpL + dpL + 2] = Interpolation.Mix7To1(patch[4], patch[7]);
                                dest[destIndex + dpL + dpL + 3] = Interpolation.Mix7To1(patch[4], patch[7]);
                                dest[destIndex + dpL + dpL + dpL] = Interpolation.Mix5To3(patch[4], patch[6]);
                                dest[destIndex + dpL + dpL + dpL + 1] = Interpolation.Mix5To2To1(patch[4], patch[7], patch[6]);
                                dest[destIndex + dpL + dpL + dpL + 2] = Interpolation.Mix5To3(patch[4], patch[7]);
                                dest[destIndex + dpL + dpL + dpL + 3] = Interpolation.Mix5To3(patch[4], patch[7]);
                                break;
                            }
                        case 103:
                            {
                                dest[destIndex] = Interpolation.Mix5To3(patch[4], patch[3]);
                                dest[destIndex + 1] = Interpolation.Mix7To1(patch[4], patch[3]);
                                dest[destIndex + 2] = Interpolation.Mix7To1(patch[4], patch[5]);
                                dest[destIndex + 3] = Interpolation.Mix5To3(patch[4], patch[5]);
                                dest[destIndex + dpL] = Interpolation.Mix5To3(patch[4], patch[3]);
                                dest[destIndex + dpL + 1] = Interpolation.Mix7To1(patch[4], patch[3]);
                                dest[destIndex + dpL + 2] = Interpolation.Mix7To1(patch[4], patch[5]);
                                dest[destIndex + dpL + 3] = Interpolation.Mix5To3(patch[4], patch[5]);
                                dest[destIndex + dpL + dpL] = Interpolation.Mix5To3(patch[4], patch[3]);
                                dest[destIndex + dpL + dpL + 1] = Interpolation.Mix7To1(patch[4], patch[3]);
                                dest[destIndex + dpL + dpL + 2] = Interpolation.Mix7To1(patch[4], patch[8]);
                                dest[destIndex + dpL + dpL + 3] = Interpolation.Mix5To2To1(patch[4], patch[5], patch[8]);
                                dest[destIndex + dpL + dpL + dpL] = Interpolation.Mix5To3(patch[4], patch[3]);
                                dest[destIndex + dpL + dpL + dpL + 1] = Interpolation.Mix7To1(patch[4], patch[3]);
                                dest[destIndex + dpL + dpL + dpL + 2] = Interpolation.Mix3To1(patch[4], patch[8]);
                                dest[destIndex + dpL + dpL + dpL + 3] = Interpolation.Mix5To3(patch[4], patch[8]);
                                break;
                            }
                        case 227:
                            {
                                dest[destIndex] = Interpolation.Mix5To3(patch[4], patch[3]);
                                dest[destIndex + 1] = Interpolation.Mix7To1(patch[4], patch[3]);
                                dest[destIndex + 2] = Interpolation.Mix3To1(patch[4], patch[2]);
                                dest[destIndex + 3] = Interpolation.Mix5To3(patch[4], patch[2]);
                                dest[destIndex + dpL] = Interpolation.Mix5To3(patch[4], patch[3]);
                                dest[destIndex + dpL + 1] = Interpolation.Mix7To1(patch[4], patch[3]);
                                dest[destIndex + dpL + 2] = Interpolation.Mix7To1(patch[4], patch[2]);
                                dest[destIndex + dpL + 3] = Interpolation.Mix5To2To1(patch[4], patch[5], patch[2]);
                                dest[destIndex + dpL + dpL] = Interpolation.Mix5To3(patch[4], patch[3]);
                                dest[destIndex + dpL + dpL + 1] = Interpolation.Mix7To1(patch[4], patch[3]);
                                dest[destIndex + dpL + dpL + 2] = Interpolation.Mix7To1(patch[4], patch[5]);
                                dest[destIndex + dpL + dpL + 3] = Interpolation.Mix5To3(patch[4], patch[5]);
                                dest[destIndex + dpL + dpL + dpL] = Interpolation.Mix5To3(patch[4], patch[3]);
                                dest[destIndex + dpL + dpL + dpL + 1] = Interpolation.Mix7To1(patch[4], patch[3]);
                                dest[destIndex + dpL + dpL + dpL + 2] = Interpolation.Mix7To1(patch[4], patch[5]);
                                dest[destIndex + dpL + dpL + dpL + 3] = Interpolation.Mix5To3(patch[4], patch[5]);
                                break;
                            }
                        case 230:
                            {
                                dest[destIndex] = Interpolation.Mix5To3(patch[4], patch[0]);
                                dest[destIndex + 1] = Interpolation.Mix3To1(patch[4], patch[0]);
                                dest[destIndex + 2] = Interpolation.Mix7To1(patch[4], patch[5]);
                                dest[destIndex + 3] = Interpolation.Mix5To3(patch[4], patch[5]);
                                dest[destIndex + dpL] = Interpolation.Mix5To2To1(patch[4], patch[3], patch[0]);
                                dest[destIndex + dpL + 1] = Interpolation.Mix7To1(patch[4], patch[0]);
                                dest[destIndex + dpL + 2] = Interpolation.Mix7To1(patch[4], patch[5]);
                                dest[destIndex + dpL + 3] = Interpolation.Mix5To3(patch[4], patch[5]);
                                dest[destIndex + dpL + dpL] = Interpolation.Mix5To3(patch[4], patch[3]);
                                dest[destIndex + dpL + dpL + 1] = Interpolation.Mix7To1(patch[4], patch[3]);
                                dest[destIndex + dpL + dpL + 2] = Interpolation.Mix7To1(patch[4], patch[5]);
                                dest[destIndex + dpL + dpL + 3] = Interpolation.Mix5To3(patch[4], patch[5]);
                                dest[destIndex + dpL + dpL + dpL] = Interpolation.Mix5To3(patch[4], patch[3]);
                                dest[destIndex + dpL + dpL + dpL + 1] = Interpolation.Mix7To1(patch[4], patch[3]);
                                dest[destIndex + dpL + dpL + dpL + 2] = Interpolation.Mix7To1(patch[4], patch[5]);
                                dest[destIndex + dpL + dpL + dpL + 3] = Interpolation.Mix5To3(patch[4], patch[5]);
                                break;
                            }
                        case 199:
                            {
                                dest[destIndex] = Interpolation.Mix5To3(patch[4], patch[3]);
                                dest[destIndex + 1] = Interpolation.Mix7To1(patch[4], patch[3]);
                                dest[destIndex + 2] = Interpolation.Mix7To1(patch[4], patch[5]);
                                dest[destIndex + 3] = Interpolation.Mix5To3(patch[4], patch[5]);
                                dest[destIndex + dpL] = Interpolation.Mix5To3(patch[4], patch[3]);
                                dest[destIndex + dpL + 1] = Interpolation.Mix7To1(patch[4], patch[3]);
                                dest[destIndex + dpL + 2] = Interpolation.Mix7To1(patch[4], patch[5]);
                                dest[destIndex + dpL + 3] = Interpolation.Mix5To3(patch[4], patch[5]);
                                dest[destIndex + dpL + dpL] = Interpolation.Mix5To2To1(patch[4], patch[3], patch[6]);
                                dest[destIndex + dpL + dpL + 1] = Interpolation.Mix7To1(patch[4], patch[6]);
                                dest[destIndex + dpL + dpL + 2] = Interpolation.Mix7To1(patch[4], patch[5]);
                                dest[destIndex + dpL + dpL + 3] = Interpolation.Mix5To3(patch[4], patch[5]);
                                dest[destIndex + dpL + dpL + dpL] = Interpolation.Mix5To3(patch[4], patch[6]);
                                dest[destIndex + dpL + dpL + dpL + 1] = Interpolation.Mix3To1(patch[4], patch[6]);
                                dest[destIndex + dpL + dpL + dpL + 2] = Interpolation.Mix7To1(patch[4], patch[5]);
                                dest[destIndex + dpL + dpL + dpL + 3] = Interpolation.Mix5To3(patch[4], patch[5]);
                                break;
                            }
                        case 220:
                            {
                                dest[destIndex] = Interpolation.Mix5To3(patch[4], patch[0]);
                                dest[destIndex + 1] = Interpolation.Mix5To2To1(patch[4], patch[1], patch[0]);
                                dest[destIndex + 2] = Interpolation.Mix5To3(patch[4], patch[1]);
                                dest[destIndex + 3] = Interpolation.Mix5To3(patch[4], patch[1]);
                                dest[destIndex + dpL] = Interpolation.Mix3To1(patch[4], patch[0]);
                                dest[destIndex + dpL + 1] = Interpolation.Mix7To1(patch[4], patch[0]);
                                dest[destIndex + dpL + 2] = Interpolation.Mix7To1(patch[4], patch[1]);
                                dest[destIndex + dpL + 3] = Interpolation.Mix7To1(patch[4], patch[1]);
                                if (Diff(patch[7], patch[3], trY, trU, trV, trA))
                                {
                                    dest[destIndex + dpL + dpL] = Interpolation.Mix3To1(patch[4], patch[6]);
                                    dest[destIndex + dpL + dpL + 1] = Interpolation.Mix7To1(patch[4], patch[6]);
                                    dest[destIndex + dpL + dpL + dpL] = Interpolation.Mix5To3(patch[4], patch[6]);
                                    dest[destIndex + dpL + dpL + dpL + 1] = Interpolation.Mix3To1(patch[4], patch[6]);
                                }
                                else
                                {
                                    dest[destIndex + dpL + dpL] = Interpolation.Mix3To1(patch[4], patch[3]);
                                    dest[destIndex + dpL + dpL + 1] = patch[4];
                                    dest[destIndex + dpL + dpL + dpL] = Interpolation.Mix2To1To1(patch[4], patch[7], patch[3]);
                                    dest[destIndex + dpL + dpL + dpL + 1] = Interpolation.Mix3To1(patch[4], patch[7]);
                                }
                                dest[destIndex + dpL + dpL + 2] = patch[4];
                                if (Diff(patch[5], patch[7], trY, trU, trV, trA))
                                {
                                    dest[destIndex + dpL + dpL + 3] = patch[4];
                                    dest[destIndex + dpL + dpL + dpL + 2] = patch[4];
                                    dest[destIndex + dpL + dpL + dpL + 3] = patch[4];
                                }
                                else
                                {
                                    dest[destIndex + dpL + dpL + 3] = Interpolation.MixEven(patch[5], patch[4]);
                                    dest[destIndex + dpL + dpL + dpL + 2] = Interpolation.MixEven(patch[7], patch[4]);
                                    dest[destIndex + dpL + dpL + dpL + 3] = Interpolation.MixEven(patch[7], patch[5]);
                                }
                                break;
                            }
                        case 158:
                            {
                                if (Diff(patch[3], patch[1], trY, trU, trV, trA))
                                {
                                    dest[destIndex] = Interpolation.Mix5To3(patch[4], patch[0]);
                                    dest[destIndex + 1] = Interpolation.Mix3To1(patch[4], patch[0]);
                                    dest[destIndex + dpL] = Interpolation.Mix3To1(patch[4], patch[0]);
                                    dest[destIndex + dpL + 1] = Interpolation.Mix7To1(patch[4], patch[0]);
                                }
                                else
                                {
                                    dest[destIndex] = Interpolation.Mix2To1To1(patch[4], patch[1], patch[3]);
                                    dest[destIndex + 1] = Interpolation.Mix3To1(patch[4], patch[1]);
                                    dest[destIndex + dpL] = Interpolation.Mix3To1(patch[4], patch[3]);
                                    dest[destIndex + dpL + 1] = patch[4];
                                }
                                if (Diff(patch[1], patch[5], trY, trU, trV, trA))
                                {
                                    dest[destIndex + 2] = patch[4];
                                    dest[destIndex + 3] = patch[4];
                                    dest[destIndex + dpL + 3] = patch[4];
                                }
                                else
                                {
                                    dest[destIndex + 2] = Interpolation.MixEven(patch[1], patch[4]);
                                    dest[destIndex + 3] = Interpolation.MixEven(patch[1], patch[5]);
                                    dest[destIndex + dpL + 3] = Interpolation.MixEven(patch[5], patch[4]);
                                }
                                dest[destIndex + dpL + 2] = patch[4];
                                dest[destIndex + dpL + dpL] = Interpolation.Mix3To1(patch[4], patch[6]);
                                dest[destIndex + dpL + dpL + 1] = Interpolation.Mix7To1(patch[4], patch[6]);
                                dest[destIndex + dpL + dpL + 2] = Interpolation.Mix7To1(patch[4], patch[7]);
                                dest[destIndex + dpL + dpL + 3] = Interpolation.Mix7To1(patch[4], patch[7]);
                                dest[destIndex + dpL + dpL + dpL] = Interpolation.Mix5To3(patch[4], patch[6]);
                                dest[destIndex + dpL + dpL + dpL + 1] = Interpolation.Mix5To2To1(patch[4], patch[7], patch[6]);
                                dest[destIndex + dpL + dpL + dpL + 2] = Interpolation.Mix5To3(patch[4], patch[7]);
                                dest[destIndex + dpL + dpL + dpL + 3] = Interpolation.Mix5To3(patch[4], patch[7]);
                                break;
                            }
                        case 234:
                            {
                                if (Diff(patch[3], patch[1], trY, trU, trV, trA))
                                {
                                    dest[destIndex] = Interpolation.Mix5To3(patch[4], patch[0]);
                                    dest[destIndex + 1] = Interpolation.Mix3To1(patch[4], patch[0]);
                                    dest[destIndex + dpL] = Interpolation.Mix3To1(patch[4], patch[0]);
                                    dest[destIndex + dpL + 1] = Interpolation.Mix7To1(patch[4], patch[0]);
                                }
                                else
                                {
                                    dest[destIndex] = Interpolation.Mix2To1To1(patch[4], patch[1], patch[3]);
                                    dest[destIndex + 1] = Interpolation.Mix3To1(patch[4], patch[1]);
                                    dest[destIndex + dpL] = Interpolation.Mix3To1(patch[4], patch[3]);
                                    dest[destIndex + dpL + 1] = patch[4];
                                }
                                dest[destIndex + 2] = Interpolation.Mix3To1(patch[4], patch[2]);
                                dest[destIndex + 3] = Interpolation.Mix5To3(patch[4], patch[2]);
                                dest[destIndex + dpL + 2] = Interpolation.Mix7To1(patch[4], patch[2]);
                                dest[destIndex + dpL + 3] = Interpolation.Mix5To2To1(patch[4], patch[5], patch[2]);
                                if (Diff(patch[7], patch[3], trY, trU, trV, trA))
                                {
                                    dest[destIndex + dpL + dpL] = patch[4];
                                    dest[destIndex + dpL + dpL + dpL] = patch[4];
                                    dest[destIndex + dpL + dpL + dpL + 1] = patch[4];
                                }
                                else
                                {
                                    dest[destIndex + dpL + dpL] = Interpolation.MixEven(patch[3], patch[4]);
                                    dest[destIndex + dpL + dpL + dpL] = Interpolation.MixEven(patch[7], patch[3]);
                                    dest[destIndex + dpL + dpL + dpL + 1] = Interpolation.MixEven(patch[7], patch[4]);
                                }
                                dest[destIndex + dpL + dpL + 1] = patch[4];
                                dest[destIndex + dpL + dpL + 2] = Interpolation.Mix7To1(patch[4], patch[5]);
                                dest[destIndex + dpL + dpL + 3] = Interpolation.Mix5To3(patch[4], patch[5]);
                                dest[destIndex + dpL + dpL + dpL + 2] = Interpolation.Mix7To1(patch[4], patch[5]);
                                dest[destIndex + dpL + dpL + dpL + 3] = Interpolation.Mix5To3(patch[4], patch[5]);
                                break;
                            }
                        case 242:
                            {
                                dest[destIndex] = Interpolation.Mix5To3(patch[4], patch[0]);
                                dest[destIndex + 1] = Interpolation.Mix3To1(patch[4], patch[0]);
                                if (Diff(patch[1], patch[5], trY, trU, trV, trA))
                                {
                                    dest[destIndex + 2] = Interpolation.Mix3To1(patch[4], patch[2]);
                                    dest[destIndex + 3] = Interpolation.Mix5To3(patch[4], patch[2]);
                                    dest[destIndex + dpL + 2] = Interpolation.Mix7To1(patch[4], patch[2]);
                                    dest[destIndex + dpL + 3] = Interpolation.Mix3To1(patch[4], patch[2]);
                                }
                                else
                                {
                                    dest[destIndex + 2] = Interpolation.Mix3To1(patch[4], patch[1]);
                                    dest[destIndex + 3] = Interpolation.Mix2To1To1(patch[4], patch[1], patch[5]);
                                    dest[destIndex + dpL + 2] = patch[4];
                                    dest[destIndex + dpL + 3] = Interpolation.Mix3To1(patch[4], patch[5]);
                                }
                                dest[destIndex + dpL] = Interpolation.Mix5To2To1(patch[4], patch[3], patch[0]);
                                dest[destIndex + dpL + 1] = Interpolation.Mix7To1(patch[4], patch[0]);
                                dest[destIndex + dpL + dpL] = Interpolation.Mix5To3(patch[4], patch[3]);
                                dest[destIndex + dpL + dpL + 1] = Interpolation.Mix7To1(patch[4], patch[3]);
                                dest[destIndex + dpL + dpL + 2] = patch[4];
                                if (Diff(patch[5], patch[7], trY, trU, trV, trA))
                                {
                                    dest[destIndex + dpL + dpL + 3] = patch[4];
                                    dest[destIndex + dpL + dpL + dpL + 2] = patch[4];
                                    dest[destIndex + dpL + dpL + dpL + 3] = patch[4];
                                }
                                else
                                {
                                    dest[destIndex + dpL + dpL + 3] = Interpolation.MixEven(patch[5], patch[4]);
                                    dest[destIndex + dpL + dpL + dpL + 2] = Interpolation.MixEven(patch[7], patch[4]);
                                    dest[destIndex + dpL + dpL + dpL + 3] = Interpolation.MixEven(patch[7], patch[5]);
                                }
                                dest[destIndex + dpL + dpL + dpL] = Interpolation.Mix5To3(patch[4], patch[3]);
                                dest[destIndex + dpL + dpL + dpL + 1] = Interpolation.Mix7To1(patch[4], patch[3]);
                                break;
                            }
                        case 59:
                            {
                                if (Diff(patch[3], patch[1], trY, trU, trV, trA))
                                {
                                    dest[destIndex] = patch[4];
                                    dest[destIndex + 1] = patch[4];
                                    dest[destIndex + dpL] = patch[4];
                                }
                                else
                                {
                                    dest[destIndex] = Interpolation.MixEven(patch[1], patch[3]);
                                    dest[destIndex + 1] = Interpolation.MixEven(patch[1], patch[4]);
                                    dest[destIndex + dpL] = Interpolation.MixEven(patch[3], patch[4]);
                                }
                                if (Diff(patch[1], patch[5], trY, trU, trV, trA))
                                {
                                    dest[destIndex + 2] = Interpolation.Mix3To1(patch[4], patch[2]);
                                    dest[destIndex + 3] = Interpolation.Mix5To3(patch[4], patch[2]);
                                    dest[destIndex + dpL + 2] = Interpolation.Mix7To1(patch[4], patch[2]);
                                    dest[destIndex + dpL + 3] = Interpolation.Mix3To1(patch[4], patch[2]);
                                }
                                else
                                {
                                    dest[destIndex + 2] = Interpolation.Mix3To1(patch[4], patch[1]);
                                    dest[destIndex + 3] = Interpolation.Mix2To1To1(patch[4], patch[1], patch[5]);
                                    dest[destIndex + dpL + 2] = patch[4];
                                    dest[destIndex + dpL + 3] = Interpolation.Mix3To1(patch[4], patch[5]);
                                }
                                dest[destIndex + dpL + 1] = patch[4];
                                dest[destIndex + dpL + dpL] = Interpolation.Mix7To1(patch[4], patch[7]);
                                dest[destIndex + dpL + dpL + 1] = Interpolation.Mix7To1(patch[4], patch[7]);
                                dest[destIndex + dpL + dpL + 2] = Interpolation.Mix7To1(patch[4], patch[8]);
                                dest[destIndex + dpL + dpL + 3] = Interpolation.Mix3To1(patch[4], patch[8]);
                                dest[destIndex + dpL + dpL + dpL] = Interpolation.Mix5To3(patch[4], patch[7]);
                                dest[destIndex + dpL + dpL + dpL + 1] = Interpolation.Mix5To3(patch[4], patch[7]);
                                dest[destIndex + dpL + dpL + dpL + 2] = Interpolation.Mix5To2To1(patch[4], patch[7], patch[8]);
                                dest[destIndex + dpL + dpL + dpL + 3] = Interpolation.Mix5To3(patch[4], patch[8]);
                                break;
                            }
                        case 121:
                            {
                                dest[destIndex] = Interpolation.Mix5To3(patch[4], patch[1]);
                                dest[destIndex + 1] = Interpolation.Mix5To3(patch[4], patch[1]);
                                dest[destIndex + 2] = Interpolation.Mix5To2To1(patch[4], patch[1], patch[2]);
                                dest[destIndex + 3] = Interpolation.Mix5To3(patch[4], patch[2]);
                                dest[destIndex + dpL] = Interpolation.Mix7To1(patch[4], patch[1]);
                                dest[destIndex + dpL + 1] = Interpolation.Mix7To1(patch[4], patch[1]);
                                dest[destIndex + dpL + 2] = Interpolation.Mix7To1(patch[4], patch[2]);
                                dest[destIndex + dpL + 3] = Interpolation.Mix3To1(patch[4], patch[2]);
                                if (Diff(patch[7], patch[3], trY, trU, trV, trA))
                                {
                                    dest[destIndex + dpL + dpL] = patch[4];
                                    dest[destIndex + dpL + dpL + dpL] = patch[4];
                                    dest[destIndex + dpL + dpL + dpL + 1] = patch[4];
                                }
                                else
                                {
                                    dest[destIndex + dpL + dpL] = Interpolation.MixEven(patch[3], patch[4]);
                                    dest[destIndex + dpL + dpL + dpL] = Interpolation.MixEven(patch[7], patch[3]);
                                    dest[destIndex + dpL + dpL + dpL + 1] = Interpolation.MixEven(patch[7], patch[4]);
                                }
                                dest[destIndex + dpL + dpL + 1] = patch[4];
                                if (Diff(patch[5], patch[7], trY, trU, trV, trA))
                                {
                                    dest[destIndex + dpL + dpL + 2] = Interpolation.Mix7To1(patch[4], patch[8]);
                                    dest[destIndex + dpL + dpL + 3] = Interpolation.Mix3To1(patch[4], patch[8]);
                                    dest[destIndex + dpL + dpL + dpL + 2] = Interpolation.Mix3To1(patch[4], patch[8]);
                                    dest[destIndex + dpL + dpL + dpL + 3] = Interpolation.Mix5To3(patch[4], patch[8]);
                                }
                                else
                                {
                                    dest[destIndex + dpL + dpL + 2] = patch[4];
                                    dest[destIndex + dpL + dpL + 3] = Interpolation.Mix3To1(patch[4], patch[5]);
                                    dest[destIndex + dpL + dpL + dpL + 2] = Interpolation.Mix3To1(patch[4], patch[7]);
                                    dest[destIndex + dpL + dpL + dpL + 3] = Interpolation.Mix2To1To1(patch[4], patch[7], patch[5]);
                                }
                                break;
                            }
                        case 87:
                            {
                                dest[destIndex] = Interpolation.Mix5To3(patch[4], patch[3]);
                                dest[destIndex + 1] = Interpolation.Mix7To1(patch[4], patch[3]);
                                if (Diff(patch[1], patch[5], trY, trU, trV, trA))
                                {
                                    dest[destIndex + 2] = patch[4];
                                    dest[destIndex + 3] = patch[4];
                                    dest[destIndex + dpL + 3] = patch[4];
                                }
                                else
                                {
                                    dest[destIndex + 2] = Interpolation.MixEven(patch[1], patch[4]);
                                    dest[destIndex + 3] = Interpolation.MixEven(patch[1], patch[5]);
                                    dest[destIndex + dpL + 3] = Interpolation.MixEven(patch[5], patch[4]);
                                }
                                dest[destIndex + dpL] = Interpolation.Mix5To3(patch[4], patch[3]);
                                dest[destIndex + dpL + 1] = Interpolation.Mix7To1(patch[4], patch[3]);
                                dest[destIndex + dpL + 2] = patch[4];
                                dest[destIndex + dpL + dpL] = Interpolation.Mix5To2To1(patch[4], patch[3], patch[6]);
                                dest[destIndex + dpL + dpL + 1] = Interpolation.Mix7To1(patch[4], patch[6]);
                                if (Diff(patch[5], patch[7], trY, trU, trV, trA))
                                {
                                    dest[destIndex + dpL + dpL + 2] = Interpolation.Mix7To1(patch[4], patch[8]);
                                    dest[destIndex + dpL + dpL + 3] = Interpolation.Mix3To1(patch[4], patch[8]);
                                    dest[destIndex + dpL + dpL + dpL + 2] = Interpolation.Mix3To1(patch[4], patch[8]);
                                    dest[destIndex + dpL + dpL + dpL + 3] = Interpolation.Mix5To3(patch[4], patch[8]);
                                }
                                else
                                {
                                    dest[destIndex + dpL + dpL + 2] = patch[4];
                                    dest[destIndex + dpL + dpL + 3] = Interpolation.Mix3To1(patch[4], patch[5]);
                                    dest[destIndex + dpL + dpL + dpL + 2] = Interpolation.Mix3To1(patch[4], patch[7]);
                                    dest[destIndex + dpL + dpL + dpL + 3] = Interpolation.Mix2To1To1(patch[4], patch[7], patch[5]);
                                }
                                dest[destIndex + dpL + dpL + dpL] = Interpolation.Mix5To3(patch[4], patch[6]);
                                dest[destIndex + dpL + dpL + dpL + 1] = Interpolation.Mix3To1(patch[4], patch[6]);
                                break;
                            }
                        case 79:
                            {
                                if (Diff(patch[3], patch[1], trY, trU, trV, trA))
                                {
                                    dest[destIndex] = patch[4];
                                    dest[destIndex + 1] = patch[4];
                                    dest[destIndex + dpL] = patch[4];
                                }
                                else
                                {
                                    dest[destIndex] = Interpolation.MixEven(patch[1], patch[3]);
                                    dest[destIndex + 1] = Interpolation.MixEven(patch[1], patch[4]);
                                    dest[destIndex + dpL] = Interpolation.MixEven(patch[3], patch[4]);
                                }
                                dest[destIndex + 2] = Interpolation.Mix7To1(patch[4], patch[5]);
                                dest[destIndex + 3] = Interpolation.Mix5To3(patch[4], patch[5]);
                                dest[destIndex + dpL + 1] = patch[4];
                                dest[destIndex + dpL + 2] = Interpolation.Mix7To1(patch[4], patch[5]);
                                dest[destIndex + dpL + 3] = Interpolation.Mix5To3(patch[4], patch[5]);
                                if (Diff(patch[7], patch[3], trY, trU, trV, trA))
                                {
                                    dest[destIndex + dpL + dpL] = Interpolation.Mix3To1(patch[4], patch[6]);
                                    dest[destIndex + dpL + dpL + 1] = Interpolation.Mix7To1(patch[4], patch[6]);
                                    dest[destIndex + dpL + dpL + dpL] = Interpolation.Mix5To3(patch[4], patch[6]);
                                    dest[destIndex + dpL + dpL + dpL + 1] = Interpolation.Mix3To1(patch[4], patch[6]);
                                }
                                else
                                {
                                    dest[destIndex + dpL + dpL] = Interpolation.Mix3To1(patch[4], patch[3]);
                                    dest[destIndex + dpL + dpL + 1] = patch[4];
                                    dest[destIndex + dpL + dpL + dpL] = Interpolation.Mix2To1To1(patch[4], patch[7], patch[3]);
                                    dest[destIndex + dpL + dpL + dpL + 1] = Interpolation.Mix3To1(patch[4], patch[7]);
                                }
                                dest[destIndex + dpL + dpL + 2] = Interpolation.Mix7To1(patch[4], patch[8]);
                                dest[destIndex + dpL + dpL + 3] = Interpolation.Mix5To2To1(patch[4], patch[5], patch[8]);
                                dest[destIndex + dpL + dpL + dpL + 2] = Interpolation.Mix3To1(patch[4], patch[8]);
                                dest[destIndex + dpL + dpL + dpL + 3] = Interpolation.Mix5To3(patch[4], patch[8]);
                                break;
                            }
                        case 122:
                            {
                                if (Diff(patch[3], patch[1], trY, trU, trV, trA))
                                {
                                    dest[destIndex] = Interpolation.Mix5To3(patch[4], patch[0]);
                                    dest[destIndex + 1] = Interpolation.Mix3To1(patch[4], patch[0]);
                                    dest[destIndex + dpL] = Interpolation.Mix3To1(patch[4], patch[0]);
                                    dest[destIndex + dpL + 1] = Interpolation.Mix7To1(patch[4], patch[0]);
                                }
                                else
                                {
                                    dest[destIndex] = Interpolation.Mix2To1To1(patch[4], patch[1], patch[3]);
                                    dest[destIndex + 1] = Interpolation.Mix3To1(patch[4], patch[1]);
                                    dest[destIndex + dpL] = Interpolation.Mix3To1(patch[4], patch[3]);
                                    dest[destIndex + dpL + 1] = patch[4];
                                }
                                if (Diff(patch[1], patch[5], trY, trU, trV, trA))
                                {
                                    dest[destIndex + 2] = Interpolation.Mix3To1(patch[4], patch[2]);
                                    dest[destIndex + 3] = Interpolation.Mix5To3(patch[4], patch[2]);
                                    dest[destIndex + dpL + 2] = Interpolation.Mix7To1(patch[4], patch[2]);
                                    dest[destIndex + dpL + 3] = Interpolation.Mix3To1(patch[4], patch[2]);
                                }
                                else
                                {
                                    dest[destIndex + 2] = Interpolation.Mix3To1(patch[4], patch[1]);
                                    dest[destIndex + 3] = Interpolation.Mix2To1To1(patch[4], patch[1], patch[5]);
                                    dest[destIndex + dpL + 2] = patch[4];
                                    dest[destIndex + dpL + 3] = Interpolation.Mix3To1(patch[4], patch[5]);
                                }
                                if (Diff(patch[7], patch[3], trY, trU, trV, trA))
                                {
                                    dest[destIndex + dpL + dpL] = patch[4];
                                    dest[destIndex + dpL + dpL + dpL] = patch[4];
                                    dest[destIndex + dpL + dpL + dpL + 1] = patch[4];
                                }
                                else
                                {
                                    dest[destIndex + dpL + dpL] = Interpolation.MixEven(patch[3], patch[4]);
                                    dest[destIndex + dpL + dpL + dpL] = Interpolation.MixEven(patch[7], patch[3]);
                                    dest[destIndex + dpL + dpL + dpL + 1] = Interpolation.MixEven(patch[7], patch[4]);
                                }
                                dest[destIndex + dpL + dpL + 1] = patch[4];
                                if (Diff(patch[5], patch[7], trY, trU, trV, trA))
                                {
                                    dest[destIndex + dpL + dpL + 2] = Interpolation.Mix7To1(patch[4], patch[8]);
                                    dest[destIndex + dpL + dpL + 3] = Interpolation.Mix3To1(patch[4], patch[8]);
                                    dest[destIndex + dpL + dpL + dpL + 2] = Interpolation.Mix3To1(patch[4], patch[8]);
                                    dest[destIndex + dpL + dpL + dpL + 3] = Interpolation.Mix5To3(patch[4], patch[8]);
                                }
                                else
                                {
                                    dest[destIndex + dpL + dpL + 2] = patch[4];
                                    dest[destIndex + dpL + dpL + 3] = Interpolation.Mix3To1(patch[4], patch[5]);
                                    dest[destIndex + dpL + dpL + dpL + 2] = Interpolation.Mix3To1(patch[4], patch[7]);
                                    dest[destIndex + dpL + dpL + dpL + 3] = Interpolation.Mix2To1To1(patch[4], patch[7], patch[5]);
                                }
                                break;
                            }
                        case 94:
                            {
                                if (Diff(patch[3], patch[1], trY, trU, trV, trA))
                                {
                                    dest[destIndex] = Interpolation.Mix5To3(patch[4], patch[0]);
                                    dest[destIndex + 1] = Interpolation.Mix3To1(patch[4], patch[0]);
                                    dest[destIndex + dpL] = Interpolation.Mix3To1(patch[4], patch[0]);
                                    dest[destIndex + dpL + 1] = Interpolation.Mix7To1(patch[4], patch[0]);
                                }
                                else
                                {
                                    dest[destIndex] = Interpolation.Mix2To1To1(patch[4], patch[1], patch[3]);
                                    dest[destIndex + 1] = Interpolation.Mix3To1(patch[4], patch[1]);
                                    dest[destIndex + dpL] = Interpolation.Mix3To1(patch[4], patch[3]);
                                    dest[destIndex + dpL + 1] = patch[4];
                                }
                                if (Diff(patch[1], patch[5], trY, trU, trV, trA))
                                {
                                    dest[destIndex + 2] = patch[4];
                                    dest[destIndex + 3] = patch[4];
                                    dest[destIndex + dpL + 3] = patch[4];
                                }
                                else
                                {
                                    dest[destIndex + 2] = Interpolation.MixEven(patch[1], patch[4]);
                                    dest[destIndex + 3] = Interpolation.MixEven(patch[1], patch[5]);
                                    dest[destIndex + dpL + 3] = Interpolation.MixEven(patch[5], patch[4]);
                                }
                                dest[destIndex + dpL + 2] = patch[4];
                                if (Diff(patch[7], patch[3], trY, trU, trV, trA))
                                {
                                    dest[destIndex + dpL + dpL] = Interpolation.Mix3To1(patch[4], patch[6]);
                                    dest[destIndex + dpL + dpL + 1] = Interpolation.Mix7To1(patch[4], patch[6]);
                                    dest[destIndex + dpL + dpL + dpL] = Interpolation.Mix5To3(patch[4], patch[6]);
                                    dest[destIndex + dpL + dpL + dpL + 1] = Interpolation.Mix3To1(patch[4], patch[6]);
                                }
                                else
                                {
                                    dest[destIndex + dpL + dpL] = Interpolation.Mix3To1(patch[4], patch[3]);
                                    dest[destIndex + dpL + dpL + 1] = patch[4];
                                    dest[destIndex + dpL + dpL + dpL] = Interpolation.Mix2To1To1(patch[4], patch[7], patch[3]);
                                    dest[destIndex + dpL + dpL + dpL + 1] = Interpolation.Mix3To1(patch[4], patch[7]);
                                }
                                if (Diff(patch[5], patch[7], trY, trU, trV, trA))
                                {
                                    dest[destIndex + dpL + dpL + 2] = Interpolation.Mix7To1(patch[4], patch[8]);
                                    dest[destIndex + dpL + dpL + 3] = Interpolation.Mix3To1(patch[4], patch[8]);
                                    dest[destIndex + dpL + dpL + dpL + 2] = Interpolation.Mix3To1(patch[4], patch[8]);
                                    dest[destIndex + dpL + dpL + dpL + 3] = Interpolation.Mix5To3(patch[4], patch[8]);
                                }
                                else
                                {
                                    dest[destIndex + dpL + dpL + 2] = patch[4];
                                    dest[destIndex + dpL + dpL + 3] = Interpolation.Mix3To1(patch[4], patch[5]);
                                    dest[destIndex + dpL + dpL + dpL + 2] = Interpolation.Mix3To1(patch[4], patch[7]);
                                    dest[destIndex + dpL + dpL + dpL + 3] = Interpolation.Mix2To1To1(patch[4], patch[7], patch[5]);
                                }
                                break;
                            }
                        case 218:
                            {
                                if (Diff(patch[3], patch[1], trY, trU, trV, trA))
                                {
                                    dest[destIndex] = Interpolation.Mix5To3(patch[4], patch[0]);
                                    dest[destIndex + 1] = Interpolation.Mix3To1(patch[4], patch[0]);
                                    dest[destIndex + dpL] = Interpolation.Mix3To1(patch[4], patch[0]);
                                    dest[destIndex + dpL + 1] = Interpolation.Mix7To1(patch[4], patch[0]);
                                }
                                else
                                {
                                    dest[destIndex] = Interpolation.Mix2To1To1(patch[4], patch[1], patch[3]);
                                    dest[destIndex + 1] = Interpolation.Mix3To1(patch[4], patch[1]);
                                    dest[destIndex + dpL] = Interpolation.Mix3To1(patch[4], patch[3]);
                                    dest[destIndex + dpL + 1] = patch[4];
                                }
                                if (Diff(patch[1], patch[5], trY, trU, trV, trA))
                                {
                                    dest[destIndex + 2] = Interpolation.Mix3To1(patch[4], patch[2]);
                                    dest[destIndex + 3] = Interpolation.Mix5To3(patch[4], patch[2]);
                                    dest[destIndex + dpL + 2] = Interpolation.Mix7To1(patch[4], patch[2]);
                                    dest[destIndex + dpL + 3] = Interpolation.Mix3To1(patch[4], patch[2]);
                                }
                                else
                                {
                                    dest[destIndex + 2] = Interpolation.Mix3To1(patch[4], patch[1]);
                                    dest[destIndex + 3] = Interpolation.Mix2To1To1(patch[4], patch[1], patch[5]);
                                    dest[destIndex + dpL + 2] = patch[4];
                                    dest[destIndex + dpL + 3] = Interpolation.Mix3To1(patch[4], patch[5]);
                                }
                                if (Diff(patch[7], patch[3], trY, trU, trV, trA))
                                {
                                    dest[destIndex + dpL + dpL] = Interpolation.Mix3To1(patch[4], patch[6]);
                                    dest[destIndex + dpL + dpL + 1] = Interpolation.Mix7To1(patch[4], patch[6]);
                                    dest[destIndex + dpL + dpL + dpL] = Interpolation.Mix5To3(patch[4], patch[6]);
                                    dest[destIndex + dpL + dpL + dpL + 1] = Interpolation.Mix3To1(patch[4], patch[6]);
                                }
                                else
                                {
                                    dest[destIndex + dpL + dpL] = Interpolation.Mix3To1(patch[4], patch[3]);
                                    dest[destIndex + dpL + dpL + 1] = patch[4];
                                    dest[destIndex + dpL + dpL + dpL] = Interpolation.Mix2To1To1(patch[4], patch[7], patch[3]);
                                    dest[destIndex + dpL + dpL + dpL + 1] = Interpolation.Mix3To1(patch[4], patch[7]);
                                }
                                dest[destIndex + dpL + dpL + 2] = patch[4];
                                if (Diff(patch[5], patch[7], trY, trU, trV, trA))
                                {
                                    dest[destIndex + dpL + dpL + 3] = patch[4];
                                    dest[destIndex + dpL + dpL + dpL + 2] = patch[4];
                                    dest[destIndex + dpL + dpL + dpL + 3] = patch[4];
                                }
                                else
                                {
                                    dest[destIndex + dpL + dpL + 3] = Interpolation.MixEven(patch[5], patch[4]);
                                    dest[destIndex + dpL + dpL + dpL + 2] = Interpolation.MixEven(patch[7], patch[4]);
                                    dest[destIndex + dpL + dpL + dpL + 3] = Interpolation.MixEven(patch[7], patch[5]);
                                }
                                break;
                            }
                        case 91:
                            {
                                if (Diff(patch[3], patch[1], trY, trU, trV, trA))
                                {
                                    dest[destIndex] = patch[4];
                                    dest[destIndex + 1] = patch[4];
                                    dest[destIndex + dpL] = patch[4];
                                }
                                else
                                {
                                    dest[destIndex] = Interpolation.MixEven(patch[1], patch[3]);
                                    dest[destIndex + 1] = Interpolation.MixEven(patch[1], patch[4]);
                                    dest[destIndex + dpL] = Interpolation.MixEven(patch[3], patch[4]);
                                }
                                if (Diff(patch[1], patch[5], trY, trU, trV, trA))
                                {
                                    dest[destIndex + 2] = Interpolation.Mix3To1(patch[4], patch[2]);
                                    dest[destIndex + 3] = Interpolation.Mix5To3(patch[4], patch[2]);
                                    dest[destIndex + dpL + 2] = Interpolation.Mix7To1(patch[4], patch[2]);
                                    dest[destIndex + dpL + 3] = Interpolation.Mix3To1(patch[4], patch[2]);
                                }
                                else
                                {
                                    dest[destIndex + 2] = Interpolation.Mix3To1(patch[4], patch[1]);
                                    dest[destIndex + 3] = Interpolation.Mix2To1To1(patch[4], patch[1], patch[5]);
                                    dest[destIndex + dpL + 2] = patch[4];
                                    dest[destIndex + dpL + 3] = Interpolation.Mix3To1(patch[4], patch[5]);
                                }
                                dest[destIndex + dpL + 1] = patch[4];
                                if (Diff(patch[7], patch[3], trY, trU, trV, trA))
                                {
                                    dest[destIndex + dpL + dpL] = Interpolation.Mix3To1(patch[4], patch[6]);
                                    dest[destIndex + dpL + dpL + 1] = Interpolation.Mix7To1(patch[4], patch[6]);
                                    dest[destIndex + dpL + dpL + dpL] = Interpolation.Mix5To3(patch[4], patch[6]);
                                    dest[destIndex + dpL + dpL + dpL + 1] = Interpolation.Mix3To1(patch[4], patch[6]);
                                }
                                else
                                {
                                    dest[destIndex + dpL + dpL] = Interpolation.Mix3To1(patch[4], patch[3]);
                                    dest[destIndex + dpL + dpL + 1] = patch[4];
                                    dest[destIndex + dpL + dpL + dpL] = Interpolation.Mix2To1To1(patch[4], patch[7], patch[3]);
                                    dest[destIndex + dpL + dpL + dpL + 1] = Interpolation.Mix3To1(patch[4], patch[7]);
                                }
                                if (Diff(patch[5], patch[7], trY, trU, trV, trA))
                                {
                                    dest[destIndex + dpL + dpL + 2] = Interpolation.Mix7To1(patch[4], patch[8]);
                                    dest[destIndex + dpL + dpL + 3] = Interpolation.Mix3To1(patch[4], patch[8]);
                                    dest[destIndex + dpL + dpL + dpL + 2] = Interpolation.Mix3To1(patch[4], patch[8]);
                                    dest[destIndex + dpL + dpL + dpL + 3] = Interpolation.Mix5To3(patch[4], patch[8]);
                                }
                                else
                                {
                                    dest[destIndex + dpL + dpL + 2] = patch[4];
                                    dest[destIndex + dpL + dpL + 3] = Interpolation.Mix3To1(patch[4], patch[5]);
                                    dest[destIndex + dpL + dpL + dpL + 2] = Interpolation.Mix3To1(patch[4], patch[7]);
                                    dest[destIndex + dpL + dpL + dpL + 3] = Interpolation.Mix2To1To1(patch[4], patch[7], patch[5]);
                                }
                                break;
                            }
                        case 229:
                            {
                                dest[destIndex] = Interpolation.Mix2To1To1(patch[4], patch[1], patch[3]);
                                dest[destIndex + 1] = Interpolation.Mix5To2To1(patch[4], patch[1], patch[3]);
                                dest[destIndex + 2] = Interpolation.Mix5To2To1(patch[4], patch[1], patch[5]);
                                dest[destIndex + 3] = Interpolation.Mix2To1To1(patch[4], patch[1], patch[5]);
                                dest[destIndex + dpL] = Interpolation.Mix5To2To1(patch[4], patch[3], patch[1]);
                                dest[destIndex + dpL + 1] = Interpolation.Mix6To1To1(patch[4], patch[3], patch[1]);
                                dest[destIndex + dpL + 2] = Interpolation.Mix6To1To1(patch[4], patch[5], patch[1]);
                                dest[destIndex + dpL + 3] = Interpolation.Mix5To2To1(patch[4], patch[5], patch[1]);
                                dest[destIndex + dpL + dpL] = Interpolation.Mix5To3(patch[4], patch[3]);
                                dest[destIndex + dpL + dpL + 1] = Interpolation.Mix7To1(patch[4], patch[3]);
                                dest[destIndex + dpL + dpL + 2] = Interpolation.Mix7To1(patch[4], patch[5]);
                                dest[destIndex + dpL + dpL + 3] = Interpolation.Mix5To3(patch[4], patch[5]);
                                dest[destIndex + dpL + dpL + dpL] = Interpolation.Mix5To3(patch[4], patch[3]);
                                dest[destIndex + dpL + dpL + dpL + 1] = Interpolation.Mix7To1(patch[4], patch[3]);
                                dest[destIndex + dpL + dpL + dpL + 2] = Interpolation.Mix7To1(patch[4], patch[5]);
                                dest[destIndex + dpL + dpL + dpL + 3] = Interpolation.Mix5To3(patch[4], patch[5]);
                                break;
                            }
                        case 167:
                            {
                                dest[destIndex] = Interpolation.Mix5To3(patch[4], patch[3]);
                                dest[destIndex + 1] = Interpolation.Mix7To1(patch[4], patch[3]);
                                dest[destIndex + 2] = Interpolation.Mix7To1(patch[4], patch[5]);
                                dest[destIndex + 3] = Interpolation.Mix5To3(patch[4], patch[5]);
                                dest[destIndex + dpL] = Interpolation.Mix5To3(patch[4], patch[3]);
                                dest[destIndex + dpL + 1] = Interpolation.Mix7To1(patch[4], patch[3]);
                                dest[destIndex + dpL + 2] = Interpolation.Mix7To1(patch[4], patch[5]);
                                dest[destIndex + dpL + 3] = Interpolation.Mix5To3(patch[4], patch[5]);
                                dest[destIndex + dpL + dpL] = Interpolation.Mix5To2To1(patch[4], patch[3], patch[7]);
                                dest[destIndex + dpL + dpL + 1] = Interpolation.Mix6To1To1(patch[4], patch[3], patch[7]);
                                dest[destIndex + dpL + dpL + 2] = Interpolation.Mix6To1To1(patch[4], patch[5], patch[7]);
                                dest[destIndex + dpL + dpL + 3] = Interpolation.Mix5To2To1(patch[4], patch[5], patch[7]);
                                dest[destIndex + dpL + dpL + dpL] = Interpolation.Mix2To1To1(patch[4], patch[7], patch[3]);
                                dest[destIndex + dpL + dpL + dpL + 1] = Interpolation.Mix5To2To1(patch[4], patch[7], patch[3]);
                                dest[destIndex + dpL + dpL + dpL + 2] = Interpolation.Mix5To2To1(patch[4], patch[7], patch[5]);
                                dest[destIndex + dpL + dpL + dpL + 3] = Interpolation.Mix2To1To1(patch[4], patch[7], patch[5]);
                                break;
                            }
                        case 173:
                            {
                                dest[destIndex] = Interpolation.Mix5To3(patch[4], patch[1]);
                                dest[destIndex + 1] = Interpolation.Mix5To3(patch[4], patch[1]);
                                dest[destIndex + 2] = Interpolation.Mix5To2To1(patch[4], patch[1], patch[5]);
                                dest[destIndex + 3] = Interpolation.Mix2To1To1(patch[4], patch[1], patch[5]);
                                dest[destIndex + dpL] = Interpolation.Mix7To1(patch[4], patch[1]);
                                dest[destIndex + dpL + 1] = Interpolation.Mix7To1(patch[4], patch[1]);
                                dest[destIndex + dpL + 2] = Interpolation.Mix6To1To1(patch[4], patch[5], patch[1]);
                                dest[destIndex + dpL + 3] = Interpolation.Mix5To2To1(patch[4], patch[5], patch[1]);
                                dest[destIndex + dpL + dpL] = Interpolation.Mix7To1(patch[4], patch[7]);
                                dest[destIndex + dpL + dpL + 1] = Interpolation.Mix7To1(patch[4], patch[7]);
                                dest[destIndex + dpL + dpL + 2] = Interpolation.Mix6To1To1(patch[4], patch[5], patch[7]);
                                dest[destIndex + dpL + dpL + 3] = Interpolation.Mix5To2To1(patch[4], patch[5], patch[7]);
                                dest[destIndex + dpL + dpL + dpL] = Interpolation.Mix5To3(patch[4], patch[7]);
                                dest[destIndex + dpL + dpL + dpL + 1] = Interpolation.Mix5To3(patch[4], patch[7]);
                                dest[destIndex + dpL + dpL + dpL + 2] = Interpolation.Mix5To2To1(patch[4], patch[7], patch[5]);
                                dest[destIndex + dpL + dpL + dpL + 3] = Interpolation.Mix2To1To1(patch[4], patch[7], patch[5]);
                                break;
                            }
                        case 181:
                            {
                                dest[destIndex] = Interpolation.Mix2To1To1(patch[4], patch[1], patch[3]);
                                dest[destIndex + 1] = Interpolation.Mix5To2To1(patch[4], patch[1], patch[3]);
                                dest[destIndex + 2] = Interpolation.Mix5To3(patch[4], patch[1]);
                                dest[destIndex + 3] = Interpolation.Mix5To3(patch[4], patch[1]);
                                dest[destIndex + dpL] = Interpolation.Mix5To2To1(patch[4], patch[3], patch[1]);
                                dest[destIndex + dpL + 1] = Interpolation.Mix6To1To1(patch[4], patch[3], patch[1]);
                                dest[destIndex + dpL + 2] = Interpolation.Mix7To1(patch[4], patch[1]);
                                dest[destIndex + dpL + 3] = Interpolation.Mix7To1(patch[4], patch[1]);
                                dest[destIndex + dpL + dpL] = Interpolation.Mix5To2To1(patch[4], patch[3], patch[7]);
                                dest[destIndex + dpL + dpL + 1] = Interpolation.Mix6To1To1(patch[4], patch[3], patch[7]);
                                dest[destIndex + dpL + dpL + 2] = Interpolation.Mix7To1(patch[4], patch[7]);
                                dest[destIndex + dpL + dpL + 3] = Interpolation.Mix7To1(patch[4], patch[7]);
                                dest[destIndex + dpL + dpL + dpL] = Interpolation.Mix2To1To1(patch[4], patch[7], patch[3]);
                                dest[destIndex + dpL + dpL + dpL + 1] = Interpolation.Mix5To2To1(patch[4], patch[7], patch[3]);
                                dest[destIndex + dpL + dpL + dpL + 2] = Interpolation.Mix5To3(patch[4], patch[7]);
                                dest[destIndex + dpL + dpL + dpL + 3] = Interpolation.Mix5To3(patch[4], patch[7]);
                                break;
                            }
                        case 186:
                            {
                                if (Diff(patch[3], patch[1], trY, trU, trV, trA))
                                {
                                    dest[destIndex] = Interpolation.Mix5To3(patch[4], patch[0]);
                                    dest[destIndex + 1] = Interpolation.Mix3To1(patch[4], patch[0]);
                                    dest[destIndex + dpL] = Interpolation.Mix3To1(patch[4], patch[0]);
                                    dest[destIndex + dpL + 1] = Interpolation.Mix7To1(patch[4], patch[0]);
                                }
                                else
                                {
                                    dest[destIndex] = Interpolation.Mix2To1To1(patch[4], patch[1], patch[3]);
                                    dest[destIndex + 1] = Interpolation.Mix3To1(patch[4], patch[1]);
                                    dest[destIndex + dpL] = Interpolation.Mix3To1(patch[4], patch[3]);
                                    dest[destIndex + dpL + 1] = patch[4];
                                }
                                if (Diff(patch[1], patch[5], trY, trU, trV, trA))
                                {
                                    dest[destIndex + 2] = Interpolation.Mix3To1(patch[4], patch[2]);
                                    dest[destIndex + 3] = Interpolation.Mix5To3(patch[4], patch[2]);
                                    dest[destIndex + dpL + 2] = Interpolation.Mix7To1(patch[4], patch[2]);
                                    dest[destIndex + dpL + 3] = Interpolation.Mix3To1(patch[4], patch[2]);
                                }
                                else
                                {
                                    dest[destIndex + 2] = Interpolation.Mix3To1(patch[4], patch[1]);
                                    dest[destIndex + 3] = Interpolation.Mix2To1To1(patch[4], patch[1], patch[5]);
                                    dest[destIndex + dpL + 2] = patch[4];
                                    dest[destIndex + dpL + 3] = Interpolation.Mix3To1(patch[4], patch[5]);
                                }
                                dest[destIndex + dpL + dpL] = Interpolation.Mix7To1(patch[4], patch[7]);
                                dest[destIndex + dpL + dpL + 1] = Interpolation.Mix7To1(patch[4], patch[7]);
                                dest[destIndex + dpL + dpL + 2] = Interpolation.Mix7To1(patch[4], patch[7]);
                                dest[destIndex + dpL + dpL + 3] = Interpolation.Mix7To1(patch[4], patch[7]);
                                dest[destIndex + dpL + dpL + dpL] = Interpolation.Mix5To3(patch[4], patch[7]);
                                dest[destIndex + dpL + dpL + dpL + 1] = Interpolation.Mix5To3(patch[4], patch[7]);
                                dest[destIndex + dpL + dpL + dpL + 2] = Interpolation.Mix5To3(patch[4], patch[7]);
                                dest[destIndex + dpL + dpL + dpL + 3] = Interpolation.Mix5To3(patch[4], patch[7]);
                                break;
                            }
                        case 115:
                            {
                                dest[destIndex] = Interpolation.Mix5To3(patch[4], patch[3]);
                                dest[destIndex + 1] = Interpolation.Mix7To1(patch[4], patch[3]);
                                if (Diff(patch[1], patch[5], trY, trU, trV, trA))
                                {
                                    dest[destIndex + 2] = Interpolation.Mix3To1(patch[4], patch[2]);
                                    dest[destIndex + 3] = Interpolation.Mix5To3(patch[4], patch[2]);
                                    dest[destIndex + dpL + 2] = Interpolation.Mix7To1(patch[4], patch[2]);
                                    dest[destIndex + dpL + 3] = Interpolation.Mix3To1(patch[4], patch[2]);
                                }
                                else
                                {
                                    dest[destIndex + 2] = Interpolation.Mix3To1(patch[4], patch[1]);
                                    dest[destIndex + 3] = Interpolation.Mix2To1To1(patch[4], patch[1], patch[5]);
                                    dest[destIndex + dpL + 2] = patch[4];
                                    dest[destIndex + dpL + 3] = Interpolation.Mix3To1(patch[4], patch[5]);
                                }
                                dest[destIndex + dpL] = Interpolation.Mix5To3(patch[4], patch[3]);
                                dest[destIndex + dpL + 1] = Interpolation.Mix7To1(patch[4], patch[3]);
                                dest[destIndex + dpL + dpL] = Interpolation.Mix5To3(patch[4], patch[3]);
                                dest[destIndex + dpL + dpL + 1] = Interpolation.Mix7To1(patch[4], patch[3]);
                                if (Diff(patch[5], patch[7], trY, trU, trV, trA))
                                {
                                    dest[destIndex + dpL + dpL + 2] = Interpolation.Mix7To1(patch[4], patch[8]);
                                    dest[destIndex + dpL + dpL + 3] = Interpolation.Mix3To1(patch[4], patch[8]);
                                    dest[destIndex + dpL + dpL + dpL + 2] = Interpolation.Mix3To1(patch[4], patch[8]);
                                    dest[destIndex + dpL + dpL + dpL + 3] = Interpolation.Mix5To3(patch[4], patch[8]);
                                }
                                else
                                {
                                    dest[destIndex + dpL + dpL + 2] = patch[4];
                                    dest[destIndex + dpL + dpL + 3] = Interpolation.Mix3To1(patch[4], patch[5]);
                                    dest[destIndex + dpL + dpL + dpL + 2] = Interpolation.Mix3To1(patch[4], patch[7]);
                                    dest[destIndex + dpL + dpL + dpL + 3] = Interpolation.Mix2To1To1(patch[4], patch[7], patch[5]);
                                }
                                dest[destIndex + dpL + dpL + dpL] = Interpolation.Mix5To3(patch[4], patch[3]);
                                dest[destIndex + dpL + dpL + dpL + 1] = Interpolation.Mix7To1(patch[4], patch[3]);
                                break;
                            }
                        case 93:
                            {
                                dest[destIndex] = Interpolation.Mix5To3(patch[4], patch[1]);
                                dest[destIndex + 1] = Interpolation.Mix5To3(patch[4], patch[1]);
                                dest[destIndex + 2] = Interpolation.Mix5To3(patch[4], patch[1]);
                                dest[destIndex + 3] = Interpolation.Mix5To3(patch[4], patch[1]);
                                dest[destIndex + dpL] = Interpolation.Mix7To1(patch[4], patch[1]);
                                dest[destIndex + dpL + 1] = Interpolation.Mix7To1(patch[4], patch[1]);
                                dest[destIndex + dpL + 2] = Interpolation.Mix7To1(patch[4], patch[1]);
                                dest[destIndex + dpL + 3] = Interpolation.Mix7To1(patch[4], patch[1]);
                                if (Diff(patch[7], patch[3], trY, trU, trV, trA))
                                {
                                    dest[destIndex + dpL + dpL] = Interpolation.Mix3To1(patch[4], patch[6]);
                                    dest[destIndex + dpL + dpL + 1] = Interpolation.Mix7To1(patch[4], patch[6]);
                                    dest[destIndex + dpL + dpL + dpL] = Interpolation.Mix5To3(patch[4], patch[6]);
                                    dest[destIndex + dpL + dpL + dpL + 1] = Interpolation.Mix3To1(patch[4], patch[6]);
                                }
                                else
                                {
                                    dest[destIndex + dpL + dpL] = Interpolation.Mix3To1(patch[4], patch[3]);
                                    dest[destIndex + dpL + dpL + 1] = patch[4];
                                    dest[destIndex + dpL + dpL + dpL] = Interpolation.Mix2To1To1(patch[4], patch[7], patch[3]);
                                    dest[destIndex + dpL + dpL + dpL + 1] = Interpolation.Mix3To1(patch[4], patch[7]);
                                }
                                if (Diff(patch[5], patch[7], trY, trU, trV, trA))
                                {
                                    dest[destIndex + dpL + dpL + 2] = Interpolation.Mix7To1(patch[4], patch[8]);
                                    dest[destIndex + dpL + dpL + 3] = Interpolation.Mix3To1(patch[4], patch[8]);
                                    dest[destIndex + dpL + dpL + dpL + 2] = Interpolation.Mix3To1(patch[4], patch[8]);
                                    dest[destIndex + dpL + dpL + dpL + 3] = Interpolation.Mix5To3(patch[4], patch[8]);
                                }
                                else
                                {
                                    dest[destIndex + dpL + dpL + 2] = patch[4];
                                    dest[destIndex + dpL + dpL + 3] = Interpolation.Mix3To1(patch[4], patch[5]);
                                    dest[destIndex + dpL + dpL + dpL + 2] = Interpolation.Mix3To1(patch[4], patch[7]);
                                    dest[destIndex + dpL + dpL + dpL + 3] = Interpolation.Mix2To1To1(patch[4], patch[7], patch[5]);
                                }
                                break;
                            }
                        case 206:
                            {
                                if (Diff(patch[3], patch[1], trY, trU, trV, trA))
                                {
                                    dest[destIndex] = Interpolation.Mix5To3(patch[4], patch[0]);
                                    dest[destIndex + 1] = Interpolation.Mix3To1(patch[4], patch[0]);
                                    dest[destIndex + dpL] = Interpolation.Mix3To1(patch[4], patch[0]);
                                    dest[destIndex + dpL + 1] = Interpolation.Mix7To1(patch[4], patch[0]);
                                }
                                else
                                {
                                    dest[destIndex] = Interpolation.Mix2To1To1(patch[4], patch[1], patch[3]);
                                    dest[destIndex + 1] = Interpolation.Mix3To1(patch[4], patch[1]);
                                    dest[destIndex + dpL] = Interpolation.Mix3To1(patch[4], patch[3]);
                                    dest[destIndex + dpL + 1] = patch[4];
                                }
                                dest[destIndex + 2] = Interpolation.Mix7To1(patch[4], patch[5]);
                                dest[destIndex + 3] = Interpolation.Mix5To3(patch[4], patch[5]);
                                dest[destIndex + dpL + 2] = Interpolation.Mix7To1(patch[4], patch[5]);
                                dest[destIndex + dpL + 3] = Interpolation.Mix5To3(patch[4], patch[5]);
                                if (Diff(patch[7], patch[3], trY, trU, trV, trA))
                                {
                                    dest[destIndex + dpL + dpL] = Interpolation.Mix3To1(patch[4], patch[6]);
                                    dest[destIndex + dpL + dpL + 1] = Interpolation.Mix7To1(patch[4], patch[6]);
                                    dest[destIndex + dpL + dpL + dpL] = Interpolation.Mix5To3(patch[4], patch[6]);
                                    dest[destIndex + dpL + dpL + dpL + 1] = Interpolation.Mix3To1(patch[4], patch[6]);
                                }
                                else
                                {
                                    dest[destIndex + dpL + dpL] = Interpolation.Mix3To1(patch[4], patch[3]);
                                    dest[destIndex + dpL + dpL + 1] = patch[4];
                                    dest[destIndex + dpL + dpL + dpL] = Interpolation.Mix2To1To1(patch[4], patch[7], patch[3]);
                                    dest[destIndex + dpL + dpL + dpL + 1] = Interpolation.Mix3To1(patch[4], patch[7]);
                                }
                                dest[destIndex + dpL + dpL + 2] = Interpolation.Mix7To1(patch[4], patch[5]);
                                dest[destIndex + dpL + dpL + 3] = Interpolation.Mix5To3(patch[4], patch[5]);
                                dest[destIndex + dpL + dpL + dpL + 2] = Interpolation.Mix7To1(patch[4], patch[5]);
                                dest[destIndex + dpL + dpL + dpL + 3] = Interpolation.Mix5To3(patch[4], patch[5]);
                                break;
                            }
                        case 205:
                        case 201:
                            {
                                dest[destIndex] = Interpolation.Mix5To3(patch[4], patch[1]);
                                dest[destIndex + 1] = Interpolation.Mix5To3(patch[4], patch[1]);
                                dest[destIndex + 2] = Interpolation.Mix5To2To1(patch[4], patch[1], patch[5]);
                                dest[destIndex + 3] = Interpolation.Mix2To1To1(patch[4], patch[1], patch[5]);
                                dest[destIndex + dpL] = Interpolation.Mix7To1(patch[4], patch[1]);
                                dest[destIndex + dpL + 1] = Interpolation.Mix7To1(patch[4], patch[1]);
                                dest[destIndex + dpL + 2] = Interpolation.Mix6To1To1(patch[4], patch[5], patch[1]);
                                dest[destIndex + dpL + 3] = Interpolation.Mix5To2To1(patch[4], patch[5], patch[1]);
                                if (Diff(patch[7], patch[3], trY, trU, trV, trA))
                                {
                                    dest[destIndex + dpL + dpL] = Interpolation.Mix3To1(patch[4], patch[6]);
                                    dest[destIndex + dpL + dpL + 1] = Interpolation.Mix7To1(patch[4], patch[6]);
                                    dest[destIndex + dpL + dpL + dpL] = Interpolation.Mix5To3(patch[4], patch[6]);
                                    dest[destIndex + dpL + dpL + dpL + 1] = Interpolation.Mix3To1(patch[4], patch[6]);
                                }
                                else
                                {
                                    dest[destIndex + dpL + dpL] = Interpolation.Mix3To1(patch[4], patch[3]);
                                    dest[destIndex + dpL + dpL + 1] = patch[4];
                                    dest[destIndex + dpL + dpL + dpL] = Interpolation.Mix2To1To1(patch[4], patch[7], patch[3]);
                                    dest[destIndex + dpL + dpL + dpL + 1] = Interpolation.Mix3To1(patch[4], patch[7]);
                                }
                                dest[destIndex + dpL + dpL + 2] = Interpolation.Mix7To1(patch[4], patch[5]);
                                dest[destIndex + dpL + dpL + 3] = Interpolation.Mix5To3(patch[4], patch[5]);
                                dest[destIndex + dpL + dpL + dpL + 2] = Interpolation.Mix7To1(patch[4], patch[5]);
                                dest[destIndex + dpL + dpL + dpL + 3] = Interpolation.Mix5To3(patch[4], patch[5]);
                                break;
                            }
                        case 174:
                        case 46:
                            {
                                if (Diff(patch[3], patch[1], trY, trU, trV, trA))
                                {
                                    dest[destIndex] = Interpolation.Mix5To3(patch[4], patch[0]);
                                    dest[destIndex + 1] = Interpolation.Mix3To1(patch[4], patch[0]);
                                    dest[destIndex + dpL] = Interpolation.Mix3To1(patch[4], patch[0]);
                                    dest[destIndex + dpL + 1] = Interpolation.Mix7To1(patch[4], patch[0]);
                                }
                                else
                                {
                                    dest[destIndex] = Interpolation.Mix2To1To1(patch[4], patch[1], patch[3]);
                                    dest[destIndex + 1] = Interpolation.Mix3To1(patch[4], patch[1]);
                                    dest[destIndex + dpL] = Interpolation.Mix3To1(patch[4], patch[3]);
                                    dest[destIndex + dpL + 1] = patch[4];
                                }
                                dest[destIndex + 2] = Interpolation.Mix7To1(patch[4], patch[5]);
                                dest[destIndex + 3] = Interpolation.Mix5To3(patch[4], patch[5]);
                                dest[destIndex + dpL + 2] = Interpolation.Mix7To1(patch[4], patch[5]);
                                dest[destIndex + dpL + 3] = Interpolation.Mix5To3(patch[4], patch[5]);
                                dest[destIndex + dpL + dpL] = Interpolation.Mix7To1(patch[4], patch[7]);
                                dest[destIndex + dpL + dpL + 1] = Interpolation.Mix7To1(patch[4], patch[7]);
                                dest[destIndex + dpL + dpL + 2] = Interpolation.Mix6To1To1(patch[4], patch[5], patch[7]);
                                dest[destIndex + dpL + dpL + 3] = Interpolation.Mix5To2To1(patch[4], patch[5], patch[7]);
                                dest[destIndex + dpL + dpL + dpL] = Interpolation.Mix5To3(patch[4], patch[7]);
                                dest[destIndex + dpL + dpL + dpL + 1] = Interpolation.Mix5To3(patch[4], patch[7]);
                                dest[destIndex + dpL + dpL + dpL + 2] = Interpolation.Mix5To2To1(patch[4], patch[7], patch[5]);
                                dest[destIndex + dpL + dpL + dpL + 3] = Interpolation.Mix2To1To1(patch[4], patch[7], patch[5]);
                                break;
                            }
                        case 179:
                        case 147:
                            {
                                dest[destIndex] = Interpolation.Mix5To3(patch[4], patch[3]);
                                dest[destIndex + 1] = Interpolation.Mix7To1(patch[4], patch[3]);
                                if (Diff(patch[1], patch[5], trY, trU, trV, trA))
                                {
                                    dest[destIndex + 2] = Interpolation.Mix3To1(patch[4], patch[2]);
                                    dest[destIndex + 3] = Interpolation.Mix5To3(patch[4], patch[2]);
                                    dest[destIndex + dpL + 2] = Interpolation.Mix7To1(patch[4], patch[2]);
                                    dest[destIndex + dpL + 3] = Interpolation.Mix3To1(patch[4], patch[2]);
                                }
                                else
                                {
                                    dest[destIndex + 2] = Interpolation.Mix3To1(patch[4], patch[1]);
                                    dest[destIndex + 3] = Interpolation.Mix2To1To1(patch[4], patch[1], patch[5]);
                                    dest[destIndex + dpL + 2] = patch[4];
                                    dest[destIndex + dpL + 3] = Interpolation.Mix3To1(patch[4], patch[5]);
                                }
                                dest[destIndex + dpL] = Interpolation.Mix5To3(patch[4], patch[3]);
                                dest[destIndex + dpL + 1] = Interpolation.Mix7To1(patch[4], patch[3]);
                                dest[destIndex + dpL + dpL] = Interpolation.Mix5To2To1(patch[4], patch[3], patch[7]);
                                dest[destIndex + dpL + dpL + 1] = Interpolation.Mix6To1To1(patch[4], patch[3], patch[7]);
                                dest[destIndex + dpL + dpL + 2] = Interpolation.Mix7To1(patch[4], patch[7]);
                                dest[destIndex + dpL + dpL + 3] = Interpolation.Mix7To1(patch[4], patch[7]);
                                dest[destIndex + dpL + dpL + dpL] = Interpolation.Mix2To1To1(patch[4], patch[7], patch[3]);
                                dest[destIndex + dpL + dpL + dpL + 1] = Interpolation.Mix5To2To1(patch[4], patch[7], patch[3]);
                                dest[destIndex + dpL + dpL + dpL + 2] = Interpolation.Mix5To3(patch[4], patch[7]);
                                dest[destIndex + dpL + dpL + dpL + 3] = Interpolation.Mix5To3(patch[4], patch[7]);
                                break;
                            }
                        case 117:
                        case 116:
                            {
                                dest[destIndex] = Interpolation.Mix2To1To1(patch[4], patch[1], patch[3]);
                                dest[destIndex + 1] = Interpolation.Mix5To2To1(patch[4], patch[1], patch[3]);
                                dest[destIndex + 2] = Interpolation.Mix5To3(patch[4], patch[1]);
                                dest[destIndex + 3] = Interpolation.Mix5To3(patch[4], patch[1]);
                                dest[destIndex + dpL] = Interpolation.Mix5To2To1(patch[4], patch[3], patch[1]);
                                dest[destIndex + dpL + 1] = Interpolation.Mix6To1To1(patch[4], patch[3], patch[1]);
                                dest[destIndex + dpL + 2] = Interpolation.Mix7To1(patch[4], patch[1]);
                                dest[destIndex + dpL + 3] = Interpolation.Mix7To1(patch[4], patch[1]);
                                dest[destIndex + dpL + dpL] = Interpolation.Mix5To3(patch[4], patch[3]);
                                dest[destIndex + dpL + dpL + 1] = Interpolation.Mix7To1(patch[4], patch[3]);
                                if (Diff(patch[5], patch[7], trY, trU, trV, trA))
                                {
                                    dest[destIndex + dpL + dpL + 2] = Interpolation.Mix7To1(patch[4], patch[8]);
                                    dest[destIndex + dpL + dpL + 3] = Interpolation.Mix3To1(patch[4], patch[8]);
                                    dest[destIndex + dpL + dpL + dpL + 2] = Interpolation.Mix3To1(patch[4], patch[8]);
                                    dest[destIndex + dpL + dpL + dpL + 3] = Interpolation.Mix5To3(patch[4], patch[8]);
                                }
                                else
                                {
                                    dest[destIndex + dpL + dpL + 2] = patch[4];
                                    dest[destIndex + dpL + dpL + 3] = Interpolation.Mix3To1(patch[4], patch[5]);
                                    dest[destIndex + dpL + dpL + dpL + 2] = Interpolation.Mix3To1(patch[4], patch[7]);
                                    dest[destIndex + dpL + dpL + dpL + 3] = Interpolation.Mix2To1To1(patch[4], patch[7], patch[5]);
                                }
                                dest[destIndex + dpL + dpL + dpL] = Interpolation.Mix5To3(patch[4], patch[3]);
                                dest[destIndex + dpL + dpL + dpL + 1] = Interpolation.Mix7To1(patch[4], patch[3]);
                                break;
                            }
                        case 189:
                            {
                                dest[destIndex] = Interpolation.Mix5To3(patch[4], patch[1]);
                                dest[destIndex + 1] = Interpolation.Mix5To3(patch[4], patch[1]);
                                dest[destIndex + 2] = Interpolation.Mix5To3(patch[4], patch[1]);
                                dest[destIndex + 3] = Interpolation.Mix5To3(patch[4], patch[1]);
                                dest[destIndex + dpL] = Interpolation.Mix7To1(patch[4], patch[1]);
                                dest[destIndex + dpL + 1] = Interpolation.Mix7To1(patch[4], patch[1]);
                                dest[destIndex + dpL + 2] = Interpolation.Mix7To1(patch[4], patch[1]);
                                dest[destIndex + dpL + 3] = Interpolation.Mix7To1(patch[4], patch[1]);
                                dest[destIndex + dpL + dpL] = Interpolation.Mix7To1(patch[4], patch[7]);
                                dest[destIndex + dpL + dpL + 1] = Interpolation.Mix7To1(patch[4], patch[7]);
                                dest[destIndex + dpL + dpL + 2] = Interpolation.Mix7To1(patch[4], patch[7]);
                                dest[destIndex + dpL + dpL + 3] = Interpolation.Mix7To1(patch[4], patch[7]);
                                dest[destIndex + dpL + dpL + dpL] = Interpolation.Mix5To3(patch[4], patch[7]);
                                dest[destIndex + dpL + dpL + dpL + 1] = Interpolation.Mix5To3(patch[4], patch[7]);
                                dest[destIndex + dpL + dpL + dpL + 2] = Interpolation.Mix5To3(patch[4], patch[7]);
                                dest[destIndex + dpL + dpL + dpL + 3] = Interpolation.Mix5To3(patch[4], patch[7]);
                                break;
                            }
                        case 231:
                            {
                                dest[destIndex] = Interpolation.Mix5To3(patch[4], patch[3]);
                                dest[destIndex + 1] = Interpolation.Mix7To1(patch[4], patch[3]);
                                dest[destIndex + 2] = Interpolation.Mix7To1(patch[4], patch[5]);
                                dest[destIndex + 3] = Interpolation.Mix5To3(patch[4], patch[5]);
                                dest[destIndex + dpL] = Interpolation.Mix5To3(patch[4], patch[3]);
                                dest[destIndex + dpL + 1] = Interpolation.Mix7To1(patch[4], patch[3]);
                                dest[destIndex + dpL + 2] = Interpolation.Mix7To1(patch[4], patch[5]);
                                dest[destIndex + dpL + 3] = Interpolation.Mix5To3(patch[4], patch[5]);
                                dest[destIndex + dpL + dpL] = Interpolation.Mix5To3(patch[4], patch[3]);
                                dest[destIndex + dpL + dpL + 1] = Interpolation.Mix7To1(patch[4], patch[3]);
                                dest[destIndex + dpL + dpL + 2] = Interpolation.Mix7To1(patch[4], patch[5]);
                                dest[destIndex + dpL + dpL + 3] = Interpolation.Mix5To3(patch[4], patch[5]);
                                dest[destIndex + dpL + dpL + dpL] = Interpolation.Mix5To3(patch[4], patch[3]);
                                dest[destIndex + dpL + dpL + dpL + 1] = Interpolation.Mix7To1(patch[4], patch[3]);
                                dest[destIndex + dpL + dpL + dpL + 2] = Interpolation.Mix7To1(patch[4], patch[5]);
                                dest[destIndex + dpL + dpL + dpL + 3] = Interpolation.Mix5To3(patch[4], patch[5]);
                                break;
                            }
                        case 126:
                            {
                                dest[destIndex] = Interpolation.Mix5To3(patch[4], patch[0]);
                                dest[destIndex + 1] = Interpolation.Mix3To1(patch[4], patch[0]);
                                if (Diff(patch[1], patch[5], trY, trU, trV, trA))
                                {
                                    dest[destIndex + 2] = patch[4];
                                    dest[destIndex + 3] = patch[4];
                                    dest[destIndex + dpL + 3] = patch[4];
                                }
                                else
                                {
                                    dest[destIndex + 2] = Interpolation.MixEven(patch[1], patch[4]);
                                    dest[destIndex + 3] = Interpolation.MixEven(patch[1], patch[5]);
                                    dest[destIndex + dpL + 3] = Interpolation.MixEven(patch[5], patch[4]);
                                }
                                dest[destIndex + dpL] = Interpolation.Mix3To1(patch[4], patch[0]);
                                dest[destIndex + dpL + 1] = Interpolation.Mix7To1(patch[4], patch[0]);
                                dest[destIndex + dpL + 2] = patch[4];
                                if (Diff(patch[7], patch[3], trY, trU, trV, trA))
                                {
                                    dest[destIndex + dpL + dpL] = patch[4];
                                    dest[destIndex + dpL + dpL + dpL] = patch[4];
                                    dest[destIndex + dpL + dpL + dpL + 1] = patch[4];
                                }
                                else
                                {
                                    dest[destIndex + dpL + dpL] = Interpolation.MixEven(patch[3], patch[4]);
                                    dest[destIndex + dpL + dpL + dpL] = Interpolation.MixEven(patch[7], patch[3]);
                                    dest[destIndex + dpL + dpL + dpL + 1] = Interpolation.MixEven(patch[7], patch[4]);
                                }
                                dest[destIndex + dpL + dpL + 1] = patch[4];
                                dest[destIndex + dpL + dpL + 2] = Interpolation.Mix7To1(patch[4], patch[8]);
                                dest[destIndex + dpL + dpL + 3] = Interpolation.Mix3To1(patch[4], patch[8]);
                                dest[destIndex + dpL + dpL + dpL + 2] = Interpolation.Mix3To1(patch[4], patch[8]);
                                dest[destIndex + dpL + dpL + dpL + 3] = Interpolation.Mix5To3(patch[4], patch[8]);
                                break;
                            }
                        case 219:
                            {
                                if (Diff(patch[3], patch[1], trY, trU, trV, trA))
                                {
                                    dest[destIndex] = patch[4];
                                    dest[destIndex + 1] = patch[4];
                                    dest[destIndex + dpL] = patch[4];
                                }
                                else
                                {
                                    dest[destIndex] = Interpolation.MixEven(patch[1], patch[3]);
                                    dest[destIndex + 1] = Interpolation.MixEven(patch[1], patch[4]);
                                    dest[destIndex + dpL] = Interpolation.MixEven(patch[3], patch[4]);
                                }
                                dest[destIndex + 2] = Interpolation.Mix3To1(patch[4], patch[2]);
                                dest[destIndex + 3] = Interpolation.Mix5To3(patch[4], patch[2]);
                                dest[destIndex + dpL + 1] = patch[4];
                                dest[destIndex + dpL + 2] = Interpolation.Mix7To1(patch[4], patch[2]);
                                dest[destIndex + dpL + 3] = Interpolation.Mix3To1(patch[4], patch[2]);
                                dest[destIndex + dpL + dpL] = Interpolation.Mix3To1(patch[4], patch[6]);
                                dest[destIndex + dpL + dpL + 1] = Interpolation.Mix7To1(patch[4], patch[6]);
                                dest[destIndex + dpL + dpL + 2] = patch[4];
                                if (Diff(patch[5], patch[7], trY, trU, trV, trA))
                                {
                                    dest[destIndex + dpL + dpL + 3] = patch[4];
                                    dest[destIndex + dpL + dpL + dpL + 2] = patch[4];
                                    dest[destIndex + dpL + dpL + dpL + 3] = patch[4];
                                }
                                else
                                {
                                    dest[destIndex + dpL + dpL + 3] = Interpolation.MixEven(patch[5], patch[4]);
                                    dest[destIndex + dpL + dpL + dpL + 2] = Interpolation.MixEven(patch[7], patch[4]);
                                    dest[destIndex + dpL + dpL + dpL + 3] = Interpolation.MixEven(patch[7], patch[5]);
                                }
                                dest[destIndex + dpL + dpL + dpL] = Interpolation.Mix5To3(patch[4], patch[6]);
                                dest[destIndex + dpL + dpL + dpL + 1] = Interpolation.Mix3To1(patch[4], patch[6]);
                                break;
                            }
                        case 125:
                            {
                                if (Diff(patch[7], patch[3], trY, trU, trV, trA))
                                {
                                    dest[destIndex] = Interpolation.Mix5To3(patch[4], patch[1]);
                                    dest[destIndex + dpL] = Interpolation.Mix7To1(patch[4], patch[1]);
                                    dest[destIndex + dpL + dpL] = patch[4];
                                    dest[destIndex + dpL + dpL + 1] = patch[4];
                                    dest[destIndex + dpL + dpL + dpL] = patch[4];
                                    dest[destIndex + dpL + dpL + dpL + 1] = patch[4];
                                }
                                else
                                {
                                    dest[destIndex] = Interpolation.Mix3To1(patch[4], patch[3]);
                                    dest[destIndex + dpL] = Interpolation.Mix3To1(patch[3], patch[4]);
                                    dest[destIndex + dpL + dpL] = Interpolation.Mix5To3(patch[3], patch[7]);
                                    dest[destIndex + dpL + dpL + 1] = Interpolation.Mix6To1To1(patch[4], patch[3], patch[7]);
                                    dest[destIndex + dpL + dpL + dpL] = Interpolation.MixEven(patch[7], patch[3]);
                                    dest[destIndex + dpL + dpL + dpL + 1] = Interpolation.Mix2To1To1(patch[7], patch[4], patch[3]);
                                }
                                dest[destIndex + 1] = Interpolation.Mix5To3(patch[4], patch[1]);
                                dest[destIndex + 2] = Interpolation.Mix5To3(patch[4], patch[1]);
                                dest[destIndex + 3] = Interpolation.Mix5To3(patch[4], patch[1]);
                                dest[destIndex + dpL + 1] = Interpolation.Mix7To1(patch[4], patch[1]);
                                dest[destIndex + dpL + 2] = Interpolation.Mix7To1(patch[4], patch[1]);
                                dest[destIndex + dpL + 3] = Interpolation.Mix7To1(patch[4], patch[1]);
                                dest[destIndex + dpL + dpL + 2] = Interpolation.Mix7To1(patch[4], patch[8]);
                                dest[destIndex + dpL + dpL + 3] = Interpolation.Mix3To1(patch[4], patch[8]);
                                dest[destIndex + dpL + dpL + dpL + 2] = Interpolation.Mix3To1(patch[4], patch[8]);
                                dest[destIndex + dpL + dpL + dpL + 3] = Interpolation.Mix5To3(patch[4], patch[8]);
                                break;
                            }
                        case 221:
                            {
                                dest[destIndex] = Interpolation.Mix5To3(patch[4], patch[1]);
                                dest[destIndex + 1] = Interpolation.Mix5To3(patch[4], patch[1]);
                                dest[destIndex + 2] = Interpolation.Mix5To3(patch[4], patch[1]);
                                if (Diff(patch[5], patch[7], trY, trU, trV, trA))
                                {
                                    dest[destIndex + 3] = Interpolation.Mix5To3(patch[4], patch[1]);
                                    dest[destIndex + dpL + 3] = Interpolation.Mix7To1(patch[4], patch[1]);
                                    dest[destIndex + dpL + dpL + 2] = patch[4];
                                    dest[destIndex + dpL + dpL + 3] = patch[4];
                                    dest[destIndex + dpL + dpL + dpL + 2] = patch[4];
                                    dest[destIndex + dpL + dpL + dpL + 3] = patch[4];
                                }
                                else
                                {
                                    dest[destIndex + 3] = Interpolation.Mix3To1(patch[4], patch[5]);
                                    dest[destIndex + dpL + 3] = Interpolation.Mix3To1(patch[5], patch[4]);
                                    dest[destIndex + dpL + dpL + 2] = Interpolation.Mix6To1To1(patch[4], patch[5], patch[7]);
                                    dest[destIndex + dpL + dpL + 3] = Interpolation.Mix5To3(patch[5], patch[7]);
                                    dest[destIndex + dpL + dpL + dpL + 2] = Interpolation.Mix2To1To1(patch[7], patch[4], patch[5]);
                                    dest[destIndex + dpL + dpL + dpL + 3] = Interpolation.MixEven(patch[7], patch[5]);
                                }
                                dest[destIndex + dpL] = Interpolation.Mix7To1(patch[4], patch[1]);
                                dest[destIndex + dpL + 1] = Interpolation.Mix7To1(patch[4], patch[1]);
                                dest[destIndex + dpL + 2] = Interpolation.Mix7To1(patch[4], patch[1]);
                                dest[destIndex + dpL + dpL] = Interpolation.Mix3To1(patch[4], patch[6]);
                                dest[destIndex + dpL + dpL + 1] = Interpolation.Mix7To1(patch[4], patch[6]);
                                dest[destIndex + dpL + dpL + dpL] = Interpolation.Mix5To3(patch[4], patch[6]);
                                dest[destIndex + dpL + dpL + dpL + 1] = Interpolation.Mix3To1(patch[4], patch[6]);
                                break;
                            }
                        case 207:
                            {
                                if (Diff(patch[3], patch[1], trY, trU, trV, trA))
                                {
                                    dest[destIndex] = patch[4];
                                    dest[destIndex + 1] = patch[4];
                                    dest[destIndex + 2] = Interpolation.Mix7To1(patch[4], patch[5]);
                                    dest[destIndex + 3] = Interpolation.Mix5To3(patch[4], patch[5]);
                                    dest[destIndex + dpL] = patch[4];
                                    dest[destIndex + dpL + 1] = patch[4];
                                }
                                else
                                {
                                    dest[destIndex] = Interpolation.MixEven(patch[1], patch[3]);
                                    dest[destIndex + 1] = Interpolation.Mix5To3(patch[1], patch[3]);
                                    dest[destIndex + 2] = Interpolation.Mix3To1(patch[1], patch[4]);
                                    dest[destIndex + 3] = Interpolation.Mix3To1(patch[4], patch[1]);
                                    dest[destIndex + dpL] = Interpolation.Mix2To1To1(patch[3], patch[4], patch[1]);
                                    dest[destIndex + dpL + 1] = Interpolation.Mix6To1To1(patch[4], patch[3], patch[1]);
                                }
                                dest[destIndex + dpL + 2] = Interpolation.Mix7To1(patch[4], patch[5]);
                                dest[destIndex + dpL + 3] = Interpolation.Mix5To3(patch[4], patch[5]);
                                dest[destIndex + dpL + dpL] = Interpolation.Mix3To1(patch[4], patch[6]);
                                dest[destIndex + dpL + dpL + 1] = Interpolation.Mix7To1(patch[4], patch[6]);
                                dest[destIndex + dpL + dpL + 2] = Interpolation.Mix7To1(patch[4], patch[5]);
                                dest[destIndex + dpL + dpL + 3] = Interpolation.Mix5To3(patch[4], patch[5]);
                                dest[destIndex + dpL + dpL + dpL] = Interpolation.Mix5To3(patch[4], patch[6]);
                                dest[destIndex + dpL + dpL + dpL + 1] = Interpolation.Mix3To1(patch[4], patch[6]);
                                dest[destIndex + dpL + dpL + dpL + 2] = Interpolation.Mix7To1(patch[4], patch[5]);
                                dest[destIndex + dpL + dpL + dpL + 3] = Interpolation.Mix5To3(patch[4], patch[5]);
                                break;
                            }
                        case 238:
                            {
                                dest[destIndex] = Interpolation.Mix5To3(patch[4], patch[0]);
                                dest[destIndex + 1] = Interpolation.Mix3To1(patch[4], patch[0]);
                                dest[destIndex + 2] = Interpolation.Mix7To1(patch[4], patch[5]);
                                dest[destIndex + 3] = Interpolation.Mix5To3(patch[4], patch[5]);
                                dest[destIndex + dpL] = Interpolation.Mix3To1(patch[4], patch[0]);
                                dest[destIndex + dpL + 1] = Interpolation.Mix7To1(patch[4], patch[0]);
                                dest[destIndex + dpL + 2] = Interpolation.Mix7To1(patch[4], patch[5]);
                                dest[destIndex + dpL + 3] = Interpolation.Mix5To3(patch[4], patch[5]);
                                if (Diff(patch[7], patch[3], trY, trU, trV, trA))
                                {
                                    dest[destIndex + dpL + dpL] = patch[4];
                                    dest[destIndex + dpL + dpL + 1] = patch[4];
                                    dest[destIndex + dpL + dpL + dpL] = patch[4];
                                    dest[destIndex + dpL + dpL + dpL + 1] = patch[4];
                                    dest[destIndex + dpL + dpL + dpL + 2] = Interpolation.Mix7To1(patch[4], patch[5]);
                                    dest[destIndex + dpL + dpL + dpL + 3] = Interpolation.Mix5To3(patch[4], patch[5]);
                                }
                                else
                                {
                                    dest[destIndex + dpL + dpL] = Interpolation.Mix2To1To1(patch[3], patch[4], patch[7]);
                                    dest[destIndex + dpL + dpL + 1] = Interpolation.Mix6To1To1(patch[4], patch[3], patch[7]);
                                    dest[destIndex + dpL + dpL + dpL] = Interpolation.MixEven(patch[7], patch[3]);
                                    dest[destIndex + dpL + dpL + dpL + 1] = Interpolation.Mix5To3(patch[7], patch[3]);
                                    dest[destIndex + dpL + dpL + dpL + 2] = Interpolation.Mix3To1(patch[7], patch[4]);
                                    dest[destIndex + dpL + dpL + dpL + 3] = Interpolation.Mix3To1(patch[4], patch[7]);
                                }
                                dest[destIndex + dpL + dpL + 2] = Interpolation.Mix7To1(patch[4], patch[5]);
                                dest[destIndex + dpL + dpL + 3] = Interpolation.Mix5To3(patch[4], patch[5]);
                                break;
                            }
                        case 190:
                            {
                                dest[destIndex] = Interpolation.Mix5To3(patch[4], patch[0]);
                                dest[destIndex + 1] = Interpolation.Mix3To1(patch[4], patch[0]);
                                if (Diff(patch[1], patch[5], trY, trU, trV, trA))
                                {
                                    dest[destIndex + 2] = patch[4];
                                    dest[destIndex + 3] = patch[4];
                                    dest[destIndex + dpL + 2] = patch[4];
                                    dest[destIndex + dpL + 3] = patch[4];
                                    dest[destIndex + dpL + dpL + 3] = Interpolation.Mix7To1(patch[4], patch[7]);
                                    dest[destIndex + dpL + dpL + dpL + 3] = Interpolation.Mix5To3(patch[4], patch[7]);
                                }
                                else
                                {
                                    dest[destIndex + 2] = Interpolation.Mix2To1To1(patch[1], patch[4], patch[5]);
                                    dest[destIndex + 3] = Interpolation.MixEven(patch[1], patch[5]);
                                    dest[destIndex + dpL + 2] = Interpolation.Mix6To1To1(patch[4], patch[5], patch[1]);
                                    dest[destIndex + dpL + 3] = Interpolation.Mix5To3(patch[5], patch[1]);
                                    dest[destIndex + dpL + dpL + 3] = Interpolation.Mix3To1(patch[5], patch[4]);
                                    dest[destIndex + dpL + dpL + dpL + 3] = Interpolation.Mix3To1(patch[4], patch[5]);
                                }
                                dest[destIndex + dpL] = Interpolation.Mix3To1(patch[4], patch[0]);
                                dest[destIndex + dpL + 1] = Interpolation.Mix7To1(patch[4], patch[0]);
                                dest[destIndex + dpL + dpL] = Interpolation.Mix7To1(patch[4], patch[7]);
                                dest[destIndex + dpL + dpL + 1] = Interpolation.Mix7To1(patch[4], patch[7]);
                                dest[destIndex + dpL + dpL + 2] = Interpolation.Mix7To1(patch[4], patch[7]);
                                dest[destIndex + dpL + dpL + dpL] = Interpolation.Mix5To3(patch[4], patch[7]);
                                dest[destIndex + dpL + dpL + dpL + 1] = Interpolation.Mix5To3(patch[4], patch[7]);
                                dest[destIndex + dpL + dpL + dpL + 2] = Interpolation.Mix5To3(patch[4], patch[7]);
                                break;
                            }
                        case 187:
                            {
                                if (Diff(patch[3], patch[1], trY, trU, trV, trA))
                                {
                                    dest[destIndex] = patch[4];
                                    dest[destIndex + 1] = patch[4];
                                    dest[destIndex + dpL] = patch[4];
                                    dest[destIndex + dpL + 1] = patch[4];
                                    dest[destIndex + dpL + dpL] = Interpolation.Mix7To1(patch[4], patch[7]);
                                    dest[destIndex + dpL + dpL + dpL] = Interpolation.Mix5To3(patch[4], patch[7]);
                                }
                                else
                                {
                                    dest[destIndex] = Interpolation.MixEven(patch[1], patch[3]);
                                    dest[destIndex + 1] = Interpolation.Mix2To1To1(patch[1], patch[4], patch[3]);
                                    dest[destIndex + dpL] = Interpolation.Mix5To3(patch[3], patch[1]);
                                    dest[destIndex + dpL + 1] = Interpolation.Mix6To1To1(patch[4], patch[3], patch[1]);
                                    dest[destIndex + dpL + dpL] = Interpolation.Mix3To1(patch[3], patch[4]);
                                    dest[destIndex + dpL + dpL + dpL] = Interpolation.Mix3To1(patch[4], patch[3]);
                                }
                                dest[destIndex + 2] = Interpolation.Mix3To1(patch[4], patch[2]);
                                dest[destIndex + 3] = Interpolation.Mix5To3(patch[4], patch[2]);
                                dest[destIndex + dpL + 2] = Interpolation.Mix7To1(patch[4], patch[2]);
                                dest[destIndex + dpL + 3] = Interpolation.Mix3To1(patch[4], patch[2]);
                                dest[destIndex + dpL + dpL + 1] = Interpolation.Mix7To1(patch[4], patch[7]);
                                dest[destIndex + dpL + dpL + 2] = Interpolation.Mix7To1(patch[4], patch[7]);
                                dest[destIndex + dpL + dpL + 3] = Interpolation.Mix7To1(patch[4], patch[7]);
                                dest[destIndex + dpL + dpL + dpL + 1] = Interpolation.Mix5To3(patch[4], patch[7]);
                                dest[destIndex + dpL + dpL + dpL + 2] = Interpolation.Mix5To3(patch[4], patch[7]);
                                dest[destIndex + dpL + dpL + dpL + 3] = Interpolation.Mix5To3(patch[4], patch[7]);
                                break;
                            }
                        case 243:
                            {
                                dest[destIndex] = Interpolation.Mix5To3(patch[4], patch[3]);
                                dest[destIndex + 1] = Interpolation.Mix7To1(patch[4], patch[3]);
                                dest[destIndex + 2] = Interpolation.Mix3To1(patch[4], patch[2]);
                                dest[destIndex + 3] = Interpolation.Mix5To3(patch[4], patch[2]);
                                dest[destIndex + dpL] = Interpolation.Mix5To3(patch[4], patch[3]);
                                dest[destIndex + dpL + 1] = Interpolation.Mix7To1(patch[4], patch[3]);
                                dest[destIndex + dpL + 2] = Interpolation.Mix7To1(patch[4], patch[2]);
                                dest[destIndex + dpL + 3] = Interpolation.Mix3To1(patch[4], patch[2]);
                                dest[destIndex + dpL + dpL] = Interpolation.Mix5To3(patch[4], patch[3]);
                                dest[destIndex + dpL + dpL + 1] = Interpolation.Mix7To1(patch[4], patch[3]);
                                if (Diff(patch[5], patch[7], trY, trU, trV, trA))
                                {
                                    dest[destIndex + dpL + dpL + 2] = patch[4];
                                    dest[destIndex + dpL + dpL + 3] = patch[4];
                                    dest[destIndex + dpL + dpL + dpL] = Interpolation.Mix5To3(patch[4], patch[3]);
                                    dest[destIndex + dpL + dpL + dpL + 1] = Interpolation.Mix7To1(patch[4], patch[3]);
                                    dest[destIndex + dpL + dpL + dpL + 2] = patch[4];
                                    dest[destIndex + dpL + dpL + dpL + 3] = patch[4];
                                }
                                else
                                {
                                    dest[destIndex + dpL + dpL + 2] = Interpolation.Mix6To1To1(patch[4], patch[5], patch[7]);
                                    dest[destIndex + dpL + dpL + 3] = Interpolation.Mix2To1To1(patch[5], patch[4], patch[7]);
                                    dest[destIndex + dpL + dpL + dpL] = Interpolation.Mix3To1(patch[4], patch[7]);
                                    dest[destIndex + dpL + dpL + dpL + 1] = Interpolation.Mix3To1(patch[7], patch[4]);
                                    dest[destIndex + dpL + dpL + dpL + 2] = Interpolation.Mix5To3(patch[7], patch[5]);
                                    dest[destIndex + dpL + dpL + dpL + 3] = Interpolation.MixEven(patch[7], patch[5]);
                                }
                                break;
                            }
                        case 119:
                            {
                                if (Diff(patch[1], patch[5], trY, trU, trV, trA))
                                {
                                    dest[destIndex] = Interpolation.Mix5To3(patch[4], patch[3]);
                                    dest[destIndex + 1] = Interpolation.Mix7To1(patch[4], patch[3]);
                                    dest[destIndex + 2] = patch[4];
                                    dest[destIndex + 3] = patch[4];
                                    dest[destIndex + dpL + 2] = patch[4];
                                    dest[destIndex + dpL + 3] = patch[4];
                                }
                                else
                                {
                                    dest[destIndex] = Interpolation.Mix3To1(patch[4], patch[1]);
                                    dest[destIndex + 1] = Interpolation.Mix3To1(patch[1], patch[4]);
                                    dest[destIndex + 2] = Interpolation.Mix5To3(patch[1], patch[5]);
                                    dest[destIndex + 3] = Interpolation.MixEven(patch[1], patch[5]);
                                    dest[destIndex + dpL + 2] = Interpolation.Mix6To1To1(patch[4], patch[5], patch[1]);
                                    dest[destIndex + dpL + 3] = Interpolation.Mix2To1To1(patch[5], patch[4], patch[1]);
                                }
                                dest[destIndex + dpL] = Interpolation.Mix5To3(patch[4], patch[3]);
                                dest[destIndex + dpL + 1] = Interpolation.Mix7To1(patch[4], patch[3]);
                                dest[destIndex + dpL + dpL] = Interpolation.Mix5To3(patch[4], patch[3]);
                                dest[destIndex + dpL + dpL + 1] = Interpolation.Mix7To1(patch[4], patch[3]);
                                dest[destIndex + dpL + dpL + 2] = Interpolation.Mix7To1(patch[4], patch[8]);
                                dest[destIndex + dpL + dpL + 3] = Interpolation.Mix3To1(patch[4], patch[8]);
                                dest[destIndex + dpL + dpL + dpL] = Interpolation.Mix5To3(patch[4], patch[3]);
                                dest[destIndex + dpL + dpL + dpL + 1] = Interpolation.Mix7To1(patch[4], patch[3]);
                                dest[destIndex + dpL + dpL + dpL + 2] = Interpolation.Mix3To1(patch[4], patch[8]);
                                dest[destIndex + dpL + dpL + dpL + 3] = Interpolation.Mix5To3(patch[4], patch[8]);
                                break;
                            }
                        case 237:
                        case 233:
                            {
                                dest[destIndex] = Interpolation.Mix5To3(patch[4], patch[1]);
                                dest[destIndex + 1] = Interpolation.Mix5To3(patch[4], patch[1]);
                                dest[destIndex + 2] = Interpolation.Mix5To2To1(patch[4], patch[1], patch[5]);
                                dest[destIndex + 3] = Interpolation.Mix2To1To1(patch[4], patch[1], patch[5]);
                                dest[destIndex + dpL] = Interpolation.Mix7To1(patch[4], patch[1]);
                                dest[destIndex + dpL + 1] = Interpolation.Mix7To1(patch[4], patch[1]);
                                dest[destIndex + dpL + 2] = Interpolation.Mix6To1To1(patch[4], patch[5], patch[1]);
                                dest[destIndex + dpL + 3] = Interpolation.Mix5To2To1(patch[4], patch[5], patch[1]);
                                dest[destIndex + dpL + dpL] = patch[4];
                                dest[destIndex + dpL + dpL + 1] = patch[4];
                                dest[destIndex + dpL + dpL + 2] = Interpolation.Mix7To1(patch[4], patch[5]);
                                dest[destIndex + dpL + dpL + 3] = Interpolation.Mix5To3(patch[4], patch[5]);
                                if (Diff(patch[7], patch[3], trY, trU, trV, trA))
                                {
                                    dest[destIndex + dpL + dpL + dpL] = patch[4];
                                }
                                else
                                {
                                    dest[destIndex + dpL + dpL + dpL] = Interpolation.Mix2To1To1(patch[4], patch[7], patch[3]);
                                }
                                dest[destIndex + dpL + dpL + dpL + 1] = patch[4];
                                dest[destIndex + dpL + dpL + dpL + 2] = Interpolation.Mix7To1(patch[4], patch[5]);
                                dest[destIndex + dpL + dpL + dpL + 3] = Interpolation.Mix5To3(patch[4], patch[5]);
                                break;
                            }
                        case 175:
                        case 47:
                            {
                                if (Diff(patch[3], patch[1], trY, trU, trV, trA))
                                {
                                    dest[destIndex] = patch[4];
                                }
                                else
                                {
                                    dest[destIndex] = Interpolation.Mix2To1To1(patch[4], patch[1], patch[3]);
                                }
                                dest[destIndex + 1] = patch[4];
                                dest[destIndex + 2] = Interpolation.Mix7To1(patch[4], patch[5]);
                                dest[destIndex + 3] = Interpolation.Mix5To3(patch[4], patch[5]);
                                dest[destIndex + dpL] = patch[4];
                                dest[destIndex + dpL + 1] = patch[4];
                                dest[destIndex + dpL + 2] = Interpolation.Mix7To1(patch[4], patch[5]);
                                dest[destIndex + dpL + 3] = Interpolation.Mix5To3(patch[4], patch[5]);
                                dest[destIndex + dpL + dpL] = Interpolation.Mix7To1(patch[4], patch[7]);
                                dest[destIndex + dpL + dpL + 1] = Interpolation.Mix7To1(patch[4], patch[7]);
                                dest[destIndex + dpL + dpL + 2] = Interpolation.Mix6To1To1(patch[4], patch[5], patch[7]);
                                dest[destIndex + dpL + dpL + 3] = Interpolation.Mix5To2To1(patch[4], patch[5], patch[7]);
                                dest[destIndex + dpL + dpL + dpL] = Interpolation.Mix5To3(patch[4], patch[7]);
                                dest[destIndex + dpL + dpL + dpL + 1] = Interpolation.Mix5To3(patch[4], patch[7]);
                                dest[destIndex + dpL + dpL + dpL + 2] = Interpolation.Mix5To2To1(patch[4], patch[7], patch[5]);
                                dest[destIndex + dpL + dpL + dpL + 3] = Interpolation.Mix2To1To1(patch[4], patch[7], patch[5]);
                                break;
                            }
                        case 183:
                        case 151:
                            {
                                dest[destIndex] = Interpolation.Mix5To3(patch[4], patch[3]);
                                dest[destIndex + 1] = Interpolation.Mix7To1(patch[4], patch[3]);
                                dest[destIndex + 2] = patch[4];
                                if (Diff(patch[1], patch[5], trY, trU, trV, trA))
                                {
                                    dest[destIndex + 3] = patch[4];
                                }
                                else
                                {
                                    dest[destIndex + 3] = Interpolation.Mix2To1To1(patch[4], patch[1], patch[5]);
                                }
                                dest[destIndex + dpL] = Interpolation.Mix5To3(patch[4], patch[3]);
                                dest[destIndex + dpL + 1] = Interpolation.Mix7To1(patch[4], patch[3]);
                                dest[destIndex + dpL + 2] = patch[4];
                                dest[destIndex + dpL + 3] = patch[4];
                                dest[destIndex + dpL + dpL] = Interpolation.Mix5To2To1(patch[4], patch[3], patch[7]);
                                dest[destIndex + dpL + dpL + 1] = Interpolation.Mix6To1To1(patch[4], patch[3], patch[7]);
                                dest[destIndex + dpL + dpL + 2] = Interpolation.Mix7To1(patch[4], patch[7]);
                                dest[destIndex + dpL + dpL + 3] = Interpolation.Mix7To1(patch[4], patch[7]);
                                dest[destIndex + dpL + dpL + dpL] = Interpolation.Mix2To1To1(patch[4], patch[7], patch[3]);
                                dest[destIndex + dpL + dpL + dpL + 1] = Interpolation.Mix5To2To1(patch[4], patch[7], patch[3]);
                                dest[destIndex + dpL + dpL + dpL + 2] = Interpolation.Mix5To3(patch[4], patch[7]);
                                dest[destIndex + dpL + dpL + dpL + 3] = Interpolation.Mix5To3(patch[4], patch[7]);
                                break;
                            }
                        case 245:
                        case 244:
                            {
                                dest[destIndex] = Interpolation.Mix2To1To1(patch[4], patch[1], patch[3]);
                                dest[destIndex + 1] = Interpolation.Mix5To2To1(patch[4], patch[1], patch[3]);
                                dest[destIndex + 2] = Interpolation.Mix5To3(patch[4], patch[1]);
                                dest[destIndex + 3] = Interpolation.Mix5To3(patch[4], patch[1]);
                                dest[destIndex + dpL] = Interpolation.Mix5To2To1(patch[4], patch[3], patch[1]);
                                dest[destIndex + dpL + 1] = Interpolation.Mix6To1To1(patch[4], patch[3], patch[1]);
                                dest[destIndex + dpL + 2] = Interpolation.Mix7To1(patch[4], patch[1]);
                                dest[destIndex + dpL + 3] = Interpolation.Mix7To1(patch[4], patch[1]);
                                dest[destIndex + dpL + dpL] = Interpolation.Mix5To3(patch[4], patch[3]);
                                dest[destIndex + dpL + dpL + 1] = Interpolation.Mix7To1(patch[4], patch[3]);
                                dest[destIndex + dpL + dpL + 2] = patch[4];
                                dest[destIndex + dpL + dpL + 3] = patch[4];
                                dest[destIndex + dpL + dpL + dpL] = Interpolation.Mix5To3(patch[4], patch[3]);
                                dest[destIndex + dpL + dpL + dpL + 1] = Interpolation.Mix7To1(patch[4], patch[3]);
                                dest[destIndex + dpL + dpL + dpL + 2] = patch[4];
                                if (Diff(patch[5], patch[7], trY, trU, trV, trA))
                                {
                                    dest[destIndex + dpL + dpL + dpL + 3] = patch[4];
                                }
                                else
                                {
                                    dest[destIndex + dpL + dpL + dpL + 3] = Interpolation.Mix2To1To1(patch[4], patch[7], patch[5]);
                                }
                                break;
                            }
                        case 250:
                            {
                                dest[destIndex] = Interpolation.Mix5To3(patch[4], patch[0]);
                                dest[destIndex + 1] = Interpolation.Mix3To1(patch[4], patch[0]);
                                dest[destIndex + 2] = Interpolation.Mix3To1(patch[4], patch[2]);
                                dest[destIndex + 3] = Interpolation.Mix5To3(patch[4], patch[2]);
                                dest[destIndex + dpL] = Interpolation.Mix3To1(patch[4], patch[0]);
                                dest[destIndex + dpL + 1] = Interpolation.Mix7To1(patch[4], patch[0]);
                                dest[destIndex + dpL + 2] = Interpolation.Mix7To1(patch[4], patch[2]);
                                dest[destIndex + dpL + 3] = Interpolation.Mix3To1(patch[4], patch[2]);
                                if (Diff(patch[7], patch[3], trY, trU, trV, trA))
                                {
                                    dest[destIndex + dpL + dpL] = patch[4];
                                    dest[destIndex + dpL + dpL + dpL] = patch[4];
                                    dest[destIndex + dpL + dpL + dpL + 1] = patch[4];
                                }
                                else
                                {
                                    dest[destIndex + dpL + dpL] = Interpolation.MixEven(patch[3], patch[4]);
                                    dest[destIndex + dpL + dpL + dpL] = Interpolation.MixEven(patch[7], patch[3]);
                                    dest[destIndex + dpL + dpL + dpL + 1] = Interpolation.MixEven(patch[7], patch[4]);
                                }
                                dest[destIndex + dpL + dpL + 1] = patch[4];
                                dest[destIndex + dpL + dpL + 2] = patch[4];
                                if (Diff(patch[5], patch[7], trY, trU, trV, trA))
                                {
                                    dest[destIndex + dpL + dpL + 3] = patch[4];
                                    dest[destIndex + dpL + dpL + dpL + 2] = patch[4];
                                    dest[destIndex + dpL + dpL + dpL + 3] = patch[4];
                                }
                                else
                                {
                                    dest[destIndex + dpL + dpL + 3] = Interpolation.MixEven(patch[5], patch[4]);
                                    dest[destIndex + dpL + dpL + dpL + 2] = Interpolation.MixEven(patch[7], patch[4]);
                                    dest[destIndex + dpL + dpL + dpL + 3] = Interpolation.MixEven(patch[7], patch[5]);
                                }
                                break;
                            }
                        case 123:
                            {
                                if (Diff(patch[3], patch[1], trY, trU, trV, trA))
                                {
                                    dest[destIndex] = patch[4];
                                    dest[destIndex + 1] = patch[4];
                                    dest[destIndex + dpL] = patch[4];
                                }
                                else
                                {
                                    dest[destIndex] = Interpolation.MixEven(patch[1], patch[3]);
                                    dest[destIndex + 1] = Interpolation.MixEven(patch[1], patch[4]);
                                    dest[destIndex + dpL] = Interpolation.MixEven(patch[3], patch[4]);
                                }
                                dest[destIndex + 2] = Interpolation.Mix3To1(patch[4], patch[2]);
                                dest[destIndex + 3] = Interpolation.Mix5To3(patch[4], patch[2]);
                                dest[destIndex + dpL + 1] = patch[4];
                                dest[destIndex + dpL + 2] = Interpolation.Mix7To1(patch[4], patch[2]);
                                dest[destIndex + dpL + 3] = Interpolation.Mix3To1(patch[4], patch[2]);
                                if (Diff(patch[7], patch[3], trY, trU, trV, trA))
                                {
                                    dest[destIndex + dpL + dpL] = patch[4];
                                    dest[destIndex + dpL + dpL + dpL] = patch[4];
                                    dest[destIndex + dpL + dpL + dpL + 1] = patch[4];
                                }
                                else
                                {
                                    dest[destIndex + dpL + dpL] = Interpolation.MixEven(patch[3], patch[4]);
                                    dest[destIndex + dpL + dpL + dpL] = Interpolation.MixEven(patch[7], patch[3]);
                                    dest[destIndex + dpL + dpL + dpL + 1] = Interpolation.MixEven(patch[7], patch[4]);
                                }
                                dest[destIndex + dpL + dpL + 1] = patch[4];
                                dest[destIndex + dpL + dpL + 2] = Interpolation.Mix7To1(patch[4], patch[8]);
                                dest[destIndex + dpL + dpL + 3] = Interpolation.Mix3To1(patch[4], patch[8]);
                                dest[destIndex + dpL + dpL + dpL + 2] = Interpolation.Mix3To1(patch[4], patch[8]);
                                dest[destIndex + dpL + dpL + dpL + 3] = Interpolation.Mix5To3(patch[4], patch[8]);
                                break;
                            }
                        case 95:
                            {
                                if (Diff(patch[3], patch[1], trY, trU, trV, trA))
                                {
                                    dest[destIndex] = patch[4];
                                    dest[destIndex + 1] = patch[4];
                                    dest[destIndex + dpL] = patch[4];
                                }
                                else
                                {
                                    dest[destIndex] = Interpolation.MixEven(patch[1], patch[3]);
                                    dest[destIndex + 1] = Interpolation.MixEven(patch[1], patch[4]);
                                    dest[destIndex + dpL] = Interpolation.MixEven(patch[3], patch[4]);
                                }
                                if (Diff(patch[1], patch[5], trY, trU, trV, trA))
                                {
                                    dest[destIndex + 2] = patch[4];
                                    dest[destIndex + 3] = patch[4];
                                    dest[destIndex + dpL + 3] = patch[4];
                                }
                                else
                                {
                                    dest[destIndex + 2] = Interpolation.MixEven(patch[1], patch[4]);
                                    dest[destIndex + 3] = Interpolation.MixEven(patch[1], patch[5]);
                                    dest[destIndex + dpL + 3] = Interpolation.MixEven(patch[5], patch[4]);
                                }
                                dest[destIndex + dpL + 1] = patch[4];
                                dest[destIndex + dpL + 2] = patch[4];
                                dest[destIndex + dpL + dpL] = Interpolation.Mix3To1(patch[4], patch[6]);
                                dest[destIndex + dpL + dpL + 1] = Interpolation.Mix7To1(patch[4], patch[6]);
                                dest[destIndex + dpL + dpL + 2] = Interpolation.Mix7To1(patch[4], patch[8]);
                                dest[destIndex + dpL + dpL + 3] = Interpolation.Mix3To1(patch[4], patch[8]);
                                dest[destIndex + dpL + dpL + dpL] = Interpolation.Mix5To3(patch[4], patch[6]);
                                dest[destIndex + dpL + dpL + dpL + 1] = Interpolation.Mix3To1(patch[4], patch[6]);
                                dest[destIndex + dpL + dpL + dpL + 2] = Interpolation.Mix3To1(patch[4], patch[8]);
                                dest[destIndex + dpL + dpL + dpL + 3] = Interpolation.Mix5To3(patch[4], patch[8]);
                                break;
                            }
                        case 222:
                            {
                                dest[destIndex] = Interpolation.Mix5To3(patch[4], patch[0]);
                                dest[destIndex + 1] = Interpolation.Mix3To1(patch[4], patch[0]);
                                if (Diff(patch[1], patch[5], trY, trU, trV, trA))
                                {
                                    dest[destIndex + 2] = patch[4];
                                    dest[destIndex + 3] = patch[4];
                                    dest[destIndex + dpL + 3] = patch[4];
                                }
                                else
                                {
                                    dest[destIndex + 2] = Interpolation.MixEven(patch[1], patch[4]);
                                    dest[destIndex + 3] = Interpolation.MixEven(patch[1], patch[5]);
                                    dest[destIndex + dpL + 3] = Interpolation.MixEven(patch[5], patch[4]);
                                }
                                dest[destIndex + dpL] = Interpolation.Mix3To1(patch[4], patch[0]);
                                dest[destIndex + dpL + 1] = Interpolation.Mix7To1(patch[4], patch[0]);
                                dest[destIndex + dpL + 2] = patch[4];
                                dest[destIndex + dpL + dpL] = Interpolation.Mix3To1(patch[4], patch[6]);
                                dest[destIndex + dpL + dpL + 1] = Interpolation.Mix7To1(patch[4], patch[6]);
                                dest[destIndex + dpL + dpL + 2] = patch[4];
                                if (Diff(patch[5], patch[7], trY, trU, trV, trA))
                                {
                                    dest[destIndex + dpL + dpL + 3] = patch[4];
                                    dest[destIndex + dpL + dpL + dpL + 2] = patch[4];
                                    dest[destIndex + dpL + dpL + dpL + 3] = patch[4];
                                }
                                else
                                {
                                    dest[destIndex + dpL + dpL + 3] = Interpolation.MixEven(patch[5], patch[4]);
                                    dest[destIndex + dpL + dpL + dpL + 2] = Interpolation.MixEven(patch[7], patch[4]);
                                    dest[destIndex + dpL + dpL + dpL + 3] = Interpolation.MixEven(patch[7], patch[5]);
                                }
                                dest[destIndex + dpL + dpL + dpL] = Interpolation.Mix5To3(patch[4], patch[6]);
                                dest[destIndex + dpL + dpL + dpL + 1] = Interpolation.Mix3To1(patch[4], patch[6]);
                                break;
                            }
                        case 252:
                            {
                                dest[destIndex] = Interpolation.Mix5To3(patch[4], patch[0]);
                                dest[destIndex + 1] = Interpolation.Mix5To2To1(patch[4], patch[1], patch[0]);
                                dest[destIndex + 2] = Interpolation.Mix5To3(patch[4], patch[1]);
                                dest[destIndex + 3] = Interpolation.Mix5To3(patch[4], patch[1]);
                                dest[destIndex + dpL] = Interpolation.Mix3To1(patch[4], patch[0]);
                                dest[destIndex + dpL + 1] = Interpolation.Mix7To1(patch[4], patch[0]);
                                dest[destIndex + dpL + 2] = Interpolation.Mix7To1(patch[4], patch[1]);
                                dest[destIndex + dpL + 3] = Interpolation.Mix7To1(patch[4], patch[1]);
                                if (Diff(patch[7], patch[3], trY, trU, trV, trA))
                                {
                                    dest[destIndex + dpL + dpL] = patch[4];
                                    dest[destIndex + dpL + dpL + dpL] = patch[4];
                                    dest[destIndex + dpL + dpL + dpL + 1] = patch[4];
                                }
                                else
                                {
                                    dest[destIndex + dpL + dpL] = Interpolation.MixEven(patch[3], patch[4]);
                                    dest[destIndex + dpL + dpL + dpL] = Interpolation.MixEven(patch[7], patch[3]);
                                    dest[destIndex + dpL + dpL + dpL + 1] = Interpolation.MixEven(patch[7], patch[4]);
                                }
                                dest[destIndex + dpL + dpL + 1] = patch[4];
                                dest[destIndex + dpL + dpL + 2] = patch[4];
                                dest[destIndex + dpL + dpL + 3] = patch[4];
                                dest[destIndex + dpL + dpL + dpL + 2] = patch[4];
                                if (Diff(patch[5], patch[7], trY, trU, trV, trA))
                                {
                                    dest[destIndex + dpL + dpL + dpL + 3] = patch[4];
                                }
                                else
                                {
                                    dest[destIndex + dpL + dpL + dpL + 3] = Interpolation.Mix2To1To1(patch[4], patch[7], patch[5]);
                                }
                                break;
                            }
                        case 249:
                            {
                                dest[destIndex] = Interpolation.Mix5To3(patch[4], patch[1]);
                                dest[destIndex + 1] = Interpolation.Mix5To3(patch[4], patch[1]);
                                dest[destIndex + 2] = Interpolation.Mix5To2To1(patch[4], patch[1], patch[2]);
                                dest[destIndex + 3] = Interpolation.Mix5To3(patch[4], patch[2]);
                                dest[destIndex + dpL] = Interpolation.Mix7To1(patch[4], patch[1]);
                                dest[destIndex + dpL + 1] = Interpolation.Mix7To1(patch[4], patch[1]);
                                dest[destIndex + dpL + 2] = Interpolation.Mix7To1(patch[4], patch[2]);
                                dest[destIndex + dpL + 3] = Interpolation.Mix3To1(patch[4], patch[2]);
                                dest[destIndex + dpL + dpL] = patch[4];
                                dest[destIndex + dpL + dpL + 1] = patch[4];
                                dest[destIndex + dpL + dpL + 2] = patch[4];
                                if (Diff(patch[5], patch[7], trY, trU, trV, trA))
                                {
                                    dest[destIndex + dpL + dpL + 3] = patch[4];
                                    dest[destIndex + dpL + dpL + dpL + 2] = patch[4];
                                    dest[destIndex + dpL + dpL + dpL + 3] = patch[4];
                                }
                                else
                                {
                                    dest[destIndex + dpL + dpL + 3] = Interpolation.MixEven(patch[5], patch[4]);
                                    dest[destIndex + dpL + dpL + dpL + 2] = Interpolation.MixEven(patch[7], patch[4]);
                                    dest[destIndex + dpL + dpL + dpL + 3] = Interpolation.MixEven(patch[7], patch[5]);
                                }
                                if (Diff(patch[7], patch[3], trY, trU, trV, trA))
                                {
                                    dest[destIndex + dpL + dpL + dpL] = patch[4];
                                }
                                else
                                {
                                    dest[destIndex + dpL + dpL + dpL] = Interpolation.Mix2To1To1(patch[4], patch[7], patch[3]);
                                }
                                dest[destIndex + dpL + dpL + dpL + 1] = patch[4];
                                break;
                            }
                        case 235:
                            {
                                if (Diff(patch[3], patch[1], trY, trU, trV, trA))
                                {
                                    dest[destIndex] = patch[4];
                                    dest[destIndex + 1] = patch[4];
                                    dest[destIndex + dpL] = patch[4];
                                }
                                else
                                {
                                    dest[destIndex] = Interpolation.MixEven(patch[1], patch[3]);
                                    dest[destIndex + 1] = Interpolation.MixEven(patch[1], patch[4]);
                                    dest[destIndex + dpL] = Interpolation.MixEven(patch[3], patch[4]);
                                }
                                dest[destIndex + 2] = Interpolation.Mix3To1(patch[4], patch[2]);
                                dest[destIndex + 3] = Interpolation.Mix5To3(patch[4], patch[2]);
                                dest[destIndex + dpL + 1] = patch[4];
                                dest[destIndex + dpL + 2] = Interpolation.Mix7To1(patch[4], patch[2]);
                                dest[destIndex + dpL + 3] = Interpolation.Mix5To2To1(patch[4], patch[5], patch[2]);
                                dest[destIndex + dpL + dpL] = patch[4];
                                dest[destIndex + dpL + dpL + 1] = patch[4];
                                dest[destIndex + dpL + dpL + 2] = Interpolation.Mix7To1(patch[4], patch[5]);
                                dest[destIndex + dpL + dpL + 3] = Interpolation.Mix5To3(patch[4], patch[5]);
                                if (Diff(patch[7], patch[3], trY, trU, trV, trA))
                                {
                                    dest[destIndex + dpL + dpL + dpL] = patch[4];
                                }
                                else
                                {
                                    dest[destIndex + dpL + dpL + dpL] = Interpolation.Mix2To1To1(patch[4], patch[7], patch[3]);
                                }
                                dest[destIndex + dpL + dpL + dpL + 1] = patch[4];
                                dest[destIndex + dpL + dpL + dpL + 2] = Interpolation.Mix7To1(patch[4], patch[5]);
                                dest[destIndex + dpL + dpL + dpL + 3] = Interpolation.Mix5To3(patch[4], patch[5]);
                                break;
                            }
                        case 111:
                            {
                                if (Diff(patch[3], patch[1], trY, trU, trV, trA))
                                {
                                    dest[destIndex] = patch[4];
                                }
                                else
                                {
                                    dest[destIndex] = Interpolation.Mix2To1To1(patch[4], patch[1], patch[3]);
                                }
                                dest[destIndex + 1] = patch[4];
                                dest[destIndex + 2] = Interpolation.Mix7To1(patch[4], patch[5]);
                                dest[destIndex + 3] = Interpolation.Mix5To3(patch[4], patch[5]);
                                dest[destIndex + dpL] = patch[4];
                                dest[destIndex + dpL + 1] = patch[4];
                                dest[destIndex + dpL + 2] = Interpolation.Mix7To1(patch[4], patch[5]);
                                dest[destIndex + dpL + 3] = Interpolation.Mix5To3(patch[4], patch[5]);
                                if (Diff(patch[7], patch[3], trY, trU, trV, trA))
                                {
                                    dest[destIndex + dpL + dpL] = patch[4];
                                    dest[destIndex + dpL + dpL + dpL] = patch[4];
                                    dest[destIndex + dpL + dpL + dpL + 1] = patch[4];
                                }
                                else
                                {
                                    dest[destIndex + dpL + dpL] = Interpolation.MixEven(patch[3], patch[4]);
                                    dest[destIndex + dpL + dpL + dpL] = Interpolation.MixEven(patch[7], patch[3]);
                                    dest[destIndex + dpL + dpL + dpL + 1] = Interpolation.MixEven(patch[7], patch[4]);
                                }
                                dest[destIndex + dpL + dpL + 1] = patch[4];
                                dest[destIndex + dpL + dpL + 2] = Interpolation.Mix7To1(patch[4], patch[8]);
                                dest[destIndex + dpL + dpL + 3] = Interpolation.Mix5To2To1(patch[4], patch[5], patch[8]);
                                dest[destIndex + dpL + dpL + dpL + 2] = Interpolation.Mix3To1(patch[4], patch[8]);
                                dest[destIndex + dpL + dpL + dpL + 3] = Interpolation.Mix5To3(patch[4], patch[8]);
                                break;
                            }
                        case 63:
                            {
                                if (Diff(patch[3], patch[1], trY, trU, trV, trA))
                                {
                                    dest[destIndex] = patch[4];
                                }
                                else
                                {
                                    dest[destIndex] = Interpolation.Mix2To1To1(patch[4], patch[1], patch[3]);
                                }
                                dest[destIndex + 1] = patch[4];
                                if (Diff(patch[1], patch[5], trY, trU, trV, trA))
                                {
                                    dest[destIndex + 2] = patch[4];
                                    dest[destIndex + 3] = patch[4];
                                    dest[destIndex + dpL + 3] = patch[4];
                                }
                                else
                                {
                                    dest[destIndex + 2] = Interpolation.MixEven(patch[1], patch[4]);
                                    dest[destIndex + 3] = Interpolation.MixEven(patch[1], patch[5]);
                                    dest[destIndex + dpL + 3] = Interpolation.MixEven(patch[5], patch[4]);
                                }
                                dest[destIndex + dpL] = patch[4];
                                dest[destIndex + dpL + 1] = patch[4];
                                dest[destIndex + dpL + 2] = patch[4];
                                dest[destIndex + dpL + dpL] = Interpolation.Mix7To1(patch[4], patch[7]);
                                dest[destIndex + dpL + dpL + 1] = Interpolation.Mix7To1(patch[4], patch[7]);
                                dest[destIndex + dpL + dpL + 2] = Interpolation.Mix7To1(patch[4], patch[8]);
                                dest[destIndex + dpL + dpL + 3] = Interpolation.Mix3To1(patch[4], patch[8]);
                                dest[destIndex + dpL + dpL + dpL] = Interpolation.Mix5To3(patch[4], patch[7]);
                                dest[destIndex + dpL + dpL + dpL + 1] = Interpolation.Mix5To3(patch[4], patch[7]);
                                dest[destIndex + dpL + dpL + dpL + 2] = Interpolation.Mix5To2To1(patch[4], patch[7], patch[8]);
                                dest[destIndex + dpL + dpL + dpL + 3] = Interpolation.Mix5To3(patch[4], patch[8]);
                                break;
                            }
                        case 159:
                            {
                                if (Diff(patch[3], patch[1], trY, trU, trV, trA))
                                {
                                    dest[destIndex] = patch[4];
                                    dest[destIndex + 1] = patch[4];
                                    dest[destIndex + dpL] = patch[4];
                                }
                                else
                                {
                                    dest[destIndex] = Interpolation.MixEven(patch[1], patch[3]);
                                    dest[destIndex + 1] = Interpolation.MixEven(patch[1], patch[4]);
                                    dest[destIndex + dpL] = Interpolation.MixEven(patch[3], patch[4]);
                                }
                                dest[destIndex + 2] = patch[4];
                                if (Diff(patch[1], patch[5], trY, trU, trV, trA))
                                {
                                    dest[destIndex + 3] = patch[4];
                                }
                                else
                                {
                                    dest[destIndex + 3] = Interpolation.Mix2To1To1(patch[4], patch[1], patch[5]);
                                }
                                dest[destIndex + dpL + 1] = patch[4];
                                dest[destIndex + dpL + 2] = patch[4];
                                dest[destIndex + dpL + 3] = patch[4];
                                dest[destIndex + dpL + dpL] = Interpolation.Mix3To1(patch[4], patch[6]);
                                dest[destIndex + dpL + dpL + 1] = Interpolation.Mix7To1(patch[4], patch[6]);
                                dest[destIndex + dpL + dpL + 2] = Interpolation.Mix7To1(patch[4], patch[7]);
                                dest[destIndex + dpL + dpL + 3] = Interpolation.Mix7To1(patch[4], patch[7]);
                                dest[destIndex + dpL + dpL + dpL] = Interpolation.Mix5To3(patch[4], patch[6]);
                                dest[destIndex + dpL + dpL + dpL + 1] = Interpolation.Mix5To2To1(patch[4], patch[7], patch[6]);
                                dest[destIndex + dpL + dpL + dpL + 2] = Interpolation.Mix5To3(patch[4], patch[7]);
                                dest[destIndex + dpL + dpL + dpL + 3] = Interpolation.Mix5To3(patch[4], patch[7]);
                                break;
                            }
                        case 215:
                            {
                                dest[destIndex] = Interpolation.Mix5To3(patch[4], patch[3]);
                                dest[destIndex + 1] = Interpolation.Mix7To1(patch[4], patch[3]);
                                dest[destIndex + 2] = patch[4];
                                if (Diff(patch[1], patch[5], trY, trU, trV, trA))
                                {
                                    dest[destIndex + 3] = patch[4];
                                }
                                else
                                {
                                    dest[destIndex + 3] = Interpolation.Mix2To1To1(patch[4], patch[1], patch[5]);
                                }
                                dest[destIndex + dpL] = Interpolation.Mix5To3(patch[4], patch[3]);
                                dest[destIndex + dpL + 1] = Interpolation.Mix7To1(patch[4], patch[3]);
                                dest[destIndex + dpL + 2] = patch[4];
                                dest[destIndex + dpL + 3] = patch[4];
                                dest[destIndex + dpL + dpL] = Interpolation.Mix5To2To1(patch[4], patch[3], patch[6]);
                                dest[destIndex + dpL + dpL + 1] = Interpolation.Mix7To1(patch[4], patch[6]);
                                dest[destIndex + dpL + dpL + 2] = patch[4];
                                if (Diff(patch[5], patch[7], trY, trU, trV, trA))
                                {
                                    dest[destIndex + dpL + dpL + 3] = patch[4];
                                    dest[destIndex + dpL + dpL + dpL + 2] = patch[4];
                                    dest[destIndex + dpL + dpL + dpL + 3] = patch[4];
                                }
                                else
                                {
                                    dest[destIndex + dpL + dpL + 3] = Interpolation.MixEven(patch[5], patch[4]);
                                    dest[destIndex + dpL + dpL + dpL + 2] = Interpolation.MixEven(patch[7], patch[4]);
                                    dest[destIndex + dpL + dpL + dpL + 3] = Interpolation.MixEven(patch[7], patch[5]);
                                }
                                dest[destIndex + dpL + dpL + dpL] = Interpolation.Mix5To3(patch[4], patch[6]);
                                dest[destIndex + dpL + dpL + dpL + 1] = Interpolation.Mix3To1(patch[4], patch[6]);
                                break;
                            }
                        case 246:
                            {
                                dest[destIndex] = Interpolation.Mix5To3(patch[4], patch[0]);
                                dest[destIndex + 1] = Interpolation.Mix3To1(patch[4], patch[0]);
                                if (Diff(patch[1], patch[5], trY, trU, trV, trA))
                                {
                                    dest[destIndex + 2] = patch[4];
                                    dest[destIndex + 3] = patch[4];
                                    dest[destIndex + dpL + 3] = patch[4];
                                }
                                else
                                {
                                    dest[destIndex + 2] = Interpolation.MixEven(patch[1], patch[4]);
                                    dest[destIndex + 3] = Interpolation.MixEven(patch[1], patch[5]);
                                    dest[destIndex + dpL + 3] = Interpolation.MixEven(patch[5], patch[4]);
                                }
                                dest[destIndex + dpL] = Interpolation.Mix5To2To1(patch[4], patch[3], patch[0]);
                                dest[destIndex + dpL + 1] = Interpolation.Mix7To1(patch[4], patch[0]);
                                dest[destIndex + dpL + 2] = patch[4];
                                dest[destIndex + dpL + dpL] = Interpolation.Mix5To3(patch[4], patch[3]);
                                dest[destIndex + dpL + dpL + 1] = Interpolation.Mix7To1(patch[4], patch[3]);
                                dest[destIndex + dpL + dpL + 2] = patch[4];
                                dest[destIndex + dpL + dpL + 3] = patch[4];
                                dest[destIndex + dpL + dpL + dpL] = Interpolation.Mix5To3(patch[4], patch[3]);
                                dest[destIndex + dpL + dpL + dpL + 1] = Interpolation.Mix7To1(patch[4], patch[3]);
                                dest[destIndex + dpL + dpL + dpL + 2] = patch[4];
                                if (Diff(patch[5], patch[7], trY, trU, trV, trA))
                                {
                                    dest[destIndex + dpL + dpL + dpL + 3] = patch[4];
                                }
                                else
                                {
                                    dest[destIndex + dpL + dpL + dpL + 3] = Interpolation.Mix2To1To1(patch[4], patch[7], patch[5]);
                                }
                                break;
                            }
                        case 254:
                            {
                                dest[destIndex] = Interpolation.Mix5To3(patch[4], patch[0]);
                                dest[destIndex + 1] = Interpolation.Mix3To1(patch[4], patch[0]);
                                if (Diff(patch[1], patch[5], trY, trU, trV, trA))
                                {
                                    dest[destIndex + 2] = patch[4];
                                    dest[destIndex + 3] = patch[4];
                                    dest[destIndex + dpL + 3] = patch[4];
                                }
                                else
                                {
                                    dest[destIndex + 2] = Interpolation.MixEven(patch[1], patch[4]);
                                    dest[destIndex + 3] = Interpolation.MixEven(patch[1], patch[5]);
                                    dest[destIndex + dpL + 3] = Interpolation.MixEven(patch[5], patch[4]);
                                }
                                dest[destIndex + dpL] = Interpolation.Mix3To1(patch[4], patch[0]);
                                dest[destIndex + dpL + 1] = Interpolation.Mix7To1(patch[4], patch[0]);
                                dest[destIndex + dpL + 2] = patch[4];
                                if (Diff(patch[7], patch[3], trY, trU, trV, trA))
                                {
                                    dest[destIndex + dpL + dpL] = patch[4];
                                    dest[destIndex + dpL + dpL + dpL] = patch[4];
                                    dest[destIndex + dpL + dpL + dpL + 1] = patch[4];
                                }
                                else
                                {
                                    dest[destIndex + dpL + dpL] = Interpolation.MixEven(patch[3], patch[4]);
                                    dest[destIndex + dpL + dpL + dpL] = Interpolation.MixEven(patch[7], patch[3]);
                                    dest[destIndex + dpL + dpL + dpL + 1] = Interpolation.MixEven(patch[7], patch[4]);
                                }
                                dest[destIndex + dpL + dpL + 1] = patch[4];
                                dest[destIndex + dpL + dpL + 2] = patch[4];
                                dest[destIndex + dpL + dpL + 3] = patch[4];
                                dest[destIndex + dpL + dpL + dpL + 2] = patch[4];
                                if (Diff(patch[5], patch[7], trY, trU, trV, trA))
                                {
                                    dest[destIndex + dpL + dpL + dpL + 3] = patch[4];
                                }
                                else
                                {
                                    dest[destIndex + dpL + dpL + dpL + 3] = Interpolation.Mix2To1To1(patch[4], patch[7], patch[5]);
                                }
                                break;
                            }
                        case 253:
                            {
                                dest[destIndex] = Interpolation.Mix5To3(patch[4], patch[1]);
                                dest[destIndex + 1] = Interpolation.Mix5To3(patch[4], patch[1]);
                                dest[destIndex + 2] = Interpolation.Mix5To3(patch[4], patch[1]);
                                dest[destIndex + 3] = Interpolation.Mix5To3(patch[4], patch[1]);
                                dest[destIndex + dpL] = Interpolation.Mix7To1(patch[4], patch[1]);
                                dest[destIndex + dpL + 1] = Interpolation.Mix7To1(patch[4], patch[1]);
                                dest[destIndex + dpL + 2] = Interpolation.Mix7To1(patch[4], patch[1]);
                                dest[destIndex + dpL + 3] = Interpolation.Mix7To1(patch[4], patch[1]);
                                dest[destIndex + dpL + dpL] = patch[4];
                                dest[destIndex + dpL + dpL + 1] = patch[4];
                                dest[destIndex + dpL + dpL + 2] = patch[4];
                                dest[destIndex + dpL + dpL + 3] = patch[4];
                                if (Diff(patch[7], patch[3], trY, trU, trV, trA))
                                {
                                    dest[destIndex + dpL + dpL + dpL] = patch[4];
                                }
                                else
                                {
                                    dest[destIndex + dpL + dpL + dpL] = Interpolation.Mix2To1To1(patch[4], patch[7], patch[3]);
                                }
                                dest[destIndex + dpL + dpL + dpL + 1] = patch[4];
                                dest[destIndex + dpL + dpL + dpL + 2] = patch[4];
                                if (Diff(patch[5], patch[7], trY, trU, trV, trA))
                                {
                                    dest[destIndex + dpL + dpL + dpL + 3] = patch[4];
                                }
                                else
                                {
                                    dest[destIndex + dpL + dpL + dpL + 3] = Interpolation.Mix2To1To1(patch[4], patch[7], patch[5]);
                                }
                                break;
                            }
                        case 251:
                            {
                                if (Diff(patch[3], patch[1], trY, trU, trV, trA))
                                {
                                    dest[destIndex] = patch[4];
                                    dest[destIndex + 1] = patch[4];
                                    dest[destIndex + dpL] = patch[4];
                                }
                                else
                                {
                                    dest[destIndex] = Interpolation.MixEven(patch[1], patch[3]);
                                    dest[destIndex + 1] = Interpolation.MixEven(patch[1], patch[4]);
                                    dest[destIndex + dpL] = Interpolation.MixEven(patch[3], patch[4]);
                                }
                                dest[destIndex + 2] = Interpolation.Mix3To1(patch[4], patch[2]);
                                dest[destIndex + 3] = Interpolation.Mix5To3(patch[4], patch[2]);
                                dest[destIndex + dpL + 1] = patch[4];
                                dest[destIndex + dpL + 2] = Interpolation.Mix7To1(patch[4], patch[2]);
                                dest[destIndex + dpL + 3] = Interpolation.Mix3To1(patch[4], patch[2]);
                                dest[destIndex + dpL + dpL] = patch[4];
                                dest[destIndex + dpL + dpL + 1] = patch[4];
                                dest[destIndex + dpL + dpL + 2] = patch[4];
                                if (Diff(patch[5], patch[7], trY, trU, trV, trA))
                                {
                                    dest[destIndex + dpL + dpL + 3] = patch[4];
                                    dest[destIndex + dpL + dpL + dpL + 2] = patch[4];
                                    dest[destIndex + dpL + dpL + dpL + 3] = patch[4];
                                }
                                else
                                {
                                    dest[destIndex + dpL + dpL + 3] = Interpolation.MixEven(patch[5], patch[4]);
                                    dest[destIndex + dpL + dpL + dpL + 2] = Interpolation.MixEven(patch[7], patch[4]);
                                    dest[destIndex + dpL + dpL + dpL + 3] = Interpolation.MixEven(patch[7], patch[5]);
                                }
                                if (Diff(patch[7], patch[3], trY, trU, trV, trA))
                                {
                                    dest[destIndex + dpL + dpL + dpL] = patch[4];
                                }
                                else
                                {
                                    dest[destIndex + dpL + dpL + dpL] = Interpolation.Mix2To1To1(patch[4], patch[7], patch[3]);
                                }
                                dest[destIndex + dpL + dpL + dpL + 1] = patch[4];
                                break;
                            }
                        case 239:
                            {
                                if (Diff(patch[3], patch[1], trY, trU, trV, trA))
                                {
                                    dest[destIndex] = patch[4];
                                }
                                else
                                {
                                    dest[destIndex] = Interpolation.Mix2To1To1(patch[4], patch[1], patch[3]);
                                }
                                dest[destIndex + 1] = patch[4];
                                dest[destIndex + 2] = Interpolation.Mix7To1(patch[4], patch[5]);
                                dest[destIndex + 3] = Interpolation.Mix5To3(patch[4], patch[5]);
                                dest[destIndex + dpL] = patch[4];
                                dest[destIndex + dpL + 1] = patch[4];
                                dest[destIndex + dpL + 2] = Interpolation.Mix7To1(patch[4], patch[5]);
                                dest[destIndex + dpL + 3] = Interpolation.Mix5To3(patch[4], patch[5]);
                                dest[destIndex + dpL + dpL] = patch[4];
                                dest[destIndex + dpL + dpL + 1] = patch[4];
                                dest[destIndex + dpL + dpL + 2] = Interpolation.Mix7To1(patch[4], patch[5]);
                                dest[destIndex + dpL + dpL + 3] = Interpolation.Mix5To3(patch[4], patch[5]);
                                if (Diff(patch[7], patch[3], trY, trU, trV, trA))
                                {
                                    dest[destIndex + dpL + dpL + dpL] = patch[4];
                                }
                                else
                                {
                                    dest[destIndex + dpL + dpL + dpL] = Interpolation.Mix2To1To1(patch[4], patch[7], patch[3]);
                                }
                                dest[destIndex + dpL + dpL + dpL + 1] = patch[4];
                                dest[destIndex + dpL + dpL + dpL + 2] = Interpolation.Mix7To1(patch[4], patch[5]);
                                dest[destIndex + dpL + dpL + dpL + 3] = Interpolation.Mix5To3(patch[4], patch[5]);
                                break;
                            }
                        case 127:
                            {
                                if (Diff(patch[3], patch[1], trY, trU, trV, trA))
                                {
                                    dest[destIndex] = patch[4];
                                }
                                else
                                {
                                    dest[destIndex] = Interpolation.Mix2To1To1(patch[4], patch[1], patch[3]);
                                }
                                dest[destIndex + 1] = patch[4];
                                if (Diff(patch[1], patch[5], trY, trU, trV, trA))
                                {
                                    dest[destIndex + 2] = patch[4];
                                    dest[destIndex + 3] = patch[4];
                                    dest[destIndex + dpL + 3] = patch[4];
                                }
                                else
                                {
                                    dest[destIndex + 2] = Interpolation.MixEven(patch[1], patch[4]);
                                    dest[destIndex + 3] = Interpolation.MixEven(patch[1], patch[5]);
                                    dest[destIndex + dpL + 3] = Interpolation.MixEven(patch[5], patch[4]);
                                }
                                dest[destIndex + dpL] = patch[4];
                                dest[destIndex + dpL + 1] = patch[4];
                                dest[destIndex + dpL + 2] = patch[4];
                                if (Diff(patch[7], patch[3], trY, trU, trV, trA))
                                {
                                    dest[destIndex + dpL + dpL] = patch[4];
                                    dest[destIndex + dpL + dpL + dpL] = patch[4];
                                    dest[destIndex + dpL + dpL + dpL + 1] = patch[4];
                                }
                                else
                                {
                                    dest[destIndex + dpL + dpL] = Interpolation.MixEven(patch[3], patch[4]);
                                    dest[destIndex + dpL + dpL + dpL] = Interpolation.MixEven(patch[7], patch[3]);
                                    dest[destIndex + dpL + dpL + dpL + 1] = Interpolation.MixEven(patch[7], patch[4]);
                                }
                                dest[destIndex + dpL + dpL + 1] = patch[4];
                                dest[destIndex + dpL + dpL + 2] = Interpolation.Mix7To1(patch[4], patch[8]);
                                dest[destIndex + dpL + dpL + 3] = Interpolation.Mix3To1(patch[4], patch[8]);
                                dest[destIndex + dpL + dpL + dpL + 2] = Interpolation.Mix3To1(patch[4], patch[8]);
                                dest[destIndex + dpL + dpL + dpL + 3] = Interpolation.Mix5To3(patch[4], patch[8]);
                                break;
                            }
                        case 191:
                            {
                                if (Diff(patch[3], patch[1], trY, trU, trV, trA))
                                {
                                    dest[destIndex] = patch[4];
                                }
                                else
                                {
                                    dest[destIndex] = Interpolation.Mix2To1To1(patch[4], patch[1], patch[3]);
                                }
                                dest[destIndex + 1] = patch[4];
                                dest[destIndex + 2] = patch[4];
                                if (Diff(patch[1], patch[5], trY, trU, trV, trA))
                                {
                                    dest[destIndex + 3] = patch[4];
                                }
                                else
                                {
                                    dest[destIndex + 3] = Interpolation.Mix2To1To1(patch[4], patch[1], patch[5]);
                                }
                                dest[destIndex + dpL] = patch[4];
                                dest[destIndex + dpL + 1] = patch[4];
                                dest[destIndex + dpL + 2] = patch[4];
                                dest[destIndex + dpL + 3] = patch[4];
                                dest[destIndex + dpL + dpL] = Interpolation.Mix7To1(patch[4], patch[7]);
                                dest[destIndex + dpL + dpL + 1] = Interpolation.Mix7To1(patch[4], patch[7]);
                                dest[destIndex + dpL + dpL + 2] = Interpolation.Mix7To1(patch[4], patch[7]);
                                dest[destIndex + dpL + dpL + 3] = Interpolation.Mix7To1(patch[4], patch[7]);
                                dest[destIndex + dpL + dpL + dpL] = Interpolation.Mix5To3(patch[4], patch[7]);
                                dest[destIndex + dpL + dpL + dpL + 1] = Interpolation.Mix5To3(patch[4], patch[7]);
                                dest[destIndex + dpL + dpL + dpL + 2] = Interpolation.Mix5To3(patch[4], patch[7]);
                                dest[destIndex + dpL + dpL + dpL + 3] = Interpolation.Mix5To3(patch[4], patch[7]);
                                break;
                            }
                        case 223:
                            {
                                if (Diff(patch[3], patch[1], trY, trU, trV, trA))
                                {
                                    dest[destIndex] = patch[4];
                                    dest[destIndex + 1] = patch[4];
                                    dest[destIndex + dpL] = patch[4];
                                }
                                else
                                {
                                    dest[destIndex] = Interpolation.MixEven(patch[1], patch[3]);
                                    dest[destIndex + 1] = Interpolation.MixEven(patch[1], patch[4]);
                                    dest[destIndex + dpL] = Interpolation.MixEven(patch[3], patch[4]);
                                }
                                dest[destIndex + 2] = patch[4];
                                if (Diff(patch[1], patch[5], trY, trU, trV, trA))
                                {
                                    dest[destIndex + 3] = patch[4];
                                }
                                else
                                {
                                    dest[destIndex + 3] = Interpolation.Mix2To1To1(patch[4], patch[1], patch[5]);
                                }
                                dest[destIndex + dpL + 1] = patch[4];
                                dest[destIndex + dpL + 2] = patch[4];
                                dest[destIndex + dpL + 3] = patch[4];
                                dest[destIndex + dpL + dpL] = Interpolation.Mix3To1(patch[4], patch[6]);
                                dest[destIndex + dpL + dpL + 1] = Interpolation.Mix7To1(patch[4], patch[6]);
                                dest[destIndex + dpL + dpL + 2] = patch[4];
                                if (Diff(patch[5], patch[7], trY, trU, trV, trA))
                                {
                                    dest[destIndex + dpL + dpL + 3] = patch[4];
                                    dest[destIndex + dpL + dpL + dpL + 2] = patch[4];
                                    dest[destIndex + dpL + dpL + dpL + 3] = patch[4];
                                }
                                else
                                {
                                    dest[destIndex + dpL + dpL + 3] = Interpolation.MixEven(patch[5], patch[4]);
                                    dest[destIndex + dpL + dpL + dpL + 2] = Interpolation.MixEven(patch[7], patch[4]);
                                    dest[destIndex + dpL + dpL + dpL + 3] = Interpolation.MixEven(patch[7], patch[5]);
                                }
                                dest[destIndex + dpL + dpL + dpL] = Interpolation.Mix5To3(patch[4], patch[6]);
                                dest[destIndex + dpL + dpL + dpL + 1] = Interpolation.Mix3To1(patch[4], patch[6]);
                                break;
                            }
                        case 247:
                            {
                                dest[destIndex] = Interpolation.Mix5To3(patch[4], patch[3]);
                                dest[destIndex + 1] = Interpolation.Mix7To1(patch[4], patch[3]);
                                dest[destIndex + 2] = patch[4];
                                if (Diff(patch[1], patch[5], trY, trU, trV, trA))
                                {
                                    dest[destIndex + 3] = patch[4];
                                }
                                else
                                {
                                    dest[destIndex + 3] = Interpolation.Mix2To1To1(patch[4], patch[1], patch[5]);
                                }
                                dest[destIndex + dpL] = Interpolation.Mix5To3(patch[4], patch[3]);
                                dest[destIndex + dpL + 1] = Interpolation.Mix7To1(patch[4], patch[3]);
                                dest[destIndex + dpL + 2] = patch[4];
                                dest[destIndex + dpL + 3] = patch[4];
                                dest[destIndex + dpL + dpL] = Interpolation.Mix5To3(patch[4], patch[3]);
                                dest[destIndex + dpL + dpL + 1] = Interpolation.Mix7To1(patch[4], patch[3]);
                                dest[destIndex + dpL + dpL + 2] = patch[4];
                                dest[destIndex + dpL + dpL + 3] = patch[4];
                                dest[destIndex + dpL + dpL + dpL] = Interpolation.Mix5To3(patch[4], patch[3]);
                                dest[destIndex + dpL + dpL + dpL + 1] = Interpolation.Mix7To1(patch[4], patch[3]);
                                dest[destIndex + dpL + dpL + dpL + 2] = patch[4];
                                if (Diff(patch[5], patch[7], trY, trU, trV, trA))
                                {
                                    dest[destIndex + dpL + dpL + dpL + 3] = patch[4];
                                }
                                else
                                {
                                    dest[destIndex + dpL + dpL + dpL + 3] = Interpolation.Mix2To1To1(patch[4], patch[7], patch[5]);
                                }
                                break;
                            }
                        case 255:
                            {
                                if (Diff(patch[3], patch[1], trY, trU, trV, trA))
                                {
                                    dest[destIndex] = patch[4];
                                }
                                else
                                {
                                    dest[destIndex] = Interpolation.Mix2To1To1(patch[4], patch[1], patch[3]);
                                }
                                dest[destIndex + 1] = patch[4];
                                dest[destIndex + 2] = patch[4];
                                if (Diff(patch[1], patch[5], trY, trU, trV, trA))
                                {
                                    dest[destIndex + 3] = patch[4];
                                }
                                else
                                {
                                    dest[destIndex + 3] = Interpolation.Mix2To1To1(patch[4], patch[1], patch[5]);
                                }
                                dest[destIndex + dpL] = patch[4];
                                dest[destIndex + dpL + 1] = patch[4];
                                dest[destIndex + dpL + 2] = patch[4];
                                dest[destIndex + dpL + 3] = patch[4];
                                dest[destIndex + dpL + dpL] = patch[4];
                                dest[destIndex + dpL + dpL + 1] = patch[4];
                                dest[destIndex + dpL + dpL + 2] = patch[4];
                                dest[destIndex + dpL + dpL + 3] = patch[4];
                                if (Diff(patch[7], patch[3], trY, trU, trV, trA))
                                {
                                    dest[destIndex + dpL + dpL + dpL] = patch[4];
                                }
                                else
                                {
                                    dest[destIndex + dpL + dpL + dpL] = Interpolation.Mix2To1To1(patch[4], patch[7], patch[3]);
                                }
                                dest[destIndex + dpL + dpL + dpL + 1] = patch[4];
                                dest[destIndex + dpL + dpL + dpL + 2] = patch[4];
                                if (Diff(patch[5], patch[7], trY, trU, trV, trA))
                                {
                                    dest[destIndex + dpL + dpL + dpL + 3] = patch[4];
                                }
                                else
                                {
                                    dest[destIndex + dpL + dpL + dpL + 3] = Interpolation.Mix2To1To1(patch[4], patch[7], patch[5]);
                                }
                                break;
                            }
                    }
                    sourceIndex++;
                    destIndex += 4;
                }
                destIndex += (dpL * 3);
            }
        }

    }
}
