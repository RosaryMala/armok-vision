using RemoteFortressReader;
using System;
using System.Collections.Generic;
using System.Text;

namespace TokenLists
{
    static class PlantTokenList
    {
        public static readonly string[] ColorTable = new string[16]
        {
            "Black",
            "Blue",
            "Green",
            "Cyan",
            "Red",
            "Magenta",
            "Brown",
            "LightGrey",
            "DarkGrey",
            "LightBlue",
            "LightGreen",
            "LightCyan",
            "LightRed",
            "LightMagenta",
            "Yellow",
            "White"
        };

        static List<PlantRaw> _plantRawList;
        public static List<PlantRaw> PlantRawList
        {
            set
            {
                if (_plantRawList != value)
                {
                    _plantRawList = value;
                    PopulateWordLists();
                }
            }
        }
        public static bool IsValid
        {
            get { return _plantRawList != null; }
        }

        public static Dictionary<string, Dictionary<string, Dictionary<string, BuildingStruct>>> GrowthIDs { get; private set; }

        private static void AddPlant(string plant, string growth, string print, BuildingStruct index)
        {
            if (GrowthIDs == null)
                GrowthIDs = new Dictionary<string, Dictionary<string, Dictionary<string, BuildingStruct>>>();
            if (!GrowthIDs.ContainsKey(plant))
                GrowthIDs[plant] = new Dictionary<string, Dictionary<string, BuildingStruct>>();
            if (!GrowthIDs[plant].ContainsKey(growth))
                GrowthIDs[plant][growth] = new Dictionary<string, BuildingStruct>();
            GrowthIDs[plant][growth][print] = index;
        }

        private static void PopulateWordLists()
        {
            foreach (PlantRaw plantRaw in _plantRawList)
            {
                if(plantRaw.Growths.Count == 0)
                    AddPlant(plantRaw.Id, "NONE", "NONE", new BuildingStruct(plantRaw.Index, -1, -1));
                foreach (TreeGrowth growthRaw in plantRaw.Growths)
                {
                    if(growthRaw.Prints.Count == 0)
                        AddPlant(plantRaw.Id, growthRaw.Id, "NONE", new BuildingStruct(plantRaw.Index, growthRaw.Index, -1));
                    int printIndex = 0;
                    foreach (GrowthPrint growthPrintRaw in growthRaw.Prints)
                    {
                        if (growthPrintRaw.Color >= 16)
                            UnityEngine.Debug.LogError("What the hell is color " + growthPrintRaw.Color);
                        AddPlant(plantRaw.Id, growthRaw.Id, ColorTable[growthPrintRaw.Color], new BuildingStruct(plantRaw.Index, growthRaw.Index, printIndex));
                        printIndex++;
                    }
                }
            }
        }
    }
}
