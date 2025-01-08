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
    private Camera[] playerCams = new Camera[4];
    [SerializeField]
    private Image[] scorePanels = new Image[4];
    private TMPro.TextMeshProUGUI[] scoreText = new TMPro.TextMeshProUGUI[4];
    [SerializeField]
    private TMPro.TextMeshProUGUI winnerText;

    public void Setup(int[] scores, bool[] isPlayer)
    {
        List<Score> scoreList = new List<Score>();
        for (int i = 0; i < 4; i++)
        {
            scoreList.Add(new Score(i, scores[i]));

            scoreText[i] = scorePanels[i].GetComponentInChildren<TMPro.TextMeshProUGUI>();
            scorePanels[i].CrossFadeColor(playerData.playerColoursOptions[i], 0, false, false);
            scoreText[i].text = scores[i].ToString();
            playerCams[i].backgroundColor = playerData.playerColoursOptions[i];
            scorePanels[i].transform.parent.gameObject.SetActive(isPlayer[i]);
        }



        scoreList.Sort((scoreA, scoreB) => scoreB.score.CompareTo(scoreA.score));
        int maxScore = scoreList[0].score;
        string winnersNames = "";
        for (int i = 0; i < 4; i++)
        {
            //Debug.Log(scoreList[i].index + " - " + scoreList[i].score);
            scorePanels[scoreList[i].index].transform.parent.SetSiblingIndex(i);
            if (scoreList[i].score == maxScore)
            {
                if (i > 0)
                    winnersNames += " ";
                winnersNames += "Player " + (scoreList[i].index + 1);
            }
        }
        winnerText.text = winnersNames;
    }
}
