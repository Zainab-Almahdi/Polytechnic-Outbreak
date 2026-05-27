using UnityEngine;
using System;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

#if UNITY_EDITOR
using UnityEditor;
#endif

/// Handles main menu and some settings UI navigation.

public class MenuNavigationManager : MonoBehaviour
{

    [Header("Fade")]
    [Tooltip("Canvas group used to fade the entire UI canvas.")]
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private float fadeOutDuration = 0.5f;

    [Header("Main Menu")]
    [Tooltip("The main menu panel that contains Play/Settings/Exit buttons.")]
    public GameObject mainMenuPanel;
    [Tooltip("Main menu background image to fade when entering settings.")]
    public Image mainMenuBackground;
    public Button playButton;
    public string PlaySceneName;
    public string CreditsSceneName;
    public Button settingsButton;
    public Button exitButton;
    public Button instructionsButton;
    public Button creditsButton;

    [Header("Exit Confirmation")]
    [Tooltip("Popup panel shown when confirming exit from the main menu.")]
    public GameObject exitConfirmationPanel;
    public Button exitConfirmButton;
    public Button exitCancelButton;

    [Header("Instructions popup")]
    [Tooltip("Popup panel shown when displaying instructions.")]
    public GameObject instructionsPanel;
    public Button instructionsCloseButton;

    [Header("Settings Menu")]
    [SerializeField] private SettingsMenuManager settingsMenuManager;


    [Header("Events")]
    public UnityEvent onPlayClicked;
    public UnityEvent onExitClicked;



    private Coroutine fadeRoutine;
    private bool isFading;
    private Coroutine settingsTransitionRoutine;

    private void Start()
    {
        if (canvasGroup == null)
            canvasGroup = GetComponentInParent<CanvasGroup>();
        if (canvasGroup != null)
            canvasGroup.alpha = 1f;

        // --- Main Menu Button Listeners ---
        if (playButton != null)
            playButton.onClick.AddListener(OnPlay);
        if (settingsButton != null)
            settingsButton.onClick.AddListener(ShowSettings);
        if (exitButton != null)
            exitButton.onClick.AddListener(ShowExitConfirmation);
        if (instructionsButton != null)
            instructionsButton.onClick.AddListener(ShowInstructions);
        if (creditsButton != null)
            creditsButton.onClick.AddListener(OnCredits);

        if (exitConfirmButton != null)
            exitConfirmButton.onClick.AddListener(ConfirmExit);
        if (exitCancelButton != null)
            exitCancelButton.onClick.AddListener(HideExitConfirmation);
        if (instructionsCloseButton != null)
            instructionsCloseButton.onClick.AddListener(HideInstructions);

        UISFXButtonHover.Ensure(exitConfirmButton);
        UISFXButtonHover.Ensure(exitCancelButton);
        UISFXButtonHover.Ensure(instructionsCloseButton);

        if (settingsMenuManager != null)
            settingsMenuManager.AddBackListener(ShowMainMenu);

        // ----- Initial UI State 
        // Main menu visible, settings hidden
        if (mainMenuPanel != null)
            mainMenuPanel.SetActive(true);
        if (settingsMenuManager != null)
            settingsMenuManager.Close();

        if (exitConfirmationPanel != null)
            exitConfirmationPanel.SetActive(false);
        if (instructionsPanel != null)
            instructionsPanel.SetActive(false);

        // Ensure only one setting sub-panel is active initially (useful when editing in the Inspector)
        if (settingsMenuManager != null)
            settingsMenuManager.InitializeView();
    }

    private void OnDestroy()
    {
        if (settingsMenuManager != null)
            settingsMenuManager.RemoveBackListener(ShowMainMenu);
    }

    private void Update()
    {
        if (!IsEscapePressed())
            return;

        if (settingsMenuManager != null && settingsMenuManager.IsOpen)
            return;

        if (mainMenuPanel != null && mainMenuPanel.activeSelf)
        {
            if (exitConfirmationPanel != null && exitConfirmationPanel.activeSelf)
                HideExitConfirmation();
            else
                ShowExitConfirmation();
        }
    }

    private bool IsEscapePressed()
    {
#if ENABLE_INPUT_SYSTEM
        return Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame;
#else
        return Input.GetKeyDown(KeyCode.Escape);
#endif
    }


    /// Called when Play button is clicked.

    private void OnPlay()
    {
        if (UISFXManager.Instance != null)
            UISFXManager.Instance.PlaySelect();
        BeginFadeOut(() => onPlayClicked?.Invoke());
        SceneManager.LoadScene(PlaySceneName);
    }

    private void OnCredits()
    {
        if (UISFXManager.Instance != null)
            UISFXManager.Instance.PlaySelect();
        BeginFadeOut(() => onPlayClicked?.Invoke());
        SceneManager.LoadScene(CreditsSceneName);
    }


    /// Called when Exit button is clicked.

    private void OnExit()
    {
        BeginFadeOut(HandleExit);
    }

    private void ShowExitConfirmation()
    {
        if (UISFXManager.Instance != null)
            UISFXManager.Instance.PlayDialogOpen();
        if (exitConfirmationPanel != null)
            exitConfirmationPanel.SetActive(true);
    }

