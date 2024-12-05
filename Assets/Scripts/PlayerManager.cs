using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerManager : MonoBehaviour
{
    [SerializeField]
    private Color[] playerColours;
    [SerializeField]
    private Transform[] spawnPositions;

    //private void Start()
    //{
    //    foreach (InputDevice device in InputSystem.devices)
    //    {
    //        Debug.Log("Device " + device.deviceId + " - " + device.description);
    //    }
    //}

    private void OnPlayerJoined(PlayerInput playerInput)
    {
        //Hack to stop it joining both xbox and ps controllers to one user
        if (playerInput.user.pairedDevices.Count > 1)
        {
            InputDevice device = playerInput.user.pairedDevices[0];
            string deviceClass = device.description.deviceClass;
            if (!(deviceClass.Equals("Keyboard") || deviceClass.Equals("Mouse")))
            {
                foreach (InputDevice inputDevice in playerInput.user.pairedDevices)
                {
                    if (inputDevice != device)
                    {
                        playerInput.user.UnpairDevice(inputDevice);
                    }
                }
            }
        }

        //foreach (InputDevice device in playerInput.devices)
        //{
        //    Debug.Log(device.name + " - " + device.description);
        //}

        int playerIndex = playerInput.playerIndex;
        playerInput.GetComponent<Player>()
            .SetupPlayer(playerIndex, playerColours[playerIndex], spawnPositions[playerIndex].position);

        Debug.Log(playerInput.name + " joined");
    }
}
