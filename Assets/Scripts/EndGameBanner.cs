using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.Advertisements.Advertisement;

public class EndGameBanner : MonoBehaviour
{
    [SerializeField]
    private Material fireMat;
    [SerializeField]
    private Image scorePanel;
    [SerializeField]
    private Transform modelHolder;
    [SerializeField]
    private TMPro.TextMeshProUGUI nameText, scoreText;
    [SerializeField]
    private Image fireBackground;

    public void Setup(int playerIndex, Color playerColour, string scoreString, GameObject playerModel)
    {
        fireBackground.material = Instantiate(fireMat);
        fireBackground.color = playerColour;
        fireBackground.material.color = playerColour;
        scorePanel.CrossFadeColor(playerColour, 0, false, false);
        nameText.text = "Player " + playerIndex;
        scoreText.text = scoreString;
        Instantiate(playerModel, modelHolder);
    }

    public void DisableFire()
    {
        Color colour = fireBackground.material.color;
        fireBackground.material = null;
    }
}
