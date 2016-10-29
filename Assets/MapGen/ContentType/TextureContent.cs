using System.Xml.Linq;
using UnityEngine;
using System.IO;
using System;
using UnityExtension;

public class TextureContent : IContent
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

    public bool AddTypeElement(XElement elemtype)
    {
        XAttribute patternAtt = elemtype.Attribute("pattern");
        if (patternAtt == null)
        {
            Debug.LogError("No pattern attribute in " + elemtype);
            //Add error message here
            return false;
        }
        string patternPath = Path.Combine(Path.GetDirectoryName(new Uri(elemtype.BaseUri).LocalPath), patternAtt.Value);
        patternPath = Path.GetFullPath(patternPath);

        if (!File.Exists(patternPath))
        {
            Debug.LogError("File not found: " + patternPath);
            return false;
        }

        byte[] patternData = File.ReadAllBytes(patternPath);

        Texture2D patternTex = new Texture2D(2, 2);
        patternTex.LoadImage(patternData);

        if(patternTex.width > GameSettings.Instance.rendering.maxTextureSize || patternTex.height > GameSettings.Instance.rendering.maxTextureSize)
        {
            if(patternTex.width > patternTex.height)
            {
                TextureScale.Bilinear(
                    patternTex,
                    GameSettings.Instance.rendering.maxTextureSize,
                    GameSettings.Instance.rendering.maxTextureSize * patternTex.height / patternTex.width);
            }
            else
            {
                TextureScale.Bilinear(
                    patternTex,
                    GameSettings.Instance.rendering.maxTextureSize * patternTex.width / patternTex.height,
                    GameSettings.Instance.rendering.maxTextureSize);
            }
        }

        XAttribute specularAtt = elemtype.Attribute("specular");
        if (specularAtt == null)
        {
            Debug.LogError("No specular attribute in " + elemtype);
            //Add error message here
            return false;
        }
        string specularPath = Path.Combine(Path.GetDirectoryName(new Uri(elemtype.BaseUri).LocalPath), specularAtt.Value);
        specularPath = Path.GetFullPath(specularPath);

        if (!File.Exists(specularPath))
        {
            Debug.LogError("File not found: " + specularPath);
            return false;
        }

        byte[] specularData = File.ReadAllBytes(specularPath);

        Texture2D specularTex = new Texture2D(2, 2);
        specularTex.LoadImage(specularData);

        if (specularTex.width != patternTex.width || specularTex.height != patternTex.height)
        {
            TextureScale.Bilinear(specularTex, patternTex.width, patternTex.height);
        }

        Texture2D combinedMap = new Texture2D(patternTex.width, patternTex.height, TextureFormat.ARGB32, false, false);
        combinedMap.name = patternPath + specularAtt.Value;

        Color[] patternColors = patternTex.GetPixels();
        Color[] specularColors = specularTex.GetPixels();

        for (int i = 0; i < patternColors.Length; i++)
        {
            patternColors[i] = new Color(patternColors[i].r, patternColors[i].g, patternColors[i].b, specularColors[i].linear.r);
        }

        combinedMap.SetPixels(patternColors);
        combinedMap.Apply();


        if (store != null)
            storageIndex = store.AddTexture(combinedMap);
        Texture = combinedMap;
        return true;
    }

    public object ExternalStorage
    {
        set
        {
            store = value as TextureStorage;
            if (store == null)
                Debug.LogError(value + "Is null, for some reason");
        }
    }
}
