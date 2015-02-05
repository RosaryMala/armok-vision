using System.Xml.Linq;

public class IndexContent : IContent
{
    public int value;
    public bool AddTypeElement(System.Xml.Linq.XElement elemtype)
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
}
