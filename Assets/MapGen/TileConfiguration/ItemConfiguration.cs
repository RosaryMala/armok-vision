using System;
using System.Xml.Linq;

public class ItemConfiguration<T> : TileConfiguration<T> where T : IContent, new()
{
    ItemMatcher<Content> itemMatcher = new ItemMatcher<Content>();

    public override object SecondaryDictionary
    {
        set
        {
        }
    }

    public override bool GetValue(MapDataStore.Tile tile, MeshLayer layer, out T value)
    {
        Content cont;
        if (layer == MeshLayer.BuildingMaterial
            || layer == MeshLayer.BuildingMaterialCutout
            || layer == MeshLayer.NoMaterialBuilding
            || layer == MeshLayer.NoMaterialBuildingCutout
            )
        {
            //Buildings are always built from the same item type, generally.
            if (itemMatcher.Get(new IntPair(-1, -1), out cont))
            {
                value = cont.GetValue(tile, layer);
                return true;
            }
        }
        else if (itemMatcher.Get(tile.construction_item, out cont))
        {
            value = cont.GetValue(tile, layer);
            return true;
        }
        value = default(T);
        return false;
    }

    protected override void ParseElementConditions(XElement elemtype, TileConfiguration<T>.Content content)
    {
        var elemItems = elemtype.Elements("item");
        foreach (XElement elemItem in elemItems)
        {
            XAttribute elemToken = elemItem.Attribute("token");
            if (elemToken != null)
            {
                itemMatcher[elemToken.Value] = content;
            }
        }
    }
}
