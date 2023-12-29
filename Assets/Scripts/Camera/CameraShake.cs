using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;

public class CameraShake : MonoBehaviour
{
    public void ShakeScreen(float duration, float positionStrength, float rotationStrength)
    {
        transform.DOComplete();
        transform.DOShakePosition(duration, positionStrength);
        transform.DOShakeRotation(duration, rotationStrength);
    }
}
