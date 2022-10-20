// Cristian Pop - https://boxophobic.com/

using UnityEngine;
using UnityEditor;
using System;

namespace Boxophobic.StyledGUI
{
    public class StyledDiffusionMaterialDrawer : MaterialPropertyDrawer
    {
        public string propName;
        //GUIStyle styleCenteredHelpBox;

        public StyledDiffusionMaterialDrawer(string propName)
        {
            this.propName = propName;
        }

        public override void OnGUI(Rect position, MaterialProperty prop, String label, MaterialEditor materialEditor)
        {
            //SetGUIStyles();

            Material material = materialEditor.target as Material;

            UnityEngine.Object materialAsset = null;

            GUILayout.Space(5);            

            if (material.GetInt(propName) == 0)
            {
                EditorGUILayout.HelpBox("Diffusion profile values not set! Due to the current HDRP architecture the diffusion profiles are not directly supported. You will need to create an HDRP Lit material and assign a Diffusion Profile to it, drag this HDRP material to the " + label + " slot to allow the profile values to be copied to the material. The HDRP material will not be saved to the property field! Please refer to the documentation for more information.", MessageType.Warning);
            }
            else
            {
                EditorGUILayout.HelpBox("Diffusion profile values set! Due to the current HDRP architecture the diffusion profiles are not directly supported. You will need to create an HDRP Lit material and assign a Diffusion Profile to it, drag this HDRP material to the " + label + " slot to allow the profile values to be copied to the material. The HDRP material will not be saved to the property field! Please refer to the documentation for more information.", MessageType.Info);
            }

            GUILayout.Space(10);

            materialAsset = (Material)EditorGUILayout.ObjectField(label, materialAsset, typeof(Material), false);

            Material materialObject = AssetDatabase.LoadAssetAtPath<Material>(AssetDatabase.GetAssetPath(materialAsset));

            if (materialAsset != null)
            {
                if (materialObject.HasProperty("_DiffusionProfileAsset") && materialObject.HasProperty("_DiffusionProfileHash"))
                {
                    var diffusionProfileAsset = materialObject.GetVector("_DiffusionProfileAsset");
                    var diffusionProfileHash = materialObject.GetFloat("_DiffusionProfileHash");

                    if (diffusionProfileAsset.x != 0 && diffusionProfileHash != 0)
                    {
                        material.SetVector(propName + "_asset", diffusionProfileAsset);
                        material.SetFloat(propName, diffusionProfileHash);

                        Debug.Log("Diffusion Profile settings copied from " + materialObject.name + "!");

                        materialAsset = null;
                    }
                    else
                    {
                        material.SetVector(propName + "_asset", Vector4.zero);
                        material.SetFloat(propName, 0.0f);

                        Debug.Log("Diffusion Profile settings set to None because " + materialObject.name + " has no Diffusion Profile asset!");

                        materialAsset = null;
                    }
                }
                else
                {
                    Debug.Log("The Material used to copy the Diffusion Profile does not a valid Diffusion Profile!");
                }
            }

            //EditorGUI.HelpBox(new Rect(position.x, position.y + top, position.width, position.height), message, mType);
        }

        public override float GetPropertyHeight(MaterialProperty prop, string label, MaterialEditor editor)
        {
            return -2;
        }

        //void SetGUIStyles()
        //{
        //    styleCenteredHelpBox = new GUIStyle(GUI.skin.GetStyle("HelpBox"))
        //    {
        //        alignment = TextAnchor.MiddleCenter,
        //    };

        //}
    }
}
