using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TextureTools
{
    public static Color[] Bevel(Color[] input, int width, int height)
    {
        Color[] outputArray = new Color[input.Length];
        for (int i = 0; i < outputArray.Length; i++)
        {
            outputArray[i] = new Color(0.5f, 0.5f, 0.5f, 0.5f);
        }
        int startRegion = -1;
        int endRegion = -1;
        for (int y = 0; y < height; y++)
        {
            startRegion = -1;
            endRegion = -1;
            for (int x = 0; x < width; x++)
            {
                if (input[x + y * width].a < 0.5f)
                {
                    startRegion = -1;
                    endRegion = -1;
                }
                else
                {
                    if (startRegion < 0)
                    {
                        startRegion = x;
                        for (endRegion = startRegion; endRegion < width; endRegion++)
                        {
                            if (input[endRegion + y * width].a < 0.5f)
                                break;
                        }
                    }
                    outputArray[x + y * width].a = Mathf.InverseLerp(startRegion, endRegion, x);
                }
            }
        }
        for (int x = 0; x < width; x++)
        {
            startRegion = -1;
            endRegion = -1;
            for (int y = 0; y < height; y++)
            {
                if (input[x + y * width].a < 0.5f)
                {
                    startRegion = -1;
                    endRegion = -1;
                }
                else
                {
                    if (startRegion < 0)
                    {
                        startRegion = y;
                        for (endRegion = startRegion; endRegion < height; endRegion++)
                        {
                            if (input[x + endRegion * width].a < 0.5f)
                                break;
                        }
                    }
                    outputArray[x + y * width].g = Mathf.InverseLerp(startRegion, endRegion, y);
                }
            }
        }
        //remove physical impossibilities
        for (int i = 0; i < outputArray.Length; i++)
        {
            Vector2 dir = new Vector2(outputArray[i].a * 2 - 1, outputArray[i].g * 2 - 1);
            if(dir.sqrMagnitude > 1)
            {
                dir.Normalize();
                outputArray[i].a = (dir.x + 1) / 2;
                outputArray[i].g = (dir.y + 1) / 2;
            }
        }
        return outputArray;
    }

    public static void PackNormals(Color[] colors)
    {
        for(int i = 0; i < colors.Length; i++)
        {
            var color = colors[i];
            colors[i] = new Color(color.g, color.g, color.g, color.r);
        }
    }
}
