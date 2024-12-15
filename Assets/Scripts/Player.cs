using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    [SerializeField]
    private PlayerDataSO playerData;
    private PlayerManager playerManager;
    [SerializeField]
    private int playerIndex = 0;
    [SerializeField]
    private Color playerColour = Color.white;
    [SerializeField]
    private Transform laserSpawn;
    [SerializeField]
    private GameObject laserPrefab;
    [SerializeField]
    private int ammo = 3;
    [SerializeField]
    private MeshRenderer playerIndicator;

    public void SetupPlayer(PlayerManager playerManager, int playerIndex, Vector3 spawnPos)
    {
        this.playerManager = playerManager;
        this.playerIndex = playerIndex;
        name = "Player " + playerIndex;
        playerColour = playerData.playerColours[playerIndex];
        playerIndicator.material.color = playerColour;
        SpawnPlayer(spawnPos);
    }

    public void SpawnPlayer(Vector3 spawnPos)
    {
        transform.position = spawnPos;
        GetComponent<CharacterController>().enabled = true; //Otherwise it resets postion to origin
    }

    public void OnFire(InputValue value)
    {
        if (ammo > 0)
        {
            Transform laserTransform = Instantiate(laserPrefab, laserSpawn.position, laserSpawn.rotation).transform;
            //laserTransform.SetParent(playerManager.transform);
            laserTransform.GetComponent<Laser>().Setup(playerIndex, playerColour, () => ammo++);
            ammo--;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out Laser laser))
            playerManager.PlayerHit(playerIndex, laser.PlayerIndex);
    }

    public void KillPlayer()
    {
        transform.position = Vector3.left * 6f;
        GetComponent<CharacterController>().enabled = false; //Otherwise it resets postion to origin
        gameObject.SetActive(false);
    }
}
