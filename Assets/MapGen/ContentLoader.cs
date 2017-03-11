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
    public TileConfiguration<MeshContent> DesignationMeshConfiguration { get; private set; }
    public TileConfiguration<MeshContent> CollisionMeshConfiguration { get; private set; }
    public TileConfiguration<MeshContent> BuildingCollisionMeshConfiguration { get; private set; }
    public TileConfiguration<MeshContent> ItemMeshConfiguration { get; private set; }
    public TileConfiguration<GrassContent> GrassTextureConfiguration { get; private set; }

    [SerializeField]
    private CreatureSpriteManager _spriteManager = new CreatureSpriteManager();
    public CreatureSpriteManager SpriteManager { get { return _spriteManager; } }

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
            HqxSharp.Scale4(texture);
        }
        else if (texture.width * 3 <= GameSettings.Instance.rendering.maxTextureSize && texture.height * 3 <= GameSettings.Instance.rendering.maxTextureSize)
        {
            HqxSharp.Scale3(texture);
        }
        else if (texture.width * 2 <= GameSettings.Instance.rendering.maxTextureSize && texture.height * 2 <= GameSettings.Instance.rendering.maxTextureSize)
        {
            HqxSharp.Scale2(texture);
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
        GameMap.Instance.ShowHelp();
        yield return StartCoroutine(ParseContentIndexFile(Application.streamingAssetsPath + "/index.txt"));
        yield return StartCoroutine(FinalizeTextureAtlases());
        Instance = this;
        watch.Stop();
        Debug.Log("Took a total of " + watch.ElapsedMilliseconds + "ms to load all XML files.");
        Debug.Log(string.Format("loaded {0} meshes, {1} pattern textures, {2} colors, and {3} shape textures.", MeshContent.NumCreated, TextureContent.NumCreated, ColorContent.NumCreated, NormalContent.NumCreated));
        Debug.Log("Loading Complete. Press ESC to change settings or leave feedback. Have a nice day!");
        GameMap.Instance.HideHelp();
        yield return null;
    }


    IEnumerator ParseContentIndexFile(string path)
    {
        Debug.Log("Loading Index File: " + path);

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

            fileArray.Add(line);
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
            if (Directory.Exists(filePath)) //if it's a directory, just parse the contents
            {
                yield return ParseContentDirectory(filePath);
            }
            else
            {
                switch (Path.GetExtension(filePath))
                {
                    case ".txt":
                        StreamReader mightBeRaw = new StreamReader(filePath);
                        //first check if it's a DF raw file.
                        if (mightBeRaw.ReadLine() == Path.GetFileNameWithoutExtension(filePath))
                            yield return StartCoroutine(ParseContentRawFile(filePath));
                        else
                            yield return StartCoroutine(ParseContentIndexFile(filePath));
                        break;
                    case ".xml":
                        yield return StartCoroutine(ParseContentXMLFile(filePath));
                        break;
                    default:
                        break;
                }
            }
        }
        yield return null;
    }

    IEnumerator ParseContentXMLFile(string path)
    {
            Debug.Log("Loading XML File: " + path);
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
                case "grassTextures":
                    if (GrassTextureConfiguration == null)
                        GrassTextureConfiguration = TileConfiguration<GrassContent>.GetFromRootElement(doc, "grassTexture");
                    GrassTextureConfiguration.AddSingleContentConfig(doc, new GrassContent.TextureStorageContainer(materialTextureStorage, shapeTextureStorage));
                    break;
                default:
                    break;
            }
            doc = doc.NextNode as XElement;
        }
        yield return null;
    }

    IEnumerator ParseContentRawFile(string path)
    {
        Debug.Log("Loading Raw File: " + path);

        var tokenList = RawLoader.SplitRawFileText(File.ReadAllText(path));
        var tokenEnumerator = tokenList.GetEnumerator();
        try
        {
            if (tokenEnumerator.MoveNext()) // Because they always start at -1.
            {
                if (tokenEnumerator.Current.Token == "OBJECT")
                {
                    switch (tokenEnumerator.Current.Parameters[0])
                    {
                        case "GRAPHICS":
                            SpriteManager.ParseGraphics(ref tokenEnumerator, path);
                            break;
                        default:
                            Debug.Log("Unhandled Token: " + tokenEnumerator.Current.Parameters[0]);
                            break;
                    }
                }
                else
                    Debug.Log("Unexpected Token: " + tokenEnumerator.Current.Token);
            }
        }
        finally
        {
            tokenEnumerator.Dispose();
        }
        yield return null;
    }

    IEnumerator ParseContentDirectory(string path)
    {
        foreach (var file in Directory.GetFiles(path, "*.txt", SearchOption.AllDirectories))
        {
            StreamReader mightBeRaw = new StreamReader(file);
            //first check if it's a DF raw file.
            if (mightBeRaw.ReadLine() == Path.GetFileNameWithoutExtension(file))
                yield return ParseContentRawFile(file);
        }
        yield return null;
    }

    IEnumerator FinalizeTextureAtlases()
    {
        Debug.Log("Building material textures...");
        yield return null;
        materialTextureStorage.CompileTextures("MaterialTexture");
        Debug.Log("Building shape textures...");
        yield return null;
        shapeTextureStorage.CompileTextures("ShapeTexture", TextureFormat.RGBA32, new Color(1.0f, 0.5f, 0.0f, 0.5f), true);
        Debug.Log("Building special textures...");
        yield return null;
        specialTextureStorage.CompileTextures("SpecialTexture");

        Vector4 arrayCount = new Vector4(materialTextureStorage.Count, shapeTextureStorage.Count, specialTextureStorage.Count);

        MaterialManager.Instance.SetTexture("_MainTex", materialTextureStorage.AtlasTexture);
        MaterialManager.Instance.SetTexture("_BumpMap", shapeTextureStorage.AtlasTexture);
        MaterialManager.Instance.SetTexture("_SpecialTex", specialTextureStorage.AtlasTexture);
        MaterialManager.Instance.SetVector("_TexArrayCount", arrayCount);

        Debug.Log("Finalizing creature sprites");
        yield return null;
        SpriteManager.FinalizeSprites();
        Debug.Log("Done!");

        //get rid of any un-used textures left over.
        Resources.UnloadUnusedAssets();
        GC.Collect();
        yield return null;
    }
}
