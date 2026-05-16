using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ScreenManager : MonoBehaviour
{
    public static ScreenManager Instance;

    [Header("Screens")]
    public GameObject mainMenu;
    public GameObject hudScreen;
    public GameObject pauseMenu;
    public GameObject gameOver;
    public GameObject settingsScreen;

    private GameObject currentScreen;
    private GameObject previousScreen; // For Back button

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        // Show main menu on start, hide everything else
        ShowOnly(mainMenu);
    }

    void Update()
    {
        // ESC key logic
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (currentScreen == hudScreen) Navigate(pauseMenu);
            else if (currentScreen == pauseMenu) Navigate(hudScreen);
            else if (currentScreen == settingsScreen) GoBack();
            else if (currentScreen == gameOver) Navigate(mainMenu);
        }
    }

    public void Navigate(GameObject target, bool saveAsPrevious = false)
    {
        if (saveAsPrevious)
            previousScreen = currentScreen;

        StartCoroutine(FadeTransition(currentScreen, target));
    }

    public void GoBack()
    {
        GameObject dest = previousScreen != null ? previousScreen : mainMenu;
        Navigate(dest);
        previousScreen = null;
    }

    // Fades out old screen, fades in new one
    IEnumerator FadeTransition(GameObject from, GameObject to)
    {
        float duration = 0.4f;

        if (from != null)
        {
            CanvasGroup fromCG = from.GetComponent<CanvasGroup>();
            if (fromCG != null)
            {
                float t = 0;
                while (t < duration)
                {
                    fromCG.alpha = 1 - (t / duration);
                    t += Time.deltaTime;
                    yield return null;
                }
                fromCG.alpha = 0;
            }
            from.SetActive(false);
        }

        to.SetActive(true);
        currentScreen = to;

        CanvasGroup toCG = to.GetComponent<CanvasGroup>();
        if (toCG != null)
        {
            float t = 0;
            toCG.alpha = 0;
            while (t < duration)
            {
                toCG.alpha = t / duration;
                t += Time.deltaTime;
                yield return null;
            }
            toCG.alpha = 1;
        }
    }

    void ShowOnly(GameObject screen)
    {
        mainMenu.SetActive(false);
        hudScreen.SetActive(false);
        pauseMenu.SetActive(false);
        gameOver.SetActive(false);
        settingsScreen.SetActive(false);

        screen.SetActive(true);
        currentScreen = screen;
    }
}