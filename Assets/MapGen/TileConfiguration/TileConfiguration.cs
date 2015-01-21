using RemoteFortressReader;
using System.Collections.Generic;
using System.Xml.Linq;

abstract public class TileConfiguration<T> 
{
    TiletypeMatcher<T> tiletypeMatcher = new TiletypeMatcher<T>();
    abstract public string rootName { get; }
    abstract protected string nodeName { get; }

    public T this[int tiletype]
    {
        get
        {
            return tiletypeMatcher[tiletype];
        }
    }

    public List<Tiletype> tiletypeTokenList
    {
        set
        {
            tiletypeMatcher.tiletypeTokenList = value;
        }
    }
    public bool AddSingleTiletypeConfig(XElement elemRoot)
    {
        if (elemRoot.Name != rootName)
            return true;
        var elemColors = elemRoot.Elements(nodeName);
        foreach (XElement elemColor in elemColors)
        {
            ParseTiletypeElement(elemColor);
        }

        return true;
    }

    abstract public bool ParseTypeElement(XElement elemtype, out T value);

    void ParseTiletypeElement(XElement elemtype)
    {
        T value;
        if(!ParseTypeElement(elemtype, out value))
        {
            //put an error message here
            return;
        }
        var elemTiletypes = elemtype.Elements("tiletype");
        foreach(XElement elemTiletype in elemTiletypes)
        {
            XAttribute elemToken = elemTiletype.Attribute("token");
            if(elemToken != null)
            {
                tiletypeMatcher[elemToken.Value] = value;
                continue;
            }
        }
    }
}
