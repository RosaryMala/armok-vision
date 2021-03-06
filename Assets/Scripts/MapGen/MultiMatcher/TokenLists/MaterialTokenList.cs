﻿using RemoteFortressReader;
using System;
using System.Collections.Generic;

namespace TokenLists
{
    public static class MaterialTokenList
    {

        static EventHandler rawUpdateEvent;
        static List<MaterialDefinition> _matTokenList;
        private static Dictionary<string, Dictionary<string, Dictionary<string, MaterialDefinition>>> s_tripleWords;

        public static List<MaterialDefinition> MaterialTokens
        {
            set
            {
                if (_matTokenList != value)
                {
                    _matTokenList = value;
                    PopulateWordLists();
                }
            }
        }
        public static Dictionary<string, Dictionary<string, Dictionary<string, MaterialDefinition>>> TripleWords {
            get
            {
                if (s_tripleWords == null)
                    s_tripleWords = new Dictionary<string, Dictionary<string, Dictionary<string, MaterialDefinition>>>();
                return s_tripleWords;
            }
            private set => s_tripleWords = value;
        }

        static void AddMat(string prefix, string word, string suffix, MaterialDefinition token)
        {
            if (TripleWords == null)
                TripleWords = new Dictionary<string, Dictionary<string, Dictionary<string, MaterialDefinition>>>();
            if (!TripleWords.ContainsKey(prefix))
                TripleWords[prefix] = new Dictionary<string, Dictionary<string, MaterialDefinition>>();
            if (!TripleWords[prefix].ContainsKey(suffix))
                TripleWords[prefix][suffix] = new Dictionary<string, MaterialDefinition>();
            TripleWords[prefix][suffix][word] = token;
        }

        static void PopulateWordLists()
        {
            if (TripleWords != null)
                TripleWords.Clear();
            foreach (MaterialDefinition token in _matTokenList)
            {
                var parts = token.id.Split(':');
                switch (parts.Length)
                {
                    case 1:
                        AddMat(parts[0], "", "", token);
                        break;
                    case 2:
                        AddMat(parts[0], parts[1], "", token);
                        break;
                    case 3:
                        AddMat(parts[0], parts[1], parts[2], token);
                        break;
                    default:
                        break;
                }
            }
        }
    }
}
