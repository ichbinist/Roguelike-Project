using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorExitController : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        CharacterSettings characterSettings = other.GetComponent<CharacterSettings>();
        if(characterSettings?.CharacterType == CharacterType.Player)
        {
            if(FloorManager.Instance.Company.FloorList.Count-1 > FloorManager.Instance.CurrentFloorIndex)
            {
                FloorManager.Instance.NextFloor();
                FloorManager.Instance.ResetFloor();
            }
            else
            {
                InventoryManager.Instance.CombatToBase();
                SceneManagement.Instance.LoadLevel("Map Scene");
                SceneManagement.Instance.UnloadLevel("Combat Scene");
            }
        }
    }
}
