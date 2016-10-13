using System;

namespace UserSettings
{
    public class DeferredRendering : SliderBase
    {
        protected override void InitValue()
        {
            slider.value = Convert.ToInt32(GameSettings.Instance.camera.deferredRendering);
            OnValueChanged(slider.value);
        }

        protected override void OnValueChanged(float value)
        {
            GameSettings.Instance.camera.deferredRendering = Convert.ToBoolean(value);
            switch (GameSettings.Instance.camera.deferredRendering)
            {
                case true:
                    cam.renderingPath = UnityEngine.RenderingPath.DeferredShading;
                    valueLabel.text = "Deferred";
                    break;
                case false:
                    cam.renderingPath = UnityEngine.RenderingPath.Forward;
                    valueLabel.text = "Forward";
                    GameSettings.Instance.camera.postProcessing = false;
                    break;
            }
        }
    }
}
