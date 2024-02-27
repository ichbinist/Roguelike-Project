using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using Sirenix.OdinInspector.Editor;
using UnityEditor;
#endif

[System.Serializable]
[CreateAssetMenu(menuName = "Inventory Items/Material", order = 2)]
public class Material : InterfaceCache, IInventoryItem
{
    #region Interface Serialized Values
    [SerializeField]
    [HideLabel]
    [LabelWidth(30)]
    [Title("Type", Bold = true, TitleAlignment = TitleAlignments.Centered)]
    [BoxGroupWithTitleColor("Inventory Settings", 0.11f, 0.88f, 0.81f, 1f)]
    [EnumToggleButtons]
    [ReadOnly]
    private ItemType itemType = ItemType.Material;

    [SerializeField]
    [HideLabel]
    [LabelWidth(30)]
    [Title("ID", Bold = true, TitleAlignment = TitleAlignments.Centered)]
    [BoxGroupWithTitleColor("Inventory Settings", 0.11f, 0.88f, 0.81f, 1f)]
    private string MaterialID;

    [SerializeField]
    [HideLabel]
    [LabelWidth(30)]
    [Title("Header", Bold = true, TitleAlignment = TitleAlignments.Centered)]
    [BoxGroupWithTitleColor("Inventory Settings", 0.11f, 0.88f, 0.81f, 1f)]
    private string MaterialHeader;

    [SerializeField]
    [HideLabel]
    [LabelWidth(30)]
    [Title("Specials", Bold = true, TitleAlignment = TitleAlignments.Centered)]
    [BoxGroupWithTitleColor("Inventory Settings", 0.11f, 0.88f, 0.81f, 1f)]
    private string MaterialSpecials;

    [SerializeField]
    [HideLabel]
    [LabelWidth(30)]
    [Title("Details", Bold = true, TitleAlignment = TitleAlignments.Centered)]
    [BoxGroupWithTitleColor("Inventory Settings", 0.11f, 0.88f, 0.81f, 1f)]
    private string MaterialDetails;

    [SerializeField]
    [HideLabel]
    [LabelWidth(30)]
    [Title("Icon", Bold = true, TitleAlignment = TitleAlignments.Centered)]
    [BoxGroupWithTitleColor("Inventory Settings", 0.11f, 0.88f, 0.81f, 1f)]
    [PreviewField(150, ObjectFieldAlignment.Center)]
    private Sprite MaterialIcon;

    [SerializeField]
    [HideLabel]
    [LabelWidth(30)]
    [Title("Inventory Image", Bold = true, TitleAlignment = TitleAlignments.Centered)]
    [BoxGroupWithTitleColor("Inventory Settings", 0.11f, 0.88f, 0.81f, 1f)]
    [PreviewField(150, ObjectFieldAlignment.Center)]
    private Sprite MaterialInventoryIcon;

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
    private bool[,] itemGrid = new bool[5,5];
    #endregion

    #region Interface Values
    public string ItemID { get => MaterialID; set => MaterialID = value; }
    public Sprite ItemIcon { get => MaterialIcon; set => MaterialIcon = value; }
    public string HeaderText { get => MaterialHeader; set => MaterialHeader = value; }
    public string SpecialsText { get => MaterialSpecials; set => MaterialSpecials = value; }
    public string DetailsText { get => MaterialDetails; set => MaterialDetails = value; }
    public Sprite InventoryIcon { get => MaterialInventoryIcon; set => MaterialInventoryIcon = value; }
    public bool[,] ItemGrid { get => itemGrid; set=> itemGrid = value; }
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