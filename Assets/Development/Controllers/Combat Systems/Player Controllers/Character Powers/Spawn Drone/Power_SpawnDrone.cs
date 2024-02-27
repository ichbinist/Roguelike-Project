using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Power_SpawnDrone : BasePower
{
    public PlayerDroneController PlayerDroneController;

    [FoldoutGroup("Drone Settings")]
    public float Health;
    [FoldoutGroup("Drone Settings")]
    public float BulletDamage;
    [FoldoutGroup("Drone Settings")]
    public float BulletAttackSpeed;
    [FoldoutGroup("Drone Settings")]
    public float TurretActiveTime = 30f;
    [FoldoutGroup("Drone Settings")]
    public float DroneMovementSpeed = 5f;
    [FoldoutGroup("Drone Settings")]
    public bool IsDamageable;

    public override void UsePower(Vector3 position)
    {
        base.UsePower(position);

        PlayerDroneController spawnedTurret = Instantiate(PlayerDroneController, position, Quaternion.identity);
        spawnedTurret.Health = Health;
        spawnedTurret.Damage = BulletDamage;
        spawnedTurret.ShootingSpeed = BulletAttackSpeed;
        spawnedTurret.IsTurretDamageable = IsDamageable;
        spawnedTurret.TurretActiveTime = TurretActiveTime;
        spawnedTurret.DroneMovementSpeed = DroneMovementSpeed;
        spawnedTurret.StartSelfDestruction();
    }
}
