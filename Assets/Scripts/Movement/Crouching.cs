using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(PlayerMovement))]
public class Crouching : MonoBehaviour
{
    [Header("Crouching")]
    public float crouchForce;

    public float crouchYScale;

    [Header("Input")]
    public KeyCode crouchKey = KeyCode.LeftControl;

    [Header("References")]
    public Transform orientation;
    public Transform playerObj;

    //Sliding
    float startYScale;
    bool safeToStandUp;

    //References
    Rigidbody rb;
    PlayerMovement pm;
    CapsuleCollider collider;
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
        rb = GetComponent<Rigidbody>();
        pm = GetComponent<PlayerMovement>();
        collider = GetComponent<CapsuleCollider>();

        collider.material.staticFriction = 0f;
        collider.material.dynamicFriction = 0f;
        startYScale = collider.height;
    }

    Vector2 movementInput;
    private void Update()
    {
        movementInput = playerInput.Movement.Move.ReadValue<Vector2>();

        if (Time.timeScale != 0)
        {
            safeToStandUp = !Physics.Raycast(transform.position, Vector3.up, startYScale / 2 + 0.3f);

            Vector3 velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);

            if (playerInput.Movement.Slide.triggered && velocity.magnitude <= 1f)
            {
                StartCrouch();
            }

            if (playerInput.Movement.Slide.ReadValue<float>() != 1 && pm.crouching && safeToStandUp) StopCrouch();
        }
    }

    private void FixedUpdate()
    {
        if (pm.crouching) CrouchMovement();
    }

    public void StartCrouch()
    {
        if (pm.grounded)
            rb.AddForce(Vector3.down * 200f, ForceMode.Force);
        pm.crouching = true;
        playerObj.localScale = new Vector3(playerObj.localScale.x, crouchYScale / 2, playerObj.localScale.z);
        collider.height = crouchYScale;
    }

    private void CrouchMovement()
    {
        Vector3 inputDirection = orientation.forward * movementInput.y + orientation.right * movementInput.x / 10;

        //Normal Crouch
        if (!pm.OnSlope() || rb.velocity.y > -0.1f)
        {
            rb.AddForce(inputDirection.normalized * crouchYScale, ForceMode.Force);
        }
        //Sliding on a Slope
        else
        {
            rb.AddForce(pm.GetSlopeDirection(inputDirection) * crouchYScale, ForceMode.Force);

        }
    }

    private void StopCrouch()
    {
        pm.crouching = false;
        playerObj.localScale = new Vector3(playerObj.localScale.x, startYScale / 2, playerObj.localScale.z);
        collider.height = startYScale;
    }
}