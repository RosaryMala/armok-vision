using RemoteFortressReader;
using System;
using System.Collections.Generic;
using UnitFlags;
using UnityEngine;

public class CreatureManager : MonoBehaviour
{

    Dictionary<int, Creature> creatureList = new Dictionary<int, Creature>();
    Dictionary<int, CreatureBody> creatureList3D = new Dictionary<int, CreatureBody>();
    public Creature creatureTemplate;

    public List<UnitDefinition> Units { get; set; }

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

    bool ShouldRender(int x, int y, int z, MapDataStore.Tile tile)
    {
        if (z >= (GameMap.Instance.firstPerson ? GameMap.Instance.PosZ + GameSettings.Instance.rendering.drawRangeUp : GameMap.Instance.PosZ))
            return false;
        if (z < GameMap.Instance.PosZ - GameSettings.Instance.rendering.drawRangeDown)
            return false;
        if (x / GameMap.blockSize < (GameMap.Instance.PosXBlock - GameSettings.Instance.rendering.drawRangeSide))
            return false;
        if (x / GameMap.blockSize > (GameMap.Instance.PosXBlock + GameSettings.Instance.rendering.drawRangeSide))
            return false;
        if (y / GameMap.blockSize < (GameMap.Instance.PosYBlock - GameSettings.Instance.rendering.drawRangeSide))
            return false;
        if (y / GameMap.blockSize > (GameMap.Instance.PosYBlock + GameSettings.Instance.rendering.drawRangeSide))
            return false;
        if (tile != null && tile.Hidden)
            return false;
        if ((Camera.main.transform.position - GameMap.DFtoUnityCoord(x, y, z)).sqrMagnitude > GameSettings.Instance.rendering.creatureDrawDistance * GameSettings.Instance.rendering.creatureDrawDistance)
            return false;
        return true;
    }

    int updateIndex = 0;

    void UpdateCreatures()
    {
        if (!GameSettings.Instance.units.drawUnits)
            return;
        if (creatureTemplate == null)
            return;
        if (ContentLoader.Instance == null)
            return;

        var tempUnitList = DFConnection.Instance.PopUnitListUpdate();
        if (tempUnitList != null)
        {
            Units = tempUnitList.creature_list;
        }
        UnityEngine.Profiling.Profiler.BeginSample("UpdateCreatures", this);
        int creatureCount = 0;
        if (updateIndex >= Units.Count)
            updateIndex = 0;
        var watch = System.Diagnostics.Stopwatch.StartNew();
        for(;updateIndex < Units.Count; updateIndex++)
        {
            if (GameSettings.Instance.units.spriteUnits)
                UpdateItem(Units[updateIndex], ref creatureCount);
            else
                Update3DUnit(Units[updateIndex], ref creatureCount);
            if (watch.ElapsedMilliseconds > 2)
                break;
        }
        UnityEngine.Profiling.Profiler.EndSample();
    }

