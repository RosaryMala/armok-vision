using DF.Enums;
using RemoteFortressReader;
using System;
using System.Collections;
using System.Collections.Generic;
using UnitFlags;
using UnityEngine;

public class LayeredSpriteManager : MonoBehaviour
{
    CreatureRaceMatcher<Dictionary<CreatureSpriteCollection.Special, Dictionary<profession, CreatureSpriteCollection>>> spriteSets
        = new CreatureRaceMatcher<Dictionary<CreatureSpriteCollection.Special, Dictionary<profession, CreatureSpriteCollection>>>();

    public static LayeredSpriteManager Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    // Use this for initialization
    void Start()
    {
        DFConnection.RegisterConnectionCallback(LoadLayeredSpriteSets);
    }

    private void LoadLayeredSpriteSets()
    {
        var spriteSetList = Resources.LoadAll<CreatureSpriteCollection>("Creatures");
        foreach (var spriteSet in spriteSetList)
        {
            MatPairStruct raceID = new MatPairStruct(-1, -1);
            //With generated creatures, the same sprite can potentially match more than one creature.
            var raceRawList = GeneratedCreatureTranslator.GetCreatureRaw(spriteSet.race);
            if (raceRawList == null)
                continue;
            foreach (var raceRaw in raceRawList)
            {
                raceID = new MatPairStruct(raceRaw.index, -1);
                if (!string.IsNullOrEmpty(spriteSet.caste))
                {
                    var casteRaw = raceRaw.caste.Find(x => x.caste_id == spriteSet.caste);
                    if (casteRaw == null)
                        continue;
                    raceID = new MatPairStruct(raceID.mat_type, casteRaw.index);
                }
                if (!spriteSets.ContainsKey(raceID))
                    spriteSets[raceID] = new Dictionary<CreatureSpriteCollection.Special, Dictionary<profession, CreatureSpriteCollection>>();
                if (!spriteSets[raceID].ContainsKey(spriteSet.special))
                    spriteSets[raceID][spriteSet.special] = new Dictionary<profession, CreatureSpriteCollection>();
                spriteSets[raceID][spriteSet.special][spriteSet.profession] = spriteSet;
            }
        }
    }

    public CreatureSpriteCollection GetCreatureSprite(UnitDefinition unit)
    {
        Dictionary<CreatureSpriteCollection.Special, Dictionary<profession, CreatureSpriteCollection>> raceSprites;
        if (!spriteSets.TryGetValue(unit.race, out raceSprites))
            return null;
        CreatureSpriteCollection.Special special = CreatureSpriteCollection.Special.Normal;
        UnitFlags3 flags3 = (UnitFlags3)unit.flags3;
        if ((flags3 & UnitFlags3.ghostly) == UnitFlags3.ghostly)
            special = CreatureSpriteCollection.Special.Ghost;
        else if (unit.inventory.FindIndex(x => x.mode == InventoryMode.Weapon) >= 0)
            special = CreatureSpriteCollection.Special.Military;
        Dictionary<profession, CreatureSpriteCollection> specialSprites;
        if (!raceSprites.TryGetValue(special, out specialSprites))
            if (!raceSprites.TryGetValue(CreatureSpriteCollection.Special.Normal, out specialSprites))
                return null;
        CreatureSpriteCollection professionSprites;
        if (!specialSprites.TryGetValue((profession)unit.profession_id, out professionSprites))
            if (!specialSprites.TryGetValue(profession.NONE, out professionSprites))
                return null;
        return professionSprites;
    }
}
