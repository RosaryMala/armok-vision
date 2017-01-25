using System;
using System.Text;
using UnityEngine;

public class RawToken
{
    public string Token { get; set; }
    public string[] Parameters { get; set; }

    public RawToken(string token)
    {
        var results = token.Split(':');
        if (results.Length < 1)
            throw new InvalidOperationException();
        Token = results[0];
        Parameters = new string[results.Length - 1];
        for (int i = 0; i < Parameters.Length; i++)
        {
            Parameters[i] = results[i + 1];
        }
    }

    public override string ToString()
    {
        StringBuilder sb = new StringBuilder("[");
        sb.Append(Token);
        foreach (var item in Parameters)
        {
            sb.Append(":").Append(item);
        }
        sb.Append("]");
        return sb.ToString();
    }
}
