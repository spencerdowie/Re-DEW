using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    private PlayerDataSO playerData;
    private GameUIManager gameUI;
    [SerializeField]
    private Transform[] spawnPositions;
    [SerializeField]
    private PlayerController[] players = new PlayerController[4];
    [SerializeField]
    private int[] scores = new int[] { 0, 0, 0, 0 };
    private bool[] isPlayerArray { get => players.Select(p => p?.PlayerIndex > -1).ToArray(); }

    private void Awake()
    {
        StartCoroutine(LoadGameUI());
    }

    public IEnumerator LoadGameUI()
    {
        yield return SceneManager.LoadSceneAsync(2, LoadSceneMode.Additive);
        gameUI = FindObjectOfType<GameUIManager>();
        StartGame();
    }

    public void Setup(PlayerManager playerManager)
    {
        foreach (Player player in playerManager.players)
        {
            if (player != null)
            {
                AddPlayerController(player);
            }
        }

        PauseMenu.Instance.EnablePause();

    }

    public void StartGame()
    {
        foreach (PlayerController player in players)
        {
            if (player != null)
            {
                StartCoroutine(RespawnPlayer(player.PlayerIndex));
                isPlayerArray[player.PlayerIndex] = true;
            }
        }
        gameUI.Setup(players);
        gameUI.StartClock(playerData.GameTime, () => StartCoroutine(EndGame()));
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
        if (scores[shootingPlayer] >= playerData.ScoreLimit)
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
        SceneManager.UnloadSceneAsync(2);
        yield return SceneManager.LoadSceneAsync(5, LoadSceneMode.Additive);
        GameOverUI gameOverUI = FindObjectOfType<GameOverUI>();
        gameOverUI.Setup(scores, players.Select(p => p?.PlayerColourIndex ?? -1).ToArray());
        SceneManager.UnloadSceneAsync(4);
    }
}
