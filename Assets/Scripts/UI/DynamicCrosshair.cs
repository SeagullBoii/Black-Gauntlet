using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DynamicCrosshair : MonoBehaviour
{
    public float distance;
    public float returnSpeed;

    Vector2 startSize;
    RectTransform rectTransform;

    private void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        startSize = rectTransform.sizeDelta;
    }

    private void FixedUpdate()
    {
        rectTransform.sizeDelta = Vector2.Lerp(rectTransform.sizeDelta, startSize, returnSpeed * Time.deltaTime);
    }

    public void MoveParts()
    {
        rectTransform.sizeDelta = new Vector2(startSize.x + distance, startSize.y);
    }
}
