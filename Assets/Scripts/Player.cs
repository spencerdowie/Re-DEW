using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    [SerializeField]
    private PlayerDataSO playerData;
    private PlayerInput playerInput;
    public EventSystem eventSystem;
    public int PlayerIndex { get; private set; } = 0;
    public int PlayerColourIndex { get; private set; } = 0;
    public int PlayerModelIndex { get; private set; } = 0;
    public int PlayerWeaponIndex { get; private set; } = 0;
    public Color PlayerColour { get => playerData.playerColoursOptions[PlayerColourIndex]; }
    public PlayerInput PlayerInput { get => playerInput; }
    public Vector2 inputMove { get => playerInput.actions["Move"].ReadValue<Vector2>(); }
    public Vector2 inputAim { get => playerInput.actions["Aim"].ReadValue<Vector2>(); }
    public InputAction onFire, onDebugFire;
    public UnityAction<int, int> LobbyColourAction, LobbyModelAction, LobbyWeaponAction;

    public void Setup(PlayerInput playerInput)
    {
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

    public void SetPlayerWeapon(int weaponIndex)
    {
        PlayerWeaponIndex = weaponIndex;
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

    public void AddLobbyBindings(UnityAction<int, int> lobbyColourAction,
        UnityAction<int, int> lobbyModelAction,
        UnityAction<int, int> lobbyWeaponAction)
    {
        playerInput.actions["LobbyColour"].started += LobbyColour;
        playerInput.actions["LobbyModel"].started += LobbyModel;
        playerInput.actions["LobbyWeapon"].started += LobbyWeapon;
        LobbyColourAction += lobbyColourAction;
        LobbyModelAction += lobbyModelAction;
        LobbyWeaponAction += lobbyWeaponAction;
    }

    public void RemoveLobbyBindings(UnityAction<int, int> lobbyColourAction,
        UnityAction<int, int> lobbyModelAction,
        UnityAction<int, int> lobbyWeaponAction)
    {
        playerInput.actions["LobbyColour"].started -= LobbyColour;
        playerInput.actions["LobbyModel"].started -= LobbyModel;
        playerInput.actions["LobbyWeapon"].started -= LobbyWeapon;
        LobbyColourAction -= lobbyColourAction;
        LobbyModelAction -= lobbyModelAction;
        LobbyWeaponAction -= lobbyWeaponAction;
    }

    public void EnableLobbyBindings()
    {
        playerInput.actions["LobbyColour"].Enable();
        playerInput.actions["LobbyModel"].Enable();
        playerInput.actions["LobbyWeapon"].Enable();
    }

    public void DisableLobbyBindings()
    {
        playerInput.actions["LobbyColour"].Disable();
        playerInput.actions["LobbyModel"].Disable();
        playerInput.actions["LobbyWeapon"].Disable();
    }

    public void LobbyColour(InputAction.CallbackContext ctx)
    {
        int changeDirection = ctx.ReadValue<float>() > 0 ? 1 : -1;
        Debug.Log("Lobby Colour: " + changeDirection);
        LobbyColourAction.Invoke(PlayerIndex, changeDirection);
    }

    public void LobbyModel(InputAction.CallbackContext ctx)
    {
        int changeDirection = ctx.ReadValue<float>() > 0 ? 1 : -1;
        Debug.Log("Lobby Model: " + changeDirection);
        LobbyModelAction.Invoke(PlayerIndex, changeDirection);
    }

    public void LobbyWeapon(InputAction.CallbackContext ctx)
    {
        int changeDirection = ctx.ReadValue<float>() > 0 ? 1 : -1;
        Debug.Log("Lobby Weapon: " + changeDirection);
        LobbyWeaponAction.Invoke(PlayerIndex, changeDirection);
    }
}
