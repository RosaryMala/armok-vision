using RemoteFortressReader;
using System.Collections.Generic;

namespace TokenLists
{
    static class ItemTokenList
    {
        static List<MaterialDefinition> _itemTokenList;
        public static List<MaterialDefinition> ItemTokens
        {
            set
            {
                if (_itemTokenList != value)
                {
                    _itemTokenList = value;
                    PopulateWordLists();
                }
            }
        }
        public static bool IsValid
        {
            get
            {
                return _itemTokenList != null;
            }
        }
        public static Dictionary<string, MaterialDefinition> ItemLookup { get; private set; }

        static void PopulateWordLists()
        {
            if (ItemLookup == null)
                ItemLookup = new Dictionary<string, MaterialDefinition>();
            foreach (MaterialDefinition token in _itemTokenList)
            {
                ItemLookup[token.Id] = token;
            }
        }

    }

}
