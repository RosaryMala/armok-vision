using System.Xml.Linq;
using System;

public class LayerContent : IContent
{
    public enum Layer
    {
        SOLID,
        TRANSPARENT
    }
    public Layer layer;
    public bool AddTypeElement(System.Xml.Linq.XElement elemtype)
    {
        XAttribute layerAtt = elemtype.Attribute("type");
        if (layerAtt == null)
        {
            //Add error message here
            layer = Layer.SOLID;
            return false;
        }
        try
        {
            layer = (Layer)Enum.Parse(typeof(Layer), layerAtt.Value);
        }
        catch (Exception)
        {
            return false;
        }
        return true;
    }
}
