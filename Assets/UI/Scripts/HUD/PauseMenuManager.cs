using UnityEngine;
using UnityEngine.UI;

public class PauseMenuManager : MonoBehaviour
{
    [Header("Buttons")]
    [SerializeField] private Button continueButton;
    [SerializeField] private Button settingsButton;
    [SerializeField] private Button giveUpButton;
    [SerializeField] private Button exitButton;

    [Header("References")]
    [SerializeField] private GamplayUIManager gameplayUiManager;
    [SerializeField] private SettingsMenuManager settingsMenuManager;

    void Start()
    {
        if (continueButton != null)
        {
            continueButton.onClick.AddListener(HandleContinue);
        }

        if (settingsButton != null)
        {
            settingsButton.onClick.AddListener(HandleSettings);
        }

        if (giveUpButton != null)
        {
            giveUpButton.onClick.AddListener(HandleGiveUp);
        }

        if (exitButton != null)
        {
            exitButton.onClick.AddListener(HandleExit);
        }

    }

    void OnDestroy()
    {
        if (continueButton != null)
        {
            continueButton.onClick.RemoveListener(HandleContinue);
        }

        if (settingsButton != null)
        {
            settingsButton.onClick.RemoveListener(HandleSettings);
        }

        if (giveUpButton != null)
        {
            giveUpButton.onClick.RemoveListener(HandleGiveUp);
        }

        if (exitButton != null)
        {
            exitButton.onClick.RemoveListener(HandleExit);
        }

    }

    void HandleContinue()
    {
        if (gameplayUiManager != null)
        {
            gameplayUiManager.ShowHud();
        }
    }

    void HandleSettings()
    {
        if (settingsMenuManager != null)
        {
            settingsMenuManager.Open(HandleSettingsBack);
        }
    }

    public void HandleSettingsBack()
    {
        if (gameplayUiManager != null)
        {
            if (settingsMenuManager != null)
            {
                settingsMenuManager.Close();
            }

            gameplayUiManager.ShowPauseMenu();
        }
    }

    void HandleGiveUp()
    {
        if (gameplayUiManager != null)
        {
            gameplayUiManager.ShowGameOver();
        }
    }

    void HandleExit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
