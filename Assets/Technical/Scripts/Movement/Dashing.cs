using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(PlayerMovement))]
public class Dashing : MonoBehaviour
{
    [Header("Dashing")]
    public float dashForce;
    public float dashDuration;
    public float dashCd;

    [Header("Settings")]
    public bool dashForward;
    public bool allowAllDirections;

    [Header("References")]
    public Transform orientation;
    public GameObject cam;

    //Dashing
    public float dashCdTimer { get; private set; }
    public Vector3 delayedForceToApply { get; private set; }
    float baseFOV;

    //References
    Rigidbody rb;
    PlayerMovement pm;
    Weapons weaponScript;
    WeaponAbilities weaponAbilities;

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
        if (GetComponent<Weapons>())
            weaponScript = GetComponent<Weapons>();
        if (GetComponent<WeaponAbilities>())
            weaponAbilities = GetComponent<WeaponAbilities>();
    }

    private void Update()
    {
        if (Time.timeScale != 0)
        {
            if (playerInput.Movement.Dash.triggered && dashCdTimer <= 0)
            {
                Dash();
            }

            if (dashCdTimer > 0) dashCdTimer -= Time.deltaTime;
        }
    }

    int fovPosInArrL = 0;
    private void Dash()
    {
        baseFOV = PlayerPrefs.GetFloat("FOV");

        if (weaponAbilities != null && !weaponAbilities.aiming)
        {
            cam.GetComponent<PlayerCameraMovement>().fovAdditives.Add(15f);
            fovPosInArrL = cam.GetComponent<PlayerCameraMovement>().fovAdditives.Count - 1;
            cam.GetComponent<PlayerCameraMovement>().AddToFOV();
        }
        Transform forward;

        if (dashForward) forward = cam.transform;
        else forward = orientation;

        Vector3 forceToApply = GetDirection(forward) * (pm.movementSpeed + dashForce - pm.walkSpeed);
        delayedForceToApply = forceToApply;

        pm.dashing = true;
        dashCdTimer = dashCd;


        if (gameObject.TryGetComponent<Jump>(out Jump jump)) jump.doubleJumps = jump.maxDoubleJumps;
        if (gameObject.TryGetComponent<WallRunning>(out WallRunning wallRun)) wallRun.exitingWall = true;

        Invoke(nameof(DelayedDashForce), 0.025f);
        Invoke(nameof(ResetDash), dashDuration);
    }

    private void DelayedDashForce()
    {
        rb.AddForce(delayedForceToApply, ForceMode.Impulse);
    }

    private void ResetDash()
    {
        if (dashForward) pm.desiredMovementSpeed = pm.walkSpeed;

        if (!weaponAbilities.aiming)
        {
            cam.GetComponent<PlayerCameraMovement>().fovAdditives.RemoveAt(fovPosInArrL);
            cam.GetComponent<PlayerCameraMovement>().AddToFOV();
            fovPosInArrL = -1;
        }
        pm.dashing = false;
    }

    private Vector3 GetDirection(Transform forward)
    {
        Vector2 movementInput = playerInput.Movement.Move.ReadValue<Vector2>();

        Vector3 direction = new Vector3();

        if (allowAllDirections)
            direction = forward.forward * movementInput.y + forward.right * movementInput.x;
        else direction = forward.forward;

        if (movementInput.magnitude == 0) direction = forward.forward;

        return direction.normalized;
    }

}
