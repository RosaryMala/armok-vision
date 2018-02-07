using UnityEngine;
using System.Collections.Generic;
using System;

public class TextureStorage
{
    List<Texture2D> textureList = new List<Texture2D>();

    public static bool UsingArray { get; private set; }

    AtlasCreator.Atlas atlas;

    Texture2DArray textureArray;

    Dictionary<int, string> texIndexToName = new Dictionary<int, string>();

    Dictionary<string, int> nameToAtlasIndex;

    Dictionary<int, int> texIndexToAtlasIndex;

    public int Count { get { return textureList.Count; } }

    public Matrix4x4 getUVTransform(int index)
    {
        if (UsingArray)
            return Matrix4x4.identity;
        return atlas.uvRects[texIndexToAtlasIndex[index]].UVTransform;
    }

    public Texture AtlasTexture
    {
        get
        {
            if (UsingArray)
                return textureArray;
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

    public void CompileTextures(string name, TextureFormat format = TextureFormat.RGBA32, Color defaultColor = default(Color), bool linear = false)
    {
        if(SystemInfo.supports2DArrayTextures)
        {
            UsingArray = true;
            BuildTextureArray(format, defaultColor, linear);
        }
        else
        {
            UsingArray = false;
            BuildAtlas(name, format, defaultColor, linear);
        }
    }

    void BuildAtlas(string name, TextureFormat format = TextureFormat.RGBA32, Color defaultColor = default(Color), bool linear = false)
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

    void BuildTextureArray(TextureFormat format = TextureFormat.RGBA32, Color defaultColor = default(Color), bool linear = false)
    {
        int maxWidth = 0;
        int maxHeight = 0;
        foreach (var texture in textureList)
        {
            maxWidth = Mathf.Max(maxWidth, texture.width);
            maxHeight = Mathf.Max(maxHeight, texture.height);
        }

        textureArray = new Texture2DArray(maxWidth, maxHeight, textureList.Count, format, true, linear);

        for (int i = 0; i < textureList.Count; i++)
        {
            if (textureList[i].width < maxWidth || textureList[i].height < maxHeight)
                TextureScale.Bilinear(textureList[i], maxWidth, maxHeight);

            textureArray.SetPixels(textureList[i].GetPixels(), i);
        }
        textureArray.Apply();
    }

    internal void AddTextureArray(Texture2DArray array)
    {
        for (int i = 0; i < array.depth; i++)
        {
            var tex = new Texture2D(array.width, array.height, array.format, false);
            tex.name = array.name + "[" + i + "]";
            tex.SetPixels(array.GetPixels(i));
            tex.Apply();
            AddTexture(tex);
        }
    }
}
