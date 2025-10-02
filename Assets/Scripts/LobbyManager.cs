using System;
using System.Linq;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using UnityEngine.UI;

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
    private Action<InputAction.CallbackContext>[]
        readyActions, unreadyActions, rightBumpActions, leftBumpActions, nextColourAction, nextModelAction;
    [SerializeField]
    private GameObject readyBanner;
    private bool gameReady = false;
    private int minPlayers = 2;
    public bool isTeams = false;
    ///<summary>true = stock | false = score</summary>
    public bool winCon = true;
    public int gameTime = 5, startStocks = 5, scoreLimit = 10, maxStocks = 20, maxScore = 40, maxTime = 15;
    [SerializeField]
    private Selectable gameSettingsBtn, optionsBtn;
    private bool[] colourChosen;
    //private bool unsavedSettings = false;
    [SerializeField]
    private GameSettingsMenu gameSettingsMenu;

    private void Awake()
    {
        playerManager = FindObjectOfType<PlayerManager>();
        playerManager.onPlayerJoin += OnPlayerJoin;
        playerManager.onPlayerLeave += OnPlayerLeave;
        playerManager.SetJoining(true);
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

        colourChosen = new bool[playerData.playerColoursOptions.Length];
        for (int i = 0; i < colourChosen.Length; i++)
        {
            colourChosen[i] = false;
        }

        gameSettingsMenu.LoadGameSettings(this, playerData.LastGameSettings, playerData.GameModeDescription);

#if UNITY_EDITOR
        minPlayers = 1;
#endif
    }

    private void OnDestroy()
    {
        playerManager.onPlayerJoin -= OnPlayerJoin;
        playerManager.onPlayerLeave -= OnPlayerLeave;
        for (int i = 0; i < 4; i++)
        {
            if (players[i] == null)
                continue;

            players[i].PlayerInput.actions["Join"].performed -= readyActions[i];
            players[i].PlayerInput.actions["Cancel"].performed -= unreadyActions[i];
            //players[i].PlayerInput.actions["RightBumper"].performed -= rightBumpActions[i];
            //players[i].PlayerInput.actions["LeftBumper"].performed -= leftBumpActions[i];
            //players[i].PlayerInput.actions["FaceNorth"].performed -= nextColourAction[i];
            //players[i].PlayerInput.actions["FaceWest"].performed -= nextModelAction[i];
            players[i].RemoveLobbyBindings(ChangePlayerColour, ChangePlayerModel, ChangePlayerWeapon);
        }
    }

    private void Start()
    {
        foreach (Player player in playerManager.players)
        {
            if (player != null)
                OnPlayerJoin(player);
        }
    }

    private void SelectUI(Player player)
    {
        if (!player.EventSystem.alreadySelecting)
        {
            player.EventSystem.SetSelectedGameObject(playerIcons[0].gameObject);
        }
    }

    public void OnPlayerJoin(Player player)
    {
        players[player.PlayerIndex] = player;
        playerStatuses[player.PlayerIndex] = LobbyStatus.Joined;

        ChangePlayerColour(player.PlayerIndex, 0);

        LobbyPlayerIcon icon = playerIcons[player.PlayerIndex];
        icon.SetControllerType(player.ControllerType);
        icon.SetPlayerStatus(LobbyStatus.Joined);
        icon.SetPlayerModel(playerData.characterPrefabs[player.PlayerModelIndex]);
        icon.SetPlayerWeapon(playerData.weaponsOptions[player.PlayerWeaponIndex].WeaponName);

        player.PlayerInput.actions["Join"].performed += readyActions[player.PlayerIndex];
        player.PlayerInput.actions["Cancel"].performed += unreadyActions[player.PlayerIndex];
        player.AddLobbyBindings(ChangePlayerColour, ChangePlayerModel, ChangePlayerWeapon);

        if (player.PlayerIndex == 0)
            SelectUI(player);
        else
            ReadyCheck();
    }

    private void OnPlayerLeave(Player player)
    {
        RemovePlayer(player.PlayerIndex);
    }

    private void OnReady(int playerIndex)
    {
        if (gameReady)
        {
            Debug.Log("Map Select");
            StartCoroutine(LoadMapSelect());
        }
        else if (playerStatuses[playerIndex] == LobbyStatus.Joined)
        {
            playerStatuses[playerIndex] = LobbyStatus.Ready;
            playerIcons[playerIndex].SetPlayerStatus(LobbyStatus.Ready);
            players[playerIndex].DisableLobbyBindings();
            ReadyCheck();
        }
    }

    private void OnUnReady(int playerIndex)
    {
        if (playerStatuses[playerIndex] == LobbyStatus.Ready)
        {
            playerStatuses[playerIndex] = LobbyStatus.Joined;
            playerIcons[playerIndex].SetPlayerStatus(LobbyStatus.Joined);
            players[playerIndex].DisableLobbyBindings();
            ReadyCheck();
        }
        else if (playerStatuses[playerIndex] == LobbyStatus.Joined)
        {
            RemovePlayer(playerIndex);
            playerManager.RemovePlayer(playerIndex);
        }
    }

    public void RemovePlayer(int playerIndex)
    {
        playerStatuses[playerIndex] = LobbyStatus.NoPlayer;
        if (playerStatuses.Count(s => s > LobbyStatus.NoPlayer) == 0)
        {
            EventSystem.current.SetSelectedGameObject(null);
        }

        playerIcons[playerIndex].SetPlayerStatus(LobbyStatus.NoPlayer);
        players[playerIndex].RemoveLobbyBindings(ChangePlayerColour, ChangePlayerModel, ChangePlayerWeapon);
    }

    private void ReadyCheck()
    {
        if (playerStatuses.Count(s => s > LobbyStatus.NoPlayer) >= minPlayers &&
            playerStatuses.Count(s => s == LobbyStatus.Joined) == 0)
        {
            readyBanner.SetActive(true);
            gameReady = true;
        }
        else
        {
            readyBanner.SetActive(false);
            gameReady = false;
        }
    }

    public void ChangePlayerColour(int playerIndex, int direction)
    {
        Player player = players[playerIndex];
        int colourIndex = player.PlayerColourIndex;
        if (colourIndex > -1)
            colourChosen[colourIndex] = false;

        if (!isTeams)
        {
            int i = 0;
            do
            {
                colourIndex = (colourIndex + playerData.playerColoursOptions.Length + direction)
                    % playerData.playerColoursOptions.Length;
                i++;
            }
            while (colourChosen[colourIndex] && i <= 5);
            if (i > 5)
                Debug.LogError("ColourIndex out of bounds");
        }
        else
        {
            colourIndex = (colourIndex + playerData.playerColoursOptions.Length + direction)
                % playerData.playerColoursOptions.Length;
        }

        colourChosen[colourIndex] = true;

        player.SetPlayerColour(colourIndex);
        playerIcons[playerIndex].SetPlayerColour(player.PlayerColour);
        playerIcons[playerIndex].HighlightControl(0, direction);
    }

    public void ChangePlayerModel(int playerIndex, int direction)
    {
        Player player = players[playerIndex];
        int modelIndex = (player.PlayerModelIndex + playerData.characterPrefabs.Length + direction)
            % playerData.characterPrefabs.Length;

        player.SetPlayerModel(modelIndex);
        playerIcons[playerIndex].SetPlayerModel(playerData.characterPrefabs[modelIndex]);
        playerIcons[playerIndex].HighlightControl(1, direction);
    }

    public void ChangePlayerWeapon(int playerIndex, int direction)
    {
        Player player = players[playerIndex];
        int weaponIndex = (player.PlayerWeaponIndex + playerData.weaponsOptions.Length + direction)
            % playerData.weaponsOptions.Length;

        player.SetPlayerWeapon(weaponIndex);
        playerIcons[playerIndex].SetPlayerWeapon(playerData.weaponsOptions[weaponIndex].WeaponName);
        playerIcons[playerIndex].HighlightControl(2, direction);
    }

    public void ReturnToMainMenu()
    {
        playerManager.SetJoining(false);
        SceneManager.LoadScene(0);
    }

    private IEnumerator LoadMapSelect()
    {
        playerManager.SetJoining(false);
        FindObjectOfType<AudioListener>().enabled = false;
        yield return SceneManager.LoadSceneAsync((int)Scenes.MapSelect, LoadSceneMode.Additive);

        WinCon gameWinCon = winCon ? WinCon.STOCK : WinCon.SCORE;
        GameSetting setting = new GameSetting(gameTime, gameWinCon, startStocks, scoreLimit, isTeams);
        playerData.LastGameSettings = setting;
        FindObjectOfType<MapSelect>().Setup(players, setting);

        SceneManager.UnloadSceneAsync((int)Scenes.Lobby);
    }

    public void OpenSettingsMenu()
    {
        gameSettingsMenu.OpenGameSettings();
        for (int i = 0; i < 4; i++)
        {
            if (players[i] == null)
                continue;

            players[i].PlayerInput.actions["Join"].Disable();
            players[i].PlayerInput.actions["Cancel"].performed -= unreadyActions[i];
            players[i].PlayerInput.actions["Cancel"].performed += gameSettingsMenu.CloseMenu;
            players[i].DisableLobbyBindings();

        }
    }

    public void CloseSettingsMenu()
    {
        gameSettingsMenu.gameObject.SetActive(false);
        for (int i = 0; i < 4; i++)
        {
            if (players[i] == null)
                continue;

            players[i].PlayerInput.actions["Join"].Enable();
            players[i].PlayerInput.actions["Cancel"].performed += unreadyActions[i];
            players[i].PlayerInput.actions["Cancel"].performed -= gameSettingsMenu.CloseMenu;
            players[i].EnableLobbyBindings();
        }
        gameSettingsBtn.Select();
    }

    public void OpenOptionsMenu()
    {
        FindObjectOfType<OptionsManager>(true).OpenOptionsMenu(players.First(p => p != null), CloseOptionsMenu);
        for (int i = 0; i < 4; i++)
        {
            if (players[i] == null)
                continue;

            players[i].PlayerInput.actions["Join"].Disable();
            players[i].PlayerInput.actions["Cancel"].performed -= unreadyActions[i];
            players[i].DisableLobbyBindings();

        }
    }

    public void CloseOptionsMenu()
    {
        for (int i = 0; i < 4; i++)
        {
            if (players[i] == null)
                continue;

            players[i].PlayerInput.actions["Join"].Enable();
            players[i].PlayerInput.actions["Cancel"].performed += unreadyActions[i];
            players[i].EnableLobbyBindings();
        }
        optionsBtn.Select();
    }
}
