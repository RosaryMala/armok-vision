using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;

public class RandomConfiguration<T> : ContentConfiguration<T> where T : IContent, new()
{
    class RandomItem
    {
        public Content content;
        public OpenSimplexNoise noise;
    }
    List<RandomItem> items = new List<RandomItem>();

    public override bool GetValue(MapTile tile, out T value)
    {
        double maxValue = float.MinValue;
        Content maxContent = null;
        foreach (var item in items)
        {
            double curValue = item.noise.eval(tile.position.x, tile.position.y, tile.position.z);
            if(curValue > maxValue)
            {
                maxValue = curValue;
                maxContent = item.content;
            }
        }
        if (maxContent == null)
        {
            value = default(T);
            return false;
        }
        value = maxContent.GetValue(tile);
        return true;
    }

    protected override void ParseElementConditions(System.Xml.Linq.XElement elemtype, ContentConfiguration<T>.Content content)
    {
        var elemRandoms = elemtype.Elements("random");
        foreach (XElement elemRandom in elemRandoms)
        {
            //right now, we don't actuallyu care about any parameters
            int seed = items.Count;
            RandomItem item = new RandomItem();
            item.content = content;
            item.noise = new OpenSimplexNoise(seed);
            items.Add(item);
        }
    }
}
