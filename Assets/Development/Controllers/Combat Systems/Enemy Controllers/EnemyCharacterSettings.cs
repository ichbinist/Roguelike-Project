using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCharacterSettings : CharacterSettings
{
    private RoomSettings roomSettings;
    public RoomSettings RoomSettings { get { return (roomSettings == null) ? roomSettings = GetComponentInParent<RoomSettings>(): roomSettings; } }

    [FoldoutGroup("Enemy Settings")]
    [Min(0f)]
    public float RotationSpeed = 6f;

    [FoldoutGroup("Enemy Settings")]
    [Min(0f)]
    public float CollisionDamage = 0.5f;
    [FoldoutGroup("Enemy Settings")]
    [Min(0f)]
    public float DashDistanceToPlayer = 10f;
    [FoldoutGroup("Enemy Settings")]
    [Min(0f)]
    public Vector2 DashCooldown;
    [FoldoutGroup("Statistics", true, 0)]
    [ReadOnly]
    public bool LockMovementRotation = false;
    public override void Start()
    {
        RoomSettings.Enemies.Add(this);
        Despawn();
    }

    public void Despawn()
    {
        Graphics.SetActive(false);
        _Collider.enabled = false;
        StopControl();
    }

    public void Spawn()
    {
        Graphics.SetActive(true);
        _Collider.enabled = true;
        StartControl();
    }
}