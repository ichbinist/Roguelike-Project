using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInventoryItem
{
    string ItemID { get; set; }
    Sprite ItemIcon { get; set; }
    Sprite InventoryIcon { get; set; }
    string HeaderText { get; set; }
    string SpecialsText { get; set; }
    string DetailsText { get; set; }
    bool[,] ItemGrid { get; set; }
    bool AllGridCellsFalse { get; }
    List<ComponentInfo> ComponentData { get; set; }
    ItemType ItemType { get;}
}

public enum ItemType
{
    Weapon,
    Power,
    Material
}