using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlayerManager : MonoBehaviour
{
    [SerializeField]
    private PlayerDataSO playerData;
    private GameUIManager gameUI;
    [SerializeField]
    private Transform[] spawnPositions;
    [SerializeField]
    private Player[] players = new Player[4];
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

    private void CleanUserDevices(PlayerInput input)
    {    //Hack to stop it joining both xbox and ps controllers to one user
        if (input.user.pairedDevices.Count > 1)
        {
            InputDevice device = input.user.pairedDevices[0];
            string deviceClass = device.description.deviceClass;
            if (!(deviceClass.Equals("Keyboard") || deviceClass.Equals("Mouse")))
            {
                foreach (InputDevice inputDevice in input.user.pairedDevices)
                {
                    if (inputDevice != device)
                    {
                        input.user.UnpairDevice(inputDevice);
                    }
                }
            }
        }
    }

    private void OnPlayerJoined(PlayerInput playerInput)
    {
        CleanUserDevices(playerInput);

        //foreach (InputDevice device in playerInput.devices)
        //{
        //    Debug.Log(device.name + " - " + device.description);
        //}

        int playerIndex = playerInput.playerIndex;
        playerInput.GetComponent<Player>()
            .SetupPlayer(this, playerIndex, spawnPositions[playerIndex].position);

        Debug.Log(playerInput.name + " joined");
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
