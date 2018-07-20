﻿using System.Collections.Generic;
using RemoteFortressReader;
using UnityEngine;

public class CreatureBody : MonoBehaviour
{
    public enum BodyCategory
    {
        Humanoid,
        Quadruped,
        Avian,
        Bug
    }
    public BodyCategory bodyCategory;

    public CreatureRaw race;
    public CasteRaw caste;
    public BodyPart rootPart;
    public int stanceCount;
    public Bounds bounds;

    public void MakeBody()
    {
        var spawnedParts = new Dictionary<int, BodyPart>();
        float scale = caste.adult_size / (float)caste.total_relsize * 10;
        for (int i = 0; i < caste.body_parts.Count; i++)
        {
            var part = caste.body_parts[i];
            if (part.flags[(int)BodyPart.BodyPartRawFlags.INTERNAL])
                continue;
            var spawnedPart = new GameObject().AddComponent<BodyPart>();
            spawnedPart.name = string.Format("{0} ({1})", part.token, part.category);
            spawnedPart.token = part.token;
            spawnedPart.category = part.category;
            for (int j = 0; j < part.flags.Count; j++)
            {
                spawnedPart.flags[(BodyPart.BodyPartRawFlags)j] = part.flags[j];
            }
            var cube = GameObject.CreatePrimitive(PrimitiveType.Cube).AddComponent<VolumeKeeper>();
            cube.name = spawnedPart.name + " cube";
            cube.transform.SetParent(spawnedPart.transform);
            cube.volume = part.relsize * scale;
            cube.FixVolume();
            spawnedPart.volume = cube.volume;
            spawnedPart.placeholder = cube;
            spawnedParts[i] = spawnedPart;
        }
        for (int i = 0; i < caste.body_parts.Count; i++)
        {
            if (!spawnedParts.ContainsKey(i))
                continue;
            var part = caste.body_parts[i];
            if (!spawnedParts.ContainsKey(part.parent))
                spawnedParts[i].transform.SetParent(transform);
            else
                spawnedParts[i].transform.SetParent(spawnedParts[part.parent].transform);
            if (part.parent < 0)
                rootPart = spawnedParts[i];
            if (spawnedParts[i].flags[BodyPart.BodyPartRawFlags.STANCE])
                stanceCount++;
        }

        if (stanceCount > 4)
            bodyCategory = BodyCategory.Bug;
        else if (stanceCount > 2)
            bodyCategory = BodyCategory.Quadruped;
        else if (rootPart.FindChild("ARM_UPPER") == null)
            bodyCategory = BodyCategory.Avian;
        else
            bodyCategory = BodyCategory.Humanoid;

        rootPart.Arrange(this);
        bounds = rootPart.GetComponentInChildren<MeshRenderer>().bounds;
        foreach (var item in rootPart.GetComponentsInChildren<MeshRenderer>())
        {
            bounds.Encapsulate(item.bounds);
        }
        rootPart.transform.localPosition = new Vector3(0, -bounds.min.y, 0);

    }
}