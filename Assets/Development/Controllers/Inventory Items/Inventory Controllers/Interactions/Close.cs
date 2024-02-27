using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Close : InventoryInteraction
{
    public override void Interaction(InventoryObject inventoryObject, InteractionObjectInteractionPanelController interactionPanel)
    {
        interactionPanel.ClosePanel();
    }

    public override void InteractionStateControl(InventoryObject inventoryObject, InteractionObjectInteractionPanelController interactionPanel)
    {
        IsInteractionEnabled = true;
    }
}