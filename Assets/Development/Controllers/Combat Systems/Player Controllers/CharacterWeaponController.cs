using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterWeaponController : MonoBehaviour
{
    [ReadOnly]
    public string CurrentWeaponID;

    private CharacterSettings characterSettings;
    public CharacterSettings CharacterSettings { get { return (characterSettings == null) ? characterSettings = GetComponent<CharacterSettings>() : characterSettings; } }

    private CharacterShootingController characterShootingController;
    public CharacterShootingController CharacterShootingController { get { return (characterShootingController == null) ? characterShootingController = GetComponent<CharacterShootingController>() : characterShootingController; } }

    private CharacterAmmoController characterAmmoController;
    public CharacterAmmoController CharacterAmmoController { get { return (characterAmmoController == null) ? characterAmmoController = GetComponent<CharacterAmmoController>() : characterAmmoController; } }

    [ReadOnly]
    public ParticleSystem InstantiatedShootingParticles;

    private void Start()
    {
        // Initialize with the default weapon or any initialization logic you prefer.
    }

    private void ChangeShootingParticles()
    {
        if(InstantiatedShootingParticles != null)
        {
            Destroy(InstantiatedShootingParticles.gameObject);
        }

        if(string.IsNullOrEmpty(CurrentWeaponID) == false)
        {
            InstantiatedShootingParticles = Instantiate(ItemManager.Instance.GetInventoryItemAsWeapon(CurrentWeaponID).ShootingParticles, transform);
            InstantiatedShootingParticles.transform.localPosition = new Vector3(0f, CharacterSettings.ShootingHeight, 0f);
            InstantiatedShootingParticles.transform.SetSiblingIndex(1);
            CharacterShootingController.AssignNewWeapon(InstantiatedShootingParticles);
            CharacterAmmoController.InitializeAmmoSystem(InstantiatedShootingParticles);
        }
        else
        {
            InstantiatedShootingParticles = null;
            CharacterAmmoController.InitializeAmmoSystem(null);
        }
    }

    public void EquipWeapon(string id)
    {
        CurrentWeaponID = id;
        if(string.IsNullOrEmpty(CurrentWeaponID) == false)
        {
            CharacterSettings.TemporaryDamage = ItemManager.Instance.GetInventoryItemAsWeapon(CurrentWeaponID).Damage;
            CharacterSettings.OnWeaponSwitched?.Invoke(CurrentWeaponID);
        }
        ChangeShootingParticles();
    }
}