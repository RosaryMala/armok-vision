using System.Collections.Generic;

[System.Serializable]
public class CreatureRawFlags
{
    public enum RawFlags
    {
        unk_wagon, // 0, 0x0
        EQUIPMENT_WAGON, // 1, 0x1
        MUNDANE, // 2, 0x2
        VERMIN_EATER, // 3, 0x3
        VERMIN_GROUNDER, // 4, 0x4
        VERMIN_ROTTER, // 5, 0x5
        VERMIN_SOIL, // 6, 0x6
        VERMIN_SOIL_COLONY, // 7, 0x7
        LARGE_ROAMING, // 8, 0x8
        VERMIN_FISH, // 9, 0x9
        LOOSE_CLUSTERS, // 10, 0xA
        FANCIFUL, // 11, 0xB
        BIOME_MOUNTAIN, // 12, 0xC
        BIOME_GLACIER, // 13, 0xD
        BIOME_TUNDRA, // 14, 0xE
        BIOME_SWAMP_TEMPERATE_FRESHWATER, // 15, 0xF
        BIOME_SWAMP_TEMPERATE_SALTWATER, // 16, 0x10
        BIOME_MARSH_TEMPERATE_FRESHWATER, // 17, 0x11
        BIOME_MARSH_TEMPERATE_SALTWATER, // 18, 0x12
        BIOME_SWAMP_TROPICAL_FRESHWATER, // 19, 0x13
        BIOME_SWAMP_TROPICAL_SALTWATER, // 20, 0x14
        BIOME_SWAMP_MANGROVE, // 21, 0x15
        BIOME_MARSH_TROPICAL_FRESHWATER, // 22, 0x16
        BIOME_MARSH_TROPICAL_SALTWATER, // 23, 0x17
        BIOME_FOREST_TAIGA, // 24, 0x18
        BIOME_FOREST_TEMPERATE_CONIFER, // 25, 0x19
        BIOME_FOREST_TEMPERATE_BROADLEAF, // 26, 0x1A
        BIOME_FOREST_TROPICAL_CONIFER, // 27, 0x1B
        BIOME_FOREST_TROPICAL_DRY_BROADLEAF, // 28, 0x1C
        BIOME_FOREST_TROPICAL_MOIST_BROADLEAF, // 29, 0x1D
        BIOME_GRASSLAND_TEMPERATE, // 30, 0x1E
        BIOME_SAVANNA_TEMPERATE, // 31, 0x1F
        BIOME_SHRUBLAND_TEMPERATE, // 32, 0x20
        BIOME_GRASSLAND_TROPICAL, // 33, 0x21
        BIOME_SAVANNA_TROPICAL, // 34, 0x22
        BIOME_SHRUBLAND_TROPICAL, // 35, 0x23
        BIOME_DESERT_BADLAND, // 36, 0x24
        BIOME_DESERT_ROCK, // 37, 0x25
        BIOME_DESERT_SAND, // 38, 0x26
        BIOME_OCEAN_TROPICAL, // 39, 0x27
        BIOME_OCEAN_TEMPERATE, // 40, 0x28
        BIOME_OCEAN_ARCTIC, // 41, 0x29
        BIOME_SUBTERRANEAN_WATER, // 42, 0x2A
        BIOME_SUBTERRANEAN_CHASM, // 43, 0x2B
        BIOME_SUBTERRANEAN_LAVA, // 44, 0x2C
        BIOME_POOL_TEMPERATE_FRESHWATER, // 45, 0x2D
        BIOME_POOL_TEMPERATE_BRACKISHWATER, // 46, 0x2E
        BIOME_POOL_TEMPERATE_SALTWATER, // 47, 0x2F
        BIOME_POOL_TROPICAL_FRESHWATER, // 48, 0x30
        BIOME_POOL_TROPICAL_BRACKISHWATER, // 49, 0x31
        BIOME_POOL_TROPICAL_SALTWATER, // 50, 0x32
        BIOME_LAKE_TEMPERATE_FRESHWATER, // 51, 0x33
        BIOME_LAKE_TEMPERATE_BRACKISHWATER, // 52, 0x34
        BIOME_LAKE_TEMPERATE_SALTWATER, // 53, 0x35
        BIOME_LAKE_TROPICAL_FRESHWATER, // 54, 0x36
        BIOME_LAKE_TROPICAL_BRACKISHWATER, // 55, 0x37
        BIOME_LAKE_TROPICAL_SALTWATER, // 56, 0x38
        BIOME_RIVER_TEMPERATE_FRESHWATER, // 57, 0x39
        BIOME_RIVER_TEMPERATE_BRACKISHWATER, // 58, 0x3A
        BIOME_RIVER_TEMPERATE_SALTWATER, // 59, 0x3B
        BIOME_RIVER_TROPICAL_FRESHWATER, // 60, 0x3C
        BIOME_RIVER_TROPICAL_BRACKISHWATER, // 61, 0x3D
        BIOME_RIVER_TROPICAL_SALTWATER, // 62, 0x3E
        GOOD, // 63, 0x3F
        EVIL, // 64, 0x40
        SAVAGE, // 65, 0x41
        NOT_ASEXUAL, // 66, 0x42
        /**
         * capable of breeding
         */
        unk_43, // 67, 0x43
        unk_44, // 68, 0x44
        unk_45, // 69, 0x45
        any_vermin, // 70, 0x46
        CASTE_CAN_LEARN, // 71, 0x47
        CASTE_VERMIN_HATEABLE, // 72, 0x48
        unk_49, // 73, 0x49
        CASTE_LARGE_PREDATOR, // 74, 0x4A
        CASTE_CURIOUSBEAST, // 75, 0x4B
        CASTE_BENIGN, // 76, 0x4C
        CASTE_NATURAL, // 77, 0x4D
        CASTE_MEGABEAST, // 78, 0x4E
        CASTE_SEMIMEGABEAST, // 79, 0x4F
        CASTE_POWER, // 80, 0x50
        CASTE_VERMIN_MICRO, // 81, 0x51
        CASTE_NOT_FIREIMMUNE, // 82, 0x52
        CASTE_MUST_BREATHE_AIR, // 83, 0x53
        CASTE_MUST_BREATHE_WATER, // 84, 0x54
        unk_55, // 85, 0x55
        CASTE_SWIMS_LEARNED, // 86, 0x56
        CASTE_COMMON_DOMESTIC, // 87, 0x57
        CASTE_UTTERANCES, // 88, 0x58
        CASTE_CAN_SPEAK, // 89, 0x59
        CASTE_FEATURE_BEAST, // 90, 0x5A
        GENERATED, // 91, 0x5B
        CASTE_TITAN, // 92, 0x5C
        CASTE_UNIQUE_DEMON, // 93, 0x5D
        DOES_NOT_EXIST, // 94, 0x5E
        CASTE_NOT_LIVING, // 95, 0x5F
        CASTE_MISCHIEVOUS, // 96, 0x60
        CASTE_FLIER, // 97, 0x61
        CASTE_DEMON, // 98, 0x62
        CASTE_NIGHT_CREATURE_ANY, // 99, 0x63
        CASTE_NIGHT_CREATURE_HUNTER, // 100, 0x64
        CASTE_NIGHT_CREATURE_BOGEYMAN, // 101, 0x65
        CASTE_CARNIVORE, // 102, 0x66
        ARTIFICIAL_HIVEABLE, // 103, 0x67
        UBIQUITOUS, // 104, 0x68
        unk_69, // 105, 0x69
        CASTE_SUPERNATURAL, // 106, 0x6A
        CASTE_BLOOD, // 107, 0x6B
        CASTE_GRAZER, // 108, 0x6C
        CASTE_unk_31, // 109, 0x6D
        unk_6e, // 110, 0x6E
        unk_6f // 111, 0x6F
    };
    public bool unk_wagon; // 0; 0x0
    public bool EQUIPMENT_WAGON; // 1; 0x1
    public bool MUNDANE; // 2; 0x2
    public bool VERMIN_EATER; // 3; 0x3
    public bool VERMIN_GROUNDER; // 4; 0x4
    public bool VERMIN_ROTTER; // 5; 0x5
    public bool VERMIN_SOIL; // 6; 0x6
    public bool VERMIN_SOIL_COLONY; // 7; 0x7
    public bool LARGE_ROAMING; // 8; 0x8
    public bool VERMIN_FISH; // 9; 0x9
    public bool LOOSE_CLUSTERS; // 10; 0xA
    public bool FANCIFUL; // 11; 0xB
    public bool BIOME_MOUNTAIN; // 12; 0xC
    public bool BIOME_GLACIER; // 13; 0xD
    public bool BIOME_TUNDRA; // 14; 0xE
    public bool BIOME_SWAMP_TEMPERATE_FRESHWATER; // 15; 0xF
    public bool BIOME_SWAMP_TEMPERATE_SALTWATER; // 16; 0x10
    public bool BIOME_MARSH_TEMPERATE_FRESHWATER; // 17; 0x11
    public bool BIOME_MARSH_TEMPERATE_SALTWATER; // 18; 0x12
    public bool BIOME_SWAMP_TROPICAL_FRESHWATER; // 19; 0x13
    public bool BIOME_SWAMP_TROPICAL_SALTWATER; // 20; 0x14
    public bool BIOME_SWAMP_MANGROVE; // 21; 0x15
    public bool BIOME_MARSH_TROPICAL_FRESHWATER; // 22; 0x16
    public bool BIOME_MARSH_TROPICAL_SALTWATER; // 23; 0x17
    public bool BIOME_FOREST_TAIGA; // 24; 0x18
    public bool BIOME_FOREST_TEMPERATE_CONIFER; // 25; 0x19
    public bool BIOME_FOREST_TEMPERATE_BROADLEAF; // 26; 0x1A
    public bool BIOME_FOREST_TROPICAL_CONIFER; // 27; 0x1B
    public bool BIOME_FOREST_TROPICAL_DRY_BROADLEAF; // 28; 0x1C
    public bool BIOME_FOREST_TROPICAL_MOIST_BROADLEAF; // 29; 0x1D
    public bool BIOME_GRASSLAND_TEMPERATE; // 30; 0x1E
    public bool BIOME_SAVANNA_TEMPERATE; // 31; 0x1F
    public bool BIOME_SHRUBLAND_TEMPERATE; // 32; 0x20
    public bool BIOME_GRASSLAND_TROPICAL; // 33; 0x21
    public bool BIOME_SAVANNA_TROPICAL; // 34; 0x22
    public bool BIOME_SHRUBLAND_TROPICAL; // 35; 0x23
    public bool BIOME_DESERT_BADLAND; // 36; 0x24
    public bool BIOME_DESERT_ROCK; // 37; 0x25
    public bool BIOME_DESERT_SAND; // 38; 0x26
    public bool BIOME_OCEAN_TROPICAL; // 39; 0x27
    public bool BIOME_OCEAN_TEMPERATE; // 40; 0x28
    public bool BIOME_OCEAN_ARCTIC; // 41; 0x29
    public bool BIOME_SUBTERRANEAN_WATER; // 42; 0x2A
    public bool BIOME_SUBTERRANEAN_CHASM; // 43; 0x2B
    public bool BIOME_SUBTERRANEAN_LAVA; // 44; 0x2C
    public bool BIOME_POOL_TEMPERATE_FRESHWATER; // 45; 0x2D
    public bool BIOME_POOL_TEMPERATE_BRACKISHWATER; // 46; 0x2E
    public bool BIOME_POOL_TEMPERATE_SALTWATER; // 47; 0x2F
    public bool BIOME_POOL_TROPICAL_FRESHWATER; // 48; 0x30
    public bool BIOME_POOL_TROPICAL_BRACKISHWATER; // 49; 0x31
    public bool BIOME_POOL_TROPICAL_SALTWATER; // 50; 0x32
    public bool BIOME_LAKE_TEMPERATE_FRESHWATER; // 51; 0x33
    public bool BIOME_LAKE_TEMPERATE_BRACKISHWATER; // 52; 0x34
    public bool BIOME_LAKE_TEMPERATE_SALTWATER; // 53; 0x35
    public bool BIOME_LAKE_TROPICAL_FRESHWATER; // 54; 0x36
    public bool BIOME_LAKE_TROPICAL_BRACKISHWATER; // 55; 0x37
    public bool BIOME_LAKE_TROPICAL_SALTWATER; // 56; 0x38
    public bool BIOME_RIVER_TEMPERATE_FRESHWATER; // 57; 0x39
    public bool BIOME_RIVER_TEMPERATE_BRACKISHWATER; // 58; 0x3A
    public bool BIOME_RIVER_TEMPERATE_SALTWATER; // 59; 0x3B
    public bool BIOME_RIVER_TROPICAL_FRESHWATER; // 60; 0x3C
    public bool BIOME_RIVER_TROPICAL_BRACKISHWATER; // 61; 0x3D
    public bool BIOME_RIVER_TROPICAL_SALTWATER; // 62; 0x3E
    public bool GOOD; // 63; 0x3F
    public bool EVIL; // 64; 0x40
    public bool SAVAGE; // 65; 0x41
    public bool NOT_ASEXUAL; // 66; 0x42
    /**
    * capable of breeding
    */
    public bool unk_43; // 67; 0x43
    public bool unk_44; // 68; 0x44
    public bool unk_45; // 69; 0x45
    public bool any_vermin; // 70; 0x46
    public bool CASTE_CAN_LEARN; // 71; 0x47
    public bool CASTE_VERMIN_HATEABLE; // 72; 0x48
    public bool unk_49; // 73; 0x49
    public bool CASTE_LARGE_PREDATOR; // 74; 0x4A
    public bool CASTE_CURIOUSBEAST; // 75; 0x4B
    public bool CASTE_BENIGN; // 76; 0x4C
    public bool CASTE_NATURAL; // 77; 0x4D
    public bool CASTE_MEGABEAST; // 78; 0x4E
    public bool CASTE_SEMIMEGABEAST; // 79; 0x4F
    public bool CASTE_POWER; // 80; 0x50
    public bool CASTE_VERMIN_MICRO; // 81; 0x51
    public bool CASTE_NOT_FIREIMMUNE; // 82; 0x52
    public bool CASTE_MUST_BREATHE_AIR; // 83; 0x53
    public bool CASTE_MUST_BREATHE_WATER; // 84; 0x54
    public bool unk_55; // 85; 0x55
    public bool CASTE_SWIMS_LEARNED; // 86; 0x56
    public bool CASTE_COMMON_DOMESTIC; // 87; 0x57
    public bool CASTE_UTTERANCES; // 88; 0x58
    public bool CASTE_CAN_SPEAK; // 89; 0x59
    public bool CASTE_FEATURE_BEAST; // 90; 0x5A
    public bool GENERATED; // 91; 0x5B
    public bool CASTE_TITAN; // 92; 0x5C
    public bool CASTE_UNIQUE_DEMON; // 93; 0x5D
    public bool DOES_NOT_EXIST; // 94; 0x5E
    public bool CASTE_NOT_LIVING; // 95; 0x5F
    public bool CASTE_MISCHIEVOUS; // 96; 0x60
    public bool CASTE_FLIER; // 97; 0x61
    public bool CASTE_DEMON; // 98; 0x62
    public bool CASTE_NIGHT_CREATURE_ANY; // 99; 0x63
    public bool CASTE_NIGHT_CREATURE_HUNTER; // 100; 0x64
    public bool CASTE_NIGHT_CREATURE_BOGEYMAN; // 101; 0x65
    public bool CASTE_CARNIVORE; // 102; 0x66
    public bool ARTIFICIAL_HIVEABLE; // 103; 0x67
    public bool UBIQUITOUS; // 104; 0x68
    public bool unk_69; // 105; 0x69
    public bool CASTE_SUPERNATURAL; // 106; 0x6A
    public bool CASTE_BLOOD; // 107; 0x6B
    public bool CASTE_GRAZER; // 108; 0x6C
    public bool CASTE_unk_31; // 109; 0x6D
    public bool unk_6e; // 110; 0x6E
    public bool unk_6f; // 111; 0x6F

