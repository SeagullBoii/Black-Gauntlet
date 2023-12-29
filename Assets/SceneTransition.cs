using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneTransition : MonoBehaviour
{
    [SerializeField] Material mat;
    [SerializeField] float transitionTime = 1;
    [SerializeField] string propertyName = "_Progress";

    float vel;

    private void Start()
    {
        StartCoroutine(TransitionCoroutine());
    }

    private IEnumerator TransitionCoroutine()
    {
        float currentTime = 0;
        while (currentTime < transitionTime)
        {
            currentTime += Time.unscaledDeltaTime;
            mat.SetFloat(propertyName, Mathf.Clamp01(currentTime / transitionTime));
            yield return null;
        }
    }
}
