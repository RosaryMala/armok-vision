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

        private static Dictionary<string, Dictionary<string, MatPairStruct>> _casteIds;

        public static Dictionary<string, Dictionary<string, MatPairStruct>> CasteIDs
        {
            get
            {
                return _casteIds;
            }
        }



        static void AddCreature(string race, string caste, MatPairStruct id)
        {
            if (_casteIds == null)
                _casteIds = new Dictionary<string, Dictionary<string, MatPairStruct>>();
            if (!_casteIds.ContainsKey(race))
                _casteIds[race] = new Dictionary<string, MatPairStruct>();
            _casteIds[race][caste] = id;
        }

        static private void PopulateWordLists()
        {
            foreach (var race in _creatureRawList)
            {
                foreach (var caste in race.caste)
                {
                    MatPairStruct id = new MatPairStruct(race.index, caste.index);
                    if (_casteIds == null)
                        _casteIds = new Dictionary<string, Dictionary<string, MatPairStruct>>();
                    if (!_casteIds.ContainsKey(race.creature_id))
                        _casteIds[race.creature_id] = new Dictionary<string, MatPairStruct>();
                    _casteIds[race.creature_id][caste.caste_id] = id;
                }
            }
        }
    }
}
