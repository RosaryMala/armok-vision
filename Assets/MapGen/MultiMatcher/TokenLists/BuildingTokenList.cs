using RemoteFortressReader;
using System.Collections.Generic;

namespace TokenLists
{
    static class BuildingTokenList
    {
        static List<BuildingDefinition> _buildingTokenList;
        public static List<BuildingDefinition> BuildingTokens
        {
            set
            {
                if (_buildingTokenList != value)
                {
                    _buildingTokenList = value;
                    PopulateWordLists();
                }
            }
        }

        public static bool IsValid
        {
            get
            {
                return _buildingTokenList != null;
            }
        }

        public static Dictionary<string, BuildingDefinition> BuildingLookup { get; private set; }

        static void PopulateWordLists()
        {
            if (BuildingLookup == null)
                BuildingLookup = new Dictionary<string, BuildingDefinition>();
            foreach (BuildingDefinition token in _buildingTokenList)
            {
                BuildingLookup[token.id] = token;
            }
        }

    }
}
