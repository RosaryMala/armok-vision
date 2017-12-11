using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ColorQuantitizer
{
    public static List<List<Color32>> Split(List<Color32> colors)
    {
        byte minR = 255;
        byte minG = 255;
        byte minB = 255;
        byte minA = 255;
        byte maxR = 0;
        byte maxG = 0;
        byte maxB = 0;
        byte maxA = 0;


        //find the min and max of each channel.
        foreach (var color in colors)
        {
            minR = (byte)Mathf.Min(color.r, minR);
            minG = (byte)Mathf.Min(color.r, minG);
            minB = (byte)Mathf.Min(color.r, minB);
            minA = (byte)Mathf.Min(color.r, minA);
            maxR = (byte)Mathf.Max(color.r, maxR);
            maxG = (byte)Mathf.Max(color.r, maxG);
            maxB = (byte)Mathf.Max(color.r, maxB);
            maxA = (byte)Mathf.Max(color.r, maxA);
        }

        //find the variation for each.
        int red = maxR - minR;
        int green = maxG - minG;
        int blue = maxB - minB;
        int alpha = maxA - minA;

        //find the channel with max variation
        int max = Mathf.Max(Mathf.Max(Mathf.Max(red, green), blue), alpha);

        if (max == red)
            colors.Sort((x, y) => x.r.CompareTo(y.r));
        else if (max == green)
            colors.Sort((x, y) => x.g.CompareTo(y.g));
        else if (max == blue)
            colors.Sort((x, y) => x.b.CompareTo(y.b));
        else if (max == alpha)
            colors.Sort((x, y) => x.a.CompareTo(y.a));

        List<List<Color32>> output = new List<List<Color32>>();
        output.Add(new List<Color32>());
        output.Add(new List<Color32>());
        int midPoint = colors.Count / 2;
        for (int i = 0; i < colors.Count; i++)
        {
            if (i < midPoint)
                output[0].Add(colors[i]);
            else
                output[1].Add(colors[i]);
        }
        return output;
    }

    public static Color32[] Quantize (List<Color32> colors, int power)
    {
        Color32[] output = new Color32[1 << power];

        if(colors.Count <= output.Length)
        {
            for (int i = 0; i < colors.Count; i++)
            {
                output[i] = colors[i];
            }
            return output;
        }

        List<List<Color32>> finalList = new List<List<Color32>>();
        finalList.Add(colors);
        for (int i = 0; i < power; i++)
        {
            List<List<Color32>> tempLists = new List<List<Color32>>();
            foreach (var item in finalList)
            {
                tempLists.AddRange(Split(item));
            }
            finalList = tempLists;
        }



        for (int i = 0; i < finalList.Count; i++)
        {
            if (finalList[i].Count == 0)
            {
                output[i] = new Color32(255, 0, 255, 255);
                continue;
            }
            int red = 0;
            int green = 0;
            int blue = 0;
            int alpha = 0;
            foreach (var item in finalList[i])
            {
                red += item.r;
                green += item.g;
                blue += item.b;
                alpha += item.a;
            }
            red /= finalList[i].Count;
            green /= finalList[i].Count;
            blue /= finalList[i].Count;
            alpha /= finalList[i].Count;
            output[i] = new Color32((byte)red, (byte)green, (byte)blue, (byte)alpha);
        }
        return output;
    }
}
