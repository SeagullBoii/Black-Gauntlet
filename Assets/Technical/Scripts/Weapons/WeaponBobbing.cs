using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;

public class WeaponBobbing : MonoBehaviour
{
    public PlayerMovement pm;
    public Rigidbody rb;
    public WeaponAbilities weaponAbilities;

    float movementCounter;
    float idleCounter;
    Vector3 originalOffset;

    PlayerInput playerInput;

    private void Awake()
    {
        playerInput = new PlayerInput();
    }

    private void OnEnable()
    {
        playerInput.Enable();
    }

    private void OnDisable()
    {
        playerInput.Disable();
    }

    private void Start()
    {
        originalOffset = transform.localPosition;
    }

    private void Update()
    {
        Vector2 inputVector = playerInput.Movement.Move.ReadValue<Vector2>();
        bool slidingOnSlope = (pm.sliding || pm.crouching) && pm.OnSlope() && rb.velocity.y < 0.1f;

        if (inputVector.magnitude != 0f && pm.grounded && rb.velocity.magnitude != 0 && !slidingOnSlope && !pm.sliding)
        {
            HeadBob(movementCounter, 0.025f, 0.01f, 7);
            movementCounter += Time.deltaTime * 1.25f;
        }
        else if (pm.grounded)
        {
            HeadBob(idleCounter, 0.01f, 0.01f, 1);
            idleCounter += Time.deltaTime;
        }

        if (!pm.grounded)
        {
            transform.localPosition = Vector3.Lerp(transform.localPosition, Vector3.zero, 8 * Time.deltaTime);
        }
    }

    private void HeadBob(float z, float xIntensity, float yIntensity, float xMultiplier)
    {
        Vector3 normalTarget = new Vector3(Mathf.Cos(z * xMultiplier) * xIntensity, Mathf.Sin(z * xMultiplier * 2) * yIntensity, originalOffset.z);
        Vector3 target = Vector3.zero;

        if (weaponAbilities != null && weaponAbilities.aiming)
        {
            if (pm.grounded)
                target = Vector3.Scale(normalTarget, new Vector3(0.05f, 0.05f, 0.05f));
        }
        else
        {
            target = normalTarget;
        }
        transform.localPosition = Vector3.Lerp(transform.localPosition, target, 8 * Time.deltaTime);
    }

}
