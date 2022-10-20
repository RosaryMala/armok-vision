// Cristian Pop - https://boxophobic.com/

using UnityEngine;
using UnityEditor;
using System;

public class StyledOptionsSliderDrawer : MaterialPropertyDrawer
{
    public string nameMin = "";
    public string nameMax = "";
    public string nameVal = "";
    public float min = 0;
    public float max = 0;
    public float val = 0;
    public float top = 0;
    public float down = 0;

    bool showAdvancedOptions = false;

    public StyledOptionsSliderDrawer(string nameMin, string nameMax, string nameVal, float min, float max, float val)
    {
        this.nameMin = nameMin;
        this.nameMax = nameMax;
        this.nameVal = nameVal;
        this.min = min;
        this.max = max;
        this.val = val;
        this.top = 0;
        this.down = 0;
    }

    public StyledOptionsSliderDrawer(string nameMin, string nameMax, string nameVal, float min, float max, float val, float top, float down)
    {
        this.nameMin = nameMin;
        this.nameMax = nameMax;
        this.nameVal = nameVal;
        this.min = min;
        this.max = max;
        this.val = val;
        this.top = top;
        this.down = down;
    }

    public override void OnGUI(Rect position, MaterialProperty prop, String label, MaterialEditor editor)
    {
        var internalPropMin = MaterialEditor.GetMaterialProperty(editor.targets, nameMin);
        var internalPropMax = MaterialEditor.GetMaterialProperty(editor.targets, nameMax);
        var internalPropVal = MaterialEditor.GetMaterialProperty(editor.targets, nameVal);

        if (internalPropMin.displayName != null && internalPropMax.displayName != null && internalPropVal.displayName != null)
        {
            var stylePopup = new GUIStyle(EditorStyles.popup)
            {
                fontSize = 9,
            };

            var styleButton = new GUIStyle(EditorStyles.label)
            {

            };

            var internalValueMin = internalPropMin.floatValue;
            var internalValueMax = internalPropMax.floatValue;
            var internalValueVal = internalPropVal.floatValue;
            Vector4 propVector = prop.vectorValue;

            EditorGUI.BeginChangeCheck();

            if (propVector.w == 2)
            {
                propVector.x = min;
                propVector.y = max;
                propVector.z = internalValueVal;
            }
            else
            {
                if (internalValueMin <= internalValueMax)
                {
                    propVector.w = 0;
                }
                else if (internalValueMin > internalValueMax)
                {
                    propVector.w = 1;
                }

                if (propVector.w == 0)
                {
                    propVector.x = internalValueMin;
                    propVector.y = internalValueMax;
                }
                else
                {
                    propVector.x = internalValueMax;
                    propVector.y = internalValueMin;
                }

                propVector.z = val;
            }

            GUILayout.Space(top);

            EditorGUI.showMixedValue = prop.hasMixedValue;

            GUILayout.BeginHorizontal();

            if (GUILayout.Button(label, styleButton, GUILayout.Width(EditorGUIUtility.labelWidth), GUILayout.Height(18)))
            {
                showAdvancedOptions = !showAdvancedOptions;
            }

            if (propVector.w == 2)
            {
                propVector.z = GUILayout.HorizontalSlider(propVector.z, min, max);
            }
            else
            {
                EditorGUILayout.MinMaxSlider(ref propVector.x, ref propVector.y, min, max);
            }

            GUILayout.Space(2);

            propVector.w = (float)EditorGUILayout.Popup((int)propVector.w, new string[] { "Remap", "Invert", "Simple"}, stylePopup, GUILayout.Width(50));

            GUILayout.EndHorizontal();

            if (showAdvancedOptions)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Space(-1);
                GUILayout.Label("      Remap Min", GUILayout.Width(EditorGUIUtility.labelWidth));
                propVector.x = EditorGUILayout.Slider(propVector.x, min, max);
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Space(-1);
                GUILayout.Label("      Remap Max", GUILayout.Width(EditorGUIUtility.labelWidth));
                propVector.y = EditorGUILayout.Slider(propVector.y, min, max);
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Space(-1);
                GUILayout.Label("      Simple Value", GUILayout.Width(EditorGUIUtility.labelWidth));
                propVector.z = EditorGUILayout.Slider(propVector.z, min, max);
                GUILayout.EndHorizontal();
            }

            if (propVector.w == 0f)
            {
                internalValueMin = propVector.x;
                internalValueMax = propVector.y;
                internalValueVal = val;
            }
            else if (propVector.w == 1f)
            {
                internalValueMin = propVector.y;
                internalValueMax = propVector.x;
                internalValueVal = val;
            }
            else if (propVector.w == 2f)
            {
                internalValueMin = min;
                internalValueMax = max;
                internalValueVal = propVector.z;
            }

            EditorGUI.showMixedValue = false;

            if (EditorGUI.EndChangeCheck())
            {
                prop.vectorValue = propVector;
                internalPropMin.floatValue = internalValueMin;
                internalPropMax.floatValue = internalValueMax;
                internalPropVal.floatValue = internalValueVal;
            }

            GUILayout.Space(down);
        }
    }

    public override float GetPropertyHeight(MaterialProperty prop, string label, MaterialEditor editor)
    {
        return -2;
    }
}