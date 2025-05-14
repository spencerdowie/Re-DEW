using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum WinCon
{
    STOCK = 0,
    SCORE
}

[System.Serializable]
public struct GameSetting
{
    public int GameTime { get; private set; }
    public int StartStocks { get; private set; }
    public int ScoreLimit { get; private set; }
    public bool IsTeams { get; private set; }
    public WinCon WinCon { get; private set; }

    public GameSetting(int gameTime, WinCon winCon, int maxStocks, int scoreLimit, bool isTeams)
    {
        GameTime = gameTime;
        StartStocks = maxStocks;
        ScoreLimit = scoreLimit;
        IsTeams = isTeams;
        WinCon = winCon;
    }
}

public class GameManager : MonoBehaviour
{
    [SerializeField]
    private PlayerDataSO playerData;
    [SerializeField]
    private Camera gameCamera;
    private GameUIManager gameUI;
    [SerializeField]
    private Transform[] spawnPositions;
    [SerializeField]
    private PlayerController[] players = new PlayerController[4];
    [SerializeField]
    private int[] scores = new int[] { 0, 0, 0, 0 };
    [SerializeField]
    private int[] stocks = new int[] { 0, 0, 0, 0 };
    private bool[] isPlayerArray { get => players.Select(p => p?.PlayerIndex > -1).ToArray(); }
    private int mapID = -1;
    [SerializeField]
    public GameSetting GameSetting { get; private set; }
    [SerializeField]
    private int countdownTime = 5;
    private Dictionary<int, int> teams = new Dictionary<int, int>();
    [SerializeField]
    private Cinemachine.CinemachineTargetGroup camTargetGroup;
    [SerializeField]
    private bool Testing = false;

    private void Awake()
    {
        StartCoroutine(LoadGameUI());
    }

    public IEnumerator LoadGameUI()
    {
        yield return SceneManager.LoadSceneAsync((int)Scenes.GameUI, LoadSceneMode.Additive);
        gameUI = FindObjectOfType<GameUIManager>();
        if (!Testing)
            StartCoroutine(StartGameCountdown());
        else
        {
            SceneManager.LoadSceneAsync((int)Scenes.DebugMenu, LoadSceneMode.Additive);
        }
    }

    public void Setup(GameSetting gameSetting, int mapID)
    {
        GameSetting = gameSetting;
        this.mapID = mapID;
        foreach (Player player in PlayerManager.Instance.players)
        {
            if (player != null)
            {
                int teamID = player.PlayerColourIndex;
                if (GameSetting.IsTeams)
                {
                    if (!teams.TryGetValue(player.PlayerColourIndex, out teamID))
                    {
                        teamID = teams.Count;
                        teams.Add(player.PlayerColourIndex, teamID);
                    }
                }

                if (GameSetting.WinCon == WinCon.STOCK)
                    stocks[player.PlayerIndex] = GameSetting.StartStocks;

                AddPlayerController(player, teamID);
            }
        }

        PauseMenu.Instance.EnablePause();
    }

    public IEnumerator StartGameCountdown()
    {
        int startingValue = GameSetting.WinCon == WinCon.STOCK ? GameSetting.StartStocks : 0;
        gameUI.SetupPlayers(players, startingValue);
        int numPlayers = 0;
        foreach (PlayerController player in players)
        {
            if (player != null)
            {
                StartCoroutine(RespawnPlayer(player.PlayerIndex));
                isPlayerArray[player.PlayerIndex] = true;
                player.HoldPlayer();
                camTargetGroup.AddMember(player.transform, 1, 2);
                numPlayers++;
            }
        }

#if UNITY_EDITOR
        if (numPlayers < 2)
        {
            camTargetGroup.AddMember(spawnPositions[3], 1, 2);
        }
#endif

        yield return new WaitForSeconds(countdownTime);
        foreach (PlayerController player in players)
        {
            if (player != null)
            {
                player.HoldPlayer(false);
            }
        }
        gameUI.StartClock(GameSetting.GameTime * 60, () => StartCoroutine(EndGame()));
    }

    public void AddPlayerController(Player player, int teamID)
    {
        PlayerController playerController =
            Instantiate(playerData.playerCharacterPrefab, transform).GetComponent<PlayerController>();
        playerController.Setup(this, player, teamID);
        players[playerController.PlayerIndex] = playerController;
    }

    public void PlayerHit(int playerHit, int shootingPlayer)
    {
        StartCoroutine(players[playerHit].KillPlayer());
        UpdateScore(shootingPlayer, 1);
        UpdateStock(playerHit, -1);
        StartCoroutine(RespawnPlayer(playerHit));
        CheckWinCon();
    }

    public void PlayerFall(int playerIndex)
    {
        StartCoroutine(players[playerIndex].KillPlayer());
        StartCoroutine(RespawnPlayer(playerIndex));
        if (!players[playerIndex].isInvuln)
        {
            UpdateScore(playerIndex, -1);
            UpdateStock(playerIndex, -1);
            CheckWinCon();
        }
    }

    public void UpdateScore(int playerIndex, int scoreChange)
    {
        scores[playerIndex] += scoreChange;
        if (GameSetting.WinCon == WinCon.SCORE)
            gameUI.SetValue(playerIndex, scores[playerIndex]);
    }

    public void UpdateStock(int playerIndex, int stockChange)
    {
        stocks[playerIndex] += stockChange;
        if (GameSetting.WinCon == WinCon.STOCK)
            gameUI.SetValue(playerIndex, stocks[playerIndex]);
    }

    public IEnumerator RespawnPlayer(int playerIndex)
    {
        float timer = 0f;
        while (timer < playerData.RespawnTime)
        {
            timer += Time.deltaTime;

            yield return null;
        }
        players[playerIndex].SpawnPlayer(spawnPositions[playerIndex].position);
    }

    private IEnumerator EndGame()
    {
        Debug.Log("Game Over");
        //Time.timeScale = 0f;
        PauseMenu.Instance.DisablePause();
        SceneManager.UnloadSceneAsync((int)Scenes.GameUI);
        yield return SceneManager.LoadSceneAsync((int)Scenes.GameEndScreen, LoadSceneMode.Additive);
        GameOverUI gameOverUI = FindObjectOfType<GameOverUI>();
        gameOverUI.Setup(scores,
            players.Select(p => p?.PlayerColourIndex ?? -1).ToArray(),
            players.Select(p => p?.PlayerModelIndex ?? -1).ToArray());
        SceneManager.UnloadSceneAsync(mapID);
    }

    private void CheckWinCon()
    {
        bool winConMet = false;
        if (GameSetting.WinCon == WinCon.STOCK)
        {
            int playersAlive = 0;
            for (int i = 0; i < 4; i++)
            {
                if (stocks[i] > 0)
                    playersAlive++;
            }
#if DEBUG
            if (players.Count(p => p != null) > 1 && playersAlive < 2)
                winConMet = true;
#else
            if (playersAlive < 2)
                winConMet = true;
#endif
        }
        else
        {
            for (int i = 0; i < 4; i++)
            {
                if (scores[i] > GameSetting.ScoreLimit)
                    winConMet = true;
            }
        }
        if (winConMet)
        {
            StartCoroutine(EndGame());
        }
    }

    public void EndGameDebug()
    {
        StartCoroutine(EndGame());
    }

    public void SetClockTimeDebug(int time)
    {
        gameUI.DebugSetTime(time);
    }
}
