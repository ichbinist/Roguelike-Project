using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioPlayerOnParticle : BulletFiredController
{
    public AudioClip AudioClip;
    public float Volume = 0.3f;

    public override void OnParticleEmitted()
    {
        AudioPoolManager.Instance.PlaySound(AudioClip, Volume);
    }
}

public abstract class BulletFiredController : MonoBehaviour
{
    private CharacterSettings characterSettings;
    public CharacterSettings CharacterSettings { get { return (characterSettings == null) ? characterSettings = GetComponentInParent<CharacterSettings>() : characterSettings; } }

    private void OnEnable()
    {
        if (CharacterSettings != null)
            CharacterSettings.OnBulletFired += OnParticleEmitted;
        else if (GetComponentInParent<PlayerTurretController>() != null)
            GetComponentInParent<PlayerTurretController>().OnBulletFired += OnParticleEmitted;
    }

    private void OnDisable()
    {
        if (CharacterSettings)
            CharacterSettings.OnBulletFired -= OnParticleEmitted;
        else if(GetComponentInParent<PlayerTurretController>() != null)
            GetComponentInParent<PlayerTurretController>().OnBulletFired -= OnParticleEmitted;
    }

    public abstract void OnParticleEmitted();
}