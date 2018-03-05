using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using UnityEngine;

public class GrassContent : IContent
{
    public struct TextureStorageContainer
    {
        public readonly TextureStorage materialStore;
        public readonly TextureStorage shapeStore;

        public TextureStorageContainer(TextureStorage material, TextureStorage shape)
        {
            materialStore = material;
            shapeStore = shape;
        }
    }
    TextureStorageContainer storeContainer;
    public object ExternalStorage
    {
        set
        {
            if (value is TextureStorageContainer)
            {
                storeContainer = (TextureStorageContainer)value;
            }
        }
    }

    NormalContent _normalTexture = null;
    public NormalContent ShapeTexture { get { return _normalTexture; } }

    TextureContent _matTexture = null;
    public TextureContent MaterialTexture { get { return _matTexture; } }

    public bool AddTypeElement(XElement elemtype)
    {
        _normalTexture = new NormalContent();
        _normalTexture.ExternalStorage = storeContainer.shapeStore;
        if (!_normalTexture.AddTypeElement(elemtype))
            _normalTexture = null;

        _matTexture = new TextureContent();
        _matTexture.ExternalStorage = storeContainer.materialStore;
        if (!_matTexture.AddTypeElement(elemtype))
            _matTexture = null;

        return _normalTexture != null && _matTexture != null;
    }
}
