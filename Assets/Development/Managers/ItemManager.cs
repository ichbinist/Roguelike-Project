using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemManager : Singleton<ItemManager>
{
    [SerializeField]
    public List<InterfaceCache> Items = new List<InterfaceCache>();

    public IInventoryItem GetInventoryItem(string ID)
    {
        return Items.Find(x=>x.GetInterfaceInstance().ItemID == ID).GetInterfaceInstance();
    }

    public IInventoryItem GetRandomInventoryItem()
    {
        int randomIndex = Random.Range(0, Items.Count);

        return Items[randomIndex].GetInterfaceInstance();
    }

    public Material GetInventoryItemAsMaterial(string ID)
    {
        return Items.Find(x => x.GetInterfaceInstance().ItemID == ID).GetInterfaceInstance() as Material;
    }
    public Weapon GetInventoryItemAsWeapon(string ID)
    {
        return Items.Find(x => x.GetInterfaceInstance().ItemID == ID).GetInterfaceInstance() as Weapon;
    }
    public Power GetInventoryItemAsPower(string ID)
    {
        return Items.Find(x => x.GetInterfaceInstance().ItemID == ID).GetInterfaceInstance() as Power;
    }
}