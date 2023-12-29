using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Jump))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    public float walkSpeed;
    public float crouchSpeed;
    public float slideSpeed;
    public float dashSpeed;
    public float wallrunSpeed;

    public bool crouching;
    public bool sliding;
    public bool dashing;
    public bool wallrunning;

    public enum MovementStates
    {
        wallrunning, dashing, sliding, crouching, walking, air
    }

    public float maxSlopeAngle;
    public float groundDrag;

    [Header("Input")]
    PlayerInput playerInput;

    [Header("Ground Check")]
    public float playerHeight;
    public LayerMask ground;

    [Header("References")]
    public Transform orientation;
    public Transform guns;

    //WASD or Arrow Key Inputs
    Vector2 movementInput;

    //Movement States
    [HideInInspector]
    public float movementSpeed;
    [HideInInspector]
    public float desiredMovementSpeed;
    float lastDesiredMovementSpeed;

    public bool grounded { get; private set; }
    public MovementStates state { get; private set; }


    //Slope
    RaycastHit slopeHit;
    [HideInInspector]
    public bool exitingSlope;

    //Other
    [HideInInspector]
    public float bhopBoost;
    float groundedTime;
    Vector3 gunStartPos;
    Vector3 movementDirection;

    Rigidbody rb;
    Jump jumpScript;

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
        rb.freezeRotation = true;

        jumpScript = GetComponent<Jump>();

        gunStartPos = transform.localPosition;
    }

    private void Update()
    {
        grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f, ground);

        MyInput();
        SpeedControl();
        HandleMovementStates();
        WeaponCamTilt();


        //Reset Bhop Boost

        if (grounded && groundedTime > 0)
            groundedTime -= Time.deltaTime;
        else if (!grounded)
            groundedTime = 0.5f;

        if (groundedTime <= 0) bhopBoost = 0;

        //Add Drag and Limit Speed
        if (grounded)
        {
            rb.drag = groundDrag;
        }
        else
        {
            rb.drag = 0;
        }

        rb.useGravity = !OnSlope();
    }

    private void FixedUpdate()
    {
        MovePlayer();
    }

    private void MyInput()
    {
        movementInput = playerInput.Movement.Move.ReadValue<Vector2>();
    }

    private void HandleMovementStates()
    {

        float deaccelerationRate = 0;

        if (dashing)
        {
            if (GetComponent<Dashing>())
                desiredMovementSpeed = GetComponent<Dashing>().delayedForceToApply.magnitude + bhopBoost;
            else
                desiredMovementSpeed = dashSpeed + bhopBoost;

            deaccelerationRate = 8;
            state = MovementStates.dashing;
        }
        else if (wallrunning)
        {
            desiredMovementSpeed = wallrunSpeed + bhopBoost;
            deaccelerationRate = 6;
            state = MovementStates.wallrunning;
        }
        else if (sliding)
        {
            if (OnSlope() && rb.velocity.y < 0.1f)
                desiredMovementSpeed = slideSpeed + bhopBoost;
            else
                desiredMovementSpeed = walkSpeed + 5 + bhopBoost;
            deaccelerationRate = 8;
            state = MovementStates.sliding;
        }
        else if (crouching)
        {
            if (OnSlope() && rb.velocity.y < 0.1f)
                desiredMovementSpeed = slideSpeed + bhopBoost;
            else if (!grounded)
                desiredMovementSpeed = walkSpeed + bhopBoost;
            else
                desiredMovementSpeed = crouchSpeed + bhopBoost;

            deaccelerationRate = 8;
            state = MovementStates.crouching;
        }
        else if (grounded)
        {
            desiredMovementSpeed = walkSpeed + bhopBoost;
            deaccelerationRate = 8;
            state = MovementStates.walking;
        }
        else
        {
            state = MovementStates.air;
        }


        if (Mathf.Abs(desiredMovementSpeed - lastDesiredMovementSpeed) >= 5 && movementSpeed != 0 && !dashing)
        {
            StopAllCoroutines();
            StartCoroutine(LerpSpeed(deaccelerationRate));
        }
        else
        {
            movementSpeed = desiredMovementSpeed;
        }

        if (movementInput.magnitude == 0 && rb.velocity.magnitude < 0.5f && movementSpeed > desiredMovementSpeed) StopAllCoroutines();

        lastDesiredMovementSpeed = desiredMovementSpeed;
    }

    private IEnumerator LerpSpeed(float multiplier)
    {
        float time = 0;
        float difference = Mathf.Abs(desiredMovementSpeed - movementSpeed);
        float startValue = movementSpeed;

        while (time < difference)
        {
            movementSpeed = Mathf.Lerp(startValue, desiredMovementSpeed, time / difference * multiplier);
            time += Time.deltaTime;
            yield return null;
        }

        movementSpeed = desiredMovementSpeed;
    }

    private void MovePlayer()
    {
        //Calculate the Movement Direction
        movementDirection = orientation.forward * movementInput.y + orientation.right * movementInput.x;

        //On Slope
        if (OnSlope() && !exitingSlope)
        {
            rb.AddForce(GetSlopeDirection(movementDirection) * movementSpeed * 20f, ForceMode.Force);

            if (rb.velocity.y > 0) rb.AddForce(Vector3.down * 80f, ForceMode.Force);

        }
        else
        {

            //Drag
            if (grounded && !dashing)
                rb.AddForce(movementDirection * movementSpeed * 10, ForceMode.Force);
            else
                rb.AddForce(movementDirection.normalized * movementSpeed * 10 * jumpScript.airMultiplier, ForceMode.Force);
        }

    }

    private void SpeedControl()
    {
        if (OnSlope() && !exitingSlope)
        {
            if (rb.velocity.magnitude > movementSpeed) rb.velocity = rb.velocity.normalized * movementSpeed;
        }
        else
        {
            Vector3 flatVelocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);

            if (flatVelocity.magnitude > movementSpeed)
            {
                Vector3 limitedVelocity = flatVelocity.normalized * movementSpeed;
                rb.velocity = new Vector3(limitedVelocity.x, rb.velocity.y, limitedVelocity.z);
            }

            if (movementSpeed > 125) movementSpeed = 125;
        }
    }

    private void WeaponCamTilt()
    {
        gameObject.TryGetComponent<WeaponAbilities>(out WeaponAbilities weaponAbilities);
        WallRunning wallRunCode = gameObject.TryGetComponent<WallRunning>(out WallRunning wallRun) ? wallRun : null;

        Vector3 tilt = new Vector3(0, 0, 20);

        if ((state == MovementStates.sliding) && !weaponAbilities.aiming)
        {
            guns.DOLocalRotate(tilt, 0.15f);
            //   guns.localPosition = Vector3.Lerp(guns.localPosition, new Vector3(0.2f, 0.15f, 0) + gunStartPos, 10f * Time.deltaTime);
        }
        else if (state == MovementStates.wallrunning && !weaponAbilities.aiming)
        {
            if (wallRunCode.wallLeft)
            {
                guns.DOLocalRotate(tilt, 0.15f);
                //  guns.localPosition = Vector3.Lerp(guns.localPosition, new Vector3(0.1f, 0.2f, 0) + gunStartPos, 10f * Time.deltaTime);
            }
            if (wallRunCode.wallRight)
            {
                guns.DOLocalRotate(tilt, 0.15f);
                //  guns.localPosition = Vector3.Lerp(guns.localPosition, new Vector3(0.1f, 0.15f, 0) + gunStartPos, 10f * Time.deltaTime);
            }
        }
        else if (true)
        {
            guns.DOLocalRotate(Vector3.zero, 0.15f);
            //    guns.localPosition = Vector3.Lerp(guns.localPosition, gunStartPos, 10f * Time.deltaTime);
        }
    }

    public bool OnSlope()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out slopeHit, playerHeight * 0.5f + 0.3f))
        {
            float angle = Vector3.Angle(Vector3.up, slopeHit.normal);
            return angle < maxSlopeAngle && angle != 0;
        }

        return false;
    }

    public Vector3 GetSlopeDirection(Vector3 direction)
    {
        return Vector3.ProjectOnPlane(direction, slopeHit.normal).normalized;
    }
}
