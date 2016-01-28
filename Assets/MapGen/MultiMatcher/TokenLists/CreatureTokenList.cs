using RemoteFortressReader;
using System.Collections.Generic;

namespace TokenLists
{
    static class CreatureTokenList
    {
        static List<CreatureRaw> _creatureRawList;
        public static List<CreatureRaw> CreatureRawList
        {
            set
            {
                if (_creatureRawList != value)
                {
                    _creatureRawList = value;
                    PopulateWordLists();
                }
            }
        }

        private static Dictionary<string, Dictionary<string, IntPair>> _casteIds;

        public static Dictionary<string, Dictionary<string, IntPair>> CasteIDs
        {
            get
            {
                return _casteIds;
            }
        }



        static void AddCreature(string race, string caste, IntPair id)
        {
            if (_casteIds == null)
                _casteIds = new Dictionary<string, Dictionary<string, IntPair>>();
            if (!_casteIds.ContainsKey(race))
                _casteIds[race] = new Dictionary<string, IntPair>();
            _casteIds[race][caste] = id;
        }

        static private void PopulateWordLists()
        {
            foreach (var race in _creatureRawList)
            {
                foreach (var caste in race.caste)
                {
                    IntPair id = new IntPair(race.index, caste.index);
                    if (_casteIds == null)
                        _casteIds = new Dictionary<string, Dictionary<string, IntPair>>();
                    if (!_casteIds.ContainsKey(race.creature_id))
                        _casteIds[race.creature_id] = new Dictionary<string, IntPair>();
                    _casteIds[race.creature_id][caste.caste_id] = id;
                }
            }
        }
    }
}
