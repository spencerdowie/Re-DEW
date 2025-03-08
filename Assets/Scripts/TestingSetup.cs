using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestingSetup : MonoBehaviour
{
    [SerializeField]
    private PlayerDataSO playerData;
    public PlayerManager playerManager;
    public Player player;
    public Transform spawnPosition;
    [SerializeField]
    public GameSetting gameSetting = new GameSetting(5, WinCon.STOCK, 3, 3, false);

    private void Awake()
    {
        playerManager = FindObjectOfType<PlayerManager>();
        playerManager.onPlayerJoin += OnPlayerJoin;
        playerManager.onPlayerLeave += OnPlayerLeave;
        playerManager.LoadPause();

    }
    public void OnPlayerJoin(Player player)
    {
        this.player = player;

        //spawnPosition.gameObject.SetActive(true);

        FindObjectOfType<GameManager>().Setup(gameSetting, 7);

    }

    private void OnPlayerLeave(Player player)
    {

    }
}
