using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Selectable))]
public class UISelectable : MonoBehaviour, ISelectHandler, IDeselectHandler
{
    public UnityEvent onSelect, onDeselect;

    public void OnSelect(BaseEventData e)
    {
        onSelect?.Invoke();
    }

    public void OnDeselect(BaseEventData e)
    {
        onDeselect?.Invoke();
    }
}
