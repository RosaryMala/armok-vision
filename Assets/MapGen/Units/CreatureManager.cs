using RemoteFortressReader;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using UnitFlags;
using UnityEngine;
using UnityEngine.UI;

public class CreatureManager : MonoBehaviour
{

    Dictionary<int, Transform> creatureList;
    public Transform creatureTemplate;

    public UnitList lastUnitList = null;
    public UnitList unitList = null;

    public bool singleRow;

    MaterialPropertyBlock creatureMaterialProperties = null;
    int layerIndexID;
    int layerColorID;
    public float spacing;

    public static CreatureManager Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
        layerIndexID = Shader.PropertyToID("_LayerIndex");
        layerColorID = Shader.PropertyToID("_LayerColor");
        creatureMaterialProperties = new MaterialPropertyBlock();
    }

    // Update is called once per frame
    void Update()
    {
        if (ContentLoader.Instance == null)
            return;
        UpdateCreatures();
    }

    void UpdateCreatures()
    {
        if (!GameSettings.Instance.units.drawUnits)
            return;
        if (creatureTemplate == null)
            return;
        if (ContentLoader.Instance == null)
            return;

        CultureInfo cultureInfo = Thread.CurrentThread.CurrentCulture;
        TextInfo textInfo = cultureInfo.TextInfo;
        var tempUnitList = DFConnection.Instance.PopUnitListUpdate();
        if (tempUnitList == null)
            return;
        else
            unitList = tempUnitList;
        UnityEngine.Profiling.Profiler.BeginSample("UpdateCreatures", this);
        lastUnitList = unitList;
        int creatureCount = 0;
        foreach (var unit in unitList.creature_list)
        {
            if (creatureList == null)
                creatureList = new Dictionary<int, Transform>();
            UnitFlags1 flags1 = (UnitFlags1)unit.flags1;
            //UnitFlags2 flags2 = (UnitFlags2)unit.flags2;
            //UnitFlags3 flags3 = (UnitFlags3)unit.flags3;
            if (((flags1 & UnitFlags1.dead) == UnitFlags1.dead)
                 || ((flags1 & UnitFlags1.left) == UnitFlags1.left)
                 || ((flags1 & UnitFlags1.caged) == UnitFlags1.caged)
                 || ((flags1 & UnitFlags1.forest) == UnitFlags1.forest)
                 )
            {
                if (creatureList.ContainsKey(unit.id))
                {
                    Destroy(creatureList[unit.id].gameObject);
                    creatureList.Remove(unit.id);
                }
            }
            else
            {
                CreatureRaw creatureRaw = null;
                if (DFConnection.Instance.CreatureRaws != null)
                    creatureRaw = DFConnection.Instance.CreatureRaws[unit.race.mat_type];

                if (!creatureList.ContainsKey(unit.id))
                {
                    creatureList[unit.id] = Instantiate(creatureTemplate);
                    creatureList[unit.id].transform.parent = gameObject.transform;
                    creatureList[unit.id].name = "Unit_" + unit.id;
                    creatureList[unit.id].GetComponent<Creature>().creatureRaw = creatureRaw;
                    creatureList[unit.id].GetComponent<Creature>().casteRaw = creatureRaw.caste[unit.race.mat_index];
                    creatureList[unit.id].GetComponent<Creature>().unitDef = unit;


                    Color color = Color.white;
                    if (unit.profession_color != null)
                        color = new Color(unit.profession_color.red / 255.0f, unit.profession_color.green / 255.0f, unit.profession_color.blue / 255.0f, 1);

                    if (creatureRaw != null)
                    {
                        creatureMaterialProperties.SetFloat(layerIndexID, creatureRaw.creature_tile);
                        creatureMaterialProperties.SetColor(layerColorID, new Color(unit.profession_color.red / 255.0f, unit.profession_color.green / 255.0f, unit.profession_color.blue / 255.0f, 0.5f));
                        creatureList[unit.id].GetComponentInChildren<MeshRenderer>().SetPropertyBlock(creatureMaterialProperties);
                    }

                }
                if (!singleRow)
                {
                    MapDataStore.Tile tile = null;
                    if (MapDataStore.Main != null)
                        tile = MapDataStore.Main[unit.pos_x, unit.pos_y, unit.pos_z];
                    creatureList[unit.id].gameObject.SetActive(
                        unit.pos_z < GameMap.Instance.PosZ && unit.pos_z >= (GameMap.Instance.PosZ - GameSettings.Instance.rendering.drawRangeDown)
                        && (tile != null ? !tile.Hidden : false)
                        );
                }

                if (creatureList[unit.id].gameObject.activeSelf) //Only update stuff if it's actually visible.
                {
                    if (singleRow)
                    {
                        creatureList[unit.id].transform.position = new Vector3(creatureCount * spacing, 0, 0);
                    }
                    else
                    {
                        Vector3 position = GameMap.DFtoUnityCoord(unit.pos_x, unit.pos_y, unit.pos_z);
                        creatureList[unit.id].transform.position = position + new Vector3(0, 0.51f, 0);
                    }
                    if (unit.rider_id >= 0 && creatureList.ContainsKey(unit.rider_id))
                    {
                        creatureList[unit.id].transform.position += new Vector3(0, creatureList[unit.rider_id].localScale.y, 0);
                    }
                    float scale;
                    if (GameSettings.Instance.units.scaleUnits)
                        scale = unit.size_info.length_cur / 391.0f;
                    else
                        scale = 1;
                    creatureList[unit.id].transform.localScale = new Vector3(scale, scale, scale);
                    CameraFacing cameraFacing = creatureList[unit.id].GetComponentInChildren<CameraFacing>();
                    if ((flags1 & UnitFlags1.on_ground) == UnitFlags1.on_ground)
                    {
                        cameraFacing.transform.localPosition = Vector3.zero;
                        cameraFacing.enabled = false;
                        cameraFacing.transform.rotation = Quaternion.Euler(90, 0, 0);
                    }
                    else
                    {
                        cameraFacing.transform.localPosition = new Vector3(0, 1.0f, 0);
                        cameraFacing.enabled = true;
                    }
                    Material mat;
                    int index;
                    bool colored;
                    if (ContentLoader.Instance.SpriteManager.getCreatureSprite(unit, out mat, out index, out colored))
                    {
                        creatureList[unit.id].GetComponentInChildren<MeshRenderer>().material = mat;
                        creatureMaterialProperties.SetFloat(layerIndexID, index);
                    }
                    else
                    {
                        creatureList[unit.id].GetComponentInChildren<MeshRenderer>().material = creatureTemplate.GetComponentInChildren<MeshRenderer>().sharedMaterial;
                        if (creatureRaw != null)
                        {
                            if (unit.is_soldier && creatureRaw.creature_soldier_tile != 0)
                            {
                                creatureMaterialProperties.SetFloat(layerIndexID, creatureRaw.creature_soldier_tile);
                            }
                            else
                            {
                                creatureMaterialProperties.SetFloat(layerIndexID, creatureRaw.creature_tile);
                            }
                        }
                    }
                    if (colored && unit.profession_color != null)
                        creatureMaterialProperties.SetColor(layerColorID, new Color(unit.profession_color.red / 255.0f, unit.profession_color.green / 255.0f, unit.profession_color.blue / 255.0f, 0.5f));
                    else
                        creatureMaterialProperties.SetColor(layerColorID, Color.white);


                    creatureList[unit.id].GetComponentInChildren<MeshRenderer>().SetPropertyBlock(creatureMaterialProperties);

                    if (creatureRaw != null)
                    {
                        Text unitText = creatureList[unit.id].GetComponentInChildren<Text>();
                        if (unitText != null)
                        {
                            if (unit.name == "")
                                unitText.text = textInfo.ToTitleCase(creatureRaw.caste[unit.race.mat_index].caste_name[0]);
                            else
                            {
                                unitText.text = unit.name;
                            }
                        }
                    }
                }
                creatureCount++;
            }
        }
        UnityEngine.Profiling.Profiler.EndSample();
    }

    internal void Clear()
    {
        if (creatureList != null)
        {
            foreach (var item in creatureList)
            {
                Destroy(item.Value.gameObject);
            }
            creatureList.Clear();
        }
    }
}
