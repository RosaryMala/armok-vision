using System;

namespace UserSettings
{
    public class ItemDistanceSlider : SliderBase
    {
        protected override void InitValue()
        {
            slider.value = GameSettings.Instance.rendering.itemDrawDistance;
            valueLabel.text = slider.value.ToString();
        }

        protected override void OnValueChanged(float value)
        {
            GameSettings.Instance.rendering.itemDrawDistance = value;
            valueLabel.text = value.ToString();
        }
    }
}
