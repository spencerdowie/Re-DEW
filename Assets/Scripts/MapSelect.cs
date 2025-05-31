using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MapSelect : MonoBehaviour
{
    [SerializeField]
    private PlayerDataSO playerData;
    [SerializeField]
    private Transform mapButtonHolder;
    [SerializeField]
    private GameObject mapButtonPrefab;
    [SerializeField]
    private Image selectedMapImage;
    [SerializeField]
    private GameObject readyBanner;
    private int selectedMapIndex = 1;
    private GameSetting gameSetting;
    private bool mapConfirmed = false;

    private void Awake()
    {
        int index = 0;
        foreach (MapPreview mapPreview in playerData.Maps)
        {
            MapButton button = Instantiate(mapButtonPrefab, mapButtonHolder)
                .GetComponent<MapButton>();
            button.Setup(index);
            button.onSelect += SelectMap;
            button.GetComponent<Button>().onClick.AddListener(ConfirmMap);
            index++;
        }
        EventSystem.current.SetSelectedGameObject(mapButtonHolder.GetChild(0).gameObject);
    }

    private void OnDestroy()
    {
        foreach(Player player in FindObjectsOfType<Player>())
        {
            player.PlayerInput.actions["Join"].performed -= StartGame;
            player.PlayerInput.actions["Cancel"].performed -= CancelMap;
        }
    }

    public void SelectMap(int mapIndex)
    {
        selectedMapIndex = mapIndex;
        selectedMapImage.sprite = playerData.Maps[mapIndex].sprite;
    }

    public void ConfirmMap()
    {
        foreach (Button mapButton in mapButtonHolder.GetComponentsInChildren<Button>())
        {
            mapButton.interactable = false;
        }
        readyBanner.SetActive(true);
        mapConfirmed = true;
    }

    public void CancelMap(InputAction.CallbackContext ctx)
    {
        foreach (Button mapButton in mapButtonHolder.GetComponentsInChildren<Button>())
        {
            mapButton.interactable = true;
        }
        EventSystem.current.SetSelectedGameObject(mapButtonHolder.GetChild(selectedMapIndex).gameObject);
        readyBanner.SetActive(false);
        mapConfirmed = false;
    }

    public void StartGame(InputAction.CallbackContext ctx)
    {
        if (mapConfirmed)
        {
            StartCoroutine(LoadGameScene(playerData.Maps[selectedMapIndex].sceneID));
        }
    }

    public void Setup(Player[] players, GameSetting gameSetting)
    {
        this.gameSetting = gameSetting;
        foreach (Player player in players)
        {
            if (player == null)
                continue;

            player.PlayerInput.actions["Join"].performed += StartGame;
            player.PlayerInput.actions["Cancel"].performed += CancelMap;
        }
    }

    private IEnumerator LoadGameScene(int mapID)
    {
        FindObjectOfType<AudioListener>().enabled = false;
        yield return SceneManager.LoadSceneAsync(mapID, LoadSceneMode.Additive);

        GameManager gameManager = FindObjectOfType<GameManager>();
        gameManager.Setup(gameSetting, mapID);

        SceneManager.UnloadSceneAsync((int)Scenes.MapSelect);
    }

    public void ReturnToLobby()
    {
        PauseMenu.Instance.ReturnToLobby();
    }
}
