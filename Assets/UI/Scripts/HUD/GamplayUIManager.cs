using UnityEngine;
using UnityEngine.UI;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

public class GamplayUIManager : MonoBehaviour
{
    [Header("Screens")]
    [SerializeField] private GameObject hudScreen;
    [SerializeField] private GameObject pauseMenuScreen;
    [SerializeField] private GameObject gameOverScreen;

    [Header("Buttons")]
    [SerializeField] private Button pauseBackButton;

    [Header("Settings")]
    [SerializeField] private SettingsMenuManager settingsMenuManager;

    private bool isPaused;

    void Start()
    {
        if (pauseBackButton != null)
        {
            pauseBackButton.onClick.AddListener(HandleBack);
        }

        ShowHud();
    }

    void OnDestroy()
    {
        if (pauseBackButton != null)
        {
            pauseBackButton.onClick.RemoveListener(HandleBack);
        }
    }

    void Update()
    {
        if (gameOverScreen != null && gameOverScreen.activeSelf)
        {
            return;
        }

        if (settingsMenuManager != null && settingsMenuManager.IsOpen)
        {
            return;
        }

        if (IsEscapePressed())
        {
            HandleBack();
        }
    }

    bool IsEscapePressed()
    {
#if ENABLE_INPUT_SYSTEM
        return Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame;
#else
        return Input.GetKeyDown(KeyCode.Escape);
#endif
    }

    public void ShowHud()
    {
        if (hudScreen != null)
        {
            hudScreen.SetActive(true);
        }

        if (pauseMenuScreen != null)
        {
            pauseMenuScreen.SetActive(false);
        }

        if (gameOverScreen != null)
        {
            gameOverScreen.SetActive(false);
        }

        ResumeTime();
        SetCursorState(false);
        isPaused = false;
    }

    public void ShowPauseMenu()
    {
        if (hudScreen != null)
        {
            hudScreen.SetActive(true);
        }

        if (pauseMenuScreen != null)
        {
            pauseMenuScreen.SetActive(true);
        }

        PauseTime();
        SetCursorState(true);
        isPaused = true;
    }

    public void ShowGameOver()
    {
        if (hudScreen != null)
        {
            hudScreen.SetActive(false);
        }

        if (pauseMenuScreen != null)
        {
            pauseMenuScreen.SetActive(false);
        }

        if (gameOverScreen != null)
        {
            gameOverScreen.SetActive(true);
        }

        ResumeTime();
        SetCursorState(true);
        isPaused = false;
    }

    void HandleBack()
    {
        if (isPaused)
        {
            ShowHud();
        }
        else
        {
            ShowPauseMenu();
        }
    }

    void PauseTime()
    {
        Time.timeScale = 0f;
    }

    void ResumeTime()
    {
        Time.timeScale = 1f;
    }

    void SetCursorState(bool isVisible)
    {
        Cursor.visible = isVisible;
        Cursor.lockState = isVisible ? CursorLockMode.None : CursorLockMode.Locked;
    }
}
