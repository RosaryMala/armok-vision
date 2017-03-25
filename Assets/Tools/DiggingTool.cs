using UnityEngine;
using System.Collections;
using RemoteFortressReader;
using DFHack;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using System;

public class DiggingTool : MonoBehaviour
{
    public enum DigMode
    {
        None,
        Dig,
        Channel,
        UpStair,
        DownStair,
        UpDownStair,
        UpRamp,
        RemoveUpStairRamp,
        ChopTrees,
        GatherPlants,
        SmoothStone,
        EngraveStone,
        CarveFortifications,
        RemoveConstruction,
        RemoveDesignation
    }

    public DigMode digMode;

    public void SetDigMode(string mode)
    {
        try
        {
            digMode = (DigMode)Enum.Parse(typeof(DigMode), mode, true);
        }
        catch (Exception)
        {

        }

    }

    public void Apply(List<DFCoord> coordList)
    {
        DigCommand command = new DigCommand();
        TileDigDesignation designation;
        if (!GetDesignation(out designation))
            return;
        command.designation = designation;
        foreach (var item in coordList)
        {
            MapDataStore.Tile tile = MapDataStore.Main[item];
            if (tile == null)
                continue;
            if (DesignationApplies(tile))
                command.locations.Add(item);
        }
        if (command.locations.Count > 0)
            DFConnection.Instance.EnqueueDigCommand(command);

    }

    bool GetDesignation(out TileDigDesignation designation)
    {
        switch (digMode)
        {
            case DigMode.Dig:
                designation = TileDigDesignation.DEFAULT_DIG;
                break;
            case DigMode.Channel:
                designation = TileDigDesignation.CHANNEL_DIG;
                break;
            case DigMode.UpStair:
                designation = TileDigDesignation.UP_STAIR_DIG;
                break;
            case DigMode.DownStair:
                designation = TileDigDesignation.DOWN_STAIR_DIG;
                break;
            case DigMode.UpDownStair:
                designation = TileDigDesignation.UP_DOWN_STAIR_DIG;
                break;
            case DigMode.UpRamp:
                designation = TileDigDesignation.RAMP_DIG;
                break;
            case DigMode.RemoveUpStairRamp:
                designation = TileDigDesignation.DEFAULT_DIG;
                break;
            case DigMode.ChopTrees:
                designation = TileDigDesignation.DEFAULT_DIG;
                break;
            case DigMode.GatherPlants:
                designation = TileDigDesignation.DEFAULT_DIG;
                break;
            //case DigMode.SmoothStone:
            //    command.designation = TileDigDesignation.DEFAULT_DIG;
            //    break;
            //case DigMode.EngraveStone:
            //    command.designation = TileDigDesignation.DEFAULT_DIG;
            //    break;
            //case DigMode.CarveFortifications:
            //    command.designation = TileDigDesignation.DEFAULT_DIG;
            //    break;
            //case DigMode.RemoveConstruction:
            //    command.designation = TileDigDesignation.DEFAULT_DIG;
            //    break;
            case DigMode.RemoveDesignation:
                designation = TileDigDesignation.NO_DIG;
                break;
            default:
                designation = TileDigDesignation.NO_DIG;
                return false;
        }
        return true;
    }

    bool DesignationApplies(MapDataStore.Tile tile)
    {
        switch (digMode)
        {
            case DigMode.None:
                break;
            case DigMode.Dig:
            case DigMode.UpStair:
            case DigMode.UpDownStair:
            case DigMode.UpRamp:
                return tile.Hidden || (tile.isWall && tile.tiletypeMaterial != TiletypeMaterial.TREE_MATERIAL);
            case DigMode.Channel:
            case DigMode.DownStair:
                return tile.Hidden || (tile.tiletypeMaterial != TiletypeMaterial.TREE_MATERIAL);
            case DigMode.RemoveUpStairRamp:
                return tile.shape == TiletypeShape.RAMP || tile.shape == TiletypeShape.STAIR_UP || tile.shape == TiletypeShape.STAIR_UPDOWN;
            case DigMode.ChopTrees:
                return tile.tiletypeMaterial == TiletypeMaterial.TREE_MATERIAL;
            case DigMode.GatherPlants:
                return tile.tiletypeMaterial == TiletypeMaterial.PLANT;
            case DigMode.SmoothStone:
                break;
            case DigMode.EngraveStone:
                break;
            case DigMode.CarveFortifications:
                break;
            case DigMode.RemoveConstruction:
                break;
            case DigMode.RemoveDesignation:
                return true;
            default:
                break;
        }
        return false;
    }
}
