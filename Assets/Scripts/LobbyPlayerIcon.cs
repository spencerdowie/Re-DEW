using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using LobbyStatus = LobbyManager.LobbyStatus;

public class LobbyPlayerIcon : MonoBehaviour
{
    [SerializeField]
    private Material bannerMat;
    private Material defaultMat;
    [SerializeField]
    private Image background, banner;
    [SerializeField]
    private GameObject noPlayerPrompt, joinedPrompt, readyPrompt;
    private LobbyStatus status = LobbyStatus.NoPlayer;

    private void Awake()
    {
        defaultMat = banner.material;
    }

    public void SetPlayerColour(Color playerColour)
    {
        playerColour.a = background.color.a;
        background.color = playerColour;
        playerColour.a = banner.color.a;

        banner.color = playerColour;
        banner.material.color = playerColour;

    }

    public void SetPlayerStatus(LobbyStatus status)
    {
        noPlayerPrompt.SetActive(status == LobbyStatus.NoPlayer);
        joinedPrompt.SetActive(status == LobbyStatus.Joined);
        readyPrompt.SetActive(status == LobbyStatus.Ready);

        if (this.status == LobbyStatus.NoPlayer)
        {
            banner.material = Instantiate(bannerMat);
        }
        else if (status == LobbyStatus.NoPlayer)
        {
            banner.material = Instantiate(defaultMat);
        }

        this.status = status;
    }
}
