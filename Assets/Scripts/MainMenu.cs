using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField]
    private PlayerDataSO playerData;
    [SerializeField]
    private GameObject StartButton;

    private void Start()
    {
        SceneManager.LoadSceneAsync((int)Scenes.DebugMenu, LoadSceneMode.Additive);
        EventSystem.current.SetSelectedGameObject(StartButton);

        if (!playerData.LastGameSettings.IsInitialized)
        {
            playerData.LastGameSettings = playerData.DefaultGameSettings();
        }
        Debug.Log(playerData.LastGameSettings.ToString());

#if PLATFORM_WEBGL
        GameObject.Find("Quit").SetActive(false);
#endif
    }

    public void LoadGame()
    {
        SceneManager.LoadScene((int)Scenes.Lobby);
    }

    public void OpenItchio()
    {
        Debug.Log("Open Itchio");
        Application.OpenURL("https://augex.itch.io/");
    }

    public void OpenBluesky()
    {
        Debug.Log("Open Bsky");
        Application.OpenURL("https://augex.bsky.social");
    }

    public void OpenSteam()
    {
        Debug.Log("Open Steam");
        Application.OpenURL("https://store.steampowered.com/app/3727560/Delta_Epsilon_Warriors");
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
