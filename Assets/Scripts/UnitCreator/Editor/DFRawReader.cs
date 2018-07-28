using System;
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
    [SerializeField]
    private List<CreatureRaw> filteredRaws;

    [MenuItem("Mytools/DF Raw Reader")]
    public static void ShowWindow()
    {
        GetWindow<DFRawReader>();
    }

    [SerializeField]
    string filter;

    [SerializeField]
    CreatureBody.BodyCategory bodyCategoryFilter;

    class ChildCount
    {
        public int min = int.MaxValue;
        public int max = int.MinValue;
    }

    bool FitsFilter(CreatureRaw creature)
    {
        if (!string.IsNullOrEmpty(filter))
        {
            bool matched = false;
            if (creature.creature_id.ToUpper().Contains(filter.ToUpper()))
                matched = true;
            if (creature.name[0].ToUpper().Contains(filter.ToUpper()))
                matched = true;
            if(!matched)
                foreach (var caste in creature.caste)
                {
                    if (caste.caste_name[0].ToUpper().Contains(filter.ToUpper()))
                        matched = true;
                    if (caste.description.ToUpper().Contains(filter.ToUpper()))
                        matched = true;
                }
            if (!matched)
                return false;
        }
        if (bodyCategoryFilter != CreatureBody.BodyCategory.None)
        {
            foreach (var caste in creature.caste)
            {
                if (bodyCategoryFilter == CreatureBody.FindBodyCategory(caste))
                    return true;
            }
            return false;
        }
        return true;
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
            RefilterList();
        }
        if (creatureRaws != null)
        {
            EditorGUI.BeginChangeCheck();
            filter = EditorGUILayout.TextField(filter);

            bodyCategoryFilter = (CreatureBody.BodyCategory)EditorGUILayout.EnumPopup(bodyCategoryFilter);
            if(EditorGUI.EndChangeCheck())
            {
                RefilterList();
            }

            if (GUILayout.Button("Dump Part Categories"))
            {
                var path = EditorUtility.SaveFilePanel("Save bodypart list", "", "Bodyparts.csv", "csv");
                Dictionary<string, Dictionary<string, ChildCount>> parts = new Dictionary<string, Dictionary<string, ChildCount>>();
                foreach (var creature in filteredRaws)
                {
                    foreach (var caste in creature.caste)
                    {
                        if (bodyCategoryFilter != CreatureBody.BodyCategory.None && bodyCategoryFilter != CreatureBody.FindBodyCategory(caste))
                            continue;

                        for(int i = 0; i < caste.body_parts.Count; i++)
                        {
                            var part = caste.body_parts[i];
                            //this is an internal part, and doesn't need modeling.
                            if (part.flags[(int)BodyPartFlags.BodyPartRawFlags.INTERNAL])
                                continue;
                            if (!parts.ContainsKey(part.category))
                                parts[part.category] = new Dictionary<string, ChildCount>();

                            Dictionary<string, int> childCounts = new Dictionary<string, int>();

                            foreach (var sub in caste.body_parts)
                            {
                                if (sub.parent != i)
                                    continue;
                                if (sub.flags[(int)BodyPartFlags.BodyPartRawFlags.INTERNAL])
                                    continue;
                                if (!childCounts.ContainsKey(sub.category))
                                    childCounts[sub.category] = 1;
                                else
                                    childCounts[sub.category]++;
                            }

                            foreach (var item in childCounts)
                            {
                                if (!parts[part.category].ContainsKey(item.Key))
                                    parts[part.category][item.Key] = new ChildCount();
                                if (parts[part.category][item.Key].min > item.Value)
                                    parts[part.category][item.Key].min = item.Value;
                                if (parts[part.category][item.Key].max < item.Value)
                                    parts[part.category][item.Key].max = item.Value;
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

            GUILayout.BeginHorizontal();
            if(GUILayout.Button("Sort by name"))
            {
                creatureRaws.Sort((x, y) => x.creature_id.CompareTo(y.creature_id));
                RefilterList();
            }
            if (GUILayout.Button("Sort by size"))
            {
                creatureRaws.Sort((x, y) => x.adultsize.CompareTo(y.adultsize));
                RefilterList();
            }
            if (GUILayout.Button("Sort by index"))
            {
                creatureRaws.Sort((x, y) => x.index.CompareTo(y.index));
                RefilterList();
            }
            GUILayout.EndHorizontal();

            scrollPosition = GUILayout.BeginScrollView(scrollPosition);
            foreach (var creature in filteredRaws)
            {
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
            if (GUILayout.Button("Place all creatures"))
            {
                var watch = System.Diagnostics.Stopwatch.StartNew();
                CreatureBody prevCreature = null;
                foreach (var creature in filteredRaws)
                {
                    var creatureBase = new GameObject().AddComponent<CreatureBody>();
                    creatureBase.name = creature.creature_id + "_" + creature.caste[0].caste_id;
                    creatureBase.race = creature;
                    creatureBase.caste = creature.caste[0];
                    creatureBase.MakeBody();
                    if(prevCreature != null)
                    {
                        creatureBase.transform.position = new Vector3(prevCreature.transform.position.x + prevCreature.bounds.max.x - creatureBase.bounds.min.x, 0, 0);
                    }
                    prevCreature = creatureBase;
                }
                watch.Stop();
                Debug.Log(string.Format("Took {0}ms to create {1} creatures, averaging {2}ms per creature.", watch.ElapsedMilliseconds, filteredRaws.Count, (float)watch.ElapsedMilliseconds / filteredRaws.Count));
            }
        }
    }

    private void RefilterList()
    {
        if (creatureRaws == null)
        {
            filteredRaws = null;
            return;
        }
        if (filteredRaws == null)
            filteredRaws = new List<CreatureRaw>();
        filteredRaws.Clear();
        foreach (var creature in creatureRaws)
        {
            if (FitsFilter(creature))
                filteredRaws.Add(creature);
        }
    }
}
