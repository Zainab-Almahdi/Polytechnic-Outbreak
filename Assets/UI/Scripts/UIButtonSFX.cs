using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.VFX;

public class UIButtonSFX : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler
{
    public void OnPointerEnter(PointerEventData eventData)
    {
        UISFXManager.Instance?.PlayHover();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        UISFXManager.Instance?.PlayClick();
    }
}