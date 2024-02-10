using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(PlayerMovement))]
public class Jump : MonoBehaviour
{
    [Header("Jumping")]
    public float jumpForce;
    public float jumpCooldown;
    public float airMultiplier;

    public float maxDoubleJumps = 1;

    [Header("Input")]
    public KeyCode jumpKey = KeyCode.Space;

    [Header("References")]
    public Transform cameraTransform;
    public Transform weaponTransform;

    bool readyToJump;
    [HideInInspector]
    public float doubleJumps;
    float airTime;


    //References
    Rigidbody rb;
    PlayerMovement pm;

    //Camera 
    Vector3 currentRotation;
    Vector3 targetRotation;
    Vector3 currentWeaponRotation;
    Vector3 weaponTargetRotation;

    //Input
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
        readyToJump = true;
    }

    private void Update()
    {
        if (Time.timeScale != 0)
        {
            ResetCameraTilt(cameraTransform, 4);
            GetInput();

            if (pm.grounded) airTime = 0.2f;
            else airTime -= Time.deltaTime;

            //Reset double Jump
            if (pm.grounded) doubleJumps = maxDoubleJumps;
        }
    }

    private void GetInput()
    {

        Vector3 horizontalVel = new Vector3(rb.velocity.x, 0, rb.velocity.z);

        if (playerInput.Movement.Jump.ReadValue<float>() == 1 && readyToJump && pm.grounded)
        {
            StartJump();

            if (pm.bhopBoost < 10 && horizontalVel.magnitude > 0.1f)
                pm.bhopBoost++;
        }
        else if (playerInput.Movement.Jump.triggered && readyToJump && airTime > 0)
        {
            StartJump();

            if (pm.bhopBoost < 10 && horizontalVel.magnitude > 0.1f)
                pm.bhopBoost++;
        }
        else if (playerInput.Movement.Jump.triggered && readyToJump && airTime <= 0 && doubleJumps > 0 && !pm.wallrunning)
        {
            doubleJumps--;
            StartJump();
        }
    }

    private void StartJump()
    {
        //Jump
        rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        pm.exitingSlope = true;



        //ResetJump
        readyToJump = false;
        Invoke("ResetJump", 0.15f);

        //Camera Effect
        targetRotation = new Vector3(10, 0, 0);
        //weaponTargetRotation = new Vector3(123, 0, 0);
    }

    private void ResetJump()
    {
        readyToJump = true;
        pm.exitingSlope = false;
    }

    private void ResetCameraTilt(Transform camTransform, float speed)
    {
        targetRotation = Vector3.Lerp(targetRotation, Vector3.zero, speed * Time.deltaTime);
        currentRotation = Vector3.Slerp(currentRotation, targetRotation, speed * Time.fixedDeltaTime);
        camTransform.localRotation = Quaternion.Euler(currentRotation);
    }

    private void ResetWeaponTilt(Transform weaponTransform, float speed)
    {
        weaponTargetRotation = Vector3.Lerp(targetRotation, Vector3.zero, speed * Time.deltaTime);
        currentWeaponRotation = Vector3.Slerp(currentRotation, targetRotation, speed * Time.fixedDeltaTime);
        weaponTransform.localRotation = Quaternion.Euler(currentRotation);
    }
}
