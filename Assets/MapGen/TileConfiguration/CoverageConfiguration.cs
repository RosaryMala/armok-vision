using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using UnityEngine;

public class CoverageConfiguration<T> : TileConfiguration<T> where T : IContent, new()
{
    Content covered = null;
    Content open = null;

    public override object SecondaryDictionary
    {
        set
        {
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
            case RemoteFortressReader.TiletypeShape.NoShape:
            case RemoteFortressReader.TiletypeShape.Empty:
            case RemoteFortressReader.TiletypeShape.RampTop:
            case RemoteFortressReader.TiletypeShape.BrookTop:
            case RemoteFortressReader.TiletypeShape.EndlessPit:
            case RemoteFortressReader.TiletypeShape.Branch:
            case RemoteFortressReader.TiletypeShape.TrunkBranch:
            case RemoteFortressReader.TiletypeShape.Twig:
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
                if (covered != null)
                {
                    value = covered.GetValue(tile, layer);
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
                    case "OPEN":
                        open = content;
                        break;
                    case "COVERED":
                        covered = content;
                        break;
                    default:
                        Debug.LogErrorFormat("{0} is not a valid coverage state. Needs OPEN or COVERED");
                        continue;
                }
            }
        }
    }
}
