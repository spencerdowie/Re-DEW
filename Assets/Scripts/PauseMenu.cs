using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    [SerializeField]
    private PlayerDataSO playerData;
    private RectTransform rectTransform;
    [SerializeField]
    private EventSystem eventSystem;
    private GameObject prevSelected;
    [SerializeField]
    private float animTimeMax = 0.5f;
    [SerializeField]
    private bool isPaused = false;
    private float timer = 0f;
    private Vector2Int lerp = Vector2Int.right;
    private UnityEvent PauseGame = new UnityEvent(), ResumeGame = new UnityEvent();
    [SerializeField]
    private Selectable resumeBtn;
    private InputAction[] pauseActions = new InputAction[4];
    [SerializeField]
    private GameObject xboxControls, psControls;
    [SerializeField]
    private TMPro.TextMeshProUGUI playerNameText;
    private Player pausePlayer = null;

    public static PauseMenu Instance { get; private set; }

    private bool IsDisabled { get; set; }
    public bool IsPaused
    {
        get => isPaused;

        private set
        {
            //if (isPaused == value)
            //    return;

            isPaused = value;
            if (value)
            {
                PauseGame.Invoke();
            }
            else
            {
                ResumeGame.Invoke();
            }
        }
    }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }

        rectTransform = transform.GetChild(0).GetComponent<RectTransform>();
        rectTransform.anchorMax = Vector2.right;
        rectTransform.anchorMin = Vector2.down;
        rectTransform.gameObject.SetActive(false);

        PlayerManager.Instance.onPlayerDisconnect += PlayerDisconnected;
        PlayerManager.Instance.onPlayerReconnect += PlayerReconnected;
    }

    private void OnDestroy()
    {
        for (int i = 0; i < 4; i++)
        {
            if (pauseActions[i] != null)
            {
                pauseActions[i].started -= TogglePause;
                pauseActions[i] = null;
            }
        }
        Time.timeScale = 1f;
        PauseGame.RemoveAllListeners();
        ResumeGame.RemoveAllListeners();

        PlayerManager.Instance.onPlayerDisconnect -= PlayerDisconnected;
        PlayerManager.Instance.onPlayerReconnect -= PlayerReconnected;
    }

    public void EnablePause()
    {
        foreach (InputAction pauseAction in pauseActions)
        {
            if (pauseAction != null)
                pauseAction.Enable();
        }
        IsDisabled = false;
    }

    public void DisablePause()
    {
        foreach (InputAction pauseAction in pauseActions)
        {
            if (pauseAction != null)
                pauseAction.Disable();
        }
        IsDisabled = true;
    }

    public void AddPlayerInput(PlayerInput playerInput)
    {
        pauseActions[playerInput.playerIndex] = playerInput.actions["Pause"];
        pauseActions[playerInput.playerIndex].started += TogglePause;
        if (IsDisabled)
            pauseActions[playerInput.playerIndex].Disable();
    }

    public void RemovePlayerInput(PlayerInput playerInput)
    {
        pauseActions[playerInput.playerIndex].started -= TogglePause;
    }

    public void AddPauseListeners(UnityAction pause, UnityAction resume)
    {
        PauseGame.AddListener(pause);
        ResumeGame.AddListener(resume);
    }

    public void RemovePauseListeners(UnityAction pause, UnityAction resume)
    {
        PauseGame.RemoveListener(pause);
        ResumeGame.RemoveListener(resume);
    }

    public void Pause(Player player)
    {
        if (isPaused)
            return;

        pausePlayer = player;
        if (pausePlayer.ControllerType == ControllerType.Xbox)
        {
            xboxControls.SetActive(true);
            psControls.SetActive(false);
        }
        else
        {
            xboxControls.SetActive(false);
            psControls.SetActive(true);
        }

        foreach (EventSystem eventSys in FindObjectsOfType<EventSystem>())
        {
            Debug.Log(eventSys.name);
            eventSys.enabled = false;
        }

        eventSystem = pausePlayer.EventSystem;
        eventSystem.enabled = true;

        playerNameText.text = pausePlayer.name;

        Debug.Log("Pause Game.");
        IsPaused = true;
        rectTransform.gameObject.SetActive(true);
        Time.timeScale = 0f;

        prevSelected = eventSystem.currentSelectedGameObject;
        resumeBtn.Select();

        lerp = Vector2Int.up;
        if (timer > 0)
        {
            timer = animTimeMax - timer;
        }
        else
            StartCoroutine(Move());
    }

    public void Resume()
    {
        Debug.Log("Resume Game.");
        lerp = Vector2Int.right;

        if (timer > 0)
        {
            timer = animTimeMax - timer;
        }
        else
            StartCoroutine(Move());
    }

    public void ReturnToMenu()
    {
        SceneManager.LoadScene(0);
    }

    public void ReturnToLobby()
    {
        SceneManager.LoadScene((int)Scenes.Lobby);
    }

    public void QuitGame()
    {
        foreach (InputAction pauseAction in pauseActions)
        {
            if (pauseAction != null)
                pauseAction.started -= TogglePause;
        }
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
    }

    public IEnumerator Move()
    {
        Vector2 newPos = rectTransform.anchorMax;
        while (timer <= animTimeMax)
        {
            timer += Time.unscaledDeltaTime;
            newPos.y = Mathf.Lerp(lerp.x, lerp.y, timer / animTimeMax);
            rectTransform.anchorMax = newPos;
            rectTransform.anchorMin = newPos - Vector2.one;
            yield return null;
        }
        timer = 0f;
        if (lerp.y == 0)//disable the popup when shrunk
        {
            rectTransform.gameObject.SetActive(false);
            IsPaused = false;
            Time.timeScale = 1f;

            if (prevSelected != null)
                eventSystem.SetSelectedGameObject(prevSelected);
            prevSelected = null;
        }
    }

    private void TogglePause(InputAction.CallbackContext ctx)
    {
        if (lerp.y == 1)
        {
            Resume();
            FindObjectOfType<OptionsManager>(true).CloseOptionsMenu();
        }
        else
        {
            Pause(PlayerManager.Instance.GetPlayerFromDevice(ctx.control.device));
        }
    }

    private void OnApplicationFocus(bool focus)
    {
#if !UNITY_EDITOR
        if (!focus && !IsDisabled)
        {
            Pause(PlayerManager.Instance.players[0]);
        }
#endif
    }

    private void PlayerDisconnected(Player player)
    {
        Pause(player);
    }

    private void PlayerReconnected(Player player)
    {

    }

    public void OpenOptionsMenu()
    {
        FindObjectOfType<OptionsManager>(true).OpenOptionsMenu(pausePlayer);
    }
}
