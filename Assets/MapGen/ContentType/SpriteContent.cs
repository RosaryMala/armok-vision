using System;
using System.Xml.Linq;
using UnityEngine;

public class SpriteContent : IContent
{
    TextureStorage store;
    int storageIndex;

    public Matrix4x4 UVTransform
    {
        get
        {
            return store.getUVTransform(storageIndex);
        }
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

    public bool AddTypeElement(XElement elemtype)
    {
        throw new NotImplementedException();
    }
}
