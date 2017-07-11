using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace MaterialStore
{
    [CreateAssetMenu]
    public class MaterialCollection : ScriptableObject
    {
        public List<MaterialTextureSet> textures;
    }
}
