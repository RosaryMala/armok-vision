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
}
