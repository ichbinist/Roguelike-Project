using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InteractionUIController : MonoBehaviour
{
    public Image InteractionFillImage;

    private CanvasGroup _canvasGroup;
    public CanvasGroup CanvasGroup { get { return (_canvasGroup == null) ? _canvasGroup = GetComponent<CanvasGroup>() : _canvasGroup; } }

    public void ShowInteractionUI()
    {
        CanvasGroup.interactable = true;
        CanvasGroup.alpha = 1f;
    }

    public void HideInteractionUI()
    {
        CanvasGroup.interactable = false;
        CanvasGroup.alpha = 0f;
    }

    public void UpdateInteractionUI(float fillAmount)
    {
        InteractionFillImage.fillAmount = fillAmount;
    }
    private void Update()
    {
        transform.LookAt(transform.position + Camera.main.transform.rotation * Vector3.forward,
                         Camera.main.transform.rotation * Vector3.up);
    }
}
