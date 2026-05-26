using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;

public class CinematicIntro : MonoBehaviour
{
    [Header("UI References")]
    public RawImage storyImage;
    public RawImage nextImage;
    public TextMeshProUGUI narrationText;
    public Image blackOverlay;
    public Button skipButton;

    [Header("Content")]
    public Texture[] images;
    public string[] narrations;

    [Header("Timing")]
    public float typingSpeed = 0.025f;
    public float holdTime = 3.5f;
    public float transitionDuration = 1.5f;

    private bool skipped = false;

    void Start()
    {
        blackOverlay.color = new Color(0, 0, 0, 1);
        narrationText.alpha = 0;
        nextImage.color = new Color(1, 1, 1, 0);
        skipButton.onClick.AddListener(SkipIntro);
        StartCoroutine(PlayIntro());
    }

    void SkipIntro()
    {
        skipped = true;
        StopAllCoroutines();
        SceneManager.LoadScene("MainMenu");
    }

    IEnumerator PlayIntro()
    {
        storyImage.texture = images[0];
        storyImage.transform.localScale = new Vector3(1.15f, 1.15f, 1f);
        storyImage.transform.localPosition = Vector3.zero;
        yield return StartCoroutine(FadeOverlay(0f));

        for (int i = 0; i < images.Length; i++)
        {
            // Reset
            storyImage.transform.localScale = new Vector3(1.15f, 1.15f, 1f);
            storyImage.transform.localPosition = Vector3.zero;

            // Start animation
            Coroutine anim = StartCoroutine(AnimateScene(i));

            // Type narration
            yield return StartCoroutine(TypeText(narrations[i]));

            // Hold
            yield return new WaitForSeconds(holdTime);

            // Fade text out
            yield return StartCoroutine(FadeText(0f));
            narrationText.text = "";

            // Stop animation
            StopCoroutine(anim);

            // Smooth crossfade to next scene
            if (i < images.Length - 1)
            {
                nextImage.texture = images[i + 1];
                nextImage.transform.localScale = new Vector3(1.15f, 1.15f, 1f);
                nextImage.transform.localPosition = Vector3.zero;
                yield return StartCoroutine(CrossFade());
                storyImage.texture = images[i + 1];
                storyImage.transform.localScale = new Vector3(1.15f, 1.15f, 1f);
                storyImage.transform.localPosition = Vector3.zero;
                nextImage.color = new Color(1, 1, 1, 0);
            }
        }

        yield return StartCoroutine(FadeOverlay(1f));
        yield return new WaitForSeconds(1.5f);
        SceneManager.LoadScene("MainMenu");
    }

    IEnumerator CrossFade()
    {
        float time = 0;
        nextImage.color = new Color(1, 1, 1, 0);

        while (time < transitionDuration)
        {
            if (skipped) yield break;
            time += Time.deltaTime;
            float alpha = Mathf.SmoothStep(0f, 1f, time / transitionDuration);
            nextImage.color = new Color(1, 1, 1, alpha);
            yield return null;
        }

        nextImage.color = new Color(1, 1, 1, 1);
    }

