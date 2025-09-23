using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestingSetup : MonoBehaviour
{
    [SerializeField]
    private PlayerDataSO playerData;
    public PlayerManager playerManager;
    public GameManager gameManager;
    [SerializeField]
    public GameSetting gameSetting = new GameSetting(5, WinCon.STOCK, 3, 3, false);

    private void Awake()
    {
        playerManager = FindObjectOfType<PlayerManager>();
        playerManager.onPlayerJoin += OnPlayerJoin;
        playerManager.onPlayerLeave += OnPlayerLeave;
        //playerManager.LoadPause();
    }

    public void OnPlayerJoin(Player player)
    {
        //spawnPosition.gameObject.SetActive(true);

        gameManager.Setup(gameSetting, 7);
        StartCoroutine(gameManager.StartGameCountdown());
    }

    private void OnPlayerLeave(Player player)
    {

    }

    private void OnDestroy()
    {
        playerManager.onPlayerJoin -= OnPlayerJoin;
        playerManager.onPlayerLeave -= OnPlayerLeave;
    }
}
