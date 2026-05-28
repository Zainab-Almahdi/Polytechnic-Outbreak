using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UISFXButtonHover : MonoBehaviour, IPointerEnterHandler
{
    public static void Ensure(Button button)
    {
        if (button == null)
        {
            return;
        }

        if (button.GetComponent<UISFXButtonHover>() == null)
        {
            button.gameObject.AddComponent<UISFXButtonHover>();
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (UISFXManager.Instance != null)
        {
            UISFXManager.Instance.PlayHover();
        }
    }
}
