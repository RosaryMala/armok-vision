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

        public static void Init()
        {
            _instance = Resources.Load<MaterialCollection>("materialDefinitions");
        }

        public IEnumerable<MatPairStruct> Keys => ((IReadOnlyDictionary<MatPairStruct, MaterialTextureSet>)matTextures).Keys;

        public IEnumerable<MaterialTextureSet> Values => ((IReadOnlyDictionary<MatPairStruct, MaterialTextureSet>)matTextures).Values;

        public int Count => ((IReadOnlyDictionary<MatPairStruct, MaterialTextureSet>)matTextures).Count;

        public MaterialTextureSet this[MatPairStruct key] => ((IReadOnlyDictionary<MatPairStruct, MaterialTextureSet>)matTextures)[key];

        private void Awake()
        {
            if(_instance != null && _instance != this)
            {
                if (Application.isPlaying)
                    Destroy(_instance);
                else
                    DestroyImmediate(_instance, true);
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
        private Texture2DArray ShapeTextureArray;
        private int ShapeTextureDepth;
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
            if (Resources.Load<Texture2DArray>("patternTextures") == null)
                return;
            if (MaterialRaws.Instance != null)
            {
                PopulateMatTextures();
                PatternTextureArray = Resources.Load<Texture2DArray>("patternTextures");
                ShapeTextureArray = Resources.Load<Texture2DArray>("shapeTextures");
                PatternTextureDepth = PatternTextureArray.depth;
                ShapeTextureDepth = ShapeTextureArray.depth;
                Shader.SetGlobalTexture("_MatTexArray", PatternTextureArray);
                Shader.SetGlobalTexture("_ShapeMap", ShapeTextureArray);
                Vector4 arrayCount = new Vector4(PatternTextureDepth, ShapeTextureDepth, 1, 1);
                Shader.SetGlobalVector("_TexArrayCount", arrayCount);
            }
        }
    }
}
