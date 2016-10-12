using System;
using System.IO;
using System.Xml.Linq;
using UnityEngine;

public class NormalContent : IContent
{

    TextureStorage store;
    int storageIndex;

    public Matrix4x4 UVTransform
    {
        get
        {
            return store.getUVTransform(storageIndex);
        }
    }

    public float ArrayIndex
    {
        get
        {
            return (float)storageIndex / store.Count;
        }
    }


    public bool AddTypeElement(System.Xml.Linq.XElement elemtype)
    {
        if (store == null) //nowhere to put the image.
        {
            Debug.LogError("Texture Storage is Null: " + elemtype);
            return false;
        } 

        XAttribute normalAtt = elemtype.Attribute("normal");
        if (normalAtt == null)
        {
            Debug.LogError("No normal map in " + elemtype);
            return false;
        }
        string normalPath = Path.Combine(Path.GetDirectoryName(new Uri(elemtype.BaseUri).LocalPath), normalAtt.Value);
        normalPath = Path.GetFullPath(normalPath);

        if (!File.Exists(normalPath))
        {
            Debug.LogError("File not found: " + normalPath);
            return false;
        }

        byte[] normalData = File.ReadAllBytes(normalPath);
        Texture2D normalMap = new Texture2D(2, 2, TextureFormat.ARGB32, false, true);
        normalMap.LoadImage(normalData);

        if (normalMap.width > GameSettings.Instance.rendering.maxTextureSize || normalMap.height > GameSettings.Instance.rendering.maxTextureSize)
        {
            if (normalMap.width > normalMap.height)
            {
                TextureScale.Bilinear(
                    normalMap,
                    GameSettings.Instance.rendering.maxTextureSize,
                    GameSettings.Instance.rendering.maxTextureSize * normalMap.height / normalMap.width);
            }
            else
            {
                TextureScale.Bilinear(
                    normalMap,
                    GameSettings.Instance.rendering.maxTextureSize * normalMap.width / normalMap.height,
                    GameSettings.Instance.rendering.maxTextureSize);
            }
        }


        XAttribute alphaAtt = elemtype.Attribute("alpha");
        if (alphaAtt == null)
        {
            Debug.LogError("No alpha map in " + elemtype);
            //Add error message here
            return false;
        }
        string alphaPath = Path.Combine(Path.GetDirectoryName(new Uri(elemtype.BaseUri).LocalPath), alphaAtt.Value);
        alphaPath = Path.GetFullPath(alphaPath);

        if (!File.Exists(alphaPath))
        {
            Debug.LogError("File not found: " + alphaPath);
            return false;
        }

        byte[] alphaData = File.ReadAllBytes(alphaPath);
        Texture2D alphaMap = new Texture2D(2, 2, TextureFormat.ARGB32, false, false);
        alphaMap.LoadImage(alphaData);

        if ((alphaMap.width != normalMap.width) || (alphaMap.height != normalMap.height))
        {
            TextureScale.Bilinear(alphaMap, normalMap.width, normalMap.height);
        }

        XAttribute occlusionAtt = elemtype.Attribute("occlusion");
        if (occlusionAtt == null)
        {
            Debug.LogError("No occlusion map in " + elemtype);
            //Add error message here
            return false;
        }
        string occlusionPath = Path.Combine(Path.GetDirectoryName(new Uri(elemtype.BaseUri).LocalPath), occlusionAtt.Value);
        occlusionPath = Path.GetFullPath(occlusionPath);

        if (!File.Exists(occlusionPath))
        {
            Debug.LogError("File not found: " + occlusionPath);
            return false;
        }

        byte[] occlusionData = File.ReadAllBytes(occlusionPath);

        Texture2D occlusionMap = new Texture2D(2, 2, TextureFormat.ARGB32, false, true);
        occlusionMap.LoadImage(occlusionData);

        if (occlusionMap.width != normalMap.width || occlusionMap.height != normalMap.height)
        {
            TextureScale.Bilinear(occlusionMap, normalMap.width, normalMap.height);
        }

        Texture2D combinedMap = new Texture2D(normalMap.width, normalMap.height, TextureFormat.ARGB32, false, true);

        combinedMap.name = normalPath + occlusionAtt.Value + alphaAtt.Value;

        Color[] normalColors = normalMap.GetPixels();
        Color[] occlusionColors = occlusionMap.GetPixels();
        Color[] alphaColors = alphaMap.GetPixels();

        if (normalColors.Length != alphaColors.Length)
            Debug.LogError("Maps aren't same size!");

        for (int i = 0; i < normalColors.Length; i++)
        {
            normalColors[i] = new Color(occlusionColors[i].r, normalColors[i].g, alphaColors[i].r, normalColors[i].r);
        }

        combinedMap.SetPixels(normalColors);
        combinedMap.Apply();

        storageIndex = store.AddTexture(combinedMap);
        return true;
    }

    public object ExternalStorage
    {
        set
        {
            store = value as TextureStorage;
            if (store == null)
                Debug.LogError(value + " is null, for some reason");
        }
    }
}
