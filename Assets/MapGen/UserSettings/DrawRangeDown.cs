using System;

namespace UserSettings
{
    public class DrawRangeDown : SliderBase
    {
        protected override void InitValue()
        {
            slider.value = GameSettings.Instance.rendering.drawRangeDown;
            valueLabel.text = ((int)slider.value).ToString();
        }

        protected override void OnValueChanged(float value)
        {
            GameSettings.Instance.rendering.drawRangeDown = (int)value;
            valueLabel.text = ((int)value).ToString();
        }
    }
}
