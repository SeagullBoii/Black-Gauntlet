using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(PlayerMovement))]
[RequireComponent(typeof(Crouching))]
public class Sliding : MonoBehaviour
{
    [Header("Sliding")]
    public float slideForce;
    public float slideDuration;
    public float slideYScale;

    [Header("Input")]
    public KeyCode slideKey = KeyCode.LeftControl;

    [Header("References")]
    public Transform orientation;
    public Transform playerObj;
    public WeaponCameraTiltAndFOV weaponCam;
    public GameObject cam;

    //Sliding
    float startYScale;
    float slideTime;
    bool safeToStandUp;

    //References
    Rigidbody rb;
    PlayerMovement pm;
    Crouching crouch;
    CapsuleCollider playerCollider;

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
        crouch = GetComponent<Crouching>();
        playerCollider = GetComponent<CapsuleCollider>();

        playerCollider.material.staticFriction = 0f;
        playerCollider.material.dynamicFriction = 0f;
        startYScale = playerCollider.height;
    }

    Vector2 movementInput;
    private void Update()
    {
        if (Time.timeScale != 0)
        {
            movementInput = playerInput.Movement.Move.ReadValue<Vector2>();
            safeToStandUp = !Physics.Raycast(transform.position, Vector3.up, startYScale / 2 + 0.3f);

            Vector3 velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);

            if (playerInput.Movement.Slide.triggered && velocity.magnitude > 1f) StartSlide();

            if (playerInput.Movement.Slide.ReadValue<float>() != 1 && pm.sliding && safeToStandUp) StopSlide();
            else if (playerInput.Movement.Slide.ReadValue<float>() != 1 && pm.sliding && !safeToStandUp) crouch.StartCrouch();
        }
    }

    private void FixedUpdate()
    {
        if (pm.sliding) SlidingMovement();
    }

    private void StartSlide()
    {
        if (pm.grounded)
            rb.AddForce(Vector3.down * 200f, ForceMode.Force);
        else
            rb.velocity = new Vector3(rb.velocity.x, rb.velocity.y / 1.25f, rb.velocity.z);
        pm.sliding = true;
        playerObj.localScale = new Vector3(playerObj.localScale.x, slideYScale / 2, playerObj.localScale.z);
        playerCollider.height = slideYScale;
        slideTime = slideDuration;
    }

    private void SlidingMovement()
    {
        Vector3 inputDirection = orientation.forward * movementInput.y + orientation.right * movementInput.x / 100;

        //Normal Slide
        if (!pm.OnSlope() || rb.velocity.y > -0.1f)
        {
            rb.AddForce(inputDirection.normalized * slideForce, ForceMode.Force);

            if (pm.grounded)
                slideTime -= Time.deltaTime;

            if (slideTime <= 0)
            {
                if (!safeToStandUp)
                {
                    StopSlide();

                    crouch.StartCrouch();
                }
                else StopSlide();
            }
        }
        //Sliding on a Slope
        else
        {
            rb.AddForce(pm.GetSlopeDirection(inputDirection) * slideForce, ForceMode.Force);
        }
    }

    private void StopSlide()
    {
        pm.sliding = false;
        playerObj.localScale = new Vector3(playerObj.localScale.x, startYScale / 2, playerObj.localScale.z);
        playerCollider.height = startYScale;
    }
}