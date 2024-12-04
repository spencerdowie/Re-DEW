using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class FireController : MonoBehaviour
{
    [SerializeField]
    private Transform laserSpawn;
    [SerializeField]
    private GameObject laserPrefab;
    [SerializeField]
    private int maxLasers = 3;
    [SerializeField]
    private int ammo = 3;

    public void OnFire(InputValue value)
    {
        if (ammo > 0)
        {
            Instantiate(laserPrefab, laserSpawn.position, laserSpawn.rotation)
                .GetComponent<Laser>().Setup(0, () => ammo++);
            ammo--;
        }
    }
}
