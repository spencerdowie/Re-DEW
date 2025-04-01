using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField]
    private PlayerDataSO playerData;
    private void Start()
    {
        SceneManager.LoadSceneAsync(9, LoadSceneMode.Additive);
#if PLATFORM_WEBGL
        GameObject.Find("Quit").SetActive(false);
#endif
    }

    public void LoadGame()
    {
        SceneManager.LoadScene(playerData.Lobby);
    }

    public void OpenItchio()
    {
        Application.OpenURL("https://augex.itch.io/");
    }

    public void OpenBluesky()
    {
        Application.OpenURL("https://augex.bsky.social");
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
