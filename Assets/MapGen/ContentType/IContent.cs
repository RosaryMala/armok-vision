using System.Xml.Linq;

public interface IContent {
    bool AddTypeElement(XElement elemtype);
    object ExternalStorage { set; }
}
