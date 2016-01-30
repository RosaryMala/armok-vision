using System;
using System.Xml.Linq;
using RemoteFortressReader;

class PlantGrowthConfiguration<T> : TileConfiguration<T> where T : IContent, new()
{
    const int PlantType = 419;
    PlantGrowthMatcher<Content> growthMatcher = new PlantGrowthMatcher<Content>();

    public override object SecondaryDictionary
    {
        set
        {
        }
    }

    public override bool GetValue(MapDataStore.Tile tile, MeshLayer layer, out T value)
    {
        //let's just get rid of dead trees right away
        if(tile.special == TiletypeSpecial.DEAD
            || tile.special == TiletypeSpecial.SMOOTH_DEAD)
        {
            value = default(T);
            return false;
        }
        int growthLayer = int.MaxValue;
        switch (layer)
        {
            case MeshLayer.GrowthMaterial:
            case MeshLayer.GrowthCutout:
            case MeshLayer.GrowthTransparent:
                growthLayer = 0;
                break;
            case MeshLayer.GrowthMaterial1:
            case MeshLayer.GrowthCutout1:
            case MeshLayer.GrowthTransparent1:
                growthLayer = 1;
                break;
            case MeshLayer.GrowthMaterial2:
            case MeshLayer.GrowthCutout2:
            case MeshLayer.GrowthTransparent2:
                growthLayer = 2;
                break;
            case MeshLayer.GrowthMaterial3:
            case MeshLayer.GrowthCutout3:
            case MeshLayer.GrowthTransparent3:
                growthLayer = 3;
                break;
            default:
                break;
        }
        MatPairStruct mat = tile.material;
        int plantIndex = mat.mat_index;
        if ((mat.mat_type != PlantType)
            || DFConnection.Instance.NetPlantRawList == null
            || DFConnection.Instance.NetPlantRawList.plant_raws.Count <= plantIndex
            || DFConnection.Instance.NetPlantRawList.plant_raws[plantIndex].growths.Count <= growthLayer)
        {
            value = default(T);
            return false;
        }
        TreeGrowth growth = DFConnection.Instance.NetPlantRawList.plant_raws[plantIndex].growths[growthLayer];
        int currentTicks = TimeHolder.DisplayedTime.CurrentYearTicks;
        if ((growth.timing_start != -1 && growth.timing_start > currentTicks) || (growth.timing_end != -1 && growth.timing_end < currentTicks))
        {
            value = default(T);
            return false;
        }

        switch (tile.tiletypeMaterial)
        {
            case TiletypeMaterial.PLANT:
                switch (tile.shape)
                {
                    case TiletypeShape.SAPLING:
                        if (!growth.sapling)
                        {
                            value = default(T);
                            return false;
                        }
                        break;
                    case TiletypeShape.SHRUB:
                        //so far as I can understand, this is always on
                        break;
                    default:
                        value = default(T);
                        return false;
                }
                break;
            case TiletypeMaterial.ROOT:
                if (!growth.roots)
                {
                    value = default(T);
                    return false;
                }
                break;
            case TiletypeMaterial.TREE_MATERIAL:
                switch (tile.shape)
                {
                    case TiletypeShape.WALL:
                    case TiletypeShape.RAMP:
                    case TiletypeShape.TRUNK_BRANCH:
                        if (!growth.trunk)
                        {
                            value = default(T);
                            return false;
                        }
                        break;
                    case TiletypeShape.BRANCH:
                        if(tile.special == TiletypeSpecial.SMOOTH)
                        {
                            value = default(T);
                            return false;
                        }
                        if (tile.direction == "--------")
                        {
                            if (!growth.light_branches)
                            {
                                value = default(T);
                                return false;
                            }
                        }
                        else
                        {
                            if (!growth.heavy_branches)
                            {
                                value = default(T);
                                return false;
                            }
                        }
                        break;
                    case TiletypeShape.TWIG:
                        if (!growth.twigs)
                        {
                            value = default(T);
                            return false;
                        }
                        break;
                    default:
                        value = default(T);
                        return false;
                }
                break;
            case TiletypeMaterial.MUSHROOM:
                if(!growth.cap)
                {
                    value = default(T);
                    return false;
                }
                break;
            default:
                value = default(T);
                return false;
        }
        
        if((growth.trunk_height_start != -1 && growth.trunk_height_start > tile.trunkPercent) || (growth.trunk_height_end != -1 && growth.trunk_height_end < tile.trunkPercent))
        {
            value = default(T);
            return false;
        }

        GrowthPrint print = null;
        int printIndex = 0;
        for (int i = 0; i < growth.prints.Count; i++)
        {
            var tempPrint = growth.prints[i];
            if (!((tempPrint.timing_start != -1 && tempPrint.timing_start > currentTicks) || (tempPrint.timing_end != -1 && tempPrint.timing_end < currentTicks)))
            {
                print = tempPrint;
                printIndex = i;
            }
        }
        if (print == null)
        {
            value = default(T);
            return false;
        }
        BuildingStruct growthIndex = new BuildingStruct(plantIndex, growthLayer, printIndex);
        Content cont;

        if (growthMatcher.TryGetValue(growthIndex, out cont))
        {
            value = cont.GetValue(tile, layer);
            return true;
        }
        value = default(T);
        return false;
    }

    protected override void ParseElementConditions(XElement elemtype, Content content)
    {
        var elemGrowths = elemtype.Elements("growth");
        foreach (XElement elemGrowth in elemGrowths)
        {
            XAttribute elemToken = elemGrowth.Attribute("token");
            if (elemToken == null)
            {
                //maybe put an error message here?
                continue;
            }
            growthMatcher[elemToken.Value] = content;
        }
    }
}
