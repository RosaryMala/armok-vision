using RemoteFortressReader;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

[CreateAssetMenu]
public class BodyDefinition : ScriptableObject
{
    static readonly string[] generatedCreatureTypes = {
    "BLOB",        "QUADRUPED",     /*"HUMANOID",*/   "SILVERFISH",    "MAYFLY",        "DRAGONFLY",   "DAMSELFLY",    "STONEFLY",
    "EARWIG",      "GRASSHOPPER",     "CRICKET",      "STICK INSECT",  "COCKROACH",     "TERMITE",     "MANTIS",       "LOUSE",
    "THRIPS",      "APHID",           "CICADA",       "ASSASSIN BUG",  "WASP",          "HORNET",      "TIGER BEETLE", "LADYBUG",
    "WEEVIL",      "DARKLING BEETLE", "CLICK BEETLE", "FIREFLY",       "SCARAB BEETLE", "STAG BEETLE", "DUNG BEETLE",  "RHINOCEROS BEETLE",
    "ROVE BEETLE", "SNAKEFLY",        "LACEWING",     "ANTLION LARVA", "MOSQUITO",      "FLEA",        "SCORPIONFLY",  "CADDISFLY",
    "BUTTERFLY",   "MOTH",            "CATERPILLAR",  "MAGGOT",        "SPIDER",        "TARANTULA",   "SCORPION",     "TICK",
    "MITE",        "SHRIMP",          "LOBSTER",      "CRAB",          "NEMATODE",      "SNAIL",       "SLUG",         "EARTHWORM",
    "LEECH",       "BRISTLEWORM",     "RIBBON WORM",  "FLAT WORM",     "TOAD",          "FROG",        "SALAMANDER",   "NEWT",
    "ALLIGATOR",   "CROCODILE",       "LIZARD",       "CHAMELEON",     "IGUANA",        "GECKO",       "SKINK",        "GILA MONSTER",
    "MONITOR",     "SERPENT",         "VIPER",        "RATTLESNAKE",   "COBRA",         "PYTHON",      "ANACONDA",     "TURTLE",
    "TORTOISE",    "PTEROSAUR",       "DIMETRODON",   "SAUROPOD",      "THEROPOD",      "IGUANODONT",  "HADROSAURID",  "STEGOSAURID",
    "CERATOPSID",  "ANKYLOSAURID",    "DUCK",         "GOOSE",         "SWAN",          "TURKEY",      "GROUSE",       "CHICKEN",
    "QUAIL",       "PHEASANT",        "GULL",         "LOON",          "GREBE",         "ALBATROSS",   "PETREL",       "PENGUIN",
    "PELICAN",     "STORK",           "VULTURE",      "FLAMINGO",      "FALCON",        "KESTREL",     "CONDOR",       "OSPREY",
    "BUZZARD",     "EAGLE",           "HARRIER",      "KITE",          "CRANE",         "DOVE",        "PIGEON",       "PARROT",
    "COCKATOO",    "CUCKOO",          "NIGHTJAR",     "SWIFT",         "HUMMINGBIRD",   "KINGFISHER",  "HORNBILL",     "QUETZAL",
    "TOUCAN",      "WOODPECKER",      "LYREBIRD",     "THORNBILL",     "HONEYEATER",    "ORIOLE",      "FANTAIL",      "SHRIKE",
    "CROW",        "RAVEN",           "MAGPIE",       "KINGLET",       "LARK",          "SWALLOW",     "MARTIN",       "BUSHTIT",
    "WARBLER",     "THRUSH",          "OXPECKER",     "STARLING",      "MOCKINGBIRD",   "WREN",        "NUTHATCH",     "SPARROW",
    "TANAGER",     "CARDINAL",        "BUNTING",      "FINCH",         "TITMOUSE",      "CHICKADEE",   "WAXWING",      "FLYCATCHER",
    "OPOSSUM",     "KOALA",           "WOMBAT",       "KANGAROO",      "SLOTH",         "ANTEATER",    "ARMADILLO",    "SQUIRREL",
    "MARMOT",      "BEAVER",          "GOPHER",       "MOUSE",         "PORCUPINE",     "CHINCHILLA",  "CAVY",         "CAPYBARA",
    "RABBIT",      "HARE",            "LEMUR",        "LORIS",         "MONKEY",        "HEDGEHOG",    "SHREW",        "MOLE",
    "FRUIT BAT",   "WOLF",            "COYOTE",       "JACKAL",        "RACCOON",       "COATI",       "WEASEL",       "OTTER",
    "BADGER",      "SKUNK",           "BEAR",         "PANDA",         "PANTHER",       "MONGOOSE",    "HYENA",        "CIVET",
    "WALRUS",      "PANGOLIN",        "ELEPHANT",     "MAMMOTH",       "HORSE",         "ZEBRA",       "TAPIR",        "RHINOCEROS",
    "WARTHOG",     "HIPPOPOTAMUS",    "CAMEL",        "LLAMA",         "GIRAFFE",       "DEER",        "MOOSE",        "ANTELOPE",
    "SHEEP",       "GOAT",            "BISON",        "BUFFALO",       "BULL",          "APE",         "ANT",          "BAT",
    "OWL",         "PIG",             "BEE",          "FLY",           "HAWK",          "JAY",         "RAT",          "FOX",
    "CAT",         "ASS",             "ELK"
    };


