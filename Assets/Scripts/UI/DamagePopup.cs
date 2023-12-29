using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DamagePopup : MonoBehaviour
{
    public float destroyTime = 1.5f;
    public float damage { get; set; }
    public Vector3 randomizeIntensity = new Vector3(0.5f, 0, 0);

    TMP_Text text;

    private void Start()
    {
        text = GetComponent<TMP_Text>();
        text.text = damage.ToString();

        float xRandomization = Random.Range(-randomizeIntensity.x, randomizeIntensity.x);
        float yRandomization = Random.Range(-randomizeIntensity.y, randomizeIntensity.y);
        float zRandomization = Random.Range(-randomizeIntensity.z, randomizeIntensity.z);

        Vector3 randomPos = new Vector3(xRandomization, yRandomization, zRandomization);

        transform.localPosition += randomPos;

        Destroy(gameObject, destroyTime);
    }

    private void FixedUpdate()
    {
        transform.LookAt(Camera.main.transform);
    }
}
