using System.Collections.Generic;
using UnityEngine;

public class BodyPart : MonoBehaviour
{
    public enum BodyPartRawFlags
    {
        HEAD,
        UPPERBODY,
        LOWERBODY,
        SIGHT,
        EMBEDDED,
        INTERNAL,
        CIRCULATION,
        SKELETON,
        LIMB,
        GRASP,
        STANCE,
        GUTS,
        BREATHE,
        SMALL,
        THROAT,
        JOINT,
        THOUGHT,
        NERVOUS,
        RIGHT,
        LEFT,
        HEAR,
        SMELL,
        FLIER,
        DIGIT,
        MOUTH,
        APERTURE,
        SOCKET,
        TOTEMABLE,
        anon_1,
        anon_2,
        UNDER_PRESSURE,
        anon_3,
        VERMIN_BUTCHER_ITEM,
        CONNECTOR,
        anon_4,
        anon_5,
        anon_6,
        anon_7,
        GELDABLE
    };

    public string token;
    public string category;
    public VolumeKeeper placeholder;
    public Dictionary<BodyPartRawFlags, bool> flags = new Dictionary<BodyPartRawFlags, bool>();
    [SerializeField]
    private Bounds bounds;
    public float volume;

    private BodyPart FindChild(string category)
    {
        foreach (Transform child in transform)
        {
            var childPart = child.GetComponent<BodyPart>();
            if (childPart == null)
                continue;
            if (childPart.category == category)
                return childPart;
        }
        return null;
    }

