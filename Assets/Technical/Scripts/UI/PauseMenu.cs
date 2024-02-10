using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] GameObject crosshair;
    public KeyCode pauseKey = KeyCode.Escape;
    public Canvas pauseCanvas;

    bool paused = false;

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
        paused = false;
        ManagePause();

        pauseCanvas.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (paused && Time.time == 0) paused = false;
        if (playerInput.UI.Cancel.triggered)
        {
            paused = !paused;
            ManagePause();
        }
    }

    private void ManagePause()
    {
        Time.timeScale = paused ? 0 : 1;
        Cursor.lockState = paused ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = paused;
        crosshair.SetActive(!paused);
        pauseCanvas.gameObject.SetActive(paused);
    }

}
