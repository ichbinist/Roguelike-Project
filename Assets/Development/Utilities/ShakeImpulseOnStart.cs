using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShakeImpulseOnStart : MonoBehaviour
{
    public Cinemachine.CinemachineImpulseSource CinemachineImpulseSource;

    private void Start()
    {
        OnParticleEmitted();
    }

    public void OnParticleEmitted()
    {
        //Screen Shake
        CinemachineImpulseSource.GenerateImpulseWithVelocity(Vector3.up * 1.25f);
    }
}