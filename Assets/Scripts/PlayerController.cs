using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    [SerializeField]
    private PlayerDataSO playerData;
    private GameManager gameManager;
    private new Rigidbody rigidbody;
    [SerializeField]
    private Animator animator;
    private Player player;
    [SerializeField]
    private Transform laserSpawn;
    [SerializeField]
    private MeshRenderer playerIndicator;
    public int PlayerIndex { get => player?.PlayerIndex ?? -1; }
    public Color PlayerColour { get => player?.PlayerColour ?? Color.black; }
    public int PlayerColourIndex { get => player?.PlayerColourIndex ?? -1; }
    [SerializeField]
    private int ammo = 3;
    public UnityAction<int> updateAmmo;
    public bool isInvuln { get; private set; }
    [SerializeField]
    private SkinnedMeshRenderer[] playerModels;
    private float rotationVelocity = 0f;
    private int hitLayer;
    private bool holdPlayer = false;

    private void Awake()
    {
        //Debug.Log(name + " Spawned");
        rigidbody = GetComponent<Rigidbody>();
    }

    private void OnDestroy()
    {
        Debug.Log(name + " Destroyed");
        if (player != null)
            player.RemoveFireCallback(OnFire, OnDebugFire);
    }

    public void Setup(GameManager gameManager, Player player, int teamID)
    {
        this.gameManager = gameManager;
        this.player = player;
        name = player.name + " Character";
        hitLayer = LayerMask.NameToLayer("Team" + teamID);


        gameObject.layer = hitLayer;
        playerIndicator.material.color = PlayerColour;
        playerIndicator.material.SetColor("_EmissionColor", PlayerColour);
        GetComponentInChildren<Light>().color = PlayerColour;
        player.onFire.performed += OnFire;
        player.onDebugFire.performed += OnDebugFire;
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
        //rigidbody.enabled = true; //Otherwise it resets postion to origin
        StartCoroutine(MakeInvuln(playerData.RespawnInvulnTime, true));
        rigidbody.velocity = Vector3.zero;
    }

    private void FixedUpdate()
    {
        if (!holdPlayer)
            Move();
    }

    private void Move()
    {
        float deltaTime = Time.deltaTime;
        float speed = 0f;
        float targetSpeed = player.inputMove == Vector2.zero ? 0f : playerData.MoveSpeed * player.inputMove.magnitude;

        float currentSpeed = new Vector2(rigidbody.velocity.x, rigidbody.velocity.z).magnitude;

        if (Mathf.Abs(targetSpeed - currentSpeed) > 0.1f)
        {
            speed = Mathf.Lerp(currentSpeed, targetSpeed, deltaTime * playerData.SpeedChangeRate);
        }

        Vector3 moveDirection = new Vector3(player.inputMove.x, 0f, player.inputMove.y).normalized;

        if (player.inputMove != Vector2.zero || player.inputAim != Vector2.zero)
        {
            Vector3 aimDirection = player.inputAim != Vector2.zero ?
                new Vector3(player.inputAim.x, 0f, player.inputAim.y).normalized : moveDirection;

            float rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y,
                Mathf.Atan2(aimDirection.x, aimDirection.z) * Mathf.Rad2Deg,
                ref rotationVelocity,
                playerData.RotationSmoothTime);

            transform.rotation = Quaternion.Euler(0f, rotation, 0f);
        }

        Vector3 gravVel = rigidbody.velocity.y * Vector3.up;
        rigidbody.velocity = (moveDirection * speed) + gravVel;
        rigidbody.angularVelocity = Vector3.zero;
        animator.SetFloat("MoveSpeed", speed);
        if (gravVel.y < -1)
            animator.SetBool("Fall", true);
    }

    public void OnFire(InputAction.CallbackContext ctx)
    {
        //Debug.Log(name);
        if (ammo > 0 && !Physics.CheckSphere(laserSpawn.position, 0.05f, LayerMask.GetMask("Default")))
        {
            Transform laserTransform = Instantiate(playerData.laserPrefab, laserSpawn).transform;
            laserTransform.GetComponent<Laser>().Setup(PlayerIndex, hitLayer, PlayerColour, () => UpdateAmmo(1));
            UpdateAmmo(-1);
        }
    }

    private void UpdateAmmo(int change)
    {
        ammo += change;
        updateAmmo?.Invoke(ammo);
    }

    public void OnDebugFire(InputAction.CallbackContext ctx)
    {
        //Debug.Log(name);
        if (!Physics.CheckSphere(laserSpawn.position, 0.05f, LayerMask.GetMask("Default")))
        {
            Transform laserTransform = Instantiate(playerData.laserPrefab, laserSpawn).transform;
            laserTransform.GetComponent<Laser>().Setup(PlayerIndex, hitLayer, PlayerColour, null);

            Destroy(laserTransform.gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Laser laser = other.GetComponentInParent<Laser>();
        if (laser != null)
            gameManager.PlayerHit(PlayerIndex, laser.PlayerIndex);
        if (other.gameObject.layer == LayerMask.NameToLayer("KillPlane"))
            gameManager.PlayerFall(PlayerIndex);
    }

    public IEnumerator KillPlayer()
    {
        float killTime = 1f;
        float timer = 0f;
        //animator.SetBool("Die", true);
        animator.SetBool("Fall", false);
        while (timer < killTime)
        {
            timer += Time.deltaTime;
            foreach (SkinnedMeshRenderer renderer in playerModels)
            {
                renderer.material.SetFloat("_DissolveTime", timer);
            }
            yield return null;
        }
        //animator.SetBool("Die", false);
        transform.position = Vector3.down * 6f;
        foreach (SkinnedMeshRenderer renderer in playerModels)
        {
            renderer.material.SetFloat("_DissolveTime", 0);
        }
        //rigidbody.enabled = false; //Otherwise it resets postion to origin
        gameObject.SetActive(false);
    }

    public IEnumerator MakeInvuln(float invulnTime, bool disableFire = false)
    {
        gameObject.layer = LayerMask.NameToLayer("Invuln");
        isInvuln = true;
        foreach (SkinnedMeshRenderer renderer in playerModels)
        {
            renderer.material.SetFloat("_IsInvuln", 1);
        }
        if (disableFire)
            player.onFire.Disable();

        yield return new WaitForSeconds(invulnTime);

        gameObject.layer = hitLayer;
        isInvuln = false;
        foreach (SkinnedMeshRenderer renderer in playerModels)
        {
            renderer.material.SetFloat("_IsInvuln", 0);
        }
        if (disableFire)
            player.onFire.Enable();
    }

    public void HoldPlayer(bool holdPlayer = true)
    {
        this.holdPlayer = holdPlayer;
        if (holdPlayer)
            player.onFire.Disable();
        else
            player.onFire.Enable();
    }
}
