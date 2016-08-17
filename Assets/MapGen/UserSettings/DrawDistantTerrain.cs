using System;

namespace UserSettings
{
    public class DrawDistantTerrain : SliderBase
    {
        protected override void InitValue()
        {
            slider.value = Convert.ToInt32(GameSettings.Instance.rendering.drawDistantTerrain);
            valueLabel.text = GameSettings.Instance.rendering.drawDistantTerrain.ToString();
        }

        protected override void OnValueChanged(float value)
        {
            GameSettings.Instance.rendering.drawDistantTerrain = Convert.ToBoolean(value);
            valueLabel.text = GameSettings.Instance.rendering.drawDistantTerrain.ToString();
        }
    }
}
