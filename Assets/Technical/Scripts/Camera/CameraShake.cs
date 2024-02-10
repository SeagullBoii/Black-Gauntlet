using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;

public class CameraShake : MonoBehaviour
{
    public static event Action<float, float, float, float, Transform, float> Shake;

    public static void Invoke(float duration, float positionStrength, float rotationStrength, float vibrato, Transform originTransform, float maxDistance)
    {
        Shake?.Invoke(duration, positionStrength, rotationStrength, vibrato, originTransform, maxDistance);
    }

    private void OnEnable() => Shake += ShakeScreen;
    private void OnDisable() => Shake -= ShakeScreen;

    public void ShakeScreen(float duration, float positionStrength, float rotationStrength, float vibrato, Transform originTransform, float maxDistance)
    {
        float distance = Mathf.Abs((transform.position - originTransform.position).magnitude);

        int vibrationAmount = 15;
        float shakeMultiplier = 0.1f;

        if (distance <= maxDistance)
        {
            shakeMultiplier = 1;
            vibrationAmount = (int) (vibrato * (maxDistance - distance) / maxDistance);
        }

        transform.DOComplete();
        transform.DOShakePosition(duration, positionStrength * shakeMultiplier, vibrationAmount);
        transform.DOShakeRotation(duration, rotationStrength * shakeMultiplier, vibrationAmount);
    }
}
