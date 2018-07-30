using System;
using System.Collections.Generic;
using System.IO;
using DFHack;
using dfproto;
using MaterialStore;
using RemoteFortressReader;
using TokenLists;
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

    [SerializeField]
    bool filterName = true;
    [SerializeField]
    bool filterToken = true;
    [SerializeField]
    bool filterDescription = true;

    class ChildCount
    {
        public int min = int.MaxValue;
        public int max = int.MinValue;
    }

    bool FitsFilter(CreatureRaw creature)
    {
        if (!string.IsNullOrEmpty(filter) && (filterName || filterDescription || filterToken))
        {
            bool matched = false;
            if (filterToken && creature.creature_id.ToUpper().Contains(filter.ToUpper()))
                matched = true;
            if (filterName && creature.name[0].ToUpper().Contains(filter.ToUpper()))
                matched = true;
            if(!matched)
                foreach (var caste in creature.caste)
                {
                    if (filterName && caste.caste_name[0].ToUpper().Contains(filter.ToUpper()))
                        matched = true;
                    if (filterDescription && caste.description.ToUpper().Contains(filter.ToUpper()))
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
            client.SuspendGame();
            var getCreatureRaws = new RemoteFunction<EmptyMessage, CreatureRawList>(client, "GetCreatureRaws", "RemoteFortressReader");
            var materialListCall = new RemoteFunction<EmptyMessage, MaterialList>(client, "GetMaterialList", "RemoteFortressReader");
            client.ResumeGame();
            creatureRaws = getCreatureRaws.Execute().creature_raws;
            var ExistingMatList = AssetDatabase.LoadAssetAtPath<MaterialRaws>("Assets/Resources/MaterialRaws.asset");
            MaterialRaws.Instance.MaterialList = materialListCall.Execute().material_list;
            if(ExistingMatList == null)
                AssetDatabase.CreateAsset(MaterialRaws.Instance, "Assets/Resources/MaterialRaws.asset");
            AssetDatabase.SaveAssets();
            Debug.Log(string.Format("Pulled {0} creature raws from DF.", creatureRaws.Count));
            MaterialCollection.Instance.PopulateMatTextures();
            client.Disconnect();
            //foreach (var raw in creatureRaws)
            //{
            //    raw.creature_id = BodyDefinition.GetCorrectedCreatureID(raw);
            //}
            RefilterList();
        }
        if (creatureRaws != null)
        {
            EditorGUI.BeginChangeCheck();
            filter = EditorGUILayout.TextField(filter);
            filterToken = EditorGUILayout.Toggle("Token", filterToken);
            filterName = EditorGUILayout.Toggle("Name", filterName);
            filterDescription = EditorGUILayout.Toggle("Description", filterDescription);

            bodyCategoryFilter = (CreatureBody.BodyCategory)EditorGUILayout.EnumPopup(bodyCategoryFilter);

            if (EditorGUI.EndChangeCheck())
            {
                RefilterList();
            }
            EditorGUILayout.Space();

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

            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
            foreach (var creature in filteredRaws)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.PrefixLabel(string.Format("{0} ({1})", creature.creature_id, creature.name[0]));
                EditorGUILayout.BeginVertical();
                foreach (var caste in creature.caste)
                {
                    if (GUILayout.Button(string.Format("{0} ({1})", caste.caste_id, caste.caste_name[0])))
                    {
                        var creatureBase = new GameObject().AddComponent<CreatureBody>();
                        creatureBase.name = caste.caste_name[0];
                        creatureBase.race = creature;
                        creatureBase.caste = caste;
                        creatureBase.MakeBody();
                        Selection.SetActiveObjectWithContext(creatureBase, null);
                    }
                }
                EditorGUILayout.EndVertical();
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.EndScrollView();
            if (GUILayout.Button("Dump Part Categories"))
            {
                var path = EditorUtility.SaveFilePanel("Save bodypart list", "", "Bodyparts.csv", "csv");
                if (!string.IsNullOrEmpty(path))
                {
                    Dictionary<string, Dictionary<string, ChildCount>> parts = new Dictionary<string, Dictionary<string, ChildCount>>();
                    foreach (var creature in filteredRaws)
                    {
                        foreach (var caste in creature.caste)
                        {
                            if (bodyCategoryFilter != CreatureBody.BodyCategory.None && bodyCategoryFilter != CreatureBody.FindBodyCategory(caste))
                                continue;

                            for (int i = 0; i < caste.body_parts.Count; i++)
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
            }
            if (GUILayout.Button("Place all creatures"))
            {
                var watch = System.Diagnostics.Stopwatch.StartNew();
                CreatureBody prevCreature = null;
                foreach (var creature in filteredRaws)
                {
                    var creatureBase = new GameObject().AddComponent<CreatureBody>();
                    creatureBase.name = creature.caste[0].caste_name[0];
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
