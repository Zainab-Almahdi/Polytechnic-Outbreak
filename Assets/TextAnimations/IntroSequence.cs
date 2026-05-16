using System.Collections;
using TMPro;
using UnityEngine;

public class IntroSequence : MonoBehaviour
{
    public GameObject titleText;
    public GameObject narration01;
    public GameObject narration02;
    public GameObject warningText;
    public GameObject lockdownText;
    public GameObject surviveText;
    public GameObject truthText;
    public GameObject escapeText;

    void Start()
    {
        StartCoroutine(PlayIntro());
    }

    IEnumerator PlayIntro()
    {
        HideAll();

        titleText.SetActive(true);
        yield return new WaitForSeconds(3f);
        titleText.SetActive(false);

        narration01.SetActive(true);
        yield return new WaitForSeconds(4f);
        narration01.SetActive(false);

        narration02.SetActive(true);
        yield return new WaitForSeconds(4f);
        narration02.SetActive(false);

        warningText.SetActive(true);
        yield return new WaitForSeconds(2f);
        warningText.SetActive(false);

        lockdownText.SetActive(true);
        yield return new WaitForSeconds(2f);
        lockdownText.SetActive(false);

        surviveText.SetActive(true);
        yield return new WaitForSeconds(1f);
        surviveText.SetActive(false);

        truthText.SetActive(true);
        yield return new WaitForSeconds(1f);
        truthText.SetActive(false);

        escapeText.SetActive(true);
    }

    void HideAll()
    {
        titleText.SetActive(false);
        narration01.SetActive(false);
        narration02.SetActive(false);
        warningText.SetActive(false);
        lockdownText.SetActive(false);
        surviveText.SetActive(false);
        truthText.SetActive(false);
        escapeText.SetActive(false);
    }
}