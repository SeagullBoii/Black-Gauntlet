using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(PlayerMovement))]
[RequireComponent(typeof(Jump))]
public class WallRunning : MonoBehaviour
{
    [Header("Wallrunning")]
    public float wallRunForce;
    public float wallCheckDistance;

    public float wallJumpUpForce;
    public float wallJumpForce;

    public bool exitingWall;
    public float exitingWallTime;

    public LayerMask ground;
    public LayerMask canWallrunOn;

    [Header("References")]
    public Transform orientation;
    public PlayerCameraMovement cam;
    public WeaponCameraTiltAndFOV weaponCam;

    //Wallrunning
    [HideInInspector]
    public bool wallLeft;
    [HideInInspector]
    public bool wallRight;

    float exitingWallTimer;

    RaycastHit leftWallHit;
    RaycastHit rightWallHit;

    //References
    Rigidbody rb;
    PlayerMovement pm;
    Jump jump;
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
        jump = GetComponent<Jump>();
    }

    Vector2 movementInput;
    private void Update()
    {
        movementInput = playerInput.Movement.Move.ReadValue<Vector2>();
        CheckForWall();
        WallRunStates();
    }

    private void FixedUpdate()
    {
        if (pm.wallrunning) WallRunningMovement();
    }

    private void CheckForWall()
    {
        wallRight = Physics.Raycast(transform.position, orientation.right, out rightWallHit, wallCheckDistance, canWallrunOn);
        wallLeft = Physics.Raycast(transform.position, -orientation.right, out leftWallHit, wallCheckDistance, canWallrunOn);
    }

    private void WallRunStates()
    {


        //Wallrun
        if ((wallLeft || wallRight) && movementInput.y != 0 && !pm.grounded && !exitingWall)
        {
            if (!pm.wallrunning) StartWallRun();

            if (playerInput.Movement.Jump.triggered)
                WallJump();
        }

        //Wall Jump

        else if (exitingWall)
        {
            if (pm.wallrunning) StopWallRun();

            if (exitingWallTimer > 0)
                exitingWallTimer -= Time.deltaTime;
            else
                exitingWall = false;

        }

        //Stop Wallrunning
        else
        {
            if (pm.wallrunning) StopWallRun();
        }
    }

    private void StartWallRun()
    {
        pm.wallrunning = true;
        jump.doubleJumps = jump.maxDoubleJumps;
    }

    private void WallRunningMovement()
    {
        rb.useGravity = false;
        rb.velocity = new Vector3(rb.velocity.x, -0.4f, rb.velocity.z);

        wallRunForce = movementInput.y > 0 ? Mathf.Abs(wallRunForce) : -Mathf.Abs(wallRunForce);

        Vector3 wallNormal = wallRight ? rightWallHit.normal : leftWallHit.normal;
        Vector3 wallForward = Vector3.Cross(wallNormal, transform.up);

        rb.AddForce(wallForward * wallRunForce, ForceMode.Force);
        if (!(wallLeft && movementInput.x > 0) && !(wallRight && movementInput.x < 0)) rb.AddForce(-wallNormal * 100, ForceMode.Force);


        cam.DoTilt(wallRight ? 10 : -10);
    }

    private void WallJump()
    {
        Vector3 wallNormal = wallRight ? rightWallHit.normal : leftWallHit.normal;
        Vector3 forceToApply = transform.up * wallJumpUpForce + wallNormal * wallJumpForce;

        exitingWall = true;
        exitingWallTimer = exitingWallTime;

        rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
        rb.AddForce(forceToApply, ForceMode.Impulse);
    }

    private void StopWallRun()
    {
        pm.wallrunning = false;
        cam.DoTilt(0);
    }


}
