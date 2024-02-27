using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WeaponUnequip : InventoryInteraction
{
    public override void Interaction(InventoryObject inventoryObject, InteractionObjectInteractionPanelController interactionPanel)
    {
        inventoryObject.isEquipped = false;
        CombatGameManager.Instance.PlayerCharacter.GetComponent<CharacterWeaponController>().EquipWeapon("");
        interactionPanel.ClosePanel();
    }

    public override void InteractionStateControl(InventoryObject inventoryObject, InteractionObjectInteractionPanelController interactionPanel)
    {
        if (inventoryObject.isEquipped == true && inventoryObject.InventoryPanelController.InventoryObjects.Contains(inventoryObject) && ItemManager.Instance.GetInventoryItem(inventoryObject.ID).ItemType == ItemType.Weapon)
        {
            IsInteractionEnabled = true;
        }
        else
        {
            IsInteractionEnabled = false;
        }
    }
}
