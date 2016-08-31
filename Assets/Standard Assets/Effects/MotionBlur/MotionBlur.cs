// Main part of the motion blur effect

using UnityEngine;

namespace UnityStandardAssets.CinematicEffects
{
    [RequireComponent(typeof(Camera))]
    [AddComponentMenu("Image Effects/Cinematic/Motion Blur")]
    public partial class MotionBlur : MonoBehaviour
    {
        #region Public properties

        /// Effect settings.
        public Settings settings
        {
            get { return _settings; }
            set { _settings = value; }
        }

        [SerializeField]
        Settings _settings = Settings.defaultSettings;

        #endregion

        #region Private properties and methods

        [SerializeField] Shader _reconstructionShader;
        [SerializeField] Shader _frameBlendingShader;

        ReconstructionFilter _reconstructionFilter;
        FrameBlendingFilter _frameBlendingFilter;

        #endregion

        #region MonoBehaviour functions

        #if UNITY_5_4_OR_NEWER

        void OnEnable()
        {
            _reconstructionFilter = new ReconstructionFilter();
            _frameBlendingFilter = new FrameBlendingFilter();
        }

        void OnDisable()
        {
            _reconstructionFilter.Release();
            _frameBlendingFilter.Release();

            _reconstructionFilter = null;
            _frameBlendingFilter = null;
        }

        void Update()
        {
            // Enable motion vector rendering if reuqired.
            if (_settings.shutterAngle > 0)
                GetComponent<Camera>().depthTextureMode |=
                    DepthTextureMode.Depth | DepthTextureMode.MotionVectors;
        }

        void OnRenderImage(RenderTexture source, RenderTexture destination)
        {
            if (_settings.shutterAngle > 0 && _settings.frameBlending > 0)
            {
                // Reconstruction and frame blending
                var temp = RenderTexture.GetTemporary(source.width, source.height, 0, source.format);
                _reconstructionFilter.ProcessImage(_settings.shutterAngle, _settings.sampleCount, source, temp);
                _frameBlendingFilter.BlendFrames(_settings.frameBlending, temp, destination);
                _frameBlendingFilter.PushFrame(temp);
                RenderTexture.ReleaseTemporary(temp);
            }
            else if (_settings.shutterAngle > 0)
            {
                // Reconstruction only
                _reconstructionFilter.ProcessImage(_settings.shutterAngle, _settings.sampleCount, source, destination);
            }
            else if (_settings.frameBlending > 0)
            {
                // Frame blending only
                _frameBlendingFilter.BlendFrames(_settings.frameBlending, source, destination);
                _frameBlendingFilter.PushFrame(source);
            }
            else
            {
                // Nothing to do!
                Graphics.Blit(source, destination);
            }
        }

        #endif

        #endregion
    }
}
