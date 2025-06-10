using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(PlayerInputManager))]
public class PlayerManager : MonoBehaviour
{
    [SerializeField]
    private PlayerDataSO playerData;
    private PlayerInputManager inputManager;
    [field: SerializeField]
    public Player[] players { get; private set; } = new Player[4];
    public UnityAction<Player> onPlayerJoin, onPlayerLeave;
    public static PlayerManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded;
            inputManager = GetComponent<PlayerInputManager>();
        }

    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode LoadSceneMode)
    {
        if (scene.buildIndex == (int)Scenes.Lobby)
        {
            if (!inputManager.joiningEnabled)
            {
                inputManager.EnableJoining();
                RemoveAllPlayers();
            }
        }
        else if (scene.buildIndex == 0)
        {
            inputManager.DisableJoining();
        }
    }

    public Coroutine LoadPause()
    {
        return StartCoroutine(LoadPauseMenu());
    }

    private IEnumerator LoadPauseMenu()
    {
        yield return SceneManager.LoadSceneAsync((int)Scenes.PauseMenu, LoadSceneMode.Additive);

        PauseMenu.Instance.DisablePause();

        LobbyManager lobby = FindObjectOfType<LobbyManager>();
        foreach (Player player in players)
        {
            if (player != null)
            {
                if (lobby != null)
                    lobby.OnPlayerJoin(player);

                PauseMenu.Instance.AddPlayerInput(player.PlayerInput);
            }
        }
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

        playerInput.deviceLostEvent.AddListener(OnDeviceLost);

        int playerIndex = playerInput.playerIndex;
        Player player = playerInput.GetComponent<Player>();
        player.Setup(this, playerInput);
        //playerInput.actions["Interact"].Disable();
        player.transform.SetParent(transform);
        players[playerIndex] = player;
        onPlayerJoin?.Invoke(player);
        //PauseMenu.Instance.AddPlayerInput(playerInput);

        Debug.Log(playerInput.name + " joined");
    }

    public void OnDeviceLost(PlayerInput playerInput)
    {
        Debug.Log(playerInput.name + " disconnected");

        //int playerIndex = playerInput.playerIndex;
        //onPlayerLeave?.Invoke(players[playerIndex]);
        //Destroy(players[playerIndex].gameObject);
        //players[playerIndex] = null;
    }

    public void RemoveAllPlayers()
    {
        for (int i = 0; i < 4; i++)
        {
            if (players[i] != null)
            {
                players[i].PlayerInput.user.UnpairDevicesAndRemoveUser();
                Destroy(players[i].gameObject);
                players[i] = null;
            }
        }
    }
    public void RemovePlayer(int playerIndex)
    {
        if (players[playerIndex] != null)
        {
            players[playerIndex].PlayerInput.user.UnpairDevicesAndRemoveUser();
            Destroy(players[playerIndex].gameObject);
            players[playerIndex] = null;
        }

    }
}
