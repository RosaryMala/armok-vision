using System;
using System.Collections.Generic;
using System.Xml.Linq;
using DFHack;

public class BuildingPosConfiguration<T> : TileConfiguration<T> where T : IContent, new()
{
    Dictionary<DFCoord2d, Content> coordList = new Dictionary<DFCoord2d, Content>();

    public override object SecondaryDictionary
    {
        set
        {
        }
    }

    public override bool GetValue(MapDataStore.Tile tile, MeshLayer layer, out T value)
    {

        if (coordList.ContainsKey(tile.buildingLocalPos))
        {
            value = coordList[tile.buildingLocalPos].GetValue(tile, layer);
            return true;
        }
        DFCoord2d any = new DFCoord2d(-1, -1);
        if (coordList.ContainsKey(any))
        {
            value = coordList[any].GetValue(tile, layer);
            return true;
        }
        value = default(T);
        return false;

    }

    protected override void ParseElementConditions(XElement elemtype, Content content)
    {
        var elemBuildingPositions = elemtype.Elements("buildingPosition");
        foreach (XElement elemBuildingPosition in elemBuildingPositions)
        {
            int x = -1;
            int y = -1;
            XAttribute elemX = elemBuildingPosition.Attribute("x");
            if (elemX != null)
            {
                if (!int.TryParse(elemX.Value, out x))
                    continue;
            }
            else continue;
            XAttribute elemY = elemBuildingPosition.Attribute("y");
            if (elemY != null)
            {
                if (!int.TryParse(elemY.Value, out y))
                    continue;
            }
            else continue;

            coordList[new DFCoord2d(x, y)] = content;
        }
    }
}
