using System;
using System.IO;
using System.Xml.Linq;
using UnityEngine;

public class SpecialMapContent : IContent
{
    TextureStorage store;
    int storageIndex;

    public Texture2D Texture { get; private set; }

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

    public object ExternalStorage
    {
        set
        {
            store = value as TextureStorage;
            if (store == null)
                Debug.LogError(value + " is null, for some reason");
        }
    }

    public bool AddTypeElement(XElement elemtype)
    {
        XAttribute metalAtt = elemtype.Attribute("metallic");
        if(metalAtt == null)
        {
            Debug.LogError("No metallic map in " + elemtype);
            return false;
        }
        string metalPath = Path.Combine(Path.GetDirectoryName(new Uri(elemtype.BaseUri).LocalPath), metalAtt.Value);
        metalPath = Path.GetFullPath(metalPath);

        if(!File.Exists(metalPath))
        {
            Debug.LogError("File not found: " + metalPath);
            return false;
        }

        byte[] metalData = File.ReadAllBytes(metalPath);
        Texture2D metalMap = new Texture2D(2, 2, TextureFormat.ARGB32, false, true);
        metalMap.LoadImage(metalData);

        metalMap.name = metalPath;

        if (metalMap.width > GameSettings.Instance.rendering.maxTextureSize || metalMap.height > GameSettings.Instance.rendering.maxTextureSize)
        {
            if (metalMap.width > metalMap.height)
            {
                TextureScale.Bilinear(
                    metalMap,
                    GameSettings.Instance.rendering.maxTextureSize,
                    GameSettings.Instance.rendering.maxTextureSize * metalMap.height / metalMap.width);
            }
            else
            {
                TextureScale.Bilinear(
                    metalMap,
                    GameSettings.Instance.rendering.maxTextureSize * metalMap.width / metalMap.height,
                    GameSettings.Instance.rendering.maxTextureSize);
            }
        }

        XAttribute illuminationAtt = elemtype.Attribute("illumination");
        if (illuminationAtt == null)
        {
            Debug.LogError("No illumination map in " + elemtype);
            //Add error message here
            return false;
        }
        string illuminationPath = Path.Combine(Path.GetDirectoryName(new Uri(elemtype.BaseUri).LocalPath), illuminationAtt.Value);
        illuminationPath = Path.GetFullPath(illuminationPath);

        if (!File.Exists(illuminationPath))
        {
            Debug.LogError("File not found: " + illuminationPath);
            return false;
        }

        byte[] illuminationData = File.ReadAllBytes(illuminationPath);

        Texture2D illuminationMap = new Texture2D(2, 2, TextureFormat.ARGB32, false, true);
        illuminationMap.LoadImage(illuminationData);

        if (illuminationMap.width != metalMap.width || illuminationMap.height != metalMap.height)
        {
            TextureScale.Bilinear(illuminationMap, metalMap.width, metalMap.height);
        }


        Texture2D combinedMap = new Texture2D(metalMap.width, metalMap.height, TextureFormat.ARGB32, false, true);

        combinedMap.name = metalPath + illuminationAtt.Value;

        Color[] metalColors = metalMap.GetPixels();
        Color[] illuminationColors = illuminationMap.GetPixels();

        if (metalColors.Length != illuminationColors.Length)
            Debug.LogError("Maps aren't same size!");

        for (int i = 0; i < metalColors.Length; i++)
        {
            metalColors[i] = new Color(metalColors[i].r, illuminationColors[i].g, 1, 1);
        }

        combinedMap.SetPixels(metalColors);
        combinedMap.Apply();

        if (store != null)
        {
            storageIndex = store.AddTexture(combinedMap);
            Texture = null;
        }
        else
        {
            storageIndex = -1;
            Texture = combinedMap;
        }

        return true;
    }
}
