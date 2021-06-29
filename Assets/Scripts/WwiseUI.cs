using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine;

public class WwiseUI : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler
{
    [SerializeField] private AK.Wwise.Event hoverEvent;
    [SerializeField] private AK.Wwise.Event clickEvent;

    public void OnPointerClick(PointerEventData eventData)
    {
        clickEvent.Post(gameObject);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        hoverEvent.Post(gameObject);
    }
}
