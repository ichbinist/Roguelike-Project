using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmbienceMusicController : MonoBehaviour
{
    private AudioSource audioSource;
    public AudioSource AudioSource { get { return (audioSource == null) ? audioSource = GetComponent<AudioSource>(): audioSource; } }

    private void Update()
    {
        AudioSource.pitch = Time.timeScale;
    }
}
