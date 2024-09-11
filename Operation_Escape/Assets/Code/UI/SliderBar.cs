using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SliderBar : MonoBehaviour
{
    public Slider slider;
    public float value;
    public GameObject bar;
    public bool canShow;
    private void Update()
    {
        if (slider.value != value)
        {
            slider.value = Mathf.Lerp(slider.value, value, 0.1f);
        }
    }

    public void SetMax(float _value)
    {
        slider.maxValue = _value;
        slider.value = _value;
        value = _value;
    }

    public void SetMax(float _valueMax,float _value)
    {
        slider.maxValue = _valueMax;
        slider.value = _value;
        value = _value;
    }

    public void SetValue(float _value)
    {
        if (_value < slider.minValue)
        {
            Debug.Log(_value);
            value = slider.minValue;
        }

        value = _value;
    }

    public void Off(bool off)
    {
        canShow = off;
        bar.SetActive(canShow);
    }
}
