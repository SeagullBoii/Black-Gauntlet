using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnscaledTimeShader : MonoBehaviour
{
    public Material mat;
    void Update()
    {
        mat.SetFloat("_UnscaledTime", Time.unscaledTime);
    }
}
