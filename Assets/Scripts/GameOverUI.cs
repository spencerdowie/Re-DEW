using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;


public class GameOverUI : MonoBehaviour
{
    readonly private string[] place = { "1st", "2nd", "3rd", "4th" };
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
    [SerializeField]
    private Transform[] playerModels = new Transform[4];
    private TMPro.TextMeshProUGUI[] scoreText = new TMPro.TextMeshProUGUI[4];
    [SerializeField]
    private TMPro.TextMeshProUGUI winnerText;

    public void Setup(int[] scores, int[] playerColourIndex, int[] playerModelIndex, WinCon winCon)
    {
        List<Score> scoreList = new List<Score>();
        for (int i = 0; i < 4; i++)
        {
            scoreList.Add(new Score(i, scores[i]));

            if (playerColourIndex[i] < 0)
            {
                scorePanels[i].gameObject.SetActive(false);
                continue;
            }

            Color playerColour = playerData.playerColoursOptions[playerColourIndex[i]];

            scoreText[i] = scorePanels[i].GetComponentInChildren<TMPro.TextMeshProUGUI>();
            scorePanels[i].CrossFadeColor(playerColour, 0, false, false);
            if (winCon == WinCon.SCORE)
                scoreText[i].text = scores[i].ToString();
            else
                scoreText[i].text = place[i];

                Instantiate(playerData.characterPrefabs[playerModelIndex[i]], playerModels[i]);
        }

        scoreList.Sort((scoreA, scoreB) => scoreB.score.CompareTo(scoreA.score));
        int maxScore = scoreList[0].score;
        string winnersNames = "";
        for (int i = 0; i < 4; i++)
        {
            //Debug.Log(scoreList[i].index + " - " + scoreList[i].score);
            scorePanels[scoreList[i].index].transform.parent.SetSiblingIndex(i);
            if (playerColourIndex[i] >= 0 && scoreList[i].score == maxScore)
            {
                if (i > 0)
                    winnersNames += " ";
                winnersNames += "Player " + (scoreList[i].index + 1);
            }
        }
        winnerText.text = winnersNames;

        FindObjectOfType<EventSystem>().SetSelectedGameObject(GameObject.Find("LobbyBtn"));

        SceneManager.UnloadSceneAsync((int)Scenes.PauseMenu);
    }

    public void ReturnToLobby()
    {
        SceneManager.LoadScene((int)Scenes.Lobby);
    }

    public void ReturnToMenu()
    {
        SceneManager.LoadScene(0);
    }
}
