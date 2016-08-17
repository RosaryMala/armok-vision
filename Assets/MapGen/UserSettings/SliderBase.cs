using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace UserSettings
{
    [RequireComponent(typeof(Slider))]
    public abstract class SliderBase : MonoBehaviour
    {
        protected Slider slider;
        protected Text valueLabel;
        public int Value
        {
            get
            {
                return (int)slider.value;
            }
        }
        protected GameMap gameMap;

        // Awake is called when the script instance is being loaded
        public void Awake()
        {
            slider = GetComponent<Slider>();
            valueLabel = transform.FindChild("Value").GetComponent<Text>();
            gameMap = FindObjectOfType<GameMap>();
        }

        // Start is called just before any of the Update methods is called the first time
        public void Start()
        {
            InitValue();
            slider.onValueChanged.AddListener(OnValueChanged);
        }

        protected abstract void OnValueChanged(float value);
        protected abstract void InitValue();
    }
}
