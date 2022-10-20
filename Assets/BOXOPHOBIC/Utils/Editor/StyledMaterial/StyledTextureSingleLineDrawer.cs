// Cristian Pop - https://boxophobic.com/

using UnityEngine;
using UnityEditor;

namespace Boxophobic.StyledGUI
{
    public class StyledTextureSingleLineDrawer : MaterialPropertyDrawer
    {
        public float top;
        public float down;

        public StyledTextureSingleLineDrawer()
        {
            this.top = 0;
            this.down = 0;
        }

        public StyledTextureSingleLineDrawer(float top, float down)
        {
            this.top = top;
            this.down = down;
        }

        public override void OnGUI(Rect position, MaterialProperty prop, string label, MaterialEditor materialEditor)
        {
            GUILayout.Space(top);

            materialEditor.TexturePropertySingleLine(new GUIContent(prop.displayName), prop);

            GUILayout.Space(down);
        }

        public override float GetPropertyHeight(MaterialProperty prop, string label, MaterialEditor editor)
        {
            return -2;
        }
    }
}
