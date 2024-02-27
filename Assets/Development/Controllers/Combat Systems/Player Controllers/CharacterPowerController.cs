using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Sirenix.OdinInspector;

public class CharacterPowerController : MonoBehaviour
{
    [ReadOnly]
    public string CurrentPowerID;
    private Power currentPower { get { return ItemManager.Instance.GetInventoryItemAsPower(CurrentPowerID); } }

    [HideInInspector]
    public UnityEvent<string> OnPowerTaken = new UnityEvent<string>();

    private CharacterSettings characterSettings;
    public CharacterSettings CharacterSettings { get { return (characterSettings == null) ? characterSettings = GetComponent<CharacterSettings>() : characterSettings; } }

    public bool DoesHavePower { get { return !string.IsNullOrEmpty(CurrentPowerID); } }

    public float LocalCooldown = 0f;

    private void Update()
    {
        if (CharacterSettings.MapState != MapState.Combat)
            return;

        if(CombatGameManager.Instance.isDead == false && CombatGameManager.Instance.isPaused == false && CharacterSettings.IsControlAllowed == true)
        {
            if (LocalCooldown <= 0)
            {
                LocalCooldown = 0f;
                UseCooldown();
            }
            else
            {
                LocalCooldown -= Time.deltaTime;
            }
        }
    }

    public void UseCooldown()
    {
        if (Input.GetKeyDown(KeyCode.Q) && DoesHavePower)
        {
            LocalCooldown = currentPower.Cooldown;

            currentPower.BasePowerController.UsePower(transform.position);
        }
    }

    [Button]
    public void GetSpesificPower(string id)
    {
        CurrentPowerID = id;
        OnPowerTaken.Invoke(CurrentPowerID);
    }

    [Button]
    public void ClearPower()
    {
        CurrentPowerID = null;
    }
}
