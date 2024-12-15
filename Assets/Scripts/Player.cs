using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    [SerializeField]
    private PlayerDataSO playerData;
    private PlayerManager playerManager;
    private PlayerInput playerInput;
    [SerializeField]
    private int playerIndex = 0;
    [SerializeField]
    private Color playerColour = Color.white;
    private PlayerController playerController;

    public int PlayerIndex { get => playerIndex; }
    public Color PlayerColour { get => playerColour; }
    public PlayerInput PlayerInput { get => playerInput;}

    public void Setup(PlayerManager playerManager, PlayerInput playerInput)
    {
        this.playerManager = playerManager;
        this.playerInput = playerInput;
        playerIndex = playerInput.playerIndex;
        name = "Player " + playerIndex;
        playerColour = playerData.playerColours[playerIndex];
    }
}
