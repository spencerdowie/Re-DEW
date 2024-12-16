using System;
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
    private PlayerController playerController;
    public int PlayerIndex { get; private set; } = 0;
    public Color PlayerColour { get; private set; } = Color.white;
    public PlayerInput PlayerInput { get => playerInput; }
    public Vector2 inputMove { get => playerInput.actions["Move"].ReadValue<Vector2>(); }
    public Vector2 inputAim { get => playerInput.actions["Aim"].ReadValue<Vector2>(); }
    public InputAction onFire;

    public void Setup(PlayerManager playerManager, PlayerInput playerInput)
    {
        this.playerManager = playerManager;
        this.playerInput = playerInput;
        PlayerIndex = playerInput.playerIndex;
        name = "Player " + PlayerIndex;
        PlayerColour = playerData.playerColours[PlayerIndex];
        onFire = playerInput.actions["Fire"];
    }

    public void RemoveFireCallback(Action<InputAction.CallbackContext> callback)
    {
        if (playerInput != null)
            playerInput.actions["Fire"].performed -= callback;
    }
}
