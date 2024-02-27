using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class InventoryInteraction : MonoBehaviour
{
    private bool isInteractionEnabled;
    public bool IsInteractionEnabled { get => isInteractionEnabled; set => isInteractionEnabled = value;  }

    public string Header;
    public abstract void InteractionStateControl(InventoryObject inventoryObject, InteractionObjectInteractionPanelController interactionPanel);
    public abstract void Interaction(InventoryObject inventoryObject, InteractionObjectInteractionPanelController interactionPanel);
}
