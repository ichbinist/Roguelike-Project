using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioPlayerOnStart : MonoBehaviour
{
    public AudioClip AudioClip;
    public float Volume = 0.2f;
    private void Start()
    {
        AudioPoolManager.Instance.PlaySound(AudioClip, Volume);
    }
}
