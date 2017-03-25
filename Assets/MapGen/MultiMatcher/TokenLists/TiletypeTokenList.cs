using RemoteFortressReader;
using System.Collections.Generic;

namespace TokenLists
{
    public static class TiletypeTokenList
    {
        static List<Tiletype> _tiletypeTokenList;
        public static Dictionary<TiletypeShape, Dictionary<TiletypeSpecial, Dictionary<TiletypeMaterial, Dictionary<TiletypeVariant, Dictionary<string, Tiletype>>>>> tileDefs;
        public static Dictionary<string, Tiletype> tileTokens;
        public static List<Tiletype> tiletypeTokenList
        {
            set
            {
                if (_tiletypeTokenList != value)
                {
                    _tiletypeTokenList = value;
                    PopulateWordLists();
                }
            }
        }
        static void AddTile(TiletypeShape tileShape, TiletypeSpecial tileSpecial, TiletypeMaterial tileMaterial, TiletypeVariant tileVariant, string tileDirection, Tiletype token)
        {
            if (tileDefs == null)
                tileDefs = new Dictionary<TiletypeShape, Dictionary<TiletypeSpecial, Dictionary<TiletypeMaterial, Dictionary<TiletypeVariant, Dictionary<string, Tiletype>>>>>();
            if (!tileDefs.ContainsKey(tileShape))
                tileDefs[tileShape] = new Dictionary<TiletypeSpecial, Dictionary<TiletypeMaterial, Dictionary<TiletypeVariant, Dictionary<string, Tiletype>>>>();
            if (!tileDefs[tileShape].ContainsKey(tileSpecial))
                tileDefs[tileShape][tileSpecial] = new Dictionary<TiletypeMaterial, Dictionary<TiletypeVariant, Dictionary<string, Tiletype>>>();
            if (!tileDefs[tileShape][tileSpecial].ContainsKey(tileMaterial))
                tileDefs[tileShape][tileSpecial][tileMaterial] = new Dictionary<TiletypeVariant, Dictionary<string, Tiletype>>();
            if (!tileDefs[tileShape][tileSpecial][tileMaterial].ContainsKey(tileVariant))
                tileDefs[tileShape][tileSpecial][tileMaterial][tileVariant] = new Dictionary<string, Tiletype>();
            tileDefs[tileShape][tileSpecial][tileMaterial][tileVariant][tileDirection] = token;

        }

        static void PopulateWordLists()
        {
            foreach (Tiletype token in _tiletypeTokenList)
            {
                if (tileTokens == null)
                    tileTokens = new Dictionary<string, Tiletype>();
                tileTokens[token.Name] = token;
                AddTile(token.Shape, token.Special, token.Material, token.Variant, token.Direction, token);
            }
        }
    }
}
