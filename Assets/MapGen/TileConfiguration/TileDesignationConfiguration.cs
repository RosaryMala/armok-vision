using System;
using System.Collections.Generic;
using System.Xml.Linq;

public class TileDesignationConfiguration<T> : TileConfiguration<T> where T : IContent, new()
{
    enum DesignationTypes
    {
        Hidden,
        Default,
        UpDownStairs,
        Channel,
        Ramp,
        DownStairs,
        UpStairs
    }

    Dictionary<DesignationTypes, Content> designationBlocks = new Dictionary<DesignationTypes, Content>();

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
            case RemoteFortressReader.TileDigDesignation.NoDig:
                break;
            case RemoteFortressReader.TileDigDesignation.DefaultDig:
                if (designationBlocks.ContainsKey(DesignationTypes.Default))
                {
                    value = designationBlocks[DesignationTypes.Default].GetValue(tile, layer);
                    return true;
                }
                break;
            case RemoteFortressReader.TileDigDesignation.UpDownStairDig:
                if (designationBlocks.ContainsKey(DesignationTypes.UpDownStairs))
                {
                    value = designationBlocks[DesignationTypes.UpDownStairs].GetValue(tile, layer);
                    return true;
                }
                break;
            case RemoteFortressReader.TileDigDesignation.ChannelDig:
                if (designationBlocks.ContainsKey(DesignationTypes.Channel))
                {
                    value = designationBlocks[DesignationTypes.Channel].GetValue(tile, layer);
                    return true;
                }
                break;
            case RemoteFortressReader.TileDigDesignation.RampDig:
                if (designationBlocks.ContainsKey(DesignationTypes.Ramp))
                {
                    value = designationBlocks[DesignationTypes.Ramp].GetValue(tile, layer);
                    return true;
                }
                break;
            case RemoteFortressReader.TileDigDesignation.DownStairDig:
                if (designationBlocks.ContainsKey(DesignationTypes.DownStairs))
                {
                    value = designationBlocks[DesignationTypes.DownStairs].GetValue(tile, layer);
                    return true;
                }
                break;
            case RemoteFortressReader.TileDigDesignation.UpStairDig:
                if (designationBlocks.ContainsKey(DesignationTypes.UpStairs))
                {
                    value = designationBlocks[DesignationTypes.UpStairs].GetValue(tile, layer);
                    return true;
                }
                break;
            default:
                break;
        }
        if(tile.Hidden)
        {
            if(designationBlocks.ContainsKey(DesignationTypes.Hidden))
            {
                value = designationBlocks[DesignationTypes.Hidden].GetValue(tile, layer);
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
                DesignationTypes desig;
                try
                {
                    desig = (DesignationTypes)Enum.Parse(typeof(DesignationTypes), elemToken.Value);
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
