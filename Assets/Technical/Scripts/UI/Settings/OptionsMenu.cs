using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class OptionsMenu : MonoBehaviour
{
    [Header("Audio")]
    public AudioMixer masterAudio;
    public AudioMixer musicAudio;
    public Slider masterAudioSlider;
    public Slider musicAudioSlider;

    [Header("Graphics")]
    public Toggle bloomToggle;
    public Toggle vignetteToggle;
    public TMP_Dropdown qualityDropdown;
    public Slider chromaticAbberationSlider;
    public Slider motionBlurSlider;

    [Header("References")]
    public RenderPipelineAsset[] renderPipelineAsset;

    private void Start()
    {
        //Audio
        if (masterAudioSlider != null)
            masterAudioSlider.value = PlayerPrefs.GetFloat("MasterVolume");
        if (musicAudioSlider != null)
            musicAudioSlider.value = PlayerPrefs.GetFloat("MusicVolume");

        //Graphics
        if (chromaticAbberationSlider != null)
            chromaticAbberationSlider.value = PlayerPrefs.GetFloat("ChromaticAberration");
        if (motionBlurSlider != null)
            motionBlurSlider.value = PlayerPrefs.GetFloat("MotionBlur");
        if (qualityDropdown != null)
            qualityDropdown.value = PlayerPrefs.GetInt("GraphicsQuality");

        if (bloomToggle != null)
            bloomToggle.isOn = PlayerPrefs.GetInt("Bloom") != 1;
        if (vignetteToggle != null)
            vignetteToggle.isOn = PlayerPrefs.GetInt("Vignette") != 1;
    }




    public void SetMasterVolume(float volume)
    {
        PlayerPrefs.SetFloat("MasterVolume", volume);

        masterAudio.SetFloat("volume", volume);
    }

    public void SetMusicVolume(float volume)
    {
        PlayerPrefs.SetFloat("MusicVolume", volume);

        musicAudio.SetFloat("volume", volume);
    }



    public void SetGraphicsQuality(int index)
    {
        PlayerPrefs.SetInt("GraphicsQuality", index);
        QualitySettings.SetQualityLevel(index);
        QualitySettings.renderPipeline = renderPipelineAsset[index];
    }

    public void SetBloom(bool bloom)
    {
        PlayerPrefs.SetInt("Bloom", bloom ? 0 : 1);
    }

    public void SetVignette(bool vignette)
    {
        PlayerPrefs.SetInt("Vignette", vignette ? 0 : 1);
    }

    public void SetMotionBlur(float angle)
    {
        PlayerPrefs.SetFloat("MotionBlur", angle);
    }
    public void SetChromaticAberration(float intensity)
    {
        PlayerPrefs.SetFloat("ChromaticAberration", intensity);
    }
}
