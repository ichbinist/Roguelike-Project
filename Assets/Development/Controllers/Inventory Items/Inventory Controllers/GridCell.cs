using Lean.Gui;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GridCell : MonoBehaviour
{
    private InventoryGrid grid;
    public InventoryGrid Grid { get { return (grid == null) ? grid = GetComponentInParent<InventoryGrid>(): grid; } }

    [OnValueChanged("GridStateChanged")]
    public CellState CellState;

    public List<MaskableGraphic> Graphics = new List<MaskableGraphic>();

    public Color FilledColor;
    public Color EmptyColor;

    private Color baseCellColor;
    private Color unchangedbaseCellColor;

    public bool IsEmpty
    {
        get
        {
            return (CellState == CellState.Empty);
        }
    }

    public bool IsFullOrNone
    {
        get
        {
            return (CellState == CellState.Full || CellState == CellState.None);
        }
    }
    public bool IsFullButExist
    {
        get
        {
            return (CellState == CellState.Full && CellState != CellState.None);
        }
    }

    public bool IsExist
    {
        get
        {
            return (CellState != CellState.None);
        }
    }

    private void Awake()
    {
        if (CellState != CellState.None)
        {
            Grid.Grid.Add(this);
        }
        baseCellColor = Graphics[0].color;
        unchangedbaseCellColor = Graphics[0].color;
    }

    public void ShowStatus()
    {
        if (IsExist)
        {           
            if (IsEmpty)
            {
                Graphics[0].color = EmptyColor;
            }
            else if (IsFullButExist)
            {
                Graphics[0].color = FilledColor;
            }
        }
    }

    public void HideStatus()
    {
        if (IsExist)
        {
            if (IsEmpty)
            {
                Graphics[0].color = unchangedbaseCellColor;
            }
            else if (IsFullButExist)
            {
                baseCellColor = unchangedbaseCellColor * new Color(1, 1, 1, 0.1f);
                Graphics[0].color = baseCellColor;
            }
        }
    }

    public void FillCell()
    {
        if (IsExist)
        {
            CellState = CellState.Full;
            GridStateChanged();
        }
    }

    public void EmptyCell()
    {
        if (IsExist)
        {
            CellState = CellState.Empty;
            GridStateChanged();
        }
    }


    private void GridStateChanged()
    {
        if(CellState == CellState.Empty)
        {
            ShowCell();
        }
        else
        {
            HideCell();
        }
    }

    private void ShowCell()
    {
        foreach (MaskableGraphic g in Graphics)
        {
            g.color = new Color(g.color.r, g.color.g, g.color.b, 1f);
        }
        HideStatus();
    }

    private void HideCell()
    {
        foreach(MaskableGraphic g in Graphics)
        {
            g.color = new Color(g.color.r, g.color.g, g.color.b, 0.1f);
        }
    }
}


public enum CellState
{
    Empty,
    Full,
    None
}