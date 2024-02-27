using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InteractionSelection : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [FoldoutGroup("Selection Action")]
    public SelectionAction SelectionAction;
    [FoldoutGroup("References")]
    public Image SelectionIcon;
    [FoldoutGroup("References")]
    public Button Button;
    [FoldoutGroup("References")]
    public RectTransform RectTransform;

    [ReadOnly]
    public BaseInteractionController BaseInteractionController;

    private Vector2 startingSelectionRectSize;
    private Vector2 targetSize;

    private void Awake()
    {
        startingSelectionRectSize = RectTransform.sizeDelta;
        targetSize = startingSelectionRectSize;
    }

    private void Update()
    {
        RectTransform.sizeDelta = Vector2.Lerp(RectTransform.sizeDelta, targetSize, Time.deltaTime * 4f);
    }

    public void Initialize()
    {
        Button.onClick.AddListener(Selection);
    }

    private void OnDisable()
    {
        Button.onClick.RemoveListener(Selection);
    }

    public void SetSize(Vector2 size)
    {
        targetSize = size;
    }

    public void ReturnSize()
    {
        targetSize = startingSelectionRectSize;
    }

    private void Selection()
    {
        SelectionAction.Action(this);
        BaseInteractionController.SelectedSelection = this;
        BaseInteractionController.SelectionSelected();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        BaseInteractionController.HoveringSelection = this;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        BaseInteractionController.HoveringSelection = null;
    }
}
