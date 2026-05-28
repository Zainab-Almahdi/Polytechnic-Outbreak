using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneChanger : MonoBehaviour
{
    [Header("Load by Scene Name")]
    [Tooltip("Type the exact name of the target scene (case‑sensitive)")]
    public string sceneName = "GameScene";

    [Header("OR Load by Build Index (overrides name if non‑negative)")]
    [Tooltip("Use build index if >= 0; otherwise uses sceneName")]
    public int sceneIndex = -1;

    [Header("UI")]
    [SerializeField] private Button button;

    void Start()
    {
        if (button != null)
        {
            button.onClick.AddListener(ChangeScene);
        }
    }

    void OnDestroy()
    {
        if (button != null)
        {
            button.onClick.RemoveListener(ChangeScene);
        }
    }

    public void ChangeScene()
    {
        if (UISFXManager.Instance != null)
            UISFXManager.Instance.PlaySelect();
        if (sceneIndex >= 0)
            SceneManager.LoadScene(sceneIndex);
        else if (!string.IsNullOrEmpty(sceneName))
            SceneManager.LoadScene(sceneName);
        else
            Debug.LogError("No valid scene specified. Set either sceneName or sceneIndex.");
    }
}