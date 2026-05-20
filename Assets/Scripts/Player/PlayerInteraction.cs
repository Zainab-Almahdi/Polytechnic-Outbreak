using UnityEngine;
using Assets.UI.Scripts;


// Handles interaction prompts when the player enters/exits trigger volumes.
public class PlayerInteraction : MonoBehaviour
{
    private InteractionPrompt currentPrompt;

    private void OnTriggerEnter(Collider other)
    {
        var prompt = other.GetComponent<InteractionPrompt>();
        if (prompt == null)
        {
            return;
        }

        currentPrompt = prompt;
        if (HUDManager.Instance != null)
        {
            HUDManager.Instance.SetInteractText(currentPrompt.GetPromptText(), true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (currentPrompt == null || other.GetComponent<InteractionPrompt>() != currentPrompt)
        {
            return;
        }

        currentPrompt = null;
        if (HUDManager.Instance != null)
        {
            HUDManager.Instance.SetInteractText(string.Empty, false);
        }
    }
}
