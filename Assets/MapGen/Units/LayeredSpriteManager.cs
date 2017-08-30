using DF.Enums;
using RemoteFortressReader;
using System.Collections.Generic;
using UnitFlags;
using UnityEngine;


public class LayeredSpriteManager : MonoBehaviour
{
    class CasteDictionary : Dictionary<int, CreatureSpriteCollection>
    {
        new public bool TryGetValue(int caste, out CreatureSpriteCollection collection)
        {
            if (base.TryGetValue(caste, out collection))
                return true;
            if (base.TryGetValue(-1, out collection))
                return true;
            return false;
        }

        public bool TryGetValue(UnitDefinition unit, out CreatureSpriteCollection collection)
        {
            return TryGetValue(unit.race.mat_index, out collection);
        }
    }
    class SpecialDictionary : Dictionary<CreatureSpriteCollection.Special, CasteDictionary>
    {
        new public bool TryGetValue(CreatureSpriteCollection.Special special, out CasteDictionary dict)
        {
            if (base.TryGetValue(special, out dict))
                return true;
            if (base.TryGetValue(CreatureSpriteCollection.Special.Normal, out dict))
                return true;
            return false;
        }

        public bool TryGetValue(UnitDefinition unit, out CreatureSpriteCollection collection)
        {
            CreatureSpriteCollection.Special special = CreatureSpriteCollection.Special.Normal;
            UnitFlags3 flags3 = (UnitFlags3)unit.flags3;
            if ((flags3 & UnitFlags3.ghostly) == UnitFlags3.ghostly)
                special = CreatureSpriteCollection.Special.Ghost;
            else if (unit.inventory.FindIndex(x => x.mode == InventoryMode.Weapon) >= 0)
                special = CreatureSpriteCollection.Special.Military;
            CasteDictionary dict;
            if (TryGetValue(special, out dict) && dict.TryGetValue(unit, out collection))
                return true;
            else if (TryGetValue(CreatureSpriteCollection.Special.Normal, out dict) && dict.TryGetValue(unit, out collection))
                return true;
            collection = default(CreatureSpriteCollection);
            return false;
        }

    }
    class ProfessionDictionary : Dictionary<profession, SpecialDictionary>
    {
        new public bool TryGetValue(profession prof, out SpecialDictionary dict)
        {
            if (base.TryGetValue(prof, out dict))
                return true;
            if (base.TryGetValue(profession.NONE, out dict))
                return true;
            return false;
        }

        public bool TryGetValue(UnitDefinition unit, out CreatureSpriteCollection collection)
        {
            SpecialDictionary dict;
            if (TryGetValue((profession)unit.profession_id, out dict) && dict.TryGetValue(unit, out collection))
                return true;
            else if (TryGetValue(profession.NONE, out dict) && dict.TryGetValue(unit, out collection))
                return true;
            collection = default(CreatureSpriteCollection);
            return false;
        }
    }
    class RaceDictionary : Dictionary<int, ProfessionDictionary>
    {
        new public bool TryGetValue(int race, out ProfessionDictionary collection)
        {
            if (base.TryGetValue(race, out collection))
                return true;
            if (base.TryGetValue(-1, out collection))
                return true;
            return false;
        }
        public bool TryGetValue(UnitDefinition unit, out CreatureSpriteCollection collection)
        {
            ProfessionDictionary dict;
            if (TryGetValue(unit.race.mat_type, out dict) && dict.TryGetValue(unit, out collection))
                return true;
            else if (TryGetValue(-1, out dict) && dict.TryGetValue(unit, out collection))
                return true;
            collection = default(CreatureSpriteCollection);
            return false;
        }
    }

    RaceDictionary spriteSets = new RaceDictionary();

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
                    raceID = new MatPairStruct(raceID.Race, casteRaw.index);
                }
                if (!spriteSets.ContainsKey(raceID.Race))
                    spriteSets[raceID.Race] = new ProfessionDictionary();
                if (!spriteSets[raceID.Race].ContainsKey(spriteSet.profession))
                    spriteSets[raceID.Race][spriteSet.profession] = new SpecialDictionary();
                if (!spriteSets[raceID.Race][spriteSet.profession].ContainsKey(spriteSet.special))
                    spriteSets[raceID.Race][spriteSet.profession][spriteSet.special] = new CasteDictionary();
                spriteSets[raceID.Race][spriteSet.profession][spriteSet.special][raceID.Caste] = spriteSet;
            }
        }
    }

    public CreatureSpriteCollection GetCreatureSprite(UnitDefinition unit)
    {
        CreatureSpriteCollection casteSprites;
        if (!spriteSets.TryGetValue(unit, out casteSprites))
            return null;
        return casteSprites;
    }
}
