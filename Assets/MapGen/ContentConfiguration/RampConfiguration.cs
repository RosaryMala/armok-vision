using System.Xml.Linq;
using UnityEngine;

public class RampConfiguration<T> : ContentConfiguration<T> where T : IContent, new()
{
    Content[] rampList = new Content[26];
    public override bool GetValue(MapDataStore.Tile tile, MeshLayer layer, out T value)
    {
        if (tile.shape != RemoteFortressReader.TiletypeShape.RAMP)
        {
            value = default(T);
            return false;
        }
        int index = tile.RampType - 1;
        if(rampList[index] == null)
        {
            value = default(T);
            return false;
        }
        value = rampList[index].GetValue(tile, layer);
        return true;
    }

    protected override void ParseElementConditions(XElement elemtype, ContentConfiguration<T>.Content content)
    {
        var elemRamps = elemtype.Elements("ramp");
        foreach (XElement elemRamp in elemRamps)
        {
            XAttribute indexAttr = elemRamp.Attribute("index");
            if (indexAttr == null)
                continue;
            int index = 0;
            if (!int.TryParse(indexAttr.Value, out index))
                continue;
            if (index > 26)
                continue;
            rampList[index - 1] = content;
        }
    }

}
