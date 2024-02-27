using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EnemyCharacterDamageController : MonoBehaviour
{
    private Transform graphics;
    public Transform Graphics { get { return (graphics == null) ? graphics = transform.GetChild(0) : graphics; } }

    private EnemyCharacterSettings enemyCharacterSettings;
    public EnemyCharacterSettings EnemyCharacterSettings { get { return (enemyCharacterSettings == null) ? enemyCharacterSettings = GetComponent<EnemyCharacterSettings>() : enemyCharacterSettings; } }

    private CharacterWeaponController characterWeaponController;
    public CharacterWeaponController CharacterWeaponController { get { return (characterWeaponController == null) ? characterWeaponController = GetComponent<CharacterWeaponController>() : characterWeaponController; } }


    private bool isShooting;
    private float shootTimer;
    public ParticleSystem ShootingParticles;
    public float ShootingSpeed = 1f;
    protected virtual void OnEnable()
    {
        EnemyCharacterSettings.OnControlStateChanged.AddListener(ControlStateUpdate);
    }

    protected virtual void OnDisable()
    {
        EnemyCharacterSettings.OnControlStateChanged.RemoveListener(ControlStateUpdate);
    }

    protected virtual void ControlStateUpdate(bool state)
    {
        if (state)
        {
            ControlStarted();
        }
        else
        {
            ControlStopped();
        }
    }

    protected abstract void ControlStarted();
    protected abstract void ControlStopped();

    protected virtual Vector4 GetRoomDimensionsAsWorldPosition()
    {
        return EnemyCharacterSettings.RoomSettings.GetRoomDimensionsAsWorldPosition();
    }

    protected virtual Vector4 AdjustedRoomDimensions()
    {
        Vector4 adjustingValue = Vector4.one * 1.25f;
        return GetRoomDimensionsAsWorldPosition() - adjustingValue;
    }

    protected virtual Vector3 GetPlayerPosition()
    {
        return EnemyCharacterSettings.RoomSettings.PlayerCharacter.transform.position;
    }

    protected virtual Quaternion GetPlayerDirection()
    {
        Vector3 direction = GetPlayerPosition() - transform.position;
        direction.y = 0f;
        return Quaternion.LookRotation(direction);
    }

    protected void Rotation(Quaternion direction)
    {
        if (EnemyCharacterSettings.LockMovementRotation)
            Graphics.rotation = Quaternion.Lerp(Graphics.rotation, direction, Time.deltaTime * EnemyCharacterSettings.RotationSpeed); // Mostly, direction will be GetPlayerDirection()
    }

    protected void HandleShooting(Action e)
    {
        if (EnemyCharacterSettings.IsControlAllowed)
        {
            if (!isShooting)
            {
                isShooting = true;
                shootTimer = 0f;
            }

            shootTimer += Time.deltaTime;
            if(CharacterWeaponController != null)
            {
                if (shootTimer >= 1f / ItemManager.Instance.GetInventoryItemAsWeapon(CharacterWeaponController.CurrentWeaponID).ShootingSpeed)
                {
                    e.Invoke(); // Mostly, it will be, ShootBullet(GetPlayerDirection())
                    shootTimer = 0f;
                }
            }
            else
            {
                if (shootTimer >= 1f / ShootingSpeed)
                {
                    e.Invoke(); // Mostly, it will be, ShootBullet(GetPlayerDirection())
                    shootTimer = 0f;
                }
            }

        }
        else
        {
            isShooting = false;
        }
    }

    protected void ShootBullet(Quaternion direction)
    {
        if (ShootingParticles != null)
        {
            ShootingParticles.transform.rotation = direction;
            ShootingParticles.Play();
        }
        else
        {
            Debug.LogError("Please assign the shooting particle system in the Inspector.");
        }
    }
}