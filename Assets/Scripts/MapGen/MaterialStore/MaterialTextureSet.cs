using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MaterialStore
{
    [System.Serializable]
    public class MaterialTextureSet
    {
        public MaterialTag tag;
        public Color color = Color.grey;
        public int patternIndex;
        public int shapeIndex;
    }
}
