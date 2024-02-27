using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public abstract class InterfaceCache : SerializedScriptableObject
{
    public abstract IInventoryItem GetInterfaceInstance();
}
