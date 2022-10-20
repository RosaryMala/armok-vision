// Cristian Pop - https://boxophobic.com/

using UnityEngine;
using UnityEditor;

namespace Boxophobic.StyledGUI
{
    public class StyledSpaceDrawer : MaterialPropertyDrawer
    {
        public float space;
        public string conditions = "";

        public StyledSpaceDrawer(float space)
        {
            this.space = space;
        }

        public StyledSpaceDrawer(float space, string conditions)
        {
            this.space = space;
            this.conditions = conditions;
        }

        public override void OnGUI(Rect position, MaterialProperty prop, string label, MaterialEditor materialEditor)
        {
            if (conditions == "")
            {
                GUILayout.Space(space);
            }
            else
            {
                Material material = materialEditor.target as Material;

                bool showInspector = false;

                string[] split = conditions.Split(char.Parse(" "));

                for (int i = 0; i < split.Length; i++)
                {
                    if (material.HasProperty(split[i]))
                    {
                        showInspector = true;
                        break;
                    }
                }

                if (showInspector)
                {
                    GUILayout.Space(space);
                }
            }

        }

        public override float GetPropertyHeight(MaterialProperty prop, string label, MaterialEditor editor)
        {
            return -2;
        }
    }
}
