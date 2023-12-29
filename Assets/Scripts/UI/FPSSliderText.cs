using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FPSSliderText : MonoBehaviour
{
    public Slider slider;
    public float multiplier = 10;
    TMP_Text text;

    private void Start()
    {
        text = GetComponent<TMP_Text>();
    }

    void Update()
    {
        if (slider.value == slider.maxValue)
            text.text = "Unlimited FPS";
        else
            text.text = ((int)slider.value * multiplier).ToString() + " FPS";
    }
}
