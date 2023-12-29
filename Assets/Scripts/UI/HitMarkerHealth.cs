using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class HitMarkerHealth : MonoBehaviour
{
    public float health;

    // Start is called before the first frame update
    void Update()
    {
        if (gameObject.TryGetComponent<TextMesh>(out TextMesh text)) text.text = health.ToString();

        Destroy(gameObject, 1f);    
    }
}