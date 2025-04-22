using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class DebugMenu : MonoBehaviour
{
    [SerializeField]
    private PlayerDataSO playerData;
    private GameObject debugMenu;
    private GameObject lastSelected;
    [SerializeField]
    private GameObject firstSelected;

    [SerializeField]
    private GameObject gameplayCommands;
    private GameManager gameManager;

    private GameSetting storedGameSetting;
    //private bool hasStoredGS = false;

    private List<int> mapIDs = new List<int>();

    private void Awake()
    {
        if (FindObjectsByType<DebugMenu>(FindObjectsSortMode.None).Length > 1)
        {
            Destroy(gameObject);
            //StartCoroutine(UnloadScene());
            return;
        }

        DontDestroyOnLoad(gameObject);

        debugMenu = transform.GetChild(0).gameObject;
        debugMenu.SetActive(false);
        //gameplayCommands.SetActive(false);
        InputSystem.actions["Debug"].started += ToggleDebug;
        SceneManager.sceneLoaded += OnSceneLoaded;
        SceneManager.sceneUnloaded += OnSceneUnloaded;

        foreach (MapPreview map in playerData.Maps)
        {
            mapIDs.Add(map.sceneID);
        }
        LoadDebugGameSettings();
    }

    private IEnumerator UnloadDebugScene()
    {
        yield return new WaitForEndOfFrame();

        SceneManager.UnloadSceneAsync((int)Scenes.DebugMenu);
    }

    private void LoadDebugGameSettings()
    {
        storedGameSetting = new GameSetting(playerData.gameTime, playerData.winCon,
            playerData.startStocks, playerData.scoreLimit, playerData.isTeams);
    }

    private void ToggleDebug(InputAction.CallbackContext ctx)
    {
        ToggleDebug();
    }

    private void ToggleDebug()
    {
        if (debugMenu.activeSelf)
        {
            CloseDebug();
        }
        else
        {
            lastSelected = EventSystem.current?.currentSelectedGameObject;
            debugMenu.SetActive(true);
            EventSystem.current.SetSelectedGameObject(firstSelected);
        }
    }

    private void CloseDebug()
    {
        if (EventSystem.current && lastSelected)
            EventSystem.current.SetSelectedGameObject(lastSelected);
        debugMenu.SetActive(false);
    }

    private void OnDestroy()
    {
        InputSystem.actions["Debug"].started -= ToggleDebug;
    }

    public void ToggleSceneChanges()
    {

    }

    public void LoadMainMenu()
    {
        SceneManager.LoadScene(0);
    }

    public void LoadLobby()
    {
        SceneManager.LoadScene((int)Scenes.Lobby);
    }

    public void LoadMapSelect()
    {
        UnloadNonPauseScenes();

        SceneManager.LoadScene((int)Scenes.MapSelect, LoadSceneMode.Additive);
    }

    public void LoadMap(int map)
    {
        StartCoroutine(LoadMapScene(mapIDs[map]));
    }

    private IEnumerator LoadMapScene(int mapID)
    {
        UnloadNonPauseScenes();

        yield return SceneManager.LoadSceneAsync(mapID, LoadSceneMode.Additive);

        GameManager gameManager = FindObjectOfType<GameManager>();
        gameManager.Setup(storedGameSetting, mapID);
    }

    public void UnloadNonPauseScenes()
    {
        List<Scene> scenes = new List<Scene>();
        for (int i = 0; i < SceneManager.loadedSceneCount; i++)
        {
            scenes.Add(SceneManager.GetSceneAt(i));
        }
        foreach (Scene scene in scenes)
        {
            if (scene.buildIndex != (int)Scenes.PauseMenu)
                SceneManager.UnloadSceneAsync(scene);
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode loadMode)
    {
        switch ((Scenes)scene.buildIndex)
        {
            case Scenes.MainMenu:
                lastSelected = GameObject.Find("Start");
                break;
            case Scenes.DebugMenu:
                StartCoroutine(UnloadDebugScene());
                return;
            default:
                break;
        }

        if (scene.buildIndex != (int)Scenes.GameUI)
            ShowGameplayCommands(mapIDs.Contains(scene.buildIndex));

        if (debugMenu.activeSelf)
        {
            CloseDebug();
        }
    }

    private void OnSceneUnloaded(Scene scene)
    {
        //if (scene.buildIndex == playerData.Lobby)
        //{

        //}
    }

    public void ShowGameplayCommands(bool show)
    {
        gameplayCommands.SetActive(show);
        gameManager = FindObjectOfType<GameManager>();
    }

    public void SetClockTime(int time)
    {

    }

    public void EndGame()
    {
        gameManager.EndGameDebug();
    }

    public void SetStockScore(int stockScore)
    {

    }

    public void ResetPlayerPositions()
    {

    }
}
