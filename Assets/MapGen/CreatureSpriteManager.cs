using RemoteFortressReader;
using System;
using System.Collections.Generic;
using System.IO;
using TokenLists;
using UnityEngine;
using UnityEngine.Assertions;

[Serializable]
public class CreatureSpriteManager
{
    Dictionary<string, int> tilePageIndices = new Dictionary<string, int>();
    [SerializeField]
    List<TilePage> tilePages = new List<TilePage>();
    List<Material> mats = new List<Material>();

    CreatureRaceMatcher<ProfessionMatcher<TileDef>> creatureMatcher = new CreatureRaceMatcher<ProfessionMatcher<TileDef>>();

    public Material baseCreatureMaterial;

    public bool getCreatureSprite(UnitDefinition unit, out Material mat, out int index, out bool colored)
    {
        ProfessionMatcher<TileDef> prof;
        if (!creatureMatcher.TryGetValue(unit.race, out prof))
        {
            mat = null;
            index = 0;
            colored = true;
            return false;
        }
        TileDef def = new TileDef(-1,-1,true);
        bool set = false;
        foreach (var item in unit.noble_positions)
        {
            if(prof.TryGetValue(item, out def))
            {
                set = true;
                break;
            }
        }
        if(!set)
        {
            prof.TryGetValue((DF.Enums.profession)unit.profession_id, out def);
            set = (def.page != -1);
        }
        if(!set || def.page == -1)
        {
            mat = null;
            index = 0;
            colored = true;
            return false;
        }
        mat = mats[def.page];
        index = def.index;
        colored = def.colored;
        return true;
    }

    public void ParseGraphics(ref List<RawToken>.Enumerator tokenEnumerator, string path)
    {
        Assert.AreEqual("GRAPHICS", tokenEnumerator.Current.Parameters[0]);
        bool rawLeft = true;
        while (rawLeft)
        {
            switch (tokenEnumerator.Current.Token)
            {
                case "TILE_PAGE":
                    rawLeft = ParseTilePage(ref tokenEnumerator, path);
                    break;
                case "CREATURE_GRAPHICS":
                    rawLeft = ParseGreatureGraphics(ref tokenEnumerator);
                    break;
                default:
                    rawLeft = tokenEnumerator.MoveNext();
                    break;
            }
        }
    }

    private bool ParseGreatureGraphics(ref List<RawToken>.Enumerator tokenEnumerator)
    {
        Assert.AreEqual("CREATURE_GRAPHICS", tokenEnumerator.Current.Token);
        string raceToken = tokenEnumerator.Current.Parameters[0];
        MatPairStruct creatureID;
        bool raceRecognized = true;
        if(!CreatureTokenList.TryGetCasteID(raceToken, out creatureID))
        {
            raceRecognized = false;
        }
        ProfessionMatcher<TileDef> professionCollection = null;
        if (raceRecognized)
        {
            if (!creatureMatcher.TryGetValue(creatureID, out professionCollection))
            {
                professionCollection = new ProfessionMatcher<TileDef>(new TileDef(-1, -1, true));
                creatureMatcher[creatureID] = professionCollection;
            }
        }


        bool rawLeft = true;
        while (rawLeft = tokenEnumerator.MoveNext())
        {
            var token = tokenEnumerator.Current;
            switch (token.Token)
            {
                case "CREATURE_GRAPHICS":
                case "TILE_PAGE":
                    goto loopExit;
                default:
                    break;
            }
            if (raceRecognized)
            {
                if (!tilePageIndices.ContainsKey(token.Parameters[0]))
                {
                    Debug.LogWarning("Could not find tile page for: " + token);
                    continue;
                }
                int pageIndex = tilePageIndices[token.Parameters[0]];
                int pagesubIndex =
                    tilePages[pageIndex].AddTilePage(
                        new DFHack.DFCoord2d(
                            int.Parse(token.Parameters[1]),
                            int.Parse(token.Parameters[2])
                            )
                        );
                bool colored = token.Parameters[3] == "ADD_COLOR";
                TileDef tile = new TileDef(pageIndex, pagesubIndex, colored);
                professionCollection[token.Token] = tile;
            }
        }
    loopExit:
        return rawLeft;
    }

    private bool ParseTilePage(ref List<RawToken>.Enumerator tokenEnumerator, string path)
    {
        Assert.AreEqual("TILE_PAGE", tokenEnumerator.Current.Token);

        string pageName = tokenEnumerator.Current.Parameters[0];

        path = Path.GetDirectoryName(path);
        int tileWidth = 0;
        int tileHeight = 0;
        int pageWidth = 0;
        int pageHeight = 0;

        bool rawLeft = true;
        while (rawLeft = tokenEnumerator.MoveNext())
        {
            var token = tokenEnumerator.Current;
            switch (tokenEnumerator.Current.Token)
            {
                case "CREATURE_GRAPHICS":
                case "TILE_PAGE":
                    goto loopExit;
                case "FILE":
                    path = Path.Combine(path, token.Parameters[0]);
                    break;
                case "TILE_DIM":
                    tileWidth = int.Parse(token.Parameters[0]);
                    tileHeight = int.Parse(token.Parameters[1]);
                    break;
                case "PAGE_DIM":
                    pageWidth = int.Parse(token.Parameters[0]);
                    pageHeight = int.Parse(token.Parameters[1]);
                    break;
                default:
                    break;
            }
        }
        loopExit:
        TilePage page = new TilePage(path, pageName, tileWidth, tileHeight, pageWidth, pageHeight);
        tilePageIndices[pageName] = tilePages.Count;
        tilePages.Add(page);

        return rawLeft;
    }

    public void FinalizeSprites()
    {
        int count = 0;
        foreach (var page in tilePages)
        {
            page.FinalizeTextures();
            count += page.Count;
            var mat = new Material(baseCreatureMaterial);
            mat.SetTexture("_MatTex", page.TileArray);
            mat.SetTexture("_BumpMap", page.NormalArray);
            mats.Add(mat);
        }
        Debug.LogFormat("Loaded {0} creature sprites.", count);
    }
}
