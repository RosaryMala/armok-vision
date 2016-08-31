using System;
using UnityStandardAssets.CinematicEffects;

namespace UserSettings
{
    public class EnableSSAO : SliderBase
    {
        AmbientOcclusion ssao;

        protected override void InitValue()
        {
            slider.value = Convert.ToInt32(GameSettings.Instance.camera.SSAO);
            valueLabel.text = GameSettings.Instance.camera.SSAO.ToString();
            ssao = cam.GetComponent<AmbientOcclusion>();
            ssao.enabled = GameSettings.Instance.camera.SSAO;
        }

        protected override void OnValueChanged(float value)
        {
            GameSettings.Instance.camera.SSAO = Convert.ToBoolean(value);
            valueLabel.text = GameSettings.Instance.camera.SSAO.ToString();
            ssao.enabled = GameSettings.Instance.camera.SSAO;
        }
    }
}
