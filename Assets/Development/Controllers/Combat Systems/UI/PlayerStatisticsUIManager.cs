using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStatisticsUIManager : MonoBehaviour, IPanel
{
    public CharacterSettings PlayerCharacterSettings { get { return CombatGameManager.Instance.PlayerCharacter; } }
    
    private CharacterAmmoController characterAmmoController;
    public CharacterAmmoController CharacterAmmoController { get { return (characterAmmoController == null) ? characterAmmoController = PlayerCharacterSettings.GetComponent<CharacterAmmoController>(): characterAmmoController; } }

    private CharacterMovementController characterMovementController;
    public CharacterMovementController CharacterMovementController { get { return (characterMovementController == null) ? characterMovementController = PlayerCharacterSettings.GetComponent<CharacterMovementController>() : characterMovementController; } }
    
    private CharacterPowerController characterPowerController;
    public CharacterPowerController CharacterPowerController { get { return (characterPowerController == null) ? characterPowerController = PlayerCharacterSettings.GetComponent<CharacterPowerController>() : characterPowerController; } }

    private CharacterWeaponController characterWeaponController;
    public CharacterWeaponController CharacterWeaponController { get { return (characterWeaponController == null) ? characterWeaponController = PlayerCharacterSettings.GetComponent<CharacterWeaponController>() : characterWeaponController; } }

    [FoldoutGroup("References")]
    public TextMeshProUGUI CurrentAmmo;
    [FoldoutGroup("References")]
    public Image AmmoReloadTimer;
    [FoldoutGroup("References")]
    public Image HealthBar;
    [FoldoutGroup("References")]
    public Image DashBar;
    [FoldoutGroup("References")]
    public Image PowerImage;
    [FoldoutGroup("References")]
    public Image PowerCooldownImage;

    private void Update()
    {
        if(string.IsNullOrEmpty(CharacterWeaponController.CurrentWeaponID) == false)
        {
            CurrentAmmo.SetText(PlayerCharacterSettings.CurrentAmmo.ToString() + "\n" + "<color=#ff1500>" + ItemManager.Instance.GetInventoryItemAsWeapon(CharacterWeaponController.CurrentWeaponID).AmmoCapacity.ToString() + "</color>");
            AmmoReloadTimer.fillAmount = CharacterAmmoController.ReloadTimer / ItemManager.Instance.GetInventoryItemAsWeapon(CharacterWeaponController.CurrentWeaponID).ReloadTime;
        }
        else
        {
            CurrentAmmo.SetText(0.ToString() + "\n" + "<color=#ff1500>" + 0.ToString() + "</color>");
            AmmoReloadTimer.fillAmount = 0;
        }
       
        HealthBar.fillAmount = PlayerCharacterSettings.CurrentHealth / PlayerCharacterSettings.MaximumHealth;
        DashBar.fillAmount = CharacterMovementController.LocalDashCooldown / PlayerCharacterSettings.PlayerDashCooldown;

        if (CharacterPowerController.DoesHavePower)
        {
            PowerImage.sprite = ItemManager.Instance.GetInventoryItemAsPower(CharacterPowerController.CurrentPowerID).ItemIcon;
            PowerCooldownImage.fillAmount = CharacterPowerController.LocalCooldown / ItemManager.Instance.GetInventoryItemAsPower(CharacterPowerController.CurrentPowerID).Cooldown;
        }
        else
        {
            PowerImage.sprite = null;
            PowerCooldownImage.fillAmount = 0f;
        }
    }

    #region IPanel Region
    private CanvasGroup canvasGroup;
    public CanvasGroup CanvasGroup { get { return (canvasGroup == null) ? canvasGroup = GetComponent<CanvasGroup>() : canvasGroup; } }
    public bool IsOpen { get => CanvasGroup.alpha == 1; }

    [SerializeField]
    private PanelType panelType;
    public PanelType PanelType { get => panelType; set => panelType = value; }

    public bool IsKeyable => false;

    [SerializeField]
    [ShowIf("IsKeyable")]
    private KeyCode panelKey;
    public KeyCode PanelKey { get => panelKey; set => panelKey = value; }

    public void ClosePanel()
    {
        CanvasGroup.alpha = 0f;
        CanvasGroup.interactable = false;
        MainCanvasManager.Instance.OnPanelClosed.Invoke(this);
    }

    public void OpenPanel()
    {
        CanvasGroup.alpha = 1f;
        CanvasGroup.interactable = true;
        MainCanvasManager.Instance.OnPanelOpened.Invoke(this);
    }
    #endregion
}
