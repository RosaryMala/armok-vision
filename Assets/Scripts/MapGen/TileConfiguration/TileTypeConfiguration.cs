using System.Xml.Linq;

public class TileTypeConfiguration<T> : TileConfiguration<T> where T : IContent, new()
{
    TiletypeMatcher<Content> tiletypeMatcher = new TiletypeMatcher<Content>();

    public override object SecondaryDictionary
    {
        set
        {
        }
    }

    protected override void ParseElementConditions(XElement elemtype, Content content)
    {
        var elemTiletypes = elemtype.Elements("tiletype");
        foreach (XElement elemTiletype in elemTiletypes)
        {
            XAttribute elemToken = elemTiletype.Attribute("token");
            if (elemToken != null)
            {
                tiletypeMatcher[elemToken.Value] = content;
                continue;
            }
        }
    }

    public override bool GetValue(MapDataStore.Tile tile, MeshLayer layer, out T value)
    {
        Content cont;
        if (tiletypeMatcher.Get(tile.tileType, out cont))
        {
            value = cont.GetValue(tile, layer);
            return true;
        }
        value = default(T);
        return false;
    }
}
