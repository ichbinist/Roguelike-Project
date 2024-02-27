using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPanel
{
    PanelType PanelType { get; set; }
    bool IsOpen { get; }
    bool IsKeyable { get; }
    void OpenPanel();
    void ClosePanel();
    KeyCode PanelKey { get; set; }
}

public enum PanelType
{
    Locked, 
    Standart,
    Override,
    Elastic
}