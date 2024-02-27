using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WeaponEquip : InventoryInteraction
{
    public override void Interaction(InventoryObject inventoryObject, InteractionObjectInteractionPanelController interactionPanel)
    {
        List<InventoryObject> inventoryWeapons = new List<InventoryObject>();
        inventoryWeapons.AddRange(inventoryObject.InventoryPanelController.InventoryObjects.Where(x => ItemManager.Instance.GetInventoryItem(x.ID).ItemType == ItemType.Weapon));

        foreach (InventoryObject item in inventoryWeapons)
        {
            item.isEquipped = false;
        }

        inventoryObject.isEquipped = true;
        CombatGameManager.Instance.PlayerCharacter.GetComponent<CharacterWeaponController>().EquipWeapon(inventoryObject.ID);
        interactionPanel.ClosePanel();
    }

    public override void InteractionStateControl(InventoryObject inventoryObject, InteractionObjectInteractionPanelController interactionPanel)
    {
        if(inventoryObject.isEquipped == false && inventoryObject.InventoryPanelController.InventoryObjects.Contains(inventoryObject) && ItemManager.Instance.GetInventoryItem(inventoryObject.ID).ItemType == ItemType.Weapon)
        {
            IsInteractionEnabled = true;
        }
        else
        {
            IsInteractionEnabled = false;
        }
    }
}
