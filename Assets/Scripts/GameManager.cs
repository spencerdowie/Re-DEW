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
    private bool isPaused = false;

    private void Awake()
    {
        StartCoroutine(LoadGameUI());
    }

    public IEnumerator LoadGameUI()
    {
        yield return SceneManager.LoadSceneAsync(2, LoadSceneMode.Additive);
        gameUI = FindObjectOfType<GameUIManager>();
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

        StartGame();

    }

    public void StartGame()
    {
        foreach (PlayerController player in players)
        {
            if (player != null)
                StartCoroutine(RespawnPlayer(player.PlayerIndex));
        }
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
    }

    public IEnumerator RespawnPlayer(int playerIndex)
    {
        float timer = 0f;
        while (timer < playerData.RespawnTime)
        {
            if (!isPaused)
                timer += Time.deltaTime;

            yield return null;
        }
        players[playerIndex].SpawnPlayer(spawnPositions[playerIndex].position);
    }
}
