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
    private List<SpriteRenderer> spriteList;

    private void Start()
    {
        BuildSpriteLayers(spriteCollection.spriteLayers);
    }

    private void Update()
    {
        UpdateLayers(spriteCollection.spriteLayers);
    }

    private void UpdateLayers(List<CreatureSpriteLayer> spriteLayers)
    {
        for(int i = 0; i < spriteList.Count; i++)
        {
            spriteList[i].gameObject.SetActive(spriteLayers[i].preview);
            if(spriteLayers[i].colorSource != CreatureSpriteLayer.ColorSource.None)
                spriteList[i].color = spriteLayers[i].color;
        }
    }

    private void BuildSpriteLayers(List<CreatureSpriteLayer> spriteLayers)
    {
        spriteList = new List<SpriteRenderer>();
        float depth = 0;
        foreach (var layer in spriteLayers)
        {
            GameObject go = new GameObject(layer.spriteTexture.name);
            go.transform.SetParent(transform, false);
            var sr = go.AddComponent<SpriteRenderer>();
            sr.color = layer.color;
            sr.sprite = layer.spriteTexture;
            sr.sharedMaterial = SpriteMat;
            var pos = go.transform.localPosition;
            pos.z = depth;
            pos.x += layer.positionOffset.x;
            pos.y += layer.positionOffset.y;
            go.transform.localPosition = pos;
            go.SetActive(layer.preview);
            spriteList.Add(sr);
            depth -= gap;
        }
    }
}
