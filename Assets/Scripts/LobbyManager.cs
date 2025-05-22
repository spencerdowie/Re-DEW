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
    private bool isTeams = false;
    ///<summary>true = stock | false = score</summary>
    private bool winCon = true;
    private int gameTime = 5, stockAmt = 5, scoreGoal = 10, maxStocks = 20, maxScore = 40, maxTime = 15;
    [SerializeField]
    private TMPro.TextMeshProUGUI gameTimeText, stockText, scoreText,
        overviewStockText, overviewScoreText, overviewTimeText;
    [SerializeField]
    Toggle overviewTeamToggle;
    [SerializeField]
    private GameObject settingsMenu, settingsOpenBtn, settingsFirstBtn;
    [SerializeField]
    private float SelectDelay = 0.1f;
    [SerializeField]
    private Selectable selectedSetting = null;
    private bool[] colourChosen;

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
        nextColourAction = new Action<InputAction.CallbackContext>[] {
            (ctx)=>NextColour(0),
            (ctx)=>NextColour(1),
            (ctx)=>NextColour(2),
            (ctx)=>NextColour(3)
        };
        nextModelAction = new Action<InputAction.CallbackContext>[] {
            (ctx)=>NextModel(0),
            (ctx)=>NextModel(1),
            (ctx)=>NextModel(2),
            (ctx)=>NextModel(3)
        };

        colourChosen = new bool[playerData.playerColoursOptions.Length];
        for (int i = 0; i < colourChosen.Length; i++)
        {
            colourChosen[i] = false;
        }


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

            players[i].PlayerInput.actions["Join"].performed -= readyActions[i];
            players[i].PlayerInput.actions["Cancel"].performed -= unreadyActions[i];
            players[i].PlayerInput.actions["RightBumper"].performed -= rightBumpActions[i];
            players[i].PlayerInput.actions["LeftBumper"].performed -= leftBumpActions[i];
            players[i].PlayerInput.actions["FaceNorth"].performed -= nextColourAction[i];
            players[i].PlayerInput.actions["FaceWest"].performed -= nextModelAction[i];
            //Destroy(players[i].GetComponent<PlayerController>().gameObject);
        }
    }

    private void Start()
    {
        SetGameTime(gameTime);
        SetStockAmount(stockAmt);
        SetScoreGoal(scoreGoal);
    }

    private void SelectUI()
    {
        if (!EventSystem.current.alreadySelecting)
        {
            EventSystem.current.SetSelectedGameObject(playerIcons[0].gameObject);
        }
    }

    private void DeselectUI()
    {
        if (EventSystem.current.alreadySelecting)
            EventSystem.current.SetSelectedGameObject(null);
    }

    public void OnPlayerJoin(Player player)
    {
        players[player.PlayerIndex] = player;
        playerStatuses[player.PlayerIndex] = LobbyStatus.Joined;

        ChangeColour(player.PlayerIndex, 1);

        LobbyPlayerIcon icon = playerIcons[player.PlayerIndex];
        icon.SetPlayerStatus(LobbyStatus.Joined);
        icon.SetPlayerModel(playerData.characterPrefabs[player.PlayerModelIndex]);
        icon.ShowPlayerModel(true);

        player.PlayerInput.actions["Join"].performed += readyActions[player.PlayerIndex];
        player.PlayerInput.actions["Cancel"].performed += unreadyActions[player.PlayerIndex];
        player.PlayerInput.actions["RightBumper"].performed += rightBumpActions[player.PlayerIndex];
        player.PlayerInput.actions["LeftBumper"].performed += leftBumpActions[player.PlayerIndex];
        player.PlayerInput.actions["FaceNorth"].performed += nextColourAction[player.PlayerIndex];
        player.PlayerInput.actions["FaceWest"].performed += nextModelAction[player.PlayerIndex];

        SelectUI();
    }

    private void OnPlayerLeave(Player player)
    {
        playerStatuses[player.PlayerIndex] = LobbyStatus.NoPlayer;
        LobbyPlayerIcon icon = playerIcons[player.PlayerIndex];
        icon.SetPlayerStatus(LobbyStatus.NoPlayer);
        icon.SetPlayerColour(Color.grey);

        if (playerStatuses.Count(s => s > LobbyStatus.NoPlayer) == 0)
        {
            DeselectUI();
        }
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
            ReadyCheck();
        }
    }

    private void OnUnReady(int playerIndex)
    {
        if (playerStatuses[playerIndex] == LobbyStatus.Ready)
        {
            playerStatuses[playerIndex] = LobbyStatus.Joined;
            playerIcons[playerIndex].SetPlayerStatus(LobbyStatus.Joined);
            ReadyCheck();
        }
        else if (playerStatuses[playerIndex] == LobbyStatus.Joined)
        {
            playerStatuses[playerIndex] = LobbyStatus.NoPlayer;
            playerIcons[playerIndex].SetPlayerStatus(LobbyStatus.NoPlayer);
            playerIcons[playerIndex].SetPlayerColour(Color.grey);
            playerIcons[playerIndex].ShowPlayerModel(false);
            playerManager.RemovePlayer(playerIndex);
        }
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
        int colourIndex = player.PlayerColourIndex;
        if (colourIndex > -1)
            colourChosen[colourIndex] = false;

        if (!isTeams)
        {
            int i = 0;
            do
            {
                colourIndex = (colourIndex + direction) % playerData.playerColoursOptions.Length;
                i++;
            }
            while (colourChosen[colourIndex] && i <= 5);
            if (i > 5)
                Debug.LogError("ColourIndex out of bounds");
        }
        else
        {
            colourIndex = (colourIndex + direction) % playerData.playerColoursOptions.Length;
        }

        colourChosen[colourIndex] = true;
        player.SetPlayerColour(colourIndex);
        playerIcons[playerIndex].SetPlayerColour(player.PlayerColour);
    }

    public void NextModel(int playerIndex)
    {
        ChangePlayerModel(playerIndex, 1);
    }

    public void PrevModel(int playerIndex)
    {
        ChangePlayerModel(playerIndex, -1);
    }

    public void ChangePlayerModel(int playerIndex, int direction)
    {
        if (PauseMenu.Instance.IsPaused)
            return;

        Player player = players[playerIndex];
        int modelIndex = (player.PlayerModelIndex + playerData.characterPrefabs.Length + direction)
            % playerData.characterPrefabs.Length;

        player.SetPlayerModel(modelIndex);
        playerIcons[playerIndex].SetPlayerModel(playerData.characterPrefabs[modelIndex]);
    }

    #region Game Settings
    public void ToggleTeamMode(bool teamMode)
    {
        isTeams = teamMode;
        overviewTeamToggle.isOn = isTeams;
    }

    ///<summary>true = stock | false = score</summary>
    public void ToggleWinCondition()
    {
        SetWinCondition(!winCon);
    }

    public void SetWinCondition(bool winCon)
    {
        this.winCon = winCon;
    }

    private void SetStockAmount(int stockAmt)
    {
        this.stockAmt = stockAmt;
        stockText.text = stockAmt.ToString();
        overviewStockText.text = stockAmt.ToString();
    }

    private void SetScoreGoal(int scoreGoal)
    {
        this.scoreGoal = scoreGoal;
        scoreText.text = scoreGoal.ToString();
        overviewScoreText.text = scoreGoal.ToString();
    }

    private void SetGameTime(int gameTime)
    {
        this.gameTime = gameTime;
        gameTimeText.text = gameTime.ToString();
        overviewTimeText.text = gameTime.ToString();
    }

    public void ChangeStockScore(int change)
    {
        if (winCon)
        {
            int newStock = stockAmt + change;
            if (newStock > 0 && newStock <= maxStocks)
                SetStockAmount(newStock);
        }
        else
        {
            int newScore = scoreGoal + change;
            if (newScore > 0 && newScore <= maxScore)
                SetScoreGoal(newScore);
        }
    }

    public void ChangeGameTime(int change)
    {
        int newTime = gameTime + change;
        if (newTime > 0 && newTime <= maxTime)
            SetGameTime(newTime);

    }
    #endregion

    public void ReturnToMainMenu()
    {
        PauseMenu.Instance.ReturnToMenu();
    }

    private IEnumerator LoadMapSelect()
    {
        FindObjectOfType<AudioListener>().enabled = false;
        yield return SceneManager.LoadSceneAsync((int)Scenes.MapSelect, LoadSceneMode.Additive);

        WinCon gameWinCon = winCon ? WinCon.STOCK : WinCon.SCORE;

        FindObjectOfType<MapSelect>().SetGameSetting(new GameSetting(gameTime, gameWinCon, stockAmt, scoreGoal, isTeams));

        SceneManager.UnloadSceneAsync((int)Scenes.Lobby);
    }

    public void OpenSettingsMenu(bool open = true)
    {
        settingsMenu.SetActive(open);
        for (int i = 0; i < 4; i++)
        {
            if (players[i] == null)
                continue;

            if (open)
            {
                EventSystem.current.SetSelectedGameObject(settingsFirstBtn);
                players[i].PlayerInput.actions["Join"].Disable();
                players[i].PlayerInput.actions["Cancel"].performed -= unreadyActions[i];
                players[i].PlayerInput.actions["Cancel"].performed += CloseMenu;
                players[i].PlayerInput.actions["RightBumper"].Disable();
                players[i].PlayerInput.actions["LeftBumper"].Disable();
            }
            else
            {
                EventSystem.current.SetSelectedGameObject(settingsOpenBtn);
                players[i].PlayerInput.actions["Join"].Enable();
                players[i].PlayerInput.actions["Cancel"].performed += unreadyActions[i];
                players[i].PlayerInput.actions["Cancel"].performed -= CloseMenu;
                players[i].PlayerInput.actions["RightBumper"].Enable();
                players[i].PlayerInput.actions["LeftBumper"].Enable();
            }
        }
    }

    public void SelectSetting(Selectable selected)
    {
        selectedSetting = selected;
    }

    private void CloseMenu(InputAction.CallbackContext ctx)
    {
        if (selectedSetting)
        {
            selectedSetting.Select();
            selectedSetting = null;
        }
        else
            OpenSettingsMenu(false);
    }

    public void SelectNext(GameObject gameObject)
    {
        StartCoroutine(SelectNextDelay(gameObject));
    }

    private IEnumerator SelectNextDelay(GameObject gameObject)
    {
        yield return new WaitForSeconds(SelectDelay);
        EventSystem.current.SetSelectedGameObject(gameObject);
    }
}
