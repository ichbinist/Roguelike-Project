using MoreMountains.Feedbacks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseInventoryAnimationController : MonoBehaviour
{
    private bool isOpen = false;

    private BaseInteractionController baseInteractionController;
    public BaseInteractionController BaseInteractionController { get { return(baseInteractionController == null) ? baseInteractionController = GetComponentInParent<BaseInteractionController>() : baseInteractionController; } }

    public MMF_Player OpenAnimationEffect, CloseAnimationEffect;

    private void Update()
    {
        if (isOpen)
        {
            if (!InventoryPanelController.Instance.IsOpen)
                CloseAnimation();
        }
        else
        {
            if (BaseInteractionController.isInRange)
            {
                if(InventoryPanelController.Instance.IsOpen)
                    OpenAnimation();
            }
        }
    }


    private void CloseAnimation()
    {
        isOpen = false;
        OpenAnimationEffect.StopFeedbacks();
        CloseAnimationEffect.PlayFeedbacks();
    }

    private void OpenAnimation()
    {
        isOpen = true;
        OpenAnimationEffect.PlayFeedbacks();
        CloseAnimationEffect.StopFeedbacks();
    }
}
