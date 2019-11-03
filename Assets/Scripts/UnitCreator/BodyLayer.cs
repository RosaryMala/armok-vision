using System;
using System.Collections;
using System.Collections.Generic;
using RemoteFortressReader;
using UnityEngine;

public class BodyLayer : MonoBehaviour, IBodyLayer
{
    public string layerName;
    public bool placed = false;
    public BodyPartLayerRaw layerRaw;
    private readonly List<BodyPart.ModValue> mods = new List<BodyPart.ModValue>();
    public List<Color> layerColors = new List<Color>();
    public PatternDescriptor pattern;
    public bool IsActive { get { return gameObject.activeSelf; } }
    public int TissueID { get { return layerRaw.tissue_id; } }
    public string RawLayerName { get { return layerRaw.layer_name; } }
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

    static bool inited = false;
    static int _MatColorProperty;
    static int _MatIndexProperty;
    static int _ShapeIndexProperty;
    static int _Color1Property;
    static int _Color2Property;
    static int _Color3Property;
    static int _PatternMaskProperty;

    static void GetShaderIDs()
    {
        _MatColorProperty = Shader.PropertyToID("_MatColor");
        _MatIndexProperty = Shader.PropertyToID("_MatIndex");
        _Color1Property = Shader.PropertyToID("_Color1");
        _Color2Property = Shader.PropertyToID("_Color2");
        _Color3Property = Shader.PropertyToID("_Color3");
        _PatternMaskProperty = Shader.PropertyToID("_PatternMask");
        _ShapeIndexProperty = Shader.PropertyToID("_ShapeIndex");
    }

    public List<BodyModTarget> bodyModTargets;

    new Renderer renderer;
    SkinnedMeshRenderer skinnedRenderer;

    private void Init()
    {
        inited = true;
        GetShaderIDs();
        renderer = GetComponentInChildren<MeshRenderer>();
        skinnedRenderer = GetComponentInChildren<SkinnedMeshRenderer>();
        if (renderer == null)
            renderer = skinnedRenderer;
    }

    public void ApplyMaterials(CreatureRaw race, MaterialPropertyBlock propertyBlock)
    {
        if (!inited)
            Init();
        if (renderer != null)
        {
            var tissue = race.tissues[layerRaw.tissue_id];
            var color = ContentLoader.GetColor(tissue.material);
            var index = ContentLoader.GetPatternIndex(tissue.material);
            var shapeIndex = ContentLoader.GetShapeIndex(tissue.material);
            propertyBlock.SetColor(_MatColorProperty, color);
            propertyBlock.SetColor(_Color1Property, color);
            propertyBlock.SetColor(_Color2Property, color);
            propertyBlock.SetColor(_Color3Property, color);
            propertyBlock.SetInt(_MatIndexProperty, index);
            propertyBlock.SetInt(_ShapeIndexProperty, shapeIndex);
            renderer.SetPropertyBlock(propertyBlock);
        }
    }

    public void ApplyMods()
    {
        if (bodyModTargets == null || bodyModTargets.Count == 0)
            return;
        var skinnedRenderer = GetComponentInChildren<SkinnedMeshRenderer>();
        if(skinnedRenderer != null)
        {
            foreach (var bodyModTarget in bodyModTargets)
            {
                BodyPart.ModValue mod;
                bool modFound = TryFindMod(bodyModTarget.modToken, out mod);
                if(modFound)
                {
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

    public bool TryFindMod(string modToken, out BodyPart.ModValue mod)
    {
        if (modToken.Contains("/"))
            return parentPart.TryFindModTree(modToken, out mod);
        int modIndex = mods.FindIndex(x => x.type == modToken);
        if(modIndex >= 0)
        {
            mod = mods[modIndex];
            return true;
        }
        else
        {
            return parentPart.TryFindMod(modToken, out mod);
        }
    }

    static Texture2D SpotsTexture = null;
    static Texture2D StripesTexture = null;
    internal BodyPart parentPart;

    static Color SetAlpha(Color c, float a)
    {
        return new Color(c.r, c.g, c.b, a);
    }

    public void ApplyPattern(PatternDescriptor pattern, float t, MaterialPropertyBlock propertyBlock, int materialIndex, int shapeIndex)
    {
        if (!inited || renderer == null)
            Init();
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

        if (renderer != null)
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
            propertyBlock.SetInt(_ShapeIndexProperty, shapeIndex);
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

            renderer.SetPropertyBlock(propertyBlock);
        }
    }

    public void SetPropertyBlock(MaterialPropertyBlock propertyBlock)
    {
        if (!inited)
            Init();
        if(renderer != null)
            renderer.SetPropertyBlock(propertyBlock);
    }

    public void AddMod(BodyPart.ModValue modValue)
    {
        mods.Add(modValue);
    }
}
