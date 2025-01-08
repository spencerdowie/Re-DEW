using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlayerManager : MonoBehaviour
{
    [field: SerializeField]
    public Player[] players { get; private set; } = new Player[4];
    public UnityAction<Player> onPlayerJoin, onPlayerLeave;

    private void Awake()
    {
        if (FindObjectsOfType<PlayerManager>().Length > 1)
            Destroy(this.gameObject);

        DontDestroyOnLoad(this.gameObject);
    }

    public void LoadPause()
    {
        StartCoroutine(LoadPauseMenu());
    }

    private IEnumerator LoadPauseMenu()
    {
        yield return SceneManager.LoadSceneAsync(1, LoadSceneMode.Additive);
        PauseMenu pause = FindObjectOfType<PauseMenu>(true);
        foreach (Player player in players)
        {
            if (player != null)
                OnPlayerJoined(player.PlayerInput);
        }
        //pause.DisablePause();
    }

    private void CleanUserDevices(PlayerInput input)
    {    //Hack to stop it joining both xbox and ps controllers to one user
        if (input.user.pairedDevices.Count > 1)
        {
            InputDevice device = input.user.pairedDevices[0];
            string deviceClass = device.description.deviceClass;
            if (!(deviceClass.Equals("Keyboard") || deviceClass.Equals("Mouse")))
            {
                InputDevice[] devices = input.user.pairedDevices.ToArray();
                foreach (InputDevice inputDevice in devices)
                {
                    if (inputDevice != device)
                    {
                        input.user.UnpairDevice(inputDevice);
                    }
                }
            }
        }
    }

    public void OnPlayerJoined(PlayerInput playerInput)
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
        PauseMenu.Instance.AddPlayerInput(playerInput);

        Debug.Log(playerInput.name + " joined");
    }

    private void OnPlayerLeft(PlayerInput playerInput)
    {
        PauseMenu.Instance.RemovePlayerInput(playerInput);
    }

    public void RemoveAllPlayers()
    {
        foreach (Player player in players)
        {
            if (player != null)
                Destroy(player.gameObject);
        }
    }
}
