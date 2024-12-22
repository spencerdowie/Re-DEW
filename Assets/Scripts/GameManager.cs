using System.Collections;
using System.Collections.Generic;
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
    private bool[] isPlayer = new bool[] { false, false, false, false };
    [SerializeField]
    private int gameTime = 300, scoreLimit = 10;

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
                isPlayer[player.PlayerIndex] = true;
            }
        }
        gameUI.Setup(isPlayer);
        gameUI.StartClock(gameTime, () => StartCoroutine(EndGame()));
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
        players[playerHit].KillPlayer();
        scores[shootingPlayer]++;
        gameUI.SetScore(shootingPlayer, scores[shootingPlayer]);
        StartCoroutine(RespawnPlayer(playerHit));
        if (scores[shootingPlayer] >= scoreLimit)
        {
            StartCoroutine(EndGame());
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
        PauseMenu.Instance.DisablePause();
        yield return SceneManager.LoadSceneAsync(6, LoadSceneMode.Additive);
        GameOverUI gameOverUI = FindObjectOfType<GameOverUI>();
        gameOverUI.Setup(new int[] { 1, 2, 4, 4 }, new bool[] { true, true, true, true });
        SceneManager.UnloadSceneAsync(4);
    }
}
