using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveCameraToThePlayer : MonoBehaviour
{
    public Transform cameraPos;
    // Update is called once per frame
    void Update()
    {
        transform.position = cameraPos.position;
    }
}
