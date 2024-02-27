using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoomMapIconController : MonoBehaviour
{
    private RectTransform rectTransform;
    public RectTransform RectTransform { get { return (rectTransform == null) ? rectTransform = GetComponent<RectTransform>(): rectTransform; } }

    public float Margin = 1f;
    [ReadOnly]
    public RoomSettings RoomSettings;
    [ReadOnly]
    public List<Image> DoorImages = new List<Image>();
    private void Start()
    {
        BoxCollider boxCollider = RoomManager.Instance.DungeonGenerator.InstantiatedRooms[0].GetComponent<BoxCollider>();
        RectTransform.sizeDelta = new Vector2(boxCollider.size.x - Margin, boxCollider.size.z - Margin);
        RectTransform.sizeDelta *= MapManager.Instance.MapMagnificationValue;
    }

    private void OnDisable()
    {
        RoomSettings.OnEnteredRoom.RemoveListener(Reveal);
    }

    public void Hide()
    {
        GetComponent<Image>().enabled = false;
        foreach (Image image in DoorImages)
        {
            image.enabled = false;
        }
    }

    public void Reveal()
    {
        GetComponent<Image>().enabled = true;
        foreach (Image image in DoorImages)
        {
            image.enabled = true;
        }
    }
}
