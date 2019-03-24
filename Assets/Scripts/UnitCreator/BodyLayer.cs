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
    public Color layerColor = Color.white;

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

    public List<BodyModTarget> bodyModTargets;

    internal void ApplyMaterials(CreatureRaw race, MaterialPropertyBlock propertyBlock)
    {
        var renderer = GetComponentInChildren<MeshRenderer>();
        if (renderer != null)
        {
            var tissue = race.tissues[layerRaw.tissue_id];
            var color = ContentLoader.GetColor(tissue.material);
            var index = ContentLoader.GetPatternIndex(tissue.material);
            propertyBlock.SetColor("_MatColor", color);
            propertyBlock.SetFloat("_MatIndex", index);
            renderer.SetPropertyBlock(propertyBlock);
        }
        var skinnedRenderer = GetComponentInChildren<SkinnedMeshRenderer>();
        if (skinnedRenderer != null)
        {
            var tissue = race.tissues[layerRaw.tissue_id];
            var color = ContentLoader.GetColor(tissue.material);
            var index = ContentLoader.GetPatternIndex(tissue.material);
            propertyBlock.SetColor("_MatColor", color);
            propertyBlock.SetFloat("_MatIndex", index);
            skinnedRenderer.SetPropertyBlock(propertyBlock);
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
}
