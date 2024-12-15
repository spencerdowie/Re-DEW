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
    private Action<InputAction.CallbackContext>[] readyActions, unreadyActions;

    [SerializeField]
    private int minPlayers = 2;

    private void Awake()
    {
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
    }

    private void OnDestroy()
    {
        for (int i = 0; i < 4; i++)
        {
            if (players[i] == null)
                continue;

            players[i].PlayerInput.actions["Interact"].performed -= readyActions[i];
            players[i].PlayerInput.actions["Cancel"].performed -= unreadyActions[i];
        }
    }

    private void OnPlayerJoin(Player player)
    {
        playerStatuses[player.PlayerIndex] = LobbyStatus.Joined;
        LobbyPlayerIcon icon = playerIcons[player.PlayerIndex];
        icon.SetPlayerStatus(LobbyStatus.Joined);
        icon.SetPlayerColour(player.PlayerColour);
        player.PlayerInput.actions["Interact"].performed += readyActions[player.PlayerIndex];
        player.PlayerInput.actions["Cancel"].performed += unreadyActions[player.PlayerIndex];
        players[player.PlayerIndex] = player;

        PlayerController playerController =
            Instantiate(playerData.playerCharacterPrefab).GetComponent<PlayerController>();
        playerController.transform.position = spawnPositions[player.PlayerIndex].position;
        playerController.GetComponent<CharacterController>().enabled = true;
    }

    private void OnPlayerLeave(Player player)
    {
        playerStatuses[player.PlayerIndex] = LobbyStatus.NoPlayer;
        LobbyPlayerIcon icon = playerIcons[player.PlayerIndex];
        icon.SetPlayerStatus(LobbyStatus.NoPlayer);
        icon.SetPlayerColour(Color.black);
        player.PlayerInput.actions["Interact"].performed -= readyActions[player.PlayerIndex];
        player.PlayerInput.actions["Cancel"].performed -= unreadyActions[player.PlayerIndex];
    }

    private void OnReady(int playerIndex)
    {
        if (playerStatuses[playerIndex] == LobbyStatus.Joined)
        {
            playerStatuses[playerIndex] = LobbyStatus.Ready;
            playerIcons[playerIndex].SetPlayerStatus(LobbyStatus.Ready);
            ReadyCheck();
        }
    }

    private void OnUnReady(int playerIndex)
    {
        if (playerStatuses[playerIndex] == LobbyStatus.Ready)
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

        SceneManager.MoveGameObjectToScene(playerManager.gameObject, SceneManager.GetSceneByBuildIndex(sceneIndex));

        GameManager gameManager = FindObjectOfType<GameManager>();
        playerManager.StartGame(gameManager);

        SceneManager.UnloadSceneAsync(3);
    }
}
