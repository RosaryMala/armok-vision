using System.Xml.Linq;

public interface IContent {
    bool AddTypeElement(System.Xml.Linq.XElement elemtype);
    object ExternalStorage { set; }
}
