using System.Xml.Linq;
using UnityEngine;
using System.Linq;

abstract public class ContentConfiguration<T> where T : IContent, new()
{
    protected class Content
    {
        public T defaultItem { private get; set; }
        public ContentConfiguration<T> overloadedItem { get; set; }
        public T GetValue(MapDataStore.Tile tile, MeshLayer layer)
        {
            if (overloadedItem == null)
                return defaultItem;
            else
            {
                T item;
                if (overloadedItem.GetValue(tile, layer, out item))
                {
                    return item;
                }
                else
                    return defaultItem;
            }
        }
    }
    abstract public bool GetValue(MapDataStore.Tile tile, MeshLayer layer, out T value);

    abstract protected void ParseElementConditions(XElement elemtype, Content content);

    string nodeName { get; set; }

    void ParseContentElement(XElement elemtype)
    {
        T value = new T();
        if (!value.AddTypeElement(elemtype))
        {
            Debug.LogError("Couldn't parse " + elemtype);
            //There was an error parsing the type
            //There's nothing to work with.
            return;
        }
        Content content = new Content();
        content.defaultItem = value;
        ParseElementConditions(elemtype, content);
        if (elemtype.Element("subObject") != null)
        {
            content.overloadedItem = GetFromRootElement(elemtype, "subObject");
            content.overloadedItem.AddSingleContentConfig(elemtype);
        }
    }

    public bool AddSingleContentConfig(XElement elemRoot)
    {
        var elemValues = elemRoot.Elements(nodeName);
        foreach (XElement elemValue in elemValues)
        {
            ParseContentElement(elemValue);
        }
        return true;
    }

    public static ContentConfiguration<T> GetFromRootElement(XElement elemRoot, XName name)
    {
        ContentConfiguration<T> output;
        if (elemRoot.Element(name).Elements().Count() == 0)
        {
            output = new MaterialConfiguration<T>();
            output.nodeName = name.LocalName;
            return output;
        }
        switch (elemRoot.Element(name).Elements().First().Name.LocalName)
        {
            case "material":
                output = new MaterialConfiguration<T>();
                break;
            case "tiletype":
                output = new TileConfiguration<T>();
                break;
            case "random":
                output = new RandomConfiguration<T>();
                break;
            case "ramp":
                output = new RampConfiguration<T>();
                break;
            case "item":
                output = new ItemConfiguration<T>();
                break;
            default:
                Debug.LogError("Found unknown matching method \"" + elemRoot.Element(name).Elements().First().Name.LocalName + "\", assuming material.");
                output = new MaterialConfiguration<T>();
                break;
        }
        output.nodeName = name.LocalName;
        return output;
    }
}

