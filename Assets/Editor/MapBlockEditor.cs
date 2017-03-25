using UnityEditor;
using UnityEngine;
using System;
using RemoteFortressReader;

[CustomEditor(typeof(MapBlock))]
[CanEditMultipleObjects]
public class MapBlockEditor : Editor
{
    static Color32 selectedColor = Color.white;
    static TiletypeShape selectedShape = TiletypeShape.Wall;
    bool showEditorGrid = false;
    public GameMap parent = null;

    public override void OnInspectorGUI()
    {
        MapBlock[] targetBlocks = Array.ConvertAll(targets, element => (MapBlock)element);
        if(targets.Length == 1)
            EditorGUILayout.LabelField(targets.Length + " Map Block selected.");
        else
            EditorGUILayout.LabelField(targets.Length + " Map Blocks selected.");

        parent = targetBlocks[0].parent;
        parent = (GameMap)EditorGUILayout.ObjectField("Parent", parent, typeof(GameMap), true);
        for (int index = 0; index < targetBlocks.Length; index++)
        {
            targetBlocks[index].parent = parent;
        }

            selectedColor = EditorGUILayout.ColorField("Material Color", selectedColor);
        selectedShape = (TiletypeShape)EditorGUILayout.EnumPopup("Terrain Shape ", selectedShape);
        showEditorGrid = EditorGUILayout.Foldout(showEditorGrid, "Block Tiles");
        if (showEditorGrid)
        {
            EditorGUILayout.BeginVertical();
            DFHack.DFCoord2d tempCoord = new DFHack.DFCoord2d();
            for (int i = 0; i < MapBlock.blockWidthTiles; i++)
            {
                EditorGUILayout.BeginHorizontal();
                for (int j = 0; j < MapBlock.blockWidthTiles; j++)
                {
                    tempCoord.x = j;
                    tempCoord.y = i;
                    Color currentColor = targetBlocks[0].GetColor(tempCoord);
                    for (int index = 1; index < targetBlocks.Length; index++)
                    {
                        if (currentColor != targetBlocks[index].GetColor(tempCoord))
                        {
                            currentColor = Color.white;
                            break;
                        }
                    }
                    currentColor.a = 1.0f;
                    GUI.color = currentColor;
                    string buttonIcon = "\u00A0";
                    TiletypeShape tile = targetBlocks[0].GetSingleTile(tempCoord);
                    for (int index = 1; index < targetBlocks.Length; index++)
                    {
                        if (tile != targetBlocks[index].GetSingleTile(tempCoord))
                        {
                            tile = TiletypeShape.Empty;
                            break;
                        }

                    }
                    switch (tile)
                    {
                        case TiletypeShape.NoShape:
                            buttonIcon = "?";
                            break;
                        case TiletypeShape.Wall:
                            buttonIcon = "▓";
                            break;
                        case TiletypeShape.Floor:
                            buttonIcon = "+";
                            break;
                        case TiletypeShape.Empty:
                            buttonIcon = "\u00A0";
                            break;
                        case TiletypeShape.Ramp:
                            buttonIcon = "▲";
                            break;
                        case TiletypeShape.RampTop:
                            buttonIcon = "▼";
                            break;
                        default:
                            buttonIcon = "?";
                            break;
                    }
                    if (GUILayout.Button(buttonIcon))
                    {
                        for (int index = 0; index < targetBlocks.Length; index++)
                        {
                            targetBlocks[index].SetSingleTile(tempCoord, selectedShape);
                            targetBlocks[index].SetColor(tempCoord, selectedColor);
                            targetBlocks[index].Regenerate();
                            EditorUtility.SetDirty(targetBlocks[index]);
                        }
                    }
                }
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.EndVertical();
        }
        GUI.color = Color.white;
        if (GUILayout.Button("Fill"))
        {
            for (int index = 0; index < targetBlocks.Length; index++)
            {
                for (int i = 0; i < MapBlock.blockWidthTiles; i++)
                    for (int j = 0; j < MapBlock.blockWidthTiles; j++)
                    {
                        DFHack.DFCoord2d here = new DFHack.DFCoord2d(j, i);
                        targetBlocks[index].SetSingleTile(here, selectedShape);
                        targetBlocks[index].SetColor(here, selectedColor);

                    }
                targetBlocks[index].Regenerate();
                EditorUtility.SetDirty(targetBlocks[index]);
            }
        }
    }
}
