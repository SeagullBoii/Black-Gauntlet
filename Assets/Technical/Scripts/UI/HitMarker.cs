using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HitMarker : MonoBehaviour
{
    public Weapons weapons;
    private void Update()
    {
        gameObject.SetActive(weapons.didHit);
    }
}
