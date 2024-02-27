using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MoreMountains.Feedbacks;

public class BaseInteractionController : MonoBehaviour
{
    [FoldoutGroup("Settings")]
    public float InteractionDistance = 10f;
    [FoldoutGroup("References")]
    public InteractionSelection InteractionSelectionPrefab;
    [FoldoutGroup("References")]
    public Transform InteractionHolder;
    [FoldoutGroup("References")]
    public CanvasGroup CanvasGroup;
    [FoldoutGroup("References")]
    public MMF_Player OpenInteractionsEffect, CloseInteractionsEffect;
    [FoldoutGroup("References")]
    public Transform RadialSelectionTarget;
    [FoldoutGroup("References")]
    public Canvas Canvas;
    [FoldoutGroup("Details")]
    [ReadOnly]
    public InteractionSelection HoveringSelection;
    [FoldoutGroup("Details")]
    [ReadOnly]
    public InteractionSelection SelectedSelection;
    [FoldoutGroup("Details")]
    [ReadOnly]
    public bool isOpen;

    [FoldoutGroup("Selections")]
    public List<Selection> Selections = new List<Selection>();
    [FoldoutGroup("Selections")]
    [ReadOnly]
    public List<InteractionSelection> InteractionObjects = new List<InteractionSelection>();

    private Vector2 selectionTargetPosition;

    public bool isInRange;
    void Update()
    {
        Canvas.transform.LookAt(Canvas.transform.position + Camera.main.transform.rotation * Vector3.forward,
                 Camera.main.transform.rotation * Vector3.up);

        if (CombatGameManager.Instance.PlayerCharacter.MapState != MapState.Base)
        {
            if (isOpen)
                CloseInteractions();
            return;
        }

        else if (Vector3.Distance(transform.position, CombatGameManager.Instance.PlayerCharacter.transform.position) > InteractionDistance)
        {
            if (isOpen)
                CloseInteractions();
            if (isInRange)
            {
                isInRange = false;
                OnRangeExit();
            }
            return;
        }

        if (Vector3.Distance(transform.position, CombatGameManager.Instance.PlayerCharacter.transform.position) <= InteractionDistance)
        {
            if (!isInRange)
            {
                isInRange = true;
                OnRangeEnter();
            }
        }

        AdditionalInteraction();

        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hitInfo = new RaycastHit();
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hitInfo, 90f, LayerMask.GetMask("ClickInteraction")) && hitInfo.transform.gameObject == gameObject)
            {
                OpenInteractions();
            }
            else
            {
                if(isOpen)
                    CloseInteractions();
            }
        }

        if (isOpen)
        {
            if (HoveringSelection)
            {
                foreach (InteractionSelection interactionObject in InteractionObjects)
                {
                    interactionObject.SetSize(Vector2.one * 270);
                }
                HoveringSelection.SetSize(Vector2.one * 400);
                selectionTargetPosition = CalculatePosition(Vector2.zero, HoveringSelection.RectTransform.anchoredPosition, 120);
            }
            else
            {
                foreach (InteractionSelection interactionObject in InteractionObjects)
                {
                    interactionObject.ReturnSize();
                }
                selectionTargetPosition = Vector2.zero;
            }

            RadialSelectionTarget.transform.localPosition = Vector2.Lerp(RadialSelectionTarget.transform.localPosition, selectionTargetPosition, Time.deltaTime * 5f);
        }
    }
    protected virtual void OnRangeEnter()
    {

    }

    protected virtual void OnRangeExit()
    {

    }

    protected virtual void AdditionalInteraction()
    {

    }

    public Vector2 CalculatePosition(Vector2 target1, Vector2 target2, float distanceFromTarget1)
    {
        // Calculate the direction vector from target1 to target2
        Vector2 direction = (target2 - target1).normalized;

        // Calculate the new position based on the distance from target1
        Vector2 newPosition = target1 + direction * distanceFromTarget1;

        return newPosition;
    }

    public void SelectionSelected()
    {
        CloseInteractions();
    }

    public void OpenInteractions()
    {
        OpenInteractionsEffect.PlayFeedbacks();
        isOpen = true;
        //Cursor.visible = false;
        if (InteractionObjects.Count == 0)
        {
            InitializeSelections();
        }
    }

    public void CloseInteractions()
    {
        CloseInteractionsEffect.PlayFeedbacks();
        isOpen = false;
        //Cursor.visible = true;
        SelectedSelection = null;
    }

    private void InitializeSelections()
    {
        foreach (Selection selection in Selections)
        {
            InteractionSelection interactionObject = Instantiate(InteractionSelectionPrefab, InteractionHolder);
            interactionObject.SelectionAction = selection.SelectionAction;
            interactionObject.SelectionIcon.sprite = selection.SelectionIconSprite;
            selection.InteractionSelection = interactionObject;
            InteractionObjects.Add(interactionObject);
            interactionObject.Initialize();
            interactionObject.BaseInteractionController = this;
        }
    }
}

public abstract class SelectionAction : MonoBehaviour
{
    public Action OnActionSelected, OnActionCompleted;
    public abstract void Action();
    public abstract void Action(InteractionSelection selection);
}

[System.Serializable]
public class Selection
{
    [ReadOnly]
    public InteractionSelection InteractionSelection;
    public Sprite SelectionIconSprite;
    public SelectionAction SelectionAction;
}