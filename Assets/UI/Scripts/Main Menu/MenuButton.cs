using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class MenuButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("References")]
    public TMP_Text label;
    public Image icon;
    public HorizontalLayoutGroup iconLayoutGroup;
    public Image underline; // A thin Image stretched below the button

    [Header("Colors")]
    public Color normalColor = Color.white;
    public Color hoverColor = new Color(0.91f, 0.30f, 0.24f); // #E74C3C

    [Header("Settings")]
    public float normalSpacing = 0.14f;  // letter-spacing: 0.14em
    public float hoverSpacing = 0.18f;   // letter-spacing: 0.18em
    public float iconHoverOffset = 4f;   // translateX(4px)
    public float animDuration = 0.2f;

    [Header("Cursor")]
    public Texture2D hoverCursor;
    public Vector2 hoverCursorHotSpot = Vector2.zero;

    private bool isHovered = false;
    private int originalLeftPadding;
    private float underlineOriginalWidth;
    private bool underlineHadCustomWidth;
    private Color underlineOriginalColor;

    void Start()
    {
        if (iconLayoutGroup != null)
            originalLeftPadding = iconLayoutGroup.padding.left;

        if (underline != null)
        {
            underlineOriginalWidth = underline.rectTransform.sizeDelta.x;
            underlineHadCustomWidth = true;
            underlineOriginalColor = underline.color;
            Vector2 size = underline.rectTransform.sizeDelta;
            size.x = 0f;
            underline.rectTransform.sizeDelta = size;
            underline.color = new Color(underlineOriginalColor.r, underlineOriginalColor.g, underlineOriginalColor.b, 0f);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (isHovered) return;
        isHovered = true;
        ApplyHoverCursor();
        StopAllCoroutines();
        StartCoroutine(AnimateHover(true));
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isHovered = false;
        ClearHoverCursor();
        StopAllCoroutines();
        StartCoroutine(AnimateHover(false));
    }

    void OnDisable()
    {
        if (isHovered)
            ClearHoverCursor();
        isHovered = false;
        RestoreLayoutPadding();
    }


    IEnumerator AnimateHover(bool hovering)
    {
        float targetSpacing = hovering ? hoverSpacing : normalSpacing;
        Color targetColor = hovering ? hoverColor : normalColor;
        float targetLeftPadding = hovering ? originalLeftPadding + iconHoverOffset : originalLeftPadding;
        float underlineTargetWidth = hovering && underlineHadCustomWidth ? underlineOriginalWidth : 0f;
        float underlineTargetAlpha = hovering && underline != null ? underlineOriginalColor.a : 0f;

        float startSpacing = label.characterSpacing;
        Color startColor = label.color;
        float startLeftPadding = iconLayoutGroup != null ? iconLayoutGroup.padding.left : 0f;
        Color startIconColor = icon != null ? icon.color : Color.white;
        float startUnderlineWidth = underline != null ? underline.rectTransform.sizeDelta.x : 0;
        Color startUnderlineColor = underline != null ? underline.color : Color.white;

        float t = 0;
        while (t < animDuration)
        {
            float p = t / animDuration;

            label.characterSpacing = Mathf.Lerp(startSpacing, targetSpacing * 100f, p);
            label.color = Color.Lerp(startColor, targetColor, p);

            if (iconLayoutGroup != null)
            {
                int padding = Mathf.RoundToInt(Mathf.Lerp(startLeftPadding, targetLeftPadding, p));
                iconLayoutGroup.padding.left = padding;
                LayoutRebuilder.MarkLayoutForRebuild(iconLayoutGroup.transform as RectTransform);
            }

            if (icon != null)
            {
                icon.color = Color.Lerp(startIconColor, targetColor, p);
            }

            if (underline != null)
            {
                Vector2 size = underline.rectTransform.sizeDelta;
                size.x = Mathf.Lerp(startUnderlineWidth, underlineTargetWidth, p);
                underline.rectTransform.sizeDelta = size;
                underline.color = Color.Lerp(startUnderlineColor, new Color(underlineOriginalColor.r, underlineOriginalColor.g, underlineOriginalColor.b, underlineTargetAlpha), p);
            }

            t += Time.deltaTime;
            yield return null;
        }

        // Snap to final values
        label.characterSpacing = targetSpacing * 100f;
        label.color = targetColor;

        if (underline != null)
        {
            underline.rectTransform.sizeDelta = new Vector2(underlineTargetWidth, underline.rectTransform.sizeDelta.y);
            underline.color = new Color(underlineOriginalColor.r, underlineOriginalColor.g, underlineOriginalColor.b, underlineTargetAlpha);
        }
    }

    public IEnumerator ClickPunch()
    {
        RectTransform rt = GetComponent<RectTransform>();
        Vector3 original = rt.localScale;
        rt.localScale = original * 0.98f;
        yield return new WaitForSeconds(0.1f);
        rt.localScale = original;
    }

    private void ApplyHoverCursor()
    {
        if (hoverCursor != null)
            Cursor.SetCursor(hoverCursor, hoverCursorHotSpot, CursorMode.Auto);
    }

    private void ClearHoverCursor()
    {
        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
    }

    private void RestoreLayoutPadding()
    {
        if (iconLayoutGroup != null)
            iconLayoutGroup.padding.left = originalLeftPadding;
    }
}