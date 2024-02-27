using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterAmmoController : MonoBehaviour
{
    private CharacterSettings characterSettings;
    public CharacterSettings CharacterSettings { get { return (characterSettings == null) ? characterSettings = GetComponent<CharacterSettings>() : characterSettings; } }

    private CharacterWeaponController characterWeaponController;
    public CharacterWeaponController CharacterWeaponController { get { return (characterWeaponController == null) ? characterWeaponController = GetComponent<CharacterWeaponController>() : characterWeaponController; } }

    private CharacterShootingController characterShootingController;
    public CharacterShootingController CharacterShootingController { get { return (characterShootingController == null) ? characterShootingController = GetComponent<CharacterShootingController>() : characterShootingController; } }

    [HideInInspector]
    public int lastParticleCount;
    [HideInInspector]
    public float ReloadTimer = 0f;
    private ParticleSystem shootingParticles;

    private void OnEnable()
    {
        CharacterSettings.OnWeaponSwitched.AddListener(RefillAmmo);
        CharacterSettings.OnBulletFired += (OnParticleEmitted);
    }

    private void OnDisable()
    {
        CharacterSettings.OnWeaponSwitched.RemoveListener(RefillAmmo);
        CharacterSettings.OnBulletFired -= (OnParticleEmitted);
    }

    public void InitializeAmmoSystem(ParticleSystem particleSystem)
    {  
        if(particleSystem != null)
        {
            shootingParticles = particleSystem;
            var emissionModule = particleSystem.emission;

            // Get an array of bursts
            var bursts = new ParticleSystem.Burst[emissionModule.burstCount];
            emissionModule.GetBursts(bursts);

            lastParticleCount = bursts.Length;
        }
        else
        {
            shootingParticles = null;
            lastParticleCount = 0;
        }
    }

    public void RefillAmmo()
    {
        CharacterSettings.CurrentAmmo = ItemManager.Instance.GetInventoryItemAsWeapon(CharacterWeaponController.CurrentWeaponID).AmmoCapacity;
    }
    public void RefillAmmo(string weaponID)
    {
        CharacterSettings.CurrentAmmo = ItemManager.Instance.GetInventoryItemAsWeapon(weaponID).AmmoCapacity;
    }

    private void Update()
    {
        if (!shootingParticles) return;

        if (Input.GetKeyDown(KeyCode.R) && CharacterSettings.CurrentAmmo != ItemManager.Instance.GetInventoryItemAsWeapon(CharacterWeaponController.CurrentWeaponID).AmmoCapacity)
        {
            CharacterSettings.IsAmmoReloading = true;
            if(CharacterSettings.ReloadSound)
                AudioPoolManager.Instance.PlaySound(CharacterSettings.ReloadSound);
        }
 
        if (CharacterSettings.IsAmmoReloading)
        {
            ReloadTimer += Time.deltaTime;
            if(ReloadTimer >= ItemManager.Instance.GetInventoryItemAsWeapon(CharacterWeaponController.CurrentWeaponID).ReloadTime)
            {
                ReloadTimer = 0f;                
                CharacterSettings.IsAmmoReloading = false;
                CharacterShootingController.shootTimer = 0f;
                RefillAmmo();
            }
        }
    }

    private void OnParticleEmitted()
    {
        CharacterSettings.CurrentAmmo -= 1;

        if(CharacterSettings.CurrentAmmo <= 0)
        {
            CharacterSettings.CurrentAmmo = 0;
        }
    }
}
