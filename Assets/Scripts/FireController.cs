using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class FireController : MonoBehaviour
{
    [SerializeField]
    private int playerNumber = 0;
    [SerializeField]
    private Transform laserSpawn;
    [SerializeField]
    private GameObject laserPrefab;
    [SerializeField]
    private int ammo = 3;
    [SerializeField]
    private int score = 0;

    public void OnFire(InputValue value)
    {
        if (ammo > 0)
        {
            Instantiate(laserPrefab, laserSpawn.position, laserSpawn.rotation)
                .GetComponent<Laser>().Setup(playerNumber, () => ammo++, RecieveKill);
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
