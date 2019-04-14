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
    private Vector2 raceScroll;
    private Vector2 unitScroll;
    private List<CreatureRaw> filteredRaws;

    [MenuItem("Window/DF Raw Reader")]
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
    [SerializeField]
    bool filterParts = true;
    private bool showRaces;
    private bool showUnits;
    private List<UnitDefinition> units;

    class ChildCount
    {
        public int min = int.MaxValue;
        public int max = int.MinValue;
    }

    bool FitsFilter(CreatureRaw creatureRaw)
    {
        if (!string.IsNullOrEmpty(filter) && (filterName || filterDescription || filterToken || filterParts))
        {
            bool matched = false;
            if (filterToken && creatureRaw.creature_id.ToUpper().Contains(filter.ToUpper()))
                matched = true;
            if (filterName && creatureRaw.name[0].ToUpper().Contains(filter.ToUpper()))
                matched = true;
            if(!matched)
                foreach (var caste in creatureRaw.caste)
                {
                    if (filterName && caste.caste_name[0].ToUpper().Contains(filter.ToUpper()))
                        matched = true;
                    if (filterDescription && caste.description.ToUpper().Contains(filter.ToUpper()))
                        matched = true;
                    if(filterParts)
                    {
                        foreach (var part in caste.body_parts)
                        {
                            if (part.category.ToUpper().Contains(filter.ToUpper()))
                            {
                                matched = true;
                                break;
                            }
                        }
                    }
                }
            if (!matched)
                return false;
        }
        if (bodyCategoryFilter != CreatureBody.BodyCategory.None)
        {
            foreach (var caste in creatureRaw.caste)
            {
                if (bodyCategoryFilter == CreatureBody.FindBodyCategory(caste))
                    return true;
            }
            return false;
        }
        return true;
    }

    bool FitsFilter(UnitDefinition unit)
    {
        if (!string.IsNullOrEmpty(filter))
        {
            if (filterToken)
                if (!CreatureRaws.Instance[unit.race.mat_type].creature_id.ToUpper().Contains(filter.ToUpper()))
                    return false;
            if (filterName)
                if (!name.ToUpper().Contains(filter.ToUpper()))
                    return false;
            if (filterParts)
            {
                if (unit.inventory.Count == 0)
                    return false;
                foreach (var item in unit.inventory)
                {
                    if (!ItemRaws.Instance.ContainsKey(item.item.type))
                        continue;
                    if (ItemRaws.Instance[item.item.type].id.ToUpper().Contains(filter.ToUpper()))
                        return true;
                }
                return false;
            }
        }
        if (bodyCategoryFilter != CreatureBody.BodyCategory.None)
        {
            foreach (var caste in CreatureRaws.Instance[unit.race.mat_type].caste)
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
            var itemListCall = new RemoteFunction<EmptyMessage, MaterialList>(client, "GetItemList", "RemoteFortressReader");
            var unitListCall = new RemoteFunction<EmptyMessage, UnitList>(client, "GetUnitList", "RemoteFortressReader");
            client.ResumeGame();
            MaterialRaws.Instance.MaterialList = materialListCall.Execute().material_list;
            ItemRaws.Instance.ItemList = itemListCall.Execute().material_list;
            CreatureRaws.Instance.CreatureList = getCreatureRaws.Execute().creature_raws;
            units = unitListCall.Execute().creature_list;
            AssetDatabase.SaveAssets();
            Debug.Log(string.Format("Pulled {0} creature raws from DF.", CreatureRaws.Instance.Count));
            if (MaterialCollection.Instance == null)
                MaterialCollector.BuildMaterialCollection();
            MaterialCollection.Instance.PopulateMatTextures();
            client.Disconnect();
            //foreach (var raw in CreatureRaws.Instance)
            //{
            //    raw.creature_id = BodyDefinition.GetCorrectedCreatureID(raw);
            //}
            RefilterList();
        }
        if (CreatureRaws.Instance.Count == 0)
        {
            if (filteredRaws != null)
                filteredRaws.Clear();
            if (units != null)
                units.Clear();
        }
        if (CreatureRaws.Instance.Count > 0)
        {
            EditorGUI.BeginChangeCheck();
            filter = EditorGUILayout.TextField(filter);
            filterToken = EditorGUILayout.Toggle("Token", filterToken);
            filterName = EditorGUILayout.Toggle("Name", filterName);
            filterDescription = EditorGUILayout.Toggle("Description", filterDescription);
            filterParts = EditorGUILayout.Toggle("Parts", filterParts);

            bodyCategoryFilter = (CreatureBody.BodyCategory)EditorGUILayout.EnumPopup(bodyCategoryFilter);

            if (EditorGUI.EndChangeCheck() || filteredRaws == null)
            {
                RefilterList();
            }
            EditorGUILayout.Space();

            GUILayout.BeginHorizontal();
            //if(GUILayout.Button("Sort by name"))
            //{
            //    CreatureRaws.Instance.Sort((x, y) => x.creature_id.CompareTo(y.creature_id));
            //    RefilterList();
            //}
            //if (GUILayout.Button("Sort by size"))
            //{
            //    CreatureRaws.Instance.Sort((x, y) => x.adultsize.CompareTo(y.adultsize));
            //    RefilterList();
            //}
            //if (GUILayout.Button("Sort by index"))
            //{
            //    CreatureRaws.Instance.Sort((x, y) => x.index.CompareTo(y.index));
            //    RefilterList();
            //}
            GUILayout.EndHorizontal();

            showRaces = EditorGUILayout.Foldout(showRaces, "Races");
            if (showRaces)
            {
                raceScroll = EditorGUILayout.BeginScrollView(raceScroll);
                foreach (var creature in filteredRaws)
                {
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.PrefixLabel(string.Format("{0} ({1})", creature.creature_id, creature.name[0]));
                    EditorGUILayout.BeginVertical();
                    foreach (var caste in creature.caste)
                    {
                        if (GUILayout.Button(string.Format("{0} ({1})", caste.caste_id, caste.caste_name[0])))
                        {
                            AssetDatabase.Refresh();
                            var creatureBase = new GameObject().AddComponent<CreatureBody>();
                            creatureBase.name = caste.caste_name[0];
                            creatureBase.race = creature;
                            creatureBase.caste = caste;
                            creatureBase.MakeBody();
                            creatureBase.transform.localRotation = Quaternion.Euler(0, 180, 0);
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
                if (GUILayout.Button("Place all races"))
                {
                    AssetDatabase.Refresh();
                    var watch = System.Diagnostics.Stopwatch.StartNew();
                    CreatureBody prevCreature = null;
                    foreach (var creature in filteredRaws)
                    {
                        var creatureBase = new GameObject().AddComponent<CreatureBody>();
                        creatureBase.name = creature.caste[0].caste_name[0];
                        creatureBase.race = creature;
                        creatureBase.caste = creature.caste[0];
                        creatureBase.MakeBody();
                        if (prevCreature != null)
                        {
                            creatureBase.transform.position = new Vector3(prevCreature.transform.position.x + prevCreature.bounds.max.x - creatureBase.bounds.min.x, 0, 0);
                        }
                        creatureBase.transform.localRotation = Quaternion.Euler(0, 180, 0);
                        prevCreature = creatureBase;
                    }
                    watch.Stop();
                    Debug.Log(string.Format("Took {0}ms to create {1} creatures, averaging {2}ms per creature.", watch.ElapsedMilliseconds, filteredRaws.Count, (float)watch.ElapsedMilliseconds / filteredRaws.Count));
                }
            }
            showUnits = EditorGUILayout.Foldout(showUnits, "Units");
            if(showUnits)
            {
                unitScroll = EditorGUILayout.BeginScrollView(unitScroll);
                foreach (var unit in units)
                {
                    string name = unit.name;
                    if (string.IsNullOrEmpty(name))
                        name = CreatureRaws.Instance[unit.race.mat_type].caste[unit.race.mat_index].caste_name[0];
                    if (!FitsFilter(unit))
                        continue;
                    if (GUILayout.Button(name))
                    {
                        AssetDatabase.Refresh();
                        var creatureBase = new GameObject().AddComponent<CreatureBody>();
                        creatureBase.name = name;
                        creatureBase.race = CreatureRaws.Instance[unit.race.mat_type];
                        creatureBase.caste = CreatureRaws.Instance[unit.race.mat_type].caste[unit.race.mat_index];
                        creatureBase.unit = unit;
                        creatureBase.MakeBody();
                        creatureBase.UpdateUnit(unit);
                        creatureBase.transform.localRotation = Quaternion.Euler(0, 180, 0);
                        Selection.SetActiveObjectWithContext(creatureBase, null);
                    }
                }
                EditorGUILayout.EndScrollView();
                if (GUILayout.Button("Place all units"))
                {
                    AssetDatabase.Refresh();
                    var watch = System.Diagnostics.Stopwatch.StartNew();
                    CreatureBody prevCreature = null;
                    int creatureCount = 0;
                    foreach (var unit in units)
                    {
                        string name = unit.name;
                        if (string.IsNullOrEmpty(name))
                            name = CreatureRaws.Instance[unit.race.mat_type].caste[unit.race.mat_index].caste_name[0];
                        if (!FitsFilter(unit))
                            continue;
                        var creatureBase = new GameObject().AddComponent<CreatureBody>();
                        creatureBase.name = name;
                        creatureBase.race = CreatureRaws.Instance[unit.race.mat_type];
                        creatureBase.caste = CreatureRaws.Instance[unit.race.mat_type].caste[unit.race.mat_index];
                        creatureBase.unit = unit;
                        creatureBase.MakeBody();
                        creatureBase.UpdateUnit(unit);
                        creatureBase.transform.localRotation = Quaternion.Euler(0,180,0);
                        if (prevCreature != null)
                        {
                            creatureBase.transform.position = new Vector3(prevCreature.transform.position.x + prevCreature.bounds.max.x - creatureBase.bounds.min.x, 0, 0);
                        }
                        prevCreature = creatureBase;
                        creatureCount++;
                    }
                    watch.Stop();
                    Debug.Log(string.Format("Took {0}ms to create {1} creatures, averaging {2}ms per creature.", watch.ElapsedMilliseconds, creatureCount, (float)watch.ElapsedMilliseconds / creatureCount));
                }
            }
        }
    }

    private void RefilterList()
    {
        if (CreatureRaws.Instance == null)
        {
            filteredRaws = null;
            return;
        }
        if (filteredRaws == null)
            filteredRaws = new List<CreatureRaw>();
        filteredRaws.Clear();
        foreach (var creature in CreatureRaws.Instance)
        {
            if (FitsFilter(creature))
                filteredRaws.Add(creature);
        }
    }
}
