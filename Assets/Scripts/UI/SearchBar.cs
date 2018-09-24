using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine.UI;
using System;

public class SearchBar : Slider, IBeginDragHandler, IEndDragHandler
{
    [SerializeField]
    public UnityEvent onBarSelect;
    public UnityEvent onBarReleased;

    public void OnBeginDrag(PointerEventData eventData)
    {
        Debug.Log("SELECT");
        this.onBarSelect.Invoke();
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        Debug.Log("RELEASE");
        this.onBarReleased.Invoke();
    }
}
