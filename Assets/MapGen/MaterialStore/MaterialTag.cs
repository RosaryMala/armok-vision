using System;


namespace MaterialStore
{
    [Serializable]
    public class MaterialTag
    {
        public MaterialType type = MaterialType.NONE;
        public string tag1;
        public string tag2;

        public MaterialTag()
        {
            type = MaterialType.NONE;
            tag1 = null;
            tag2 = null;
        }
        public MaterialTag(MaterialType type = MaterialType.NONE, string tag1 = null, string tag2 = null)
        {
            this.type = type;
            this.tag1 = tag1;
            this.tag2 = tag2;
        }

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
            if (value.Contains(":"))
            {
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
                if (values.Length > 1)
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
            else
            {
                var values = value.Split('-');
                if (values[0] == "_")
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
                if (values.Length > 1)
                {
                    if (values[1] != "_")
                        result.tag1 = values[1];
                }
                if (values.Length > 2)
                {
                    if (values[2] != "_")
                        result.tag2 = values[2];
                }
                return true;
            }
        }
        public void SetBasic(MatBasic basic)
        {
            switch (basic)
            {
                case MatBasic.INVALID:
                    type = MaterialType.NONE;
                    break;
                case MatBasic.INORGANIC:
                    type = MaterialType.INORGANIC;
                    break;
                case MatBasic.AMBER:
                    type = MaterialType.AMBER;
                    break;
                case MatBasic.CORAL:
                    type = MaterialType.CORAL;
                    break;
                case MatBasic.GREEN_GLASS:
                    type = MaterialType.GLASS_GREEN;
                    break;
                case MatBasic.CLEAR_GLASS:
                    type = MaterialType.GLASS_CLEAR;
                    break;
                case MatBasic.CRYSTAL_GLASS:
                    type = MaterialType.GLASS_CRYSTAL;
                    break;
                case MatBasic.ICE:
                    type = MaterialType.WATER;
                    break;
                case MatBasic.COAL:
                    type = MaterialType.COAL;
                    break;
                case MatBasic.POTASH:
                    type = MaterialType.POTASH;
                    break;
                case MatBasic.ASH:
                    type = MaterialType.ASH;
                    break;
                case MatBasic.PEARLASH:
                    type = MaterialType.PEARLASH;
                    break;
                case MatBasic.LYE:
                    type = MaterialType.LYE;
                    break;
                case MatBasic.MUD:
                    type = MaterialType.MUD;
                    break;
                case MatBasic.VOMIT:
                    type = MaterialType.VOMIT;
                    break;
                case MatBasic.SALT:
                    type = MaterialType.SALT;
                    break;
                case MatBasic.FILTH:
                    type = MaterialType.FILTH_B;
                    break;
                case MatBasic.FILTH_FROZEN:
                    type = MaterialType.FILTH_Y;
                    break;
                case MatBasic.UNKOWN_FROZEN:
                    type = MaterialType.UNKNOWN_SUBSTANCE;
                    break;
                case MatBasic.GRIME:
                    type = MaterialType.GRIME;
                    break;
                case MatBasic.ICHOR:
                    type = MaterialType.CREATURE;
                    tag2 = "ICHOR";
                    break;
                case MatBasic.LEATHER:
                    type = MaterialType.CREATURE;
                    tag2 = "LEATHER";
                    break;
                case MatBasic.BLOOD_1:
                case MatBasic.BLOOD_2:
                case MatBasic.BLOOD_3:
                case MatBasic.BLOOD_4:
                case MatBasic.BLOOD_5:
                case MatBasic.BLOOD_6:
                case MatBasic.BLOOD_NAMED:
                    type = MaterialType.CREATURE;
                    tag2 = "BLOOD";
                    break;
                case MatBasic.PLANT:
                    type = MaterialType.PLANT;
                    break;
                case MatBasic.WOOD:
                    type = MaterialType.PLANT;
                    tag2 = "WOOD";
                    break;
                case MatBasic.PLANTCLOTH:
                    type = MaterialType.PLANT;
                    tag2 = "THREAD";
                    break;
                case MatBasic.DESIGNATION:
                    type = MaterialType.NONE;
                    break;
                case MatBasic.CONSTRUCTION:
                    type = MaterialType.NONE;
                    break;
                default:
                    type = MaterialType.NONE;
                    break;
            }
        }

        public override string ToString()
        {
            string value = type.ToString();
            if (NumTags > 1)
                value += ":" + (string.IsNullOrEmpty(tag1) ? "*" : tag1);
            if (NumTags > 2)
                value += ":" + (string.IsNullOrEmpty(tag2) ? "*" : tag2);
            return value;
        }

        public string ToFileName()
        {
            string value = type.ToString();
            if (NumTags > 1)
                value += "-" + (string.IsNullOrEmpty(tag1) ? "_" : tag1);
            if (NumTags > 2)
                value += "-" + (string.IsNullOrEmpty(tag2) ? "_" : tag2);
            return value;
        }

        public int NumTags
        {
            get
            {
                switch (type)
                {
                    case MaterialType.INORGANIC:
                    case MaterialType.COAL:
                        return 2;
                    case MaterialType.CREATURE:
                    case MaterialType.PLANT:
                        return 3;
                    default:
                        return 1;
                }
            }
        }
    }
}
