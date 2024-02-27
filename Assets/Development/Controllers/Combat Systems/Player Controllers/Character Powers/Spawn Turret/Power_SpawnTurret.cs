using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Power_SpawnTurret : BasePower
{
    public PlayerTurretController PlayerTurretController;
    [FoldoutGroup("Turret Settings")]
    public float Health;
    [FoldoutGroup("Turret Settings")]
    public float BulletDamage;
    [FoldoutGroup("Turret Settings")]
    public float BulletAttackSpeed;
    [FoldoutGroup("Turret Settings")]
    public float TurretActiveTime = 30f;
    [FoldoutGroup("Turret Settings")]
    public bool IsDamageable;
    public override void UsePower(Vector3 position)
    {
        base.UsePower(position);

        PlayerTurretController spawnedTurret = Instantiate(PlayerTurretController, position, Quaternion.identity);
        spawnedTurret.Health = Health;
        spawnedTurret.Damage = BulletDamage;
        spawnedTurret.ShootingSpeed = BulletAttackSpeed;
        spawnedTurret.IsTurretDamageable = IsDamageable;
        spawnedTurret.TurretActiveTime = TurretActiveTime;
        spawnedTurret.StartSelfDestruction();
    }
}
