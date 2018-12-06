using RemoteFortressReader;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu]
public class ClothingTexture : ScriptableObject
{
    public bool isDress;
    public Material baseMat;

    static Dictionary<MatPairStruct, ClothingTexture> clothingLookup;

    private List<Material> _matList = new List<Material>();

    static bool gotMatIDs;
    static int mainTexID;
    static int maskTexID;
    static int bumpTexID;

    static void GetIDs()
    {
        mainTexID = Shader.PropertyToID("_MainTex");
        maskTexID = Shader.PropertyToID("_DFMask");
        bumpTexID = Shader.PropertyToID("_BumpMap");
    }

    public Material GetMaterial(int layer)
    {
        if (layer >= _matList.Count)
        {
            _matList.AddRange(Enumerable.Repeat<Material>(null, layer - _matList.Count + 1));
        }
        if (_matList[layer] == null || !Application.isPlaying)
        {
            _matList[layer] = new Material(baseMat);
            _matList[layer].renderQueue = baseMat.renderQueue + layer;
        }
        return _matList[layer];
    }

    public static ClothingTexture GetTexture(MatPairStruct item)
    {
        if (clothingLookup == null)
            clothingLookup = new Dictionary<MatPairStruct, ClothingTexture>();
        if (!clothingLookup.ContainsKey(item))
            clothingLookup[item] = Resources.Load<ClothingTexture>("Clothing/" + ItemRaws.Instance[item].id);
        return clothingLookup[item];
    }
}
