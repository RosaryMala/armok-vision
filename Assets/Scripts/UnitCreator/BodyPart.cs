using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

public class BodyPart : MonoBehaviour
{
    public string token;
    public string category;
    public VolumeKeeper placeholder;
    public BodyPartFlags flags;
    [SerializeField]
    private Bounds bounds;
    public float volume;
    internal BodyPartModel modeledPart;

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

    class ChildPlacement
    {
        public string category;
        public bool categoryRegex;
        public string token;
        public bool tokenRegex;
        public Transform start;
        public Transform end;
        public Transform single;
        bool kill = false;

        List<BodyPart> bodyParts = new List<BodyPart>();

        public ChildPlacement(BodyPartChildPlaceholder placeholder)
        {
            category = placeholder.category;
            categoryRegex = placeholder.categoryRegex;
            token = placeholder.token;
            tokenRegex = placeholder.tokenRegex;
            Add(placeholder);
        }

        public void Add(BodyPartChildPlaceholder placeholder)
        {
            if (kill)
                return;
            switch (placeholder.placement)
            {
                case BodyPartChildPlaceholder.PlacementCategory.Singular:
                    single = placeholder.transform;
                    break;
                case BodyPartChildPlaceholder.PlacementCategory.ArrayStart:
                    start = placeholder.transform;
                    break;
                case BodyPartChildPlaceholder.PlacementCategory.ArrayEnd:
                    end = placeholder.transform;
                    break;
                case BodyPartChildPlaceholder.PlacementCategory.Kill:
                    kill = true;
                    break;
            }
        }

        public bool Matches(BodyPartChildPlaceholder placeholder)
        {
            return
                category == placeholder.category &&
                categoryRegex == placeholder.categoryRegex &&
                token == placeholder.token &&
                tokenRegex == placeholder.tokenRegex;
        }

        public bool Matches(BodyPart part)
        {
            if (!string.IsNullOrEmpty(category))
            {
                if (categoryRegex)
                {
                    if (!Regex.IsMatch(part.category, category))
                        return false;
                }
                else
                {
                    if (category != part.category)
                        return false;
                }
            }
            if (!string.IsNullOrEmpty(token))
            {
                if (tokenRegex)
                {
                    if (!Regex.IsMatch(part.token, token))
                        return false;
                }
                else
                {
                    if (token != part.token)
                        return false;
                }
            }
            return true;
        }

        internal void Place(BodyPart childPart)
        {
            throw new NotImplementedException();
        }

        internal void Add(BodyPart childPart)
        {
            if (kill)
                childPart.gameObject.SetActive(false);
            else
                bodyParts.Add(childPart);
        }

        internal void Arrange()
        {
            //This shouldn't be, but just in case.
            if (kill)
            {
                foreach (var item in bodyParts)
                {
                    item.gameObject.SetActive(false);
                }
                return;
            }
            if (bodyParts.Count == 0)
                return;
            if(bodyParts.Count == 1)
            {
                if (single != null)
                {
                    bodyParts[0].transform.position = single.position;
                    bodyParts[0].transform.rotation = single.rotation;
                    bodyParts[0].transform.localScale = single.localScale;
                }
                else
                    SetTransform(bodyParts[0].transform, 0.5f);
            }
            else
            {
                for(int i = 0; i < bodyParts.Count; i++)
                {
                    SetTransform(bodyParts[i].transform, i / (float)(bodyParts.Count - 1));
                }
            }
        }

        private void SetTransform(Transform trans, float t)
        {
            var tempStart = start;
            var tempEnd = end;
            if(tempStart == null)
            {
                if (end != null)
                    tempStart = end;
                else
                    tempStart = single;
            }
            if(tempEnd == null)
            {
                if (start != null)
                    tempEnd = start;
                else
                    tempEnd = single;
            }
            trans.position = Vector3.Lerp(tempStart.position, tempEnd.position, t);
            trans.rotation = Quaternion.Lerp(tempStart.rotation, tempEnd.rotation, t);
            trans.localScale = Vector3.Lerp(tempStart.localScale, tempEnd.localScale, t);
        }
    }

