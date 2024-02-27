using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DeathPanelController : Singleton<DeathPanelController>, IPanel
{
    public Button ReturnToBase;

    public void OnEnable()
    {
        ReturnToBase.onClick.AddListener(ClosePanel);
    }

    public void OnDisable()
    {
        ReturnToBase.onClick.RemoveListener(ClosePanel);
    }

    public void PauseGame()
    {
        Cursor.visible = true;
        CombatGameManager.Instance.isPaused = true;
        OpenPanel();
        CombatGameManager.Instance.isDead = true;
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
        Time.timeScale = 1f;
        Time.fixedDeltaTime = 0.02f;
        BaseReturnManager.Instance.ReturnToBase();
        MainCanvasManager.Instance.OnPanelClosed.Invoke(this);
    }

    public void OpenPanel()
    {
        CanvasGroup.alpha = 1f;
        CanvasGroup.interactable = true;
        CanvasGroup.blocksRaycasts = true;
        Time.timeScale = 0.1f;
        Time.fixedDeltaTime = Time.timeScale * 0.02f;
        MainCanvasManager.Instance.OnPanelOpened.Invoke(this);
    }
    #endregion
}
