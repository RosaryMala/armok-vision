using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using UnityEngine;

public class CoverageConfiguration<T> : TileConfiguration<T> where T : IContent, new()
{
    Content closed = null;
    Content open = null;

    public override object SecondaryDictionary
    {
        set
        {
            throw new NotImplementedException();
        }
    }

    public override bool GetValue(MapDataStore.Tile tile, MeshLayer layer, out T value)
    {
        if (tile.Up == null)
        {
            if (open != null)
            {
                value = open.GetValue(tile, layer);
                return true;
            }
            else
            {
                value = default(T);
                return false;
            }
        }
        switch (tile.Up.shape)
        {
            case RemoteFortressReader.TiletypeShape.NO_SHAPE:
            case RemoteFortressReader.TiletypeShape.EMPTY:
            case RemoteFortressReader.TiletypeShape.RAMP_TOP:
            case RemoteFortressReader.TiletypeShape.BROOK_TOP:
            case RemoteFortressReader.TiletypeShape.ENDLESS_PIT:
            case RemoteFortressReader.TiletypeShape.BRANCH:
            case RemoteFortressReader.TiletypeShape.TRUNK_BRANCH:
            case RemoteFortressReader.TiletypeShape.TWIG:
                if (open != null)
                {
                    value = open.GetValue(tile, layer);
                    return true;
                }
                else
                {
                    value = default(T);
                    return false;
                }
            default:
                if (closed != null)
                {
                    value = closed.GetValue(tile, layer);
                    return true;
                }
                else
                {
                    value = default(T);
                    return false;
                }
        }
    }

    protected override void ParseElementConditions(XElement elemtype, Content content)
    {
        var elemCoverage = elemtype.Elements("coverage");
        foreach (XElement coverState in elemCoverage)
        {
            XAttribute coverElem = coverState.Attribute("state");
            if(coverElem != null)
            {
                switch (coverElem.Value)
                {
                    case "open":
                        open = content;
                        break;
                    case "covered":
                        closed = content;
                        break;
                    default:
                        continue;
                }
            }
        }
    }
}
