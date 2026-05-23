using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;

public class RebindUI : MonoBehaviour
{
    public InputActionReference actionReference;
    public TMP_Text bindingText;
    public Button rebindButton;

    private InputActionRebindingExtensions.RebindingOperation rebindingOperation;
    private Coroutine rebindIndicatorCoroutine;

    void Start()
    {
        if (rebindButton != null)
        {
            rebindButton.onClick.AddListener(StartRebind);
        }

        UpdateBindingDisplay();
    }

    void OnDestroy()
    {
        if (rebindButton != null)
        {
            rebindButton.onClick.RemoveListener(StartRebind);
        }

        StopRebindIndicator();
    }

    public void StartRebind()
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
            return;
        }

        StartRebindIndicator();

        rebindingOperation = actionReference.action
            .PerformInteractiveRebinding()
            .WithControlsExcluding("<Keyboard>/home")
            .WithControlsExcluding("<Keyboard>/end")
            .WithControlsExcluding("<Keyboard>/insert")
            .WithControlsExcluding("<Keyboard>/delete")
            .WithControlsExcluding("<Keyboard>/pageUp")
            .WithControlsExcluding("<Keyboard>/pageDown")
            .WithControlsExcluding("<Keyboard>/escape")
            .OnComplete(operation =>
            {
                operation.Dispose();

                StopRebindIndicator();
                UpdateBindingDisplay();
                Debug.Log($"RebindUI: rebind complete. New binding: '{actionReference.action.GetBindingDisplayString()}'.");
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

    System.Collections.IEnumerator AnimateRebindIndicator()
    {
        var toggle = false;

        while (true)
        {
            if (bindingText != null)
            {
                bindingText.text = toggle ? "__" : "_";
            }

            toggle = !toggle;
            yield return new WaitForSeconds(0.75f);
        }
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

        bindingText.text = actionReference.action.GetBindingDisplayString();
    }
}