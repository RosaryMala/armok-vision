using System;

namespace UserSettings
{
    public class DrawRangeUp : SliderBase
    {
        protected override void InitValue()
        {
            slider.value = GameSettings.Instance.rendering.drawRangeUp;
            valueLabel.text = ((int)slider.value).ToString();
        }

        protected override void OnValueChanged(float value)
        {
            GameSettings.Instance.rendering.drawRangeUp = (int)value;
            valueLabel.text = ((int)value).ToString();
        }
    }
}
