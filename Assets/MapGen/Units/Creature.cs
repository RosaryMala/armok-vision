using RemoteFortressReader;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using System.Globalization;
using System.Threading;

public class Creature : MonoBehaviour
{
    public CreatureRaw creatureRaw;
    public CasteRaw casteRaw;
    public UnitDefinition unit;
    public Color professionColor;

    static MaterialPropertyBlock creatureMaterialProperties = null;
    static int layerIndexID;
    static int layerColorID;
    static bool inited = false;

    static TextInfo textInfo;

    MeshRenderer legacySprite;
    LayeredSprite layeredSprite;

    static void InitProperties()
    {
        layerIndexID = Shader.PropertyToID("_LayerIndex");
        layerColorID = Shader.PropertyToID("_LayerColor");
        creatureMaterialProperties = new MaterialPropertyBlock();
        textInfo = Thread.CurrentThread.CurrentCulture.TextInfo;
        inited = true;
    }

    bool localInited = false;
    void InitLocal()
    {
        legacySprite = GetComponentInChildren<MeshRenderer>();
        layeredSprite = GetComponentInChildren<LayeredSprite>();
        localInited = true;
    }

    internal void UpdateCreature(UnitDefinition unit)
    {
        if (!inited)
            InitProperties();
        if (!localInited)
            InitLocal();
        this.unit = unit;
        if (DFConnection.Instance.CreatureRaws != null)
            creatureRaw = DFConnection.Instance.CreatureRaws[unit.race.mat_type];
        else
            return; //can't work without raws
        casteRaw = creatureRaw.caste[unit.race.mat_index];

        if (unit.profession_color != null)
            professionColor = new Color(unit.profession_color.red / 255.0f, unit.profession_color.green / 255.0f, unit.profession_color.blue / 255.0f, 0.5f);
        else
            professionColor = new Color(1, 1, 1, 0.5f);


        var layers = LayeredSpriteManager.Instance.GetCreatureSprite(unit);

        if (layers != null)
        {
            layeredSprite.SpriteCollection = layers;
            layeredSprite.enabled = true;
            legacySprite.enabled = false;
            layeredSprite.UpdateLayers(unit, creatureRaw, casteRaw);
            var group = GetComponentInChildren<LODGroup>();
            if (group != null)
            {
                var lods = new LOD[1];
                lods[0] = new LOD(0.05f, GetComponentsInChildren<SpriteRenderer>());
                group.SetLODs(lods);
                group.RecalculateBounds();
            }
        }
        else
        {
            layeredSprite.SpriteCollection = layers;
            layeredSprite.enabled = false;
            legacySprite.enabled = true;
            UpdateTileCreature();
            var group = GetComponentInChildren<LODGroup>();
            if (group != null)
            {
                var lods = new LOD[1];
                lods[0] = new LOD(0.05f, GetComponentsInChildren<MeshRenderer>());
                group.SetLODs(lods);
                group.RecalculateBounds();
            }
        }

    }

    /// <summary>
    /// Used as a last resort, when all we have is the original creature tile
    /// </summary>
    void UpdateTileCreature()
    {
        Material mat;
        int index;
        bool colored;
        if (ContentLoader.Instance.SpriteManager.getCreatureSprite(unit, out mat, out index, out colored))
        {
            legacySprite.material = mat;
            creatureMaterialProperties.SetFloat(layerIndexID, index);
            if (colored)
                creatureMaterialProperties.SetColor(layerColorID, professionColor);
            else
                creatureMaterialProperties.SetColor(layerColorID, Color.white);
        }
        else
        {
            legacySprite.material = CreatureManager.Instance.baseTileMaterial;
            if (unit.is_soldier && creatureRaw.creature_soldier_tile != 0)
            {
                creatureMaterialProperties.SetFloat(layerIndexID, creatureRaw.creature_soldier_tile);
            }
            else
            {
                creatureMaterialProperties.SetFloat(layerIndexID, creatureRaw.creature_tile);
            }
            creatureMaterialProperties.SetColor(layerColorID, professionColor);
        }

        legacySprite.SetPropertyBlock(creatureMaterialProperties);

        Text unitText = GetComponentInChildren<Text>();
        if (unitText != null && textInfo != null)
        {
            if (unit.name == "")
                unitText.text = textInfo.ToTitleCase(creatureRaw.caste[unit.race.mat_index].caste_name[0]);
            else
            {
                unitText.text = unit.name;
            }
        }

    }
}
