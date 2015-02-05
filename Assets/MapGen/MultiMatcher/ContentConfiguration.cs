using System.Xml.Linq;
using UnityEngine;
using System.Linq;

abstract public class ContentConfiguration<T> where T : IContent, new()
{
    protected class Content
    {
        public T defaultItem { private get; set; }
        ContentConfiguration<T> overloadedItem;
        public T GetValue(MapTile tile)
        {
                if (overloadedItem == null)
                    return defaultItem;
                else
                {
                    T item;
                    if (overloadedItem.GetValue(tile, out item))
                    {
                        return item;
                    }
                    else
                        return defaultItem;
                }
        }
    }
    abstract public bool GetValue(MapTile tile, out T value);

    abstract protected void ParseElementConditions(XElement elemtype, Content content);

    public string nodeName { get; set; }

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


    public static ContentConfiguration<T> GetFromElement(XElement elemRoot)
    {
        switch (elemRoot.Elements().First().Elements().First().Name.LocalName)
        {
            case "material":
                return new MaterialConfiguration<T>();
            case "tiletype":
                return new TileConfiguration<T>();
            default:
                Debug.LogError("Found unknown matching method \"" + elemRoot.Elements().First().Elements().First().Name.LocalName + "\", assuming material.");
                return new MaterialConfiguration<T>();
        }
    }
}

