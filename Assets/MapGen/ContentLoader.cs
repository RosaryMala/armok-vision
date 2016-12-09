using hqx;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;
using UnityEngine;
using UnityEngine.UI;

public enum MatBasic
{
    INVALID = -1,
    INORGANIC = 0,
    AMBER = 1,
    CORAL = 2,
    GREEN_GLASS = 3,
    CLEAR_GLASS = 4,
    CRYSTAL_GLASS = 5,
    ICE = 6,
    COAL = 7,
    POTASH = 8,
    ASH = 9,
    PEARLASH = 10,
    LYE = 11,
    MUD = 12,
    VOMIT = 13,
    SALT = 14,
    FILTH = 15,
    FILTH_FROZEN = 16,
    UNKOWN_FROZEN = 17,
    GRIME = 18,
    ICHOR = 20,
    LEATHER = 37,
    BLOOD_1 = 39,
    BLOOD_2 = 40,
    BLOOD_3 = 41,
    BLOOD_4 = 42,
    BLOOD_5 = 43,
    BLOOD_6 = 44,
    BLOOD_NAMED = 242,
    PLANT = 419,
    WOOD = 420,
    PLANTCLOTH = 421,

    // filthy hacks to get interface stuff
    DESIGNATION = 422,
    CONSTRUCTION = 423,

}

public class ContentLoader : MonoBehaviour
{
    public void Start()
    {
        DFConnection.RegisterConnectionCallback(Initialize);
    }

    public Text statusText;

    public static ContentLoader Instance { get; private set; }

    public static MatBasic lookupMaterialType(string value)
    {
        if (value == null)
            return MatBasic.INVALID;
        switch (value)
        {
            case "Stone":
                return MatBasic.INORGANIC;
            case "Metal":
                return MatBasic.INORGANIC;
            case "Inorganic":
                return MatBasic.INORGANIC;
            case "GreenGlass":
                return MatBasic.GREEN_GLASS;
            case "Wood":
                return MatBasic.WOOD;
            case "Plant":
                return MatBasic.PLANT;
            case "Ice":
                return MatBasic.ICE;
            case "ClearGlass":
                return MatBasic.CLEAR_GLASS;
            case "CrystalGlass":
                return MatBasic.CRYSTAL_GLASS;
            case "PlantCloth":
                return MatBasic.PLANTCLOTH;
            case "Leather":
                return MatBasic.LEATHER;
            case "Vomit":
                return MatBasic.VOMIT;
            case "Designation":
                return MatBasic.DESIGNATION;
            case "Construction":
                return MatBasic.CONSTRUCTION;
            default:
                return MatBasic.INVALID;
        }
    }

    TextureStorage materialTextureStorage;
    TextureStorage shapeTextureStorage;
    TextureStorage specialTextureStorage;
    MaterialMatcher<ColorContent> materialColors;
    MaterialMatcher<TextureContent> materialTextures;

    public int DefaultMatTexIndex { get; private set; }

    public Matrix4x4 DefaultMatTexTransform
    {
        get
        {
            return materialTextureStorage.getUVTransform(DefaultMatTexIndex);
        }
    }
    public float DefaultMatTexArrayIndex
    {
        get
        {
            return (float)DefaultMatTexIndex / materialTextureStorage.Count;
        }
    }

    public int DefaultShapeTexIndex { get; private set; }

    public Matrix4x4 DefaultShapeTexTransform
    {
        get
        {
            return shapeTextureStorage.getUVTransform(DefaultShapeTexIndex);
        }
    }
    public float DefaultShapeTexArrayIndex
    {
        get
        {
            return (float)DefaultShapeTexIndex / shapeTextureStorage.Count;
        }
    }

    int defaultSpecialTexIndex;

    public Matrix4x4 DefaultSpecialTexTransform
    {
        get
        {
            return specialTextureStorage.getUVTransform(defaultSpecialTexIndex);
        }
    }
    public float DefaultSpecialTexArrayIndex
    {
        get
        {
            return (float)defaultSpecialTexIndex / specialTextureStorage.Count;
        }
    }

