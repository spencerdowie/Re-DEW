using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class GameUIManager : MonoBehaviour
{
    [SerializeField]
    private Canvas canvas;
    [SerializeField]
    private PlayerDataSO playerData;
    [SerializeField]
    private PlayerUI[] UIPanels = new PlayerUI[4];
    [SerializeField]
    private TMPro.TextMeshProUGUI clockText;
    private int timer = 0;

    public void SetupPlayers(PlayerController[] players, int startingValue)
    {
        for (int i = 0; i < 4; i++)
        {
            if (players[i] == null)
            {
                UIPanels[i].gameObject.SetActive(false);
            }
            else
            {
                UIPanels[i].SetPlayerColour(players[i].PlayerColour);
                UIPanels[i].SetValue(startingValue);
                UIPanels[i].SetAmmo(playerData.StartAmmo);
                players[i].updateAmmo += UIPanels[i].SetAmmo;
            }
        }
    }

    public void SetValue(int playerIndex, int value)
    {
        UIPanels[playerIndex].SetValue(value);
    }

    ///<summary>Time in seconds</summary>
    public void StartClock(int clockTime, UnityAction onClockEnd = null)
    {
        StartCoroutine(RunClock(clockTime, onClockEnd));
    }

    public IEnumerator RunClock(int clockTime, UnityAction onClockEnd = null)
    {
        timer = clockTime;
        while (timer > 0)
        {
            int minutes = timer / 60;
            int seconds = timer % 60;
            clockText.text = minutes + ":" + seconds.ToString("D2");
            yield return new WaitForSeconds(1f);
            timer--;
        }
        onClockEnd?.Invoke();
    }

    //<summary>Time in minutes</sumary>
    public void DebugSetTime(int time)
    {
        timer = time * 60;
    }
}
