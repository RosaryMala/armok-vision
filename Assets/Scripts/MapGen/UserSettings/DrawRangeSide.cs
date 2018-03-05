using System;

namespace UserSettings
{
    public class DrawRangeSide : SliderBase
    {
        protected override void InitValue()
        {
            slider.value = GameSettings.Instance.rendering.drawRangeSide;
            valueLabel.text = ((int)slider.value).ToString();
        }

        protected override void OnValueChanged(float value)
        {
            GameSettings.Instance.rendering.drawRangeSide = (int)value;
            valueLabel.text = ((int)value).ToString();
        }
    }
}