    public TileConfiguration<ColorContent> ColorConfiguration { get; private set; }
    public TileConfiguration<TextureContent> MaterialTextureConfiguration { get; private set; }
    public TileConfiguration<NormalContent> ShapeTextureConfiguration { get; private set; }
    public TileConfiguration<NormalContent> TerrainShapeTextureConfiguration { get; private set; }
    public TileConfiguration<MeshContent> TileMeshConfiguration { get; private set; }
    public TileConfiguration<MeshContent> GrowthMeshConfiguration { get; private set; }
    public TileConfiguration<LayerContent> MaterialLayerConfiguration { get; private set; }
    public TileConfiguration<MeshContent> BuildingMeshConfiguration { get; private set; }
    public TileConfiguration<NormalContent> BuildingShapeTextureConfiguration { get; private set; }
    public CreatureConfiguration<MeshContent> CreatureMeshConfiguration { get; private set; }
    public TileConfiguration<MeshContent> DesignationMeshConfiguration { get; private set; }
    public TileConfiguration<MeshContent> CollisionMeshConfiguration { get; private set; }
    public TileConfiguration<MeshContent> BuildingCollisionMeshConfiguration { get; private set; }
    public TileConfiguration<MeshContent> ItemMeshConfiguration { get; private set; }

    public void Awake()
    {
        materialTextureStorage = new TextureStorage();
        shapeTextureStorage = new TextureStorage();
        specialTextureStorage = new TextureStorage();
        materialColors = new MaterialMatcher<ColorContent>();
        materialTextures = new MaterialMatcher<TextureContent>();



        DefaultMatTexIndex = materialTextureStorage.AddTexture(CreateFlatTexture(new Color(0.5f, 0.5f, 0.5f, 0)));
        DefaultShapeTexIndex = shapeTextureStorage.AddTexture(CreateFlatTexture(new Color(1f, 0.5f, 1f, 0.5f)));
        defaultSpecialTexIndex = specialTextureStorage.AddTexture(Texture2D.blackTexture);
    }

    public static Texture2D CreateFlatTexture(Color color)
    {
        Texture2D tex = new Texture2D(4, 4);
        var pix = tex.GetPixels();
        for (int i = 0; i < pix.Length; i++)
        {
            pix[i] = color;
        }
        tex.SetPixels(pix);
        tex.Apply();
        return tex;
    }
    public static Texture2D LoadTexture(XAttribute textureAtt, XElement elemtype, Color defaultColor, bool linear = false)
    {
        if (textureAtt == null)
            return CreateFlatTexture(defaultColor);

        string texturePath = Path.Combine(Path.GetDirectoryName(new Uri(elemtype.BaseUri).LocalPath), textureAtt.Value);
        texturePath = Path.GetFullPath(texturePath);
        if (!File.Exists(texturePath))
        {
            Debug.LogError("File not found: " + texturePath);
            return CreateFlatTexture(defaultColor);
        }

        byte[] patternData = File.ReadAllBytes(texturePath);
        Texture2D texture = new Texture2D(2, 2, TextureFormat.ARGB32, false, linear);
        texture.LoadImage(patternData);
        GameSettings.ClampToMaxSize(texture);
        texture.name = texturePath;
        if (texture.width * 4 <= GameSettings.Instance.rendering.maxTextureSize && texture.height * 4 <= GameSettings.Instance.rendering.maxTextureSize)
        {
            texture = HqxSharp.Scale4(texture);
        }
        else if (texture.width * 3 <= GameSettings.Instance.rendering.maxTextureSize && texture.height * 3 <= GameSettings.Instance.rendering.maxTextureSize)
        {
            texture = HqxSharp.Scale3(texture);
        }
        else if (texture.width * 2 <= GameSettings.Instance.rendering.maxTextureSize && texture.height * 2 <= GameSettings.Instance.rendering.maxTextureSize)
        {
            texture = HqxSharp.Scale2(texture);
        }
        return texture;
    }


    public void Initialize()
    {
        StartCoroutine(LoadAssets());
    }

    IEnumerator LoadAssets()
    {
        System.Diagnostics.Stopwatch watch = new System.Diagnostics.Stopwatch();
        watch.Start();
        yield return StartCoroutine(ParseContentIndexFile(Application.streamingAssetsPath + "/index.txt"));
        yield return StartCoroutine(FinalizeTextureAtlases());
        Instance = this;
        watch.Stop();
        Debug.Log("Took a total of " + watch.ElapsedMilliseconds + "ms to load all XML files.");
        Debug.Log(string.Format("loaded {0} meshes, {1} pattern textures, {2} colors, and {3} shape textures.", MeshContent.NumCreated, TextureContent.NumCreated, ColorContent.NumCreated, "XXX"));
        statusText.gameObject.SetActive(false);
        yield return null;
    }


