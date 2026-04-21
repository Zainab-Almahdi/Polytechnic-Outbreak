using UnityEngine;
using System.Collections;

public class EntranceAnimation : MonoBehaviour
{
    public float duration = 0.8f;
    public float yOffset = 20f; // Slides up from 20px below

    void OnEnable()
    {
        StartCoroutine(PlayEntrance());
    }

    IEnumerator PlayEntrance()
    {
        CanvasGroup cg = GetComponent<CanvasGroup>();
        RectTransform rt = GetComponent<RectTransform>();

        Vector2 startPos = rt.anchoredPosition - new Vector2(0, yOffset);
        Vector2 endPos = rt.anchoredPosition;

        float t = 0;
        while (t < duration)
        {
            float progress = t / duration;
            // Ease out
            float easedProgress = 1 - Mathf.Pow(1 - progress, 3);

            if (cg) cg.alpha = easedProgress;
            rt.anchoredPosition = Vector2.Lerp(startPos, endPos, easedProgress);

            t += Time.deltaTime;
            yield return null;
        }

        if (cg) cg.alpha = 1;
        rt.anchoredPosition = endPos;
    }
}