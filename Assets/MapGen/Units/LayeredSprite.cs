using System;
using System.Collections;
using System.Collections.Generic;
using RemoteFortressReader;
using UnityEngine;
using MaterialStore;
using DF.Enums;

public class LayeredSprite : MonoBehaviour
{
    private static Material _spriteMat;
    private static Material _spriteMatTrans;

    public static Material SpriteMat
    {
        get
        {
            if (_spriteMat == null)
                _spriteMat = Resources.Load<Material>("SpriteMat");
            return _spriteMat;
        }
    }
    public static Material SpriteMatTrans
    {
        get
        {
            if (_spriteMatTrans == null)
                _spriteMatTrans = Resources.Load<Material>("SpriteMatTrans");
            return _spriteMatTrans;
        }
    }

    public float gap = 0.0001f;
    public CreatureSpriteCollection SpriteCollection
    {
        get
        {
            return currentSpriteCollection;
        }
        set
        {
            if(value != currentSpriteCollection)
            {
                currentSpriteCollection = value;
                if (currentSpriteCollection == null)
                    Clear();
                else
                    BuildSpriteLayers(currentSpriteCollection.spriteLayers);
            }
        }
    }
    private CreatureSpriteCollection currentSpriteCollection = null;
    private List<SpriteRenderer> spriteList = new List<SpriteRenderer>();

    private void Start()
    {
        if (SpriteCollection == null)
        {
            enabled = false;
            return;
        }
    }

