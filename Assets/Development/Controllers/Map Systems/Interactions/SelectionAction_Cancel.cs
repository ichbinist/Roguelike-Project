using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectionAction_Cancel : SelectionAction
{
    public override void Action()
    {
        Debug.LogError("Selection olmadan action cancel çalýþmaz");
    }

    public override void Action(InteractionSelection selection)
    {
        selection.BaseInteractionController.CloseInteractions();
    }
}
