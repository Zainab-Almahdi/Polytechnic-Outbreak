using UnityEngine;
using UnityEngine.UI;

public class ToggleImageSwap : MonoBehaviour
{
    public Toggle toggle;
    public Image targetImage;

    public Sprite onSprite;
    public Sprite offSprite;

    void Start()
    {
        toggle.onValueChanged.AddListener(UpdateVisual);
        UpdateVisual(toggle.isOn);
    }

    void UpdateVisual(bool isOn)
    {
        targetImage.sprite = isOn ? onSprite : offSprite;
    }
}