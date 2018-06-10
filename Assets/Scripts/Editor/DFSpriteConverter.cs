using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions;

public class DFSpriteConverter
{
    [MenuItem("Mytools/Split DF Creature Tiles")]
    public static void SplitDFTiles()
    {
        var path = EditorUtility.OpenFolderPanel("Pick Sprite Folder", "", "SpriteFolder");
        var files = Directory.GetFiles(path, "*.txt");
        foreach (var file in files)
        {
            Debug.Log("Loading Raw File: " + file);
            var tokenList = RawLoader.SplitRawFileText(File.ReadAllText(file));
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
                                var parser = ParseGraphics(tokenEnumerator, file);
                                while (parser.MoveNext()) ;
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
    }

    static IEnumerator ParseGraphics(List<RawToken>.Enumerator tokenEnumerator, string path)
    {
        Assert.AreEqual("GRAPHICS", tokenEnumerator.Current.Parameters[0]);
        var stopWatch = System.Diagnostics.Stopwatch.StartNew();
        bool rawLeft = true;
        while (rawLeft)
        {
            GameMap.BeginSample(tokenEnumerator.Current.Token);
            switch (tokenEnumerator.Current.Token)
            {
                case "TILE_PAGE":
                    rawLeft = ParseTilePage(ref tokenEnumerator, path);
                    break;
                case "CREATURE_GRAPHICS":
                    rawLeft = ParseGreatureGraphics(ref tokenEnumerator, path);
                    break;
                default:
                    rawLeft = tokenEnumerator.MoveNext();
                    break;
            }
            GameMap.EndSample();
            if (stopWatch.ElapsedMilliseconds > ContentLoader.LoadFrameTimeout)
            {
                yield return null;
                stopWatch.Reset();
                stopWatch.Start();
            }
        }
    }

    private static bool ParseGreatureGraphics(ref List<RawToken>.Enumerator tokenEnumerator, string path)
    {
        Assert.AreEqual("CREATURE_GRAPHICS", tokenEnumerator.Current.Token);
        string race = tokenEnumerator.Current.Parameters[0];

        bool rawLeft = true;
        while (rawLeft = tokenEnumerator.MoveNext())
        {
            var token = tokenEnumerator.Current;
            switch (token.Token)
            {
                case "CREATURE_GRAPHICS":
                case "TILE_PAGE":
                    goto loopExit;
                default:
                    if(token.Parameters[4] == "DEFAULT")
                        pages[token.Parameters[0]].SaveSprite(race, token.Token, int.Parse(token.Parameters[1]), int.Parse(token.Parameters[2]), path);
                    break;
            }
        }
        loopExit:
        return rawLeft;
    }

    static Dictionary<string, TilePage> pages = new Dictionary<string, TilePage>();

    private static bool ParseTilePage(ref List<RawToken>.Enumerator tokenEnumerator, string path)
    {
        Assert.AreEqual("TILE_PAGE", tokenEnumerator.Current.Token);

        string pageName = tokenEnumerator.Current.Parameters[0];

        path = Path.GetDirectoryName(path);
        int tileWidth = 0;
        int tileHeight = 0;
        int pageWidth = 0;
        int pageHeight = 0;

        bool rawLeft = true;
        while (rawLeft = tokenEnumerator.MoveNext())
        {
            var token = tokenEnumerator.Current;
            switch (tokenEnumerator.Current.Token)
            {
                case "CREATURE_GRAPHICS":
                case "TILE_PAGE":
                    goto loopExit;
                case "FILE":
                    path = Path.Combine(path, token.Parameters[0]);
                    break;
                case "TILE_DIM":
                    tileWidth = int.Parse(token.Parameters[0]);
                    tileHeight = int.Parse(token.Parameters[1]);
                    break;
                case "PAGE_DIM":
                    pageWidth = int.Parse(token.Parameters[0]);
                    pageHeight = int.Parse(token.Parameters[1]);
                    break;
                default:
                    break;
            }
        }
        loopExit:
        TilePage page = new TilePage(path, tileWidth, tileHeight, pageWidth, pageHeight);
        pages[pageName] = page;
        return rawLeft;
    }

    class TilePage
    {
        private Texture2D texture;
        private int tileWidth;
        private int tileHeight;
        private int pageWidth;
        private int pageHeight;

        public TilePage(string path, int tileWidth, int tileHeight, int pageWidth, int pageHeight)
        {
            texture = new Texture2D(4, 4);
            texture.LoadImage(File.ReadAllBytes(path));
            this.tileWidth = tileWidth;
            this.tileHeight = tileHeight;
            this.pageWidth = pageWidth;
            this.pageHeight = pageHeight;
        }

        internal void SaveSprite(string race, string job, int x, int y, string path)
        {
            if (job != "DEFAULT")
                return;
            path = Path.GetDirectoryName(path);
            path = Path.Combine(path, "EXPORT");
            Directory.CreateDirectory(path);
            path = Path.Combine(path, race + ".png");
            if (y >= pageHeight)
                return;
            if (x >= pageWidth)
                return;
            var pix = texture.GetPixels(x * tileWidth, (pageHeight - y - 1) * tileHeight, tileWidth, tileHeight);
            float alpha = 0;
            foreach (var pixel in pix)
            {
                alpha += pixel.a;
            }
            if (alpha < 1)
                return;
            Texture2D sprite = new Texture2D(tileWidth, tileHeight, TextureFormat.ARGB32, false);
            sprite.SetPixels(pix);
            sprite.Apply();
            File.WriteAllBytes(path, sprite.EncodeToPNG());
        }
    }
}
