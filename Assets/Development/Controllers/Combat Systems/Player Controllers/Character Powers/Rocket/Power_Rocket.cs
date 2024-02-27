using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Power_Rocket : BasePower
{
    public float ExplosionDamage, ExplosionRadius;
    public ParticleSystem Rocket;

    public override void UsePower(Vector3 position)
    {
        base.UsePower(position);

        ParticleSystem rocket = Instantiate(Rocket, CombatGameManager.Instance.PlayerCharacter.transform.position + Vector3.up, CombatGameManager.Instance.PlayerCharacter.GetComponent<CharacterShootingController>().GetDirection());
        ParticleSystem explosion = rocket.transform.GetChild(5).GetComponent<ParticleSystem>();
        CharacterExplosionDamage characterExplosionDamage = explosion.GetComponent<CharacterExplosionDamage>();
        characterExplosionDamage.ExplosionDamage = ExplosionDamage;
        characterExplosionDamage.ExplosionRadius = ExplosionRadius;
      
        rocket.Play();
    }
}
