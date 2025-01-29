using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MapSelect : MonoBehaviour
{
    [SerializeField]
    private PlayerDataSO playerData;
    [SerializeField]
    private Transform mapButtonHolder;
    [SerializeField]
    private Image selectedMapImage;
    private int selectedMapID = 1;
    private GameSetting gameSetting;

    private void Awake()
    {
        int index = 0;
        foreach (MapPreview mapPreview in playerData.Maps)
        {
            MapButton button = Instantiate(playerData.mapSelectButtonPrefab, mapButtonHolder)
                .GetComponent<MapButton>();
            button.Setup(index);
            button.onSelect += SelectMap;
            button.GetComponent<Button>().onClick.AddListener(ConfirmMap);
            index++;
        }
        FindObjectOfType<EventSystem>().SetSelectedGameObject(mapButtonHolder.GetChild(0).gameObject);
    }

    public void SelectMap(int mapID)
    {
        MapPreview preview = playerData.Maps[mapID];
        selectedMapID = preview.sceneID;
        selectedMapImage.sprite = preview.sprite;
    }

    public void ConfirmMap()
    {
        StartCoroutine(LoadGameScene(selectedMapID));
    }

    public void SetGameSetting(GameSetting gameSetting)
    {
        this.gameSetting = gameSetting;
    }


    private IEnumerator LoadGameScene(int mapID)
    {
        yield return SceneManager.LoadSceneAsync(mapID, LoadSceneMode.Additive);

        GameManager gameManager = FindObjectOfType<GameManager>();
        gameManager.Setup(gameSetting, mapID);

        SceneManager.UnloadSceneAsync(playerData.MapSelect);
    }
}
