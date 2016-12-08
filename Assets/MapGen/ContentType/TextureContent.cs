using System.Xml.Linq;
using UnityEngine;

public class TextureContent : IContent
{
    static int num_created = 0;
    public static int NumCreated { get { return num_created; } }
    public int UniqueIndex { get; private set; }

    TextureStorage store;
    public int StorageIndex { get; private set; }

    public Texture2D Texture { get; private set; }

    public Matrix4x4 UVTransform
    {
        get
        {
            return store.getUVTransform(StorageIndex);
        }
    }

    public float ArrayIndex
    {
        get
        {
            return (float)StorageIndex / store.Count;
        }
    }

    public bool AddTypeElement(XElement elemtype)
    {
        XAttribute patternAtt = elemtype.Attribute("pattern");
        Texture2D patternTex = ContentLoader.LoadTexture(patternAtt, elemtype, Color.gray);

        XAttribute specularAtt = elemtype.Attribute("specular");
        Texture2D specularTex = ContentLoader.LoadTexture(specularAtt, elemtype, Color.black);

        GameSettings.MatchSizes(patternTex, specularTex);

        Texture2D combinedMap = new Texture2D(patternTex.width, patternTex.height, TextureFormat.ARGB32, true, false);
        combinedMap.filterMode = FilterMode.Trilinear;
        combinedMap.name = patternTex.name + specularTex.name;

        Color[] patternColors = patternTex.GetPixels();
        Color[] specularColors = specularTex.GetPixels();

        for (int i = 0; i < patternColors.Length; i++)
        {
            patternColors[i] = new Color(patternColors[i].r, patternColors[i].g, patternColors[i].b, specularColors[i].linear.r);
        }

        combinedMap.SetPixels(patternColors);
        combinedMap.Apply();

        if (store != null)
            StorageIndex = store.AddTexture(combinedMap);
        else
            StorageIndex = -1;
        Texture = combinedMap;
        UniqueIndex = num_created;
        num_created++;
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
