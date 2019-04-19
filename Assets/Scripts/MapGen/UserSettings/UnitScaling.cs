using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UserSettings
{
    public class UnitScaling : SliderBase
    {
        private GameSettings.UnitScale oldUnitScale;

        protected override void InitValue()
        {
            slider.value = (int)GameSettings.Instance.units.scaleUnits;
            OnValueChanged(slider.value);
        }

        protected override void OnValueChanged(float value)
        {
            GameSettings.Instance.units.scaleUnits = (GameSettings.UnitScale)Mathf.RoundToInt(value);
            oldUnitScale = GameSettings.Instance.units.scaleUnits;
            switch (GameSettings.Instance.units.scaleUnits)
            {
                case GameSettings.UnitScale.Fixed:
                    valueLabel.text = "Fixed Size";
                    break;
                case GameSettings.UnitScale.Logarithmic:
                    valueLabel.text = "Logarithmic Scaling";
                    break;
                case GameSettings.UnitScale.Real:
                default:
                    valueLabel.text = "Actual Size";
                    break;
            }
        }

        private void Update()
        {
            if (GameSettings.Instance.units.scaleUnits != oldUnitScale)
            {
                OnValueChanged((float)GameSettings.Instance.units.scaleUnits);
            }
        }
    }
}
