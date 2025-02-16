using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LobbyPlayerIcon : MonoBehaviour
{
    [SerializeField]
    private Material bannerMat;
    [SerializeField]
    private Image background, banner;
    [SerializeField]
    private GameObject noPlayerPrompt, joinedPrompt, readyPrompt;

    private void Awake()
    {
        Debug.Log(banner.material.name);
        banner.material = Instantiate(bannerMat);
    }

    public void SetPlayerColour(Color playerColour)
    {
        playerColour.a = background.color.a;
        background.color = playerColour;

        if (banner.material.name != "Default UI Material")
        {
            playerColour.a = banner.color.a;

            banner.color = playerColour;
            banner.material.SetColor("_Color", playerColour);
        }
    }

    public void SetPlayerStatus(LobbyManager.LobbyStatus status)
    {
        noPlayerPrompt.SetActive(status == LobbyManager.LobbyStatus.NoPlayer);
        joinedPrompt.SetActive(status == LobbyManager.LobbyStatus.Joined);
        readyPrompt.SetActive(status == LobbyManager.LobbyStatus.Ready);
    }
}