    public void Arrange()
    {
        if (placeholder == null)
            return;
        Shapen();
        List<BodyPart> toes = new List<BodyPart>();
        List<BodyPart> fingers = new List<BodyPart>();
        foreach (Transform child in transform)
        {
            var childPart = child.GetComponent<BodyPart>();
            if (childPart == null)
                continue;
            childPart.Arrange();
            switch (childPart.category)
            {
                case "BODY_LOWER":
                    childPart.transform.localPosition = new Vector3(0, bounds.min.y, 0);
                    childPart.transform.localRotation = Quaternion.LookRotation(Vector3.down, Vector3.forward);
                    break;
                case "LEG_UPPER":
                    childPart.transform.localPosition = new Vector3(bounds.extents.x / 2 * (childPart.flags[BodyPartRawFlags.LEFT] ? -1 : 1), 0, bounds.max.z);
                    break;
                case "FOOT":
                    childPart.transform.localPosition = new Vector3(0, 0, bounds.max.z);
                    childPart.transform.localRotation = Quaternion.LookRotation(Vector3.up, Vector3.back);
                    break;
                case "TOE":
                    toes.Add(childPart);
                    break;
                case "FINGER":
                    if (childPart.token.EndsWith("1")) //It's a thumb
                    {
                        childPart.transform.localPosition = new Vector3(-bounds.extents.x - childPart.bounds.extents.x, 0, bounds.center.z);
                    }
                    else
                        fingers.Add(childPart);
                    break;
                case "NECK":
                    childPart.transform.localPosition = new Vector3(0, bounds.max.y, 0);
                    childPart.transform.localRotation = Quaternion.LookRotation(Vector3.up, Vector3.forward);
                    break;
                case "ARM_UPPER":
                    childPart.transform.localPosition = new Vector3((bounds.extents.x + childPart.bounds.extents.x) * (childPart.flags[BodyPartRawFlags.LEFT] ? -1 : 1), bounds.max.y - childPart.bounds.extents.x, 0);
                    childPart.transform.localRotation = Quaternion.LookRotation(Vector3.down, new Vector3(child.transform.localPosition.x, 0, 0));
                    break;
                case "HEAD":
                    childPart.transform.localPosition = new Vector3(0, 0, bounds.max.z);
                    childPart.transform.localRotation = Quaternion.LookRotation(Vector3.up, Vector3.forward);
                    break;
                case "EYE":
                    if (childPart.flags[BodyPartRawFlags.RIGHT])
                        childPart.transform.localPosition = new Vector3(bounds.center.x - bounds.extents.x / 2, bounds.center.y, bounds.max.z);
                    else if (childPart.flags[BodyPartRawFlags.LEFT])
                        childPart.transform.localPosition = new Vector3(bounds.center.x + bounds.extents.x / 2, bounds.center.y, bounds.max.z);
                    else
                        childPart.transform.localPosition = new Vector3(bounds.center.x, bounds.center.y + bounds.extents.y / 2, bounds.max.z);
                    break;
                case "EYELID":
                    {
                        float offset = 0;
                        var eyeball = FindChild("EYE");
                        if (eyeball != null)
                            offset = Mathf.Pow(eyeball.volume, 1 / 3.0f) / 200;
                        if (childPart.token.StartsWith("R"))
                            childPart.transform.localPosition = new Vector3(bounds.center.x - bounds.extents.x / 2, bounds.center.y + offset, bounds.max.z);
                        else if (childPart.token.StartsWith("L"))
                            childPart.transform.localPosition = new Vector3(bounds.center.x + bounds.extents.x / 2, bounds.center.y + offset, bounds.max.z);
                        else
                            childPart.transform.localPosition = new Vector3(bounds.center.x, bounds.center.y + bounds.extents.y / 2, bounds.max.z);
                        break;
                    }
                case "MOUTH":
                    childPart.transform.localPosition = new Vector3(0, bounds.min.y, bounds.max.z);
                    break;
                case "NOSE":
                    childPart.transform.localPosition = new Vector3(0, bounds.center.y - (bounds.extents.y / 2), bounds.max.z);
                    break;
                case "EAR":
                    childPart.transform.localPosition = new Vector3(bounds.center.x + (bounds.extents.x * (childPart.flags[BodyPartRawFlags.LEFT] ? -1 : 1)), bounds.center.y, bounds.center.z);
                    childPart.transform.localRotation = Quaternion.LookRotation(new Vector3(child.transform.localPosition.x, 0, 0), Vector3.up);
                    break;
                default:
                    childPart.transform.localPosition = new Vector3(0, 0, bounds.max.z);
                    break;
            }
            if (childPart.flags[BodyPartRawFlags.LEFT] && !flags[BodyPartRawFlags.LEFT])
                childPart.transform.localScale = new Vector3(-1, 1, 1);
        }
        for (int i = 0; i < toes.Count; i++)
        {
            float basecoord = bounds.min.x;
            float step = bounds.size.x / toes.Count;
            toes[i].transform.localPosition = new Vector3(basecoord + step / 2 + step * i, bounds.center.y, bounds.max.z);
        }
        for (int i = 0; i < fingers.Count; i++)
        {
            float basecoord = bounds.min.x;
            float step = bounds.size.x / fingers.Count;
            fingers[i].transform.localPosition = new Vector3(basecoord + step / 2 + step * i, bounds.center.y, bounds.max.z);
        }
    }
    public void Shapen()
    {
        switch (category)
        {
            case "BODY_UPPER":
                placeholder.transform.localScale = new Vector3(1.5f, 1.5f, 1);
                placeholder.FixVolume();
                break;
            case "BODY_LOWER":
                placeholder.transform.localScale = new Vector3(1.5f, 1, 1.5f);
                placeholder.FixVolume();
                placeholder.transform.localPosition = new Vector3(0, 0, placeholder.transform.localScale.z / 2);
                break;
            case "ARM_UPPER":
                placeholder.transform.localScale = new Vector3(0.75f, 0.75f, 2f);
                placeholder.FixVolume();
                placeholder.transform.localPosition = new Vector3(0, 0, (placeholder.transform.localScale.z / 2) - (placeholder.transform.localScale.x / 2));
                break;
            case "LEG_UPPER":
            case "LEG_LOWER":
            case "ARM_LOWER":
                placeholder.transform.localScale = new Vector3(0.75f, 0.75f, 2f);
                placeholder.FixVolume();
                placeholder.transform.localPosition = new Vector3(0, 0, placeholder.transform.localScale.z / 2);
                break;
            case "FOOT":
                placeholder.transform.localScale = new Vector3(0.5f, 0.25f, 1f);
                placeholder.FixVolume();
                placeholder.transform.localPosition = new Vector3(0, -placeholder.transform.localScale.y / 2, (placeholder.transform.localScale.z / 2) - (placeholder.transform.localScale.x / 2));
                break;
            case "TOE":
                placeholder.transform.localScale = new Vector3(1, 1, 2);
                placeholder.FixVolume();
                placeholder.transform.localPosition = new Vector3(0, 0, placeholder.transform.localScale.z / 2);
                break;
            case "HAND":
                placeholder.transform.localScale = new Vector3(5, 2, 6);
                placeholder.FixVolume();
                placeholder.transform.localPosition = new Vector3(0, 0, placeholder.transform.localScale.z / 2);
                break;
            case "FINGER":
                placeholder.transform.localScale = new Vector3(1, 1, 4);
                placeholder.FixVolume();
                placeholder.transform.localPosition = new Vector3(0, 0, placeholder.transform.localScale.z / 2);
                break;
            case "HEAD":
                placeholder.transform.localScale = new Vector3(1, 1, 1);
                placeholder.FixVolume();
                placeholder.transform.localPosition = new Vector3(0, placeholder.transform.localScale.y / 2, 0);
                break;
            case "MOUTH":
                placeholder.transform.localScale = new Vector3(2, 1, 1);
                placeholder.FixVolume();
                placeholder.transform.localPosition = Vector3.zero;
                break;
            case "EYE":
            case "EAR":
                placeholder.transform.localScale = Vector3.one;
                placeholder.FixVolume();
                placeholder.transform.localPosition = Vector3.zero;
                break;
            case "EYELID":
                placeholder.transform.localScale = new Vector3(2.5f, 1, 1);
                placeholder.FixVolume();
                placeholder.transform.localPosition = new Vector3(0, 0, placeholder.transform.localScale.z / 2);
                break;
            default:
                placeholder.transform.localScale = Vector3.one;
                placeholder.FixVolume();
                if (flags[BodyPartRawFlags.EMBEDDED])
                    placeholder.transform.localPosition = Vector3.zero;
                else
                    placeholder.transform.localPosition = new Vector3(0, 0, placeholder.transform.localScale.z / 2);
                break;
        }
        bounds = new Bounds(placeholder.transform.localPosition, placeholder.transform.localScale);
    }
}
