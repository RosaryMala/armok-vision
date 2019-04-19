

using System;
using System.Collections.Generic;
using RemoteFortressReader;
using System.Text.RegularExpressions;

public class GeneratedCreatureTranslator
{
    static string[] templateList = new string[]
    {
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

    internal static CreatureRaw[] GetCreatureRaw(string race)
    {
        var creatureRaw = CreatureRaws.Instance.CreatureList.FindAll(x => x.creature_id == race);
        if (creatureRaw == null)
            creatureRaw = new List<CreatureRaw>();
        var tokenParts = race.Split('_');
        switch (tokenParts[0])
        {
            case "ANGEL":
                creatureRaw.AddRange( CreatureRaws.Instance.CreatureList.FindAll(x => x.creature_id.StartsWith("HF") && Regex.IsMatch(x.caste[0].description.Split('.')[0], "\\b" + tokenParts[1] + "\\b", RegexOptions.IgnoreCase)));
                break;
            case "DEMON":
                creatureRaw.AddRange(CreatureRaws.Instance.CreatureList.FindAll(x => x.creature_id.StartsWith("DEMON") && Regex.IsMatch(x.caste[0].description.Split('.')[0], "\\b" + tokenParts[1] + "\\b", RegexOptions.IgnoreCase)));
                break;
            case "FB":
                creatureRaw.AddRange(CreatureRaws.Instance.CreatureList.FindAll(x => x.creature_id.StartsWith("FORGOTTEN_BEAST") && Regex.IsMatch(x.caste[0].description.Split('.')[0], "\\b" + tokenParts[1] + "\\b", RegexOptions.IgnoreCase)));
                break;
            case "TITAN":
                creatureRaw.AddRange(CreatureRaws.Instance.CreatureList.FindAll(x => x.creature_id.StartsWith("TITAN") && Regex.IsMatch(x.caste[0].description.Split('.')[0], "\\b" + tokenParts[1] + "\\b", RegexOptions.IgnoreCase)));
                break;
            case "WERE":
                creatureRaw.AddRange(CreatureRaws.Instance.CreatureList.FindAll(x => x.name[0].ToUpper() == ("were" + tokenParts[1]).ToUpper()));
                break;
            default:
                break;
        }
        return creatureRaw.ToArray();
    }
    static int count;
    internal static void AddFakeCreaturesToList(Dictionary<MatPairStruct, MaterialDefinition> creatures)
    {
        count = -2;
        AddFakeCreaturesToList(creatures, "ANGEL");
        AddFakeCreaturesToList(creatures, "DEMON");
        AddFakeCreaturesToList(creatures, "FB");
        AddFakeCreaturesToList(creatures, "TITAN");
        AddFakeCreaturesToList(creatures, "WERE");
    }
    internal static void AddFakeCreaturesToList(Dictionary<MatPairStruct, MaterialDefinition> creatures, string prefix)
    {
        foreach (string template in templateList)
        {
            MatPairStruct creatureCaste = new MatPairStruct(count, -1);
            MaterialDefinition creatureDef = new MaterialDefinition();
            creatureDef.mat_pair = creatureCaste;
            creatureDef.id = prefix.ToUpper() + "_" + template.ToUpper() + ":DEFAULT";
            creatureDef.name = prefix.ToLower() + " " + template.ToLower();
            creatures[creatureCaste] = creatureDef;
            count--;
        }
    }
}
