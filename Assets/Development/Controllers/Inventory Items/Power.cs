using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
[CreateAssetMenu(menuName = "Inventory Items/Power", order = 0)]
public class Power : InterfaceCache, IInventoryItem
{
    [FoldoutGroup("Power Settings")]
    public BasePower BasePowerController;

    [FoldoutGroup("Power Settings")]
    public float Cooldown;

    #region Interface Serialized Values
    [SerializeField]
    [HideLabel]
    [LabelWidth(30)]
    [Title("Type", Bold = true, TitleAlignment = TitleAlignments.Centered)]
    [BoxGroupWithTitleColor("Inventory Settings", 0.11f, 0.88f, 0.81f, 1f)]
    [EnumToggleButtons]
    [ReadOnly]
    private ItemType itemType = ItemType.Power;

    [SerializeField]
    [HideLabel]
    [LabelWidth(30)]
    [Title("ID", Bold = true, TitleAlignment = TitleAlignments.Centered)]
    [BoxGroupWithTitleColor("Inventory Settings", 0.11f, 0.88f, 0.81f, 1f)]
    private string PowerID;

    [SerializeField]
    [HideLabel]
    [LabelWidth(30)]
    [Title("Header", Bold = true, TitleAlignment = TitleAlignments.Centered)]
    [BoxGroupWithTitleColor("Inventory Settings", 0.11f, 0.88f, 0.81f, 1f)]
    private string PowerHeaderText;

    [SerializeField]
    [HideLabel]
    [LabelWidth(30)]
    [Title("Specials", Bold = true, TitleAlignment = TitleAlignments.Centered)]
    [BoxGroupWithTitleColor("Inventory Settings", 0.11f, 0.88f, 0.81f, 1f)]
    private string PowerSpecialsText;

    [SerializeField]
    [HideLabel]
    [LabelWidth(30)]
    [Title("Details", Bold = true, TitleAlignment = TitleAlignments.Centered)]
    [BoxGroupWithTitleColor("Inventory Settings", 0.11f, 0.88f, 0.81f, 1f)]
    private string PowerDetailsText;


    [SerializeField]
    [HideLabel]
    [LabelWidth(30)]
    [Title("Icon", Bold = true, TitleAlignment = TitleAlignments.Centered)]
    [BoxGroupWithTitleColor("Inventory Settings", 0.11f, 0.88f, 0.81f, 1f)]
    [PreviewField(150, ObjectFieldAlignment.Center)]
    private Sprite PowerIcon;

    [SerializeField]
    [HideLabel]
    [LabelWidth(30)]
    [Title("Inventory Image", Bold = true, TitleAlignment = TitleAlignments.Centered)]
    [BoxGroupWithTitleColor("Inventory Settings", 0.11f, 0.88f, 0.81f, 1f)]
    [PreviewField(150, ObjectFieldAlignment.Center)]
    private Sprite PowerInventoryIcon;

    [BoxGroupWithTitleColor("Inventory Settings/Components", 0.11f, 0.88f, 0.81f, 1f)]
    [HideLabel]
    [LabelWidth(30)]
    [Title("Contained Components", Bold = true, TitleAlignment = TitleAlignments.Centered)]
    [TableList(ShowPaging = true, IsReadOnly = false, AlwaysExpanded = true, HideToolbar = false, CellPadding = 10)]
    [SerializeField]
    private List<ComponentInfo> ComponentInfos = new List<ComponentInfo>() { new ComponentInfo(Component.Metal, 0), new ComponentInfo(Component.Cloth, 0), new ComponentInfo(Component.Electronics, 0), new ComponentInfo(Component.Chemicals, 0), };

    [BoxGroupWithTitleColor("Inventory Settings/Item Grid", 0.11f, 0.88f, 0.81f, 1f)]
    [HideLabel]
    [LabelWidth(30)]
    [Title("Inventory Item Size", Bold = true, TitleAlignment = TitleAlignments.Centered)]
    [SerializeField, TableMatrix(HorizontalTitle = "Item Grid", SquareCells = true, ResizableColumns = false, HideColumnIndices = true)]
    [InfoBox("$errorText", InfoMessageType.Error, VisibleIf = "AllItemsAreFalse")]
    private bool[,] itemGrid = new bool[5, 5];
    #endregion

    #region Interface Values
    public string ItemID { get { return PowerID; } set { PowerID = value; } }
    public Sprite ItemIcon { get { return PowerIcon; } set { PowerIcon = value; } }
    public string HeaderText { get { return PowerHeaderText; } set { PowerHeaderText = value; } }
    public string SpecialsText { get { return PowerSpecialsText; } set { PowerSpecialsText = value; } }
    public string DetailsText { get { return PowerDetailsText; } set { PowerDetailsText = value; } }
    public Sprite InventoryIcon { get => PowerInventoryIcon; set => PowerInventoryIcon = value; }
    public bool[,] ItemGrid { get => itemGrid; set => itemGrid = value; }
    public List<ComponentInfo> ComponentData { get => ComponentInfos; set => ComponentInfos = value; }
    public ItemType ItemType { get => itemType; }
    #endregion


    public bool AllGridCellsFalse => AllItemsAreFalse();

    private string errorText = "All elements in the ItemGrid array are false.";

    private bool AllItemsAreFalse()
    {
        foreach (bool item in ItemGrid)
        {
            if (item)
            {
                return false;
            }
        }
        return true;
    }

    public override IInventoryItem GetInterfaceInstance()
    {
        return this;
    }
}
