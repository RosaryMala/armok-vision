using UnityEngine;
using System.Collections.Generic;

public class TerrainMaterialManager : MonoBehaviour
{
    /// <summary>
    /// Not technically a material, but it's a unique combination of pattern, shape, and color.
    /// </summary>
    struct SplatMaterial
    {
        public int patternIndex;
        public int shapeIndex;

        public override bool Equals(object obj)
        {
            if (!(obj is SplatMaterial))
                return false;
            SplatMaterial b = (SplatMaterial)obj;
            if (patternIndex != b.patternIndex)
                return false;
            if (shapeIndex != b.shapeIndex)
                return false;
            return true;
        }

        public override int GetHashCode()
        {
            return patternIndex *65545 + shapeIndex;
        }
    }
    public Material terrainSplatMaterial;
    Dictionary<HashSet<MatPairStruct>, Material> _materialList = new Dictionary<HashSet<MatPairStruct>, Material>(new HashSetEqualityComparer<MatPairStruct>());

    List<Color[]>[] _splatControlLayers;

    public void Initialize()
    {
        _splatControlLayers = new List<Color[]>[MapDataStore.MapSize.z];
        _materialList.Clear();
    }

    public void BuildSplatControlSet(int z)
    {
        //We don't have a large number of materials, so lists are better, presumably.
        List<SplatMaterial> matList = new List<SplatMaterial>(32);

        for(int x = 0; x < MapDataStore.MapSize.x; x++)
            for(int y = 0; y < MapDataStore.MapSize.y; y++)
            {
                SplatMaterial material = new SplatMaterial();
                var tile = MapDataStore.Main[x, y, z];
                if (tile == null)
                    continue;

                NormalContent normalContent;
                if (ContentLoader.Instance.TerrainShapeTextureConfiguration.GetValue(tile, MeshLayer.LayerMaterial, out normalContent))
                    material.shapeIndex = normalContent.StorageIndex;
                else
                    material.shapeIndex = ContentLoader.Instance.DefaultShapeTexIndex;

                TextureContent matContent;
                if (ContentLoader.Instance.MaterialTextureConfiguration.GetValue(tile, MeshLayer.LayerMaterial, out matContent))
                    material.patternIndex = matContent.StorageIndex;
                else
                    material.patternIndex = ContentLoader.Instance.DefaultMatTexIndex;

                if (!matList.Contains(material))
                    matList.Add(material);
            }
    }
    private void Start()
    {
        for(int i = 1; i < 62; i++)
        {
            TextureFormat format = (TextureFormat)i;
            Debug.Log(format.ToString() + ": " + SystemInfo.SupportsTextureFormat(format));
        }
    }
}
