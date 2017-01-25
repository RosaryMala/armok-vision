using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Assertions;

public class RawLoader
{
    public static List<RawToken> SplitRawFileText(string rawFile)
    {
        var matches = Regex.Matches(rawFile, @"\[([^]]*)\]");
        Debug.Log("Found " + matches.Count + " matches");
        List<RawToken> output = new List<RawToken>(matches.Count);

        foreach (Match item in matches)
        {
            output.Add(new RawToken(item.Groups[1].Value));
        }

        return output;
    }

    public static void ParseRaw(string rawFile)
    {
        var tokenList = SplitRawFileText(rawFile);
        var tokenEnumerator = tokenList.GetEnumerator();
        try
        {
            if (tokenEnumerator.MoveNext()) // Because they always start at -1.
            {
                if (tokenEnumerator.Current.Token == "OBJECT")
                {
                    switch (tokenEnumerator.Current.Parameters[0])
                    {
                        case "GRAPHICS":
                            Debug.Log("Found graphics raws");
                            ParseGraphics(ref tokenEnumerator);
                            break;
                        default:
                            Debug.Log("Unhandled Token: " + tokenEnumerator.Current.Parameters[0]);
                            break;
                    }
                }
                else
                    Debug.Log("Unexpected Token: " + tokenEnumerator.Current.Token);
            }
        }
        finally
        {
            tokenEnumerator.Dispose();
        }
    }

    private static void ParseGraphics(ref List<RawToken>.Enumerator tokenEnumerator)
    {
        Assert.AreEqual("GRAPHICS", tokenEnumerator.Current.Parameters[0]);
        bool rawLeft = true;
        int count = 0;

        while (rawLeft)
        {
            if(count > 1000)
            {
                Debug.LogError("Infinite loop!");
                break;
            }
            count++;
            switch (tokenEnumerator.Current.Token)
            {
                case "TILE_PAGE":
                    rawLeft = ParseTilePage(ref tokenEnumerator);
                    break;
                case "CREATURE_GRAPHICS":
                    rawLeft = ParseGreatureGraphics(ref tokenEnumerator);
                    break;
                default:
                    rawLeft = tokenEnumerator.MoveNext();
                    break;
            }
        }
    }

    private static bool ParseGreatureGraphics(ref List<RawToken>.Enumerator tokenEnumerator)
    {
        Assert.AreEqual("CREATURE_GRAPHICS", tokenEnumerator.Current.Token);
        string creatureType = tokenEnumerator.Current.Parameters[0];
        int graphicsCount = 0;

        bool rawLeft = true;
        while(rawLeft = tokenEnumerator.MoveNext())
        {
            switch (tokenEnumerator.Current.Token)
            {
                case "CREATURE_GRAPHICS":
                case "TILE_PAGE":
                    goto loopExit;
                default:
                    //do actual parsing
                    graphicsCount++;
                    break;
            }
        }
        loopExit:
        Debug.Log("Found " + graphicsCount + " " + creatureType + " graphics");
        return rawLeft;
    }

    private static bool ParseTilePage(ref List<RawToken>.Enumerator tokenEnumerator)
    {
        Assert.AreEqual("TILE_PAGE", tokenEnumerator.Current.Token);

        string tilesetFile = "";

        bool rawLeft = true;
        while (rawLeft = tokenEnumerator.MoveNext())
        {
            switch (tokenEnumerator.Current.Token)
            {
                case "CREATURE_GRAPHICS":
                case "TILE_PAGE":
                    goto loopExit;
                case "FILE":
                    tilesetFile = tokenEnumerator.Current.Parameters[0];
                    break;
                default:
                    //do actual parsing
                    break;
            }
        }
        loopExit:
        Debug.Log("Found reference to tileset at " + tilesetFile);
        return rawLeft;
    }
}
