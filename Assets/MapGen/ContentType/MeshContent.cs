using System.Xml.Linq;
using UnityEngine;
using System.IO;
using System;
using UnityExtension;

public class MeshContent : IContent
{
    public Mesh mesh { get; private set; }
    public bool AddTypeElement(System.Xml.Linq.XElement elemtype)
    {
        XAttribute fileAtt = elemtype.Attribute("file");
        if (fileAtt == null)
        {
            //Add error message here
            return false;
        }
        string filePath = Path.Combine(Path.GetDirectoryName(new Uri(elemtype.BaseUri).LocalPath), fileAtt.Value);
        filePath = Path.GetFullPath(filePath);

        //	Load the OBJ in
        var lStream = new FileStream(filePath, FileMode.Open);
        var lOBJData = OBJLoader.LoadOBJ(lStream);
        lStream.Close();
        mesh = new Mesh();
        mesh.LoadOBJ(lOBJData);
        lStream = null;
        lOBJData = null;
        return true;
    }
}
