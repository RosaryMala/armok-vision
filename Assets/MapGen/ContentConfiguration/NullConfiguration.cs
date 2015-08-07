using System.Xml.Linq;

public class NullConfiguration<T> : ContentConfiguration<T> where T : IContent, new()
{
    ContentConfiguration<T>.Content content;

    public override bool GetValue(MapDataStore.Tile tile, MeshLayer layer, out T value)
    {
        value = content.GetValue(tile, layer);
        return true;
    }

    protected override void ParseElementConditions(XElement elemtype, ContentConfiguration<T>.Content content)
    {
        this.content = content;
    }
}
