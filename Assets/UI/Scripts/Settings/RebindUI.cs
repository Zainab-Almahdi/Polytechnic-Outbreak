using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;

public class RebindUI : MonoBehaviour
{
    public InputActionReference actionReference;
    public TMP_Text bindingText;
    public Button rebindButton;
    public int bindingIndex;

    [SerializeField] private Image rebindHintPanel;
    [SerializeField] private float hintEnterDuration = 0.2f;
    [SerializeField] private float hintEnterOffsetY = -20f;
    [SerializeField] private float hintExitDuration = 0.2f;

    private InputActionRebindingExtensions.RebindingOperation rebindingOperation;
    private Coroutine rebindIndicatorCoroutine;
    private Coroutine hintAnimationCoroutine;
    private RectTransform hintRectTransform;
    private Vector2 hintTargetPosition;
    private bool wasActionEnabled;
    private string previousOverridePath;
    private bool isCanceling;

    void Start()
    {
        if (rebindButton != null)
        {
            rebindButton.onClick.AddListener(StartRebind);
        }

        if (rebindHintPanel != null)
        {
            rebindHintPanel.enabled = false;
            hintRectTransform = rebindHintPanel.rectTransform;
            hintTargetPosition = hintRectTransform.anchoredPosition;
        }

        UpdateBindingDisplay();
    }

    void OnDestroy()
    {
        if (rebindButton != null)
        {
            rebindButton.onClick.RemoveListener(StartRebind);
        }

        CancelRebind();
        StopRebindIndicator();
    }

    void Update()
    {
        if (rebindingOperation == null)
        {
            return;
        }

        if (IsBackspacePressed() || IsEscapePressed())
        {
            CancelRebind();
        }
    }

    public void StartRebind()
    {
        if (UISFXManager.Instance != null)
            UISFXManager.Instance.PlayClick();
        if (rebindingOperation != null)
        {
            return;
        }
        if (bindingText == null)
        {
            var parentName = transform.parent != null ? transform.parent.name : "None";
            Debug.LogWarning($"RebindUI: bindingText is not assigned on parent '{parentName}'.");
            return;
        }

        if (actionReference == null || actionReference.action == null)
        {
            var parentName = transform.parent != null ? transform.parent.name : "None";
            Debug.LogWarning($"RebindUI: actionReference is not assigned on parent '{parentName}'.");
            return;
        }

        StartRebindIndicator();
        ToggleHint(true);

        previousOverridePath = actionReference.action.bindings[bindingIndex].overridePath;
        wasActionEnabled = actionReference.action.enabled;
        if (wasActionEnabled)
        {
            actionReference.action.Disable();
        }

        rebindingOperation = actionReference.action
            .PerformInteractiveRebinding(bindingIndex)
            .WithControlsExcluding("<Keyboard>/home")
            .WithControlsExcluding("<Keyboard>/end")
            .WithControlsExcluding("<Keyboard>/insert")
            .WithControlsExcluding("<Keyboard>/delete")
            .WithControlsExcluding("<Keyboard>/pageUp")
            .WithControlsExcluding("<Keyboard>/pageDown")
            .WithCancelingThrough("<Keyboard>/escape")
            .WithCancelingThrough("<Keyboard>/backspace")
            .OnComplete(operation =>
            {
                operation.Dispose();
                rebindingOperation = null;

                StopRebindIndicator();
                ToggleHint(false);
                UpdateBindingDisplay();
                RestoreActionState();
                Debug.Log($"RebindUI: rebind complete. New binding: '{actionReference.action.GetBindingDisplayString(bindingIndex)}'.");
            })
            .OnCancel(operation =>
            {
                operation.Dispose();

                if (isCanceling)
                {
                    return;
                }

                rebindingOperation = null;
                FinalizeCancel();
            });

        rebindingOperation.Start();
    }

    void StartRebindIndicator()
    {
        if (rebindIndicatorCoroutine != null)
        {
            StopCoroutine(rebindIndicatorCoroutine);
        }

        rebindIndicatorCoroutine = StartCoroutine(AnimateRebindIndicator());
    }

    void StopRebindIndicator()
    {
        if (rebindIndicatorCoroutine != null)
        {
            StopCoroutine(rebindIndicatorCoroutine);
            rebindIndicatorCoroutine = null;
        }
    }

