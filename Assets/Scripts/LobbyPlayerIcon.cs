using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LobbyPlayerIcon : MonoBehaviour
{
    [SerializeField]
    private Image background;
    [SerializeField]
    private GameObject noPlayerPrompt, joinedPrompt, readyPrompt;

    public void SetPlayerColour(Color playerColour)
    {
        playerColour.a = background.color.a;
        background.color = playerColour;
    }

    public void SetPlayerStatus(LobbyManager.LobbyStatus status)
    {
        noPlayerPrompt.SetActive(status == LobbyManager.LobbyStatus.NoPlayer);
        joinedPrompt.SetActive(status == LobbyManager.LobbyStatus.Joined);
        readyPrompt.SetActive(status == LobbyManager.LobbyStatus.Ready);
    }
}
