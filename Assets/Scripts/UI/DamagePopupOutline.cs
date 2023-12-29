using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamagePopupOutline : MonoBehaviour
{
    public GameObject parent;
    TextMesh textMesh;
    // Start is called before the first frame update
    void Start()
    {
        textMesh = GetComponent<TextMesh>();
    }

    private void Update()
    {
        if (parent.TryGetComponent<DamagePopup>(out DamagePopup damagePopup)) textMesh.text = damagePopup.damage.ToString();
    }
}
