using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class GameUIManager : MonoBehaviour
{
    [SerializeField]
    private PlayerDataSO playerData;
    [SerializeField]
    private Image[] scorePanels = new Image[4];
    private TMPro.TextMeshProUGUI[] scoreText = new TMPro.TextMeshProUGUI[4];
    [SerializeField]
    private TMPro.TextMeshProUGUI clockText;

    public void SetupPlayers(PlayerController[] players)
    {
        for (int i = 0; i < 4; i++)
        {
            if (players[i] == null)
            {
                scorePanels[i].gameObject.SetActive(false);
                continue;
            }

            scorePanels[i].CrossFadeColor(players[i].PlayerColour, 0, false, false);
            scoreText[i] = scorePanels[i].GetComponentInChildren<TMPro.TextMeshProUGUI>();
            scoreText[i].text = "0";
            scorePanels[i].gameObject.SetActive(true);
        }
    }

    public void SetScore(int playerIndex, int score)
    {
        scoreText[playerIndex].text = score.ToString();
    }

    ///<summary>Time in seconds</summary>
    public void StartClock(int clockTime, UnityAction onClockEnd = null)
    {
        StartCoroutine(RunClock(clockTime, onClockEnd));
    }

    public IEnumerator RunClock(int clockTime, UnityAction onClockEnd = null)
    {
        int timer = clockTime;
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
}
