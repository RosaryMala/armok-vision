using RemoteFortressReader;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

[CreateAssetMenu]
public class BodyDefinition : ScriptableObject
{
    readonly string[] generatedCreatureTypes = {
    "blob",        "quadruped",       "humanoid",     "silverfish",    "mayfly",        "dragonfly",   "damselfly",    "stonefly",
    "earwig",      "grasshopper",     "cricket",      "stick insect",  "cockroach",     "termite",     "mantis",       "louse",
    "thrips",      "aphid",           "cicada",       "assassin bug",  "wasp",          "hornet",      "tiger beetle", "ladybug",
    "weevil",      "darkling beetle", "click beetle", "firefly",       "scarab beetle", "stag beetle", "dung beetle",  "rhinoceros beetle",
    "rove beetle", "snakefly",        "lacewing",     "antlion larva", "mosquito",      "flea",        "scorpionfly",  "caddisfly",
    "butterfly",   "moth",            "caterpillar",  "maggot",        "spider",        "tarantula",   "scorpion",     "tick",
    "mite",        "shrimp",          "lobster",      "crab",          "nematode",      "snail",       "slug",         "earthworm",
    "leech",       "bristleworm",     "ribbon worm",  "flat worm",     "toad",          "frog",        "salamander",   "newt",
    "alligator",   "crocodile",       "lizard",       "chameleon",     "iguana",        "gecko",       "skink",        "gila monster",
    "monitor",     "serpent",         "viper",        "rattlesnake",   "cobra",         "python",      "anaconda",     "turtle",
    "tortoise",    "pterosaur",       "dimetrodon",   "sauropod",      "theropod",      "iguanodont",  "hadrosaurid",  "stegosaurid",
    "ceratopsid",  "ankylosaurid",    "duck",         "goose",         "swan",          "turkey",      "grouse",       "chicken",
    "quail",       "pheasant",        "gull",         "loon",          "grebe",         "albatross",   "petrel",       "penguin",
    "pelican",     "stork",           "vulture",      "flamingo",      "falcon",        "kestrel",     "condor",       "osprey",
    "buzzard",     "eagle",           "harrier",      "kite",          "crane",         "dove",        "pigeon",       "parrot",
    "cockatoo",    "cuckoo",          "nightjar",     "swift",         "hummingbird",   "kingfisher",  "hornbill",     "quetzal",
    "toucan",      "woodpecker",      "lyrebird",     "thornbill",     "honeyeater",    "oriole",      "fantail",      "shrike",
    "crow",        "raven",           "magpie",       "kinglet",       "lark",          "swallow",     "martin",       "bushtit",
    "warbler",     "thrush",          "oxpecker",     "starling",      "mockingbird",   "wren",        "nuthatch",     "sparrow",
    "tanager",     "cardinal",        "bunting",      "finch",         "titmouse",      "chickadee",   "waxwing",      "flycatcher",
    "opossum",     "koala",           "wombat",       "kangaroo",      "sloth",         "anteater",    "armadillo",    "squirrel",
    "marmot",      "beaver",          "gopher",       "mouse",         "porcupine",     "chinchilla",  "cavy",         "capybara",
    "rabbit",      "hare",            "lemur",        "loris",         "monkey",        "hedgehog",    "shrew",        "mole",
    "fruit bat",   "wolf",            "coyote",       "jackal",        "raccoon",       "coati",       "weasel",       "otter",
    "badger",      "skunk",           "bear",         "panda",         "panther",       "mongoose",    "hyena",        "civet",
    "walrus",      "pangolin",        "elephant",     "mammoth",       "horse",         "zebra",       "tapir",        "rhinoceros",
    "warthog",     "hippopotamus",    "camel",        "llama",         "giraffe",       "deer",        "moose",        "antelope",
    "sheep",       "goat",            "bison",        "buffalo",       "bull",          "ape",         "ant",          "bat",
    "owl",         "pig",             "bee",          "fly",           "hawk",          "jay",         "rat",          "fox",
    "cat",         "ass",             "elk"
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

    static void LoadDefaultBodyParts()
    {
        categoryParts = new Dictionary<CreatureBody.BodyCategory, BodyDefinition>();
        LoadDefaultBodyParts(CreatureBody.BodyCategory.Humanoid);
        LoadDefaultBodyParts(CreatureBody.BodyCategory.Quadruped);
        LoadDefaultBodyParts(CreatureBody.BodyCategory.Avian);
        LoadDefaultBodyParts(CreatureBody.BodyCategory.Bug);
        LoadDefaultBodyParts(CreatureBody.BodyCategory.Fish);
    }

    static void LoadDefaultBodyParts(CreatureBody.BodyCategory category)
    {
        var part = Resources.Load<BodyDefinition>("BodyDefinitions/Default/" + category);
        if (part != null)
            categoryParts[category] = part;
    }

    public static BodyPartModel GetPart(CreatureBody.BodyCategory category, CreatureRaw race, CasteRaw caste, BodyPartRaw part)
    {
        if (categoryParts == null)
            LoadDefaultBodyParts();

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
}
