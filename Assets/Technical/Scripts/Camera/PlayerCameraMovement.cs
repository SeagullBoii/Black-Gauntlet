using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class PlayerCameraMovement : MonoBehaviour
{
    public float sensX;
    public float sensY;

    public Transform orientation;
    public Transform camHolder;

    public Health health;

    [HideInInspector] public ArrayList fovAdditives = new ArrayList();

    float xRotation;
    float yRotation;

    Vector3 startPos;
    Vector3 shake;

    Camera cam;
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
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        cam = GetComponent<Camera>();
        cam.fieldOfView = PlayerPrefs.GetFloat("FOV");
        startPos = transform.localPosition;
    }

    private void Update()
    {

        if (Time.timeScale != 0)
        {
            if (health.isDead)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }

            else
            {
                // get mouse input
                float mouseX = playerInput.Movement.Look.ReadValue<Vector2>().x * 0.02f * sensX;
                float mouseY = playerInput.Movement.Look.ReadValue<Vector2>().y * 0.02f * sensY;

                yRotation += mouseX;

                xRotation -= mouseY;
                xRotation = Mathf.Clamp(xRotation, -90f, 90f);

                // rotate cam and orientation
                camHolder.rotation = Quaternion.Euler(xRotation, yRotation, 0);
                orientation.rotation = Quaternion.Euler(0, yRotation, 0);
            }
        }
    }

    public void AddToFOV()
    {
        if (fovAdditives.Count == 0)
        {
            GetComponent<Camera>().DOFieldOfView(PlayerPrefs.GetFloat("FOV"), 0.25f);
        }
        else
        {
            float sum = 0;
            foreach (float num in fovAdditives)
                sum += num;
            GetComponent<Camera>().DOFieldOfView(PlayerPrefs.GetFloat("FOV") + sum, 0.25f);
        }
    }

    public void DoFOV(float endValue)
    {
        GetComponent<Camera>().DOFieldOfView(endValue, 0.25f);
    }

    public void DoTilt(float zTilt)
    {
        transform.DOLocalRotate(new Vector3(0, 0, zTilt), 0.25f);
    }
}
