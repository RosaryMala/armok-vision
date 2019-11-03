using System.Collections;
using System.Collections.Generic;
using RemoteFortressReader;
using UnityEngine;

public interface IBodyLayer
{
    bool IsActive { get; }
    int TissueID { get; }
    string RawLayerName { get; }
    void AddMod(BodyPart.ModValue modValue);
    bool TryFindMod(string modToken, out BodyPart.ModValue mod);
}
