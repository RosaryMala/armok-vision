using System.Collections;
using System.Collections.Generic;
using RemoteFortressReader;
using UnityEngine;

public class BodyLayerPlaceholder : IBodyLayer
{
    private BodyPartLayerRaw layerRaw;

    public BodyLayerPlaceholder(BodyPartLayerRaw layerRaw)
    {
        this.layerRaw = layerRaw;
    }

    public bool IsActive { get { return false; } }

    public int TissueID { get { return layerRaw.tissue_id; } }
    public string RawLayerName { get { return layerRaw.layer_name; } }

    public List<BodyPart.ModValue> Mods { get { return mods; } }

    private readonly List<BodyPart.ModValue> mods = new List<BodyPart.ModValue>();
    public void AddMod(BodyPart.ModValue modValue)
    {
        mods.Add(modValue);
    }

    public bool TryFindMod(string modToken, out BodyPart.ModValue mod)
    {
        int modIndex = mods.FindIndex(x => x.type == modToken);
        if (modIndex >= 0)
        {
            mod = mods[modIndex];
            return true;
        }
        mod = default;
        return false;
    }
}
