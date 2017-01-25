using DFHack;
using hqx;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class TilePage
{
    Texture2D originalPage;
    readonly int tileWidth;
    readonly int tileHeight;
    readonly int pageWidth;
    readonly int pageHeight;
    readonly string pageName;
    Dictionary<DFCoord2d, int> spriteIndices = new Dictionary<DFCoord2d, int>();
    List<DFCoord2d> coordList = new List<DFCoord2d>();
    Texture2DArray tileArray;


    public TilePage(string path, string pageName, int tileWidth, int tileHeight, int pageWidth, int pageHeight)
    {
        this.tileWidth = tileWidth;
        this.tileHeight = tileHeight;
        this.pageWidth = pageWidth;
        this.pageHeight = pageHeight;
        this.pageName = pageName;


        var pageData = File.ReadAllBytes(path);
        originalPage = new Texture2D(2, 2, TextureFormat.ARGB32, false);
        originalPage.LoadImage(pageData);
        originalPage.name = pageName;
    }

    public int AddTilePage(DFCoord2d coord)
    {
        if (!spriteIndices.ContainsKey(coord))
        {
            spriteIndices[coord] = coordList.Count;
            coordList.Add(coord);
        }
        return spriteIndices[coord];
    }

    public void FinalizeTextures()
    {
        int scaleFactor = 1;
        if (tileWidth * 4 <= GameSettings.Instance.rendering.maxTextureSize && tileHeight * 4 <= GameSettings.Instance.rendering.maxTextureSize)
        {
            scaleFactor = 4;
        }
        else if (tileWidth * 3 <= GameSettings.Instance.rendering.maxTextureSize && tileHeight * 3 <= GameSettings.Instance.rendering.maxTextureSize)
        {
            scaleFactor = 3;
        }
        else if (tileWidth * 2 <= GameSettings.Instance.rendering.maxTextureSize && tileHeight * 2 <= GameSettings.Instance.rendering.maxTextureSize)
        {
            scaleFactor = 2;
        }

        tileArray = new Texture2DArray(tileWidth * scaleFactor, tileHeight * scaleFactor, coordList.Count, TextureFormat.ARGB32, true);

        for (int i = 0; i < coordList.Count; i++)
        {
            var coord = coordList[i];
            var tileSource = originalPage.GetPixels(coord.x * tileWidth, coord.y * tileHeight, tileWidth, tileHeight);
            var tileSource32 = new Color32[tileSource.Length];
            for(int j = 0; j < tileSource.Length; j++)
            {
                tileSource32[j] = tileSource[j];
            }
            var tileDest32 = new Color32[tileWidth * scaleFactor * tileHeight * scaleFactor];
            switch (scaleFactor)
            {
                case 4:
                    HqxSharp.Scale4(tileSource32, tileDest32, tileWidth, tileHeight);
                    break;
                case 3:
                    HqxSharp.Scale3(tileSource32, tileDest32, tileWidth, tileHeight);
                    break;
                case 2:
                    HqxSharp.Scale2(tileSource32, tileDest32, tileWidth, tileHeight);
                    break;
                default:
                    tileDest32 = tileSource32;
                    break;
            }
            tileArray.SetPixels32(tileDest32, i);
        }
        tileArray.Apply();
    }
}
