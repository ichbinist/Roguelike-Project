using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadingScreenController : MonoBehaviour
{
    private CanvasGroup _canvasGroup;
    public CanvasGroup CanvasGroup { get { return (_canvasGroup == null) ? _canvasGroup = GetComponent<CanvasGroup>() : _canvasGroup; } }

    public Image LoadingBarImage;

    private float loadingFillAmount = 0f;
    private bool loadingScreenActivated = true;
    public AudioSource AmbianceAudioSource;

    private void OnEnable()
    {
        CombatGameManager.Instance.OnGameReset.AddListener(() => { loadingFillAmount = 0; loadingScreenActivated = true; });
    }

    private void OnDisable()
    {
        CombatGameManager.Instance.OnGameReset.RemoveListener(() => { loadingFillAmount = 0; loadingScreenActivated = true; });
    }

    private void Update()
    {
        if (loadingScreenActivated)
        {
            if (loadingFillAmount == 1)
            {
                LoadingBarImage.fillAmount = 1f;
                CanvasGroup.alpha = 0f;
                CanvasGroup.interactable = false;
                CanvasGroup.blocksRaycasts = false;
                CombatGameManager.Instance.isPaused = false;
                loadingScreenActivated = false;
                AmbianceAudioSource?.Play();
            }
            else
            {
                loadingFillAmount = Mathf.Clamp01(loadingFillAmount + Time.deltaTime / 2.25f);
                LoadingBarImage.fillAmount = loadingFillAmount;
                CanvasGroup.alpha = 1f;
                CanvasGroup.interactable = true;
                CanvasGroup.blocksRaycasts = true;
                CombatGameManager.Instance.isPaused = true;
                AmbianceAudioSource?.Stop();
            }
        }
        else
        {
            CanvasGroup.alpha = 0f;
            CanvasGroup.interactable = false;
            CanvasGroup.blocksRaycasts = false;
        }

    }
}
