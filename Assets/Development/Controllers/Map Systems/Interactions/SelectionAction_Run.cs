using System.Collections;
using System.Collections.Generic;
using UnityEngine;




public class SelectionAction_Run : SelectionAction
{
    public override void Action()
    {
        SceneManagement.Instance.LoadLevel("Combat Scene");
        SceneManagement.Instance.UnloadLevel("Map Scene");
    }

    public override void Action(InteractionSelection selection)
    {
        selection.BaseInteractionController.CloseInteractions();
        InventoryManager.Instance.BaseToCombat();
        SceneManagement.Instance.LoadLevel("Combat Scene", ()=> FloorManager.Instance.InitializeFloors(1, 1));    
        SceneManagement.Instance.UnloadLevel("Map Scene");        
    }
}