    public CreatureRawFlags(List<bool> flags)
    {
        if (flags.Count == 0)
            return;
        unk_wagon = flags[0]; // 0x0
        EQUIPMENT_WAGON = flags[1]; // 0x1
        MUNDANE = flags[2]; // 0x2
        VERMIN_EATER = flags[3]; // 0x3
        VERMIN_GROUNDER = flags[4]; // 0x4
        VERMIN_ROTTER = flags[5]; // 0x5
        VERMIN_SOIL = flags[6]; // 0x6
        VERMIN_SOIL_COLONY = flags[7]; // 0x7
        LARGE_ROAMING = flags[8]; // 0x8
        VERMIN_FISH = flags[9]; // 0x9
        LOOSE_CLUSTERS = flags[10]; // 0xA
        FANCIFUL = flags[11]; // 0xB
        BIOME_MOUNTAIN = flags[12]; // 0xC
        BIOME_GLACIER = flags[13]; // 0xD
        BIOME_TUNDRA = flags[14]; // 0xE
        BIOME_SWAMP_TEMPERATE_FRESHWATER = flags[15]; // 0xF
        BIOME_SWAMP_TEMPERATE_SALTWATER = flags[16]; // 0x10
        BIOME_MARSH_TEMPERATE_FRESHWATER = flags[17]; // 0x11
        BIOME_MARSH_TEMPERATE_SALTWATER = flags[18]; // 0x12
        BIOME_SWAMP_TROPICAL_FRESHWATER = flags[19]; // 0x13
        BIOME_SWAMP_TROPICAL_SALTWATER = flags[20]; // 0x14
        BIOME_SWAMP_MANGROVE = flags[21]; // 0x15
        BIOME_MARSH_TROPICAL_FRESHWATER = flags[22]; // 0x16
        BIOME_MARSH_TROPICAL_SALTWATER = flags[23]; // 0x17
        BIOME_FOREST_TAIGA = flags[24]; // 0x18
        BIOME_FOREST_TEMPERATE_CONIFER = flags[25]; // 0x19
        BIOME_FOREST_TEMPERATE_BROADLEAF = flags[26]; // 0x1A
        BIOME_FOREST_TROPICAL_CONIFER = flags[27]; // 0x1B
        BIOME_FOREST_TROPICAL_DRY_BROADLEAF = flags[28]; // 0x1C
        BIOME_FOREST_TROPICAL_MOIST_BROADLEAF = flags[29]; // 0x1D
        BIOME_GRASSLAND_TEMPERATE = flags[30]; // 0x1E
        BIOME_SAVANNA_TEMPERATE = flags[31]; // 0x1F
        BIOME_SHRUBLAND_TEMPERATE = flags[32]; // 0x20
        BIOME_GRASSLAND_TROPICAL = flags[33]; // 0x21
        BIOME_SAVANNA_TROPICAL = flags[34]; // 0x22
        BIOME_SHRUBLAND_TROPICAL = flags[35]; // 0x23
        BIOME_DESERT_BADLAND = flags[36]; // 0x24
        BIOME_DESERT_ROCK = flags[37]; // 0x25
        BIOME_DESERT_SAND = flags[38]; // 0x26
        BIOME_OCEAN_TROPICAL = flags[39]; // 0x27
        BIOME_OCEAN_TEMPERATE = flags[40]; // 0x28
        BIOME_OCEAN_ARCTIC = flags[41]; // 0x29
        BIOME_SUBTERRANEAN_WATER = flags[42]; // 0x2A
        BIOME_SUBTERRANEAN_CHASM = flags[43]; // 0x2B
        BIOME_SUBTERRANEAN_LAVA = flags[44]; // 0x2C
        BIOME_POOL_TEMPERATE_FRESHWATER = flags[45]; // 0x2D
        BIOME_POOL_TEMPERATE_BRACKISHWATER = flags[46]; // 0x2E
        BIOME_POOL_TEMPERATE_SALTWATER = flags[47]; // 0x2F
        BIOME_POOL_TROPICAL_FRESHWATER = flags[48]; // 0x30
        BIOME_POOL_TROPICAL_BRACKISHWATER = flags[49]; // 0x31
        BIOME_POOL_TROPICAL_SALTWATER = flags[50]; // 0x32
        BIOME_LAKE_TEMPERATE_FRESHWATER = flags[51]; // 0x33
        BIOME_LAKE_TEMPERATE_BRACKISHWATER = flags[52]; // 0x34
        BIOME_LAKE_TEMPERATE_SALTWATER = flags[53]; // 0x35
        BIOME_LAKE_TROPICAL_FRESHWATER = flags[54]; // 0x36
        BIOME_LAKE_TROPICAL_BRACKISHWATER = flags[55]; // 0x37
        BIOME_LAKE_TROPICAL_SALTWATER = flags[56]; // 0x38
        BIOME_RIVER_TEMPERATE_FRESHWATER = flags[57]; // 0x39
        BIOME_RIVER_TEMPERATE_BRACKISHWATER = flags[58]; // 0x3A
        BIOME_RIVER_TEMPERATE_SALTWATER = flags[59]; // 0x3B
        BIOME_RIVER_TROPICAL_FRESHWATER = flags[60]; // 0x3C
        BIOME_RIVER_TROPICAL_BRACKISHWATER = flags[61]; // 0x3D
        BIOME_RIVER_TROPICAL_SALTWATER = flags[62]; // 0x3E
        GOOD = flags[63]; // 0x3F
        EVIL = flags[64]; // 0x40
        SAVAGE = flags[65]; // 0x41
        NOT_ASEXUAL = flags[66]; // 0x42
                                 /**
                                 * capable of breeding
                                 */
        unk_43 = flags[67]; // 0x43
        unk_44 = flags[68]; // 0x44
        unk_45 = flags[69]; // 0x45
        any_vermin = flags[70]; // 0x46
        CASTE_CAN_LEARN = flags[71]; // 0x47
        CASTE_VERMIN_HATEABLE = flags[72]; // 0x48
        unk_49 = flags[73]; // 0x49
        CASTE_LARGE_PREDATOR = flags[74]; // 0x4A
        CASTE_CURIOUSBEAST = flags[75]; // 0x4B
        CASTE_BENIGN = flags[76]; // 0x4C
        CASTE_NATURAL = flags[77]; // 0x4D
        CASTE_MEGABEAST = flags[78]; // 0x4E
        CASTE_SEMIMEGABEAST = flags[79]; // 0x4F
        CASTE_POWER = flags[80]; // 0x50
        CASTE_VERMIN_MICRO = flags[81]; // 0x51
        CASTE_NOT_FIREIMMUNE = flags[82]; // 0x52
        CASTE_MUST_BREATHE_AIR = flags[83]; // 0x53
        CASTE_MUST_BREATHE_WATER = flags[84]; // 0x54
        unk_55 = flags[85]; // 0x55
        CASTE_SWIMS_LEARNED = flags[86]; // 0x56
        CASTE_COMMON_DOMESTIC = flags[87]; // 0x57
        CASTE_UTTERANCES = flags[88]; // 0x58
        CASTE_CAN_SPEAK = flags[89]; // 0x59
        CASTE_FEATURE_BEAST = flags[90]; // 0x5A
        GENERATED = flags[91]; // 0x5B
        CASTE_TITAN = flags[92]; // 0x5C
        CASTE_UNIQUE_DEMON = flags[93]; // 0x5D
        DOES_NOT_EXIST = flags[94]; // 0x5E
        CASTE_NOT_LIVING = flags[95]; // 0x5F
        CASTE_MISCHIEVOUS = flags[96]; // 0x60
        CASTE_FLIER = flags[97]; // 0x61
        CASTE_DEMON = flags[98]; // 0x62
        CASTE_NIGHT_CREATURE_ANY = flags[99]; // 0x63
        CASTE_NIGHT_CREATURE_HUNTER = flags[100]; // 0x64
        CASTE_NIGHT_CREATURE_BOGEYMAN = flags[101]; // 0x65
        CASTE_CARNIVORE = flags[102]; // 0x66
        ARTIFICIAL_HIVEABLE = flags[103]; // 0x67
        UBIQUITOUS = flags[104]; // 0x68
        unk_69 = flags[105]; // 0x69
        CASTE_SUPERNATURAL = flags[106]; // 0x6A
        CASTE_BLOOD = flags[107]; // 0x6B
        CASTE_GRAZER = flags[108]; // 0x6C
        CASTE_unk_31 = flags[109]; // 0x6D
        unk_6e = flags[110]; // 0x6E
        unk_6f = flags[111]; // 0x6F
    }
}
