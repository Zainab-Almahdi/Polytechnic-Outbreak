using UnityEngine;
using System.Collections;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

public class SettingsMenuManager : MonoBehaviour
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
        private SettingsMenuManager owner;
        private TabButtonState tab;

        public void Initialize(SettingsMenuManager owner, TabButtonState tab)
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

    [Header("Settings Menu")]
    [Tooltip("The entire settings panel (includes tabs, sub-panels, back button).")]
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private Button backButton;
    [SerializeField] private CanvasGroup settingsCanvasGroup;
    [SerializeField] private bool isMainMenu = true;
    [SerializeField] private PauseMenuManager pauseMenuManager;

    [Header("Settings Sub-Panels")]
    [Tooltip("The panel that shows Controls options (Move Forward, Shoot, etc.).")]
    [SerializeField] private GameObject controlsPanel;
    [Tooltip("The panel that shows Display options (Resolution, V-Sync, etc.).")]
    [SerializeField] private GameObject displayPanel;
    [Tooltip("The panel that shows Difficulty options (Game Difficulty, Friendly Fire, etc.).")]
    [SerializeField] private GameObject difficultyPanel;

    [Header("Settings Tab Buttons")]
    [SerializeField] private TabButtonState controlsTabButton;
    [SerializeField] private TabButtonState displayTabButton;
    [SerializeField] private TabButtonState difficultyTabButton;

    [Header("Rebind Hint")]
    [SerializeField] private Image rebindHintImage;

    [Header("Events")]
    [SerializeField] private UnityEvent onBackRequested;



    private UnityAction backOverride;

    public CanvasGroup SettingsCanvasGroup => settingsCanvasGroup;
    public bool IsOpen => settingsPanel != null && settingsPanel.activeSelf;

    public void AddBackListener(UnityAction listener)
    {
        if (listener != null)
        {
            onBackRequested.AddListener(listener);
        }
    }

    public void RemoveBackListener(UnityAction listener)
    {
        if (listener != null)
        {
            onBackRequested.RemoveListener(listener);
        }
    }

    void Start()
    {
        if (settingsCanvasGroup == null && settingsPanel != null)
        {
            settingsCanvasGroup = settingsPanel.GetComponent<CanvasGroup>();
        }

        if (backButton != null)
        {
            backButton.onClick.RemoveAllListeners();
            backButton.onClick.AddListener(RequestBack);
        }

        UISFXButtonHover.Ensure(backButton);

        if (controlsTabButton != null && controlsTabButton.button != null)
        {
            controlsTabButton.button.onClick.AddListener(ShowControlsPanel);
        }

        UISFXButtonHover.Ensure(controlsTabButton != null ? controlsTabButton.button : null);

        if (displayTabButton != null && displayTabButton.button != null)
        {
            displayTabButton.button.onClick.AddListener(ShowDisplayPanel);
        }

        UISFXButtonHover.Ensure(displayTabButton != null ? displayTabButton.button : null);

        if (difficultyTabButton != null && difficultyTabButton.button != null)
        {
            difficultyTabButton.button.onClick.AddListener(ShowDifficultyPanel);
        }

        UISFXButtonHover.Ensure(difficultyTabButton != null ? difficultyTabButton.button : null);

        SetupTabHandler(controlsTabButton);
        SetupTabHandler(displayTabButton);
        SetupTabHandler(difficultyTabButton);

        ApplyTabState(controlsTabButton, TabVisualState.Inactive);
        ApplyTabState(displayTabButton, TabVisualState.Inactive);
        ApplyTabState(difficultyTabButton, TabVisualState.Inactive);

        if (IsOpen)
        {
            InitializeView();
        }

        HideRebindHints();
    }

    void OnEnable()
    {
        HideRebindHints();
    }

    void OnDestroy()
    {
        if (backButton != null)
        {
            backButton.onClick.RemoveListener(RequestBack);
        }

        if (controlsTabButton != null && controlsTabButton.button != null)
        {
            controlsTabButton.button.onClick.RemoveListener(ShowControlsPanel);
        }

        if (displayTabButton != null && displayTabButton.button != null)
        {
            displayTabButton.button.onClick.RemoveListener(ShowDisplayPanel);
        }

        if (difficultyTabButton != null && difficultyTabButton.button != null)
        {
            difficultyTabButton.button.onClick.RemoveListener(ShowDifficultyPanel);
        }
    }

    void Update()
    {
        if (!IsOpen)
        {
            return;
        }

        if (IsEscapePressed())
        {
            RequestBack();
        }
    }

    public void InitializeView()
    {
        SetDefaultSettingsView();
    }

    public void Open()
    {
        Open(null);
    }

    public void Open(UnityAction overrideBack)
    {
        backOverride = overrideBack;

        if (settingsPanel != null)
        {
            settingsPanel.SetActive(true);
        }

        InitializeView();
    }

    public void Close()
    {
        backOverride = null;

        if (settingsPanel != null)
        {
            settingsPanel.SetActive(false);
        }

        HideRebindHints();
    }

    public void ShowControlsPanel()
    {
        if (!IsOpen)
        {
            return;
        }

        if (UISFXManager.Instance != null)
            UISFXManager.Instance.PlaySelect();

        DisableAllSettingsPanels();
        if (controlsPanel != null)
        {
            controlsPanel.SetActive(true);
        }

        SetActiveTab(controlsTabButton);
    }

    public void ShowDisplayPanel()
    {
        if (!IsOpen)
        {
            return;
        }

        if (UISFXManager.Instance != null)
            UISFXManager.Instance.PlaySelect();

        DisableAllSettingsPanels();
        if (displayPanel != null)
        {
            displayPanel.SetActive(true);
        }

        SetActiveTab(displayTabButton);
    }

    public void ShowDifficultyPanel()
    {
        if (!IsOpen)
        {
            return;
        }

        if (UISFXManager.Instance != null)
            UISFXManager.Instance.PlaySelect();

        DisableAllSettingsPanels();
        if (difficultyPanel != null)
        {
            difficultyPanel.SetActive(true);
        }

        SetActiveTab(difficultyTabButton);
    }

    void RequestBack()
    {
        if (UISFXManager.Instance != null)
            UISFXManager.Instance.PlayCancel();
        Close();

        if (!isMainMenu && pauseMenuManager != null)
        {
            pauseMenuManager.HandleSettingsBack();
            return;
        }

        UnityAction overrideHandler = backOverride;
        backOverride = null;

        if (overrideHandler != null)
        {
            overrideHandler.Invoke();
            return;
        }

        onBackRequested?.Invoke();
    }

    void SetDefaultSettingsView()
    {
        DisableAllSettingsPanels();
        if (controlsPanel != null)
        {
            controlsPanel.SetActive(true);
        }

        SetActiveTab(controlsTabButton);
    }

    void DisableAllSettingsPanels()
    {
        if (controlsPanel != null)
        {
            controlsPanel.SetActive(false);
        }

        if (displayPanel != null)
        {
            displayPanel.SetActive(false);
        }

        if (difficultyPanel != null)
        {
            difficultyPanel.SetActive(false);
        }
    }

    void SetupTabHandler(TabButtonState tab)
    {
        if (tab == null || tab.button == null)
        {
            return;
        }

        if (tab.image == null)
        {
            tab.image = tab.button.GetComponent<Image>();
        }

        TabButtonHandler handler = tab.button.GetComponent<TabButtonHandler>();
        if (handler == null)
        {
            handler = tab.button.gameObject.AddComponent<TabButtonHandler>();
        }

        handler.Initialize(this, tab);

        if (tab.image != null)
        {
            tab.originalScale = tab.image.rectTransform.localScale;
        }
    }

    enum TabVisualState
    {
        Inactive,
        Hover,
        Active
    }

    void HandleTabHover(TabButtonState tab, bool isHovering)
    {
        if (tab == null)
        {
            return;
        }

        if (isHovering && UISFXManager.Instance != null)
        {
            UISFXManager.Instance.PlayHover();
        }

        if (GetActiveTab() == tab)
        {
            return;
        }

        ApplyTabState(tab, isHovering ? TabVisualState.Hover : TabVisualState.Inactive);
    }

    void SetActiveTab(TabButtonState activeTab)
    {
        ApplyTabState(controlsTabButton, TabVisualState.Inactive);
        ApplyTabState(displayTabButton, TabVisualState.Inactive);
        ApplyTabState(difficultyTabButton, TabVisualState.Inactive);

        ApplyTabState(activeTab, TabVisualState.Active);
    }

    TabButtonState GetActiveTab()
    {
        if (controlsTabButton != null && controlsTabButton.image != null && controlsTabButton.image.sprite == controlsTabButton.activeSprite)
            return controlsTabButton;
        if (displayTabButton != null && displayTabButton.image != null && displayTabButton.image.sprite == displayTabButton.activeSprite)
            return displayTabButton;
        if (difficultyTabButton != null && difficultyTabButton.image != null && difficultyTabButton.image.sprite == difficultyTabButton.activeSprite)
            return difficultyTabButton;

        return null;
    }

    void ApplyTabState(TabButtonState tab, TabVisualState state)
    {
        if (tab == null)
        {
            return;
        }

        if (tab.image == null)
        {
            tab.image = tab.button != null ? tab.button.GetComponent<Image>() : null;
        }

        if (tab.image == null)
        {
            return;
        }

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

    bool IsEscapePressed()
    {
#if ENABLE_INPUT_SYSTEM
        return Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame;
#else
        return Input.GetKeyDown(KeyCode.Escape);
#endif
    }

    void HideRebindHints()
    {
        if (rebindHintImage == null)
        {
            return;
        }

        rebindHintImage.enabled = false;
    }
}
