using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TextureStorage
{
    List<Texture2D> textureList;

    AtlasCreator.Atlas atlas;

    Dictionary<int, string> texIndexToName;

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
        if (textureList == null)
            textureList = new List<Texture2D>();
        if (texIndexToName == null)
            texIndexToName = new Dictionary<int, string>();
        textureList.Add(tex);
        texIndexToName[textureList.Count - 1] = tex.name;
        return textureList.Count - 1;
    }

    public void BuildAtlas(string name, TextureFormat format = TextureFormat.RGBA32, Color defaultColor = default(Color), bool linear = false)
    {
        AtlasCreator.Atlas[] atlasList = AtlasCreator.CreateAtlas(name, textureList.ToArray(), null, format, defaultColor, linear);
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
            texIndexToAtlasIndex[item.Key] = nameToAtlasIndex[item.Value];
        }

        //AtlasCreator.SaveAtlas(atlas, name);
    }
}
