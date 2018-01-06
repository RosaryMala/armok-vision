using RemoteFortressReader;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemModel : MonoBehaviour
{
    public Item originalItem;
    bool setMaterials = false;
    int originalMaterialID;
    static Dictionary<int, Material> opaqueMaterialVersions = new Dictionary<int, Material>();
    static Dictionary<int, Material> transparentMaterialVersions = new Dictionary<int, Material>();
    private MeshRenderer meshRenderer;

    [System.Serializable]
    public class AlternateShape
    {
        public string ID;
        public Mesh Mesh;
    }
    public List<AlternateShape> alternateShapes = new List<AlternateShape>();

    Mesh getAltShapeMesh(int i)
    {
        if (alternateShapes.Count == 0)
            return null;
        if (DFConnection.Instance.NetLanguageList == null)
            return null;
        string name = DFConnection.Instance.NetLanguageList.shapes[i].id;

        int index = alternateShapes.FindIndex(x => x.ID == name);
        if (index >= 0)
            return alternateShapes[index].Mesh;
        return null;
    }

    Material OriginalMaterial
    {
        get
        {
            if (!setMaterials)
                UpdateMaterialVersions();
            return opaqueMaterialVersions[originalMaterialID];
        }
    }
    Material TransparentMaterial
    {
        get
        {
            if (!setMaterials)
                UpdateMaterialVersions();
            return transparentMaterialVersions[originalMaterialID];
        }
    }

    private void Awake()
    {
        meshRenderer = GetComponentInChildren<MeshRenderer>();
    }


    private void UpdateMaterialVersions()
    {
        if (setMaterials)
            return;
        setMaterials = true;
        var originalMat = meshRenderer.sharedMaterial;
        originalMaterialID = originalMat.GetInstanceID();
        if (!opaqueMaterialVersions.ContainsKey(originalMaterialID))
        {
            if (originalMat.shader.name == "Standard")
            {
                Debug.LogWarning(Building.MaterialPart.GetTreeName(gameObject) + " Has a standard shader!");
                opaqueMaterialVersions[originalMaterialID] = originalMat;
                transparentMaterialVersions[originalMaterialID] = originalMat;
            }
            else
            {
                opaqueMaterialVersions[originalMaterialID] = originalMat;
                transparentMaterialVersions[originalMaterialID] = new Material(originalMat);
                transparentMaterialVersions[originalMaterialID].shader = Shader.Find("Building/Transparent");
            }
        }
    }

    public void UpdateMaterial(Item itemInput)
    {
        if (setMaterials)
            return;
        originalItem = itemInput;

        if (meshRenderer == null)
            meshRenderer = GetComponentInChildren<MeshRenderer>();

        Color partColor = ContentLoader.GetColor(itemInput);
        float textureIndex = ContentLoader.GetPatternIndex(itemInput.material);

        if (partColor.a < 0.5f)
            meshRenderer.sharedMaterial = TransparentMaterial;
        else
            meshRenderer.sharedMaterial = OriginalMaterial;

        MaterialPropertyBlock prop = new MaterialPropertyBlock();
        prop.SetColor("_MatColor", partColor);
        prop.SetFloat("_MatIndex", textureIndex);
        meshRenderer.SetPropertyBlock(prop);

        var altmesh = getAltShapeMesh(itemInput.shape);
        if(altmesh != null)
        {
            GetComponentInChildren<MeshFilter>().sharedMesh = altmesh;
            var collider = GetComponentInChildren<MeshCollider>();
            if (collider != null)
                collider.sharedMesh = altmesh;
        }
    }

    public void OnMouseDown()
    {
        string mat = originalItem.material.ToString();
        if (GameMap.materials.ContainsKey(originalItem.material))
            mat = GameMap.materials[originalItem.material].id;
        if(originalItem.stack_size > 1)
            Debug.Log(string.Format("{0} {1} [{2}]", mat, GameMap.items[originalItem.type].id, originalItem.stack_size));
        else
            Debug.Log(string.Format("{0} {1}", mat, GameMap.items[originalItem.type].id));
    }
}
