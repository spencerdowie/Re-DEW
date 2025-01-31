using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public struct GameSetting
{
    public int GameTime { get; private set; }
    public int MaxStocks { get; private set; }
    public int ScoreLimit { get; private set; }
    public bool IsTeams { get; private set; }
    public bool WinCon { get; private set; }

    public GameSetting(int gameTime, bool winCon, int maxStocks, int scoreLimit, bool isTeams)
    {
        GameTime = gameTime;
        MaxStocks = maxStocks;
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
    private bool[] isPlayerArray { get => players.Select(p => p?.PlayerIndex > -1).ToArray(); }
    private int mapID = -1;
    private GameSetting gameSetting;
    [SerializeField]
    private int countdownTime = 5;

    private void Awake()
    {
        StartCoroutine(LoadGameUI());
    }

    public IEnumerator LoadGameUI()
    {
        yield return SceneManager.LoadSceneAsync(playerData.GameUI, LoadSceneMode.Additive);
        gameUI = FindObjectOfType<GameUIManager>();
        gameUI.SetUICamera(gameCamera);
        StartCoroutine(StartGameCountdown());
    }

    public void Setup(GameSetting gameSetting, int mapID)
    {
        this.gameSetting = gameSetting;
        this.mapID = mapID;
        foreach (Player player in PlayerManager.Instance.players)
        {
            if (player != null)
            {
                AddPlayerController(player);
            }
        }

        PauseMenu.Instance.EnablePause();

    }

    public IEnumerator StartGameCountdown()
    {
        gameUI.SetupPlayers(players);
        foreach (PlayerController player in players)
        {
            if (player != null)
            {
                StartCoroutine(RespawnPlayer(player.PlayerIndex));
                isPlayerArray[player.PlayerIndex] = true;
                player.HoldPlayer();
            }
        }
        yield return new WaitForSeconds(countdownTime);
        foreach (PlayerController player in players)
        {
            if (player != null)
            {
                player.HoldPlayer(false);
            }
        }
        gameUI.StartClock(gameSetting.GameTime * 60, () => StartCoroutine(EndGame()));
    }

    public void AddPlayerController(Player player)
    {
        PlayerController playerController =
            Instantiate(playerData.playerCharacterPrefab, transform).GetComponent<PlayerController>();
        playerController.Setup(this, player);
        players[playerController.PlayerIndex] = playerController;
    }

    public void PlayerHit(int playerHit, int shootingPlayer)
    {
        StartCoroutine(players[playerHit].KillPlayer());
        scores[shootingPlayer]++;
        gameUI.SetScore(shootingPlayer, scores[shootingPlayer]);
        StartCoroutine(RespawnPlayer(playerHit));
        if (scores[shootingPlayer] >= gameSetting.ScoreLimit)
        {
            StartCoroutine(EndGame());
        }
    }

    public void PlayerFall(int playerIndex)
    {
        StartCoroutine(players[playerIndex].KillPlayer());
        StartCoroutine(RespawnPlayer(playerIndex));
        if (!players[playerIndex].isInvuln)
        {
            scores[playerIndex]--;
            gameUI.SetScore(playerIndex, scores[playerIndex]);
        }
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
        Time.timeScale = 0f;
        PauseMenu.Instance.DisablePause();
        SceneManager.UnloadSceneAsync(playerData.GameUI);
        yield return SceneManager.LoadSceneAsync(playerData.EndScene, LoadSceneMode.Additive);
        GameOverUI gameOverUI = FindObjectOfType<GameOverUI>();
        gameOverUI.Setup(scores, players.Select(p => p?.PlayerColourIndex ?? -1).ToArray());
        SceneManager.UnloadSceneAsync(mapID);
    }
}
