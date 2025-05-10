using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    [SerializeField]
    private PlayerDataSO playerData;
    //private PlayerManager playerManager;
    private PlayerInput playerInput;
    //private PlayerController playerController;
    public int PlayerIndex { get; private set; } = 0;
    public int PlayerColourIndex { get; private set; } = 0;
    public int PlayerModelIndex { get; private set; } = 0;
    public Color PlayerColour { get => playerData.playerColoursOptions[PlayerColourIndex]; }
    public PlayerInput PlayerInput { get => playerInput; }
    public Vector2 inputMove { get => playerInput.actions["Move"].ReadValue<Vector2>(); }
    public Vector2 inputAim { get => playerInput.actions["Aim"].ReadValue<Vector2>(); }
    public InputAction onFire, onDebugFire;

    public void Setup(PlayerManager playerManager, PlayerInput playerInput)
    {
        //this.playerManager = playerManager;
        this.playerInput = playerInput;
        PlayerIndex = playerInput.playerIndex;
        name = "Player " + PlayerIndex;
        PlayerColourIndex = PlayerIndex;
        onFire = playerInput.actions["Fire"];
        onDebugFire = playerInput.actions["DebugFire"];
    }

    public void SetPlayerColour(int colourIndex)
    {
        PlayerColourIndex = colourIndex;
    }

    public void SetPlayerModel(int modelIndex)
    {
        PlayerModelIndex = modelIndex;
    }

    public void RemoveFireCallback(Action<InputAction.CallbackContext> callback, Action<InputAction.CallbackContext> debugCallback = null)
    {
        if (playerInput != null)
        {
            playerInput.actions["Fire"].performed -= callback;
            if (debugCallback != null)
            {
                playerInput.actions["DebugFire"].performed -= debugCallback;
            }
        }
    }
}
