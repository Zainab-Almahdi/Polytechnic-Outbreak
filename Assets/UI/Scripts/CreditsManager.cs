using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

public class CreditsManager : MonoBehaviour
{
    [Header("Fade")]
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private float fadeInDuration = 0.5f;
    [SerializeField] private float fadeOutDuration = 0.5f;

    [Header("Exit")]
    [SerializeField] private Button backButton;

    private Coroutine fadeRoutine;
    private bool isFading;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        if (canvasGroup == null)
            canvasGroup = GetComponentInParent<CanvasGroup>();

        if (canvasGroup != null)
        {
            canvasGroup.alpha = 0f;
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
        }

        if (backButton != null)
            backButton.onClick.AddListener(RequestExit);

        BeginFadeIn();
    }

    private void OnDestroy()
    {
        if (backButton != null)
            backButton.onClick.RemoveListener(RequestExit);
    }

    private void Update()
    {
        if (IsEscapePressed())
            RequestExit();
    }

    private bool IsEscapePressed()
    {
#if ENABLE_INPUT_SYSTEM
        return Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame;
#else
        return Input.GetKeyDown(KeyCode.Escape);
#endif
    }

    private void BeginFadeIn()
    {
        if (isFading)
            return;

        if (canvasGroup == null)
            return;

        if (fadeRoutine != null)
            StopCoroutine(fadeRoutine);

        fadeRoutine = StartCoroutine(FadeCanvas(0f, 1f, fadeInDuration, () =>
        {
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;
        }));
    }

    private void RequestExit()
    {
        if (isFading)
            return;

        if (canvasGroup == null)
        {
            SceneManager.LoadScene("MainMenuScene");
            return;
        }

        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;

        if (fadeRoutine != null)
            StopCoroutine(fadeRoutine);

        fadeRoutine = StartCoroutine(FadeCanvas(canvasGroup.alpha, 0f, fadeOutDuration, () =>
        {
            SceneManager.LoadScene("MainMenuScene");
        }));
    }

    private IEnumerator FadeCanvas(float from, float to, float duration, Action onComplete)
    {
        isFading = true;

        if (duration <= 0f)
        {
            canvasGroup.alpha = to;
            isFading = false;
            onComplete?.Invoke();
            yield break;
        }

        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            canvasGroup.alpha = Mathf.Lerp(from, to, t);
            yield return null;
        }

        canvasGroup.alpha = to;
        isFading = false;
        onComplete?.Invoke();
    }
}
