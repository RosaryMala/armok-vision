using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

[RequireComponent(typeof(Slider))]
public class UpdateLabel : MonoBehaviour
{
    Text valueLabel;
    Slider slider;

    public enum ValueType
    {
        Number,
        Bool,
        Custom
    }

    public ValueType valueType;

    public string[] customValues;

    void Awake()
    {
        valueLabel = transform.FindChild("Value").GetComponent<Text>();
        slider = GetComponent<Slider>();
        slider.onValueChanged.AddListener(SetLabel);
    }

    void Start()
    {
        SetLabel(slider.value);
    }

    public void SetLabel(float value)
    {
        switch (valueType)
        {
            case ValueType.Number:
                valueLabel.text = ((int)value).ToString();
                break;
            case ValueType.Bool:
                valueLabel.text = Convert.ToBoolean(value).ToString();
                break;
            case ValueType.Custom:
                int index = (int)value;
                if (index >= 0 && index < customValues.Length)
                    valueLabel.text = customValues[index];
                else
                    valueLabel.text = index.ToString();
                break;
            default:
                break;
        }
    }
}
