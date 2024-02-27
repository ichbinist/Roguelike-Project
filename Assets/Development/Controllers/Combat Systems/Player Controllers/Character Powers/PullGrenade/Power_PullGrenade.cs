using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Power_PullGrenade : BasePower
{
    public ParticleSystem PullGrenadeParticle;
    public override void UsePower(Vector3 position)
    {
        base.UsePower(position);

        ParticleSystem pullGrenade = Instantiate(PullGrenadeParticle, position + Vector3.up, Quaternion.identity);
        pullGrenade.transform.rotation = CombatGameManager.Instance.PlayerCharacter.GetComponent<CharacterShootingController>().GetDirection();
        pullGrenade.Play();
    }
}
