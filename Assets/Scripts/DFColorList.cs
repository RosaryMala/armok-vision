using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DFColorList
{
    static ColorDescriptor[] descriptorList = {
        new ColorDescriptor("AMBER", "amber", "AMBER", 255,191,0),
        new ColorDescriptor("AMETHYST", "amethyst", "AMETHYST", 153,102,204),
        new ColorDescriptor("AQUA", "aqua", "AQUA", 0,255,255),
        new ColorDescriptor("AQUAMARINE", "aquamarine", "AQUAMARINE", 127,255,212),
        new ColorDescriptor("ASH_GRAY", "ash gray", "GRAY", 178,190,181),
        new ColorDescriptor("AUBURN", "auburn", "AUBURN", 111,53,26),
        new ColorDescriptor("AZURE", "azure", "AZURE", 0,127,255),
        new ColorDescriptor("BEIGE", "beige", "BEIGE", 245,245,220),
        new ColorDescriptor("BLACK", "black", "BLACK", 0,0,0),
        new ColorDescriptor("BLUE", "blue", "BLUE", 0,0,255),
        new ColorDescriptor("BRASS", "brass", "BRASS", 181,166,66),
        new ColorDescriptor("BRONZE", "bronze", "BRONZE", 205,127,50),
        new ColorDescriptor("BROWN", "brown", "BROWN", 150,75,0),
        new ColorDescriptor("BUFF", "buff", "BUFF", 240,220,130),
        new ColorDescriptor("BURNT_SIENNA", "burnt sienna", "SIENNA", 233,116,81),
        new ColorDescriptor("BURNT_UMBER", "burnt umber", "UMBER", 138,51,36),
        new ColorDescriptor("CARDINAL", "cardinal", "CARDINAL_COLOR", 196,30,58),
        new ColorDescriptor("CARMINE", "carmine", "CARMINE", 150,0,24),
        new ColorDescriptor("CERULEAN", "cerulean", "CERULEAN", 0,123,167),
        new ColorDescriptor("CHARCOAL", "charcoal", "CHARCOAL", 54,69,79),
        new ColorDescriptor("CHARTREUSE", "chartreuse", "CHARTREUSE", 127,255,0),
        new ColorDescriptor("CHESTNUT", "chestnut", "CHESTNUT", 205,92,92),
        new ColorDescriptor("CHOCOLATE", "chocolate", "CHOCOLATE", 210,105,30),
        new ColorDescriptor("CINNAMON", "cinnamon", "CINNAMON", 123,63,0),
        new ColorDescriptor("CLEAR", "clear", "CLEAR", 128,128,128),
        new ColorDescriptor("COBALT", "cobalt", "COBALT", 0,71,171),
        new ColorDescriptor("COPPER", "copper", "COPPER", 184,115,51),
        new ColorDescriptor("CREAM", "cream", "CREAM", 255,253,208),
        new ColorDescriptor("CRIMSON", "crimson", "CRIMSON", 220,20,60),
        new ColorDescriptor("DARK_BLUE", "dark blue", "BLUE", 0,0,139),
        new ColorDescriptor("DARK_BROWN", "dark brown", "BROWN", 101,67,33),
        new ColorDescriptor("DARK_CHESTNUT", "dark chestnut", "CHESTNUT", 152,105,96),
        new ColorDescriptor("DARK_GREEN", "dark green", "GREEN", 1,50,32),
        new ColorDescriptor("DARK_INDIGO", "dark indigo", "INDIGO", 49,0,98),
        new ColorDescriptor("DARK_OLIVE", "dark olive", "OLIVE", 85,104,50),
        new ColorDescriptor("DARK_PEACH", "dark peach", "PEACH", 255,218,185),
        new ColorDescriptor("DARK_PINK", "dark pink", "PINK", 231,84,128),
        new ColorDescriptor("DARK_SCARLET", "dark scarlet", "SCARLET", 86,3,25),
        new ColorDescriptor("DARK_TAN", "dark tan", "TAN", 145,129,81),
        new ColorDescriptor("DARK_VIOLET", "dark violet", "VIOLET", 66,49,137),
        new ColorDescriptor("ECRU", "ecru", "ECRU", 194,178,128),
        new ColorDescriptor("EGGPLANT", "eggplant", 97,64,81),
        new ColorDescriptor("EMERALD", "emerald", "EMERALD", 80,200,120),
        new ColorDescriptor("FERN_GREEN", "fern green", "GREEN", 79,121,66),
        new ColorDescriptor("FLAX", "flax", "FLAX", 238,220,130),
        new ColorDescriptor("FUCHSIA", "fuchsia", "FUCHSIA", 244,0,161),
        new ColorDescriptor("GOLD", "gold", "GOLD", 212,175,55),
        new ColorDescriptor("GOLDEN_YELLOW", "golden yellow", "GOLD", "YELLOW", 255,223,0),
        new ColorDescriptor("GOLDENROD", "goldenrod", "GOLDENROD", 218,165,32),
        new ColorDescriptor("GRAY", "gray", "GRAY", 128,128,128),
        new ColorDescriptor("GREEN", "green", "GREEN", 0,255,0),
        new ColorDescriptor("GREEN-YELLOW", "green-yellow", "GREEN", "YELLOW", 173,255,47),
        new ColorDescriptor("HELIOTROPE", "heliotrope", "HELIOTROPE", 223,115,255),
        new ColorDescriptor("INDIGO", "indigo", "INDIGO", 75,0,130),
        new ColorDescriptor("IVORY", "ivory", "IVORY", 255,255,240),
        new ColorDescriptor("JADE", "jade", "JADE", 0,168,107),
        new ColorDescriptor("LAVENDER", "lavender", "LAVENDER", 230,230,250),
        new ColorDescriptor("LAVENDER_BLUSH", "lavender blush", "LAVENDER", 255,240,245),
        new ColorDescriptor("LEMON", "lemon", "LEMON", 253,233,16),
        new ColorDescriptor("LIGHT_BLUE", "light blue", "BLUE", 173,216,230),
        new ColorDescriptor("LIGHT_BROWN", "light brown", "BROWN", 205,133,63),
        new ColorDescriptor("LILAC", "lilac", "LILAC", 200,162,200),
        new ColorDescriptor("LIME", "lime", "LIME", 204,255,0),
        new ColorDescriptor("MAHOGANY", "mahogany", "MAHOGANY", 192,64,0),
        new ColorDescriptor("MAROON", "maroon", "MAROON_COLOR", 128,0,0),
        new ColorDescriptor("MAUVE", "mauve", "MAUVE", 153,51,102),
        new ColorDescriptor("MAUVE_TAUPE", "mauve taupe", "MAUVE", "TAUPE", 145,95,109),
        new ColorDescriptor("MIDNIGHT_BLUE", "midnight blue", "BLUE", 0,51,102),
        new ColorDescriptor("MINT_GREEN", "mint green", "GREEN", 152,255,152),
        new ColorDescriptor("MOSS_GREEN", "moss green", "GREEN", 173,223,173),
        new ColorDescriptor("OCHRE", "ochre", "OCHRE", 204,119,34),
        new ColorDescriptor("OLIVE", "olive", "OLIVE", 128,128,0),
        new ColorDescriptor("ORANGE", "orange", "ORANGE", 255,165,0),
        new ColorDescriptor("PALE_BLUE", "pale blue", "BLUE", 175,238,238),
        new ColorDescriptor("PALE_BROWN", "pale brown", "BROWN", 152,118,84),
        new ColorDescriptor("PALE_CHESTNUT", "pale chestnut", "CHESTNUT", 221,173,175),
        new ColorDescriptor("PALE_PINK", "pale pink", "PINK", 250,218,221),
        new ColorDescriptor("PEACH", "peach", "PEACH", 255,229,180),
        new ColorDescriptor("PEARL", "pearl", "PEARL", 240,234,214),
        new ColorDescriptor("PERIWINKLE", "periwinkle", "PERIWINKLE", 204,204,255),
        new ColorDescriptor("PINE_GREEN", "pine green", "GREEN", "PINE", 1,121,111),
        new ColorDescriptor("PINK", "pink", "PINK", 255,192,203),
        new ColorDescriptor("PLUM", "plum", "PLUM", 102,0,102),
        new ColorDescriptor("PUCE", "puce", "PUCE", 204,136,153),
        new ColorDescriptor("PUMPKIN", "pumpkin", "PUMPKIN", 255,117,24),
        new ColorDescriptor("PURPLE", "purple", "PURPLE", 102,0,153),
        new ColorDescriptor("RAW_UMBER", "raw umber", "UMBER", 115,74,18),
        new ColorDescriptor("RED", "red", "RED", 255,0,0),
        new ColorDescriptor("RED_PURPLE", "red-purple", "RED", "PURPLE", 178,0,75),
        new ColorDescriptor("ROSE", "rose", "ROSE", 244,194,194), //went with tea rose
        new ColorDescriptor("RUSSET", "russet", "RUSSET", 117,90,87),
        new ColorDescriptor("RUST", "rust", "RUST", 183,65,14),
        new ColorDescriptor("SAFFRON", "saffron", "SAFFRON", 244,196,48),
        new ColorDescriptor("SCARLET", "scarlet", "SCARLET", 255,36,0),
        new ColorDescriptor("SEA_GREEN", "sea green", "GREEN", "SEA", "OCEAN", 46,139,87),
        new ColorDescriptor("SEPIA", "sepia", "SEPIA", 112,66,20),
        new ColorDescriptor("SILVER", "silver", "SILVER", 192,192,192),
        new ColorDescriptor("SKY_BLUE", "sky blue", "BLUE", 135,206,235),
        new ColorDescriptor("SLATE_GRAY", "slate gray", "GRAY", 112,128,144),
        new ColorDescriptor("SPRING_GREEN", "spring green", "GREEN", 0,255,127),
        new ColorDescriptor("TAN", "tan", "TAN", 210,180,140),
        new ColorDescriptor("TAUPE_DARK", "dark taupe", "TAUPE", 72,60,50),
        new ColorDescriptor("TAUPE_GRAY", "taupe gray", "TAUPE", "GRAY", 139,133,137),
        new ColorDescriptor("TAUPE_MEDIUM", "taupe", "TAUPE", 103,76,71),
        new ColorDescriptor("TAUPE_PURPLE", "purple taupe", "PURPLE", "TAUPE", 80,64,77),
        new ColorDescriptor("TAUPE_PALE", "pale taupe", "TAUPE", 188,152,126),
        new ColorDescriptor("TAUPE_ROSE", "rose taupe", "TAUPE", 144,93,93),
        new ColorDescriptor("TAUPE_SANDY", "sandy taupe", "TAUPE", 150,113,23),
        new ColorDescriptor("TEAL", "teal", "TEAL", 0,128,128),
        new ColorDescriptor("TURQUOISE", "turquoise", "TURQUOISE", 48,213,200),
        new ColorDescriptor("VERMILION", "vermilion", "VERMILION", 227,66,52),
        new ColorDescriptor("VIOLET", "violet", "VIOLET", 139,0,255),
        new ColorDescriptor("WHITE", "white", "WHITE", 255,255,255),
        new ColorDescriptor("YELLOW", "yellow", "YELLOW", 255,255,0),
        new ColorDescriptor("YELLOW_GREEN", "yellow-green", "GREEN", "YELLOW", 154,205,50)
    };
    private class ColorDescriptor
    {
        public string token;
        public string name;
        public Color color;

        public ColorDescriptor(string token, string name, string word, byte red, byte green, byte blue)
        {
            this.token = token;
            this.name = name;
            color = new Color32(red, green, blue, 255);
        }
        public ColorDescriptor(string token, string name, byte red, byte green, byte blue)
        {
            this.token = token;
            this.name = name;
            color = new Color32(red, green, blue, 255);
        }
        public ColorDescriptor(string token, string name, string word1, string word2, byte red, byte green, byte blue)
        {
            this.token = token;
            this.name = name;
            color = new Color32(red, green, blue, 255);
        }
        public ColorDescriptor(string token, string name, string word1, string word2, string word3, byte red, byte green, byte blue)
        {
            this.token = token;
            this.name = name;
            color = new Color32(red, green, blue, 255);
        }

        public float DistanceSquared(Color comparison)
        {
            float h1, h2, s1, s2, v1, v2;
            Color.RGBToHSV(color, out h1, out s1, out v1);
            Color.RGBToHSV(comparison, out h2, out s2, out v2);
            return ((h1 - h2) * (h1 - h2) + (s1 - s2) * (s1 - s2) + (v1 - v2) * (v1 - v2));
        }
    }
    public static string FindNearestColor(Color color)
    {
        string token = "";
        float distance = 99999999999;
        foreach (var item in descriptorList)
        {
            float dist = item.DistanceSquared(color);
            if (dist < distance)
            {
                distance = dist;
                token = item.token;
            }
        }
        return token;
    }
}
