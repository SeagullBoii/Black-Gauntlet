using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SliderPercentageText : MonoBehaviour
{
    public Slider slider;
    public bool showValue;
    public bool decimals;
    public int decimalCount;
    public float multiplier = 1;
    TMP_Text text;

    private void Start()
    {
        text = GetComponent<TMP_Text>();
    }

    private void Update()
    {
        if (!showValue)
        {
            text.text = slider.minValue < 0 ? (int)((slider.value + Mathf.Abs(slider.minValue)) * 100 / (slider.maxValue + Mathf.Abs(slider.minValue))) + "%" : (int)Mathf.Round((slider.value * 100 / slider.maxValue)) + "%";
            if (slider.minValue >= 0 && slider.value == slider.minValue) text.text = "0%";
        }

        else
        {
            ShowValue();
        }
    }

    private void ShowValue()
    {
        float decimalAmount = Mathf.Pow(10, decimalCount);
        if (decimals)
            text.text = (Mathf.Round(slider.value * multiplier * decimalAmount) / decimalAmount).ToString();
        else
            text.text = ((int)slider.value * multiplier).ToString();
    }
}
