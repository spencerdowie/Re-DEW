using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [SerializeField]
    private PlayerDataSO playerData;
    private GameManager gameManager;
    private CharacterController controller;
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
    private float rotationVelocity = 0f;

    private void Awake()
    {
        //Debug.Log(name + " Spawned");
        controller = GetComponent<CharacterController>();
    }

    private void OnDestroy()
    {
        Debug.Log(name + " Destroyed");
        player.RemoveFireCallback(OnFire);
    }

    public void Setup(GameManager gameManager, Player player)
    {
        this.gameManager = gameManager;
        this.player = player;
        name = player.name + " Character";
        gameObject.layer = LayerMask.NameToLayer("Player" + player.PlayerIndex);
        playerIndicator.material.color = PlayerColour;
        player.onFire.performed += OnFire;
        player.onFire.Disable();
        gameObject.SetActive(false);

        PauseMenu.Instance.AddPauseListeners(Pause, Resume);
    }

    private void Pause()
    {
        player.onFire.Disable();
    }

    private void Resume()
    {
        player.onFire.Enable();
    }

    public void SpawnPlayer(Vector3 spawnPos)
    {
        gameObject.SetActive(true);
        transform.position = spawnPos;
        controller.enabled = true; //Otherwise it resets postion to origin
        player.onFire.Enable();
    }

    private void FixedUpdate()
    {
        //if (!PauseMenu.Instance.IsPaused)
        {
            Move();
        }
    }

    private void Move()
    {
        float deltaTime = Time.deltaTime;
        float speed = 0f;
        float targetSpeed = player.inputMove == Vector2.zero ? 0f : playerData.MoveSpeed;

        float currentSpeed = new Vector2(controller.velocity.x, controller.velocity.z).magnitude;

        if (Mathf.Abs(targetSpeed - currentSpeed) > 0.1f)
        {
            //to use analog movement targetSpeed * inputMove.magnitude
            speed = Mathf.Lerp(currentSpeed, targetSpeed * player.inputMove.magnitude, deltaTime * playerData.SpeedChangeRate);
        }

        Vector3 moveDirection = new Vector3(player.inputMove.x, 0f, player.inputMove.y).normalized;

        //Add aim here
        if (player.inputMove != Vector2.zero)
        {

            float rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y,
                Mathf.Atan2(moveDirection.x, moveDirection.z) * Mathf.Rad2Deg,
                ref rotationVelocity,
                playerData.RotationSmoothTime);

            transform.rotation = Quaternion.Euler(0f, rotation, 0f);
        }

        controller.Move(moveDirection * (speed * deltaTime));
    }

    public void OnFire(InputAction.CallbackContext ctx)
    {
        Debug.Log(name);
        if (ammo > 0)
        {
            Transform laserTransform = Instantiate(laserPrefab, laserSpawn).transform;
            //laserTransform.SetParent(playerManager.transform);
            laserTransform.GetComponent<Laser>().Setup(PlayerIndex, PlayerColour, () => ammo++);
            ammo--;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Laser laser = other.GetComponentInParent<Laser>();
        if (laser != null)
            gameManager.PlayerHit(PlayerIndex, laser.PlayerIndex);
    }

    public void KillPlayer()
    {
        transform.position = Vector3.left * 6f;
        GetComponent<CharacterController>().enabled = false; //Otherwise it resets postion to origin
        gameObject.SetActive(false);
    }
}
