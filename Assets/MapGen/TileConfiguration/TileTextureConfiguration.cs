using System;
using System.Xml.Linq;

public class TileTextureConfiguration : TileConfiguration<int>
{
    public override string rootName
    {
        get { return "tileTextures"; }
    }

    protected override string nodeName
    {
        get { return "tileTexture"; }
    }

    public override bool ParseTypeElement(System.Xml.Linq.XElement elemtype, out int value)
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
