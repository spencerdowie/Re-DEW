using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    private void Start()
    {
#if PLATFORM_WEBGL
        GameObject.Find("Quit").SetActive(false);
#endif
    }

    public void LoadGame()
    {
        SceneManager.LoadScene(3);
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
