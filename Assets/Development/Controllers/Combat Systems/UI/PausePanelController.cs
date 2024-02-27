using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PausePanelController : Singleton<PausePanelController>, IPanel
{
    public Button ReturnToMainMenuButton;

    private void OnEnable()
    {
        ReturnToMainMenuButton.onClick.AddListener(ReturnToMainMenu);
    }

    private void OnDisable()
    {
        ReturnToMainMenuButton.onClick.RemoveListener(ReturnToMainMenu);
    }

    public void ReturnToMainMenu()
    {
        ResumeGame();
        SceneManagement.Instance.LoadLevel("Main Menu Scene");
        Cursor.visible = true;
        SceneManagement.Instance.UnloadLevel(SceneManager.GetActiveScene().name);
    }

    void Update()
    {
        if (CombatGameManager.Instance.isDead)
        {
            CanvasGroup.alpha = 0f;
            CanvasGroup.interactable = false;
            return;
        }
    }

    void PauseGame()
    {
        // Pause the game
        CombatGameManager.Instance.isPaused = true;
        CanvasGroup.alpha = 1f;
        CanvasGroup.interactable = true;
        CanvasGroup.blocksRaycasts = true;
        Time.timeScale = 0f;
    }

    void ResumeGame()
    {
        // Resume the game
        CombatGameManager.Instance.isPaused = false;
        CanvasGroup.alpha = 0f;
        CanvasGroup.interactable = false;
        CanvasGroup.blocksRaycasts = false;
        Time.timeScale = 1f;
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
        ResumeGame();
        MainCanvasManager.Instance.OnPanelClosed.Invoke(this);
    }

    public void OpenPanel()
    {
        PauseGame();
        MainCanvasManager.Instance.OnPanelOpened.Invoke(this);
    }
    #endregion
}
