using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class GameSettingsMenu : MonoBehaviour
{
    private LobbyManager lobbyManager;
    [SerializeField]
    private float SelectDelay = 0.1f;
    [SerializeField]
    private TMPro.TextMeshProUGUI overviewStockText, overviewScoreText, overviewTimeText, gamemodeDescription;
    [SerializeField]
    private GameObject overviewFFA, overviewTeam, overviewStock, overviewScore;
    [SerializeField]
    private Toggle overviewTeamMode, overviewWinCon;
    [SerializeField]
    private TMPro.TextMeshProUGUI gameTimeText, stockText, scoreText;
    [SerializeField]
    private Toggle winConToggle, winConAmtToggle, teamToggle;
    private string[] gameModeDescriptions;
    private Selectable selectedSetting = null;

    public void LoadGameSettings(LobbyManager lobbyManager, GameSetting lastGameSetting, string[] gameModeDescriptions)
    {
        this.lobbyManager = lobbyManager;
        this.gameModeDescriptions = gameModeDescriptions;
        SetGameTime(lastGameSetting.GameTime);
        SetWinCondition(lastGameSetting.WinConBool);
        SetStartStock(lastGameSetting.StartStocks);
        SetScoreLimit(lastGameSetting.ScoreLimit);
        ToggleTeamMode(lastGameSetting.IsTeams);
    }

    public void ToggleTeamMode(bool teamMode)
    {
        lobbyManager.isTeams = teamMode;
        teamToggle.isOn = teamMode;
        overviewTeamMode.isOn = teamMode;
        //overviewFFA.SetActive(!teamMode);
        //overviewTeam.SetActive(teamMode);
    }

    ///<summary>true = stock | false = score</summary>
    public void ToggleWinCondition()
    {
        SetWinCondition(!lobbyManager.winCon);
    }

    public void SetWinCondition(bool winCon)
    {
        lobbyManager.winCon = winCon;
        winConToggle.isOn = winCon;
        winConAmtToggle.isOn = winCon;
        gamemodeDescription.text = gameModeDescriptions[winCon ? 1 : 0];
        overviewWinCon.isOn = winCon;
        //overviewStock.SetActive(winCon);
        //overviewScore.SetActive(!winCon);
    }

    private void SetStartStock(int startStocks)
    {
        lobbyManager.startStocks = startStocks;
        stockText.text = startStocks.ToString();
        overviewStockText.text = startStocks.ToString();
    }

    private void SetScoreLimit(int scoreLimit)
    {
        lobbyManager.scoreLimit = scoreLimit;
        scoreText.text = scoreLimit.ToString();
        overviewScoreText.text = scoreLimit.ToString();
    }

    private void SetGameTime(int gameTime)
    {
        lobbyManager.gameTime = gameTime;
        gameTimeText.text = gameTime.ToString();
        overviewTimeText.text = gameTime.ToString();
    }

    public void ChangeStockScore(int change)
    {
        if (lobbyManager.winCon)
        {
            int newStock = lobbyManager.startStocks + change;
            if (newStock > 0 && newStock <= lobbyManager.maxStocks)
                SetStartStock(newStock);
        }
        else
        {
            int newScore = lobbyManager.scoreLimit + change;
            if (newScore > 0 && newScore <= lobbyManager.maxScore)
                SetScoreLimit(newScore);
        }
    }

    public void ChangeGameTime(int change)
    {
        int newTime = lobbyManager.gameTime + change;
        if (newTime > 0 && newTime <= lobbyManager.maxTime)
            SetGameTime(newTime);

    }

    public void SelectSetting(Selectable selected)
    {
        selectedSetting = selected;
    }

    public void SelectNext(GameObject gameObject)
    {
        StartCoroutine(SelectNextDelay(gameObject));
    }

    private IEnumerator SelectNextDelay(GameObject gameObject)
    {
        yield return new WaitForSeconds(SelectDelay);
        EventSystem.current.SetSelectedGameObject(gameObject);
    }

    public void CloseMenu(InputAction.CallbackContext ctx)
    {
        if (selectedSetting)
        {
            selectedSetting.Select();
            selectedSetting = null;
        }
        else
            lobbyManager.OpenSettingsMenu(false);
    }

}
