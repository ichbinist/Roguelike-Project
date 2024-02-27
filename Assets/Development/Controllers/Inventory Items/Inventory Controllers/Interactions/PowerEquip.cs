using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PowerEquip : InventoryInteraction
{
    public override void Interaction(InventoryObject inventoryObject, InteractionObjectInteractionPanelController interactionPanel)
    {
        List<InventoryObject> inventoryPowers = new List<InventoryObject>();
        inventoryPowers.AddRange(inventoryObject.InventoryPanelController.InventoryObjects.Where(x => ItemManager.Instance.GetInventoryItem(x.ID).ItemType == ItemType.Power));

        foreach (InventoryObject item in inventoryPowers)
        {
            item.isEquipped = false;
        }

        inventoryObject.isEquipped = true;
        CombatGameManager.Instance.PlayerCharacter.GetComponent<CharacterPowerController>().CurrentPowerID = inventoryObject.ID;
        interactionPanel.ClosePanel();
    }

    public override void InteractionStateControl(InventoryObject inventoryObject, InteractionObjectInteractionPanelController interactionPanel)
    {
        if (inventoryObject.isEquipped == false && inventoryObject.InventoryPanelController.InventoryObjects.Contains(inventoryObject) && ItemManager.Instance.GetInventoryItem(inventoryObject.ID).ItemType == ItemType.Power)
        {
            IsInteractionEnabled = true;
        }
        else
        {
            IsInteractionEnabled = false;
        }
    }
}
