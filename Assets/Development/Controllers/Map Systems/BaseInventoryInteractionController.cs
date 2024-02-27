using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class BaseInventoryInteractionController : BaseInteractionController
{
    public PickUpController PickUpControllerPrefab;
    private CharacterInventoryController characterInventoryController;
    public CharacterInventoryController CharacterInventoryController { get { return (characterInventoryController == null) ? characterInventoryController = CombatGameManager.Instance.PlayerCharacter.GetComponent<CharacterInventoryController>() : characterInventoryController; } }

    [HideInInspector]
    public UnityEvent<InventoryBaseObjectData> OnBaseInventoryDataAdded = new UnityEvent<InventoryBaseObjectData>();
    [HideInInspector]
    public UnityEvent<InventoryBaseObjectData> OnBaseInventoryDataRemoved = new UnityEvent<InventoryBaseObjectData>();

    protected override void AdditionalInteraction()
    {
        base.AdditionalInteraction();

    }

    protected override void OnRangeEnter()
    {
        base.OnRangeEnter();
        InventoryPanelController.Instance.ChangeGroundPanelSetting();
    }

    protected override void OnRangeExit()
    {
        base.OnRangeExit();
        InventoryPanelController.Instance.ChangeGroundPanelSetting();
    }
}

[System.Serializable]
public class InventoryBaseObjectData
{
    public string ID;
    public Vector2Int FirstCellPosition;
    public Vector3 Rotation;
    public InventoryType InventoryType;

    public InventoryBaseObjectData(string id, Vector2Int firstCellPosition, Vector3 rotation, InventoryType ýnventoryType)
    {
        ID = id;
        FirstCellPosition = firstCellPosition;
        Rotation = rotation;
        InventoryType = ýnventoryType;
    }
}