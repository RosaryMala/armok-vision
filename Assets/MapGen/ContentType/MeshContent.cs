using System.Xml.Linq;
using UnityEngine;
using System.IO;
using System;
using UnityExtension;

public enum MeshLayer
{
    StaticMaterial,
    BaseMaterial,
    LayerMaterial,
    VeinMaterial,
    NoMaterial,
    StaticCutout,
    BaseCutout,
    LayerCutout,
    VeinCutout,
    Growth0Cutout,
    Growth1Cutout,
    Growth2Cutout,
    Growth3Cutout,
    NoMaterialCutout,
    Count
}

public class MeshContent : IContent
{

    public Mesh[] mesh { get; private set; }
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
        mesh = new Mesh[(int)MeshLayer.Count];
        for (int i = 0; i < mesh.Length; i++)
        {
            mesh[i] = new Mesh();
            mesh[i].LoadOBJ(lOBJData, ((MeshLayer)i).ToString());
        }
        lStream = null;
        lOBJData = null;
        return true;
    }
}
