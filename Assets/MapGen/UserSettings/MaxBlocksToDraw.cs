
namespace UserSettings
{
    public class MaxBlocksToDraw : SliderBase
    {
        protected override void InitValue()
        {
            slider.value = GameSettings.Instance.rendering.maxBlocksToDraw;
            valueLabel.text = ((int)slider.value).ToString();
        }

        protected override void OnValueChanged(float value)
        {
            GameSettings.Instance.rendering.maxBlocksToDraw = (int)value;
            valueLabel.text = ((int)value).ToString();
        }
    }
}
