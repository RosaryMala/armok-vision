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

        public static bool TryGetCasteID(string token, out MatPairStruct id)
        {
            string[] parts = token.Split(':');
            string race = parts[0];
            string caste = "*";
            if (parts.Length > 1)
                caste = parts[1];
            if(!CasteIDs.ContainsKey(race))
            {
                id = new MatPairStruct(-1, -1);
                return false;
            }
            if(!CasteIDs[race].ContainsKey(caste))
                caste = "*";
            id = CasteIDs[race][caste];
            return true;
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
                foreach (var caste in race.Caste)
                {
                    MatPairStruct id = new MatPairStruct(race.Index, caste.Index);
                    if (_casteIds == null)
                        _casteIds = new Dictionary<string, Dictionary<string, MatPairStruct>>();
                    if (!_casteIds.ContainsKey(race.CreatureId))
                        _casteIds[race.CreatureId] = new Dictionary<string, MatPairStruct>();
                    _casteIds[race.CreatureId][caste.CasteId] = id;
                }
                {
                    MatPairStruct id = new MatPairStruct(race.Index, -1);
                    if (_casteIds == null)
                        _casteIds = new Dictionary<string, Dictionary<string, MatPairStruct>>();
                    if (!_casteIds.ContainsKey(race.CreatureId))
                        _casteIds[race.CreatureId] = new Dictionary<string, MatPairStruct>();
                    _casteIds[race.CreatureId]["*"] = id;
                }
            }
        }
    }
}
