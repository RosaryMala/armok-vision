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
        }
    }

    public bool AddTypeElement(XElement elemtype)
    {
        XAttribute metalAtt = elemtype.Attribute("metallic");
        Texture2D metalMap = ContentLoader.LoadTexture(metalAtt, elemtype, Color.black, false);

        XAttribute illuminationAtt = elemtype.Attribute("illumination");
        Texture2D illuminationMap = ContentLoader.LoadTexture(illuminationAtt, elemtype, Color.black, false);

        GameSettings.MatchSizes(metalMap, illuminationMap);

        Texture2D combinedMap = new Texture2D(metalMap.width, metalMap.height, TextureFormat.ARGB32, true, true);
        combinedMap.filterMode = FilterMode.Trilinear;
        if (string.IsNullOrEmpty(metalMap.name))
            combinedMap.name = illuminationMap.name;
        else
            combinedMap.name = metalMap.name + illuminationAtt.Value;

        Color[] metalColors = metalMap.GetPixels();
        Color[] illuminationColors = illuminationMap.GetPixels();

        for (int i = 0; i < metalColors.Length; i++)
        {
            metalColors[i] = new Color(metalColors[i].r, illuminationColors[i].g, 1, 1);
        }

        combinedMap.SetPixels(metalColors);
        combinedMap.Apply();

        if (store != null)
            storageIndex = store.AddTexture(combinedMap);
        else
            storageIndex = -1;
        Texture = combinedMap;

        return true;
    }
}
