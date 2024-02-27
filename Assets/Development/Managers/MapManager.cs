using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static Cinemachine.DocumentationSortingAttribute;

public class MapManager : Singleton<MapManager>, IPanel
{
    public TextMeshProUGUI FloorInfoText;

    public RectTransform MapHolder;
    public RectTransform MapMask;

    public RectTransform RoomMapIconPrefab, PlayerMapIconPrefab, DoorMapIconPrefab, BossRoomMapIconPrefab;

    public Sprite LobbyIconSprite;

    private List<RectTransform> MapIcons = new List<RectTransform>();
    private RectTransform PlayerMapIconInstance;

    public float MapMagnificationValue;
    private void CreateMap()
    {
        DungeonGenerator dungeonGenerator = RoomManager.Instance.DungeonGenerator;
        bool isFirstRoom = true;

        foreach (RoomBehaviour room in dungeonGenerator.InstantiatedRooms)
        {
            RectTransform roomIcon;

            if (room.GetComponent<RoomSettings>().RoomType == RoomType.Boss_Combat)
                roomIcon = Instantiate(BossRoomMapIconPrefab, MapHolder);
            else
                roomIcon = Instantiate(RoomMapIconPrefab, MapHolder);

            RoomMapIconController roomMapIconController = roomIcon.GetComponent<RoomMapIconController>(); 
            roomMapIconController.RoomSettings = room.GetComponent<RoomSettings>();
            roomMapIconController.RoomSettings.OnEnteredRoom.AddListener(roomMapIconController.Reveal);
            // Set the position of the room icon based on the room's position in the world
            roomIcon.anchoredPosition = new Vector2(room.transform.position.x, room.transform.position.z) * MapMagnificationValue;
            MapIcons.Add(roomIcon);

            foreach (GameObject door in room.doors)
            {
                if (door.activeSelf)
                {
                    RectTransform doorIcon = Instantiate(DoorMapIconPrefab, MapHolder);
                    // Set the position of the room icon based on the room's position in the world
                    doorIcon.anchoredPosition = new Vector2(door.transform.GetChild(0).position.x, door.transform.GetChild(0).position.z) * MapMagnificationValue;
                    doorIcon.sizeDelta *= MapMagnificationValue;
                    MapIcons.Add(doorIcon);
                    roomMapIconController.DoorImages.Add(doorIcon.GetComponent<Image>());
                }
            }


            if (isFirstRoom)
            {
                // Lobby Room
                roomIcon.GetComponent<Image>().sprite = LobbyIconSprite;
                roomMapIconController.Reveal();
                isFirstRoom = false;
            }
            else
            {
                roomMapIconController.Hide();
            }
        }

        // Instantiate PlayerMapIcon at the center of the map
        PlayerMapIconInstance = Instantiate(PlayerMapIconPrefab, MapMask);
        PlayerMapIconInstance.localPosition = Vector2.zero;
        PlayerMapIconInstance.sizeDelta *= MapMagnificationValue;
    }

    private void MoveMap()
    {
        Vector3 playerWorldPosition = CombatGameManager.Instance.PlayerCharacter.transform.position;
        Vector3 distanceFromTheCenter = -playerWorldPosition;        
        distanceFromTheCenter = new Vector3(distanceFromTheCenter.x, distanceFromTheCenter.z);
        MapHolder.anchoredPosition = new Vector3(distanceFromTheCenter.x, distanceFromTheCenter.y, 0) * MapMagnificationValue;
    }

    private void SetFloorInfo()
    {
        FloorInfoText.SetText(FloorManager.Instance.Company.CompanyName + " - " + FloorManager.Instance.GetCurrentFloor().SectionName + " - " + FloorManager.Instance.GetCurrentFloor().FloorIndexName);
    }

    public IEnumerator CreateMapCoroutine()
    {
        foreach (RectTransform icon in MapIcons)
        {
            Destroy(icon.gameObject);
        }

        if (PlayerMapIconInstance?.gameObject != null)
            Destroy(PlayerMapIconInstance.gameObject);

        MapIcons.Clear();
        yield return new WaitForSeconds(0.05f);
        CreateMap();
        SetFloorInfo();
    }

    void Update()
    {
        if (CombatGameManager.Instance.isDead)
        {
            ClosePanel();
            return;
        }

        MoveMap();
    }

    #region IPanel Region
    private CanvasGroup canvasGroup;
    public CanvasGroup CanvasGroup { get { return (canvasGroup == null) ? canvasGroup = GetComponent<CanvasGroup>() : canvasGroup; } }
    public bool IsOpen { get => CanvasGroup.alpha == 1; }

    [SerializeField]
    private PanelType panelType;
    public PanelType PanelType { get => panelType; set => panelType = value; }
    public bool IsKeyable => true;

    [SerializeField]
    [ShowIf("IsKeyable")]
    private KeyCode panelKey;
    public KeyCode PanelKey { get => panelKey; set => panelKey = value; }

    public void ClosePanel()
    {
        CanvasGroup.alpha = 0f;
        CanvasGroup.interactable = false;
        CanvasGroup.blocksRaycasts = false;
        MainCanvasManager.Instance.OnPanelClosed.Invoke(this);
    }

    public void OpenPanel()
    {
        CanvasGroup.alpha = 1f;
        CanvasGroup.interactable = true;
        CanvasGroup.blocksRaycasts = true;
        MainCanvasManager.Instance.OnPanelOpened.Invoke(this);
    }
    #endregion
}
