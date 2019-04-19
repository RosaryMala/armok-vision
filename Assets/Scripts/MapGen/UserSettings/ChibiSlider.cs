using System;

namespace UserSettings
{
    public class ChibiSlider : SliderBase
    {
        protected override void InitValue()
        {
            slider.value = GameSettings.Instance.units.chibiness;
            valueLabel.text = slider.value.ToString();
        }

        protected override void OnValueChanged(float value)
        {
            GameSettings.Instance.units.chibiness = value;
            valueLabel.text = value.ToString();
        }
    }
}
