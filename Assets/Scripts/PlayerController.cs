using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    private PlayerDataSO playerData;
    private GameManager gameManager;
    private Player player;
    [SerializeField]
    private Transform laserSpawn;
    [SerializeField]
    private GameObject laserPrefab;
    [SerializeField]
    private int ammo = 3;
    [SerializeField]
    private MeshRenderer playerIndicator;
    public int PlayerIndex { get => player?.PlayerIndex ?? -1; }
    public Color PlayerColour { get => player?.PlayerColour ?? Color.black; }

    public void Setup(GameManager gameManager, Player player)
    {
        this.gameManager = gameManager;
        this.player = player;
        name = player.name + " Character";
        playerIndicator.material.color = PlayerColour;
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
            laserTransform.GetComponent<Laser>().Setup(PlayerIndex, PlayerColour, () => ammo++);
            ammo--;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out Laser laser))
            gameManager.PlayerHit(PlayerIndex, laser.PlayerIndex);
    }

    public void KillPlayer()
    {
        transform.position = Vector3.left * 6f;
        GetComponent<CharacterController>().enabled = false; //Otherwise it resets postion to origin
        gameObject.SetActive(false);
    }
}
