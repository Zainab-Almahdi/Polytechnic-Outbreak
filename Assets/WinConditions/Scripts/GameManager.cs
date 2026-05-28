using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("UI Panels")]
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private GameObject winPanel;

    [Header("Stat Labels")]
    [SerializeField] private TMP_Text killsValueLabel;
    [SerializeField] private TMP_Text downsValueLabel;
    [SerializeField] private TMP_Text deathsValueLabel;
    [SerializeField] private TMP_Text maxFloorValueLabel;

    private int zombiesRemaining = 0;
    private bool gameEnded = false;

    // Stats
    private int totalKills = 0;
    private int totalDowns = 0;
    private int totalDeaths = 0;
    private int maxFloorReached = 1;

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
        PlayerHealth player = Object.FindFirstObjectByType<PlayerHealth>();
        if (player != null)
            player.Died += OnPlayerDied;

        // Count all zombies in scene at start
        zombiesRemaining = Object.FindObjectsByType<ZombieHealth>(FindObjectsSortMode.None).Length;

        if (gameOverPanel) gameOverPanel.SetActive(false);
        if (winPanel) winPanel.SetActive(false);
    }

    public void UpdateMaxFloor(int floorIndex)
    {
        int floor = floorIndex + 1;
        if (floor > maxFloorReached)
        {
            maxFloorReached = floor;
        }
    }

    // Called by ZombieHealth when a zombie dies
    public void OnZombieKilled()
    {
        zombiesRemaining--;
        totalKills++;
        Debug.Log($"Zombies remaining: {zombiesRemaining} | Total Kills: {totalKills}");
    }

    public void RecordDown()
    {
        totalDowns++;
        if (downsValueLabel != null) downsValueLabel.text = totalDowns.ToString();
    }

    public bool AllZombiesDead() => zombiesRemaining <= 0;

    public void OnPlayerDied()
    {
        if (gameEnded) return;
        totalDowns++;
        totalDeaths++;
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

    // Update UI Stats
    if (!playerWon && gameOverPanel != null)
    {
        if (killsValueLabel) killsValueLabel.text = totalKills.ToString();
        if (downsValueLabel) downsValueLabel.text = totalDowns.ToString();
        if (deathsValueLabel) deathsValueLabel.text = totalDeaths.ToString();
        if (maxFloorValueLabel) maxFloorValueLabel.text = maxFloorReached.ToString();
    }

    if (playerWon)
{
        if (winPanel)
        {
            StartCoroutine(FadeInSequence(winPanel, 1.5f));
        }

        if (winAudio)
            winAudio.Play();
    }
    else
    {
        if (gameOverPanel)
        {
            StartCoroutine(FadeInSequence(gameOverPanel, 1.5f));
        }

        if (gameOverAudio)
            gameOverAudio.Play();
    }

    StartCoroutine(ReturnToMenu());
}

private IEnumerator FadeInSequence(GameObject panel, float duration)
{
    CanvasGroup panelCG = panel.GetComponent<CanvasGroup>();
    if (panelCG == null) panelCG = panel.AddComponent<CanvasGroup>();

    panel.SetActive(true);
    panelCG.alpha = 0f;

    // Find Background and Content (Main Vertical Container)
    Transform background = panel.transform.Find("Background");
    Transform content = panel.transform.Find("Main Vertical Container");

    CanvasGroup contentCG = null;
    if (content != null)
    {
        contentCG = content.GetComponent<CanvasGroup>();
        if (contentCG == null) contentCG = content.gameObject.AddComponent<CanvasGroup>();
        contentCG.alpha = 0f;
    }

    // Fade in the background/panel overall first (which is black)
    float t = 0f;
    while (t < duration)
    {
        t += Time.unscaledDeltaTime;
        panelCG.alpha = Mathf.Lerp(0f, 1f, t / duration);
        yield return null;
    }
    panelCG.alpha = 1f;

    // Then fade in the content
    if (contentCG != null)
    {
        t = 0f;
        while (t < duration * 0.5f)
        {
            t += Time.unscaledDeltaTime;
            contentCG.alpha = Mathf.Lerp(0f, 1f, t / (duration * 0.5f));
            yield return null;
        }
        contentCG.alpha = 1f;
    }

    panelCG.interactable = true;
    panelCG.blocksRaycasts = true;
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