using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

[System.Serializable]
public class ComponentInfo
{
    [TableColumnWidth(70)]
    public Component Component;

    [TableColumnWidth(50)]
    public int Value;

    public ComponentInfo(Component _component, int _value)
    {
        Component = _component;
        Value = _value;
    }

}

public enum Component
{
    Metal,
    Cloth,
    Electronics,
    Chemicals
}