    public void UpdateLayers(List<CreatureSpriteLayer> spriteLayers)
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
        Clear();
        float depth = 0;
        foreach (var layer in spriteLayers)
        {
            GameObject go = new GameObject(layer.spriteTexture.name);
            go.transform.SetParent(transform, false);
            var sr = go.AddComponent<SpriteRenderer>();
            sr.color = layer.color;
            sr.sprite = layer.spriteTexture;
            if(layer.color.a < 0.5f)
                sr.sharedMaterial = SpriteMatTrans;
            else
                sr.sharedMaterial = SpriteMat;
            sr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
            sr.receiveShadows = true;
            var pos = go.transform.localPosition;
            pos.z = depth;
            pos.x += layer.positionOffset.x;
            pos.y += layer.positionOffset.y;
            go.transform.localPosition = pos;
            spriteList.Add(sr);
            depth -= gap;
        }
    }

    private void Clear()
    {
        foreach (var sprite in spriteList)
        {
            Destroy(sprite.gameObject);
        }
        spriteList.Clear();
    }

    internal void UpdateLayers(UnitDefinition unit, CreatureRaw creatureRaw, CasteRaw casteRaw)
    {
        for (int i = 0; i < spriteList.Count; i++)
        {
            var spriteLayerDef = SpriteCollection.spriteLayers[i];
            var sprite = spriteList[i];
            switch (spriteLayerDef.spriteSource)
            {
                case CreatureSpriteLayer.SpriteSource.Static:
                    sprite.gameObject.SetActive(true);
                    switch (spriteLayerDef.colorSource)
                    {
                        case CreatureSpriteLayer.ColorSource.Fixed:
                            sprite.color = spriteLayerDef.color;
                            break;
                        case CreatureSpriteLayer.ColorSource.Job:
                            sprite.color = new Color(unit.profession_color.red / 255.0f, unit.profession_color.green / 255.0f, unit.profession_color.blue / 255.0f, 0.5f);
                            break;
                        default:
                            sprite.color = new Color32(128, 128, 128, 128);
                            break;
                    }
                    break;
                case CreatureSpriteLayer.SpriteSource.Bodypart:
                    sprite.gameObject.SetActive(true);
                    switch (spriteLayerDef.colorSource)
                    {
                        case CreatureSpriteLayer.ColorSource.Fixed:
                            sprite.color = spriteLayerDef.color;
                            break;
                        case CreatureSpriteLayer.ColorSource.Material:
                            ColorDefinition unitColor = new ColorDefinition();
                            int colorModIndex = casteRaw.color_modifiers.FindIndex(x => x.part == spriteLayerDef.token && x.start_date == 0);
                            if(colorModIndex >= 0 && unit.appearance != null)
                            {
                                unitColor = casteRaw.color_modifiers[colorModIndex].patterns[unit.appearance.colors[colorModIndex]].colors[spriteLayerDef.patternIndex];
                                sprite.color = new Color32((byte)unitColor.red, (byte)unitColor.green, (byte)unitColor.blue, 128);
                            }
                            else
                            {
                                int tissueIndex = creatureRaw.tissues.FindIndex(x => x.name == spriteLayerDef.token);
                                if (tissueIndex >= 0)
                                    sprite.color = ContentLoader.GetColor(creatureRaw.tissues[tissueIndex].material);
                                else
                                    sprite.gameObject.SetActive(false);
                                break;
                            }
                            break;
                        case CreatureSpriteLayer.ColorSource.Job:
                            sprite.color = new Color(unit.profession_color.red / 255.0f, unit.profession_color.green / 255.0f, unit.profession_color.blue / 255.0f, 0.5f);
                            break;
                        case CreatureSpriteLayer.ColorSource.BodyPart:
                            var part = casteRaw.body_parts.Find(x => x.token == spriteLayerDef.token);
                            if(part == null)
                            {
                                sprite.gameObject.SetActive(false);
                                break;
                            }
                            sprite.color = ContentLoader.GetColor(creatureRaw.tissues[part.layers[0].tissue_id].material);
                            break;
                        default:
                            sprite.color = new Color32(128, 128, 128, 128);
                            break;
                    }
                    switch (spriteLayerDef.hairType)
                    {
                        case CreatureSpriteLayer.HairType.Hair:
                            if (unit.inventory.FindIndex(x => x.item.type.mat_type == (int)item_type.HELM) >= 0)
                            {
                                sprite.gameObject.SetActive(false);
                                break;
                            }
                            if(unit.appearance == null || unit.appearance.hair == null)
                            {
                                sprite.gameObject.SetActive(spriteLayerDef.hairStyle == HairStyle.UNKEMPT);
                            }
                            else
                            {
                                sprite.gameObject.SetActive((spriteLayerDef.hairStyle == unit.appearance.hair.style)
                                    && (spriteLayerDef.hairMin <= unit.appearance.hair.length)
                                    && ((spriteLayerDef.hairMax < 0) || (spriteLayerDef.hairMax > unit.appearance.hair.length)));
                            }
                            break;
                        case CreatureSpriteLayer.HairType.Beard:
                            if (unit.appearance == null || unit.appearance.beard == null)
                            {
                                sprite.gameObject.SetActive(spriteLayerDef.hairStyle == HairStyle.UNKEMPT);
                            }
                            else
                            {
                                sprite.gameObject.SetActive((spriteLayerDef.hairStyle == unit.appearance.beard.style)
                                    && (spriteLayerDef.hairMin <= unit.appearance.beard.length)
                                    && ((spriteLayerDef.hairMax < 0) || (spriteLayerDef.hairMax > unit.appearance.beard.length)));
                            }
                            break;
                        case CreatureSpriteLayer.HairType.Moustache:
                            if (unit.appearance == null || unit.appearance.moustache == null)
                            {
                                sprite.gameObject.SetActive(spriteLayerDef.hairStyle == HairStyle.UNKEMPT);
                            }
                            else
                            {
                                sprite.gameObject.SetActive((spriteLayerDef.hairStyle == unit.appearance.moustache.style)
                                    && (spriteLayerDef.hairMin <= unit.appearance.moustache.length)
                                    && ((spriteLayerDef.hairMax < 0) || (spriteLayerDef.hairMax > unit.appearance.moustache.length)));
                            }
                            break;
                        case CreatureSpriteLayer.HairType.Sideburns:
                            if (unit.appearance == null || unit.appearance.sideburns == null)
                            {
                                sprite.gameObject.SetActive(spriteLayerDef.hairStyle == HairStyle.UNKEMPT);
                            }
                            else
                            {
                                sprite.gameObject.SetActive((spriteLayerDef.hairStyle == unit.appearance.sideburns.style)
                                    && (spriteLayerDef.hairMin <= unit.appearance.sideburns.length)
                                    && ((spriteLayerDef.hairMax < 0) || (spriteLayerDef.hairMax > unit.appearance.sideburns.length)));
                            }
                            break;
                        default:
                            sprite.gameObject.SetActive(true);
                            break;
                    }
                    break;
                case CreatureSpriteLayer.SpriteSource.Equipment:
                    int inventoryIndex = unit.inventory.FindIndex(
                        x =>
                        (x.mode == InventoryMode.Weapon || x.mode == InventoryMode.Worn)
                        && spriteLayerDef.token == GameMap.items[x.item.type].id
                        );
                    if (inventoryIndex < 0)
                    {
                        sprite.gameObject.SetActive(false);
                        break;
                    }
                    else
                        sprite.enabled = true;
                    var item = unit.inventory[inventoryIndex].item;

                    switch (spriteLayerDef.colorSource)
                    {
                        case CreatureSpriteLayer.ColorSource.Fixed:
                            sprite.color = spriteLayerDef.color;
                            break;
                        case CreatureSpriteLayer.ColorSource.Material:
                            sprite.color = ContentLoader.GetColor(item);
                            break;
                        case CreatureSpriteLayer.ColorSource.Job:
                            sprite.color = new Color(unit.profession_color.red / 255.0f, unit.profession_color.green / 255.0f, unit.profession_color.blue / 255.0f, 0.5f);
                            break;
                        default:
                            sprite.color = new Color32(128, 128, 128, 128);
                            break;
                    }
                    break;
                default:
                    sprite.gameObject.SetActive(false);
                    break;
            }
            if (sprite.color.a < 0.5f)
                sprite.sharedMaterial = SpriteMatTrans;
            else
                sprite.sharedMaterial = SpriteMat;

        }
    }
}
