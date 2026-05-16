using UnityEngine;
using UnityEngine.UI;

public class ToggleImageSwap : MonoBehaviour
{
    public Toggle toggle;
    public Image targetImage;

    [Header("Player Prefs")]
    public string playerPrefsKey;

    public Sprite onSprite;
    public Sprite offSprite;

    void Start()
    {
        if (toggle == null)
        {
            Debug.LogWarning("ToggleImageSwap: toggle is not assigned.");
            return;
        }

        if (targetImage != null && toggle.graphic == targetImage)
        {
            toggle.graphic = null;
        }

        if (!string.IsNullOrWhiteSpace(playerPrefsKey) && PlayerPrefs.HasKey(playerPrefsKey))
        {
            bool isOn = PlayerPrefs.GetInt(playerPrefsKey, toggle.isOn ? 1 : 0) != 0;
            toggle.SetIsOnWithoutNotify(isOn);
        }

        toggle.onValueChanged.AddListener(UpdateVisual);
        UpdateVisual(toggle.isOn);
    }

    void UpdateVisual(bool isOn)
    {
        if (targetImage == null)
        {
            Debug.LogWarning("ToggleImageSwap: targetImage is not assigned.");
            return;
        }

        Sprite nextSprite = isOn ? onSprite : offSprite;

        if (nextSprite == null)
        {
            Debug.LogWarning("ToggleImageSwap: missing sprite for current toggle state.");
            return;
        }

        targetImage.sprite = nextSprite;
        targetImage.enabled = true;

        if (!string.IsNullOrWhiteSpace(playerPrefsKey))
        {
            PlayerPrefs.SetInt(playerPrefsKey, isOn ? 1 : 0);
            PlayerPrefs.Save();
        }
    }
}