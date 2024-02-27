using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenShakeOnParticle : BulletFiredController
{
    public Cinemachine.CinemachineImpulseSource CinemachineImpulseSource;
    private CharacterShootingController CharacterShootingController;
    public float ShakeVelocity = 0.015f;
    private void Start()
    {
        CharacterShootingController = GetComponentInParent<CharacterShootingController>();
    }

    public override void OnParticleEmitted()
    {
        //Screen Shake
        CinemachineImpulseSource.GenerateImpulseWithVelocity(CharacterShootingController.LookDirection * ShakeVelocity);
    }
}
