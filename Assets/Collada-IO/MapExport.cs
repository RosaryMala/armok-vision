using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine.UI;
using System.IO;
using RemoteFortressReader;
using Collada141;
using DFHack;

public class MapExport : MonoBehaviour
{
    public Text statusText;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("SaveMeshes") && Input.GetButton("Mod"))
        {
            StartCoroutine(ExportMap("map.dae"));
        }
    }

    private IEnumerator ExportMap(string v)
    {
        if (ContentLoader.Instance == null)
            yield return null;
        statusText.gameObject.SetActive(true);
        int tileCount = 0;
        for (int z = 0; z < MapDataStore.MapSize.z; z++)
        {
            statusText.text = string.Format("Collecting meshes from Z-level {0}/{1}", z, MapDataStore.MapSize.z);
            if (tileCount > 0)
                yield return null;
            tileCount = 0;
            for (int y = 0; y < MapDataStore.MapSize.y; y++)
                for (int x = 0; x < MapDataStore.MapSize.x; x++)
                {
                    var tile = MapDataStore.Main[x, y, z];
                    if (tile == null) continue;
                    for (int i = 0; i < (int)MeshLayer.Count; i++)
                    {
                        CollectModel(tile, (MeshLayer)i, new DFCoord(x,y,z));
                    }
                    tileCount++;
                }
        }
        statusText.text = string.Format("Saving {0} textures to disk.", diffuseTextures.Count);
        yield return null;
        foreach (var item in diffuseTextures)
        {
            var imageBytes = item.Value.EncodeToPNG();
            File.WriteAllBytes(item.Value.name + ".png", imageBytes);
        }
        foreach (var item in specularTextures)
        {
            var imageBytes = item.Value.EncodeToPNG();
            File.WriteAllBytes(item.Value.name + ".png", imageBytes);
        }

        //build the collada file
        COLLADA exportScene = new COLLADA();

        List<object> sceneItems = new List<object>();

        #region Geometry Library
        library_geometries geometryLib = new library_geometries();
        geometryLib.geometry = new geometry[geometryLibrary.Count];
        {
            int geometryIndex = 0;
            foreach (var item in geometryLibrary)
            {
                geometryLib.geometry[geometryIndex] = item.Value;
                geometryIndex++;
            }
        }
        sceneItems.Add(geometryLib);
        #endregion

        #region Add scenes
        library_visual_scenes visualSceneLib = new library_visual_scenes();
        visual_scene visualScene = new visual_scene();

        visualSceneLib.visual_scene = new visual_scene[] { visualScene };

        visualScene.id = "Map";
        visualScene.name = "Map";

        visualScene.node = nodeList.ToArray();
        sceneItems.Add(visualSceneLib);
        #endregion

        exportScene.Items = sceneItems.ToArray();

        COLLADAScene sceneInstance = new COLLADAScene();
        sceneInstance.instance_visual_scene = new InstanceWithExtra();
        sceneInstance.instance_visual_scene.url = "#" + visualScene.id;

        exportScene.scene = sceneInstance;

        statusText.text = "Added geometry to scene";
        yield return null;

        asset assetHeader = new asset();
        assetHeader.unit = new assetUnit();
        assetHeader.unit.meter = 1;
        assetHeader.unit.name = "meter";
        assetHeader.up_axis = UpAxisType.Y_UP;

        exportScene.asset = assetHeader;
        statusText.text = "Saving scene to file.";
        yield return null;

        if (File.Exists("Map.dae"))
            File.Delete("Map.dae");
        exportScene.Save("Map.dae");
        Debug.Log("Saved Scene");

        statusText.gameObject.SetActive(false);
        yield return null;
    }

    Dictionary<string, Texture2D> diffuseTextures = new Dictionary<string, Texture2D>();
    Dictionary<string, Texture2D> specularTextures = new Dictionary<string, Texture2D>();
    Dictionary<string, geometry> geometryLibrary = new Dictionary<string, geometry>();
    List<node> nodeList = new List<node>();

    //this is very similar to the blockmesher function.
    void CollectModel(MapDataStore.Tile tile, MeshLayer layer, DFCoord pos)
    {
        if (layer == MeshLayer.Collision || layer == MeshLayer.BuildingCollision)
            return;

        #region Mesh Selection
        MeshContent meshContent = null;
        switch (layer)
        {
            case MeshLayer.GrowthMaterial:
            case MeshLayer.GrowthMaterial1:
            case MeshLayer.GrowthMaterial2:
            case MeshLayer.GrowthMaterial3:
            case MeshLayer.GrowthCutout:
            case MeshLayer.GrowthCutout1:
            case MeshLayer.GrowthCutout2:
            case MeshLayer.GrowthCutout3:
            case MeshLayer.GrowthTransparent:
            case MeshLayer.GrowthTransparent1:
            case MeshLayer.GrowthTransparent2:
            case MeshLayer.GrowthTransparent3:
                {
                    switch (tile.tiletypeMaterial)
                    {
                        case TiletypeMaterial.PLANT:
                        case TiletypeMaterial.ROOT:
                        case TiletypeMaterial.TREE_MATERIAL:
                        case TiletypeMaterial.MUSHROOM:
                            if (!ContentLoader.Instance.GrowthMeshConfiguration.GetValue(tile, layer, out meshContent))
                                return;
                            break;
                        default:
                            return;
                    }
                }
                break;
            case MeshLayer.BuildingMaterial:
            case MeshLayer.NoMaterialBuilding:
            case MeshLayer.BuildingMaterialCutout:
            case MeshLayer.NoMaterialBuildingCutout:
            case MeshLayer.BuildingMaterialTransparent:
            case MeshLayer.NoMaterialBuildingTransparent:
                {
                    if (tile.buildingType == default(BuildingStruct))
                        return;
                    if (!ContentLoader.Instance.BuildingMeshConfiguration.GetValue(tile, layer, out meshContent))
                        return;
                }
                break;
            default:
                {
                    if (!ContentLoader.Instance.TileMeshConfiguration.GetValue(tile, layer, out meshContent))
                        return;
                }
                break;
        }

        if (!meshContent.MeshData.ContainsKey(layer))
            return;
        #endregion

        node tileNode = new node();

        tileNode.id = string.Format("Tile[{0},{1},{2}]_{3}", pos.x, pos.y, pos.z, layer);

        tileNode.Items = new object[]
        {
            COLLADA.ConvertMatrix(Matrix4x4.TRS(
                GameMap.DFtoUnityCoord(pos),
                meshContent.GetRotation(tile),
                Vector3.one))
        };
        tileNode.ItemsElementName = new ItemsChoiceType2[] { ItemsChoiceType2.matrix};

        string geometryName = "Mesh-" + meshContent.UniqueIndex;

        if(!geometryLibrary.ContainsKey(geometryName))
        {
            geometryLibrary[geometryName] = COLLADA.MeshToGeometry(meshContent.MeshData[layer], geometryName);
        }

        instance_geometry geometryInstance = new instance_geometry();
        geometryInstance.url = "#" + geometryLibrary[geometryName].id;
        tileNode.instance_geometry = new instance_geometry[] { geometryInstance };

        nodeList.Add(tileNode);
        return;
        //-----------------------------------------------------------
        //Put normal map stuff here! Remember!
        //-----------------------------------------------------------


        string patternName = "Tex-";

        Texture2D tiletexture = null;
        TextureContent textureContent;
        if (ContentLoader.Instance.MaterialTextureConfiguration.GetValue(tile, layer, out textureContent))
        {
            tiletexture = textureContent.Texture;
            patternName += textureContent.UniqueIndex;
        }
        else patternName += "#";

        patternName += "-#";

        Color color = Color.grey;
        ColorContent colorContent;
        if (ContentLoader.Instance.ColorConfiguration.GetValue(tile, layer, out colorContent))
        {
            color = colorContent.color;
        }

        patternName += string.Format("{0:X2}{1:X2}{2:X2}", ((Color32)color).r, ((Color32)color).g, ((Color32)color).b);

        if (diffuseTextures.ContainsKey(patternName))
            return;

        Color neutralSpec = new Color(0.04f, 0.04f, 0.04f);
        Texture2D outputDiffuse;
        Texture2D outputSpec;
        if (tiletexture != null)
        {
            outputDiffuse = new Texture2D(tiletexture.width, tiletexture.height);
            outputSpec = new Texture2D(tiletexture.width, tiletexture.height);
            Color[] colors = tiletexture.GetPixels();
            Color[] specs = new Color[colors.Length];
            for (int i = 0; i < colors.Length; i++)
            {
                var diffuseColor = OverlayBlend(colors[i], color);
                diffuseColor.a = 1;
                colors[i] = Color.Lerp(Color.black, diffuseColor, color.a);
                specs[i] = Color.Lerp(diffuseColor, neutralSpec, color.a);
            }
            outputDiffuse.SetPixels(colors);
            outputSpec.SetPixels(specs);
        }
        else
        {
            outputDiffuse = ContentLoader.CreateFlatTexture(color);
            outputSpec = ContentLoader.CreateFlatTexture(neutralSpec);
        }
        outputDiffuse.name = patternName + "_Diffuse";
        outputSpec.name = patternName + "_Specular";

        diffuseTextures[patternName] = outputDiffuse;
        specularTextures[patternName] = outputSpec;

    }


    static float OverlayBlend(float a, float b)
    {
        if (a < b)
            return 2 * a * b;
        else
            return 1 - (2 * (1 - a) * (1 - b));
    }

    static Color OverlayBlend(Color a, Color b)
    {
        return new Color(
            OverlayBlend(a.r, b.r),
            OverlayBlend(a.g, b.g),
            OverlayBlend(a.b, b.b),
            a.a * b.a
            );
    }
}
