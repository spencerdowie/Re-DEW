using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class PauseMenu : MonoBehaviour
{
    private RectTransform rectTransform;
    [SerializeField]
    private float animTimeMax = 0.5f;
    [SerializeField]
    private bool isPaused = false;
    private float timer = 0f;
    private Vector2Int lerp = Vector2Int.right;
    private UnityEvent PauseGame = new UnityEvent(), ResumeGame = new UnityEvent();
    [SerializeField]
    private GameObject resumeBtn;
    private InputAction[] pauseActions = new InputAction[4];

    public static PauseMenu Instance { get; private set; }


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

        rectTransform = GetComponentInChildren<RectTransform>();
        rectTransform.anchorMax = Vector2.right;
        rectTransform.anchorMin = Vector2.down;
        rectTransform.gameObject.SetActive(false);

    }

    public void AddPlayerInput(PlayerInput playerInput)
    {
        pauseActions[playerInput.playerIndex] = playerInput.actions["Pause"];
        pauseActions[playerInput.playerIndex].started += TogglePause;
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

    private void Pause()
    {
        if (isPaused)
            return;

        Debug.Log("Pause Game.");
        IsPaused = true;
        rectTransform.gameObject.SetActive(true);
        Time.timeScale = 0f;

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
        foreach (InputAction pauseAction in pauseActions)
        {
            if (pauseAction != null)
                pauseAction.started -= TogglePause;
        }
        UnityEngine.SceneManagement.SceneManager.LoadScene(0);
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
        }
    }

    private void TogglePause(InputAction.CallbackContext ctx)
    {
        if (lerp.y == 1)
        {
            Resume();
        }
        else
        {
            Pause();
        }
    }

    private void OnApplicationFocus(bool focus)
    {
        if (!focus)
        {
            Pause();
        }
    }
}
