using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class MenuButtonHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public TMP_Text label;
    public Color normalColor = Color.white;
    public Color hoverColor = Color.yellow;

    private void Reset()
    {
        if (label == null)
            label = GetComponentInChildren<TMP_Text>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (label != null)
            label.color = hoverColor;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (label != null)
            label.color = normalColor;
    }
}
