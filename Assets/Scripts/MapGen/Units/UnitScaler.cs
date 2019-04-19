using RemoteFortressReader;
using UnityEngine;

public class UnitScaler : MonoBehaviour {

    const int HumanSize = 7000;
    static float logMultiplier = HumanSize / Mathf.Pow(HumanSize, 1/2f);

    public static float GetAdjustedUnitSize(float unitSize)
    {
        switch (GameSettings.Instance.units.scaleUnits)
        {
            case GameSettings.UnitScale.Fixed:
                return HumanSize;
            case GameSettings.UnitScale.Logarithmic:
                return Mathf.Pow(Mathf.Max(unitSize, 1), 1/2f) * logMultiplier;
            case GameSettings.UnitScale.Real:
            default:
                return Mathf.Max(unitSize, 1);
        }
    }

    internal void UpdateSize(UnitDefinition unit, LayeredSprite layeredSprite)
    {
        float scale = 1;
        if (GameSettings.Instance.units.scaleUnits != GameSettings.UnitScale.Fixed && unit.size_info != null)
        {
            float baseSize = 391.0f;
            if (layeredSprite != null && layeredSprite.SpriteCollection != null && layeredSprite.SpriteCollection.standardLength > 0)
            {
                baseSize = layeredSprite.SpriteCollection.standardLength;
            }
            scale = Mathf.Pow(GetAdjustedUnitSize(unit.size_info.size_cur), 1/3f) / baseSize;
        }
        transform.localScale = new Vector3(scale, scale, scale);
    }
}
