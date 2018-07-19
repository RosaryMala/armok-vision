using System.Collections.Generic;
using DFHack;
using dfproto;
using RemoteFortressReader;
using UnityEditor;
using UnityEngine;

public class DFRawReader : EditorWindow
{
    [SerializeField]
    private Vector2 scrollPosition;
    [SerializeField]
    private List<CreatureRaw> creatureRaws;

    [MenuItem("Mytools/DF Raw Reader")]
    public static void ShowWindow()
    {
        GetWindow<DFRawReader>();
    }

    private void OnGUI()
    {
        if (GUILayout.Button("Read Raws"))
        {
            var client = new RemoteClient();
            if (!client.Connect())
                return;
            var getCreatureRaws = new RemoteFunction<EmptyMessage, CreatureRawList>(client, "GetCreatureRaws", "RemoteFortressReader");
            creatureRaws = getCreatureRaws.Execute().creature_raws;
            Debug.Log(string.Format("Pulled {0} creature raws from DF.", creatureRaws.Count));
            client.Disconnect();
            creatureRaws.Sort((x, y) => x.creature_id.CompareTo(y.creature_id));
        }
        if (creatureRaws != null)
        {
            scrollPosition = GUILayout.BeginScrollView(scrollPosition);
            foreach (var creature in creatureRaws)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label(creature.creature_id);

                foreach (var caste in creature.caste)
                {
                    if (GUILayout.Button(caste.caste_id))
                    {
                        float scale = caste.adult_size / (float)caste.total_relsize;

                        var spawnedParts = new Dictionary<int, BodyPart>();
                        var creatureBase = new GameObject().AddComponent<CreatureBody>();
                        creatureBase.name = creature.creature_id + "_" + caste.caste_id;
                        creatureBase.race = creature;
                        creatureBase.caste = caste;
                        BodyPart rootPart = null;
                        for (int i = 0; i < caste.body_parts.Count; i++)
                        {
                            var part = caste.body_parts[i];
                            if (part.flags[(int)BodyPart.BodyPartRawFlags.INTERNAL])
                                continue;
                            var spawnedPart = new GameObject().AddComponent<BodyPart>();
                            spawnedPart.name = string.Format("{0} ({1})", part.token, part.category);
                            spawnedPart.token = part.token;
                            spawnedPart.category = part.category;
                            for(int j = 0; j < part.flags.Count; j++)
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
                        int stanceParts = 0;
                        for (int i = 0; i < caste.body_parts.Count; i++)
                        {
                            if (!spawnedParts.ContainsKey(i))
                                continue;
                            var part = caste.body_parts[i];
                            if (!spawnedParts.ContainsKey(part.parent))
                                spawnedParts[i].transform.SetParent(creatureBase.transform);
                            else
                                spawnedParts[i].transform.SetParent(spawnedParts[part.parent].transform);
                            if (part.parent < 0)
                                rootPart = spawnedParts[i];
                            if (spawnedParts[i].flags[BodyPart.BodyPartRawFlags.STANCE])
                                stanceParts++;
                        }
                        Debug.Log(string.Format("Found {0} feet.", stanceParts));
                        rootPart.Arrange();
                        var bounds = rootPart.GetComponentInChildren<MeshRenderer>().bounds;
                        foreach (var item in rootPart.GetComponentsInChildren<MeshRenderer>())
                        {
                            bounds.Encapsulate(item.bounds);
                        }
                        rootPart.transform.localPosition = new Vector3(0, -bounds.min.y, 0);
                        Selection.SetActiveObjectWithContext(creatureBase, null);
                    }
                }
                GUILayout.EndHorizontal();
            }
            GUILayout.EndScrollView();
        }
    }
}