    IEnumerator ParseContentIndexFile(string path)
    {
        if (statusText != null)
            statusText.text = "Loading Index File: " + path;

        string line;
        List<string> fileArray = new List<string>(); //This allows us to parse the file in reverse.
        StreamReader file = new StreamReader(path);
        while ((line = file.ReadLine()) != null)
        {
            line = line.Trim(); //remove trailing spaces
            if (string.IsNullOrEmpty(line))
                continue;
            if (line[0] == '#') //Allow comments
                continue;

            fileArray.Add(string.Copy(line));
        }
        file.Close();
        string filePath;
        for (int i = fileArray.Count - 1; i >= 0; i--)
        {
            try
            {
                filePath = Path.Combine(Path.GetDirectoryName(path), fileArray[i]);
            }
            catch (Exception)
            {
                continue; //Todo: Make an error message here
            }
            switch (Path.GetExtension(filePath))
            {
                case ".txt":
                    yield return StartCoroutine(ParseContentIndexFile(filePath));
                    break;
                case ".xml":
                    yield return StartCoroutine(ParseContentXMLFile(filePath));
                    break;
                default:
                    break;
            }
        }
        yield return null;
    }

    IEnumerator ParseContentXMLFile(string path)
    {
        if(statusText != null)
            statusText.text = "Loading XML File: " + path;
        XElement doc = XElement.Load(path, LoadOptions.SetBaseUri | LoadOptions.SetLineInfo);
        while (doc != null)
        {
            switch (doc.Name.LocalName)
            {
                case "colors":
                    if (ColorConfiguration == null)
                        ColorConfiguration = TileConfiguration<ColorContent>.GetFromRootElement(doc, "color");
                    ColorConfiguration.AddSingleContentConfig(doc, null, materialColors);
                    break;
                case "materialTextures":
                    if (MaterialTextureConfiguration == null)
                        MaterialTextureConfiguration = TileConfiguration<TextureContent>.GetFromRootElement(doc, "materialTexture");
                    MaterialTextureConfiguration.AddSingleContentConfig(doc, materialTextureStorage, materialTextures);
                    break;
                case "shapeTextures":
                    if (ShapeTextureConfiguration == null)
                        ShapeTextureConfiguration = TileConfiguration<NormalContent>.GetFromRootElement(doc, "shapeTexture");
                    ShapeTextureConfiguration.AddSingleContentConfig(doc, shapeTextureStorage);
                    break;
                case "terrainTextures":
                    if (TerrainShapeTextureConfiguration == null)
                        TerrainShapeTextureConfiguration = TileConfiguration<NormalContent>.GetFromRootElement(doc, "terrainTexture");
                    TerrainShapeTextureConfiguration.AddSingleContentConfig(doc, shapeTextureStorage);
                    break;
                case "tileMeshes":
                    if (TileMeshConfiguration == null)
                        TileMeshConfiguration = TileConfiguration<MeshContent>.GetFromRootElement(doc, "tileMesh");
                    TileMeshConfiguration.AddSingleContentConfig(doc, new MeshContent.TextureStorageContainer(materialTextureStorage, shapeTextureStorage, specialTextureStorage));
                    break;
                case "materialLayers":
                    if (MaterialLayerConfiguration == null)
                        MaterialLayerConfiguration = TileConfiguration<LayerContent>.GetFromRootElement(doc, "materialLayer");
                    MaterialLayerConfiguration.AddSingleContentConfig(doc);
                    break;
                case "buildingMeshes":
                    if (BuildingMeshConfiguration == null)
                        BuildingMeshConfiguration = TileConfiguration<MeshContent>.GetFromRootElement(doc, "buildingMesh");
                    BuildingMeshConfiguration.AddSingleContentConfig(doc, new MeshContent.TextureStorageContainer(materialTextureStorage, shapeTextureStorage, specialTextureStorage));
                    break;
                case "buildingShapeTextures":
                    if (BuildingShapeTextureConfiguration == null)
                        BuildingShapeTextureConfiguration = TileConfiguration<NormalContent>.GetFromRootElement(doc, "buildingShapeTexture");
                    BuildingShapeTextureConfiguration.AddSingleContentConfig(doc, shapeTextureStorage);
                    break;
                case "creatureMeshes":
                    if (CreatureMeshConfiguration == null)
                        CreatureMeshConfiguration = CreatureConfiguration<MeshContent>.GetFromRootElement(doc, "creatureMesh");
                    CreatureMeshConfiguration.AddSingleContentConfig(doc, new MeshContent.TextureStorageContainer(materialTextureStorage, shapeTextureStorage, specialTextureStorage));
                    break;
                case "growthMeshes":
                    if (GrowthMeshConfiguration == null)
                        GrowthMeshConfiguration = TileConfiguration<MeshContent>.GetFromRootElement(doc, "growthMesh");
                    GrowthMeshConfiguration.AddSingleContentConfig(doc, new MeshContent.TextureStorageContainer(materialTextureStorage, shapeTextureStorage, specialTextureStorage));
                    break;
                case "designationMeshes":
                    if (DesignationMeshConfiguration == null)
                        DesignationMeshConfiguration = TileConfiguration<MeshContent>.GetFromRootElement(doc, "designationMesh");
                    DesignationMeshConfiguration.AddSingleContentConfig(doc, new MeshContent.TextureStorageContainer(materialTextureStorage, shapeTextureStorage, specialTextureStorage));
                    break;
                case "collisionMeshes":
                    if (CollisionMeshConfiguration == null)
                        CollisionMeshConfiguration = TileConfiguration<MeshContent>.GetFromRootElement(doc, "collisionMesh");
                    CollisionMeshConfiguration.AddSingleContentConfig(doc, new MeshContent.TextureStorageContainer(materialTextureStorage, shapeTextureStorage, specialTextureStorage));
                    break;
                case "buildingCollisionMeshes":
                    if (BuildingCollisionMeshConfiguration == null)
                        BuildingCollisionMeshConfiguration = TileConfiguration<MeshContent>.GetFromRootElement(doc, "buildingCollisionMesh");
                    BuildingCollisionMeshConfiguration.AddSingleContentConfig(doc, new MeshContent.TextureStorageContainer(materialTextureStorage, shapeTextureStorage, specialTextureStorage));
                    break;
                case "itemMeshes":
                    if (ItemMeshConfiguration == null)
                        ItemMeshConfiguration = TileConfiguration<MeshContent>.GetFromRootElement(doc, "itemMesh");
                    ItemMeshConfiguration.AddSingleContentConfig(doc, null);
                    break;
                default:
                    break;
            }
            doc = doc.NextNode as XElement;
        }
        yield return null;
    }

