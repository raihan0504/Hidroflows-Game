using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIButtonSound : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    private bool hovered;

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (hovered)
            return;
        hovered = true;

        AudioManager.Instance.PlayButtonHover();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        hovered = false;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        AudioManager.Instance.PlayButtonClick();
    }
}