using UnityEngine;
using Sirenix.OdinInspector;
using System.Collections.Generic;

[System.Serializable]
[CreateAssetMenu(menuName = "Inventory Items/Weapon", order = 1)]
public class Weapon : InterfaceCache, IInventoryItem
{
    [FoldoutGroup("Weapon Settings")]
    public float ShootingSpeed = 1f;

    [FoldoutGroup("Weapon Settings")]
    public float Damage = 1f;

    [FoldoutGroup("Weapon Settings")]
    public float CursorAimingMultiplier = 0.75f;

    [FoldoutGroup("Weapon Settings")]
    public float AimingMinimumValue = 0f;

    [FoldoutGroup("Weapon Settings")]
    public float AimingMovementSpeed;

    [FoldoutGroup("Weapon Settings")]
    public int AmmoCapacity = 15;

    [FoldoutGroup("Weapon Settings")]
    public float ReloadTime = 1f;

    [FoldoutGroup("Weapon Settings")]
    public ParticleSystem ShootingParticles;

    #region Interface Serialized Values
    [SerializeField]
    [HideLabel]
    [LabelWidth(30)]
    [Title("Type", Bold = true, TitleAlignment = TitleAlignments.Centered)]
    [BoxGroupWithTitleColor("Inventory Settings", 0.11f, 0.88f, 0.81f, 1f)]
    [EnumToggleButtons]
    [ReadOnly]
    private ItemType itemType = ItemType.Weapon;

    [SerializeField]
    [HideLabel]
    [LabelWidth(30)]
    [Title("ID", Bold = true, TitleAlignment = TitleAlignments.Centered)]
    [BoxGroupWithTitleColor("Inventory Settings", 0.11f, 0.88f, 0.81f, 1f)]
    private string WeaponID;

    [SerializeField]
    [HideLabel]
    [LabelWidth(30)]
    [Title("Header", Bold = true, TitleAlignment = TitleAlignments.Centered)]
    [BoxGroupWithTitleColor("Inventory Settings", 0.11f, 0.88f, 0.81f, 1f)]
    private string WeaponHeader;

    [SerializeField]
    [HideLabel]
    [LabelWidth(30)]
    [Title("Specials", Bold = true, TitleAlignment = TitleAlignments.Centered)]
    [BoxGroupWithTitleColor("Inventory Settings", 0.11f, 0.88f, 0.81f, 1f)]
    private string WeaponSpecials;

    [SerializeField]
    [HideLabel]
    [LabelWidth(30)]
    [Title("Details", Bold = true, TitleAlignment = TitleAlignments.Centered)]
    [BoxGroupWithTitleColor("Inventory Settings", 0.11f, 0.88f, 0.81f, 1f)]
    private string WeaponDetails;

    [SerializeField]
    [HideLabel]
    [LabelWidth(30)]
    [Title("Icon", Bold = true, TitleAlignment = TitleAlignments.Centered)]
    [PreviewField(150, ObjectFieldAlignment.Center)]
    [BoxGroupWithTitleColor("Inventory Settings", 0.11f, 0.88f, 0.81f, 1f)]
    private Sprite WeaponSprite;

    [SerializeField]
    [HideLabel]
    [LabelWidth(30)]
    [Title("Inventory Image", Bold = true, TitleAlignment = TitleAlignments.Centered)]
    [PreviewField(150, ObjectFieldAlignment.Center)]
    [BoxGroupWithTitleColor("Inventory Settings", 0.11f, 0.88f, 0.81f, 1f)]
    private Sprite WeaponInventoryIcon;

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
    public string ItemID { get => WeaponID; set => WeaponID = value; }
    public Sprite ItemIcon { get => WeaponSprite; set => WeaponSprite = value; }
    public string HeaderText { get => WeaponHeader; set => WeaponHeader = value; }
    public string SpecialsText { get => WeaponSpecials; set => WeaponSpecials = value; }
    public string DetailsText { get => WeaponDetails; set => WeaponDetails = value; }
    public Sprite InventoryIcon { get => WeaponInventoryIcon; set => WeaponInventoryIcon = value; }
    public bool[,] ItemGrid { get => itemGrid; set => itemGrid = value; }
    public List<ComponentInfo> ComponentData { get => ComponentInfos; set => ComponentInfos = value; }
    public ItemType ItemType { get => itemType; }
    #endregion


    public bool AllGridCellsFalse => AllItemsAreFalse();
#pragma warning disable 0168
    private string errorText = "All elements in the ItemGrid array are false.";
#pragma warning restore 0168
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