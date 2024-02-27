using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine.Events;
using static UnityEditor.Progress;
using System.Linq;

public class PickUpController : MonoBehaviour
{
   public string ID;
    public ParticleSystem OnPickedUpEffect;
    public bool PickUpAutomaticly = true;
    public float InteractionSpeed = 2f;
    [ReadOnly]
    public Vector2Int FirstCellPosition;
    [ReadOnly]
    public Vector3 Rotation;

    private InteractionUIController interactionUIController;
    public InteractionUIController InteractionUIController { get { return (interactionUIController == null) ? interactionUIController = GetComponentInChildren<InteractionUIController>() : interactionUIController; } }

    private CharacterInventoryController characterInventoryController;
    public CharacterInventoryController CharacterInventoryController { get { return (characterInventoryController == null) ? characterInventoryController = CombatGameManager.Instance.PlayerCharacter.GetComponent<CharacterInventoryController>(): characterInventoryController; } }

    protected float fillAmount;
    protected bool isPickedUp;
    protected bool isUIOpened;

    [ReadOnly]
    public bool isCreated = false;

    private void OnTriggerEnter(Collider other)
    {
        if (CombatGameManager.Instance.PlayerCharacter == other.GetComponent<CharacterSettings>())
        {
            if (PickUpAutomaticly)
                PickUpEffect(CombatGameManager.Instance.PlayerCharacter);
            else
            {
                if(string.IsNullOrEmpty(ID) == false)
                {
                    if (isCreated == false)
                    {
                        if (InventoryPanelController.Instance.FindEmptyCells(InventoryPanelController.Instance.GroundGrid, InventoryPanelController.Instance.CalculateFilledCellsFromBoolArray(ItemManager.Instance.GetInventoryItem(ID).ItemGrid)) != null)
                        {
                            FirstCellPosition = InventoryPanelController.Instance.FindEmptyCells(InventoryPanelController.Instance.GroundGrid, InventoryPanelController.Instance.CalculateFilledCellsFromBoolArray(ItemManager.Instance.GetInventoryItem(ID).ItemGrid)).First();
                            InventoryManager.Instance.AddData(InventoryType.Ground, ID, FirstCellPosition, Rotation);
                            CharacterInventoryController.AddPickup(this);
                            InventoryPanelController.Instance.ResetEverything();
                        }
                    }
                    else
                    {
                        if (InventoryPanelController.Instance.FindEmptyCells(InventoryPanelController.Instance.GroundGrid, InventoryPanelController.Instance.CalculateFilledCellsFromBoolArray(ItemManager.Instance.GetInventoryItem(ID).ItemGrid)) != null)
                        {
                            isCreated = false;
                            InventoryManager.Instance.AddData(InventoryType.Ground, ID, FirstCellPosition, Rotation);
                            CharacterInventoryController.AddPickup(this);
                            InventoryPanelController.Instance.ResetEverything();
                        }
                    }

                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(!PickUpAutomaticly && CombatGameManager.Instance.PlayerCharacter != null)
        {
            InventoryManager.Instance.RemoveData(InventoryType.Ground, ID, FirstCellPosition, Rotation);
            CharacterInventoryController.RemovePickup(this);
        }
    }

    private void OnDestroy()
    {
        if (CombatGameManager.Instance && !PickUpAutomaticly && CombatGameManager.Instance.PlayerCharacter != null)
        {
            CharacterInventoryController.RemovePickup(this);
        }
    }

    public void DestroyItself()
    {
        Destroy(gameObject);
    }

    public virtual void PickUpEffect(CharacterSettings characterSettings)
    {
        if(OnPickedUpEffect)
            Instantiate(OnPickedUpEffect, transform.position + Vector3.up, Quaternion.identity);
    }

    public virtual void OpenUI() { }
    public virtual void CloseUI() { }
}
