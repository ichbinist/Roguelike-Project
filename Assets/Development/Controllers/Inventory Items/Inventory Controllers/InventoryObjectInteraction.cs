using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.UI;
using TMPro;
using System;

public class InventoryObjectInteraction : MonoBehaviour
{
    public InteractionData InteractionData;
    public Button InteractionButton;
    public TextMeshProUGUI InteractionHeaderText;

    public void Initialize(string id, string header, Action e)
    {
        InteractionButton.onClick.AddListener(e.Invoke);
        InteractionData.InteractionID = id;
        InteractionHeaderText.text = header;
        InteractionData.InteractionHeader = header;
    }
}

[System.Serializable]
public class InteractionData
{
    public string InteractionID;
    public string InteractionHeader;
}