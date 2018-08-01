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
}
