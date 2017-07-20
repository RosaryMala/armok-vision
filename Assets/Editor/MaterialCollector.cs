using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace MaterialStore
{
    public class MaterialCollector
    {
        [MenuItem("Mytools/Build Material Collection")]
        public static void BuildMaterialCollection()
        {
            var guids = AssetDatabase.FindAssets("t:Material", new[] { "Assets/MaterialTypes" });

            int colorID = Shader.PropertyToID("_Color");
            int albedoID = Shader.PropertyToID("_MainTex");
            int specularID = Shader.PropertyToID("_Specular");
            int normalID = Shader.PropertyToID("_Normal");
            int occlusionID = Shader.PropertyToID("_Occlusion");
            int heightID = Shader.PropertyToID("_Height");

            List<MaterialTextureSet> materialList = new List<MaterialTextureSet>();

            Dictionary<string, int> patternIndex = new Dictionary<string, int>();
            List<Texture2D> patternList = new List<Texture2D>();

            foreach (var item in guids)
            {
                var mat = AssetDatabase.LoadAssetAtPath<Material>(AssetDatabase.GUIDToAssetPath(item));

                MaterialTextureSet set = new MaterialTextureSet();

                set.tag = MaterialTag.Parse(mat.name);

                Debug.Log(set.tag);

                Texture2D albedo = (Texture2D)mat.GetTexture(albedoID);
                Texture2D specular = (Texture2D)mat.GetTexture(specularID);
            }
        }
    }
}
