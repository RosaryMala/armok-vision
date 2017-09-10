using System;
using System.Collections;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using TokenLists;
using UnityEngine;

abstract public class TileConfiguration<T> where T : IContent, new()
{
    protected class Content
    {
        public T defaultItem { get; set; }
        public TileConfiguration<T> overloadedItem { get; set; }
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

    IEnumerator ParseContentElement(XElement elemtype, object externalStorage, object secondaryDictionary)
    {
        var stopWatch = System.Diagnostics.Stopwatch.StartNew();
        T value = new T();
        value.ExternalStorage = externalStorage;
        if (!value.AddTypeElement(elemtype))
        {
            Debug.LogError("Couldn't parse " + elemtype);
            //There was an error parsing the type
            //There's nothing to work with.
            yield break;
        }
        value.ExternalStorage = externalStorage;
        Content content = new Content();
        content.defaultItem = value;
        if (elemtype.Element("subObject") != null)
        {
            content.overloadedItem = GetFromRootElement(elemtype, "subObject");
            for(var e = content.overloadedItem.AddSingleContentConfig(elemtype, externalStorage, secondaryDictionary); e.MoveNext();)
            {
                if (stopWatch.ElapsedMilliseconds > 100)
                {
                    stopWatch.Reset();
                    stopWatch.Start();
                    yield return null;
                }
            }
        }
        ParseElementConditions(elemtype, content);
    }

    public IEnumerator AddSingleContentConfig(XElement elemRoot, object externalStorage = null, object secondaryDictionary = null)
    {
        var stopWatch = System.Diagnostics.Stopwatch.StartNew();
        SecondaryDictionary = secondaryDictionary;
        var elemValues = elemRoot.Elements(nodeName);
        foreach (XElement elemValue in elemValues)
        {
            for (var e = ParseContentElement(elemValue, externalStorage, secondaryDictionary); e.MoveNext();)
            {
                if(stopWatch.ElapsedMilliseconds > 100)
                {
                    stopWatch.Reset();
                    stopWatch.Start();
                    yield return null;
                }
            }
        }
        yield break;
    }

    public static TileConfiguration<T> GetFromRootElement(XElement elemRoot, XName name)
    {
        TileConfiguration<T> output;
        if (elemRoot.Element(name).Elements().Count() == 0)
        {
            output = new TileMaterialConfiguration<T>();
            output.nodeName = name.LocalName;
            return output;
        }
        string matchType = "NoMatch";
        XElement matchElement = elemRoot.Element(name).Elements().First();
        foreach (var element in elemRoot.Element(name).Elements())
        {
            if (element.Name.LocalName == "subObject") continue;
            matchType = element.Name.LocalName;
            matchElement = element;
            break;
        }
        switch (matchType)
        {
            case "material":
                output = new TileMaterialConfiguration<T>();
                break;
            case "tiletype":
                output = new TileTypeConfiguration<T>();
                break;
            case "random":
                output = new RandomConfiguration<T>();
                break;
            case "ramp":
                output = new RampConfiguration<T>();
                break;
            case "item":
                if (ItemTokenList.IsValid)
                    output = new ItemConfiguration<T>();
                else
                {
                    Debug.LogError("Item Types not available in this version of Remotefortressreader. Please upgrade.");
                    output = new TileMaterialConfiguration<T>();
                }
                break;
            case "growth":
                output = new PlantGrowthConfiguration<T>();
                break;
            case "designation":
                output = new TileDesignationConfiguration<T>();
                break;
            case "coverage":
                output = new CoverageConfiguration<T>();
                break;
            default:
                IXmlLineInfo lineinfo = matchElement;
                Debug.LogError("Found unknown matching method \"" + matchType + "\" int " + elemRoot.BaseUri + ":" + lineinfo.LineNumber + "," + lineinfo.LinePosition + ", assuming material.");
                output = new TileMaterialConfiguration<T>();
                break;
        }
        output.nodeName = name.LocalName;
        return output;
    }

    abstract public object SecondaryDictionary { set; }

    internal void GetValue(MapDataStore.Tile tempTile, MeshLayer staticMaterial, out object colorContent)
    {
        throw new NotImplementedException();
    }
}

