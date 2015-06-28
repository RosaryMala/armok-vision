using System.Xml.Linq;

public class ItemConfiguration<T> : ContentConfiguration<T> where T : IContent, new()
{
    ItemMatcher<Content> itemMatcher = new ItemMatcher<Content>();

    public override bool GetValue(MapDataStore.Tile tile, MeshLayer layer, out T value)
    {
        Content cont;
        if (itemMatcher.Get(tile.construction_item, out cont))
        {
            value = cont.GetValue(tile, layer);
            return true;
        }
        value = default(T);
        return false;
    }

    protected override void ParseElementConditions(XElement elemtype, ContentConfiguration<T>.Content content)
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
