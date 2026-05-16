using UnityEngine;
using TMPro;
using System.Collections;

public class LogoFlicker : MonoBehaviour
{
    public float cycleLength = 8f;    // 8s for POLY, 11s for OUT
    public float startDelay = 0f;     // 2s delay for OUT
    private TMP_Text label;

    void Start()
    {
        label = GetComponent<TMP_Text>();
        StartCoroutine(FlickerLoop());
    }

    IEnumerator FlickerLoop()
    {
        yield return new WaitForSeconds(startDelay);

        while (true)
        {
            // Stay fully visible for most of the cycle
            float waitTime = cycleLength * 0.88f;
            yield return new WaitForSeconds(waitTime);

            // Flicker sequence
            yield return SetAlpha(0.1f, 0.05f);
            yield return SetAlpha(1.0f, 0.05f);
            yield return SetAlpha(0.05f, 0.05f);
            yield return SetAlpha(1.0f, 0.1f);
            yield return SetAlpha(0.7f, 0.05f);
            yield return SetAlpha(1.0f, 0f);

            // Wait out remaining cycle time
            float remaining = cycleLength - (cycleLength * 0.88f) - 0.3f;
            if (remaining > 0) yield return new WaitForSeconds(remaining);
        }
    }

    IEnumerator SetAlpha(float alpha, float holdTime)
    {
        Color c = label.color;
        c.a = alpha;
        label.color = c;
        yield return new WaitForSeconds(holdTime);
    }
}