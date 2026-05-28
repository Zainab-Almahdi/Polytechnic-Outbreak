using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("UI Panels")]
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private GameObject winPanel;
    

    private int zombiesRemaining = 0;
    private bool gameEnded = false;
    [SerializeField] private string mainMenuSceneName = "MainMenuScene";
    [SerializeField] private float endScreenDuration = 5f;
    [SerializeField] private AudioSource gameOverAudio;
    [SerializeField] private AudioSource winAudio;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        // Find player and subscribe to their Died event
        PlayerHealth player = FindObjectOfType<PlayerHealth>();
        if (player != null)
            player.Died += OnPlayerDied;

        // Count all zombies in scene at start
        zombiesRemaining = FindObjectsOfType<ZombieHealth>().Length;

        if (gameOverPanel) gameOverPanel.SetActive(false);
        if (winPanel) winPanel.SetActive(false);
    }

    // Called by ZombieHealth when a zombie dies
    public void OnZombieKilled()
    {
        zombiesRemaining--;
        Debug.Log($"Zombies remaining: {zombiesRemaining}");
    }

    public bool AllZombiesDead() => zombiesRemaining <= 0;

    public void OnPlayerDied()
    {
        if (gameEnded) return;
        EndGame(false);
    }

    public void TriggerWin()
    {
        if (gameEnded) return;
        EndGame(true);
    }
private void EndGame(bool playerWon)
{
    gameEnded = true;
    Time.timeScale = 0f;

    if (playerWon)
    {
        if (winPanel)
        {
            StartCoroutine(FadeIn(winPanel.GetComponent<CanvasGroup>(), 1.5f));
        }

        if (winAudio)
            winAudio.Play();
    }
    else
    {
        if (gameOverPanel)
        {
            StartCoroutine(FadeIn(gameOverPanel.GetComponent<CanvasGroup>(), 1.5f));
        }

        if (gameOverAudio)
            gameOverAudio.Play();
    }

    StartCoroutine(ReturnToMenu());
}

      private IEnumerator FadeIn(CanvasGroup cg, float duration)
{
    float t = 0f;

    cg.gameObject.SetActive(true);
    cg.alpha = 0f;

    while (t < duration)
    {
        t += Time.unscaledDeltaTime;
        cg.alpha = Mathf.Lerp(0f, 1f, t / duration);
        yield return null;
    }

    cg.alpha = 1f;
    cg.interactable = true;
    cg.blocksRaycasts = true;
}

private IEnumerator ReturnToMenu()
{
    yield return new WaitForSecondsRealtime(endScreenDuration);

    Time.timeScale = 1f;
    SceneManager.LoadScene(mainMenuSceneName);
}

// Called by UI buttons
public void RestartGame()
{
    Time.timeScale = 1f;
    SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
}

public void QuitGame()
{
    Application.Quit();
}
}