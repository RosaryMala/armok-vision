using System.Collections.Generic;
using System.Xml.Linq;

public class RandomConfiguration<T> : ContentConfiguration<T> where T : IContent, new()
{
    class RandomItem
    {
        public Content content;
        public OpenSimplexNoise noise;
        public float scale_x = 1;
        public float scale_y = 1;
        public float scale_z = 1;
        public float intensity = 1;
    }
    List<RandomItem> items = new List<RandomItem>();

    public override bool GetValue(MapTile tile, MeshLayer layer, out T value)
    {
        double maxValue = float.MinValue;
        Content maxContent = null;
        foreach (var item in items)
        {
            double curValue = (
                item.noise.eval(tile.position.x / item.scale_x, tile.position.y / item.scale_y, tile.position.z / item.scale_z)
                + 0.944824004155211) * item.intensity; // -0.944824004155211 was found on testing to be the minimum.
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
        value = maxContent.GetValue(tile, layer);
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
            XAttribute elemScale = elemRandom.Attribute("scale");
            if (elemScale != null)
            {
                float scale = 1;
                float.TryParse(elemScale.Value, out scale);
                item.scale_x = scale;
                item.scale_y = scale;
                item.scale_z = scale;
            }
            XAttribute elemScaleX = elemRandom.Attribute("scale_x");
            if (elemScaleX != null)
            {
                float scale = 1;
                float.TryParse(elemScaleX.Value, out scale);
                item.scale_x = scale;
            }
            XAttribute elemScaleY = elemRandom.Attribute("scale_y");
            if (elemScaleY != null)
            {
                float scale = 1;
                float.TryParse(elemScaleY.Value, out scale);
                item.scale_y = scale;
            }
            XAttribute elemScaleZ = elemRandom.Attribute("scale_z");
            if (elemScaleZ != null)
            {
                float scale = 1;
                float.TryParse(elemScaleZ.Value, out scale);
                item.scale_z = scale;
            }
            XAttribute elemIntensity = elemRandom.Attribute("intensity");
            if (elemIntensity != null)
            {
                float scale = 1;
                float.TryParse(elemIntensity.Value, out scale);
                item.intensity = scale;
            }
            item.content = content;
            item.noise = new OpenSimplexNoise(seed);
            items.Add(item);
        }
    }
}
