using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionObjectInteractionPanelController : MonoBehaviour
{
    private CanvasGroup canvasGroup;
    public CanvasGroup CanvasGroup { get { return (canvasGroup == null) ? canvasGroup = GetComponent<CanvasGroup>() : canvasGroup; } }

    public Transform InteractionObjectHolder;
    public InventoryObject InventoryObject;
    public InventoryObjectInteraction InventoryObjectInteractionPrefab;
    public List<InventoryInteraction> InventoryInteractions = new List<InventoryInteraction>();

    [ReadOnly]
    public List<InventoryObjectInteraction> InteractionObjects = new List<InventoryObjectInteraction>();

    public void ClosePanel()
    {
        CanvasGroup.alpha = 0f;
        CanvasGroup.interactable = false;
        CanvasGroup.blocksRaycasts = false;
        ClearInteractions();
    }

    public void OpenPanel()
    {
        CanvasGroup.alpha = 1f;
        CanvasGroup.interactable = true;
        CanvasGroup.blocksRaycasts = true;
        CreateInteraction();
    }

    public void CreateInteraction()
    {
        ClearInteractions();

        foreach (InventoryInteraction interaction in InventoryInteractions)
        {
            interaction.InteractionStateControl(InventoryObject, this);
            if (interaction.IsInteractionEnabled)
            {
                InventoryObjectInteraction inventoryObjectInteraction = Instantiate(InventoryObjectInteractionPrefab, InteractionObjectHolder);
                InteractionObjects.Add(inventoryObjectInteraction);
                inventoryObjectInteraction.Initialize(InventoryObject.ID, interaction.Header, () => interaction.Interaction(InventoryObject, this));
            }
        }
    }

    public void ClearInteractions()
    {
        for (int i = 0; i < InteractionObjects.Count; i++)
        {
            Destroy(InteractionObjects[i].gameObject);
        }
        InteractionObjects.Clear();
    }
}
