using UnityEngine;

namespace UserSettings
{
    public class MaxTextureSize : SliderBase
    {
        protected override void InitValue()
        {
            slider.value = Mathf.Log(GameSettings.Instance.rendering.maxTextureSize) / Mathf.Log(2);
            valueLabel.text = GameSettings.Instance.rendering.maxTextureSize.ToString();
        }

        protected override void OnValueChanged(float value)
        {
            GameSettings.Instance.rendering.maxTextureSize = (int)Mathf.Pow(2, (int)value); //converting value to int to make sure it's a pure power of 2
            valueLabel.text = GameSettings.Instance.rendering.maxTextureSize.ToString();
        }
    }
}
