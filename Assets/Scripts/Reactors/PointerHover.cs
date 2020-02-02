using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PointerHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public float HoverMultiplier = 1.25f;

    public void OnPointerEnter(PointerEventData eventData)
    {
        transform.DOScale(Vector3.one * HoverMultiplier, 0.25f);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        transform.DOScale(Vector3.one, 0.25f);
    }
}