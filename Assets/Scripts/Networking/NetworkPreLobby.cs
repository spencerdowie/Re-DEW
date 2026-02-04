using UnityEngine;
using TMPro;

public class NetworkPreLobby : MonoBehaviour
{
    [SerializeField]
    private TMP_InputField LobbyCodeInput;

    public void CreateLobby()
    {

    }

    public void JoinLobby()
    {
        string lobbyCode = LobbyCodeInput.text;
    }
}