    private void HideExitConfirmation()
    {
        if (UISFXManager.Instance != null)
            UISFXManager.Instance.PlayDialogClose();
        if (exitConfirmationPanel != null)
            exitConfirmationPanel.SetActive(false);
    }

    private void ShowInstructions()
    {
        if (UISFXManager.Instance != null)
            UISFXManager.Instance.PlayDialogOpen();
        if (instructionsPanel != null)
            instructionsPanel.SetActive(true);
    }

    private void HideInstructions()
    {
        if (UISFXManager.Instance != null)
            UISFXManager.Instance.PlayDialogClose();
        if (instructionsPanel != null)
            instructionsPanel.SetActive(false);
    }

    private void ConfirmExit()
    {
        if (UISFXManager.Instance != null)
            UISFXManager.Instance.PlayConfirm();
        HideExitConfirmation();
        BeginFadeOut(HandleExit);
    }

    private void HandleExit()
    {
        onExitClicked?.Invoke();
        Debug.Log("Exit game.");
#if UNITY_EDITOR
        // If running in the Unity Editor, stop play mode.
        EditorApplication.isPlaying = false;
#else
        // In a built game, quit the application.
        Application.Quit();
#endif
    }

    private void BeginFadeOut(Action onComplete)
    {
        if (isFading)
            return;

        if (canvasGroup == null)
        {
            onComplete?.Invoke();
            return;
        }

        if (fadeRoutine != null)
            StopCoroutine(fadeRoutine);

        fadeRoutine = StartCoroutine(FadeCanvasOut(onComplete));
    }

    private IEnumerator FadeCanvasOut(Action onComplete)
    {
        isFading = true;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;

        if (fadeOutDuration <= 0f)
        {
            canvasGroup.alpha = 0f;
            isFading = false;
            onComplete?.Invoke();
            yield break;
        }

        float startAlpha = canvasGroup.alpha;
        float elapsed = 0f;

        while (elapsed < fadeOutDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = Mathf.Clamp01(elapsed / fadeOutDuration);
            canvasGroup.alpha = Mathf.Lerp(startAlpha, 0f, t);
            yield return null;
        }

        canvasGroup.alpha = 0f;
        isFading = false;
        onComplete?.Invoke();
    }


    /// Shows the main menu and hides the settings menu.

    public void ShowMainMenu()
    {
        if (mainMenuPanel != null)
            mainMenuPanel.SetActive(true);
        if (settingsMenuManager != null)
            settingsMenuManager.Close();

        if (settingsTransitionRoutine != null)
            StopCoroutine(settingsTransitionRoutine);

        CanvasGroup settingsCanvasGroup = settingsMenuManager != null
            ? settingsMenuManager.SettingsCanvasGroup
            : null;

        if (settingsCanvasGroup != null)
        {
            settingsCanvasGroup.alpha = 0f;
            settingsCanvasGroup.interactable = false;
            settingsCanvasGroup.blocksRaycasts = false;
        }

        if (mainMenuBackground != null)
        {
            mainMenuBackground.enabled = true;
            mainMenuBackground.color = new Color(
                mainMenuBackground.color.r,
                mainMenuBackground.color.g,
                mainMenuBackground.color.b,
                1f);
        }
    }


    /// Shows the settings menu and hides the main menu.
    /// Also resets the visible sub-panel to Controls by default.

    public void ShowSettings()
    {
        if (UISFXManager.Instance != null)
            UISFXManager.Instance.PlayClick();
        if (mainMenuPanel != null)
            mainMenuPanel.SetActive(false);
        if (settingsMenuManager != null)
            settingsMenuManager.Open();

        BeginSettingsTransition();
    }

    private void BeginSettingsTransition()
    {
        if (settingsTransitionRoutine != null)
            StopCoroutine(settingsTransitionRoutine);

        settingsTransitionRoutine = StartCoroutine(FadeToSettings());
    }

    private IEnumerator FadeToSettings()
    {
        float duration = Mathf.Max(0.01f, fadeOutDuration);
        float elapsed = 0f;

        CanvasGroup settingsCanvasGroup = settingsMenuManager != null
            ? settingsMenuManager.SettingsCanvasGroup
            : null;

        if (settingsCanvasGroup != null)
        {
            settingsCanvasGroup.alpha = 0f;
            settingsCanvasGroup.interactable = false;
            settingsCanvasGroup.blocksRaycasts = false;
        }

        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = Mathf.Clamp01(elapsed / duration);

            if (mainMenuBackground != null)
                mainMenuBackground.color = new Color(
                    mainMenuBackground.color.r,
                    mainMenuBackground.color.g,
                    mainMenuBackground.color.b,
                    Mathf.Lerp(1f, 0f, t));

            if (settingsCanvasGroup != null)
                settingsCanvasGroup.alpha = Mathf.Lerp(0f, 1f, t);

            yield return null;
        }

        if (mainMenuBackground != null)
        {
            mainMenuBackground.color = new Color(
                mainMenuBackground.color.r,
                mainMenuBackground.color.g,
                mainMenuBackground.color.b,
                0f);
            mainMenuBackground.enabled = false;
        }

        if (settingsCanvasGroup != null)
        {
            settingsCanvasGroup.alpha = 1f;
            settingsCanvasGroup.interactable = true;
            settingsCanvasGroup.blocksRaycasts = true;
        }
    }
}