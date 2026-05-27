using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using System.Collections;

public class UIRowHoverEffect : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Components")]
    public UnityEngine.UI.Image iconImage;
    public TextMeshProUGUI textLabel;
    public HorizontalLayoutGroup layoutGroup;

    [Header("Settings")]
    public Color hoverColor = new Color(0.702f, 0.149f, 0.176f, 0.75f); // #B3262D with 0.75 alpha
    public float hoverLetterSpacing = 25f; 
    public float hoverPaddingOffset = 30f; 
    public float animationDuration = 0.2f;

    [Header("Cursor (Optional)")]
    public Texture2D hoverCursor;
    public Vector2 hotSpot = Vector2.zero;

    private Color initialIconColor;
    private Color initialTextColor;
    private float initialLetterSpacing;
    private int initialPaddingLeft;

    private float currentLerp = 0f;
    private Coroutine animationCoroutine;

    private void Awake()
    {
        CacheInitialState();
    }

    private void CacheInitialState()
    {
        // Find components if not assigned
        if (iconImage == null) iconImage = transform.Find("Option Icon")?.GetComponent<UnityEngine.UI.Image>();
        if (textLabel == null) textLabel = transform.Find("Option Text Label")?.GetComponent<TextMeshProUGUI>();
        if (layoutGroup == null) layoutGroup = GetComponent<HorizontalLayoutGroup>();

        // Cache initial values
        if (iconImage != null) initialIconColor = iconImage.color;
        if (textLabel != null)
        {
            initialTextColor = textLabel.color;
            initialLetterSpacing = textLabel.characterSpacing;
        }
        if (layoutGroup != null) initialPaddingLeft = layoutGroup.padding.left;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (UISFXManager.Instance != null) UISFXManager.Instance.PlayHover();
        if (hoverCursor != null) Cursor.SetCursor(hoverCursor, hotSpot, CursorMode.Auto);

        StopAllCoroutines();
        animationCoroutine = StartCoroutine(TransitionCoroutine(1f));
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (hoverCursor != null) Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);

        StopAllCoroutines();
        animationCoroutine = StartCoroutine(TransitionCoroutine(0f));
    }

    private IEnumerator TransitionCoroutine(float target)
    {
        float startLerp = currentLerp;
        float elapsed = 0f;

        while (elapsed < animationDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            currentLerp = Mathf.Lerp(startLerp, target, elapsed / animationDuration);
            ApplyEffects(currentLerp);
            yield return null;
        }

        currentLerp = target;
        ApplyEffects(currentLerp);
    }

    private void ApplyEffects(float lerp)
    {
        if (iconImage != null)
            iconImage.color = Color.Lerp(initialIconColor, hoverColor, lerp);

        if (textLabel != null)
        {
            textLabel.color = Color.Lerp(initialTextColor, hoverColor, lerp);
            // Ensure we are increasing letter spacing
            textLabel.characterSpacing = Mathf.Lerp(initialLetterSpacing, initialLetterSpacing + hoverLetterSpacing, lerp);
        }

        if (layoutGroup != null)
        {
            int newLeft = (int)Mathf.Lerp(initialPaddingLeft, initialPaddingLeft + hoverPaddingOffset, lerp);
            layoutGroup.padding = new RectOffset(newLeft, layoutGroup.padding.right, layoutGroup.padding.top, layoutGroup.padding.bottom);
            
            // Force layout update so the shift is visible and smooth
            LayoutRebuilder.MarkLayoutForRebuild(transform as RectTransform);
        }
    }

    private void OnDisable()
    {
        StopAllCoroutines();
        currentLerp = 0f;
        ApplyEffects(0f);
    }

    // Refresh initial state if the script is reset or values change in editor
    private void OnValidate()
    {
        if (!Application.isPlaying)
        {
            // Just ensure components are linked
            if (iconImage == null) iconImage = transform.Find("Option Icon")?.GetComponent<UnityEngine.UI.Image>();
            if (textLabel == null) textLabel = transform.Find("Option Text Label")?.GetComponent<TextMeshProUGUI>();
            if (layoutGroup == null) layoutGroup = GetComponent<HorizontalLayoutGroup>();
        }
    }
}



