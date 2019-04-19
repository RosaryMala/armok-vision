using System;

namespace UserSettings
{
    public class DwarfDistanceSlider : SliderBase
    {
        protected override void InitValue()
        {
            slider.value = GameSettings.Instance.rendering.creatureDrawDistance;
            valueLabel.text = slider.value.ToString();
        }

        protected override void OnValueChanged(float value)
        {
            GameSettings.Instance.rendering.creatureDrawDistance = value;
            valueLabel.text = value.ToString();
        }
    }
}
