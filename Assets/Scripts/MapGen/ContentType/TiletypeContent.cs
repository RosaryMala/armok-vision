using System.Xml.Linq;

public class TileTextureContent : IContent {
    public int value;

    public bool AddTypeElement(XElement elemtype)
    {
        XAttribute indexAtt = elemtype.Attribute("index");
        if (indexAtt == null)
        {
            //Add error message here
            value = default(int);
            return false;
        }
        return int.TryParse(indexAtt.Value, out value);
    }


    public object ExternalStorage
    {
        set
        {
        }
    }
}
