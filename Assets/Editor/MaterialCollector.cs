using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace MaterialStore
{
    public class MaterialCollector
    {
        private static List<Texture2D> albedoList;
        private static List<Texture2D> alphaList;
        private static Texture2D defaultAlbedo;
        private static Texture2D defaultAlpha;
        private static Texture2D defaultNormal;
        private static Texture2D defaultOcclusion;
        private static Texture2D defaultSpecular;
        private static List<Texture2D> normalList;
        private static List<Texture2D> occlusionList;
        private static Dictionary<string, int> patternIndex;
        private static Dictionary<string, int> shapeIndex;
        private static List<Texture2D> specularList;

        [MenuItem("Mytools/Build Material Collection")]
        public static void BuildMaterialCollection()
        {
            var materialGUIDs = AssetDatabase.FindAssets("t:Material", new[] { "Assets/Materials/MaterialTypes" });

            int colorID = Shader.PropertyToID("_Color");
            int albedoID = Shader.PropertyToID("_MainTex");
            int specularID = Shader.PropertyToID("_Specular");
            int normalID = Shader.PropertyToID("_Normal");
            int occlusionID = Shader.PropertyToID("_Occlusion");
            int heightID = Shader.PropertyToID("_Height");

            MaterialCollection matCollection = ScriptableObject.CreateInstance<MaterialCollection>();

            patternIndex = new Dictionary<string, int>();
            albedoList = new List<Texture2D>();
            specularList = new List<Texture2D>();

            shapeIndex = new Dictionary<string, int>();
            normalList = new List<Texture2D>();
            occlusionList = new List<Texture2D>();
            alphaList = new List<Texture2D>();

            defaultAlbedo = Resources.Load<Texture2D>("Grey");
            defaultSpecular = Resources.Load<Texture2D>("Low_S");

            defaultNormal = Resources.Load<Texture2D>("Flat_N");
            defaultOcclusion = Resources.Load<Texture2D>("Flat_O");
            defaultAlpha = Resources.Load<Texture2D>("Opaque_A");

            matCollection.textures.Add(CreateTextureSet(new MaterialTag(), new Color(0.5f, 0.5f, 0.5f, 0.5f), null, null, null, null, null));

            foreach (var item in materialGUIDs)
            {
                var mat = AssetDatabase.LoadAssetAtPath<Material>(AssetDatabase.GUIDToAssetPath(item));

                matCollection.textures.Add(
                    CreateTextureSet(MaterialTag.Parse(mat.name),
                    mat.GetColor(colorID),
                    (Texture2D)mat.GetTexture(albedoID),
                    (Texture2D)mat.GetTexture(specularID),
                    (Texture2D)mat.GetTexture(normalID),
                    (Texture2D)mat.GetTexture(occlusionID),
                    (Texture2D)mat.GetTexture(heightID)));
            }

            Texture2DArray patternArray = new Texture2DArray(256, 256, albedoList.Count, TextureFormat.ARGB32, true, false);
            Texture2D tempTex = new Texture2D(256, 256, TextureFormat.ARGB32, false, false);
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

            Texture2DArray shapeArray = new Texture2DArray(256, 256, normalList.Count, TextureFormat.ARGB32, true, true);
            tempTex = new Texture2D(256, 256, TextureFormat.ARGB32, false, true);
            Material shapeMat = new Material(Shader.Find("Hidden/ShapeTextureMaker"));

            for (int i = 0; i < normalList.Count; i++)
            {
                Debug.Log(normalList[i].name + "," + occlusionList[i].name + "," + alphaList[i].name);
                var shapeTarget = RenderTexture.GetTemporary(256, 256, 0, RenderTextureFormat.ARGB32);
                shapeMat.SetTexture(occlusionID, occlusionList[i]);
                shapeMat.SetTexture(heightID, alphaList[i]);
                Graphics.Blit(normalList[i], shapeTarget, shapeMat);

                var backup = RenderTexture.active;
                RenderTexture.active = shapeTarget;
                tempTex.ReadPixels(new Rect(0, 0, 256, 256), 0, 0);
                shapeArray.SetPixels(tempTex.GetPixels(), i);
                RenderTexture.active = backup;
                RenderTexture.ReleaseTemporary(shapeTarget);
            }

            shapeArray.Apply(true);

            AssetDatabase.CreateAsset(patternArray, "Assets/Resources/patternTextures.asset");
            AssetDatabase.CreateAsset(shapeArray, "Assets/Resources/shapeTextures.asset");
            AssetDatabase.CreateAsset(matCollection, "Assets/Resources/materialDefinitions.asset");
            AssetDatabase.SaveAssets();
        }

        private static MaterialTextureSet CreateTextureSet(MaterialTag materialTag, Color color, Texture2D albedo = null, Texture2D specular = null, Texture2D normal = null, Texture2D occlusion = null, Texture2D alpha = null)
        {
            if (albedo == null)
                albedo = defaultAlbedo;
            if (specular == null)
                specular = defaultSpecular;
            if (normal == null)
                normal = defaultNormal;
            if (occlusion == null)
                occlusion = defaultOcclusion;
            if (alpha == null)
                alpha = defaultAlpha;

            MaterialTextureSet set = new MaterialTextureSet();

            set.tag = materialTag;
            set.color = color;

            string patternID = albedo.GetInstanceID().ToString() + "," + specular.GetInstanceID().ToString();

            if (patternIndex.ContainsKey(patternID))
            {
                set.patternIndex = patternIndex[patternID];
            }
            else
            {
                set.patternIndex = albedoList.Count;
                patternIndex[patternID] = albedoList.Count;
                albedoList.Add(albedo);
                specularList.Add(specular);
            }

            string shapeID = normal.GetInstanceID().ToString() + "," + occlusion.GetInstanceID().ToString() + "," + alpha.GetInstanceID().ToString();

            if(shapeIndex.ContainsKey(shapeID))
            {
                set.shapeIndex = shapeIndex[shapeID];
            }
            else
            {
                set.shapeIndex = normalList.Count;
                shapeIndex[shapeID] = normalList.Count;
                normalList.Add(normal);
                occlusionList.Add(occlusion);
                alphaList.Add(alpha);
            }

            return set;
        }
    }
}
