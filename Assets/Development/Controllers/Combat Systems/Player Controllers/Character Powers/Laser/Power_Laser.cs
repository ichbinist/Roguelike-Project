using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Power_Laser : BasePower
{
    public float LaserDamage, LaserAttackSpeed, LaserDuration;
    public LaserController LaserController;

    public override void UsePower(Vector3 position)
    {
        base.UsePower(position);
        LaserController laserController = Instantiate(LaserController, CombatGameManager.Instance.PlayerCharacter.transform);
        laserController.LaserDamage = LaserDamage;
        laserController.LaserAttackSpeed = LaserAttackSpeed;
        laserController.StartCloak(LaserDuration);
        laserController.CharacterShootingController = CombatGameManager.Instance.PlayerCharacter.GetComponent<CharacterShootingController>();
    }
}
