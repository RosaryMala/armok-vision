using UnityEngine;
using System.Xml.Linq;

public class ColorContent : IContent
{

    public Color value;
    public bool AddTypeElement(System.Xml.Linq.XElement elemtype)
    {
        XAttribute colorRedAtt = elemtype.Attribute("red");
        if (colorRedAtt == null)
        {
            //Add error message here
            value = default(Color);
            return false;
        }
        XAttribute colorGreenAtt = elemtype.Attribute("green");
        if (colorGreenAtt == null)
        {
            //Add error message here
            value = default(Color);
            return false;
        }
        XAttribute colorBlueAtt = elemtype.Attribute("blue");
        if (colorBlueAtt == null)
        {
            //Add error message here
            value = default(Color);
            return false;
        }
        int alpha = 255;
        XAttribute colorAlphaAtt = elemtype.Attribute("metal");
        if (colorAlphaAtt != null)
        {
            switch (colorAlphaAtt.Value)
            {
                case "yes":
                    alpha = 0;
                    break;
                case "no":
                    alpha = 255;
                    break;
                default:
                    if (!int.TryParse(colorAlphaAtt.Value, out alpha))
                        alpha = 255;
                    break;
            }
        }
        int red, green, blue;
        int.TryParse(colorRedAtt.Value, out red);
        int.TryParse(colorGreenAtt.Value, out green);
        int.TryParse(colorBlueAtt.Value, out blue);
        value = new Color(red / 255.0f, green / 255.0f, blue / 255.0f, alpha / 255.0f);
        // LINEAR
        //if (PlayerSettings.colorSpace == ColorSpace.Linear)
        {
            value = value.linear;
        }
        return true;

    }
}
