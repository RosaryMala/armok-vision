using System.Linq;
using System.Xml.Linq;
using UnityEngine;
using RemoteFortressReader;

abstract public class CreatureConfiguration<T> where T : IContent, new()
{
    protected class Content
    {
        public T defaultItem { get; set; }
        public CreatureConfiguration<T> overloadedItem { get; set; }
        public T GetValue(UnitDefinition unit)
        {
            if (overloadedItem == null)
                return defaultItem;
            else
            {
                T item;
                if (overloadedItem.GetValue(unit, out item))
                {
                    return item;
                }
                else
                    return defaultItem;
            }
        }
    }

    abstract public bool GetValue(UnitDefinition unit, out T value);

    abstract protected void ParseElementConditions(XElement elemtype, Content content);

    abstract public object SecondaryDictionary { set; }

    string nodeName { get; set; }

    void ParseContentElement(XElement elemtype, object externalStorage, object secondaryDictionary)
    {
        T value = new T();
        value.ExternalStorage = externalStorage;
        if (!value.AddTypeElement(elemtype))
        {
            Debug.LogError("Couldn't parse " + elemtype);
            //There was an error parsing the type
            //There's nothing to work with.
            return;
        }
        value.ExternalStorage = externalStorage;
        Content content = new Content();
        content.defaultItem = value;
        ParseElementConditions(elemtype, content);
        if (elemtype.Element("subObject") != null)
        {
            content.overloadedItem = GetFromRootElement(elemtype, "subObject");
            content.overloadedItem.AddSingleContentConfig(elemtype, externalStorage, secondaryDictionary);
        }
    }

    public bool AddSingleContentConfig(XElement elemRoot, object externalStorage = null, object secondaryDictionary = null)
    {
        SecondaryDictionary = secondaryDictionary;
        var elemValues = elemRoot.Elements(nodeName);
        foreach (XElement elemValue in elemValues)
        {
            ParseContentElement(elemValue, externalStorage, secondaryDictionary);
        }
        return true;
    }

    public static CreatureConfiguration<T> GetFromRootElement(XElement elemRoot, XName name)
    {
        CreatureConfiguration<T> output;
        if (elemRoot.Element(name).Elements().Count() == 0)
        {
            output = new CreatureRaceConfiguration<T>();
            output.nodeName = name.LocalName;
            return output;
        }
        switch (elemRoot.Element(name).Elements().First().Name.LocalName)
        {
            default:
                output = new CreatureRaceConfiguration<T>();
                break;
        }
        output.nodeName = name.LocalName;
        return output;
    }


}
