using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RemoteFortressReader;

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

    public void Arrange()
    {
        if (placeholder == null)
            return;
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
            case "LEG_UPPER":
            case "LEG_LOWER":
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
            default:
                placeholder.transform.localScale = new Vector3(1,1,1);
                placeholder.FixVolume();
                placeholder.transform.localPosition = new Vector3(0, 0, placeholder.transform.localScale.z / 2);
                break;
        }
        List<BodyPart> toes = new List<BodyPart>();
        foreach (Transform child in transform)
        {
            var childPart = child.GetComponent<BodyPart>();
            if (childPart == null)
                continue;
            switch (childPart.category)
            {
                case "BODY_LOWER":
                    childPart.transform.localPosition = new Vector3(0, -placeholder.transform.localScale.y / 2, 0);
                    childPart.transform.localRotation = Quaternion.LookRotation(Vector3.down, Vector3.forward);
                    break;
                case "LEG_UPPER":
                    childPart.transform.localPosition = new Vector3(placeholder.transform.localScale.x / 4 * (childPart.flags[BodyPartRawFlags.LEFT] ? -1 : 1), 0, placeholder.transform.localScale.z);
                    break;
                case "LEG_LOWER":
                    childPart.transform.localPosition = new Vector3(0, 0, placeholder.transform.localScale.z);
                    break;
                case "FOOT":
                    childPart.transform.localPosition = new Vector3(0, 0, placeholder.transform.localScale.z);
                    childPart.transform.localRotation = Quaternion.LookRotation(Vector3.up, Vector3.back);
                    break;
                case "TOE":
                    toes.Add(childPart);
                    break;
                default:
                    childPart.transform.localPosition = new Vector3(0, 0, placeholder.transform.localScale.z);
                    break;
            }
            if (childPart.flags[BodyPartRawFlags.LEFT] && !flags[BodyPartRawFlags.LEFT])
                childPart.transform.localScale = new Vector3(-1, 1, 1);
            childPart.Arrange();
        }
        for (int i = 0; i < toes.Count; i++)
        {
            float basecoord = -placeholder.transform.localScale.x / 2;
            float step = placeholder.transform.localScale.x / toes.Count;
            toes[i].transform.localPosition = new Vector3(basecoord + step / 2 + step * i, -placeholder.transform.localScale.y / 2, placeholder.transform.localScale.z - (placeholder.transform.localScale.x / 2));
        }
    }
}
