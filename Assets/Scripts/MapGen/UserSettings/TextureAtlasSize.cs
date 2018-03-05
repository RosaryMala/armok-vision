using UnityEngine;

namespace UserSettings
{
    public class TextureAtlasSize : SliderBase
    {
        protected override void InitValue()
        {
            slider.value = Mathf.Log(GameSettings.Instance.rendering.textureAtlasSize) / Mathf.Log(2);
            valueLabel.text = GameSettings.Instance.rendering.textureAtlasSize.ToString();
        }

        protected override void OnValueChanged(float value)
        {
            GameSettings.Instance.rendering.textureAtlasSize = (int)Mathf.Pow(2, (int)value); //converting value to int to make sure it's a pure power of 2
            valueLabel.text = GameSettings.Instance.rendering.textureAtlasSize.ToString();
        }
    }
}