    private void Update3DUnit(UnitDefinition unit, ref int creatureCount)
    {
        UnitFlags1 flags1 = (UnitFlags1)unit.flags1;
        //UnitFlags2 flags2 = (UnitFlags2)unit.flags2;
        //UnitFlags3 flags3 = (UnitFlags3)unit.flags3;
        if (((flags1 & UnitFlags1.dead) == UnitFlags1.dead)
             || ((flags1 & UnitFlags1.left) == UnitFlags1.left)
             || ((flags1 & UnitFlags1.caged) == UnitFlags1.caged)
             || ((flags1 & UnitFlags1.forest) == UnitFlags1.forest)
             )
        {
            if (creatureList3D.ContainsKey(unit.id))
            {
                Destroy(creatureList3D[unit.id].gameObject);
                creatureList3D.Remove(unit.id);
            }
            return;
        }
        MapDataStore.Tile tile = null;
        if (MapDataStore.Main != null)
            tile = MapDataStore.Main[unit.pos_x, unit.pos_y, unit.pos_z];

        if (!ShouldRender(unit.pos_x, unit.pos_y, unit.pos_z, tile) && !singleRow)
        {
            if (creatureList3D.ContainsKey(unit.id))
                creatureList3D[unit.id].gameObject.SetActive(false);
            return;
        }
        else if (creatureList3D.ContainsKey(unit.id))
            creatureList3D[unit.id].gameObject.SetActive(true);

        if (!creatureList3D.ContainsKey(unit.id))
        {
            var creatureBase = new GameObject().AddComponent<CreatureBody>();
            creatureBase.race = DFConnection.Instance.CreatureRaws[unit.race.mat_type];
            creatureBase.caste = creatureBase.race.caste[unit.race.mat_index];
            creatureBase.unit = unit;
            creatureBase.MakeBody();
            if (string.IsNullOrEmpty(unit.name))
                creatureBase.name = creatureBase.race.name[0];
            creatureBase.name = unit.name;
            creatureList3D[unit.id] = creatureBase;
            creatureBase.transform.parent = transform;
        }
        var placedUnit = creatureList3D[unit.id];
        if (!placedUnit.gameObject.activeSelf)
            return;
        placedUnit.UpdateUnit(unit);
        if (unit.rider_id >= 0 && creatureList3D.ContainsKey(unit.rider_id))
        {
            placedUnit.transform.parent = creatureList3D[unit.rider_id].transform;
            if (creatureList3D[unit.rider_id].riderPosition == null)
            {
                placedUnit.transform.position = creatureList3D[unit.rider_id].rootPart.transform.position;
                placedUnit.transform.rotation = creatureList3D[unit.rider_id].rootPart.transform.rotation;
            }
            else
            {
                placedUnit.transform.position = creatureList3D[unit.rider_id].riderPosition.transform.position;
                placedUnit.transform.rotation = creatureList3D[unit.rider_id].riderPosition.transform.rotation;
            }
        }
        else
        {
            placedUnit.transform.parent = transform;
            placedUnit.transform.position = GameMap.DFtoUnityCoord(unit.pos_x + unit.subpos_x, unit.pos_y + unit.subpos_y, unit.pos_z + unit.subpos_z) + new Vector3(0, GameMap.floorHeight, 0);
        }
    }

    void UpdateItem(UnitDefinition unit, ref int creatureCount)
    {
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
            MapDataStore.Tile tile = null;
            if (MapDataStore.Main != null)
                tile = MapDataStore.Main[unit.pos_x, unit.pos_y, unit.pos_z];

            if (!ShouldRender(unit.pos_x, unit.pos_y, unit.pos_z, tile) && !singleRow)
            {
                if (creatureList.ContainsKey(unit.id))
                    creatureList[unit.id].gameObject.SetActive(false);
                return;
            }
            else if (creatureList.ContainsKey(unit.id))
                creatureList[unit.id].gameObject.SetActive(true);


            if (!creatureList.ContainsKey(unit.id))
            {
                creatureList[unit.id] = Instantiate(creatureTemplate, gameObject.transform);
                creatureList[unit.id].name = "Unit_" + unit.id;
                creatureList[unit.id].transform.position = new Vector3(-3000, -3000, -3000);
            }
            if (creatureList[unit.id].gameObject.activeSelf) //Only update stuff if it's actually visible.
            {
                if (singleRow)
                {
                    creatureList[unit.id].transform.position = new Vector3(creatureCount * spacing, 0, 0);
                }
                else
                {
                    var position = GameMap.DFtoUnityCoord(unit.pos_x + unit.subpos_x, unit.pos_y + unit.subpos_y, unit.pos_z + unit.subpos_z);
                    //RaycastHit hitInfo;
                    //if (((flags1 & UnitFlags1.projectile) != UnitFlags1.projectile) && Physics.Raycast(position + new Vector3(0, 2.9f, 0), Vector3.down, out hitInfo, 3, Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore))
                    //    creatureList[unit.id].TargetPos = hitInfo.point;
                    //else
                    creatureList[unit.id].TargetPos = position + new Vector3(0, GameMap.floorHeight, 0);
                }
                if (unit.rider_id >= 0 && creatureList.ContainsKey(unit.rider_id))
                {
                    creatureList[unit.id].TargetPos += new Vector3(0, creatureList[unit.rider_id].transform.localScale.y, 0);
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

                creatureList[unit.id].UpdateCreature(unit);
            }
            creatureCount++;
        }
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
        foreach (var item in creatureList3D)
        {
            Destroy(item.Value.gameObject);
        }
        creatureList3D.Clear();
    }
}
