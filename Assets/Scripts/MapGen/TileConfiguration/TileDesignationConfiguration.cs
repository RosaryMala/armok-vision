using System;
using System.Collections.Generic;
using System.Xml.Linq;

public class TileDesignationConfiguration<T> : TileConfiguration<T> where T : IContent, new()
{

    Dictionary<DesignationType, Content> designationBlocks = new Dictionary<DesignationType, Content>();

    public override object SecondaryDictionary
    {
        set
        {
        }
    }

    public override bool GetValue(MapDataStore.Tile tile, MeshLayer layer, out T value)
    {
        switch (tile.digDesignation)
        {
            case RemoteFortressReader.TileDigDesignation.NO_DIG:
                break;
            case RemoteFortressReader.TileDigDesignation.DEFAULT_DIG:
                if (designationBlocks.ContainsKey(DesignationType.Default))
                {
                    value = designationBlocks[DesignationType.Default].GetValue(tile, layer);
                    return true;
                }
                break;
            case RemoteFortressReader.TileDigDesignation.UP_DOWN_STAIR_DIG:
                if (designationBlocks.ContainsKey(DesignationType.UpDownStairs))
                {
                    value = designationBlocks[DesignationType.UpDownStairs].GetValue(tile, layer);
                    return true;
                }
                break;
            case RemoteFortressReader.TileDigDesignation.CHANNEL_DIG:
                if (designationBlocks.ContainsKey(DesignationType.Channel))
                {
                    value = designationBlocks[DesignationType.Channel].GetValue(tile, layer);
                    return true;
                }
                break;
            case RemoteFortressReader.TileDigDesignation.RAMP_DIG:
                if (designationBlocks.ContainsKey(DesignationType.Ramp))
                {
                    value = designationBlocks[DesignationType.Ramp].GetValue(tile, layer);
                    return true;
                }
                break;
            case RemoteFortressReader.TileDigDesignation.DOWN_STAIR_DIG:
                if (designationBlocks.ContainsKey(DesignationType.DownStairs))
                {
                    value = designationBlocks[DesignationType.DownStairs].GetValue(tile, layer);
                    return true;
                }
                break;
            case RemoteFortressReader.TileDigDesignation.UP_STAIR_DIG:
                if (designationBlocks.ContainsKey(DesignationType.UpStairs))
                {
                    value = designationBlocks[DesignationType.UpStairs].GetValue(tile, layer);
                    return true;
                }
                break;
            default:
                break;
        }
        if(tile.Hidden)
        {
            if(designationBlocks.ContainsKey(DesignationType.Hidden))
            {
                value = designationBlocks[DesignationType.Hidden].GetValue(tile, layer);
                return true;
            }
        }
        value = default(T);
        return false;
    }

    protected override void ParseElementConditions(XElement elemtype, Content content)
    {
        var elemDesignations = elemtype.Elements("designation");
        foreach(XElement elemDesignation in elemDesignations)
        {
            XAttribute elemToken = elemDesignation.Attribute("type");
            if(elemToken != null)
            {
                DesignationType desig;
                try
                {
                    desig = (DesignationType)Enum.Parse(typeof(DesignationType), elemToken.Value);
                }
                catch(Exception)
                {
                    continue;
                }
                designationBlocks[desig] = content;
            }
        }
    }
}