    void CancelRebind()
    {
        if (rebindingOperation == null || isCanceling)
        {
            return;
        }

        isCanceling = true;

        var operation = rebindingOperation;
        rebindingOperation = null;
        operation.Cancel();
        operation.Dispose();

        FinalizeCancel();
        isCanceling = false;
    }

    System.Collections.IEnumerator AnimateRebindIndicator()
    {
        const string baseText = "Rebinding, Waiting for Input";
        int dotCount = 0;

        while (true)
        {
            if (bindingText != null)
            {
                string dots = new string('.', dotCount);
                bindingText.text = $"{baseText}{dots}";
            }
            dotCount = (dotCount + 1) % 4;
            yield return new WaitForSeconds(0.6f);
        }
    }

    void RestoreActionState()
    {
        if (actionReference != null && actionReference.action != null && wasActionEnabled)
        {
            actionReference.action.Enable();
        }

        wasActionEnabled = false;
    }

    void RestorePreviousBindingOverride()
    {
        if (actionReference == null || actionReference.action == null)
        {
            return;
        }

        if (string.IsNullOrEmpty(previousOverridePath))
        {
            actionReference.action.RemoveBindingOverride(bindingIndex);
        }
        else
        {
            actionReference.action.ApplyBindingOverride(bindingIndex, previousOverridePath);
        }

        previousOverridePath = null;
    }

    void FinalizeCancel()
    {
        StopRebindIndicator();
        ToggleHint(false);
        RestorePreviousBindingOverride();
        RestoreActionState();
        UpdateBindingDisplay();
    }

    void ToggleHint(bool isVisible)
    {
        if (rebindHintPanel != null)
        {
            if (hintAnimationCoroutine != null)
            {
                StopCoroutine(hintAnimationCoroutine);
                hintAnimationCoroutine = null;
            }

            if (isVisible)
            {
                rebindHintPanel.enabled = true;
                hintRectTransform.anchoredPosition = hintTargetPosition + new Vector2(0f, hintEnterOffsetY);
                hintAnimationCoroutine = StartCoroutine(AnimateHintIn());
            }
            else
            {
                if (rebindHintPanel.enabled)
                {
                    hintAnimationCoroutine = StartCoroutine(AnimateHintOut());
                }
            }
        }
    }

    System.Collections.IEnumerator AnimateHintIn()
    {
        float duration = Mathf.Max(0.01f, hintEnterDuration);
        float elapsed = 0f;
        Vector2 start = hintRectTransform.anchoredPosition;

        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            hintRectTransform.anchoredPosition = Vector2.Lerp(start, hintTargetPosition, t);
            yield return null;
        }

        hintRectTransform.anchoredPosition = hintTargetPosition;
        hintAnimationCoroutine = null;
    }

    System.Collections.IEnumerator AnimateHintOut()
    {
        float duration = Mathf.Max(0.01f, hintExitDuration);
        float elapsed = 0f;
        Vector2 start = hintRectTransform.anchoredPosition;
        Vector2 end = hintTargetPosition + new Vector2(0f, hintEnterOffsetY);

        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            hintRectTransform.anchoredPosition = Vector2.Lerp(start, end, t);
            yield return null;
        }

        hintRectTransform.anchoredPosition = end;
        rebindHintPanel.enabled = false;
        hintAnimationCoroutine = null;
    }

    bool IsBackspacePressed()
    {
#if ENABLE_INPUT_SYSTEM
        return Keyboard.current != null && Keyboard.current.backspaceKey.wasPressedThisFrame;
#else
        return Input.GetKeyDown(KeyCode.Backspace);
#endif
    }

    bool IsEscapePressed()
    {
#if ENABLE_INPUT_SYSTEM
        return Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame;
#else
        return Input.GetKeyDown(KeyCode.Escape);
#endif
    }

    void UpdateBindingDisplay()
    {
        if (bindingText == null)
        {
            var parentName = transform.parent != null ? transform.parent.name : "None";
            Debug.LogWarning($"RebindUI: bindingText is not assigned on parent '{parentName}'.");
            return;
        }

        if (actionReference == null || actionReference.action == null)
        {
            var parentName = transform.parent != null ? transform.parent.name : "None";
            Debug.LogWarning($"RebindUI: actionReference is not assigned on parent '{parentName}'.");
            bindingText.text = string.Empty;
            return;
        }

        bindingText.text = actionReference.action.GetBindingDisplayString(bindingIndex);
    }
}