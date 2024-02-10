using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StaminaBar : MonoBehaviour
{
    Slider slider;
    public GameObject player;
    public Text percentage;

    // Start is called before the first frame update
    void Start()
    {
        slider = GetComponent<Slider>();
    }

    // Update is called once per frame
    void Update()
    {
        if (player.TryGetComponent(out Dashing dash))
        {
            slider.maxValue = dash.dashCd;
            slider.value = dash.dashCd - dash.dashCdTimer;
            
        }
    }
}
