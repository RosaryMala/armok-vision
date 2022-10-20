#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using Boxophobic.StyledGUI;

public class StyledLabelGizmo : StyledMonoBehaviour
{
#if UNITY_2021_2_OR_NEWER
    [StyledMessage("Warning", "The styled label gizmo is not supported in Unity 2021.2 or newer!", 10, 10)]
    public bool message = true;
#endif

    public Transform basePosition;
    public Transform labelPosition;

    [Space(10)]
    public Object pingObject;

    public enum LabelAnchor
    {
        Center = 10,
        Left = 20,
        Right = 30,
    }

    [Space(10)]
    public LabelAnchor labelAnchor = LabelAnchor.Center;

    [TextArea]
    public string labelText;

#if !UNITY_2021_2_OR_NEWER
    bool pingable;

    void OnDrawGizmos()
    {
        var styleLabel = new GUIStyle(EditorStyles.whiteLabel)
        {
            richText = true,
            alignment = UnityEngine.TextAnchor.MiddleLeft,
            fontSize = 9,
        };

        if (basePosition == null)
        {
            basePosition = transform;
        }

        if (labelPosition == null)
        {
            labelPosition = transform;
        }

        var label = gameObject.name;

        if (labelText != null && labelText.Length != 0)
        {
            label = labelText;
        }

        var size = styleLabel.CalcSize(new GUIContent(label));
        var offset = 0f;

        if (labelAnchor == LabelAnchor.Right)
        {
            offset = size.x + 6;
        }
        else if (labelAnchor == LabelAnchor.Center)
        {
            offset = (size.x + 6) / 2;
        }

        Handles.color = Color.black;
        GUI.color = Color.white;

        Handles.DrawLine(basePosition.position, labelPosition.position);

        Handles.BeginGUI();

        var basePos2D = HandleUtility.WorldToGUIPoint(basePosition.position);
        var labelPos2D = HandleUtility.WorldToGUIPoint(labelPosition.position);

        Handles.DrawSolidRectangleWithOutline(new Rect(labelPos2D.x - offset, labelPos2D.y - 24, size.x + 10, size.y + 10), Color.black, new Color(0, 0, 0, 0));

        if (pingObject != null)
        {
            Event e = Event.current;
            var mousePos = e.mousePosition;

            if (mousePos.x > labelPos2D.x - offset && mousePos.x < labelPos2D.x - offset + size.x + 8 && mousePos.y > labelPos2D.y - 24 && mousePos.y < labelPos2D.y - 24 + size.y + 8)
            {
                GUI.color = new Color(0.9f, 0.8f, 0.3f, 1f);
                //GUI.color = new Color(0.0f, 1f, 0.6f, 1f);

                if (pingable && e.modifiers != EventModifiers.Alt)
                {
                    EditorGUIUtility.PingObject(pingObject);
                    pingable = false;
                }

                //if (e.button == 0 && e.isMouse && e.modifiers != EventModifiers.Alt)
                //{
                //    EditorGUIUtility.PingObject(pingObject);
                //}
            }
            else
            {
                pingable = true;
            }
        }

        GUI.Label(new Rect(labelPos2D.x + 4 - offset, labelPos2D.y - 20, size.x, size.y), label, styleLabel);

        Handles.EndGUI();
    }
#endif
}
#endif