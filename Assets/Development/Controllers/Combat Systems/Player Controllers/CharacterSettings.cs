using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using DG.Tweening;
using UnityEngine.Events;

public class CharacterSettings : MonoBehaviour
{
    [FoldoutGroup("General")]
    public MapState MapState = MapState.Combat;
    // General Settings
    [FoldoutGroup("General")]
    public CharacterType CharacterType;

    [FoldoutGroup("General")]
    public bool IsControlAllowed = false; // If true, can control the character.

    [FoldoutGroup("General")]
    public bool IsShootingAllowed = true;

    [FoldoutGroup("General")]
    public bool IsDamageable = true; // If true, can take damage.

    [FoldoutGroup("General")]
    public float CurrentHealth = 10f;

    [FoldoutGroup("General")]
    public int CurrentAmmo = 0;

    [FoldoutGroup("General")]
    public bool IsAmmoReloading = false;

    // Movement Settings
    [FoldoutGroup("Movement")]
    [Min(0f)] public float MovementSpeed;

    [FoldoutGroup("Movement")]
    [Min(0f)] public float DashRange;

    [FoldoutGroup("Movement")]
    [Min(0f)] public float DashSpeed;

    [FoldoutGroup("Movement")]
    public Ease DashEase = Ease.Linear;

    [FoldoutGroup("Movement")]
    [ShowIf("isPlayer")] public float PlayerDashCooldown = 4f;

    [FoldoutGroup("Movement")]
    [ShowIf("isPlayer")] public List<AudioClip> FootstepSounds = new List<AudioClip>();

    [FoldoutGroup("Movement")]
    [ShowIf("isPlayer")] public float FootstepDelayAmount = 0.075f;

    // Weapon Settings
    [FoldoutGroup("Weapon")]
    public float ShootingHeight = 1f;

    [HideInInspector] public UnityEvent<string> OnWeaponSwitched = new UnityEvent<string>();

    [FoldoutGroup("Weapon")]
    public float TemporaryDamage;

    // Damage Taken Settings
    [FoldoutGroup("Damage Taken")]
    [ShowIf("isPlayer")] public float DamageTakenDelay = 0.1f;

    [FoldoutGroup("Damage Taken")]
    [ShowIf("isPlayer")] public float DamageTakenPushbackForce = 40f;

    // Audio Settings
    [FoldoutGroup("Audio")]
    [ShowIf("isPlayer")] public AudioClip DashSound;

    [FoldoutGroup("Audio")]
    [ShowIf("isPlayer")] public AudioClip ReloadSound;

    [FoldoutGroup("Audio")]
    [ShowIf("isPlayer")] public AudioClip HitSound;

    // Death Settings (for Enemies)
    [FoldoutGroup("Death - Enemy")]
    [ShowIf("isEnemy")] public float ExecuteStateDuration = 10f;

    [FoldoutGroup("Death - Enemy")]
    [ShowIf("isEnemy")] public bool IsExecuteState = false;

    [FoldoutGroup("Death - Enemy")]
    [ShowIf("isEnemy")] public float ExecuteActionDuration = 1f;

    [FoldoutGroup("Death - Enemy")]
    [ShowIf("isEnemy")] public float ExecuteStateProbability = 0.1f;

    [FoldoutGroup("Death - Enemy")]
    [ShowIf("isEnemy")] public float ExecuteHealthRecoveryRate = 4f;

    [FoldoutGroup("Death - Enemy")]
    [ShowIf("isEnemy")] public float ExecuteRange = 2.5f;

    [FoldoutGroup("Death - Enemy")]
    [ShowIf("isEnemy")] public bool IsExecuteable = true;

    // Attributes
    [FoldoutGroup("Attributes")]
    public float DamageMultiplier = 1f;

    [FoldoutGroup("Attributes")]
    public float AttackRateMultiplier = 1f;

    [FoldoutGroup("Attributes")]
    public float MaximumHealth = 10f;

    // Events
    [HideInInspector] public UnityEvent<bool> OnControlStateChanged = new UnityEvent<bool>();

    [HideInInspector] public Action OnBulletFired;

    // Private Properties
    private GameObject graphics;
    public GameObject Graphics { get { return (graphics == null) ? graphics = transform.GetChild(0).gameObject : graphics; } }

    private Collider _collider;
    public Collider _Collider { get { return (_collider == null) ? _collider = GetComponent<Collider>() : _collider; } }

    public bool isPlayer { get { return (CharacterType == CharacterType.Player); } }
    public bool isEnemy { get { return (CharacterType == CharacterType.Enemy); } }

    public virtual void Start()
    {
        StartControl();
        Application.targetFrameRate = 60;
    }

    public void StartControl()
    {
        IsControlAllowed = true;
        OnControlStateChanged.Invoke(IsControlAllowed);
    }

    public void StopControl()
    {
        IsControlAllowed = false;
        OnControlStateChanged.Invoke(IsControlAllowed);
    }

    public void SetTemporaryDamage(float damage)
    {
        TemporaryDamage = damage;
    }
}

public enum CharacterType
{
    Player,
    Enemy,
    Neutral
}

public enum MapState
{
    Base,
    Combat,
    Overview
}