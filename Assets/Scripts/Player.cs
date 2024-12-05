using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    [SerializeField]
    private int playerNumber = 0;
    [SerializeField]
    private Color playerColour = Color.white;
    [SerializeField]
    private Transform laserSpawn;
    [SerializeField]
    private GameObject laserPrefab;
    [SerializeField]
    private int ammo = 3;
    [SerializeField]
    private int score = 0;
    [SerializeField]
    private MeshRenderer playerIndicator;

    public void SetupPlayer(int playerNumber, Color playerColour, Vector3 spawnPos)
    {
        name = "Player " + playerNumber;
        this.playerNumber = playerNumber;
        this.playerColour = playerColour;
        playerIndicator.material.color = playerColour;
        transform.position = spawnPos;
        GetComponent<CharacterController>().enabled = true; //Otherwise it resets postion to origin
    }

    public void OnFire(InputValue value)
    {
        if (ammo > 0)
        {
            Instantiate(laserPrefab, laserSpawn.position, laserSpawn.rotation)
                .GetComponent<Laser>().Setup(playerNumber, playerColour, () => ammo++, RecieveKill);
            ammo--;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log("Collision with " + other.name);
        other.GetComponentInParent<Laser>().giveKill?.Invoke(playerNumber);
        KillPlayer();
    }

    private void RecieveKill(int player)
    {
        //Debug.Log("Player" + playerNumber + " Killed Player" + player);
        score++;
    }

    private void KillPlayer()
    {
        transform.position = Vector3.left * 6f;
        gameObject.SetActive(false);
    }
}
