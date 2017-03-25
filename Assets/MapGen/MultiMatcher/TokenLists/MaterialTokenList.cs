using RemoteFortressReader;
using System.Collections.Generic;

namespace TokenLists
{
    static class MaterialTokenList
    {
        static List<MaterialDefinition> _matTokenList;
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
    static Dictionary<string, Dictionary<string, Dictionary<string, MaterialDefinition>>> _tripleWords;
    public static Dictionary<string, Dictionary<string, Dictionary<string, MaterialDefinition>>> tripleWords
    {
        get
        {
            return _tripleWords;
        }
    }

    static void AddMat(string prefix, string word, string suffix, MaterialDefinition token)
    {
        if (_tripleWords == null)
            _tripleWords = new Dictionary<string, Dictionary<string, Dictionary<string, MaterialDefinition>>>();
        if (!_tripleWords.ContainsKey(prefix))
            _tripleWords[prefix] = new Dictionary<string, Dictionary<string, MaterialDefinition>>();
        if (!_tripleWords[prefix].ContainsKey(suffix))
            _tripleWords[prefix][suffix] = new Dictionary<string, MaterialDefinition>();
        _tripleWords[prefix][suffix][word] = token;
    }

    static void PopulateWordLists()
    {
        foreach (MaterialDefinition token in _matTokenList)
        {
            var parts = token.Id.Split(':');
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
