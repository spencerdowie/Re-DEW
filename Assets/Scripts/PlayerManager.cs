using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class PlayerManager : MonoBehaviour
{
    [SerializeField]
    private Player[] players = new Player[4];
    public UnityAction<Player> onPlayerJoin, onPlayerLeave;

    private void CleanUserDevices(PlayerInput input)
    {    //Hack to stop it joining both xbox and ps controllers to one user
        if (input.user.pairedDevices.Count > 1)
        {
            InputDevice device = input.user.pairedDevices[0];
            string deviceClass = device.description.deviceClass;
            if (!(deviceClass.Equals("Keyboard") || deviceClass.Equals("Mouse")))
            {
                foreach (InputDevice inputDevice in input.user.pairedDevices)
                {
                    if (inputDevice != device)
                    {
                        input.user.UnpairDevice(inputDevice);
                    }
                }
            }
        }
    }

    private void OnPlayerJoined(PlayerInput playerInput)
    {
        CleanUserDevices(playerInput);

        //foreach (InputDevice device in playerInput.devices)
        //{
        //    Debug.Log(device.name + " - " + device.description);
        //}

        int playerIndex = playerInput.playerIndex;
        Player player = playerInput.GetComponent<Player>();
        player.Setup(this, playerInput);
        player.transform.SetParent(transform);
        players[playerIndex] = player;
        onPlayerJoin?.Invoke(player);

        Debug.Log(playerInput.name + " joined");
    }

    public void StartGame(GameManager gameManager)
    {
        foreach (Player player in players)
        {
            if (player != null)
            {
                gameManager.AddPlayerController(player);
            }
        }

        gameManager.StartGame();
    }
}