    [System.Serializable]
    public class BodyPartSelection
    {
        public string category;
        public bool categoryRegex;
        public string token;
        public bool tokenRegex;
        public BodyPartModel model;
    }
    public Vector3 bodyScale = Vector3.one;

    public List<BodyPartSelection> bodyParts;

    internal BodyPartModel GetPart(BodyPartRaw part)
    {
        foreach (var partModel in bodyParts)
        {
            if(!string.IsNullOrEmpty(partModel.category))
            {
                if(partModel.categoryRegex)
                {
                    if (!Regex.IsMatch(part.category, partModel.category))
                        continue;
                }
                else
                {
                    if (partModel.category != part.category)
                        continue;
                }
            }
            if (!string.IsNullOrEmpty(partModel.token))
            {
                if (partModel.tokenRegex)
                {
                    if (!Regex.IsMatch(part.token, partModel.token))
                        continue;
                }
                else
                {
                    if (partModel.token != part.token)
                        continue;
                }
            }
            return partModel.model;
        }
        return null;
    }

    static Dictionary<CreatureBody.BodyCategory, BodyDefinition> categoryParts = new Dictionary<CreatureBody.BodyCategory, BodyDefinition>();
    static Dictionary<string, BodyDefinition> raceParts = new Dictionary<string, BodyDefinition>();
    static Dictionary<string, Dictionary<string, BodyDefinition>> casteParts = new Dictionary<string, Dictionary<string, BodyDefinition>>();



    public static BodyPartModel GetPart(CreatureBody.BodyCategory category, CreatureRaw race, CasteRaw caste, BodyPartRaw part)
    {
        BodyPartModel partModel = null;
        var bodyDef = GetBodyDefinition(race, caste);
        if (bodyDef != null)
            partModel = bodyDef.GetPart(part);
        if (partModel != null)
            return partModel;
        bodyDef = GetBodyDefinition(race);
        if (bodyDef != null)
            partModel = bodyDef.GetPart(part);
        if (partModel != null)
            return partModel;
        bodyDef = GetBodyDefinition(category);
        if (bodyDef != null)
            partModel = bodyDef.GetPart(part);
        if (partModel != null)
            return partModel;
        bodyDef = GetBodyDefinition(CreatureBody.BodyCategory.None);
        if (bodyDef != null)
            partModel = bodyDef.GetPart(part);
        return partModel;
    }

    public static Vector3 GetBodyScale(CreatureBody.BodyCategory category, CreatureRaw race, CasteRaw caste)
    {
        var body = GetBodyDefinition(category, race, caste);
        if (body == null)
            return Vector3.one;
        return body.bodyScale;
    }

    static BodyDefinition GetBodyDefinition(CreatureBody.BodyCategory category, CreatureRaw race, CasteRaw caste)
    {
        BodyDefinition body = null;
        body = GetBodyDefinition(race, caste);
        if (body == null)
            body = GetBodyDefinition(race);
        if (body == null)
            body = GetBodyDefinition(category);
        return body;
    }
    static BodyDefinition GetBodyDefinition(CreatureRaw race, CasteRaw caste)
    {
        if (!casteParts.ContainsKey(race.creature_id))
            casteParts[race.creature_id] = new Dictionary<string, BodyDefinition>();
        if(!casteParts[race.creature_id].ContainsKey(caste.caste_id))
            casteParts[race.creature_id][caste.caste_id] = Resources.Load<BodyDefinition>("BodyDefinitions/" + race.creature_id + "/" + caste.caste_id);
        return casteParts[race.creature_id][caste.caste_id];
    }
    static BodyDefinition GetBodyDefinition(CreatureRaw race)
    {
        if (!raceParts.ContainsKey(race.creature_id))
            raceParts[race.creature_id] = Resources.Load<BodyDefinition>("BodyDefinitions/" + race.creature_id + "/Default");
        return raceParts[race.creature_id];
    }
    static BodyDefinition GetBodyDefinition(CreatureBody.BodyCategory category)
    {
        if (!categoryParts.ContainsKey(category))
            categoryParts[category] = Resources.Load<BodyDefinition>("BodyDefinitions/Default/" + category);
        return categoryParts[category];
    }

    public static string GetCorrectedCreatureID(CreatureRaw race, CasteRaw caste = null)
    {
        if(race.flags.Count == 0)
            return race.creature_id;
        if (!race.flags[(int)CreatureRawFlags.RawFlags.GENERATED])
            return race.creature_id;
        if (caste == null)
            caste = race.caste[0];
        var descParts = caste.description.Split('.');
        foreach (var type in generatedCreatureTypes)
        {
            if(Regex.IsMatch(descParts[0], @"\b" + type + @"\b", RegexOptions.IgnoreCase))
            {
                if (Regex.IsMatch(descParts[0], @"\bHUMANOID\b", RegexOptions.IgnoreCase))
                    return type + "_MAN";
                else
                    return type;
            }
        }
        if (Regex.IsMatch(descParts[0], @"\bHUMANOID\b", RegexOptions.IgnoreCase))
            return "HUMANOID";
        return race.creature_id;
    }
}
