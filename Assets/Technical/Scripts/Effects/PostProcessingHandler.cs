using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class PostProcessingHandler : MonoBehaviour
{
    Volume volume;

    private void Start()
    {
        volume = GetComponent<Volume>();
    }

    private void Update()
    {
        /*volume.profile.GetSetting<Bloom>().enabled.value = PlayerPrefs.GetInt("Bloom") != 1;
        volume.profile.GetSetting<Vignette>().enabled.value = PlayerPrefs.GetInt("Vignette") != 1;
        volume.profile.GetSetting<AmbientOcclusion>().enabled.value = PlayerPrefs.GetInt("AmbientOcclusion") != 1;
        volume.profile.GetSetting<MotionBlur>().shutterAngle.value = PlayerPrefs.GetFloat("MotionBlur");
        volume.profile.GetSetting<ChromaticAberration>().intensity.value = PlayerPrefs.GetFloat("ChromaticAberration");
        */
        if (volume.profile.TryGet<Bloom>(out Bloom bloom)) bloom.active = PlayerPrefs.GetInt("Bloom") != 1;
        if (volume.profile.TryGet<Vignette>(out Vignette vignette)) vignette.active = PlayerPrefs.GetInt("Vignette") != 1;
        if (volume.profile.TryGet<MotionBlur>(out MotionBlur blur)) blur.intensity.value = PlayerPrefs.GetFloat("MotionBlur");
        if (volume.profile.TryGet<ChromaticAberration>(out ChromaticAberration aberration)) aberration.intensity.value = PlayerPrefs.GetFloat("ChromaticAberration");
    }
}
