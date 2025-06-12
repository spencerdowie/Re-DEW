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
    private GameObject portraitBox;
    [SerializeField]
    private GameObject noPlayerPrompt, joinedPrompt, readyPrompt;
    [SerializeField]
    private Transform playerModelHolder;
    [SerializeField]
    private TMPro.TextMeshProUGUI weaponNameText;

    private void Awake()
    {
        banner.material = Instantiate(bannerMat);
        SetPlayerStatus(LobbyStatus.NoPlayer);
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

        if (status == LobbyStatus.NoPlayer)
        {
            SetPlayerColour(Color.grey);
            playerModelHolder.gameObject.SetActive(false);
            portraitBox.SetActive(false);
        }
        else if (status > LobbyStatus.NoPlayer)
        {
            playerModelHolder.gameObject.SetActive(true);
            portraitBox.SetActive(true);
        }
    }

    public void SetPlayerModel(GameObject newPlayerModel)
    {
        Destroy(playerModelHolder.GetChild(0).gameObject);

        Instantiate(newPlayerModel, playerModelHolder);
    }

    public void SetPlayerWeapon(string weaponName)
    {
        weaponNameText.text = weaponName;
    }
}
