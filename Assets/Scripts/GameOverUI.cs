using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameOverUI : MonoBehaviour
{
    private struct Score
    {
        public int index, score;

        public Score(int index, int score)
        {
            this.index = index;
            this.score = score;
        }
    }
    [SerializeField]
    private PlayerDataSO playerData;
    [SerializeField]
    private Image[] scorePanels = new Image[4];
    private TMPro.TextMeshProUGUI[] scoreText = new TMPro.TextMeshProUGUI[4];
    [SerializeField]
    private TMPro.TextMeshProUGUI winnerText;

    public void Setup(int[] scores, bool[] isPlayer)
    {
        int maxScore = scores.Max();
        winnerText.text = "Winner Player " + (scores.ToList().IndexOf(maxScore) + 1);
        for (int i = 0; i < 4; i++)
        {
            scoreText[i] = scorePanels[i].GetComponentInChildren<TMPro.TextMeshProUGUI>();
            scorePanels[i].CrossFadeColor(playerData.playerColours[i], 0, false, false);
            scoreText[i].text = scores[i].ToString();
            scorePanels[i].gameObject.SetActive(isPlayer[i]);
        }
    }
}
