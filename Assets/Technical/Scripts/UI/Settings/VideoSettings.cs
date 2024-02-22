using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class VideoSettings : MonoBehaviour
{
    public Toggle fullscreenToggle;
    public Toggle vSyncToggle;
    public Slider FOVSlider;
    public Slider FPSSlider;
    public TMP_Dropdown resolutionDropdown;
    public bool mainCam;

    Resolution[] resolutions;
    List<Resolution> filteredResolutions;

    float currentRefreshRate;
    int currentResolutionIndex = 0;

    private void Start()
    {
        if (PlayerPrefs.GetFloat("FOV") == 0) PlayerPrefs.SetFloat("FOV", 80);
        if (FOVSlider != null)
            FOVSlider.value = PlayerPrefs.GetFloat("FOV");

        if (PlayerPrefs.GetFloat("FPS") == 0) PlayerPrefs.SetFloat("FPS", FPSSlider.maxValue);
        FPSSlider.value = PlayerPrefs.GetFloat("FPS");

        vSyncToggle.isOn = PlayerPrefs.GetFloat("VSync") != 0;
        InitializeResolutions();
    }

    private void FixedUpdate()
    {
        if (fullscreenToggle != null)
            if (fullscreenToggle.isOn != Screen.fullScreen)
                fullscreenToggle.isOn = Screen.fullScreen;
    }

    private void InitializeResolutions()
    {
        //Form the List
        resolutions = Screen.resolutions;
        filteredResolutions = new List<Resolution>();

        resolutionDropdown.ClearOptions();
        currentRefreshRate = (float)Screen.currentResolution.refreshRateRatio.value;

        for (int i = 0; i < resolutions.Length; i++)
        {
            if ((float)resolutions[i].refreshRateRatio.value == currentRefreshRate)
            {
                filteredResolutions.Add(resolutions[i]);
            }
        }

        filteredResolutions.Reverse();

        //Add the Options
        List<string> options = new List<string>();
        for (int i = 0; i < filteredResolutions.Count; i++)
        {
            string resolutionOption = filteredResolutions[i].width + "x" + filteredResolutions[i].height /*+ ", " + filteredResolutions[i].refreshRateRatio.value + " Hz"*/ + "p";
            options.Add(resolutionOption);
            if (filteredResolutions[i].width == Screen.width && filteredResolutions[i].height == Screen.height)
            {
                currentResolutionIndex = i;
            }
        }

        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = currentResolutionIndex;
        resolutionDropdown.RefreshShownValue();
    }

    public void SetFOV(float fov)
    {
        PlayerPrefs.SetFloat("FOV", fov);
        if (mainCam) Camera.main.fieldOfView = fov;
        PlayerCameraMovement.InvokeChangedFov();
    }

    public void SetResolution(int index)
    {
        Resolution resolution = filteredResolutions[index];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
    }

    public void SetFullscreen(bool fullScreen)
    {
        Screen.fullScreen = fullScreen;
    }

    public void SetFPS(float targetFps)
    {
        float fps = targetFps > 30 ? -1 : targetFps * 10;
        PlayerPrefs.SetFloat("FPS", targetFps);
        Application.targetFrameRate = (int)fps;
    }

    public void SetVSync(bool vSync)
    {
        PlayerPrefs.SetFloat("VSync", vSync ? 1 : 0);
        QualitySettings.vSyncCount = vSync ? 1 : 0;
        print(QualitySettings.vSyncCount);
    }
}
