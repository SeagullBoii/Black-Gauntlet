using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpPadFunctionality : MonoBehaviour
{
    [Range(10, 3000)]
    public float jumpPadForce;
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.TryGetComponent(out Rigidbody rb))
             rb.AddForce(Vector3.up * jumpPadForce, ForceMode.Impulse);
    }
}
