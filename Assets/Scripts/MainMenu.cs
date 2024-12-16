using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
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
}
