using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using LobbyStatus = LobbyManager.LobbyStatus;

public class LobbyPlayerIcon : MonoBehaviour
{
    [SerializeField]
    private Material bannerMat;
    [SerializeField]
    private Image background, banner;
    [SerializeField]
    private GameObject noPlayerPrompt, joinedPrompt, readyPrompt;
    private LobbyStatus status = LobbyStatus.NoPlayer;
    [SerializeField]
    private Transform playerModelHolder;
    [SerializeField]
    private GameObject modelPicker;

    private void Awake()
    {
        banner.material = Instantiate(bannerMat);
        banner.enabled = false;
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

        banner.enabled = status != LobbyStatus.NoPlayer;
        //modelPicker.SetActive(status == LobbyStatus.Joined);

        this.status = status;
    }

    public void SetPlayerModel(GameObject newPlayerModel)
    {
        Destroy(playerModelHolder.GetChild(0).gameObject);

        Instantiate(newPlayerModel, playerModelHolder);
    }

    public void ShowPlayerModel(bool show = true)
    {
        playerModelHolder.gameObject.SetActive(show);
    }
}
