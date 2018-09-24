using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine.UI;
using System;

public class SearchBar : Slider, IBeginDragHandler, IEndDragHandler
{
    public UnityEvent onBarSelect;
    public UnityEvent onBarReleased;

    public void OnBeginDrag(PointerEventData eventData)
    {
        this.onBarSelect.Invoke();
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        this.onBarReleased.Invoke();
    }
}
