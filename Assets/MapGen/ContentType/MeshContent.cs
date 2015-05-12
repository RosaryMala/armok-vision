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

    public MeshData[] meshData { get; private set; }
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
        meshData = new MeshData[(int)MeshLayer.Count];
        Mesh tempMesh = new Mesh();
        for (int i = 0; i < meshData.Length; i++)
        {
            tempMesh.LoadOBJ(lOBJData, ((MeshLayer)i).ToString());
            meshData[i] = new MeshData(tempMesh);
            tempMesh.Clear();
        }
        lStream = null;
        lOBJData = null;
        return true;
    }
}
