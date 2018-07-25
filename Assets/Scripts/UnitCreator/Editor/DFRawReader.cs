using System.Collections.Generic;
using System.IO;
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

    string filter;

    class ChildCount
    {
        public int min = int.MaxValue;
        public int max = int.MinValue;
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
            if(GUILayout.Button("Dump Part Categories"))
            {
                var path = EditorUtility.SaveFilePanel("Save bodypart list", "", "Bodyparts.csv", "csv");
                Dictionary<string, Dictionary<string, ChildCount>> parts = new Dictionary<string, Dictionary<string, ChildCount>>();
                foreach (var creature in creatureRaws)
                {
                    foreach (var caste in creature.caste)
                    {
                        int numLegs = 0;
                        bool hasArms = false;
                        foreach (var part in caste.body_parts)
                        {
                            if (part.flags[(int)BodyPartFlags.BodyPartRawFlags.STANCE])
                                numLegs++;
                            if (part.category == "ARM_UPPER" || part.category == "ARM")
                                hasArms = true;
                        }
                        //Temp thing to only count humanoids.
                        if (numLegs != 2 && !hasArms)
                            continue;

                        Dictionary<string, Dictionary<string, int>> tempCount = new Dictionary<string, Dictionary<string, int>>();

                        foreach (var part in caste.body_parts)
                        {
                            //this is an internal part, and doesn't need modeling.
                            if (part.flags[(int)BodyPartFlags.BodyPartRawFlags.INTERNAL])
                                continue;
                            if (!tempCount.ContainsKey(part.category))
                                tempCount[part.category] = new Dictionary<string, int>();

                            if (part.parent >= 0)
                            {
                                if (!tempCount.ContainsKey(caste.body_parts[part.parent].category))
                                    tempCount[caste.body_parts[part.parent].category] = new Dictionary<string, int>();

                                if (!tempCount[caste.body_parts[part.parent].category].ContainsKey(part.category))
                                    tempCount[caste.body_parts[part.parent].category][part.category] = 1;
                                else
                                    tempCount[caste.body_parts[part.parent].category][part.category]++;
                            }
                        }
                        foreach (var item in tempCount)
                        {
                            if (!parts.ContainsKey(item.Key))
                                parts[item.Key] = new Dictionary<string, ChildCount>();
                            foreach (var childs in item.Value)
                            {
                                if (!parts[item.Key].ContainsKey(childs.Key))
                                    parts[item.Key][childs.Key] = new ChildCount();
                                if (parts[item.Key][childs.Key].min > childs.Value)
                                    parts[item.Key][childs.Key].min = childs.Value;
                                if (parts[item.Key][childs.Key].max < childs.Value)
                                    parts[item.Key][childs.Key].max = childs.Value;
                            }
                        }
                    }
                }
                using (var writer = new StreamWriter(path))
                {
                    foreach (var parent in parts)
                    {
                        writer.Write("\"" + parent.Key + "\",");
                        foreach (var child in parent.Value)
                        {
                            writer.Write(string.Format("\"{0}\",{1},{2},", child.Key, child.Value.min, child.Value.max));
                        }
                        writer.WriteLine();
                    }
                }
            }

            filter = EditorGUILayout.TextField(filter);

            GUILayout.BeginHorizontal();
            if(GUILayout.Button("Sort by name"))
            {
                creatureRaws.Sort((x, y) => x.creature_id.CompareTo(y.creature_id));
            }
            if(GUILayout.Button("Sort by size"))
            {
                creatureRaws.Sort((x, y) => x.adultsize.CompareTo(y.adultsize));
            }
            GUILayout.EndHorizontal();

            scrollPosition = GUILayout.BeginScrollView(scrollPosition);
            foreach (var creature in creatureRaws)
            {
                if (!string.IsNullOrEmpty(filter) && !creature.creature_id.ToUpper().Contains(filter.ToUpper()))
                    continue;
                GUILayout.BeginHorizontal();
                GUILayout.Label(creature.creature_id);

                foreach (var caste in creature.caste)
                {
                    if (GUILayout.Button(caste.caste_id))
                    {
                        var creatureBase = new GameObject().AddComponent<CreatureBody>();
                        creatureBase.name = creature.creature_id + "_" + caste.caste_id;
                        creatureBase.race = creature;
                        creatureBase.caste = caste;
                        creatureBase.MakeBody();
                        Selection.SetActiveObjectWithContext(creatureBase, null);
                    }
                }
                GUILayout.EndHorizontal();
            }
            GUILayout.EndScrollView();
            if(GUILayout.Button("Place all creatures"))
            {
                var watch = System.Diagnostics.Stopwatch.StartNew();
                CreatureBody prevCreature = null;
                foreach (var creature in creatureRaws)
                {
                    if (!string.IsNullOrEmpty(filter) && !creature.creature_id.ToUpper().Contains(filter.ToUpper()))
                        continue;
                    var creatureBase = new GameObject().AddComponent<CreatureBody>();
                    creatureBase.name = creature.creature_id + "_" + creature.caste[0].caste_id;
                    creatureBase.race = creature;
                    creatureBase.caste = creature.caste[0];
                    creatureBase.MakeBody();
                    creatureBase.transform.rotation = Quaternion.LookRotation(Vector3.back);
                    if(prevCreature != null)
                    {
                        creatureBase.transform.position = new Vector3(prevCreature.transform.position.x + prevCreature.bounds.max.x - creatureBase.bounds.min.x, 0, 0);
                    }
                    prevCreature = creatureBase;
                }
                watch.Stop();
                Debug.Log(string.Format("Took {0}ms to create {1} creatures, averaging {2}ms per creature.", watch.ElapsedMilliseconds, creatureRaws.Count, (float)watch.ElapsedMilliseconds / creatureRaws.Count));
            }
        }
    }
}
