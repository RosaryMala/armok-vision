using System;
using System.Collections.Generic;
using System.Xml.Linq;

public class TileDesignationConfiguration<T> : TileConfiguration<T> where T : IContent, new()
{
    enum DesignationTypes
    {
        Hidden
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
        if(tile.hidden)
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
