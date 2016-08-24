using System;
using UnityEngine;
using UnityStandardAssets.ImageEffects;

namespace UserSettings
{
    public class EnableFog : SliderBase
    {
        GlobalFog fogComponent;

        protected override void InitValue()
        {
            slider.value = Convert.ToInt32(GameSettings.Instance.rendering.fog);
            valueLabel.text = GameSettings.Instance.rendering.fog.ToString();
            RenderSettings.fog = GameSettings.Instance.rendering.fog;
            fogComponent = cam.GetComponent<GlobalFog>();
            fogComponent.enabled = GameSettings.Instance.rendering.fog;
        }

        protected override void OnValueChanged(float value)
        {
            GameSettings.Instance.rendering.fog = Convert.ToBoolean(value);
            valueLabel.text = GameSettings.Instance.rendering.fog.ToString();
            RenderSettings.fog = GameSettings.Instance.rendering.fog;
            fogComponent.enabled = GameSettings.Instance.rendering.fog;
        }
    }
}
