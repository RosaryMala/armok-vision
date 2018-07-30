using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MaterialStore
{
    [CreateAssetMenu]
    public class MaterialCollection : ScriptableObject, IReadOnlyDictionary<MatPairStruct, MaterialTextureSet>
    {
        public List<MaterialTextureSet> textures = new List<MaterialTextureSet>();

        static MaterialCollection _instance;
        public static MaterialCollection Instance
        {
            get
            {
                if(_instance == null)
                    _instance = Resources.Load<MaterialCollection>("materialDefinitions");
                return _instance;
            }
        }

        public IEnumerable<MatPairStruct> Keys => ((IReadOnlyDictionary<MatPairStruct, MaterialTextureSet>)matTextures).Keys;

        public IEnumerable<MaterialTextureSet> Values => ((IReadOnlyDictionary<MatPairStruct, MaterialTextureSet>)matTextures).Values;

        public int Count => ((IReadOnlyDictionary<MatPairStruct, MaterialTextureSet>)matTextures).Count;

        public MaterialTextureSet this[MatPairStruct key] => ((IReadOnlyDictionary<MatPairStruct, MaterialTextureSet>)matTextures)[key];

        private void Awake()
        {
            if(_instance != null && _instance != this)
            {
                Destroy(this);
                return;
            }
            if (_instance == null)
                _instance = this;
        }

        public bool ContainsKey(MatPairStruct key)
        {
            return ((IReadOnlyDictionary<MatPairStruct, MaterialTextureSet>)matTextures).ContainsKey(key);
        }

        public bool TryGetValue(MatPairStruct key, out MaterialTextureSet value)
        {
            return ((IReadOnlyDictionary<MatPairStruct, MaterialTextureSet>)matTextures).TryGetValue(key, out value);
        }

        public IEnumerator<KeyValuePair<MatPairStruct, MaterialTextureSet>> GetEnumerator()
        {
            return ((IReadOnlyDictionary<MatPairStruct, MaterialTextureSet>)matTextures).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IReadOnlyDictionary<MatPairStruct, MaterialTextureSet>)matTextures).GetEnumerator();
        }

        MaterialMatcher<MaterialTextureSet> matTextures = new MaterialMatcher<MaterialTextureSet>();
        private Texture2DArray PatternTextureArray;
        private int PatternTextureDepth;

        public void PopulateMatTextures()
        {
            matTextures.Clear();
            foreach (var tex in textures)
            {
                matTextures[tex.tag.ToString()] = tex;
            }
        }

        private void OnEnable()
        {
            Refresh();
        }

        public void Refresh()
        {
            if (MaterialRaws.Instance != null)
            {
                PopulateMatTextures();
                PatternTextureArray = Resources.Load<Texture2DArray>("patternTextures");
                PatternTextureDepth = PatternTextureArray.depth;
                Shader.SetGlobalTexture("_MatTexArray", PatternTextureArray);
                Vector4 arrayCount = new Vector4(PatternTextureDepth, 1, 1, 1);
                Shader.SetGlobalVector("_TexArrayCount", arrayCount);
            }
        }
    }
}
