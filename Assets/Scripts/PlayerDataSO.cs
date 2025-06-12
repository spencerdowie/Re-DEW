using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class PlayerProfile
{
    public string name;
    public int playerColourIndex;
}

[Serializable]
public struct MapPreview
{
    public int sceneID;
    public Sprite sprite;
    public string MapName;
    public string MapDescription;
}

[Serializable]
public struct WeaponData
{
    public string WeaponName;
    public GameObject LaserPrefab;
}

public enum Scenes
{
    MainMenu = 0,
    PauseMenu = 1,
    GameUI = 2,
    Lobby = 3,
    MapSelect = 4,
    GameEndScreen = 5,
    DebugMenu = 8
}

public class PlayerDataSO : ScriptableObject
{
    [SerializeField]
    public PlayerProfile[] playerProfiles;

    [SerializeField, ColorUsage(true, true)]
    public Color[] playerColoursOptions;

    public GameObject playerCharacterPrefab;
    public GameObject[] characterPrefabs;
    public WeaponData[] weaponsOptions;


    [field: SerializeField, Header("Player Settings"), Space]
    public float MoveSpeed { get; private set; } = 2f;
    [field: SerializeField]
    public float SpeedChangeRate { get; private set; } = 10f;
    [field: SerializeField, Range(0f, 0.3f)]
    public float RotationSmoothTime { get; private set; } = 0.12f;
    [field: SerializeField]
    public float RespawnTime { get; private set; } = 3f;
    [field: SerializeField]
    public float RespawnInvulnTime { get; private set; } = 2f;
    [field: SerializeField]
    public int StartAmmo { get; private set; } = 3;

    [Space, Header("Default Game Settings"), SerializeField]
    public WinCon winCon;
    [SerializeField]
    public int gameTime = 5, startStocks = 5, scoreLimit = 10;
    [SerializeField]
    public bool isTeams = false;
    public GameSetting LastGameSettings;
    public string[] GameModeDescription { get; private set; } =
        { "Be the most Lethal Warrior in the Arena!",
          "Be the Last Warrior Standing!"};


    [field: SerializeField, Space, Header("Scenes")]
    public MapPreview[] Maps { get; private set; } = new MapPreview[] { };

    public GameSetting DefaultGameSettings()
    {
        return new GameSetting(gameTime, winCon, startStocks, scoreLimit, isTeams);
    }
}
