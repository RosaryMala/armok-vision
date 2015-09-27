using System;
using System.Xml.Linq;

public class BuildingConfiguration<T> : ContentConfiguration<T> where T : IContent, new()
{
    BuildingMatcher<Content> buildingMatcher = new BuildingMatcher<Content>();

    public override bool GetValue(MapDataStore.Tile tile, MeshLayer layer, out T value)
    {
        Content cont;
        if(buildingMatcher.Get(tile.buildingType, out cont))
        {
            value = cont.GetValue(tile, layer);
            return true;
        }
        value = default(T);
        return false;
    }

    protected override void ParseElementConditions(XElement elemtype, Content content)
    {
        var elemBuildingTypes = elemtype.Elements("buildingType");
        foreach (XElement elemBuildingType in elemBuildingTypes)
        {
            XAttribute elemToken = elemBuildingType.Attribute("token");
            if (elemToken != null)
            {
                buildingMatcher[elemToken.Value] = content;
            }
        }
    }
}
