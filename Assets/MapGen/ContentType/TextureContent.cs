using System.Xml.Linq;
using UnityEngine;
using System.IO;
using System;

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
        Texture2D patternTex = new Texture2D(2, 2);
        XAttribute patternAtt = elemtype.Attribute("pattern");
        string name = "";
        if (patternAtt == null)
        {
            patternTex = ContentLoader.CreateFlatTexture(Color.grey);
        }
        else
        {
            string patternPath = Path.Combine(Path.GetDirectoryName(new Uri(elemtype.BaseUri).LocalPath), patternAtt.Value);
            patternPath = Path.GetFullPath(patternPath);
            name += patternPath;
            if (!File.Exists(patternPath))
            {
                Debug.LogError("File not found: " + patternPath);
                return false;
            }

            byte[] patternData = File.ReadAllBytes(patternPath);

            patternTex.LoadImage(patternData);
        }
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
        Texture2D specularTex = new Texture2D(2, 2);
        if (specularAtt == null)
        {
            specularTex = ContentLoader.CreateFlatTexture(Color.black);
        }
        else
        {
            string specularPath = Path.Combine(Path.GetDirectoryName(new Uri(elemtype.BaseUri).LocalPath), specularAtt.Value);
            specularPath = Path.GetFullPath(specularPath);
            if (string.IsNullOrEmpty(name))
                name = specularPath;
            else
                name += specularAtt.Value;
            if (!File.Exists(specularPath))
            {
                Debug.LogError("File not found: " + specularPath);
                return false;
            }

            byte[] specularData = File.ReadAllBytes(specularPath);

            specularTex.LoadImage(specularData);
        }

        if (specularTex.width != patternTex.width || specularTex.height != patternTex.height)
        {
            TextureScale.Bilinear(specularTex, patternTex.width, patternTex.height);
        }

        Texture2D combinedMap = new Texture2D(patternTex.width, patternTex.height, TextureFormat.ARGB32, true, false);
        combinedMap.filterMode = FilterMode.Trilinear;
        combinedMap.name = name;

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
        else
            storageIndex = -1;
        Texture = combinedMap;
        return true;
    }

    public object ExternalStorage
    {
        set
        {
            store = value as TextureStorage;
        }
    }
}
