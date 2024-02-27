using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class InventoryManager : Singleton<InventoryManager>
{

    public List<InventoryBaseObjectData> CombatPlayerInventoryData = new List<InventoryBaseObjectData>();

    public List<InventoryBaseObjectData> BasePlayerInventoryData = new List<InventoryBaseObjectData>();

    public List<InventoryBaseObjectData> StorageInventoryData = new List<InventoryBaseObjectData>();

    public List<InventoryBaseObjectData> GroundInventoryData = new List<InventoryBaseObjectData>();

    [HideInInspector]
    public UnityEvent<InventoryBaseObjectData> OnDataChanged = new UnityEvent<InventoryBaseObjectData>();
    [HideInInspector]
    public UnityEvent<InventoryBaseObjectData> OnDataAdded = new UnityEvent<InventoryBaseObjectData>();
    [HideInInspector]
    public UnityEvent<InventoryBaseObjectData> OnDataRemoved = new UnityEvent<InventoryBaseObjectData>();

    public void CombatToBase()
    {
        BasePlayerInventoryData.Clear();
        foreach (InventoryBaseObjectData data in CombatPlayerInventoryData)
        {
            BasePlayerInventoryData.Add(data);
        }
    }

    public void BaseToCombat()
    {
        CombatPlayerInventoryData.Clear();
        foreach (InventoryBaseObjectData data in BasePlayerInventoryData)
        {
            CombatPlayerInventoryData.Add(data);
        }
    }

    public InventoryBaseObjectData GetCombatPlayerData(string id, Vector2Int position, Vector3 rotation)
    {
        InventoryBaseObjectData data = CombatPlayerInventoryData.Find(x => x.ID == id && x.FirstCellPosition == position && x.Rotation == rotation);
        return data;
    }
    public InventoryBaseObjectData GetBasePlayerData(string id, Vector2Int position, Vector3 rotation)
    {
        InventoryBaseObjectData data = BasePlayerInventoryData.Find(x => x.ID == id && x.FirstCellPosition == position && x.Rotation == rotation);
        return data;
    }
    public InventoryBaseObjectData GetStorageData(string id, Vector2Int position, Vector3 rotation)
    {
        InventoryBaseObjectData data = StorageInventoryData.Find(x => x.ID == id && x.FirstCellPosition == position && x.Rotation == rotation);
        return data;
    }
    public InventoryBaseObjectData GetGroundData(string id, Vector2Int position, Vector3 rotation)
    {
        InventoryBaseObjectData data = GroundInventoryData.Find(x => x.ID == id && x.FirstCellPosition == position && x.Rotation == rotation);
        return data;
    }

    public InventoryBaseObjectData GetGeneralItemData(InventoryType inventoryType, string id, Vector2Int position, Vector3 rotation)
    {
        switch (inventoryType)
        {
            case InventoryType.CombatPlayer:
                return GetCombatPlayerData(id, position, rotation);
            case InventoryType.BasePlayer:
                return GetBasePlayerData(id, position, rotation);
            case InventoryType.Storage:
                return GetStorageData(id, position, rotation);
            case InventoryType.Ground:
                return GetGroundData(id, position, rotation);
            default:
                Debug.LogError("Inventory Manager Error: GetGeneralItemData() TYPE IS WRONG");
                return null;
        }
    }


    #region DIÞARIYA YONELÝK KULLANIM
    public List<InventoryBaseObjectData> GetList(InventoryType inventoryType)
    {
        switch (inventoryType)
        {
            case InventoryType.CombatPlayer:
                return CombatPlayerInventoryData;
            case InventoryType.BasePlayer:
                return BasePlayerInventoryData;
            case InventoryType.Storage:
                return StorageInventoryData;
            case InventoryType.Ground:
                return GroundInventoryData;
            default:
                Debug.LogError("Inventory Manager Error: GetList() TYPE IS WRONG");
                return CombatPlayerInventoryData;
        }
    }

    public void ChangeData(InventoryType inventoryType, string id, Vector2Int position, Vector3 rotation, Vector2Int newPosition, Vector3 newRotation)
    {
        InventoryBaseObjectData localObject = GetGeneralItemData(inventoryType, id, position, rotation);
        localObject.FirstCellPosition = newPosition;
        localObject.Rotation = newRotation;
        OnDataChanged.Invoke(localObject);
    }

    public void AddData(InventoryType inventoryType, string id, Vector2Int position, Vector3 rotation)
    {
        InventoryBaseObjectData localObject = new InventoryBaseObjectData(id, position, rotation, inventoryType);
        GetList(inventoryType).Add(localObject);
        OnDataAdded.Invoke(localObject);
    }

    public void RemoveData(InventoryType inventoryType, string id, Vector2Int position, Vector3 rotation)
    {
        GetList(inventoryType).Remove(GetGeneralItemData(inventoryType, id, position, rotation));
        OnDataRemoved.Invoke(new InventoryBaseObjectData(id, position, rotation, inventoryType));
    }
    #endregion
}

public enum InventoryType
{
    CombatPlayer,
    BasePlayer,
    Storage,
    Ground
}