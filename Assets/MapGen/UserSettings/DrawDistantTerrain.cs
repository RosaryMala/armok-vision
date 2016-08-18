using System;

namespace UserSettings
{
    public class DrawDistantTerrain : SliderBase
    {
        protected override void InitValue()
        {
            slider.value = (int)GameSettings.Instance.rendering.distantTerrainDetail;
            valueLabel.text = GameSettings.Instance.rendering.distantTerrainDetail.ToString();
        }

        protected override void OnValueChanged(float value)
        {
            GameSettings.Instance.rendering.distantTerrainDetail = (GameSettings.LandscapeDetail)value;
            valueLabel.text = GameSettings.Instance.rendering.distantTerrainDetail.ToString();
        }
    }
}
