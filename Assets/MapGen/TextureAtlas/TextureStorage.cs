using UnityEngine;
using System.Collections.Generic;

public class TextureStorage
{
    List<Texture2D> textureList = new List<Texture2D>();

    AtlasCreator.Atlas atlas;

    Dictionary<int, string> texIndexToName = new Dictionary<int, string>();

    Dictionary<string, int> nameToAtlasIndex;

    Dictionary<int, int> texIndexToAtlasIndex;

    public Matrix4x4 getUVTransform(int index)
    {
        return atlas.uvRects[texIndexToAtlasIndex[index]].UVTransform;
    }

    public Texture2D AtlasTexture
    {
        get
        {
            return atlas.texture;
        }
    }

    public int AddTexture(Texture2D tex)
    {
        textureList.Add(tex);
        texIndexToName[textureList.Count - 1] = tex.name;
        return textureList.Count - 1;
    }

    static int CompareBySize(Texture2D b, Texture2D a)
    {
        if(a == null)
        {
            if (b == null)
                return 0;
            else
                return -1;
        }
        else
        {
            if (b == null)
                return 1;
            else
            {
                return (a.width * a.height).CompareTo(b.width * b.height);
            }
        }
    }

    public void BuildAtlas(string name, TextureFormat format = TextureFormat.RGBA32, Color defaultColor = default(Color), bool linear = false)
    {
        textureList.Sort(CompareBySize);
        AtlasCreator.Atlas[] atlasList = AtlasCreator.CreateAtlas(name, textureList.ToArray(), null, format, defaultColor, linear);
        if((atlasList.Length > 1) || (GameSettings.Instance.rendering.debugTextureAtlas))
        {
            for(int i = 0; i < atlasList.Length; i++)
            {
                AtlasCreator.SaveAtlas(atlasList[i], i+name);
            }
        }
        atlas = atlasList[0];
        textureList.Clear();

        texIndexToAtlasIndex = new Dictionary<int, int>();
        nameToAtlasIndex = new Dictionary<string, int>();
        for (int i = 0; i < atlas.uvRects.Length; i++)
        {
            nameToAtlasIndex[atlas.uvRects[i].name] = i;
        }

        foreach (var item in texIndexToName)
        {
            if(nameToAtlasIndex.ContainsKey(item.Value))
                texIndexToAtlasIndex[item.Key] = nameToAtlasIndex[item.Value];
            else
            {
                Debug.LogError("What the fuck");
            }
        }

        //AtlasCreator.SaveAtlas(atlas, name);
    }
}
