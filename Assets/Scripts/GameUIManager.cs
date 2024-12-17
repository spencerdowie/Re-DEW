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

    public void Setup(bool[] isPlayer)
    {
        for (int i = 0; i < 4; i++)
        {
            scorePanels[i].CrossFadeColor(playerData.playerColours[i], 0, false, false);
            scoreText[i] = scorePanels[i].GetComponentInChildren<TMPro.TextMeshProUGUI>();
            scoreText[i].text = "0";
            scorePanels[i].gameObject.SetActive(isPlayer[i]);
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
            yield return new WaitForSeconds(1f);
            timer--;
            int minutes = timer / 60;
            int seconds = timer % 60;
            clockText.text = minutes + ":" + seconds.ToString("D2");
        }
        onClockEnd?.Invoke();
    }
}
