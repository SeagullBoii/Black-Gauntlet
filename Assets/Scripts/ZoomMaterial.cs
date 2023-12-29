using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZoomMaterial : MonoBehaviour
{
    public Material mat;

    private void Update()
    {
        Vector2 screenPixels = Camera.main.WorldToScreenPoint(transform.position);
        screenPixels = new Vector2(screenPixels.x / Screen.width, screenPixels.y / Screen.height);

        mat.SetVector("_objectScreenPosition", screenPixels);
    }
}
