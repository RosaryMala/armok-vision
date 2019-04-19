using UnityEngine;
namespace RemoteFortressReader
{
    public partial class ColorDefinition
    {
        public static implicit operator Color32(ColorDefinition definition)
        {
            return new Color32((byte)definition.red, (byte)definition.green, (byte)definition.blue, 255);
        }
        public static implicit operator Color(ColorDefinition definition)
        {
            return new Color32((byte)definition.red, (byte)definition.green, (byte)definition.blue, 255);
        }
    }
}

