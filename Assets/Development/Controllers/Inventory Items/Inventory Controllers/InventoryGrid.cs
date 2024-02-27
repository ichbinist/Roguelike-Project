using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryGrid : MonoBehaviour
{
    [ReadOnly]
    public List<GridCell> Grid = new List<GridCell>();
    public int GridWidth, GridHeight;
}
