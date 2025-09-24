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
    private EndGameBanner[] banners = new EndGameBanner[4];
    [SerializeField]
    private Image[] scorePanels = new Image[4];
    [SerializeField]
    private Transform[] playerModels = new Transform[4];
    [SerializeField]
    private TMPro.TextMeshProUGUI[] scoreText = new TMPro.TextMeshProUGUI[4];
    [SerializeField]
    private TMPro.TextMeshProUGUI winnerText;
    [SerializeField]
    private GameObject LobbyBtn;

    public void Setup(int[] scores, int[] playerColourIndex, int[] playerModelIndex, WinCon winCon)
    {
        List<Score> scoreList = new List<Score>();
        for (int i = 0; i < 4; i++)
        {
            scoreList.Add(new Score(i, scores[i]));

            if (playerColourIndex[i] < 0)
            {
                banners[i].gameObject.SetActive(false);
                continue;
            }

            Color playerColour = playerData.playerColoursOptions[playerColourIndex[i]];

            string scoreText;
            if (winCon == WinCon.SCORE)
                scoreText = scores[i].ToString();
            else
                scoreText = place[i];

            banners[i].Setup(i + 1, playerColour, scoreText, playerData.characterPrefabs[playerModelIndex[i]]);
        }

        scoreList.Sort((scoreA, scoreB) => scoreB.score.CompareTo(scoreA.score));
        int maxScore = scoreList[0].score;
        string winnersNames = "";
        for (int i = 0; i < 4; i++)
        {
            //Debug.Log(scoreList[i].index + " - " + scoreList[i].score);
            banners[scoreList[i].index].transform.parent.SetSiblingIndex(i);
            if (playerColourIndex[i] >= 0 && scoreList[i].score == maxScore)
            {
                if (i > 0)
                    winnersNames += " ";
                winnersNames += "Player " + (scoreList[i].index + 1);
            }
        }
        winnerText.text = winnersNames;

        EventSystem.current.SetSelectedGameObject(LobbyBtn);

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
