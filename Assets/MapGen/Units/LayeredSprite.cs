using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LayeredSprite : MonoBehaviour
{
    private static Material _spriteMat;

    public static Material SpriteMat
    {
        get
        {
            if (_spriteMat == null)
                _spriteMat = Resources.Load<Material>("SpriteMat");
            return _spriteMat;
        }
    }

    public float gap = 0.0001f;
    public CreatureSpriteCollection spriteCollection;
    private void Start()
    {
        BuildSpriteLayers(spriteCollection.spriteLayers);
    }

    private void BuildSpriteLayers(List<CreatureSpriteLayer> spriteLayers)
    {
        float depth = 0;
        foreach (var layer in spriteLayers)
        {
            if (!layer.preview)
                continue;
            GameObject go = new GameObject(layer.spriteTexture.name);
            go.transform.SetParent(transform);
            var sr = go.AddComponent<SpriteRenderer>();
            sr.color = layer.color;
            sr.sprite = layer.spriteTexture;
            sr.sharedMaterial = SpriteMat;
            var pos = go.transform.localPosition;
            pos.z = depth;
            go.transform.position = pos;
            depth -= gap;
        }
    }
}
