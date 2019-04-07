using System;
using System.Collections;
using System.Collections.Generic;
using RemoteFortressReader;
using UnityEngine;

public class BodyLayer : MonoBehaviour
{
    public string layerName;
    public bool placed = false;
    public BodyPartLayerRaw layerRaw;
    public List<BodyPart.ModValue> mods = new List<BodyPart.ModValue>();
    public List<Color> layerColors = new List<Color>();
    public PatternDescriptor pattern;

    [System.Serializable]
    public class BodyModTarget
    {
        public string modToken;
        public bool useSmall;
        public bool useMinimum;
        public int minimumValue;
        public int smallIndex;
        public int smallValue;
        public int medValue;
        public int largeIndex;
        public int largeValue;
    }

    static bool gotShaderIds = false;
    static int _MatColorProperty;
    static int _MatIndexProperty;
    static int _Color1Property;
    static int _Color2Property;
    static int _Color3Property;
    static int _PatternMaskProperty;

    static void GetShaderIDs()
    {
        gotShaderIds = true;
        _MatColorProperty = Shader.PropertyToID("_MatColor");
        _MatIndexProperty = Shader.PropertyToID("_MatIndex");
        _Color1Property = Shader.PropertyToID("_Color1");
        _Color2Property = Shader.PropertyToID("_Color2");
        _Color3Property = Shader.PropertyToID("_Color3");
        _PatternMaskProperty = Shader.PropertyToID("_PatternMask");
    }

    public List<BodyModTarget> bodyModTargets;

    internal void ApplyMaterials(CreatureRaw race, MaterialPropertyBlock propertyBlock)
    {
        if (!gotShaderIds)
            GetShaderIDs();
        var renderer = GetComponentInChildren<MeshRenderer>();
        var skinnedRenderer = GetComponentInChildren<SkinnedMeshRenderer>();
        if (renderer != null || skinnedRenderer != null)
        {
            var tissue = race.tissues[layerRaw.tissue_id];
            var color = ContentLoader.GetColor(tissue.material);
            var index = ContentLoader.GetPatternIndex(tissue.material);
            propertyBlock.SetColor(_MatColorProperty, color);
            propertyBlock.SetColor(_Color1Property, color);
            propertyBlock.SetColor(_Color2Property, color);
            propertyBlock.SetColor(_Color3Property, color);
            propertyBlock.SetFloat(_MatIndexProperty, index);
            if (renderer != null)
            {
                renderer.SetPropertyBlock(propertyBlock);
            }
            if (skinnedRenderer != null)
            {
                skinnedRenderer.SetPropertyBlock(propertyBlock);
            }
        }
    }

    internal void ApplyMods()
    {
        if (bodyModTargets == null || bodyModTargets.Count == 0)
            return;
        var skinnedRenderer = GetComponentInChildren<SkinnedMeshRenderer>();
        if(skinnedRenderer != null)
        {
            foreach (var bodyModTarget in bodyModTargets)
            {
                int modIndex = mods.FindIndex(x => x.type == bodyModTarget.modToken);
                if(modIndex >= 0)
                {
                    var mod = mods[modIndex];
                    if (bodyModTarget.useMinimum && mod.value < bodyModTarget.minimumValue)
                    {
                        gameObject.SetActive(false);
                        return;
                    }

                    if(bodyModTarget.useSmall)
                    {
                        if(mod.value < bodyModTarget.medValue)
                        {
                            skinnedRenderer.SetBlendShapeWeight(bodyModTarget.smallIndex, Mathf.Lerp(0, 100, Mathf.InverseLerp(bodyModTarget.medValue, bodyModTarget.smallValue, mod.value)));
                            skinnedRenderer.SetBlendShapeWeight(bodyModTarget.largeIndex, 0);
                        }
                        else if(mod.value > bodyModTarget.medValue)
                        {
                            skinnedRenderer.SetBlendShapeWeight(bodyModTarget.largeIndex, Mathf.Lerp(0, 100, Mathf.InverseLerp(bodyModTarget.medValue, bodyModTarget.largeValue, mod.value)));
                            skinnedRenderer.SetBlendShapeWeight(bodyModTarget.smallIndex, 0);
                        }
                        else
                        {
                            skinnedRenderer.SetBlendShapeWeight(bodyModTarget.largeIndex, 0);
                            skinnedRenderer.SetBlendShapeWeight(bodyModTarget.smallIndex, 0);
                        }
                    }
                    else
                    {
                        skinnedRenderer.SetBlendShapeWeight(bodyModTarget.largeIndex, Mathf.Lerp(0, 100, Mathf.InverseLerp(bodyModTarget.medValue, bodyModTarget.largeValue, mod.value)));
                    }
                }
            }
        }
    }

