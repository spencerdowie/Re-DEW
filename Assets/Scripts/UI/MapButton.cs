using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MapButton : MonoBehaviour, ISelectHandler
{
    [SerializeField]
    private PlayerDataSO playerData;
    public UnityAction<int> onSelect;
    [SerializeField]
    private Image mapPreview;
    private int mapIndex = -1;

    public void Setup(int index)
    {
        mapIndex = index;
        mapPreview.sprite = playerData.Maps[mapIndex].sprite;
    }

    public void OnSelect(BaseEventData e)
    {
        onSelect?.Invoke(mapIndex);
    }
}
