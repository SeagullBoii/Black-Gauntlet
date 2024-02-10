using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SpeedText : MonoBehaviour
{
    public GameObject player;
    public Rigidbody rb;
    public bool horizontal;
    private TMP_Text text;

    // Update is called once per frame
    private void Start()
    {
        text = GetComponent<TMP_Text>();
    }

    private void Update()
    {
        if (player.TryGetComponent(out PlayerMovement pm))
        {
            Vector3 velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
            text.text = "Speed: " + (horizontal ? (int)velocity.magnitude : (int)rb.velocity.magnitude);
        }
    }
}
