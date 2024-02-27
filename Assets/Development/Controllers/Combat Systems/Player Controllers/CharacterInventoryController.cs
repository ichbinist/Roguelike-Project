using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class CharacterInventoryController : MonoBehaviour
{
    [ReadOnly]
    public List<PickUpController> PickUpControllers = new List<PickUpController>();

    public void AddPickup(PickUpController pickUpController) 
    {
        if (PickUpControllers.Contains(pickUpController) == false)
        {
            PickUpControllers.Add(pickUpController);
        }
    }
    public void RemovePickup(PickUpController pickUpController) 
    {
        if (PickUpControllers.Contains(pickUpController) == true)
        {
            PickUpControllers.Remove(pickUpController);
        }
    }
}