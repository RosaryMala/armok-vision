using System.Xml.Linq;

public class TextContent : IContent
{
    public string Text { get; set; }

    public object ExternalStorage { set { } }

    public bool AddTypeElement(XElement elemtype)
    {
        XAttribute textAtt = elemtype.Attribute("text");
        if (textAtt == null)
        {
            //Add error message here
            Text = null;
            return false;
        }
        Text = textAtt.Value;
        return true;
    }
}
