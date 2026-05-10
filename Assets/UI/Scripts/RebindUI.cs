using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class RebindUI : MonoBehaviour
{
    public InputActionReference actionReference;
    public TMP_Text bindingText;

    private InputActionRebindingExtensions.RebindingOperation rebindingOperation;

    void Start()
    {
        UpdateBindingDisplay();
    }

    public void StartRebind()
    {
        bindingText.text = "_";

        rebindingOperation = actionReference.action
            .PerformInteractiveRebinding()
            .WithControlsExcluding("<Keyboard>/home")
            .WithControlsExcluding("<Keyboard>/end")
            .WithControlsExcluding("<Keyboard>/insert")
            .WithControlsExcluding("<Keyboard>/delete")
            .WithControlsExcluding("<Keyboard>/pageUp")
            .WithControlsExcluding("<Keyboard>/pageDown")
            .OnComplete(operation =>
            {
                operation.Dispose();

                UpdateBindingDisplay();
            });

        rebindingOperation.Start();
    }

    void UpdateBindingDisplay()
    {
        bindingText.text =
            actionReference.action.GetBindingDisplayString();
    }
}