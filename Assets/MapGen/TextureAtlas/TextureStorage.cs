using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TextureStorage
{
    List<Texture2D> textureList;

    AtlasCreator.Atlas atlas;

    public Matrix4x4 getUVTransform(int index)
    {
        return atlas.uvRects[index].UVTransform;
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
        textureList.Add(tex);
        return textureList.Count - 1;
    }

    public void BuildAtlas(string name)
    {
        Debug.Log("Making atlas from " + textureList.Count + " items");
        AtlasCreator.Atlas[] atlasList = AtlasCreator.CreateAtlas(name, textureList.ToArray());
        atlas = atlasList[0];
        textureList.Clear();

        AtlasCreator.SaveAtlas(atlas, name);
    }
}
