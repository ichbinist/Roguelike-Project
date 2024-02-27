using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterShootingController : MonoBehaviour
{
    private CharacterSettings characterSettings;
    public CharacterSettings CharacterSettings { get { return (characterSettings == null) ? characterSettings = GetComponent<CharacterSettings>() : characterSettings; } }

    private CharacterWeaponController characterWeaponController;
    public CharacterWeaponController CharacterWeaponController { get { return (characterWeaponController == null) ? characterWeaponController = GetComponent<CharacterWeaponController>() : characterWeaponController; } }

    private CharacterAmmoController characterAmmoController;
    public CharacterAmmoController CharacterAmmoController { get { return (characterAmmoController == null) ? characterAmmoController = GetComponent<CharacterAmmoController>() : characterAmmoController; } }


    private Transform graphics;
    public Transform Graphics { get { return (graphics == null) ? graphics = transform.GetChild(0): graphics; } }

    public ParticleSystem ShootingParticles;
    public Transform AimingHelper;
    
    [HideInInspector]
    public float shootTimer = 0f;
    
    private Quaternion lastDirection;
    private float localMovementSpeed;
    private float localShootingArea;
    public Vector3 LookDirection;

    public void AssignNewWeapon(ParticleSystem particleSystem)
    {
        ShootingParticles = particleSystem;
        localMovementSpeed = CharacterSettings.MovementSpeed;
        localShootingArea = ShootingParticles.shape.angle;
    }

    private void Update()
    {
        if (CharacterSettings.MapState == MapState.Overview)
            return;

        if (CharacterSettings.IsShootingAllowed && CharacterAmmoController.lastParticleCount != 0)
        {          
            RotateGraphics();
            if (CharacterSettings.MapState == MapState.Combat)
            {
                HandleShooting();
            }
            Aim();
        }
    }

    public Quaternion GetDirection()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        Vector3 groundPosition = Vector3.zero;

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, LayerMask.GetMask("Ground", "Wall")))
        {
            groundPosition = hit.point;
            groundPosition.y = CharacterSettings.ShootingHeight;
        }

        Vector3 direction = groundPosition - transform.position;
        direction.y = 0f;
        LookDirection = direction;
        if (direction != Vector3.zero)
        {
            Quaternion newRotation = Quaternion.LookRotation(direction);
            lastDirection = newRotation;
            return newRotation;
        }
        else
        {
            return lastDirection;
        }
    }

    public void RotateGraphics()
    {
        Graphics.rotation = GetDirection();
    }

    private void HandleShooting()
    {
        if (Input.GetMouseButton(0))
        {
            if(CharacterSettings.CurrentAmmo <= 0 && !CharacterSettings.IsAmmoReloading)
            {
                CharacterSettings.IsAmmoReloading = true;
                if(CharacterSettings.ReloadSound)
                    AudioPoolManager.Instance.PlaySound(CharacterSettings.ReloadSound);
            }
            else if(!CharacterSettings.IsAmmoReloading)
            {
                if(shootTimer > 0)
                {
                    shootTimer = Mathf.Clamp(shootTimer - Time.deltaTime, 0f, ItemManager.Instance.GetInventoryItemAsWeapon(CharacterWeaponController.CurrentWeaponID).ShootingSpeed * CharacterSettings.AttackRateMultiplier);
                }
                else
                {
                    ShootBullet();
                }
            }
        }
        else
        {
            if (shootTimer > 0)
            {
                shootTimer = Mathf.Clamp(shootTimer - Time.deltaTime, 0f, ItemManager.Instance.GetInventoryItemAsWeapon(CharacterWeaponController.CurrentWeaponID).ShootingSpeed * CharacterSettings.AttackRateMultiplier);
            }
        }
    }

    private void ShootBullet()
    {
        if (ShootingParticles != null && CharacterSettings.IsShootingAllowed)
        {
            ShootingParticles.transform.rotation = GetDirection();
            ShootingParticles.Play();
            CharacterSettings?.OnBulletFired.Invoke();
            shootTimer = ItemManager.Instance.GetInventoryItemAsWeapon(CharacterWeaponController.CurrentWeaponID).ShootingSpeed * CharacterSettings.AttackRateMultiplier;
        }
    }

    private void Aim()
    {
        if (Input.GetMouseButton(1) && (CharacterSettings.MapState == MapState.Combat))
        {
            ParticleSystem.ShapeModule shapeModule = ShootingParticles.shape;
            shapeModule.angle = ItemManager.Instance.GetInventoryItemAsWeapon(CharacterWeaponController.CurrentWeaponID).AimingMinimumValue;
            CharacterSettings.MovementSpeed = ItemManager.Instance.GetInventoryItemAsWeapon(CharacterWeaponController.CurrentWeaponID).AimingMovementSpeed;
            Camera.main.GetComponent<Cinemachine.CinemachineBrain>().ActiveVirtualCamera.LookAt = AimingHelper;
            CursorManager.Instance.SetCursorTargetSizeRelevantToCurrent(ItemManager.Instance.GetInventoryItemAsWeapon(CharacterWeaponController.CurrentWeaponID).CursorAimingMultiplier);
        }
        else
        {
            ParticleSystem.ShapeModule shapeModule = ShootingParticles.shape;
            shapeModule.angle = localShootingArea;
            CharacterSettings.MovementSpeed = localMovementSpeed;
            Camera.main.GetComponent<Cinemachine.CinemachineBrain>().ActiveVirtualCamera.LookAt = transform;
        }
    }
}
