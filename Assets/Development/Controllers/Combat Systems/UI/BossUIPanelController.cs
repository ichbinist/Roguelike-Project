using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BossUIPanelController : MonoBehaviour, IPanel
{
    public CharacterSettings PlayerCharacterSettings { get { return CombatGameManager.Instance.PlayerCharacter; } }

    private CharacterSettings enemyBoss;
    private RoomSettings bossRoom;

    [FoldoutGroup("Boss UI Settings")]
    public TMPro.TextMeshProUGUI BossNameText;
    [FoldoutGroup("Boss UI Settings")]
    public Slider BossHealthSlider;
    private void OnEnable()
    {
        RoomManager.Instance.OnRoomEntered.AddListener(PlayerEnteredRoom);
    }

    private void OnDisable()
    {
        RoomManager.Instance.OnRoomEntered.RemoveListener(PlayerEnteredRoom);
    }

    private void PlayerEnteredRoom(RoomSettings room)
    {
        if(room.RoomType == RoomType.Boss_Combat)
        {
            bossRoom = room;
            InitializeBossUI();
            OpenPanel();
        }
        else
        {
            bossRoom = null;
            ClosePanel();
        }
    }

    private void InitializeBossUI()
    {
        if(bossRoom != null)
        {
            enemyBoss = bossRoom.Enemies[0];
        }
        BossNameText.SetText(enemyBoss.gameObject.name);
        BossHealthSlider.minValue = 0;
        BossHealthSlider.maxValue = enemyBoss.MaximumHealth;
        BossHealthSlider.value = enemyBoss.CurrentHealth;
    }

    private void Update()
    {
        if(bossRoom != null)
        {
            if(enemyBoss != null)
            {
                BossHealthSlider.value = enemyBoss.CurrentHealth;
                if(enemyBoss.CurrentHealth <= 0)
                {
                    ClosePanel();
                }
            }
            else
            {
                BossHealthSlider.value = 0;
                ClosePanel();
            }
        }
    }

    #region IPanel Region
    private CanvasGroup canvasGroup;
    public CanvasGroup CanvasGroup { get { return (canvasGroup == null) ? canvasGroup = GetComponent<CanvasGroup>() : canvasGroup; } }
    public bool IsOpen { get => CanvasGroup.alpha == 1; }

    [SerializeField]
    private PanelType panelType;
    public PanelType PanelType { get => panelType; set => panelType = value; }

    public bool IsKeyable => false;

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
