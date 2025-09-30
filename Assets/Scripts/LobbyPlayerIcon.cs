using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using LobbyStatus = LobbyManager.LobbyStatus;

public class LobbyPlayerIcon : Selectable
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
    [SerializeField]
    private GameObject xboxIcons, psIcons, highlights;
    [SerializeField]
    private float HighlightTime = 0.1f;
    private Image[] controlIcons = new Image[6];

    private new void Awake()
    {
        base.Awake();
        banner.material = Instantiate(bannerMat);
        SetPlayerStatus(LobbyStatus.NoPlayer);
        controlIcons = highlights.GetComponentsInChildren<Image>();

        //If Images have no alpha they don't work, so need to remove it after load
        foreach (Image control in controlIcons)
        {
            control.CrossFadeAlpha(0, 0, true);
        }
    }

    public void SetControllerType(ControllerType controllerType)
    {
        if (controllerType == ControllerType.Xbox)
        {
            xboxIcons.SetActive(true);
            psIcons.SetActive(false);
        }
        else
        {
            xboxIcons.SetActive(false);
            psIcons.SetActive(true);
        }
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

    public void HighlightControl(int control, int direction)
    {
        int controlIndex = (control * 2) + Math.Max(direction, 0);
        StartCoroutine(HighlightControlDelay(controlIcons[controlIndex]));
    }

    private IEnumerator HighlightControlDelay(Image control)
    {
        control.CrossFadeAlpha(1, 0, true);
        yield return new WaitForSeconds(HighlightTime);
        control.CrossFadeAlpha(0, 0, true);
    }
}
