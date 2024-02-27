using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventoryObject : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerClickHandler,IPointerEnterHandler,IPointerExitHandler
{
    public Image InventoryImage;
    public Vector2 CellSize;
    public PickUpController PickUpControllerPrefab;
    public InteractionObjectInteractionPanelController InteractionObjectInteractionPanel;
    private BaseInventoryInteractionController BaseInventoryInteractionController;
    private RectTransform rectTransform;
    private Canvas canvas;
    private CanvasGroup canvasGroup;
    private Vector2 originalPosition;
    private Vector3 previousRotation;
    private ItemType itemType;
    public ItemType ItemType { get { return itemType; } set => itemType = value; }

    [ReadOnly]
    public bool isDragging = false;
    private RectTransform canvasRectTransform;
    [ReadOnly]
    public bool isEquipped = false;
    [ReadOnly]
    public InventoryPanelController InventoryPanelController;
    [ReadOnly]
    public string ID;
    [ReadOnly]
    public Vector2Int ItemSize = Vector2Int.one;
    [ReadOnly]
    public Vector2Int StartCell;
    [ReadOnly]
    public Vector2Int PreviousStartCell;
    [ReadOnly]
    public PickUpController CurrentPickUpController;
    [ReadOnly]
    public List<Vector2Int> filledCells = new List<Vector2Int>();
    [ReadOnly]
    public List<Vector2Int> objectSizeAsArea = new List<Vector2Int>();
    public List<Vector2Int> previousObjectSizeArea = new List<Vector2Int>();

    private CharacterInventoryController characterInventoryController;
    public CharacterInventoryController CharacterInventoryController { get { return (characterInventoryController == null) ? characterInventoryController = CombatGameManager.Instance.PlayerCharacter.GetComponent<CharacterInventoryController>() : characterInventoryController; } }

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        canvas = GetComponentInParent<Canvas>();
        canvasRectTransform = canvas.GetComponent<RectTransform>();
        BaseInventoryInteractionController = FindObjectOfType<BaseInventoryInteractionController>();
    }

    public void Initialize()
    {
        InventoryImage.sprite = ItemManager.Instance.GetInventoryItem(ID).InventoryIcon;
        ItemSize = CalculateVector2FromBoolArray(ItemManager.Instance.GetInventoryItem(ID).ItemGrid);
        RectTransform rectTransform = InventoryImage.GetComponent<RectTransform>();
        rectTransform.sizeDelta = CalculateImageSizeDelta();
        InventoryPanelController.OnRightClickInventoryObject.AddListener(CheckIfRightClicked);
    }

    private void OnDisable()
    {
        InventoryPanelController.OnRightClickInventoryObject.RemoveListener(CheckIfRightClicked);
    }

    private void CheckIfRightClicked(InventoryObject inventoryObject)
    {
        if(inventoryObject != null  && inventoryObject != this)
        {
            InteractionObjectInteractionPanel.ClosePanel();
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (InventoryPanelController.IsDraggingObjects() == false && eventData.button == PointerEventData.InputButton.Right)
        {
            InteractionObjectInteractionPanel.OpenPanel();
            InventoryPanelController.OnRightClickInventoryObject.Invoke(this);
        }
        else if(InventoryPanelController.IsDraggingObjects())
        {
            InteractionObjectInteractionPanel.ClosePanel();
        }
    }
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!isDragging)
        {
            transform.SetAsLastSibling();
            InteractionObjectInteractionPanel.ClosePanel();

            originalPosition = rectTransform.anchoredPosition;
            isDragging = true;
            canvasGroup.blocksRaycasts = false; // Disable raycasts on the object during drag

            if (previousObjectSizeArea.Count == 0)
            {
                previousObjectSizeArea = objectSizeAsArea;
            }

            previousRotation = new Vector3(0, 0, (InventoryImage.rectTransform.eulerAngles.z == 0) ? 0 : 360f - InventoryImage.rectTransform.eulerAngles.z);
            PreviousStartCell = StartCell;
        }
    }
    private void Update()
    {
        if (!isDragging && InventoryPanelController.IsDraggingObjects())
        {
            InteractionObjectInteractionPanel.ClosePanel();
        }

        if (isDragging)
        {
            if(Input.GetMouseButtonDown(1))
            {
                RotateClockwise();
            }
        }
    }
    public void OnDrag(PointerEventData eventData)
    {
        if (isDragging)
        {
            // Calculate local position relative to the canvas
            RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRectTransform, eventData.position, eventData.pressEventCamera, out Vector2 localMousePos);
            Vector2 localCanvasPos = canvasRectTransform.InverseTransformPoint(rectTransform.position);
            Vector3 deltaLocalPos = localMousePos - localCanvasPos;

            // Apply the calculated local position
            rectTransform.localPosition += deltaLocalPos;

            Vector2 mouseWorldPosition = eventData.position;

            if (CombatGameManager.Instance.PlayerCharacter.MapState != MapState.Overview)
                GridShowControls(mouseWorldPosition);
            else
                ClearGridShows();
        }
    }

    private void GridShowControls(Vector2 mousePos)
    {
        InventoryGridCellData gridCellData = FindClosestGridCell(mousePos);
        Vector2Int closestCell = gridCellData.Position;
        List<Vector2Int> oldFilledCells = InventoryPanelController.CalculateTargetedCells(closestCell, objectSizeAsArea);

        ClearGridShows();

        foreach (Vector2Int cell in oldFilledCells)
        {
            int index = cell.x + cell.y * gridCellData.Grid.GridWidth;
            if (cell.x < 0 || cell.x >= gridCellData.Grid.GridWidth || cell.y < 0 || cell.y >= gridCellData.Grid.GridHeight)
            {
                ClearGridShows();
                break;
            }
            else
            {
                gridCellData.Grid.Grid[index].ShowStatus();
            }
        }
    }

    private void ClearGridShows()
    {
        foreach (GridCell cell in InventoryPanelController.PlayerGrid.Grid)
        {
            cell.HideStatus();
        }

        foreach (GridCell cell in InventoryPanelController.GroundGrid.Grid)
        {
            cell.HideStatus();
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (isDragging)
        {
            isDragging = false;
            canvasGroup.blocksRaycasts = true;

            // Find the closest grid cell to the mouse position
            Vector2 mouseWorldPosition = eventData.position;

            InventoryGridCellData gridCellData = FindClosestGridCell(mouseWorldPosition);
            Vector2Int closestCell = gridCellData.Position;

            foreach (GridCell cell in gridCellData.Grid.Grid)
            {
                cell.HideStatus();
            }

            if(CombatGameManager.Instance.PlayerCharacter.MapState == MapState.Overview)
            {
                rectTransform.anchoredPosition = originalPosition;
                return;
            }

            // Check if there's enough space to drop the item
            if (IsEnoughSpaceToDrop(closestCell, objectSizeAsArea, gridCellData.Grid))
            {
                // Place the item in the closest cell
                transform.SetParent(InventoryPanelController.ItemArea, false);

                rectTransform.anchoredPosition = GetCellPosition(closestCell, gridCellData.Grid, gridCellData.Parent);

                if (gridCellData.IsPlayerItem)
                {
                    if (!InventoryPanelController.IsPlayerItem(this))
                    {
                        //Ground to Player
                        InventoryPanelController.GroundObjects.Remove(this);
                        InventoryPanelController.InventoryObjects.Add(this);

                        List<Vector2Int> oldFilledCells = InventoryPanelController.CalculateTargetedCells(StartCell, previousObjectSizeArea);
                        InventoryPanelController.MarkCellsAsEmpty(InventoryPanelController.GroundGrid, oldFilledCells);
                        List<Vector2Int> newFilledCells = InventoryPanelController.CalculateTargetedCells(closestCell, objectSizeAsArea);
                        InventoryPanelController.MarkCellsAsFull(InventoryPanelController.PlayerGrid, newFilledCells);
                        StartCell = gridCellData.Position;

                        GroundToPlayer();
                    }
                    else
                    {
                        //Player To Player
                        List<Vector2Int> oldFilledCells = InventoryPanelController.CalculateTargetedCells(StartCell, previousObjectSizeArea);
                        InventoryPanelController.MarkCellsAsEmpty(InventoryPanelController.PlayerGrid, oldFilledCells);
                        List<Vector2Int> newFilledCells = InventoryPanelController.CalculateTargetedCells(closestCell, objectSizeAsArea);
                        InventoryPanelController.MarkCellsAsFull(InventoryPanelController.PlayerGrid, newFilledCells);
                        StartCell = gridCellData.Position;

                        if (CombatGameManager.Instance.PlayerCharacter.MapState == MapState.Combat)
                        {
                            InventoryManager.Instance.ChangeData(InventoryType.CombatPlayer,ID, PreviousStartCell, previousRotation,StartCell, new Vector3(0,0, (InventoryImage.rectTransform.eulerAngles.z == 0) ? 0 : 360f - InventoryImage.rectTransform.eulerAngles.z));
                        }
                        else if(CombatGameManager.Instance.PlayerCharacter.MapState == MapState.Base)
                        {
                            InventoryManager.Instance.ChangeData(InventoryType.BasePlayer, ID, PreviousStartCell, previousRotation, StartCell, new Vector3(0, 0, (InventoryImage.rectTransform.eulerAngles.z == 0) ? 0 : 360f - InventoryImage.rectTransform.eulerAngles.z));
                        }
                        InventoryPanelController.ChangeGroundPanelSetting();
                    }
                }
                else
                {
                    if (InventoryPanelController.IsPlayerItem(this))
                    {
                        //Player To Ground
                        InventoryPanelController.GroundObjects.Add(this);
                        InventoryPanelController.InventoryObjects.Remove(this);

                        List<Vector2Int> oldFilledCells = InventoryPanelController.CalculateTargetedCells(StartCell, previousObjectSizeArea);
                        InventoryPanelController.MarkCellsAsEmpty(InventoryPanelController.PlayerGrid, oldFilledCells);
                        List<Vector2Int> newFilledCells = InventoryPanelController.CalculateTargetedCells(closestCell, objectSizeAsArea);
                        InventoryPanelController.MarkCellsAsFull(InventoryPanelController.GroundGrid, newFilledCells);
                        StartCell = gridCellData.Position;

                        PlayerToGround();
                    }
                    else
                    {
                        //Ground to Ground
                        List<Vector2Int> oldFilledCells = InventoryPanelController.CalculateTargetedCells(StartCell, previousObjectSizeArea);
                        InventoryPanelController.MarkCellsAsEmpty(InventoryPanelController.GroundGrid, oldFilledCells);
                        List<Vector2Int> newFilledCells = InventoryPanelController.CalculateTargetedCells(closestCell, objectSizeAsArea);
                        InventoryPanelController.MarkCellsAsFull(InventoryPanelController.GroundGrid, newFilledCells);

                        if (CombatGameManager.Instance.PlayerCharacter.MapState == MapState.Combat)
                        {
                            InventoryManager.Instance.ChangeData(InventoryType.Ground, ID, PreviousStartCell, previousRotation, gridCellData.Position, new Vector3(0, 0, (InventoryImage.rectTransform.eulerAngles.z == 0) ? 0 : 360f - InventoryImage.rectTransform.eulerAngles.z));
                        }
                        else if (CombatGameManager.Instance.PlayerCharacter.MapState == MapState.Base)
                        {
                            InventoryManager.Instance.ChangeData(InventoryType.Storage, ID, PreviousStartCell, previousRotation, gridCellData.Position, new Vector3(0, 0, (InventoryImage.rectTransform.eulerAngles.z == 0) ? 0 : 360f - InventoryImage.rectTransform.eulerAngles.z));
                        }

                        StartCell = gridCellData.Position;
                        InventoryPanelController.ChangeGroundPanelSetting();

                    }
                }
                previousObjectSizeArea = objectSizeAsArea;
            }
            else
            {
                // If not enough space, return to original position
                rectTransform.anchoredPosition = originalPosition;
            }
        }
    }

    private void PlayerToGround()
    {
        if (isEquipped)
        {
            if (ItemType == ItemType.Weapon)
            {
                List<InventoryObject> inventoryWeapons = new List<InventoryObject>();
                inventoryWeapons.AddRange(InventoryPanelController.InventoryObjects.Where(x => ItemManager.Instance.GetInventoryItem(x.ID).ItemType == ItemType.Weapon));

                foreach (InventoryObject item in inventoryWeapons)
                {
                    item.isEquipped = false;
                }
                CombatGameManager.Instance.PlayerCharacter.GetComponent<CharacterWeaponController>().EquipWeapon(string.Empty);
            }
            else if (ItemType == ItemType.Power)
            {
                List<InventoryObject> inventoryPowers = new List<InventoryObject>();
                inventoryPowers.AddRange(InventoryPanelController.InventoryObjects.Where(x => ItemManager.Instance.GetInventoryItem(x.ID).ItemType == ItemType.Power));

                foreach (InventoryObject item in inventoryPowers)
                {
                    item.isEquipped = false;
                }
                CombatGameManager.Instance.PlayerCharacter.GetComponent<CharacterPowerController>().CurrentPowerID = string.Empty;
            }
        }

        if (CombatGameManager.Instance.PlayerCharacter.MapState == MapState.Combat)
        {
            InventoryManager.Instance.RemoveData(InventoryType.CombatPlayer, ID, PreviousStartCell, previousRotation);
            PickUpController pickUpController = Instantiate(PickUpControllerPrefab, new Vector3(999,999,999), Quaternion.identity, CombatGameManager.Instance.PlayerCharacter.transform);
            pickUpController.transform.parent = null;
            pickUpController.isCreated = true;
            Vector2 vector2 = Random.insideUnitCircle;
            Vector3 randomizedPos = new Vector3(vector2.x, 0f, vector2.y);
            pickUpController.ID = ID;
            pickUpController.PickUpAutomaticly = false;
            pickUpController.FirstCellPosition = StartCell;
            pickUpController.transform.position = CombatGameManager.Instance.PlayerCharacter.transform.position + randomizedPos * 4f;
            Destroy(gameObject);
        }
        else
        {
            InventoryManager.Instance.RemoveData(InventoryType.BasePlayer, ID, PreviousStartCell, previousRotation);
            InventoryManager.Instance.AddData(InventoryType.Storage, ID, StartCell, new Vector3(0, 0, (InventoryImage.rectTransform.eulerAngles.z == 0) ? 0 : 360f - InventoryImage.rectTransform.eulerAngles.z));
        }

        InventoryPanelController.ChangeGroundPanelSetting();
    }

    private void GroundToPlayer()
    {
        if (CombatGameManager.Instance.PlayerCharacter.MapState == MapState.Base)
        {
            InventoryManager.Instance.RemoveData(InventoryType.Storage, ID, PreviousStartCell, previousRotation);
            InventoryManager.Instance.AddData(InventoryType.BasePlayer, ID, StartCell, new Vector3(0, 0, (InventoryImage.rectTransform.eulerAngles.z == 0) ? 0 : 360f - InventoryImage.rectTransform.eulerAngles.z));
        }
        else if (CombatGameManager.Instance.PlayerCharacter.MapState == MapState.Combat)
        {
            CharacterInventoryController.PickUpControllers.Find(x => x.ID == ID).DestroyItself();
            InventoryManager.Instance.RemoveData(InventoryType.Ground, ID, PreviousStartCell, previousRotation);
            InventoryManager.Instance.AddData(InventoryType.CombatPlayer, ID, StartCell, new Vector3(0, 0, (InventoryImage.rectTransform.eulerAngles.z == 0) ? 0 : 360f - InventoryImage.rectTransform.eulerAngles.z));
        }

        InventoryPanelController.ChangeGroundPanelSetting();
    }

    public void RotateClockwise()
    {
        // Rotate the InventoryImage by 90 degrees clockwise around the Z-axis
        InventoryImage.rectTransform.Rotate(0f, 0f, -90f);

        Vector3 rotation = InventoryImage.rectTransform.eulerAngles;
        float zRotation = rotation.z;

        if(zRotation == 270)
        {
            InventoryImage.rectTransform.anchoredPosition = new Vector2(InventoryImage.rectTransform.sizeDelta.y, 0);
        }

        if (zRotation == 180)
        {
            InventoryImage.rectTransform.anchoredPosition = new Vector2(InventoryImage.rectTransform.sizeDelta.x, -InventoryImage.rectTransform.sizeDelta.y);
        }

        if (zRotation == 90)
        {
            InventoryImage.rectTransform.anchoredPosition = new Vector2(0, -InventoryImage.rectTransform.sizeDelta.x);
        }

        if (zRotation == 0)
        {
            InventoryImage.rectTransform.anchoredPosition = new Vector2(0,0);
        }

        // Rotate the objectSizeAsArea list clockwise
        List<Vector2Int> rotatedObjectSizeAsArea = new List<Vector2Int>();
        foreach (Vector2Int cell in objectSizeAsArea)
        {
            // Calculate the new position after clockwise rotation
            int newX = ItemSize.y - 1 - cell.y;
            int newY = cell.x;

            rotatedObjectSizeAsArea.Add(new Vector2Int(newX, newY));
        }
        int minx = 0;
        int miny = 0;

        foreach (Vector2Int vector2Int in rotatedObjectSizeAsArea)
        {
            if(minx > vector2Int.x)
            {
                minx = vector2Int.x;
            }
            if(miny > vector2Int.y)
            {
                miny = vector2Int.y;
            }
        }

        for(int i = 0; i < rotatedObjectSizeAsArea.Count; i++)
        {
            rotatedObjectSizeAsArea[i] = new Vector2Int(rotatedObjectSizeAsArea[i].x + Mathf.Abs(minx), rotatedObjectSizeAsArea[i].y + Mathf.Abs(miny));
        }

        // Update the objectSizeAsArea list
        objectSizeAsArea = rotatedObjectSizeAsArea;
        GridShowControls(Input.mousePosition);
    }


    public Vector3 GetCellPosition(Vector2Int closestCell, InventoryGrid grid, RectTransform parent)
    {
        // Calculate the index of the cell in the grid's list of cells
        int index = closestCell.x + closestCell.y * grid.GridWidth;

        // Calculate the world position of the cell
        Vector3 worldPosition = grid.Grid[index].transform.position;

        // Convert the world position to local position relative to the parent object
        Vector3 localPosition = parent.InverseTransformPoint(worldPosition);

        return localPosition;
    }

    private InventoryGridCellData FindClosestGridCell(Vector2 mousePosition)
    {
        // Placeholder logic - you need to replace this with your actual implementation
        // Iterate through the PlayerGrid and GroundGrid to find the closest cell
        Vector2Int closestCell = Vector2Int.zero;
        float closestDistance = float.MaxValue;
        RectTransform parent = null;
        InventoryGrid grid = null;
        bool isPlayer = false;
        if (CombatGameManager.Instance && CombatGameManager.Instance.PlayerCharacter.MapState != MapState.Overview)
        {
            foreach (GridCell cell in InventoryPanelController.PlayerGrid.Grid)
            {
                float distance = Vector2.Distance(cell.transform.position, mousePosition);
                if (distance < closestDistance)
                {
                    closestCell = InventoryPanelController.GetGridPosition(cell).Position;
                    grid = InventoryPanelController.GetGridPosition(cell).Grid;
                    parent = InventoryPanelController.GetGridPosition(cell).Parent;
                    closestDistance = distance;
                    isPlayer = true;
                }
            }

            if (CombatGameManager.Instance && CombatGameManager.Instance.PlayerCharacter.MapState == MapState.Base)
            {
                //Diðerlerinin Base Versyonu
                if (BaseInventoryInteractionController.isInRange)
                {
                    foreach (GridCell cell in InventoryPanelController.GroundGrid.Grid)
                    {
                        float distance = Vector2.Distance(cell.transform.position, mousePosition);
                        if (distance < closestDistance)
                        {
                            closestCell = InventoryPanelController.GetGridPosition(cell).Position;
                            grid = InventoryPanelController.GetGridPosition(cell).Grid;
                            parent = InventoryPanelController.GetGridPosition(cell).Parent;
                            closestDistance = distance;
                            isPlayer = false;
                        }
                    }
                }
            }

            if (CombatGameManager.Instance && CombatGameManager.Instance.PlayerCharacter.MapState == MapState.Combat)
            {
                foreach (GridCell cell in InventoryPanelController.GroundGrid.Grid)
                {
                    float distance = Vector2.Distance(cell.transform.position, mousePosition);
                    if (distance < closestDistance)
                    {
                        closestCell = InventoryPanelController.GetGridPosition(cell).Position;
                        grid = InventoryPanelController.GetGridPosition(cell).Grid;
                        parent = InventoryPanelController.GetGridPosition(cell).Parent;
                        closestDistance = distance;
                        isPlayer = false;
                    }
                }
            }
        }
        return new InventoryGridCellData(grid, closestCell, parent, isPlayer, closestDistance);
    }

    public bool IsEnoughSpaceToDrop(Vector2Int closestCell, List<Vector2Int> filledCells, InventoryGrid grid)
    {
        // Check if any of the cells within the object area are already filled
        foreach (Vector2Int filledCell in filledCells)
        {
            // Calculate the absolute position of the filled cell
            Vector2Int absoluteCell = closestCell + filledCell;

            // Check if the calculated position is within the grid boundaries
            if (absoluteCell.x < 0 || absoluteCell.x >= grid.GridWidth || absoluteCell.y < 0 || absoluteCell.y >= grid.GridHeight)
            {
                return false; // Cell is outside the grid boundaries
            }

            int index = absoluteCell.x + absoluteCell.y * grid.GridWidth;
            if (!grid.Grid[index].IsEmpty)
            {
                return false; // Cell is already filled
            }
        }

        // If all cells are empty, there is enough space to drop the item
        return true;
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

    private Vector2 CalculateImageSizeDelta()
    {
        Vector2 imageSize = Vector2.Scale(ItemSize, CellSize);

        return imageSize;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        ItemType = ItemManager.Instance.GetInventoryItem(ID).ItemType;

        switch (ItemType)
        {
            case ItemType.Weapon:
                SetWeaponCardInfo();
                PowerCardViewRoomController.Instance.CardEnter(CardType.Weapon);
                PowerCardViewRoomController.Instance.CardExit(CardType.Power);
                break;

            case ItemType.Power:
                SetPowerCardInfo();
                PowerCardViewRoomController.Instance.CardExit(CardType.Weapon);
                PowerCardViewRoomController.Instance.CardEnter(CardType.Power);
                break;
    
            case ItemType.Material:
                SetPowerCardInfo();
                PowerCardViewRoomController.Instance.CardEnter(CardType.Weapon);
                PowerCardViewRoomController.Instance.CardExit(CardType.Power);
                break;
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        ItemType = ItemManager.Instance.GetInventoryItem(ID).ItemType;

        switch (ItemType)
        {
            case ItemType.Weapon:
                PowerCardViewRoomController.Instance.CardExit(CardType.Weapon);
                PowerCardViewRoomController.Instance.CardExit(CardType.Power);
                break;

            case ItemType.Power:
                PowerCardViewRoomController.Instance.CardExit(CardType.Weapon);
                PowerCardViewRoomController.Instance.CardExit(CardType.Power);
                break;

            case ItemType.Material:
                PowerCardViewRoomController.Instance.CardExit(CardType.Weapon);
                PowerCardViewRoomController.Instance.CardExit(CardType.Power);
                break;
        }
    }

    private void SetPowerCardInfo()
    {
        PowerCardViewRoomController.Instance.PowerCardController.SpriteRenderer.sprite = ItemManager.Instance.GetInventoryItem(ID).ItemIcon;
        PowerCardViewRoomController.Instance.PowerCardController.HeaderText.SetText(ItemManager.Instance.GetInventoryItem(ID).HeaderText);
        PowerCardViewRoomController.Instance.PowerCardController.DetailsText.SetText(ItemManager.Instance.GetInventoryItem(ID).DetailsText);
        PowerCardViewRoomController.Instance.PowerCardController.SpecialsText.SetText(ItemManager.Instance.GetInventoryItem(ID).SpecialsText);
    }

    private void SetWeaponCardInfo()
    {
        PowerCardViewRoomController.Instance.WeaponCardController.WeaponIcon.sprite = ItemManager.Instance.GetInventoryItem(ID).ItemIcon;
        PowerCardViewRoomController.Instance.WeaponCardController.HeaderText.SetText(ItemManager.Instance.GetInventoryItem(ID).HeaderText);
        PowerCardViewRoomController.Instance.WeaponCardController.DetailsText.SetText(ItemManager.Instance.GetInventoryItem(ID).DetailsText);
        PowerCardViewRoomController.Instance.WeaponCardController.SpecialsText.SetText(ItemManager.Instance.GetInventoryItem(ID).SpecialsText);
        PowerCardViewRoomController.Instance.WeaponCardController.DamageText.SetText(ItemManager.Instance.GetInventoryItemAsWeapon(ID).Damage.ToString());
        PowerCardViewRoomController.Instance.WeaponCardController.AttackSpeedText.SetText(ItemManager.Instance.GetInventoryItemAsWeapon(ID).ShootingSpeed.ToString());
        PowerCardViewRoomController.Instance.WeaponCardController.MaganizeText.SetText(ItemManager.Instance.GetInventoryItemAsWeapon(ID).AmmoCapacity.ToString());
    }

    private void OnDestroy()
    {
        if (!ItemManager.Instance || !PowerCardViewRoomController.Instance)
            return;

        ItemType = ItemManager.Instance.GetInventoryItem(ID).ItemType;

        ClearGridShows();

        switch (ItemType)
        {
            case ItemType.Weapon:
                PowerCardViewRoomController.Instance.CardExit(CardType.Weapon);
                PowerCardViewRoomController.Instance.CardExit(CardType.Power);
                break;

            case ItemType.Power:
                PowerCardViewRoomController.Instance.CardExit(CardType.Weapon);
                PowerCardViewRoomController.Instance.CardExit(CardType.Power);
                break;

            case ItemType.Material:
                PowerCardViewRoomController.Instance.CardExit(CardType.Weapon);
                PowerCardViewRoomController.Instance.CardExit(CardType.Power);
                break;
        }
    }
}
