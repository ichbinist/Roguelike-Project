using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CursorManager : Singleton<CursorManager>, IPanel
{
    private Image cursor;
    public Image CursorImage { get { return (cursor == null) ? cursor = GetComponent<Image>(): cursor; } }

    public Sprite CursorDefaultImage;
    public Sprite CursorPanelImage;
    public float CursorChangingSpeed = 3f;

    private Vector2 startingCursorSize;
    private Vector2 targetCursorSize;

    private RectTransform cursorTransform;
    public RectTransform CursorTransform { get { return (cursorTransform == null) ? cursorTransform = GetComponent<RectTransform>() : cursorTransform; } }

    private void OnEnable()
    {
        CursorImage.sprite = CursorDefaultImage;
        startingCursorSize = CursorTransform.sizeDelta;
        targetCursorSize = startingCursorSize;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Confined;
    }

    private void Update()
    {
        UpdateCursorPosition();

        targetCursorSize = Vector2.Lerp(targetCursorSize, startingCursorSize, Time.deltaTime * CursorChangingSpeed / 2f);
        UpdateCursorSize();
    }

    private void UpdateCursorPosition()
    {
        transform.localPosition = Input.mousePosition - new Vector3(Screen.width/2f, Screen.height/2f, 0f);
    }

    private void UpdateCursorSize()
    {
        CursorTransform.sizeDelta = Vector2.Lerp(CursorTransform.sizeDelta, targetCursorSize, Time.deltaTime * CursorChangingSpeed);
    }

    public void SetCursorTargetSize(Vector2 targetSize)
    {
        targetCursorSize = targetSize;
    }

    public void SetCursorTargetSizeRelevantToCurrent(float targetSize)
    {
        targetCursorSize = startingCursorSize * targetSize;
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
        CursorImage.sprite = CursorPanelImage;
        //CanvasGroup.alpha = 0f;
        CanvasGroup.interactable = false;
        CanvasGroup.blocksRaycasts = false;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Confined;
        MainCanvasManager.Instance.OnPanelClosed.Invoke(this);
    }

    public void OpenPanel()
    {
        CursorImage.sprite = CursorDefaultImage;
        //CanvasGroup.alpha = 1f;
        CanvasGroup.interactable = true;
        CanvasGroup.blocksRaycasts = true;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Confined;
        MainCanvasManager.Instance.OnPanelOpened.Invoke(this);
    }
    #endregion
}
