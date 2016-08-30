using System;

namespace UserSettings
{
    public class DeferredRendering : SliderBase
    {
        protected override void InitValue()
        {
            slider.value = GameSettings.Instance.rendering.deferredRendering;
            OnValueChanged(slider.value);
        }

        protected override void OnValueChanged(float value)
        {
            GameSettings.Instance.rendering.deferredRendering = (int)value;
            switch (GameSettings.Instance.rendering.deferredRendering)
            {
                case 2:
                    cam.renderingPath = UnityEngine.RenderingPath.DeferredShading;
                    valueLabel.text = "Deferred";
                    break;
                case 1:
                    cam.renderingPath = UnityEngine.RenderingPath.Forward;
                    valueLabel.text = "Forward";
                    break;
                case 0:
                    cam.renderingPath = UnityEngine.RenderingPath.VertexLit;
                    valueLabel.text = "Vertex Lit";
                    break;
                default:
                    break;
            }
        }
    }
}