    IEnumerator FinalizeTextureAtlases()
    {
        if (statusText != null)
            statusText.text = "Building material textures...";
        yield return null;
        materialTextureStorage.CompileTextures("MaterialTexture");
        if (statusText != null)
            statusText.text = "Building shape textures...";
        yield return null;
        shapeTextureStorage.CompileTextures("ShapeTexture", TextureFormat.RGBA32, new Color(1.0f, 0.5f, 0.0f, 0.5f), true);
        if (statusText != null)
            statusText.text = "Building special textures...";
        yield return null;
        specialTextureStorage.CompileTextures("SpecialTexture");

        Vector4 arrayCount = new Vector4(materialTextureStorage.Count, shapeTextureStorage.Count, specialTextureStorage.Count);

        GameMap gameMap = FindObjectOfType<GameMap>();
        gameMap.BasicTerrainMaterial.SetTexture("_MainTex", materialTextureStorage.AtlasTexture);
        gameMap.BasicTerrainMaterial.SetTexture("_BumpMap", shapeTextureStorage.AtlasTexture);
        gameMap.BasicTerrainMaterial.SetTexture("_SpecialTex", specialTextureStorage.AtlasTexture);
        gameMap.BasicTerrainMaterial.SetVector("_TexArrayCount", arrayCount);
        gameMap.StencilTerrainMaterial.SetTexture("_MainTex", materialTextureStorage.AtlasTexture);
        gameMap.StencilTerrainMaterial.SetTexture("_BumpMap", shapeTextureStorage.AtlasTexture);
        gameMap.StencilTerrainMaterial.SetTexture("_SpecialTex", specialTextureStorage.AtlasTexture);
        gameMap.StencilTerrainMaterial.SetVector("_TexArrayCount", arrayCount);
        gameMap.TransparentTerrainMaterial.SetTexture("_MainTex", materialTextureStorage.AtlasTexture);
        gameMap.TransparentTerrainMaterial.SetTexture("_BumpMap", shapeTextureStorage.AtlasTexture);
        gameMap.TransparentTerrainMaterial.SetTexture("_SpecialTex", specialTextureStorage.AtlasTexture);
        gameMap.TransparentTerrainMaterial.SetVector("_TexArrayCount", arrayCount);
        gameMap.voxelTerrainMaterial.SetTexture("_Splat", materialTextureStorage.AtlasTexture);
        gameMap.voxelTerrainMaterial.SetTexture("_Normal", shapeTextureStorage.AtlasTexture);

        //get rid of any un-used textures left over.
        Resources.UnloadUnusedAssets();
        GC.Collect();
        yield return null;
    }

}
