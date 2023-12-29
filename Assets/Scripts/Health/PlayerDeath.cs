using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Health))]
public class PlayerDeath : MonoBehaviour
{
    public GameObject deathScreen;
    public GameObject crosshair;

    Health health;

    private void Start()
    {
        health = GetComponent<Health>();
    }

    private void Update()
    {
        if (health.health <= 0)
        {
            health.isDead = true;
            if (deathScreen != null)
                deathScreen.SetActive(true);
            if (crosshair != null)
                crosshair.SetActive(false);
            Time.timeScale = 0;

            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

    }

    public void ResetScene()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void MainMenu()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(0);
    }
}
