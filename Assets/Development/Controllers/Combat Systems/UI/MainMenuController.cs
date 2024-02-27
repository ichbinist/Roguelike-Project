using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{
    public Button StartGameButton, SettingsButton, ExitButton;
    public Image ScreenEffectImage;

    private bool isStarting = false;
    private void OnEnable()
    {
        StartGameButton.onClick.AddListener(StartGame);
        ExitButton.onClick.AddListener(ExitGame);
    }

    private void OnDisable()
    {
        StartGameButton.onClick.RemoveListener(StartGame);
        ExitButton.onClick.RemoveListener(ExitGame);
    }

    private void StartGame()
    {
        if(!isStarting)
            StartCoroutine(ScreenEffectCoroutine());
    }

    IEnumerator ScreenEffectCoroutine()
    {
        isStarting = true;

        float fillAmount = 0;
        while (fillAmount < 1)
        {
            fillAmount += Time.deltaTime;
            ScreenEffectImage.fillAmount = fillAmount;
            yield return null;
        }

        yield return new WaitForSeconds(0.05f);

        SceneManagement.Instance.LoadLevel("Map Scene");
        SceneManagement.Instance.UnloadLevel("Main Menu Scene");
    }

    private void SettingsPanel()
    {
        //Settings Paneli daha yok. Poliþte bakarýz
    }

    private void ExitGame()
    {
        Application.Quit();
    }
}
