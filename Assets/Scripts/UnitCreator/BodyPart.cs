﻿using System.Collections.Generic;
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

    internal BodyPart FindChild(string category)
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

    public void Arrange(CreatureBody body)
    {
        if (placeholder == null)
            return;
        Shapen(body);
        List<BodyPart> toes = new List<BodyPart>();
        List<BodyPart> fingers = new List<BodyPart>();
        List<BodyPart> mouthParts = new List<BodyPart>();
        BodyPart mouth = null;
        BodyPart beak = null;
        List<BodyPart> leftLegs = new List<BodyPart>();
        List<BodyPart> rightLegs = new List<BodyPart>();
        foreach (Transform child in transform)
        {
            var childPart = child.GetComponent<BodyPart>();
            if (childPart == null)
                continue;
            childPart.Arrange(body);
            switch (childPart.category)
            {
                case "BODY_LOWER":
                    if (body.bodyCategory != CreatureBody.BodyCategory.Humanoid)
                    {
                        childPart.transform.localPosition = new Vector3(0, 0,bounds.min.z);
                        childPart.transform.localRotation = Quaternion.LookRotation(Vector3.back, Vector3.down);
                    }
                    else
                    {
                        childPart.transform.localPosition = new Vector3(0, bounds.min.y, 0);
                        childPart.transform.localRotation = Quaternion.LookRotation(Vector3.down, Vector3.forward);
                    }
                    break;
                case "LEG_UPPER":
                    if (body.bodyCategory == CreatureBody.BodyCategory.Humanoid)
                        childPart.transform.localPosition = new Vector3(bounds.extents.x / 2 * (childPart.flags[BodyPartRawFlags.LEFT] ? -1 : 1), 0, bounds.max.z);
                    else
                    {
                        childPart.transform.localPosition = new Vector3(bounds.max.x * (childPart.flags[BodyPartRawFlags.LEFT] ? -1 : 1), bounds.min.y, bounds.min.z);
                        childPart.transform.localRotation = Quaternion.LookRotation(Vector3.up, Vector3.back);
                    }
                    break;
                case "FOOT":
                case "FOOT_REAR":
                case "FOOT_FRONT":
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
                    childPart.transform.localPosition = new Vector3(0, bounds.max.y, body.bodyCategory != CreatureBody.BodyCategory.Humanoid ? bounds.max.z - childPart.bounds.extents.z : 0);
                    childPart.transform.localRotation = Quaternion.LookRotation(Vector3.up, Vector3.forward);
                    break;
                case "ARM_UPPER":
                    childPart.transform.localPosition = new Vector3((bounds.extents.x + childPart.bounds.extents.x) * (childPart.flags[BodyPartRawFlags.LEFT] ? -1 : 1), bounds.max.y - childPart.bounds.extents.x, 0);
                    childPart.transform.localRotation = Quaternion.LookRotation(Vector3.down, new Vector3(child.transform.localPosition.x, 0, 0));
                    break;
                case "HEAD":
                    if (category == "NECK")
                    {
                        childPart.transform.localPosition = new Vector3(0, 0, bounds.max.z);
                        childPart.transform.localRotation = Quaternion.LookRotation(Vector3.up, Vector3.forward);
                    }
                    else
                    {
                        switch (body.bodyCategory)
                        {
                            case CreatureBody.BodyCategory.Humanoid:
                                childPart.transform.localPosition = new Vector3(bounds.center.x, bounds.max.y, bounds.center.z);
                                break;
                            case CreatureBody.BodyCategory.Bug:
                                childPart.transform.localPosition = new Vector3(0, bounds.center.y - childPart.bounds.center.y, bounds.max.z - childPart.bounds.min.z);
                                break;
                            case CreatureBody.BodyCategory.Quadruped:
                            case CreatureBody.BodyCategory.Avian:
                            default:
                                childPart.transform.localPosition = new Vector3(0, bounds.max.y, bounds.max.z - childPart.bounds.extents.z);
                                break;
                        }
                        childPart.transform.localRotation = Quaternion.LookRotation(Vector3.forward, Vector3.up);
                    }
                    break;
                case "EYE":
                    if (childPart.token.StartsWith("R"))
                        childPart.transform.localPosition = new Vector3(bounds.center.x - bounds.extents.x / 2, bounds.center.y, bounds.max.z);
                    else if (childPart.token.StartsWith("L"))
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
                            childPart.transform.localPosition = new Vector3(bounds.center.x, bounds.center.y + bounds.extents.y / 2 + offset, bounds.max.z);
                        break;
                    }
                case "MOUTH":
                    childPart.transform.localPosition = new Vector3(0, bounds.min.y, bounds.max.z - childPart.bounds.max.z);
                    mouth = childPart;
                    break;
                case "BEAK":
                    childPart.transform.localPosition = new Vector3(bounds.center.x, bounds.min.y - childPart.bounds.min.y, bounds.max.z);
                    mouth = childPart;
                    break;
                case "NOSE":
                    childPart.transform.localPosition = new Vector3(0, bounds.center.y - (bounds.extents.y / 2), bounds.max.z);
                    break;
                case "EAR":
                    childPart.transform.localPosition = new Vector3(bounds.center.x + (bounds.extents.x * (childPart.flags[BodyPartRawFlags.LEFT] ? -1 : 1)), bounds.center.y, bounds.center.z);
                    childPart.transform.localRotation = Quaternion.LookRotation(new Vector3(child.transform.localPosition.x, 0, 0), Vector3.up);
                    break;
                case "CHEEK":
                case "TONGUE":
                case "LIP":
                case "TOOTH":
                case "TUSK":
                    mouthParts.Add(childPart);
                    break;
                case "LEG_FRONT":
                case "LEG_REAR":
                    if (childPart.token.StartsWith("L"))
                        leftLegs.Add(childPart);
                    else
                        rightLegs.Add(childPart);
                    break;
                case "WING":
                    childPart.transform.localPosition = new Vector3(bounds.extents.x * (childPart.flags[BodyPartRawFlags.LEFT] ? -1 : 1), bounds.max.y, bounds.center.z);
                    childPart.transform.localRotation = Quaternion.LookRotation(new Vector3(0, 1, -1), new Vector3(child.transform.localPosition.x, 0, 0));
                    break;
                case "TAIL":
                    childPart.transform.localPosition = new Vector3(bounds.center.x, bounds.min.y, bounds.max.z);
                    childPart.transform.localRotation = Quaternion.LookRotation(new Vector3(0, -1, 1), Vector3.forward);
                    break;
                case "STINGER":
                    childPart.transform.localPosition = new Vector3(bounds.center.x, bounds.center.y, bounds.max.z);
                    childPart.transform.localRotation = Quaternion.LookRotation(new Vector3(0, -1, 1), Vector3.forward);
                    break;
                case "ANTENNA":
                    {
                        bool left = childPart.token.StartsWith("L");
                        childPart.transform.localPosition = new Vector3(bounds.center.x + (bounds.extents.x * (left ? -1 : 1)), bounds.max.y, bounds.max.z);
                        childPart.transform.localRotation = Quaternion.LookRotation(new Vector3((left ? -1 : 1), 1, 0), Vector3.forward);
                    }
                    break;
                case "HORN":
                    {
                        bool left = childPart.token.StartsWith("L");
                        childPart.transform.localPosition = new Vector3(bounds.center.x + (bounds.extents.x * (left ? -1 : 1)), bounds.max.y, bounds.center.z);
                        childPart.transform.localRotation = Quaternion.LookRotation(new Vector3((left ? -1 : 1), 1, 0), Vector3.forward);
                    }
                    break;
                case "SHELL":
                case "HUMP":
                    if (body.bodyCategory != CreatureBody.BodyCategory.Humanoid)
                    {
                        if(FindChild("BODY_LOWER") == null)
                            childPart.transform.localPosition = new Vector3(bounds.center.x, bounds.max.y, bounds.center.z);
                        else
                            childPart.transform.localPosition = new Vector3(bounds.center.x, bounds.max.y, bounds.min.z);
                        childPart.transform.localRotation = Quaternion.LookRotation(Vector3.up, Vector3.forward);
                    }
                    else
                    {
                        childPart.transform.localPosition = new Vector3(bounds.center.x, bounds.center.y, bounds.min.z);
                        childPart.transform.localRotation = Quaternion.LookRotation(Vector3.back, Vector3.up);
                    }
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
            toes[i].transform.localPosition = new Vector3(basecoord + step / 2 + step * i, bounds.min.y - toes[i].bounds.min.y, bounds.max.z);
        }
        for (int i = 0; i < fingers.Count; i++)
        {
            float basecoord = bounds.min.x;
            float step = bounds.size.x / fingers.Count;
            fingers[i].transform.localPosition = new Vector3(basecoord + step / 2 + step * i, bounds.center.y, bounds.max.z);
        }
        if(mouth != null)
            foreach (var childPart in mouthParts)
            {
                switch (childPart.category)
                {
                    case "TONGUE":
                        childPart.transform.SetParent(mouth.transform, false);
                        childPart.transform.localPosition = new Vector3(0, 0, mouth.bounds.max.z - mouth.bounds.extents.y);
                        break;
                    case "LIP":
                        if (childPart.token.StartsWith("L"))
                        {
                            childPart.transform.SetParent(mouth.transform, false);
                            childPart.transform.localPosition = new Vector3(0, mouth.bounds.max.y, mouth.bounds.max.z);
                        }
                        else
                        {
                            childPart.transform.localPosition = new Vector3(0, bounds.min.y, bounds.max.z);
                        }
                        break;
                    case "CHEEK":
                        childPart.transform.localPosition = new Vector3(bounds.center.x + (mouth.bounds.extents.x * (childPart.token.StartsWith("L") ? -1 : 1)), bounds.min.y, bounds.max.z);
                        if (childPart.token.StartsWith("L"))
                            childPart.transform.localScale = new Vector3(-1, 1, 1);
                        break;
                    case "TOOTH":
                        if(childPart.token.StartsWith("U_F_"))
                        {
                            childPart.transform.localPosition = new Vector3(bounds.center.x, bounds.min.y - mouth.bounds.extents.y * 0.01f, bounds.max.z - childPart.bounds.extents.z);
                            childPart.transform.localScale = new Vector3(1, -1, 1);
                        }
                        else if (childPart.token.StartsWith("L_F_"))
                        {
                            childPart.transform.SetParent(mouth.transform);
                            childPart.transform.localPosition = new Vector3(mouth.bounds.center.x, mouth.bounds.max.y + mouth.bounds.extents.y * 0.01f, mouth.bounds.max.z - childPart.bounds.extents.z);
                        }
                        else if (childPart.token.StartsWith("U_R_B_"))
                        {
                            childPart.transform.localPosition = new Vector3(mouth.bounds.max.x - childPart.bounds.extents.z, bounds.min.y - mouth.bounds.extents.y * 0.50f, bounds.max.z - childPart.bounds.extents.x);
                            childPart.transform.localScale = new Vector3(1, -1, 1);
                            childPart.transform.localRotation = Quaternion.Euler(0, 90, 0);
                        }
                        else if (childPart.token.StartsWith("U_L_B_"))
                        {
                            childPart.transform.localPosition = new Vector3(mouth.bounds.min.x + childPart.bounds.extents.z, bounds.min.y - mouth.bounds.extents.y * 0.01f, bounds.max.z - childPart.bounds.extents.x);
                            childPart.transform.localScale = new Vector3(1, -1, -1);
                            childPart.transform.localRotation = Quaternion.Euler(0, 90, 0);
                        }
                        else if (childPart.token.StartsWith("L_R_B_"))
                        {
                            childPart.transform.SetParent(mouth.transform, false);
                            childPart.transform.localPosition = new Vector3(mouth.bounds.max.x - childPart.bounds.extents.z, mouth.bounds.max.y + mouth.bounds.extents.y * 0.01f, mouth.bounds.max.z - childPart.bounds.extents.x);
                            childPart.transform.localScale = new Vector3(1, 1, 1);
                            childPart.transform.localRotation = Quaternion.Euler(0, 90, 0);
                        }
                        else if (childPart.token.StartsWith("L_L_B_"))
                        {
                            childPart.transform.SetParent(mouth.transform, false);
                            childPart.transform.localPosition = new Vector3(mouth.bounds.min.x + childPart.bounds.extents.z, mouth.bounds.max.y + mouth.bounds.extents.y * 0.01f, mouth.bounds.max.z - childPart.bounds.extents.x);
                            childPart.transform.localScale = new Vector3(1, 1, -1);
                            childPart.transform.localRotation = Quaternion.Euler(0, 90, 0);
                        }
                        else if (childPart.token.StartsWith("R_EYE"))
                        {
                            childPart.transform.localPosition = new Vector3(mouth.bounds.max.x - childPart.bounds.extents.z, bounds.min.y, bounds.max.z);
                            childPart.transform.localScale = new Vector3(1, 1, 1);
                            childPart.transform.localRotation = Quaternion.LookRotation(Vector3.down, Vector3.back);
                        }
                        else if (childPart.token.StartsWith("L_EYE"))
                        {
                            childPart.transform.localPosition = new Vector3(mouth.bounds.min.x + childPart.bounds.extents.z, bounds.min.y, bounds.max.z);
                            childPart.transform.localScale = new Vector3(-1, 1, 1);
                            childPart.transform.localRotation = Quaternion.LookRotation(Vector3.down, Vector3.back);
                        }
                        break;
                    case "TUSK":
                        if (childPart.token.StartsWith("R"))
                        {
                            childPart.transform.localPosition = new Vector3(mouth.bounds.max.x - childPart.bounds.extents.z, bounds.min.y, bounds.max.z);
                            childPart.transform.localScale = new Vector3(1, 1, 1);
                            childPart.transform.localRotation = Quaternion.LookRotation(Vector3.down, Vector3.back);
                        }
                        else if (childPart.token.StartsWith("L"))
                        {
                            childPart.transform.localPosition = new Vector3(mouth.bounds.min.x + childPart.bounds.extents.z, bounds.min.y, bounds.max.z);
                            childPart.transform.localScale = new Vector3(-1, 1, 1);
                            childPart.transform.localRotation = Quaternion.LookRotation(Vector3.down, Vector3.back);
                        }
                        break;
                    default:
                        break;
                }
            }
        else if(beak != null)
        {
            foreach (var childPart in mouthParts)
            {
                switch (childPart.category)
                {
                    case "TONGUE":
                        childPart.transform.SetParent(beak.transform, false);
                        childPart.transform.localPosition = bounds.center;
                        break;
                    default:
                        break;
                }
            }
        }
        if (leftLegs.Count == 1)
        {
            if (category == "BODY_UPPER")
            {
                leftLegs[0].transform.localPosition = new Vector3(bounds.min.x, bounds.max.y, bounds.max.z - leftLegs[0].bounds.extents.y);
                leftLegs[0].transform.localRotation = Quaternion.LookRotation(Vector3.down, Vector3.forward);
            }
            else
            {
                leftLegs[0].transform.localPosition = new Vector3(bounds.min.x, bounds.min.y, bounds.max.z - leftLegs[0].bounds.extents.y);
                leftLegs[0].transform.localRotation = Quaternion.LookRotation(Vector3.up, Vector3.back);
            }
        }
        else for (int i = 0; i < leftLegs.Count; i++)
            {
                switch (body.bodyCategory)
                {
                    case CreatureBody.BodyCategory.Bug:
                        leftLegs[i].transform.localPosition = Vector3.Lerp(
                            new Vector3(bounds.min.x, bounds.center.y, bounds.max.z),
                            new Vector3(bounds.min.x, bounds.center.y, bounds.min.z),
                            (float)i / (leftLegs.Count - 1));
                        leftLegs[i].transform.localRotation = Quaternion.Lerp(
                            Quaternion.LookRotation(new Vector3(-1, -1, 1)),
                            Quaternion.LookRotation(new Vector3(-1, -1, -1)),
                            (float)i / (leftLegs.Count - 1));
                        break;
                    case CreatureBody.BodyCategory.Humanoid:
                    case CreatureBody.BodyCategory.Quadruped:
                    case CreatureBody.BodyCategory.Avian:
                    default:
                        //I don't know of any quadrupeds that have all 4 legs on one body part.
                        leftLegs[i].transform.localPosition = Vector3.Lerp(
                            new Vector3(bounds.max.x, bounds.center.y, bounds.max.z),
                            new Vector3(bounds.max.x, bounds.center.y, bounds.min.z),
                            (float)i / (leftLegs.Count));
                        leftLegs[i].transform.localRotation = Quaternion.LookRotation(new Vector3(0, -1, 0));
                        break;
                }
            }
        if (rightLegs.Count == 1)
        {
            if (category == "BODY_UPPER")
            {
                rightLegs[0].transform.localPosition = new Vector3(bounds.max.x, bounds.max.y, bounds.max.z - rightLegs[0].bounds.extents.y);
                rightLegs[0].transform.localRotation = Quaternion.LookRotation(Vector3.down, Vector3.forward);
            }
            else
            {
                rightLegs[0].transform.localPosition = new Vector3(bounds.max.x, bounds.min.y, bounds.max.z - rightLegs[0].bounds.extents.y);
                rightLegs[0].transform.localRotation = Quaternion.LookRotation(Vector3.up, Vector3.back);
            }
        }
        else for (int i = 0; i < rightLegs.Count; i++)
            {
                switch (body.bodyCategory)
                {
                    case CreatureBody.BodyCategory.Bug:
                        rightLegs[i].transform.localPosition = Vector3.Lerp(
                            new Vector3(bounds.max.x, bounds.center.y, bounds.max.z),
                            new Vector3(bounds.max.x, bounds.center.y, bounds.min.z),
                            (float)i / (rightLegs.Count - 1));
                        rightLegs[i].transform.localRotation = Quaternion.Lerp(
                            Quaternion.LookRotation(new Vector3(1, -1, 1)),
                            Quaternion.LookRotation(new Vector3(1, -1, -1)),
                            (float)i / (rightLegs.Count - 1));
                        break;
                    case CreatureBody.BodyCategory.Humanoid:
                    case CreatureBody.BodyCategory.Quadruped:
                    case CreatureBody.BodyCategory.Avian:
                    default:
                        rightLegs[i].transform.localPosition = Vector3.Lerp(
                            new Vector3(bounds.max.x, bounds.center.y, bounds.max.z),
                            new Vector3(bounds.max.x, bounds.center.y, bounds.min.z),
                            (float)i / (rightLegs.Count));
                        rightLegs[i].transform.localRotation = Quaternion.LookRotation(new Vector3(0, -1, 0));
                        break;
                }
            }
    }
    public void Shapen(CreatureBody body)
    {
        switch (category)
        {
            case "BODY_UPPER":
                if (body.bodyCategory != CreatureBody.BodyCategory.Humanoid)
                    placeholder.transform.localScale = new Vector3(1.5f, 1, 1.5f);
                else
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
            case "LEG_FRONT":
            case "LEG_REAR":
                placeholder.transform.localScale = new Vector3(1, 1, 4f);
                placeholder.FixVolume();
                placeholder.transform.localPosition = new Vector3(0, 0, placeholder.transform.localScale.z / 2);
                break;
            case "FOOT":
                placeholder.transform.localScale = new Vector3(0.5f, 0.25f, 1f);
                placeholder.FixVolume();
                placeholder.transform.localPosition = new Vector3(0, -placeholder.transform.localScale.y / 2, (placeholder.transform.localScale.z / 2) - (placeholder.transform.localScale.x / 2));
                break;
            case "FOOT_REAR":
            case "FOOT_FRONT":
                placeholder.transform.localScale = new Vector3(1, 1, 1);
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
                placeholder.transform.localScale = new Vector3(3.5f, 1, 2);
                placeholder.FixVolume();
                placeholder.transform.localPosition = new Vector3(0, -placeholder.transform.localScale.y / 2, placeholder.transform.localScale.z / 2);
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
            case "CHEEK":
                placeholder.transform.localScale = new Vector3(1, 3, 4);
                placeholder.FixVolume();
                placeholder.transform.localPosition = new Vector3(placeholder.transform.localScale.x / 2, 0, -placeholder.transform.localScale.z / 2);
                break;
            case "TONGUE":
                placeholder.transform.localScale = new Vector3(1.5f, 1, 2.8f);
                placeholder.FixVolume();
                placeholder.transform.localPosition = new Vector3(0, -placeholder.transform.localScale.y / 2, -placeholder.transform.localScale.z / 2);
                break;
            case "LIP":
                placeholder.transform.localScale = new Vector3(5, 1, 1f);
                placeholder.FixVolume();
                if (token.StartsWith("U"))
                    placeholder.transform.localPosition = new Vector3(0, placeholder.transform.localScale.y / 2, 0);
                else
                    placeholder.transform.localPosition = new Vector3(0, -placeholder.transform.localScale.y / 2, 0);
                break;
            case "TOOTH":
                if (token.EndsWith("EYE_TOOTH"))
                {
                    placeholder.transform.localScale = new Vector3(1, 1, 6);
                    placeholder.FixVolume();
                    placeholder.transform.localPosition = new Vector3(0, 0, placeholder.transform.localScale.z / 2);
                }
                else
                {
                    placeholder.transform.localScale = new Vector3(6, 1, 1);
                    placeholder.FixVolume();
                    placeholder.transform.localPosition = new Vector3(0, -placeholder.transform.localScale.y / 2, 0);
                }
                break;
            case "TUSK":
                placeholder.transform.localScale = new Vector3(1, 1, 6);
                placeholder.FixVolume();
                placeholder.transform.localPosition = new Vector3(0, 0, placeholder.transform.localScale.z / 2);
                break;
            case "WING":
                placeholder.transform.localScale = new Vector3(10, 1, 20);
                placeholder.FixVolume();
                placeholder.transform.localPosition = new Vector3(-placeholder.transform.localScale.x / 2, 0, placeholder.transform.localScale.z / 2);
                break;
            case "TAIL":
                placeholder.transform.localScale = new Vector3(1, 1, 4);
                placeholder.FixVolume();
                placeholder.transform.localPosition = new Vector3(0, 0, placeholder.transform.localScale.z / 2);
                break;
            case "STINGER":
                placeholder.transform.localScale = new Vector3(1, 1, 5);
                placeholder.FixVolume();
                placeholder.transform.localPosition = new Vector3(0, 0, placeholder.transform.localScale.z / 2);
                break;
            case "ANTENNA":
                placeholder.transform.localScale = new Vector3(1, 1, 8);
                placeholder.FixVolume();
                placeholder.transform.localPosition = new Vector3(0, 0, placeholder.transform.localScale.z / 2);
                break;
            case "HORN":
                placeholder.transform.localScale = new Vector3(1, 1, 4);
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