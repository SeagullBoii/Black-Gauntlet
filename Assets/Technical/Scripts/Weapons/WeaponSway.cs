using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net;
using UnityEngine;

public class WeaponSway : MonoBehaviour
{
    public Transform weaponTransform;

    [Header("Sway Properties")]
    public float swayAmount;
    public float maxSwayAmount;
    public float swaySmooth;
    public AnimationCurve swayCurve;

    [Range(0f, 1f)]
    public float swaySmoothCounteraction;

    [Header("Rotation")]
    public float rotationSwayMultiplier;

    [Header("Position")]
    public float positionSwayMultiplier;

    [Header("References")]
    public Rigidbody rb;
    public PlayerMovement pm;
    public WeaponAbilities weaponAbilities;

    Vector3 initialPosition;
    Vector3 firstTargetPosition;
    Vector3 secondTargetPosition;

    Quaternion initialRotation;

    Vector2 sway;

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
    private void Reset()
    {
        Keyframe[] ks = new Keyframe[] { new Keyframe(0, 0, 0, 2), new Keyframe(1, 1) };
        swayCurve = new AnimationCurve(ks);
    }

    private void Start()
    {
        if (!weaponTransform)
            weaponTransform = transform;
        initialPosition = weaponTransform.localPosition;
        initialRotation = weaponTransform.localRotation;
    }

    private void Update()
    {
        CameraSway();
        MovementSway();

        Vector3 targetPosition = firstTargetPosition + secondTargetPosition;
        weaponTransform.localPosition = Vector3.Lerp(weaponTransform.localPosition, initialPosition + targetPosition, swayCurve.Evaluate(Time.deltaTime * swaySmooth));
    }

    private void CameraSway()
    {
        Vector2 mousePos = playerInput.Movement.Look.ReadValue<Vector2>() * Time.deltaTime / 20;

        float aimingMultiplier = weaponAbilities != null && weaponAbilities.aiming ? 0.025f : 1;

        sway = Vector2.MoveTowards(sway, Vector2.zero, swayCurve.Evaluate(Time.deltaTime * swaySmoothCounteraction * sway.magnitude * swaySmooth));
        sway = Vector2.ClampMagnitude(mousePos + sway, maxSwayAmount);

        Quaternion targetRotation = Quaternion.Euler(Mathf.Rad2Deg * rotationSwayMultiplier * aimingMultiplier * new Vector3(-sway.y, sway.x, 0));
        weaponTransform.localRotation = Quaternion.Slerp(transform.localRotation, initialRotation * targetRotation, swayCurve.Evaluate(Time.deltaTime * swaySmooth));
        firstTargetPosition = new Vector3(sway.x, sway.y, 0) * positionSwayMultiplier * aimingMultiplier;
    }

    private void MovementSway()
    {
        if (!pm.OnSlope())
        {
            Vector3 aimingMultiplier = weaponAbilities != null && weaponAbilities.aiming ? new Vector3(0, 0.015f, 0.01f) : Vector3.one;

            secondTargetPosition = Vector3.Scale(new Vector3(0, -rb.velocity.y / 500, 0), aimingMultiplier);
            secondTargetPosition.y = Mathf.Clamp(secondTargetPosition.y, -0.125f, 0.125f);
        }
        else
        {
            secondTargetPosition = Vector3.Lerp(secondTargetPosition, Vector3.zero, 5 * Time.deltaTime);
        }
    }
}
