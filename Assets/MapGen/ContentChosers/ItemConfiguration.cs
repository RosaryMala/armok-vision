using System.Xml.Linq;

public class ItemContent<T> : ContentConfiguration<T> where T : IContent, new()
{
    public override bool GetValue(MapDataStore.Tile tile, MeshLayer layer, out T value)
    {
        throw new System.NotImplementedException();
    }

    protected override void ParseElementConditions(XElement elemtype, ContentConfiguration<T>.Content content)
    {
        throw new System.NotImplementedException();
    }
}
