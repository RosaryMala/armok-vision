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

            MaterialCollection matCollection = ScriptableObject.CreateInstance<MaterialCollection>();

            Dictionary<string, int> patternIndex = new Dictionary<string, int>();
            List<Texture2D> albedoList = new List<Texture2D>();
            List<Texture2D> specularList = new List<Texture2D>();

            var defaultAlbedo = Resources.Load<Texture2D>("Grey");
            var defaultSpecular = Resources.Load<Texture2D>("Low_S");

            foreach (var item in guids)
            {
                var mat = AssetDatabase.LoadAssetAtPath<Material>(AssetDatabase.GUIDToAssetPath(item));

                MaterialTextureSet set = new MaterialTextureSet();

                set.tag = MaterialTag.Parse(mat.name);

                set.color = mat.color;

                Texture2D albedo = (Texture2D)mat.GetTexture(albedoID);
                Texture2D specular = (Texture2D)mat.GetTexture(specularID);

                if (albedo == null)
                    albedo = defaultAlbedo;

                if (specular == null)
                    specular = defaultSpecular;

                string id = albedo.GetInstanceID().ToString() + "," + specular.GetInstanceID().ToString();

                if(patternIndex.ContainsKey(id))
                {
                    set.patternIndex = patternIndex[id];
                }
                else
                {
                    set.patternIndex = albedoList.Count;
                    patternIndex[id] = albedoList.Count;
                    albedoList.Add(albedo);
                    specularList.Add(specular);
                }

                matCollection.textures.Add(set);
            }

            Texture2DArray patternArray = new Texture2DArray(256, 256, albedoList.Count, TextureFormat.ARGB32, true, false);
            Texture2D tempTex = new Texture2D(256, 256, TextureFormat.ARGB32, false);
            Material patternMat = new Material(Shader.Find("Hidden/PatternTextureMaker"));

            for (int i = 0; i < albedoList.Count; i++)
            {
                Debug.Log(albedoList[i].name + "," + specularList[i].name);
                var albedoTarget = RenderTexture.GetTemporary(256, 256, 0, RenderTextureFormat.ARGB32);
                patternMat.SetTexture(specularID, specularList[i]);
                Graphics.Blit(albedoList[i], albedoTarget, patternMat);

                var backup = RenderTexture.active;
                RenderTexture.active = albedoTarget;
                tempTex.ReadPixels(new Rect(0, 0, 256, 256), 0, 0);
                patternArray.SetPixels(tempTex.GetPixels(), i);
                RenderTexture.active = backup;
                RenderTexture.ReleaseTemporary(albedoTarget);
            }

            patternArray.Apply(true);

            AssetDatabase.CreateAsset(patternArray, "Assets/Resources/patternTextures.asset");
            AssetDatabase.CreateAsset(matCollection, "Assets/Resources/materialDefinitions.asset");
            AssetDatabase.SaveAssets();
        }
    }
}
