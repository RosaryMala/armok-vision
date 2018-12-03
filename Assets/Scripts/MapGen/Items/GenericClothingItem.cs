using UnityEngine;

public class GenericClothingItem : MonoBehaviour
{
    public enum DressUsage
    {
        Any,
        DressOnly,
        NotDress
    }
    public DressUsage isDress = DressUsage.Any;
}