    void ArrangeModeledPart(CreatureBody body)
    {
        if (modeledPart == null)
            return;
        //If the're flagged as small, they don't need to worry about volume. Just need to scale according to the parent.
        if (!flags.small)
        {
            modeledPart.transform.localScale = body.bodyScale;
            modeledPart.volume = volume;
            modeledPart.FixVolume();
        }
        bounds = modeledPart.GetComponentInChildren<MeshRenderer>().bounds;
        foreach (var renderer in GetComponentsInChildren<MeshRenderer>())
        {
            bounds.Encapsulate(renderer.bounds);
        }
        List<ChildPlacement> placements = new List<ChildPlacement>();
        foreach (Transform child in modeledPart.transform)
        {
            BodyPartChildPlaceholder bodyPartChild = child.GetComponent<BodyPartChildPlaceholder>();
            if (bodyPartChild == null)
                continue;
            bool placedPart = false;
            foreach (var placement in placements)
            {
                if(placement.Matches(bodyPartChild))
                {
                    placement.Add(bodyPartChild);
                    placedPart = true;
                }
            }
            if (!placedPart)
                placements.Add(new ChildPlacement(bodyPartChild));
        }

        List<BodyPart> childParts = new List<BodyPart>();

        foreach (Transform child in transform)
        {
            var childPart = child.GetComponent<BodyPart>();
            if (childPart == null)
                continue;
            childParts.Add(childPart);
        }
        foreach (var childPart in childParts)
        {
            if (childPart.flags.small)
            {
                //Its size doesn't matter if it's considered small, so it should just take the scale directly from the parent part.
                childPart.transform.SetParent(modeledPart.transform, false);
            }
            foreach (var placement in placements)
            {
                if (placement.Matches(childPart))
                {
                    placement.Add(childPart);
                    break;
                }
            }
            childPart.Arrange(body);
        }
        foreach (var placement in placements)
        {
            placement.Arrange();
        }
    }

