using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UserSettings
{
    public class CreatureDetail : SliderBase
    {
        protected override void InitValue()
        {
            slider.value = (int)GameSettings.Instance.units.unitDetail;
            OnValueChanged(slider.value);
        }

        protected override void OnValueChanged(float value)
        {
            GameSettings.Instance.units.unitDetail = (GameSettings.UnitDetail)Mathf.RoundToInt(value);
            switch (GameSettings.Instance.units.unitDetail)
            {
                case GameSettings.UnitDetail.None:
                    valueLabel.text = "None";
                    break;
                case GameSettings.UnitDetail.ASCII:
                    valueLabel.text = "ASCII";
                    break;
                case GameSettings.UnitDetail.SDSprites:
                    valueLabel.text = "Low Detail Sprites";
                    break;
                case GameSettings.UnitDetail.HDSprites:
                    valueLabel.text = "High Detail Sprites";
                    break;
                case GameSettings.UnitDetail.Models:
                    valueLabel.text = "3D Models";
                    break;
                default:
                    break;
            }
        }
    }
}