    static Texture2D SpotsTexture = null;
    static Texture2D StripesTexture = null;

    static Color SetAlpha(Color c, float a)
    {
        return new Color(c.r, c.g, c.b, a);
    }

    internal void ApplyPattern(PatternDescriptor pattern, float t, MaterialPropertyBlock propertyBlock, int materialIndex)
    {
        if (!gotShaderIds)
            GetShaderIDs();
        this.pattern = pattern;
        if (pattern.colors.Count == 0)
            return;
        if (layerColors.Count == 0)
        {
            for (int i = 0; i < pattern.colors.Count; i++)
            {
                layerColors.Add(SetAlpha(pattern.colors[i], 0.5f));
            }
        }
        else
        {
            if (layerColors.Count < pattern.colors.Count)
            {
                var lastColor = layerColors[layerColors.Count - 1];
                for (int i = 0; i < pattern.colors.Count; i++)
                {
                    layerColors.Add(SetAlpha(lastColor, 0.5f));
                }
            }
            for (int i = 0; i < layerColors.Count; i++)
            {
                layerColors[i] = Color.Lerp(layerColors[i], SetAlpha(pattern.colors[Mathf.Min(i, pattern.colors.Count - 1)], 0.5f), t);
            }
        }


        var renderer = GetComponentInChildren<MeshRenderer>();
        var skinnedRenderer = GetComponentInChildren<SkinnedMeshRenderer>();
        if (renderer != null || skinnedRenderer != null)
        {
            propertyBlock.Clear();
            switch (pattern.pattern)
            {
                case PatternType.MONOTONE:
                    break;
                case PatternType.STRIPES:
                    if (StripesTexture == null)
                        StripesTexture = Resources.Load<Texture2D>("MASK_STRIPES");
                    propertyBlock.SetTexture(_PatternMaskProperty, StripesTexture);
                    break;
                case PatternType.IRIS_EYE:
                    break;
                case PatternType.SPOTS:
                    if (SpotsTexture == null)
                        SpotsTexture = Resources.Load<Texture2D>("MASK_SPOTS");
                    propertyBlock.SetTexture(_PatternMaskProperty, SpotsTexture);
                    break;
                case PatternType.PUPIL_EYE:
                    break;
                case PatternType.MOTTLED:
                    break;
                default:
                    break;
            }

            propertyBlock.SetInt(_MatIndexProperty, materialIndex);
            switch (layerColors.Count)
            {
                case 0:
                    break;
                case 1:
                    propertyBlock.SetColor(_MatColorProperty, layerColors[0]);
                    propertyBlock.SetColor(_Color1Property, layerColors[0]);
                    propertyBlock.SetColor(_Color2Property, layerColors[0]);
                    propertyBlock.SetColor(_Color3Property, layerColors[0]);
                    break;
                case 2:
                    propertyBlock.SetColor(_MatColorProperty, layerColors[0]);
                    propertyBlock.SetColor(_Color1Property, layerColors[0]);
                    propertyBlock.SetColor(_Color2Property, layerColors[1]);
                    propertyBlock.SetColor(_Color3Property, layerColors[1]);
                    break;
                case 3:
                default:
                    propertyBlock.SetColor(_MatColorProperty, layerColors[0]);
                    propertyBlock.SetColor(_Color1Property, layerColors[0]);
                    propertyBlock.SetColor(_Color2Property, layerColors[1]);
                    propertyBlock.SetColor(_Color3Property, layerColors[2]);
                    break;
            }

            if (renderer != null)
            {
                renderer.SetPropertyBlock(propertyBlock);
            }
            if (skinnedRenderer != null)
            {
                skinnedRenderer.SetPropertyBlock(propertyBlock);
            }
        }
    }
}