    public void Arrange(CreatureBody body)
    {
        if (modeledPart != null)
        {
            ArrangeModeledPart(body);
            return;
        }
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
        List<BodyPart> multiArms = new List<BodyPart>();
        List<BodyPart> tentacles = new List<BodyPart>();
        List<BodyPart> centerEyes = new List<BodyPart>();
        foreach (Transform child in transform)
        {
            var childPart = child.GetComponent<BodyPart>();
            if (childPart == null)
                continue;
            childPart.Arrange(body);
            switch (childPart.category)
            {
                case "BODY_LOWER":
                    if (body.bodyCategory == CreatureBody.BodyCategory.Humanoid)
                        childPart.transform.localPosition = new Vector3(0, bounds.min.y, 0);
                    else
                        childPart.transform.localPosition = new Vector3(0, 0, bounds.min.z);
                    break;
                case "LEG_UPPER":
                case "LEG":
                    childPart.transform.localPosition = new Vector3(bounds.extents.x / 2 * (childPart.flags.left ? -1 : 1), bounds.min.y, 0);
                    break;
                case "FOOT":
                case "FOOT_REAR":
                case "FOOT_FRONT":
                    childPart.transform.localPosition = new Vector3(bounds.center.x, bounds.min.y, bounds.center.z);
                    break;
                case "TOE":
                    toes.Add(childPart);
                    break;
                case "FINGER":
                    if (childPart.token.EndsWith("1")) //It's a thumb
                    {
                        childPart.transform.localPosition = new Vector3(bounds.center.x, bounds.center.y, bounds.max.z + childPart.bounds.extents.z);
                    }
                    else
                        fingers.Add(childPart);
                    break;
                case "NECK":
                    switch (body.bodyCategory)
                    {
                        case CreatureBody.BodyCategory.Humanoid:
                            childPart.transform.localPosition = new Vector3(0, bounds.max.y, 0);
                            break;
                        case CreatureBody.BodyCategory.Bug:
                        case CreatureBody.BodyCategory.Fish:
                            childPart.transform.localPosition = new Vector3(bounds.center.x, bounds.center.y, bounds.max.z);
                            childPart.transform.localRotation = Quaternion.LookRotation(Vector3.forward, Vector3.up);
                            break;
                        case CreatureBody.BodyCategory.Quadruped:
                        case CreatureBody.BodyCategory.Avian:
                        default:
                            childPart.transform.localPosition = new Vector3(0, bounds.max.y, bounds.max.z - childPart.bounds.extents.z);
                            childPart.transform.localRotation = Quaternion.LookRotation(Vector3.up, Vector3.forward);
                            break;
                    }
                    break;
                case "ARM_UPPER":
                case "ARM":
                    if (!childPart.flags.left && !childPart.flags.right)
                    {
                        multiArms.Add(childPart);
                    }
                    else
                    {
                        childPart.transform.localPosition = new Vector3((bounds.extents.x + childPart.bounds.extents.x) * (childPart.flags.left ? -1 : 1), bounds.max.y - childPart.bounds.extents.x, 0);
                        childPart.transform.localRotation = Quaternion.LookRotation(Vector3.down, new Vector3(child.transform.localPosition.x, 0, 0));
                    }
                    break;
                case "ARM_LOWER":
                case "LEG_LOWER":
                case "HAND":
                    childPart.transform.localPosition = new Vector3(bounds.center.x, bounds.min.y, bounds.center.z);
                    break;
                case "TENTACLE":
                    tentacles.Add(childPart);
                    break;
                case "HEAD":
                    if (category == "NECK")
                    {
                        switch (body.bodyCategory)
                        {
                            case CreatureBody.BodyCategory.Fish:
                                childPart.transform.localPosition = new Vector3(0, bounds.center.y - childPart.bounds.center.y, bounds.max.z + childPart.bounds.extents.z);
                                childPart.transform.localRotation = Quaternion.LookRotation(Vector3.forward, Vector3.up);
                                break;
                            case CreatureBody.BodyCategory.Humanoid:
                            case CreatureBody.BodyCategory.Quadruped:
                            case CreatureBody.BodyCategory.Avian:
                            case CreatureBody.BodyCategory.Bug:
                            default:
                                childPart.transform.localPosition = new Vector3(0, bounds.max.y, 0);
                                break;
                        }
                    }
                    else
                    {
                        switch (body.bodyCategory)
                        {
                            case CreatureBody.BodyCategory.Humanoid:
                                childPart.transform.localPosition = new Vector3(bounds.center.x, bounds.max.y, bounds.center.z);
                                break;
                            case CreatureBody.BodyCategory.Bug:
                            case CreatureBody.BodyCategory.Fish:
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
                case "EYELID":
                case "EYE":
                    if (childPart.token.StartsWith("R"))
                        childPart.transform.localPosition = new Vector3(bounds.center.x - bounds.extents.x / 2, bounds.center.y, bounds.max.z);
                    else if (childPart.token.StartsWith("L"))
                        childPart.transform.localPosition = new Vector3(bounds.center.x + bounds.extents.x / 2, bounds.center.y, bounds.max.z);
                    else
                        centerEyes.Add(childPart);
                    break;
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
                    childPart.transform.localPosition = new Vector3(bounds.center.x + (bounds.extents.x * (childPart.flags.left ? -1 : 1)), bounds.center.y, bounds.center.z);
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
                    childPart.transform.localPosition = new Vector3(bounds.extents.x * (childPart.flags.left ? -1 : 1), bounds.max.y, bounds.center.z);
                    childPart.transform.localRotation = Quaternion.LookRotation(new Vector3(0, 1, -1), new Vector3(child.transform.localPosition.x, 0, 0));
                    break;
                case "TAIL":
                    if(body.bodyCategory == CreatureBody.BodyCategory.Humanoid)
                    {
                        childPart.transform.localPosition = new Vector3(bounds.center.x, bounds.min.y, bounds.min.z);
                        childPart.transform.localRotation = Quaternion.LookRotation(new Vector3(0, -1, -1), Vector3.up);
                    }
                    else
                    {
                        childPart.transform.localPosition = new Vector3(bounds.center.x, bounds.min.y, bounds.min.z);
                        childPart.transform.localRotation = Quaternion.LookRotation(new Vector3(0, 1, -1), Vector3.up);
                    }
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
                case "FIN":
                    if(childPart.token.StartsWith("R"))
                    {
                        childPart.transform.localPosition = new Vector3(bounds.max.x, bounds.center.y, bounds.center.z);
                        childPart.transform.localRotation = Quaternion.LookRotation(Vector3.right, Vector3.forward);
                    }
                    else if(childPart.token.StartsWith("L"))
                    {
                        childPart.transform.localPosition = new Vector3(bounds.min.x, bounds.center.y, bounds.center.z);
                        childPart.transform.localRotation = Quaternion.LookRotation(Vector3.left, Vector3.forward);
                    }
                    else
                    {
                        childPart.transform.localPosition = new Vector3(bounds.center.x, bounds.max.y, bounds.center.z);
                        childPart.transform.localRotation = Quaternion.LookRotation(Vector3.up, Vector3.forward);
                    }
                    break;
                case "PINCER":
                    childPart.transform.localPosition = new Vector3(bounds.min.x * (childPart.flags.left ? -1 : 1), bounds.center.y, bounds.max.z);
                    childPart.transform.localRotation = Quaternion.LookRotation(new Vector3(childPart.flags.left ? 1 : -1, 0, 1));
                    break;
                case "FLIPPER":
                    childPart.transform.localPosition = new Vector3(bounds.min.x * (childPart.flags.left ? -1 : 1), bounds.center.y, bounds.max.z);
                    break;
                default:
                    childPart.transform.localPosition = new Vector3(bounds.center.x, bounds.center.y, bounds.max.z);
                    break;
            }
            if (childPart.flags.left && !flags.left)
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
            float basecoord = bounds.min.z;
            float step = bounds.size.z / fingers.Count;
            fingers[i].transform.localPosition = new Vector3(bounds.center.x, bounds.min.y, basecoord + step / 2 + step * i);
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
                        }
                        else if (childPart.token.StartsWith("L"))
                        {
                            childPart.transform.localPosition = new Vector3(mouth.bounds.min.x + childPart.bounds.extents.z, bounds.min.y, bounds.max.z);
                            childPart.transform.localScale = new Vector3(-1, 1, 1);
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

        var legRotation = Quaternion.LookRotation(Vector3.up, Vector3.back);
        float legY = bounds.min.y;
        if (category == "BODY_UPPER")
        {
            legRotation = Quaternion.LookRotation(Vector3.down, Vector3.forward);
            legY = bounds.max.y;
        }

        switch (body.bodyCategory)
        {
            case CreatureBody.BodyCategory.Bug:
                AlignManyParts(
                     leftLegs,
                     new Vector3(bounds.min.x, bounds.center.y, bounds.max.z),
                     Quaternion.LookRotation(new Vector3(-1, -1, 1)),
                     new Vector3(bounds.min.x, bounds.center.y, bounds.min.z),
                     Quaternion.LookRotation(new Vector3(-1, -1, -1)),
                     0);
                AlignManyParts(
                     rightLegs,
                     new Vector3(bounds.max.x, bounds.center.y, bounds.max.z),
                     Quaternion.LookRotation(new Vector3(1, -1, 1)),
                     new Vector3(bounds.max.x, bounds.center.y, bounds.min.z),
                     Quaternion.LookRotation(new Vector3(1, -1, -1)),
                     0);
                break;
            case CreatureBody.BodyCategory.Humanoid:
            case CreatureBody.BodyCategory.Quadruped:
            case CreatureBody.BodyCategory.Avian:
            default:
                AlignManyParts(
                    leftLegs,
                    new Vector3(bounds.min.x, legY, bounds.max.z),
                    legRotation,
                    new Vector3(bounds.min.x, legY, bounds.min.z),
                    legRotation,
                    0);
                AlignManyParts(
                    rightLegs,
                    new Vector3(bounds.max.x, legY, bounds.max.z),
                    legRotation,
                    new Vector3(bounds.max.x, legY, bounds.min.z),
                    legRotation,
                    0);
                break;
        }
        AlignManyParts(multiArms,
            new Vector3(bounds.max.x, bounds.min.y, bounds.max.z),
            Quaternion.LookRotation(new Vector3(1, -0.5f, 1)),
            new Vector3(bounds.min.x, bounds.min.y, bounds.max.z),
            Quaternion.LookRotation(new Vector3(-1, -0.5f, 1)),
            0.5f);
        AlignManyParts(tentacles,
            new Vector3(bounds.max.x, bounds.min.y, bounds.max.z),
            Quaternion.LookRotation(new Vector3(1, -1, 1)),
            new Vector3(bounds.min.x, bounds.min.y, bounds.max.z),
            Quaternion.LookRotation(new Vector3(-1, -1, 1)),
            0.5f);
        AlignManyParts(centerEyes,
            new Vector3(bounds.center.x + bounds.extents.x, bounds.center.y + bounds.extents.y / 2, bounds.max.z),
            Quaternion.identity,
            new Vector3(bounds.center.x - bounds.extents.x, bounds.center.y + bounds.extents.y / 2, bounds.max.z),
            Quaternion.identity,
            0.5f);
    }

    void AlignManyParts(List<BodyPart> parts, Vector3 pos1, Quaternion rot1, Vector3 pos2, Quaternion rot2, float singlePos)
    {
        if (parts.Count == 1)
        {
            parts[0].transform.localPosition = Vector3.Lerp(pos1, pos2, singlePos);
            parts[0].transform.localRotation = Quaternion.Lerp(rot1, rot2, singlePos);
        }
        else
            for (int i = 0; i < parts.Count; i++)
            {
                parts[i].transform.localPosition = Vector3.Lerp(pos1, pos2, (float)i / (parts.Count-1));
                parts[i].transform.localRotation = Quaternion.Lerp(rot1, rot2, (float)i / (parts.Count - 1));
            }
    }

    public void Shapen(CreatureBody body)
    {
        switch (category)
        {
            case "BODY_UPPER":
                if (body.bodyCategory == CreatureBody.BodyCategory.Humanoid)
                {
                    placeholder.transform.localScale = MultiplyScales(body.bodyScale,  new Vector3(1.5f, 1.5f, 1));
                    placeholder.FixVolume();
                    placeholder.transform.localPosition = new Vector3(0, placeholder.transform.localScale.y / 2, 0);
                }
                else
                {
                    placeholder.transform.localScale = MultiplyScales(body.bodyScale,  new Vector3(1.5f, 1, 1.5f));
                    placeholder.FixVolume();
                    placeholder.transform.localPosition = new Vector3(0, 0, placeholder.transform.localScale.z / 2);
                }
                break;
            case "BODY_LOWER":
                if (body.bodyCategory == CreatureBody.BodyCategory.Humanoid)
                {
                    placeholder.transform.localScale = MultiplyScales(body.bodyScale,  new Vector3(1.5f, 1.5f, 1));
                    placeholder.FixVolume();
                    placeholder.transform.localPosition = new Vector3(0, -placeholder.transform.localScale.y / 2, 0);
                }
                else
                {
                    placeholder.transform.localScale = MultiplyScales(body.bodyScale,  new Vector3(1.5f, 1, 1.5f));
                    placeholder.FixVolume();
                    placeholder.transform.localPosition = new Vector3(0, 0, -placeholder.transform.localScale.z / 2);
                }
                break;
            case "ARM_UPPER":
                placeholder.transform.localScale = MultiplyScales(body.bodyScale,  new Vector3(0.75f, 2f, 0.75f));
                placeholder.FixVolume();
                placeholder.transform.localPosition = new Vector3(0, -(placeholder.transform.localScale.y / 2) + (placeholder.transform.localScale.x / 2), 0);
                break;
            case "ARM":
                placeholder.transform.localScale = MultiplyScales(body.bodyScale,  new Vector3(0.75f, 4f, 0.75f));
                placeholder.FixVolume();
                placeholder.transform.localPosition = new Vector3(0, -(placeholder.transform.localScale.y / 2) + (placeholder.transform.localScale.x / 2), 0);
                break;
            case "TENTACLE":
                placeholder.transform.localScale = MultiplyScales(body.bodyScale,  new Vector3(0.75f, 0.75f, 4f));
                placeholder.FixVolume();
                placeholder.transform.localPosition = new Vector3(0, 0, placeholder.transform.localScale.z / 2);
                break;
            case "LEG_UPPER":
            case "LEG_LOWER":
            case "ARM_LOWER":
                placeholder.transform.localScale = MultiplyScales(body.bodyScale,  new Vector3(0.75f, 2f, 0.75f));
                placeholder.FixVolume();
                placeholder.transform.localPosition = new Vector3(0, -placeholder.transform.localScale.y / 2, 0);
                break;
            case "LEG":
                placeholder.transform.localScale = MultiplyScales(body.bodyScale,  new Vector3(0.75f, 0.75f, 4f));
                placeholder.FixVolume();
                placeholder.transform.localPosition = new Vector3(0, 0, placeholder.transform.localScale.z / 2);
                break;
            case "LEG_FRONT":
            case "LEG_REAR":
                placeholder.transform.localScale = MultiplyScales(body.bodyScale,  new Vector3(1, 1, 4f));
                placeholder.FixVolume();
                placeholder.transform.localPosition = new Vector3(0, 0, placeholder.transform.localScale.z / 2);
                break;
            case "FOOT":
                placeholder.transform.localScale = MultiplyScales(body.bodyScale,  new Vector3(0.5f, 0.25f, 1f));
                placeholder.FixVolume();
                placeholder.transform.localPosition = new Vector3(0, -placeholder.transform.localScale.y / 2, (placeholder.transform.localScale.z / 2) - (placeholder.transform.localScale.x / 2));
                break;
            case "FOOT_REAR":
            case "FOOT_FRONT":
                placeholder.transform.localScale = MultiplyScales(body.bodyScale,  new Vector3(1, 1, 1));
                placeholder.FixVolume();
                placeholder.transform.localPosition = new Vector3(0, -placeholder.transform.localScale.y / 2, (placeholder.transform.localScale.z / 2) - (placeholder.transform.localScale.x / 2));
                break;
            case "TOE":
                placeholder.transform.localScale = MultiplyScales(body.bodyScale,  new Vector3(1, 1, 2));
                placeholder.FixVolume();
                placeholder.transform.localPosition = new Vector3(0, 0, placeholder.transform.localScale.z / 2);
                break;
            case "HAND":
                placeholder.transform.localScale = MultiplyScales(body.bodyScale,  new Vector3(2, 6, 5));
                placeholder.FixVolume();
                placeholder.transform.localPosition = new Vector3(0, -placeholder.transform.localScale.y / 2, 0);
                break;
            case "FINGER":
                placeholder.transform.localScale = MultiplyScales(body.bodyScale,  new Vector3(1, 4, 1));
                placeholder.FixVolume();
                placeholder.transform.localPosition = new Vector3(0, -placeholder.transform.localScale.y / 2, 0);
                break;
            case "HEAD":
                placeholder.transform.localScale = MultiplyScales(body.bodyScale,  new Vector3(1, 1, 1));
                placeholder.FixVolume();
                placeholder.transform.localPosition = new Vector3(0, placeholder.transform.localScale.y / 2, 0);
                break;
            case "MOUTH":
                placeholder.transform.localScale = MultiplyScales(body.bodyScale,  new Vector3(3.5f, 1, 2));
                placeholder.FixVolume();
                placeholder.transform.localPosition = new Vector3(0, -placeholder.transform.localScale.y / 2, placeholder.transform.localScale.z / 2);
                break;
            case "EYE":
            case "EAR":
                placeholder.transform.localScale = MultiplyScales(body.bodyScale,  Vector3.one);
                placeholder.FixVolume();
                placeholder.transform.localPosition = Vector3.zero;
                break;
            case "EYELID":
                placeholder.transform.localScale = MultiplyScales(body.bodyScale,  new Vector3(2.5f, 1, 1));
                placeholder.FixVolume();
                placeholder.transform.localPosition = new Vector3(0, placeholder.transform.localScale.y, placeholder.transform.localScale.z / 2);
                break;
            case "CHEEK":
                placeholder.transform.localScale = MultiplyScales(body.bodyScale,  new Vector3(1, 3, 4));
                placeholder.FixVolume();
                placeholder.transform.localPosition = new Vector3(placeholder.transform.localScale.x / 2, 0, -placeholder.transform.localScale.z / 2);
                break;
            case "TONGUE":
                placeholder.transform.localScale = MultiplyScales(body.bodyScale,  new Vector3(1.5f, 1, 2.8f));
                placeholder.FixVolume();
                placeholder.transform.localPosition = new Vector3(0, -placeholder.transform.localScale.y / 2, -placeholder.transform.localScale.z / 2);
                break;
            case "LIP":
                placeholder.transform.localScale = MultiplyScales(body.bodyScale,  new Vector3(5, 1, 1f));
                placeholder.FixVolume();
                if (token.StartsWith("U"))
                    placeholder.transform.localPosition = new Vector3(0, placeholder.transform.localScale.y / 2, 0);
                else
                    placeholder.transform.localPosition = new Vector3(0, -placeholder.transform.localScale.y / 2, 0);
                break;
            case "TOOTH":
                if (token.EndsWith("EYE_TOOTH"))
                {
                    placeholder.transform.localScale = MultiplyScales(body.bodyScale,  new Vector3(1, 1, 6));
                    placeholder.FixVolume();
                    placeholder.transform.localPosition = new Vector3(0, 0, placeholder.transform.localScale.z / 2);
                }
                else
                {
                    placeholder.transform.localScale = MultiplyScales(body.bodyScale,  new Vector3(6, 1, 1));
                    placeholder.FixVolume();
                    placeholder.transform.localPosition = new Vector3(0, -placeholder.transform.localScale.y / 2, 0);
                }
                break;
            case "TUSK":
                placeholder.transform.localScale = MultiplyScales(body.bodyScale,  new Vector3(1, 6, 1));
                placeholder.FixVolume();
                placeholder.transform.localPosition = new Vector3(0, placeholder.transform.localScale.y / 2, 0);
                break;
            case "WING":
                placeholder.transform.localScale = MultiplyScales(body.bodyScale,  new Vector3(10, 1, 20));
                placeholder.FixVolume();
                placeholder.transform.localPosition = new Vector3(-placeholder.transform.localScale.x / 2, 0, placeholder.transform.localScale.z / 2);
                break;
            case "TAIL":
                placeholder.transform.localScale = MultiplyScales(body.bodyScale,  new Vector3(1, 4, 1));
                placeholder.FixVolume();
                placeholder.transform.localPosition = new Vector3(0, -placeholder.transform.localScale.y / 2, 0);
                break;
            case "STINGER":
                placeholder.transform.localScale = MultiplyScales(body.bodyScale,  new Vector3(1, 1, 5));
                placeholder.FixVolume();
                placeholder.transform.localPosition = new Vector3(0, 0, placeholder.transform.localScale.z / 2);
                break;
            case "ANTENNA":
                placeholder.transform.localScale = MultiplyScales(body.bodyScale,  new Vector3(1, 1, 8));
                placeholder.FixVolume();
                placeholder.transform.localPosition = new Vector3(0, 0, placeholder.transform.localScale.z / 2);
                break;
            case "HORN":
                placeholder.transform.localScale = MultiplyScales(body.bodyScale,  new Vector3(1, 1, 4));
                placeholder.FixVolume();
                placeholder.transform.localPosition = new Vector3(0, 0, placeholder.transform.localScale.z / 2);
                break;
            case "FIN":
                placeholder.transform.localScale = MultiplyScales(body.bodyScale,  new Vector3(1, 3, 5));
                placeholder.FixVolume();
                placeholder.transform.localPosition = new Vector3(0, -placeholder.transform.localScale.y / 2, placeholder.transform.localScale.z / 2);
                break;
            case "PINCER":
                placeholder.transform.localScale = MultiplyScales(body.bodyScale,  new Vector3(2, 1, 3));
                placeholder.FixVolume();
                placeholder.transform.localPosition = new Vector3(0, 0, placeholder.transform.localScale.z / 2);
                break;
            case "FLIPPER":
                placeholder.transform.localScale = MultiplyScales(body.bodyScale,  new Vector3(3, 1, 2));
                placeholder.FixVolume();
                placeholder.transform.localPosition = new Vector3(-placeholder.transform.localScale.y / 2, 0, placeholder.transform.localScale.z / 2);
                break;
            case "NOSE":
                placeholder.FixVolume();
                placeholder.transform.localPosition = new Vector3(0, 0, placeholder.transform.localScale.z / 2);
                break;
            default:
                placeholder.transform.localScale = MultiplyScales(body.bodyScale,  Vector3.one);
                placeholder.FixVolume();
                if (flags.embedded)
                    placeholder.transform.localPosition = Vector3.zero;
                else
                    placeholder.transform.localPosition = new Vector3(0, placeholder.transform.localScale.y / 2, 0);
                break;
        }
        bounds = new Bounds(placeholder.transform.localPosition, placeholder.transform.localScale);
    }

    static Vector3 MultiplyScales(Vector3 a, Vector3 b)
    {
        return new Vector3(a.x * b.x, a.y * b.y, a.z * b.z);
    }
}
