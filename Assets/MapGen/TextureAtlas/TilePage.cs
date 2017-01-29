using DFHack;
using hqx;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System;

public class TilePage : ICollection
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

    public int Count
    {
        get
        {
            return ((ICollection)coordList).Count;
        }
    }

    public bool IsSynchronized
    {
        get
        {
            return ((ICollection)coordList).IsSynchronized;
        }
    }

    public object SyncRoot
    {
        get
        {
            return ((ICollection)coordList).SyncRoot;
        }
    }

    public TilePage(string path, string pageName, int tileWidth, int tileHeight, int pageWidth, int pageHeight)
    {


        var pageData = File.ReadAllBytes(path);
        originalPage = new Texture2D(2, 2, TextureFormat.ARGB32, false);
        originalPage.LoadImage(pageData);
        originalPage.name = pageName;

        this.tileWidth = originalPage.width / pageWidth;
        this.tileHeight = originalPage.height / pageHeight;
        this.pageWidth = pageWidth;
        this.pageHeight = pageHeight;
        this.pageName = pageName;
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



        tileArray = new Texture2DArray(Mathf.ClosestPowerOfTwo(tileWidth * scaleFactor), Mathf.ClosestPowerOfTwo(tileHeight * scaleFactor), coordList.Count, TextureFormat.ARGB32, true);

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
            Texture2D texture = new Texture2D(tileWidth * scaleFactor, tileHeight * scaleFactor, TextureFormat.ARGB32, false);
            texture.SetPixels32(tileDest32);
            TextureScale.Bilinear(texture, Mathf.ClosestPowerOfTwo(tileWidth * scaleFactor), Mathf.ClosestPowerOfTwo(tileHeight * scaleFactor));
            tileArray.SetPixels(texture.GetPixels(), i);
        }
        tileArray.Apply();
    }

    public void CopyTo(Array array, int index)
    {
        ((ICollection)coordList).CopyTo(array, index);
    }

    public IEnumerator GetEnumerator()
    {
        return ((ICollection)coordList).GetEnumerator();
    }
}
