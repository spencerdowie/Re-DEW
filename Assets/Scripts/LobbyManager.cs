using System;
using System.Linq;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class LobbyManager : MonoBehaviour
{
    public enum LobbyStatus
    {
        NoPlayer = -1,
        Joined,
        Ready
    }

    [SerializeField]
    private PlayerDataSO playerData;
    public PlayerManager playerManager;
    private Player[] players = new Player[4];
    [SerializeField]
    private LobbyPlayerIcon[] playerIcons = new LobbyPlayerIcon[4];
    private LobbyStatus[] playerStatuses = new LobbyStatus[4] {
        LobbyStatus.NoPlayer,
        LobbyStatus.NoPlayer,
        LobbyStatus.NoPlayer,
        LobbyStatus.NoPlayer };
    [SerializeField]
    private Transform[] spawnPositions;
    [SerializeField]
    private Transform playerHolder;
    private Action<InputAction.CallbackContext>[] readyActions, unreadyActions, rightBumpActions, leftBumpActions;

    private int minPlayers = 2;

    private void Awake()
    {
        playerManager = FindObjectOfType<PlayerManager>();
        playerManager.onPlayerJoin += OnPlayerJoin;
        playerManager.onPlayerLeave += OnPlayerLeave;
        readyActions = new Action<InputAction.CallbackContext>[] {
            (ctx)=>OnReady(0),
            (ctx)=>OnReady(1),
            (ctx)=>OnReady(2),
            (ctx)=>OnReady(3)
        };
        unreadyActions = new Action<InputAction.CallbackContext>[] {
            (ctx)=>OnUnReady(0),
            (ctx)=>OnUnReady(1),
            (ctx)=>OnUnReady(2),
            (ctx)=>OnUnReady(3)
        };
        rightBumpActions = new Action<InputAction.CallbackContext>[] {
            (ctx)=>NextColour(0),
            (ctx)=>NextColour(1),
            (ctx)=>NextColour(2),
            (ctx)=>NextColour(3)
        };
        leftBumpActions = new Action<InputAction.CallbackContext>[] {
            (ctx)=>PrevColour(0),
            (ctx)=>PrevColour(1),
            (ctx)=>PrevColour(2),
            (ctx)=>PrevColour(3)
        };

#if UNITY_EDITOR
        minPlayers = 1;
#endif

        playerManager.LoadPause();
    }

    private void OnDestroy()
    {
        playerManager.onPlayerJoin -= OnPlayerJoin;
        playerManager.onPlayerLeave -= OnPlayerLeave;
        for (int i = 0; i < 4; i++)
        {
            if (players[i] == null)
                continue;

            players[i].PlayerInput.actions["Interact"].performed -= readyActions[i];
            players[i].PlayerInput.actions["Cancel"].performed -= unreadyActions[i];
            players[i].PlayerInput.actions["RightBumper"].performed -= rightBumpActions[i];
            players[i].PlayerInput.actions["LeftBumper"].performed -= leftBumpActions[i];
            //Destroy(players[i].GetComponent<PlayerController>().gameObject);
        }
    }

    private void Start()
    {
        //foreach (Player player in playerManager.players)
        //{
        //    if (player != null)
        //        OnPlayerJoin(player);
        //}
    }

    private void OnPlayerJoin(Player player)
    {
        playerStatuses[player.PlayerIndex] = LobbyStatus.Joined;
        LobbyPlayerIcon icon = playerIcons[player.PlayerIndex];
        icon.SetPlayerStatus(LobbyStatus.Joined);
        icon.SetPlayerColour(player.PlayerColour);
        player.PlayerInput.actions["Interact"].performed += readyActions[player.PlayerIndex];
        player.PlayerInput.actions["Cancel"].performed += unreadyActions[player.PlayerIndex];
        player.PlayerInput.actions["RightBumper"].performed += rightBumpActions[player.PlayerIndex];
        player.PlayerInput.actions["LeftBumper"].performed += leftBumpActions[player.PlayerIndex];
        players[player.PlayerIndex] = player;

        spawnPositions[player.PlayerIndex].gameObject.SetActive(true);
        //PlayerController playerController =
        //    Instantiate(playerData.playerCharacterPrefab, playerHolder).GetComponent<PlayerController>();
        //playerController.Setup(null, player);
        //playerController.SpawnPlayer(spawnPositions[player.PlayerIndex].position);
    }

    private void OnPlayerLeave(Player player)
    {
        playerStatuses[player.PlayerIndex] = LobbyStatus.NoPlayer;
        LobbyPlayerIcon icon = playerIcons[player.PlayerIndex];
        icon.SetPlayerStatus(LobbyStatus.NoPlayer);
        icon.SetPlayerColour(Color.black);
        player.PlayerInput.actions["Interact"].performed -= readyActions[player.PlayerIndex];
        player.PlayerInput.actions["Cancel"].performed -= unreadyActions[player.PlayerIndex];
        player.PlayerInput.actions["RightBumper"].performed -= rightBumpActions[player.PlayerIndex];
        player.PlayerInput.actions["LeftBumper"].performed -= leftBumpActions[player.PlayerIndex];
    }

    private void OnReady(int playerIndex)
    {
        if (!PauseMenu.Instance.IsPaused && playerStatuses[playerIndex] == LobbyStatus.Joined)
        {
            playerStatuses[playerIndex] = LobbyStatus.Ready;
            playerIcons[playerIndex].SetPlayerStatus(LobbyStatus.Ready);
            ReadyCheck();
        }
    }

    private void OnUnReady(int playerIndex)
    {
        if (!PauseMenu.Instance.IsPaused && playerStatuses[playerIndex] == LobbyStatus.Ready)
        {
            playerStatuses[playerIndex] = LobbyStatus.Joined;
            playerIcons[playerIndex].SetPlayerStatus(LobbyStatus.Joined);
        }
    }

    private void ReadyCheck()
    {
        if (playerStatuses.Count(s => s > LobbyStatus.NoPlayer) >= minPlayers &&
            playerStatuses.Count(s => s == LobbyStatus.Joined) == 0)
        {
            Debug.Log("Start Game");
            StartCoroutine(LoadGameScene(4));
        }
    }

    private IEnumerator LoadGameScene(int sceneIndex)
    {
        yield return SceneManager.LoadSceneAsync(sceneIndex, LoadSceneMode.Additive);

        GameManager gameManager = FindObjectOfType<GameManager>();
        gameManager.Setup(playerManager);

        SceneManager.UnloadSceneAsync(3);
    }

    public void NextColour(int playerIndex)
    {
        ChangeColour(playerIndex, 1);
    }

    public void PrevColour(int playerIndex)
    {
        ChangeColour(playerIndex, -1);
    }

    public void ChangeColour(int playerIndex, int direction)
    {
        if (PauseMenu.Instance.IsPaused)
            return;

        Player player = players[playerIndex];
        int colourIndex = (player.PlayerColourIndex + playerData.playerColoursOptions.Length + direction)
            % playerData.playerColoursOptions.Length;

        player.SetPlayerColour(colourIndex);
        playerIcons[playerIndex].SetPlayerColour(player.PlayerColour);
    }
}
