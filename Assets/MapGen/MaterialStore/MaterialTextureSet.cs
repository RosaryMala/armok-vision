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
        public Texture2D texture;
        public Texture2D normal;
        public Texture2D heightMap;
        public Texture2D aoMap;
    }
}
