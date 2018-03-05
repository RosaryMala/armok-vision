using System;
using System.Xml.Linq;

public class NullConfiguration<T> : TileConfiguration<T> where T : IContent, new()
{
    TileConfiguration<T>.Content content;

    public override object SecondaryDictionary
    {
        set
        {
        }
    }

    public override bool GetValue(MapDataStore.Tile tile, MeshLayer layer, out T value)
    {
        value = content.GetValue(tile, layer);
        return true;
    }

    protected override void ParseElementConditions(XElement elemtype, TileConfiguration<T>.Content content)
    {
        this.content = content;
    }
}
