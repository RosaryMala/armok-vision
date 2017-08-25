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

    public Material baseTileMaterial;

    public bool singleRow;

    public float spacing;

    public static CreatureManager Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
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
                if (!creatureList.ContainsKey(unit.id))
                {
                    creatureList[unit.id] = Instantiate(creatureTemplate, gameObject.transform);
                    creatureList[unit.id].name = "Unit_" + unit.id;

                    var creature = creatureList[unit.id].GetComponent<Creature>();

                    creature.UpdateCreature(unit);

                    Color color = Color.white;

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
                        RaycastHit hitInfo;
                        if (Physics.Raycast(position + new Vector3(0, 2.9f, 0), Vector3.down, out hitInfo, 3, Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore))
                            creatureList[unit.id].transform.position = hitInfo.point;
                        else
                            creatureList[unit.id].transform.position = position;
                    }
                    if (unit.rider_id >= 0 && creatureList.ContainsKey(unit.rider_id))
                    {
                        creatureList[unit.id].transform.position += new Vector3(0, creatureList[unit.rider_id].localScale.y, 0);
                    }
                    UnitScaler unitScaler = creatureList[unit.id].GetComponentInChildren<UnitScaler>();
                    if (unitScaler != null)
                        unitScaler.UpdateSize(unit, creatureList[unit.id].GetComponentInChildren<LayeredSprite>());

                    CameraFacing cameraFacing = creatureList[unit.id].GetComponentInChildren<CameraFacing>();
                    if ((flags1 & UnitFlags1.on_ground) == UnitFlags1.on_ground)
                    {
                        cameraFacing.transform.localPosition = Vector3.zero;
                        cameraFacing.enabled = false;
                        cameraFacing.transform.rotation = Quaternion.Euler(90, 0, 0);
                    }
                    else
                    {
                        cameraFacing.transform.localPosition = new Vector3(0, 1, 0);
                        cameraFacing.enabled = true;
                    }

                    creatureList[unit.id].GetComponent<Creature>().UpdateCreature(unit);
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
