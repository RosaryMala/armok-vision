using RemoteFortressReader;
using System;
using System.Collections.Generic;
using TokenLists;
using UnityEngine;

public class TiletypeMatcher<T>
{

    struct TiletypeMatch
    {
        public T item;
        public int difference;
    }
    Dictionary<int, TiletypeMatch> tiletypeList;

    void TrySetMatch(TiletypeMatch match, int tile)
    {
        if (tiletypeList == null)
            tiletypeList = new Dictionary<int, TiletypeMatch>();
        if(tiletypeList.ContainsKey(tile))
        {
            if (tiletypeList[tile].difference < match.difference)
                return;
        }
        tiletypeList[tile] = match;
    }
    void SetOptions(string direction, Dictionary<string, Tiletype> optionList, TiletypeMatch match)
    {
        if(direction == "*")
        {
            match.difference |= 1;
            foreach (var item in optionList.Values)
            {
                TrySetMatch(match, item.id);
            }
        }
        else
        {
            if (optionList.ContainsKey(direction))
                TrySetMatch(match, optionList[direction].id);
        }
    }

    void SetOptions(string variant, string direction, Dictionary<TiletypeVariant, Dictionary<string, Tiletype>> optionList, TiletypeMatch match)
    {
        if (variant == "*")
        {
            match.difference |= 2;
            foreach (var item in optionList.Values)
            {
                SetOptions(direction, item, match);
            }
        }
        else
        {
            try
            {
                TiletypeVariant tileVariant = (TiletypeVariant)Enum.Parse(typeof(TiletypeVariant), variant);
                if (optionList.ContainsKey(tileVariant))
                    SetOptions(direction, optionList[tileVariant], match);
            }
            catch (Exception)
            {
                Debug.LogError(variant + " is not a valid tile variant.");
                return;
            }
        }
    }

    void SetOptions(string material, string variant, string direction, Dictionary<TiletypeMaterial, Dictionary<TiletypeVariant, Dictionary<string, Tiletype>>> optionList, TiletypeMatch match)
    {
        if (material == "*")
        {
            match.difference |= 4;
            foreach (var item in optionList.Values)
            {
                SetOptions(variant, direction, item, match);
            }
        }
        else
        {
            try
            {
                TiletypeMaterial tileMaterial = (TiletypeMaterial)Enum.Parse(typeof(TiletypeMaterial), material);
                if (optionList.ContainsKey(tileMaterial))
                    SetOptions(variant, direction, optionList[tileMaterial], match);
            }
            catch (Exception)
            {
                Debug.LogError(material + " is not a valid tile material.");
                return;
            }
        }
    }
    void SetOptions(string special, string material, string variant, string direction,
        Dictionary<TiletypeSpecial,
        Dictionary<TiletypeMaterial,
        Dictionary<TiletypeVariant,
        Dictionary<string, Tiletype>>>> optionList, TiletypeMatch match)
    {
        if (special == "*")
        {
            match.difference |= 8;
            foreach (var item in optionList.Values)
            {
                SetOptions(material, variant, direction, item, match);
            }
        }
        else
        {
            try
            {
                TiletypeSpecial tileSpecial = (TiletypeSpecial)Enum.Parse(typeof(TiletypeSpecial), special);
                if (optionList.ContainsKey(tileSpecial))
                    SetOptions(material, variant, direction, optionList[tileSpecial], match);
            }
            catch (Exception)
            {
                Debug.LogError(special + " is not a valid tile special.");
                return;
            }
        }
    }
    void SetOptions(string shape, string special, string material, string variant, string direction, TiletypeMatch match)
    {
        if (shape == "*")
        {
            match.difference |= 16;
            foreach (var item in TiletypeTokenList.tileDefs.Values)
            {
                SetOptions(special, material, variant, direction, item, match);
            }
        }
        else
        {
            try
            {
                TiletypeShape tileShape = (TiletypeShape)Enum.Parse(typeof(TiletypeShape), shape);
                if (TiletypeTokenList.tileDefs.ContainsKey(tileShape))
                    SetOptions(special, material, variant, direction, TiletypeTokenList.tileDefs[tileShape], match);
            }
            catch (Exception)
            {
                Debug.LogError(shape + " is not a valid tile shape.");
                return;
            }
        }
    }

    public T this[string token]
    {
        set
        {
            string[] parts = token.Split(':');
            TiletypeMatch newItem;
            newItem.item = value;
            newItem.difference = 0;
            switch (parts.Length)
            {
                case 1:
                    if (TiletypeTokenList.tileTokens.ContainsKey(parts[0]))
                        TrySetMatch(newItem, TiletypeTokenList.tileTokens[parts[0]].id);
                    break;
                case 5:
                    newItem.difference |= 32;
                    SetOptions(parts[0], parts[1], parts[2], parts[3], parts[4], newItem);
                    break;
                default:
                    break;
            }
        }
    }
    public T this[int tiletype]
    {
        //get
        //{
        //    T output;
        //    Get(tiletype, out output);
        //    return output;
        //}
        set
        {
            if (tiletypeList == null)
                tiletypeList = new Dictionary<int, TiletypeMatch>();
            TiletypeMatch newItem;
            newItem.item = value;
            newItem.difference = 0;
            tiletypeList[tiletype] = newItem;
        }
    }
    public void Clear()
    {
        tiletypeList.Clear();
    }
    public bool Get(int tiletype, out T value)
    {
        if (tiletypeList != null)
        {
            TiletypeMatch output;
            if (tiletypeList.TryGetValue(tiletype, out output))
            {
                value = output.item;
                return true;
            }
        }
        value = default(T);
        return false;
    }
}

