using System;
using UnityEngine;
using UnityEngine.PostProcessing;
using UnityStandardAssets.CinematicEffects;

namespace UserSettings
{
    public class EnablePostProcessing : SliderBase
    {
        PostProcessingBehaviour post;

        Camera skyCam;

        protected override void InitValue()
        {
            skyCam = GameObject.Find("Sky Camera").GetComponent<Camera>();
            slider.value = Convert.ToInt32(GameSettings.Instance.camera.postProcessing);
            valueLabel.text = GameSettings.Instance.camera.postProcessing.ToString();
            post = cam.GetComponent<PostProcessingBehaviour>();
            post.enabled = GameSettings.Instance.camera.postProcessing;
            cam.hdr = GameSettings.Instance.camera.postProcessing;
            skyCam.hdr = GameSettings.Instance.camera.postProcessing; ;
        }

        protected override void OnValueChanged(float value)
        {
            slider.value = value;
            GameSettings.Instance.camera.postProcessing = Convert.ToBoolean(value);
            valueLabel.text = GameSettings.Instance.camera.postProcessing.ToString();
            post.enabled = GameSettings.Instance.camera.postProcessing;
            cam.hdr = GameSettings.Instance.camera.postProcessing;
            skyCam.hdr = GameSettings.Instance.camera.postProcessing; ;
        }

        bool oldValue = false;

        void Update()
        {
            if (oldValue != GameSettings.Instance.camera.postProcessing)
            {
                OnValueChanged(Convert.ToInt32(GameSettings.Instance.camera.postProcessing));
                oldValue = GameSettings.Instance.camera.postProcessing;
            }
        }
    }
}
