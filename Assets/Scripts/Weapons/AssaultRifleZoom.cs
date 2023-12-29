using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AssaultRifleZoom : MonoBehaviour
{
    public GameObject zoom;
    public int defaultLayer;
    public int weaponsLayer;

    public void SetLayer(int layer)
    {
        zoom.layer = layer;
    }
}
