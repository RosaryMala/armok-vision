using RemoteFortressReader;
using UnityEngine;

public class UnitScaler : MonoBehaviour {

    internal void UpdateSize(UnitDefinition unit, LayeredSprite layeredSprite)
    {
        float scale = 1;
        if (GameSettings.Instance.units.scaleUnits)
        {
            float baseSize = 391.0f;
            if (layeredSprite != null && layeredSprite.spriteCollection != null && layeredSprite.spriteCollection.standardLength > 0)
            {
                baseSize = layeredSprite.spriteCollection.standardLength;
            }
            scale = unit.size_info.length_cur / baseSize;
        }
        transform.localScale = new Vector3(scale, scale, scale);
    }
}
