using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class MenuButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [Header("References")]
    public TMP_Text label;
    public RectTransform icon;
    public Image underline; // A thin Image stretched below the button

    [Header("Colors")]
    public Color normalColor = Color.white;
    public Color hoverColor = new Color(0.91f, 0.30f, 0.24f); // #E74C3C

    [Header("Settings")]
    public float normalSpacing = 0.12f;  // letter-spacing: 0.12em
    public float hoverSpacing = 0.18f;   // letter-spacing: 0.18em
    public float iconHoverOffset = 4f;   // translateX(4px)
    public float animDuration = 0.2f;

    private Vector2 iconOriginalPos;
    private bool isHovered = false;

    void Start()
    {
        if (icon != null)
            iconOriginalPos = icon.anchoredPosition;

        if (underline != null)
        {
            // Start with zero width
            underline.rectTransform.sizeDelta = new Vector2(0, 1);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        isHovered = true;
        StopAllCoroutines();
        StartCoroutine(AnimateHover(true));
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isHovered = false;
        StopAllCoroutines();
        StartCoroutine(AnimateHover(false));
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        // Quick scale punch on click (like :active { transform: scale(0.98) })
        StartCoroutine(ClickPunch());
    }

    IEnumerator AnimateHover(bool hovering)
    {
        float targetSpacing = hovering ? hoverSpacing : normalSpacing;
        Color targetColor = hovering ? hoverColor : normalColor;
        float iconTargetX = hovering ? iconOriginalPos.x + iconHoverOffset : iconOriginalPos.x;
        float underlineTargetWidth = hovering ? GetComponent<RectTransform>().rect.width : 0f;

        float startSpacing = label.characterSpacing;
        Color startColor = label.color;
        float startIconX = icon != null ? icon.anchoredPosition.x : 0;
        float startUnderlineWidth = underline != null ? underline.rectTransform.sizeDelta.x : 0;

        float t = 0;
        while (t < animDuration)
        {
            float p = t / animDuration;

            label.characterSpacing = Mathf.Lerp(startSpacing, targetSpacing * 100f, p);
            label.color = Color.Lerp(startColor, targetColor, p);

            if (icon != null)
            {
                Vector2 pos = icon.anchoredPosition;
                pos.x = Mathf.Lerp(startIconX, iconTargetX, p);
                icon.anchoredPosition = pos;
            }

            if (underline != null)
            {
                Vector2 size = underline.rectTransform.sizeDelta;
                size.x = Mathf.Lerp(startUnderlineWidth, underlineTargetWidth, p);
                underline.rectTransform.sizeDelta = size;
            }

            t += Time.deltaTime;
            yield return null;
        }

        // Snap to final values
        label.characterSpacing = targetSpacing * 100f;
        label.color = targetColor;
    }

    IEnumerator ClickPunch()
    {
        RectTransform rt = GetComponent<RectTransform>();
        Vector3 original = rt.localScale;
        rt.localScale = original * 0.98f;
        yield return new WaitForSeconds(0.1f);
        rt.localScale = original;
    }
}