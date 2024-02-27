using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Power_Landmine : BasePower
{
    public float ExplosionDamage, ExplosionRadius, ExplosionDelay;
    public LandmineController LandmineController;

    public override void UsePower(Vector3 position)
    {
        base.UsePower(position);
        LandmineController landmine = Instantiate(LandmineController, CombatGameManager.Instance.PlayerCharacter.transform.position, CombatGameManager.Instance.PlayerCharacter.GetComponent<CharacterShootingController>().GetDirection());
        landmine.ExplosionRadius = ExplosionRadius;
        landmine.ExplosionDamage = ExplosionDamage;
        landmine.ExplosionDelay = ExplosionDelay;
        landmine.IsAllowedToExplode = true;
        landmine.GetComponent<SphereCollider>().radius = ExplosionRadius;
    }
}
