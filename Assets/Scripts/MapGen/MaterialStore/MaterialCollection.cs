using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MaterialStore
{
    [CreateAssetMenu]
    public class MaterialCollection : ScriptableObject
    {
        public List<MaterialTextureSet> textures = new List<MaterialTextureSet>();
    }
}
