using Sirenix.OdinInspector;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class InventoryPanelController : Singleton<InventoryPanelController>, IPanel
{
    public InventoryGrid PlayerGrid, GroundGrid;
    public InventoryObject InventoryObjectPrefab;
    public RectTransform ItemArea;

    [ReadOnly]
    public List<InventoryObject> InventoryObjects = new List<InventoryObject>();
    [ReadOnly]
    public List<InventoryObject> GroundObjects = new List<InventoryObject>();

    private CharacterInventoryController characterInventoryController;
    public CharacterInventoryController CharacterInventoryController { get { return (characterInventoryController == null) ? characterInventoryController = CombatGameManager.Instance.PlayerCharacter.GetComponent<CharacterInventoryController>() : characterInventoryController; } }

    public UnityEvent<InventoryObject> OnRightClickInventoryObject = new UnityEvent<InventoryObject>();

    public GameObject GroundPanel;
    private void OnEnable()
    {
        InventoryManager.Instance.OnDataAdded.AddListener(OnInventoryEventDataAdded);
        InventoryManager.Instance.OnDataRemoved.AddListener(OnInventoryEventDataRemoved);
    }

    private void OnDisable()
    {
        if (InventoryManager.Instance && CombatGameManager.Instance != null && CombatGameManager.Instance.PlayerCharacter != null && CharacterInventoryController)
        {
            InventoryManager.Instance.OnDataAdded.RemoveListener(OnInventoryEventDataAdded);
            InventoryManager.Instance.OnDataRemoved.RemoveListener(OnInventoryEventDataRemoved);
        }
    }

    public bool IsDraggingObjects()
    {
        return InventoryObjects.Any(x => x.isDragging) || GroundObjects.Any(x => x.isDragging);
    }

    public bool IsPlayerItem(InventoryObject inventoryObject)
    {
        return InventoryObjects.Contains(inventoryObject);
    }

    private void OnInventoryEventDataAdded(InventoryBaseObjectData inventoryBaseObjectData)
    {
        AddItem(inventoryBaseObjectData);
    }

    private void OnInventoryEventDataRemoved(InventoryBaseObjectData inventoryBaseObjectData)
    {
        RemoveSpesificItem(inventoryBaseObjectData);
    }

    public void ClearGroundItems()
    {
        GroundObjects.RemoveAll(x => x == null);
        foreach (InventoryObject item in GroundObjects)
        {
            List<Vector2Int> oldFilledCells = CalculateTargetedCells(item.StartCell, item.objectSizeAsArea);
            MarkCellsAsEmpty(GroundGrid, oldFilledCells);
            Destroy(item.gameObject);
        }

        GroundObjects.Clear();
    }

    public void ClearPlayerItems()
    {
        InventoryObjects.RemoveAll(x => x == null);
        foreach (InventoryObject item in InventoryObjects)
        {
            List<Vector2Int> oldFilledCells = CalculateTargetedCells(item.StartCell, item.objectSizeAsArea);
            MarkCellsAsEmpty(PlayerGrid, oldFilledCells);
            Destroy(item.gameObject);
        }

        InventoryObjects.Clear();
    }

    public void RemoveSpesificItem(InventoryBaseObjectData inventoryBaseObjectData)
    {
        InventoryObject item = (inventoryBaseObjectData.InventoryType == InventoryType.CombatPlayer || inventoryBaseObjectData.InventoryType == InventoryType.BasePlayer) ? InventoryObjects.Find(x => x.ID == inventoryBaseObjectData.ID && x.StartCell == inventoryBaseObjectData.FirstCellPosition) : GroundObjects.Find(x => x.ID == inventoryBaseObjectData.ID && x.StartCell == inventoryBaseObjectData.FirstCellPosition);
        
        if (inventoryBaseObjectData.InventoryType == InventoryType.CombatPlayer || inventoryBaseObjectData.InventoryType == InventoryType.BasePlayer)
            InventoryObjects.Remove(InventoryObjects.Find(x => x.ID == inventoryBaseObjectData.ID && x.StartCell == inventoryBaseObjectData.FirstCellPosition));
        else
            GroundObjects.Remove(GroundObjects.Find(x => x.ID == inventoryBaseObjectData.ID && x.StartCell == inventoryBaseObjectData.FirstCellPosition));

        if(item != null && item.gameObject != null)
        {
            MarkCellsAsEmpty(GroundGrid, item.filledCells);
            Destroy(item.gameObject);
        }
    }

    public void MarkAllCellsAsEmpty(InventoryGrid grid)
    {
        foreach (GridCell cell in grid.Grid)
        {
            cell.EmptyCell();
        }
    }

    public void AddGroundItems()
    {
        ClearGroundItems();

        foreach (InventoryBaseObjectData data in InventoryManager.Instance.GroundInventoryData)
        {
            AddItem(data);
        }
    }

    public void AddCombatPlayerItems()
    {
        ClearPlayerItems();

        foreach (InventoryBaseObjectData data in InventoryManager.Instance.CombatPlayerInventoryData)
        {
            AddItem(data);
        }
    }

    public void AddStorageItems()
    {
        ClearGroundItems();

        foreach (InventoryBaseObjectData data in InventoryManager.Instance.StorageInventoryData)
        {
            AddItem(data);
        }
    }

    public void AddBasePlayerItems()
    {
        ClearPlayerItems();

        foreach (InventoryBaseObjectData data in InventoryManager.Instance.BasePlayerInventoryData)
        {
            AddItem(data);
        }
    }

    public void ResetEverything()
    {
        if(CombatGameManager.Instance.PlayerCharacter.MapState == MapState.Combat)
        {
            AddGroundItems();
            AddCombatPlayerItems();
        }
        else if (CombatGameManager.Instance.PlayerCharacter.MapState == MapState.Base)
        {
            AddStorageItems();
            AddBasePlayerItems();
        }
    }

    public void AddItem(InventoryBaseObjectData inventoryBaseObjectData)
    {
        AdjustGridListOrder(PlayerGrid);
        AdjustGridListOrder(GroundGrid);

        InventoryGrid grid = (inventoryBaseObjectData.InventoryType == InventoryType.CombatPlayer || inventoryBaseObjectData.InventoryType == InventoryType.BasePlayer) ? PlayerGrid : GroundGrid;
        IInventoryItem item = ItemManager.Instance.GetInventoryItem(inventoryBaseObjectData.ID);
        List<Vector2Int> objectSize = CalculateFilledCellsFromBoolArray(item.ItemGrid);

        List<Vector2Int> targetedCells = CalculateTargetedCells(inventoryBaseObjectData.FirstCellPosition, objectSize);

        Vector3 position = CalculatePosition(targetedCells, grid, ItemArea);
        InventoryObject inventoryObject = InstantiateInventoryObject(inventoryBaseObjectData, position, objectSize);
        List<Vector2Int> targetedCellsAfterRotation = CalculateTargetedCells(inventoryBaseObjectData.FirstCellPosition, inventoryObject.objectSizeAsArea);
        inventoryObject.filledCells = targetedCellsAfterRotation;
        MarkCellsAsFull(grid, targetedCellsAfterRotation);        
    }

    public void RotateClockwise(InventoryObject inventoryObject)
    {
        // Rotate the InventoryImage by 90 degrees clockwise around the Z-axis
        inventoryObject.InventoryImage.rectTransform.Rotate(0f, 0f, -90f);

        Vector3 rotation = inventoryObject.InventoryImage.rectTransform.eulerAngles;
        float zRotation = rotation.z;

        if (zRotation == 270)
        {
            inventoryObject.InventoryImage.rectTransform.anchoredPosition = new Vector2(inventoryObject.InventoryImage.rectTransform.sizeDelta.y, 0);
        }

        if (zRotation == 180)
        {
            inventoryObject.InventoryImage.rectTransform.anchoredPosition = new Vector2(inventoryObject.InventoryImage.rectTransform.sizeDelta.x, -inventoryObject.InventoryImage.rectTransform.sizeDelta.y);
        }

        if (zRotation == 90)
        {
            inventoryObject.InventoryImage.rectTransform.anchoredPosition = new Vector2(0, -inventoryObject.InventoryImage.rectTransform.sizeDelta.x);
        }

        if (zRotation == 0)
        {
            inventoryObject.InventoryImage.rectTransform.anchoredPosition = new Vector2(0, 0);
        }

        // Rotate the objectSizeAsArea list clockwise
        List<Vector2Int> rotatedObjectSizeAsArea = new List<Vector2Int>();
        foreach (Vector2Int cell in inventoryObject.objectSizeAsArea)
        {
            // Calculate the new position after clockwise rotation
            int newX = inventoryObject.ItemSize.y - 1 - cell.y;
            int newY = cell.x;

            rotatedObjectSizeAsArea.Add(new Vector2Int(newX, newY));
        }
        int minx = 0;
        int miny = 0;

        foreach (Vector2Int vector2Int in rotatedObjectSizeAsArea)
        {
            if (minx > vector2Int.x)
            {
                minx = vector2Int.x;
            }
            if (miny > vector2Int.y)
            {
                miny = vector2Int.y;
            }
        }

        for (int i = 0; i < rotatedObjectSizeAsArea.Count; i++)
        {
            rotatedObjectSizeAsArea[i] = new Vector2Int(rotatedObjectSizeAsArea[i].x + Mathf.Abs(minx), rotatedObjectSizeAsArea[i].y + Mathf.Abs(miny));
        }
        // Update the objectSizeAsArea list
        inventoryObject.objectSizeAsArea = rotatedObjectSizeAsArea;
    }

    public Vector2Int CalculateVector2FromBoolArray(bool[,] boolArray)
    {
        int trueCountX = 0;
        int trueCountY = 0;

        // Iterate through the bool array
        for (int i = 0; i < boolArray.GetLength(0); i++)
        {
            for (int j = 0; j < boolArray.GetLength(1); j++)
            {
                if (boolArray[i, j])
                {
                    // Increment the count if the element is true
                    if (i >= trueCountX)
                        trueCountX = i + 1;
                    if (j >= trueCountY)
                        trueCountY = j + 1;
                }
            }
        }

        return new Vector2Int(trueCountX, trueCountY);
    }

    public List<Vector2Int> CalculateTargetedCells(Vector2Int startCell, List<Vector2Int> objectSize)
    {
        List<Vector2Int> targetedCells = new List<Vector2Int>();

        // Iterate over the object size to calculate the absolute position of each cell
        foreach (Vector2Int cell in objectSize)
        {
            // Calculate the absolute position of the cell
            Vector2Int absoluteCell = startCell + cell;

            // Add the absolute cell position to the list of targeted cells
            targetedCells.Add(absoluteCell);
        }

        return targetedCells;
    }


    private void AdjustGridListOrder(InventoryGrid grid)
    {
        grid.Grid.Sort((a, b) => a.transform.GetSiblingIndex().CompareTo(b.transform.GetSiblingIndex()));
    }
    public InventoryGridCellData GetGridPosition(GridCell gridCell)
    {
        // Get the index of the grid cell in the grid's list
        InventoryGrid grid;
        RectTransform _parent;
        bool isPlayerItem;
        if (PlayerGrid.Grid.Contains(gridCell))
        {
            grid = PlayerGrid;
            _parent = ItemArea;
            isPlayerItem = true;
        }
        else
        {
            grid = GroundGrid;
            _parent = ItemArea;
            isPlayerItem = false;
        }
        int index = grid.Grid.IndexOf(gridCell);

        // Calculate the grid position from the index
        int x = index % grid.GridWidth;
        int y = index / grid.GridWidth;

        return new InventoryGridCellData(grid, new Vector2Int(x, y), _parent, isPlayerItem);
    }

    public List<Vector2Int> FindEmptyCells(InventoryGrid grid, List<Vector2Int> filledCells)
    {
        List<Vector2Int> emptyCells = new List<Vector2Int>();

        // Iterate over the grid to find potential placement areas
        for (int x = 0; x < grid.GridWidth; x++)
        {
            for (int y = 0; y < grid.GridHeight; y++)
            {
                bool areaIsEmpty = true;

                // Check if the current cell is part of the potential placement area
                foreach (Vector2Int filledCell in filledCells)
                {
                    Vector2Int cellPosition = new Vector2Int(x, y);
                    Vector2Int absoluteCell = cellPosition + filledCell;

                    // Check if the cell is within the grid boundaries
                    if (absoluteCell.x >= 0 && absoluteCell.x < grid.GridWidth && absoluteCell.y >= 0 && absoluteCell.y < grid.GridHeight)
                    {
                        int index = absoluteCell.x + absoluteCell.y * grid.GridWidth;
                        if (!grid.Grid[index].IsEmpty)
                        {
                            areaIsEmpty = false;
                            break;
                        }
                    }
                    else
                    {
                        areaIsEmpty = false; // Cell is outside the grid boundaries
                        break;
                    }
                }

                // If the area is empty, add the starting position of the potential placement area
                if (areaIsEmpty)
                {
                    emptyCells.Add(new Vector2Int(x, y));
                }
            }
        }

        return emptyCells.Count > 0 ? emptyCells : null;
    }


    private void Update()
    {
        if(GroundGrid.Grid.FindAll(x => x.IsFullButExist).Count != 0 && GroundGrid.Grid.FindAll(x=>x.IsFullButExist).Count != GetGroundCorrectFillCellCount(CombatGameManager.Instance.PlayerCharacter.MapState))
        {
            Debug.Log("Current Full Cells: " + GroundGrid.Grid.FindAll(x => x.IsFullButExist).Count);
            Debug.Log("Data Full Cells: " + GetGroundCorrectFillCellCount(CombatGameManager.Instance.PlayerCharacter.MapState));
            //MarkAllCellsAsEmpty(GroundGrid);
            //AddGroundItems();
        }
    }

    private int GetGroundCorrectFillCellCount(MapState mapState)
    {
        int filledCellCount = 0;

        if(mapState == MapState.Combat)
        {
            foreach (InventoryBaseObjectData data in InventoryManager.Instance.GroundInventoryData)
            {
                for (int i = 0; i < ItemManager.Instance.GetInventoryItem(data.ID).ItemGrid.GetLength(0); i++)
                {
                    for (int j = 0; j < ItemManager.Instance.GetInventoryItem(data.ID).ItemGrid.GetLength(1); j++)
                    {
                        if (ItemManager.Instance.GetInventoryItem(data.ID).ItemGrid[i, j] == true)
                        {
                            filledCellCount++;
                        }
                    }
                }
            }
        }
        else if(mapState == MapState.Base)
        {
            foreach (InventoryBaseObjectData data in InventoryManager.Instance.StorageInventoryData)
            {
                for (int i = 0; i < ItemManager.Instance.GetInventoryItem(data.ID).ItemGrid.GetLength(0); i++)
                {
                    for (int j = 0; j < ItemManager.Instance.GetInventoryItem(data.ID).ItemGrid.GetLength(1); j++)
                    {
                        if (ItemManager.Instance.GetInventoryItem(data.ID).ItemGrid[i, j] == true)
                        {
                            filledCellCount++;
                        }
                    }
                }
            }
        }

        return filledCellCount;
    }

    public void MarkCellsAsFull(InventoryGrid grid, List<Vector2Int> filledCells)
    {
        foreach (Vector2Int cell in filledCells)
        {
            int index = cell.x + cell.y * grid.GridWidth;
            grid.Grid[index].FillCell();
        }
    }

    public void MarkCellsAsEmpty(InventoryGrid grid, List<Vector2Int> filledCells)
    {
        foreach (Vector2Int cell in filledCells)
        {
            int index = cell.x + cell.y * grid.GridWidth;
            grid.Grid[index].EmptyCell();
        }
    }

    private Vector3 CalculatePosition(List<Vector2Int> filledCells, InventoryGrid grid, RectTransform parent)
    {
        // Find the left-uppermost corner of the filled cell area
        Vector2Int minCell = new Vector2Int(int.MaxValue, int.MaxValue);
        foreach (Vector2Int cell in filledCells)
        {
            minCell.x = Mathf.Min(minCell.x, cell.x);
            minCell.y = Mathf.Min(minCell.y, cell.y);
        }

        // Convert the position of the left-uppermost corner to world space
        Vector3 worldPosition = grid.Grid[minCell.x + minCell.y * grid.GridWidth].transform.position;

        // Convert world position to local position
        Vector3 localPosition = parent.InverseTransformPoint(worldPosition);

        return localPosition;
    }


    public List<Vector2Int> CalculateFilledCellsFromBoolArray(bool[,] boolArray)
    {
        List<Vector2Int> filledCells = new List<Vector2Int>();

        int width = boolArray.GetLength(0);
        int height = boolArray.GetLength(1);

        // Iterate through the bool array to find the filled cells
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (boolArray[x, y])
                {
                    filledCells.Add(new Vector2Int(x, y));
                }
            }
        }

        return filledCells;
    }

    private InventoryObject InstantiateInventoryObject(InventoryBaseObjectData inventoryBaseObjectData, Vector3 localPosition, List<Vector2Int> objectSizeArea)
    {
        InventoryObject inventoryObject = Instantiate(InventoryObjectPrefab, ItemArea);
        inventoryObject.ID = inventoryBaseObjectData.ID;
        inventoryObject.transform.localPosition = localPosition;
        inventoryObject.InventoryPanelController = this;
        inventoryObject.objectSizeAsArea = objectSizeArea;
        inventoryObject.Initialize();

        int amount = Mathf.Abs((int)Mathf.Abs(inventoryBaseObjectData.Rotation.z) / 90);
        for (int i = 0; i < amount; i++)
        {
            RotateClockwise(inventoryObject);
        }
        
        inventoryObject.Initialize();
        if (inventoryBaseObjectData.InventoryType == InventoryType.CombatPlayer || inventoryBaseObjectData.InventoryType == InventoryType.BasePlayer)
            InventoryObjects.Add(inventoryObject);
        else
            GroundObjects.Add(inventoryObject);

        inventoryObject.StartCell = inventoryBaseObjectData.FirstCellPosition;

        return inventoryObject;
    }

    public void ChangeGroundPanelSetting()
    {
        BaseInventoryInteractionController baseInventoryInteractionController = FindObjectOfType<BaseInventoryInteractionController>();
        if (CombatGameManager.Instance.PlayerCharacter.MapState == MapState.Base && baseInventoryInteractionController.isInRange == false)
        {
            GroundPanel.SetActive(false);
            ClearGroundItems();
            AddBasePlayerItems();
        }
        else if (CombatGameManager.Instance.PlayerCharacter.MapState == MapState.Base && baseInventoryInteractionController.isInRange == true)
        {
            AddStorageItems();
            AddBasePlayerItems();
            GroundPanel.SetActive(true);
        }

        if (CombatGameManager.Instance.PlayerCharacter.MapState == MapState.Combat)
        {
            ResetEverything();
        }
    }

    #region IPanel Region
    private CanvasGroup canvasGroup;
    public CanvasGroup CanvasGroup { get { return (canvasGroup == null) ? canvasGroup = GetComponent<CanvasGroup>() : canvasGroup; } }

    public bool IsOpen { get => CanvasGroup.alpha == 1; }

    [SerializeField]
    private PanelType panelType;
    public PanelType PanelType { get => panelType; set => panelType = value; }

    public bool IsKeyable => true;

    [SerializeField]
    [ShowIf("IsKeyable")]
    private KeyCode panelKey;
    public KeyCode PanelKey { get => panelKey; set => panelKey = value; }

    public void ClosePanel()
    {
        CombatGameManager.Instance.PlayerCharacter.IsShootingAllowed = true;
        CanvasGroup.alpha = 0f;
        CanvasGroup.interactable = false;
        CanvasGroup.blocksRaycasts = false;
        MainCanvasManager.Instance.OnPanelClosed.Invoke(this);
        ClearGroundItems();
    }

    public void OpenPanel()
    {
        if (CombatGameManager.Instance.PlayerCharacter.MapState != MapState.Overview)
        {
            CombatGameManager.Instance.PlayerCharacter.IsShootingAllowed = false;
            CanvasGroup.alpha = 1f;
            CanvasGroup.interactable = true;
            CanvasGroup.blocksRaycasts = true;
            MainCanvasManager.Instance.OnPanelOpened.Invoke(this);
            ChangeGroundPanelSetting();
        }
    }
    #endregion
}

public class InventoryGridCellData
{
    public InventoryGrid Grid;
    public Vector2Int Position;
    public RectTransform Parent;
    public bool IsPlayerItem;
    public float Distance;
    public InventoryGridCellData(InventoryGrid grid, Vector2Int position, RectTransform parent, bool isPlayerItem, float distance = 0)
    {
        Grid = grid;
        Position = position;
        Parent = parent;
        IsPlayerItem = isPlayerItem;
        Distance = distance;
    }
}