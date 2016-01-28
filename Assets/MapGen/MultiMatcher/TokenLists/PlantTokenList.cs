using RemoteFortressReader;
using System;
using System.Collections.Generic;

namespace TokenLists
{
    static class PlantTokenList
    {
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

        private static void PopulateWordLists()
        {
            throw new NotImplementedException();
        }
    }
}
