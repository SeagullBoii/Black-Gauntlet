using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayAudio : MonoBehaviour
{
    public static void PlaySound(AudioSource audioSource, float startingPitch, float pitchRandomization)
    {
        audioSource.pitch = startingPitch + Random.Range(-pitchRandomization, pitchRandomization);
        audioSource.PlayOneShot(audioSource.clip);
    }
}
