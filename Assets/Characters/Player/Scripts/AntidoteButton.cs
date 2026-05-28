using UnityEngine;
using Assets.UI.Scripts;
using UnityEngine.SceneManagement;
using System.Collections;

public class AntidoteButton : MonoBehaviour
{
    public ParticleSystem antidoteGas;
    public AudioSource audioSource;
    public AudioClip activateSound;

    private bool used = false;
    private bool playerInRange = false; 
    private PlayerInputHandler currentPlayerInput;

    void Update()
    {
        if (used) return;

        if (playerInRange && currentPlayerInput != null)
        {
            if (ZombieHealth.BossDead)
            {
                if (HUDManager.Instance != null)
                {
                    HUDManager.Instance.SetInteractText("Press E to activate antidote", true);
                }

                if (currentPlayerInput.InteractPressed)
                {
                    Activate();
                }
            }
            else
            {
                if (HUDManager.Instance != null)
                {
                    HUDManager.Instance.SetInteractText("Antidote machine offline. Kill the boss first.", true);
                }
            }
        }
    }

    void Activate()
    {
        used = true;

        if (HUDManager.Instance != null)
        {
            HUDManager.Instance.SetInteractText("", false);
        }

        if (antidoteGas != null)
            antidoteGas.Play();

        if (audioSource != null && activateSound != null)
            audioSource.PlayOneShot(activateSound);

        if (AntidoteManager.Instance != null)
        {
            AntidoteManager.Instance.ActivateAntidote();
        }

        StartCoroutine(FadeAndExit());
    }

    private IEnumerator FadeAndExit()
    {
        // Create a black overlay at runtime
        GameObject canvasObj = new GameObject("FadeCanvas");
        Canvas canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 999;
        
        // Ensure it persists if scene changes are slow, though we load next scene immediately after fade
        // DontDestroyOnLoad(canvasObj); 

        GameObject imageObj = new GameObject("BlackImage");
        imageObj.transform.SetParent(canvasObj.transform);
        UnityEngine.UI.Image image = imageObj.AddComponent<UnityEngine.UI.Image>();
        image.color = new Color(0, 0, 0, 0);
        
        RectTransform rect = image.GetComponent<RectTransform>();
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;

        float elapsed = 0f;
        float duration = 3f; // 3 second fade
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            image.color = new Color(0, 0, 0, Mathf.Clamp01(elapsed / duration));
            yield return null;
        }

        yield return new WaitForSeconds(1f); // Brief pause at black
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        SceneManager.LoadScene("Credits");
    }

    private void OnTriggerEnter(Collider other)
    {
        PlayerInputHandler input = other.GetComponentInParent<PlayerInputHandler>();
        if (input != null)
        {
            playerInRange = true;
            currentPlayerInput = input;
        }
        else if (other.CompareTag("Player"))
        {
            playerInRange = true;
            currentPlayerInput = other.GetComponentInParent<PlayerInputHandler>();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        PlayerInputHandler input = other.GetComponentInParent<PlayerInputHandler>();
        if (input != null && input == currentPlayerInput)
        {
            playerInRange = false;
            currentPlayerInput = null;
            if (HUDManager.Instance != null)
            {
                HUDManager.Instance.SetInteractText("", false);
            }
        }
    }
}