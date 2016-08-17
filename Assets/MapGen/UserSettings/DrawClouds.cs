using System;

namespace UserSettings
{
    public class DrawClouds : SliderBase
    {
        protected override void InitValue()
        {
            slider.value = Convert.ToInt32(GameSettings.Instance.rendering.drawClouds);
            valueLabel.text = GameSettings.Instance.rendering.drawClouds.ToString();
        }

        protected override void OnValueChanged(float value)
        {
            GameSettings.Instance.rendering.drawClouds = Convert.ToBoolean(value);
            valueLabel.text = GameSettings.Instance.rendering.drawClouds.ToString();
        }
    }
}
