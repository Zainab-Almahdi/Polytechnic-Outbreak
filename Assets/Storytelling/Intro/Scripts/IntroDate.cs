using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class IntroDate : MonoBehaviour
{
    public GameObject crackedOverlay;
    public TextMeshProUGUI dateText;
    public TextMeshProUGUI locationText;
    public float waitForLogo = 8f;

    private string locationString = "Polytechnic, Bahrain";

    void Start()
    {
        crackedOverlay.SetActive(false);
        dateText.gameObject.SetActive(false);
        locationText.gameObject.SetActive(false);
        StartCoroutine(PlaySequence());
    }

    IEnumerator PlaySequence()
    {
        // Wait for logo to fully finish
        yield return new WaitForSeconds(waitForLogo);

        // Cracked screen fades in
        crackedOverlay.SetActive(true);
        CanvasGroup crackedGroup = crackedOverlay.GetComponent<CanvasGroup>();
        crackedGroup.alpha = 0f;
        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / 2f;
            crackedGroup.alpha = t;
            yield return null;
        }

        // Date fades in
        yield return new WaitForSeconds(1f);
        dateText.gameObject.SetActive(true);

        // Wait then start typing location
        yield return new WaitForSeconds(1.5f);
        locationText.gameObject.SetActive(true);
        StartCoroutine(TypeLocation());
    }

    IEnumerator TypeLocation()
    {
        locationText.text = "";
        foreach (char c in locationString)
        {
            locationText.text += c;
            yield return new WaitForSeconds(0.08f);
        }

        // Blinking cursor after typing
        StartCoroutine(BlinkCursor());
    }

    IEnumerator BlinkCursor()
    {
        bool show = true;
        while (true)
        {
            if (show)
                locationText.text = locationString + "|";
            else
                locationText.text = locationString;

            show = !show;
            yield return new WaitForSeconds(0.5f);
        }
    }
}