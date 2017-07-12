using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MaterialStore
{
    [System.Serializable]
    public class MaterialTag
    {
        public MaterialType type;
        public string tag1;
        public string tag2;

        public static MaterialTag Parse(string value)
        {
            MaterialTag result;
            if (!TryParse(value, out result))
                throw new FormatException();
            return result;
        }

        public static bool TryParse(string value, out MaterialTag result)
        {
            result = new MaterialTag();
            var values = value.Split(':');
            if (values[0] == "*")
                result.type = MaterialType.NONE;
            else
            {
                try
                {
                    result.type = (MaterialType)Enum.Parse(typeof(MaterialType), values[0]);
                }
                catch (Exception)
                {
                    return false;
                }
            }
            if(values.Length > 1)
            {
                if (values[1] != "*")
                    result.tag1 = values[1];
            }
            if (values.Length > 2)
            {
                if (values[2] != "*")
                    result.tag2 = values[2];
            }
            return true;
        }
    }
}
