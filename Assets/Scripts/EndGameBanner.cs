using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EndGameBanner : MonoBehaviour
{
    [SerializeField]
    private Image scorePanel;
    [SerializeField]
    private Transform modelHolder;
    [SerializeField]
    private TMPro.TextMeshProUGUI nameText, scoreText;

    public void Setup(int playerIndex, Color playerColour, string scoreString, GameObject playerModel)
    {
        scorePanel.CrossFadeColor(playerColour, 0, false, false);
        nameText.text = "Player " + playerIndex;
        scoreText.text = scoreString;
        Instantiate(playerModel, modelHolder);
    }
}
