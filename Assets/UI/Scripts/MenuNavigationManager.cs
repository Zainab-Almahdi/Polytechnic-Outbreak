using UnityEngine;
using System;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// Handles main menu and settings UI navigation.
/// Attach this script to any GameObject on the same Canvas as your menus.
/// </summary>
public class MenuNavigationManager : MonoBehaviour
{
    [System.Serializable]
    public class TabButtonState
    {
        public Button button;
        public Image image;
        public Sprite activeSprite;
        public Sprite inactiveSprite;
        public Sprite hoverSprite;

        [HideInInspector] public Coroutine animationRoutine;
        [HideInInspector] public Vector3 originalScale;
    }

    private class TabButtonHandler : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        private MenuNavigationManager owner;
        private TabButtonState tab;

        public void Initialize(MenuNavigationManager owner, TabButtonState tab)
        {
            this.owner = owner;
            this.tab = tab;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            owner.HandleTabHover(tab, true);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            owner.HandleTabHover(tab, false);
        }
    }

    [Header("Fade")]
    [Tooltip("Canvas group used to fade the entire UI canvas.")]
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private float fadeOutDuration = 0.5f;

    [Header("Main Menu")]
    [Tooltip("The main menu panel that contains Play/Settings/Exit buttons.")]
    public GameObject mainMenuPanel;
    public Button playButton;
    public Button settingsButton;
    public Button exitButton;

    [Header("Exit Confirmation")]
    [Tooltip("Popup panel shown when confirming exit from the main menu.")]
    public GameObject exitConfirmationPanel;
    public Button exitConfirmButton;
    public Button exitCancelButton;

    [Header("Settings Menu")]
    [Tooltip("The entire settings panel (includes tabs, sub-panels, back button).")]
    public GameObject settingsPanel;
    public Button backButton;      // "BACK" button inside settings

    [Header("Settings Sub-Panels")]
    [Tooltip("The panel that shows Controls options (Move Forward, Shoot, etc.).")]
    public GameObject controlsPanel;
    [Tooltip("The panel that shows Display options (Resolution, V-Sync, etc.).")]
    public GameObject displayPanel;
    [Tooltip("The panel that shows Difficulty options (Game Difficulty, Friendly Fire, etc.).")]
    public GameObject difficultyPanel;

    [Header("Settings Tab Buttons")]
    public TabButtonState controlsTabButton;
    public TabButtonState displayTabButton;
    public TabButtonState difficultyTabButton;


    [Header("Events")]
    public UnityEvent onPlayClicked;
    public UnityEvent onExitClicked;



    private Coroutine fadeRoutine;
    private bool isFading;

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

        if (exitConfirmButton != null)
            exitConfirmButton.onClick.AddListener(ConfirmExit);
        if (exitCancelButton != null)
            exitCancelButton.onClick.AddListener(HideExitConfirmation);

        // --- Settings Close/Back Listeners ---
        if (backButton != null)
            backButton.onClick.AddListener(ShowMainMenu);

        // --- Settings Tab Listeners ---
        if (controlsTabButton != null)
            controlsTabButton.button.onClick.AddListener(ShowControlsPanel);
        if (displayTabButton != null)
            displayTabButton.button.onClick.AddListener(ShowDisplayPanel);
        if (difficultyTabButton != null)
            difficultyTabButton.button.onClick.AddListener(ShowDifficultyPanel);

        // ----- Initial UI State 
        // Main menu visible, settings hidden
        if (mainMenuPanel != null)
            mainMenuPanel.SetActive(true);
        if (settingsPanel != null)
            settingsPanel.SetActive(false);

        if (exitConfirmationPanel != null)
            exitConfirmationPanel.SetActive(false);

        // Ensure only one setting sub-panel is active initially (useful when editing in the Inspector)
        SetDefaultSettingsView();
        InitializeTabStates();
    }

    private void Update()
    {
        if (!IsEscapePressed())
            return;

        if (settingsPanel != null && settingsPanel.activeSelf)
        {
            ShowMainMenu();
            return;
        }

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
        // Invoke the event so you can attach scene loading or other logic in the Inspector.
        BeginFadeOut(() => onPlayClicked?.Invoke());
    }


    /// Called when Exit button is clicked.

    private void OnExit()
    {
        BeginFadeOut(HandleExit);
    }

    private void ShowExitConfirmation()
    {
        if (exitConfirmationPanel != null)
            exitConfirmationPanel.SetActive(true);
    }

    private void HideExitConfirmation()
    {
        if (exitConfirmationPanel != null)
            exitConfirmationPanel.SetActive(false);
    }

    private void ConfirmExit()
    {
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
        if (settingsPanel != null)
            settingsPanel.SetActive(false);
    }


    /// Shows the settings menu and hides the main menu.
    /// Also resets the visible sub-panel to Controls by default.

    public void ShowSettings()
    {
        if (mainMenuPanel != null)
            mainMenuPanel.SetActive(false);
        if (settingsPanel != null)
            settingsPanel.SetActive(true);

        // When opening settings, show the Controls panel by default
        SetDefaultSettingsView();
    }


    /// Resets settings sub-panels: only Controls panel active.

    private void SetDefaultSettingsView()
    {
        DisableAllSettingsPanels();
        if (controlsPanel != null)
            controlsPanel.SetActive(true);

        SetActiveTab(controlsTabButton);
    }


    /// Deactivates all three settings sub-panels.

    private void DisableAllSettingsPanels()
    {
        if (controlsPanel != null)
            controlsPanel.SetActive(false);
        if (displayPanel != null)
            displayPanel.SetActive(false);
        if (difficultyPanel != null)
            difficultyPanel.SetActive(false);
    }


    /// Shows the Controls panel and hides Display/Difficulty panels.

    public void ShowControlsPanel()
    {
        // Only switch panels if the settings menu is actually visible
        if (settingsPanel != null && !settingsPanel.activeSelf)
            return;

        DisableAllSettingsPanels();
        if (controlsPanel != null)
            controlsPanel.SetActive(true);

        SetActiveTab(controlsTabButton);
    }


    /// Shows the Display panel and hides Controls/Difficulty panels.

    public void ShowDisplayPanel()
    {
        if (settingsPanel != null && !settingsPanel.activeSelf)
            return;

        DisableAllSettingsPanels();
        if (displayPanel != null)
            displayPanel.SetActive(true);

        SetActiveTab(displayTabButton);
    }


    /// Shows the Difficulty panel and hides Controls/Display panels.

    public void ShowDifficultyPanel()
    {
        if (settingsPanel != null && !settingsPanel.activeSelf)
            return;

        DisableAllSettingsPanels();
        if (difficultyPanel != null)
            difficultyPanel.SetActive(true);

        SetActiveTab(difficultyTabButton);
    }

    private void InitializeTabStates()
    {
        SetupTabHandler(controlsTabButton);
        SetupTabHandler(displayTabButton);
        SetupTabHandler(difficultyTabButton);

        ApplyTabState(controlsTabButton, TabVisualState.Inactive);
        ApplyTabState(displayTabButton, TabVisualState.Inactive);
        ApplyTabState(difficultyTabButton, TabVisualState.Inactive);
    }

    private void SetupTabHandler(TabButtonState tab)
    {
        if (tab == null || tab.button == null)
            return;

        if (tab.image == null)
            tab.image = tab.button.GetComponent<Image>();

        TabButtonHandler handler = tab.button.GetComponent<TabButtonHandler>();
        if (handler == null)
            handler = tab.button.gameObject.AddComponent<TabButtonHandler>();

        handler.Initialize(this, tab);

        if (tab.image != null)
            tab.originalScale = tab.image.rectTransform.localScale;
    }

    private enum TabVisualState
    {
        Inactive,
        Hover,
        Active
    }

    private void HandleTabHover(TabButtonState tab, bool isHovering)
    {
        if (tab == null)
            return;

        if (GetActiveTab() == tab)
            return;

        ApplyTabState(tab, isHovering ? TabVisualState.Hover : TabVisualState.Inactive);
    }

    private void SetActiveTab(TabButtonState activeTab)
    {
        ApplyTabState(controlsTabButton, TabVisualState.Inactive);
        ApplyTabState(displayTabButton, TabVisualState.Inactive);
        ApplyTabState(difficultyTabButton, TabVisualState.Inactive);

        ApplyTabState(activeTab, TabVisualState.Active);
    }

    private TabButtonState GetActiveTab()
    {
        if (controlsTabButton != null && controlsTabButton.image != null && controlsTabButton.image.sprite == controlsTabButton.activeSprite)
            return controlsTabButton;
        if (displayTabButton != null && displayTabButton.image != null && displayTabButton.image.sprite == displayTabButton.activeSprite)
            return displayTabButton;
        if (difficultyTabButton != null && difficultyTabButton.image != null && difficultyTabButton.image.sprite == difficultyTabButton.activeSprite)
            return difficultyTabButton;

        return null;
    }

    private void ApplyTabState(TabButtonState tab, TabVisualState state)
    {
        if (tab == null)
            return;

        if (tab.image == null)
            tab.image = tab.button != null ? tab.button.GetComponent<Image>() : null;

        if (tab.image == null)
            return;

        Sprite targetSprite = tab.inactiveSprite;
        switch (state)
        {
            case TabVisualState.Active:
                targetSprite = tab.activeSprite != null ? tab.activeSprite : tab.inactiveSprite;
                break;
            case TabVisualState.Hover:
                targetSprite = tab.hoverSprite != null ? tab.hoverSprite : tab.inactiveSprite;
                break;
        }

        tab.image.sprite = targetSprite;
    }
}