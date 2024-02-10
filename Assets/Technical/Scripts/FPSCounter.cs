using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FPSCounter : MonoBehaviour
{
    [SerializeField] TMP_Text text;
    private int lastFrameIndex;
    private float[] frameDeltaTimeArray;

    private void Awake()
    {
        frameDeltaTimeArray = new float[50];
    }

    private void Update()
    {
        frameDeltaTimeArray[lastFrameIndex] = Time.deltaTime;
        lastFrameIndex = (lastFrameIndex + 1) % frameDeltaTimeArray.Length;
        text.text = ((int) FPSCalculator()).ToString() + " " + Application.targetFrameRate;
    }

    private float FPSCalculator() 
    {
        float total = 0f;
        foreach (float deltaTime in frameDeltaTimeArray) {
            total += deltaTime;
        }

        return frameDeltaTimeArray.Length / total;
    }

}