    IEnumerator AnimateScene(int index)
    {
        float time = 0;
        float duration = holdTime + 4f;

        switch (index)
        {
            case 0: // Researcher in lab — slow gentle zoom in
                while (true)
                {
                    if (skipped) yield break;
                    time += Time.deltaTime;
                    float t = Mathf.Clamp01(time / duration);
                    float scale = Mathf.Lerp(1.15f, 1.25f, t);
                    storyImage.transform.localScale = new Vector3(scale, scale, 1f);
                    yield return null;
                }

            case 1: // Documents — slow pan left to right
                while (true)
                {
                    if (skipped) yield break;
                    time += Time.deltaTime;
                    float t = Mathf.Clamp01(time / duration);
                    float x = Mathf.Lerp(-20f, 20f, t);
                    storyImage.transform.localPosition = new Vector3(x, 0, 0);
                    yield return null;
                }

            case 2: // Mice in cage — slow zoom in
                while (true)
                {
                    if (skipped) yield break;
                    time += Time.deltaTime;
                    float t = Mathf.Clamp01(time / duration);
                    float scale = Mathf.Lerp(1.15f, 1.25f, t);
                    storyImage.transform.localScale = new Vector3(scale, scale, 1f);
                    yield return null;
                }

            case 3: // Mutated mouse — very subtle continuous shake
                while (true)
                {
                    if (skipped) yield break;
                    float shakeX = Mathf.PerlinNoise(Time.time * 0.8f, 0f) * 6f - 3f;
                    float shakeY = Mathf.PerlinNoise(0f, Time.time * 0.8f) * 6f - 3f;
                    storyImage.transform.localPosition = new Vector3(shakeX, shakeY, 0);
                    storyImage.transform.localScale = new Vector3(1.15f, 1.15f, 1f);
                    yield return null;
                }

            case 4: // Notice board — slow pan down to up
                while (true)
                {
                    if (skipped) yield break;
                    time += Time.deltaTime;
                    float t = Mathf.Clamp01(time / duration);
                    float y = Mathf.Lerp(-20f, 20f, t);
                    storyImage.transform.localPosition = new Vector3(0, y, 0);
                    yield return null;
                }

            case 5: // Student in chair — slow zoom in
                while (true)
                {
                    if (skipped) yield break;
                    time += Time.deltaTime;
                    float t = Mathf.Clamp01(time / duration);
                    float scale = Mathf.Lerp(1.15f, 1.28f, t);
                    storyImage.transform.localScale = new Vector3(scale, scale, 1f);
                    yield return null;
                }

            case 6: // Destroyed lab — slow pan right to left
                while (true)
                {
                    if (skipped) yield break;
                    time += Time.deltaTime;
                    float t = Mathf.Clamp01(time / duration);
                    float x = Mathf.Lerp(20f, -20f, t);
                    storyImage.transform.localPosition = new Vector3(x, 0, 0);
                    yield return null;
                }

            case 7: // Researcher face — slow creepy zoom in
                while (true)
                {
                    if (skipped) yield break;
                    time += Time.deltaTime;
                    float t = Mathf.Clamp01(time / duration);
                    float scale = Mathf.Lerp(1.15f, 1.3f, t);
                    storyImage.transform.localScale = new Vector3(scale, scale, 1f);
                    yield return null;
                }

            case 8: // Syringe — slow zoom with very subtle perlin shake
                while (true)
                {
                    if (skipped) yield break;
                    time += Time.deltaTime;
                    float t = Mathf.Clamp01(time / duration);
                    float scale = Mathf.Lerp(1.15f, 1.25f, t);
                    float shakeX = Mathf.PerlinNoise(Time.time * 0.5f, 0f) * 2f - 1f;
                    float shakeY = Mathf.PerlinNoise(0f, Time.time * 0.5f) * 2f - 1f;
                    storyImage.transform.localScale = new Vector3(scale, scale, 1f);
                    storyImage.transform.localPosition = new Vector3(shakeX, shakeY, 0);
                    yield return null;
                }

            case 9: // Dark corridor — slow ominous zoom in
                while (true)
                {
                    if (skipped) yield break;
                    time += Time.deltaTime;
                    float t = Mathf.Clamp01(time / duration);
                    float scale = Mathf.Lerp(1.15f, 1.35f, t);
                    storyImage.transform.localScale = new Vector3(scale, scale, 1f);
                    yield return null;
                }
        }
    }

    IEnumerator FadeOverlay(float target)
    {
        float start = blackOverlay.color.a;
        float time = 0;
        while (time < 1f)
        {
            time += Time.deltaTime * 1.2f;
            float alpha = Mathf.SmoothStep(start, target, time);
            blackOverlay.color = new Color(0, 0, 0, alpha);
            yield return null;
        }
        blackOverlay.color = new Color(0, 0, 0, target);
    }

    IEnumerator FadeText(float target)
    {
        float start = narrationText.alpha;
        float time = 0;
        while (time < 1f)
        {
            if (skipped) yield break;
            time += Time.deltaTime * 1.5f;
            narrationText.alpha = Mathf.SmoothStep(start, target, time);
            yield return null;
        }
        narrationText.alpha = target;
    }

    IEnumerator TypeText(string text)
    {
        narrationText.alpha = 1;
        narrationText.text = "";
        foreach (char letter in text)
        {
            if (skipped) yield break;
            narrationText.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }
    }
}