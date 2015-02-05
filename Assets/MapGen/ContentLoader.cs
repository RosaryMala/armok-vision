using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;
using UnityEngine;
using RemoteFortressReader;

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

public class ContentLoader
{

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


    public ContentConfiguration<ColorContent> colorConfiguration { get; private set; }
    public ContentConfiguration<IndexContent> materialTextureConfiguration { get; private set; }
    public ContentConfiguration<IndexContent> tileTextureConfiguration { get; private set; }



    //public ContentLoader()
    //{
    //    colorConfiguration = new MaterialConfiguration<ColorContent>();
    //    colorConfiguration.nodeName = "color";
    //    materialTextureConfiguration = new MaterialConfiguration<IndexContent>();
    //    tileTextureConfiguration = new TileConfiguration<IndexContent>();
    //}


    public bool ParseContentIndexFile(string path)
    {
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
            catch(Exception)
            {
                continue; //Todo: Make an error message here
            }
            switch (Path.GetExtension(filePath))
            {
                case ".txt":
                    if (!ParseContentIndexFile(filePath))
                        break; //Todo: replace with an error message
                    break;
                case ".xml":
                    if (!ParseContentXMLFile(filePath))
                        break; //Todo: replace with an error message
                    break;
                default:
                    break;
            }
        }
        return true;
    }

    bool ParseContentXMLFile(string path)
    {
        bool runningResult = true;
        XElement doc = XElement.Load(path, LoadOptions.SetBaseUri);
        while (doc != null)
        {
            switch (doc.Name.LocalName)
            {
                case "colors":
                    if(colorConfiguration == null)
                        colorConfiguration = ContentConfiguration<ColorContent>.GetFromRootElement(doc, "color");
                    colorConfiguration.AddSingleContentConfig(doc);
                    break;
                case "materialTextures":
                    if(materialTextureConfiguration == null)
                        materialTextureConfiguration = ContentConfiguration<IndexContent>.GetFromRootElement(doc, "materialTexture");
                    materialTextureConfiguration.AddSingleContentConfig(doc);
                    break;
                case "tileTextures":
                    if(tileTextureConfiguration == null)
                        tileTextureConfiguration = ContentConfiguration<IndexContent>.GetFromRootElement(doc, "tileTexture");
                    tileTextureConfiguration.AddSingleContentConfig(doc);
                    break;
                //case "tileMeshes":
                //    tileMeshConfiguration.AddSingleTiletypeConfig(doc);
                //    break;
                default:
                    break;
            }
            doc = doc.NextNode as XElement;
        }
        return runningResult;
    }

}
