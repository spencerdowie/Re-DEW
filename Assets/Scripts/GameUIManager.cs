using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameUIManager : MonoBehaviour
{
    [SerializeField]
    private PlayerDataSO playerData;
    [SerializeField]
    private Image[] scorePanels = new Image[4];
    [SerializeField]
    private TMPro.TextMeshProUGUI[] scoreText = new TMPro.TextMeshProUGUI[4];

    private void Start()
    {
        for (int i = 0; i < 4; i++)
        {
            scorePanels[i].CrossFadeColor(playerData.playerColours[i], 0, false, false);
            scoreText[i].text = "0";
        }
    }

    public void SetScore(int playerIndex, int score)
    {
        scoreText[playerIndex].text = score.ToString();
    }
}
