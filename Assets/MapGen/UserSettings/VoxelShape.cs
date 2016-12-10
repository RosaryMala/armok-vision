using System;

namespace UserSettings
{
    public class VoxelShape : SliderBase
    {
        protected override void InitValue()
        {
            slider.value = (float)GameSettings.Instance.meshing.cornerType;
            OnValueChanged(slider.value);
        }

        protected override void OnValueChanged(float value)
        {
            GameSettings.Instance.meshing.cornerType = (VoxelGenerator.CornerType)value;
            valueLabel.text = GameSettings.Instance.meshing.cornerType.ToString();
            gameMap.Refresh();
        }
    }
}
