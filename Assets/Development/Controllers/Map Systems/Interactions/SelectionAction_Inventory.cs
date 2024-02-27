using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectionAction_Inventory : SelectionAction
{
    public override void Action()
    {
        Debug.LogError("Inventory Bilgisi Ve Selection olmadan action ger�ekle�tirilemez");
    }

    public override void Action(InteractionSelection selection)
    {
        InventoryPanelController.Instance.OpenPanel();
    }
}
