using System;
using System.IO;
using System.Xml.Linq;
using UnityEngine;

public class NormalContent : IContent
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
        XAttribute normalAtt = elemtype.Attribute("normal");
        Texture2D normalMap = ContentLoader.LoadTexture(normalAtt, elemtype, new Color(0.5f, 0.5f, 1f), true);

        XAttribute alphaAtt = elemtype.Attribute("alpha");
        Texture2D alphaMap = ContentLoader.LoadTexture(alphaAtt, elemtype, Color.white, true);

        XAttribute occlusionAtt = elemtype.Attribute("occlusion");
        Texture2D occlusionMap = ContentLoader.LoadTexture(occlusionAtt, elemtype, Color.white, true);

        XAttribute patternAtt = elemtype.Attribute("pattern");
        Texture2D patternTex = ContentLoader.LoadTexture(patternAtt, elemtype, Color.gray);

        GameSettings.MatchSizes(new Texture2D[] { normalMap, alphaMap, occlusionMap, patternTex });

        Texture2D combinedMap = new Texture2D(normalMap.width, normalMap.height, TextureFormat.ARGB32, true, true);
        combinedMap.filterMode = FilterMode.Trilinear;

        if(!string.IsNullOrEmpty(normalMap.name))
        {
            combinedMap.name = normalMap.name + occlusionAtt.Value + alphaAtt.Value + patternAtt.Value;
        }
        else if(!string.IsNullOrEmpty(occlusionMap.name))
        {
            combinedMap.name = occlusionMap.name + alphaAtt.Value + patternAtt.Value;
        }
        else if (!string.IsNullOrEmpty(alphaMap.name))
        {
            combinedMap.name = alphaMap.name + patternAtt.Value;
        }
        else
        {
            combinedMap.name = patternTex.name;
        }

        Color[] normalColors = normalMap.GetPixels();
        Color[] occlusionColors = occlusionMap.GetPixels();
        Color[] alphaColors = alphaMap.GetPixels();
        Color[] patternColors = patternTex.GetPixels();

        for (int i = 0; i < normalColors.Length; i++)
        {
            normalColors[i] = new Color(occlusionColors[i].r, normalColors[i].g, alphaColors[i].r * patternColors[i].a, normalColors[i].r);
        }

        combinedMap.SetPixels(normalColors);
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
