using System.Xml.Linq;
using UnityEngine;
using System.IO;
using System;
using UnityExtension;

public class TextureContent : IContent
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

    public bool AddTypeElement(System.Xml.Linq.XElement elemtype)
    {
        if (store == null) //nowhere to put the image.
        {
            Debug.LogError("Texture Storage is Null: " + elemtype);
            return false;
        }

        XAttribute fileAtt = elemtype.Attribute("file");
        if (fileAtt == null)
        {
            Debug.LogError("No file attribute in " + elemtype);
            //Add error message here
            return false;
        }
        string filePath = Path.Combine(Path.GetDirectoryName(new Uri(elemtype.BaseUri).LocalPath), fileAtt.Value);
        filePath = Path.GetFullPath(filePath);

        if (!File.Exists(filePath))
        {
            Debug.LogError("File not found: " + filePath);
            return false;
        }

        byte[] fileData = File.ReadAllBytes(filePath);

        Texture2D tex = new Texture2D(2,2);
        tex.LoadImage(fileData);

        tex.name = filePath;

        storageIndex = store.AddTexture(tex);
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
