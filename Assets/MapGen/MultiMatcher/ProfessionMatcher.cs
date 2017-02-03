using DF.Enums;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProfessionMatcher<T>
{
    T _default;
    Dictionary<string, T> professionCollection = new Dictionary<string, T>();

    public ProfessionMatcher(T def)
    {
        _default = def;
    }

    public T this[string token]
    {
        set
        {
            if (token == "DEFAULT")
                _default = value;
            else
                professionCollection[token] = value;
        }
        get
        {
            if (professionCollection.ContainsKey(token))
                return professionCollection[token];
            else
                return _default;
        }
    }

    public T this[profession id]
    {
        set
        {
            if (id == profession.NONE)
                _default = value;
            else
                this[id.ToString()] = value;
        }
        get
        {
            if (id == profession.NONE)
                return _default;
            else
                return this[id.ToString()];
        }
    }

    public bool TryGetValue(string token, out T result)
    {
        if (professionCollection.ContainsKey(token))
        {
            result = professionCollection[token];
            return true;
        }
        else
        {
            result = _default;
            return true;
        }
    }

    public bool TryGetValue(profession id, out T result)
    {
        return TryGetValue(id.ToString(), out result);
    }
}
