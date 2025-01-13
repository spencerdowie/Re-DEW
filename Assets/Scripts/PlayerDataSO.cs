using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class PlayerProfile
{
    public string name;
    public int playerColourIndex;
}

public class PlayerDataSO : ScriptableObject
{
    [SerializeField]
    public PlayerProfile[] playerProfiles;

    [SerializeField]
    public Color[] playerColoursOptions;

    [Space, Header("Player Settings")]
    public GameObject playerCharacterPrefab;
    [field: SerializeField]
    public float MoveSpeed { get; private set; } = 2f;
    [field: SerializeField]
    public float SpeedChangeRate { get; private set; } = 10f;
    [field: SerializeField, Range(0f, 0.3f)]
    public float RotationSmoothTime { get; private set; } = 0.12f;
    [field: SerializeField]
    public float RespawnTime { get; private set; } = 3f;
    [field: SerializeField]
    public float RespawnInvulnTime { get; private set; } = 2f;

    [Space, Header("Laser Settings")]
    public GameObject laserPrefab;
    [field: SerializeField]
    public float LaserSpeed { get; private set; } = 2f;
    [field: SerializeField]
    public float LaserHeight { get; private set; } = 0.2f;
    [field: SerializeField]
    public float LaserMaxDistance { get; private set; } = 10f;
    [field: SerializeField]
    public int MaxSegments { get; private set; } = 3;
    [field: SerializeField]
    public float LaserLifetime { get; private set; } = 5f;

    [Space, Header("Game Settings")]
    public int mapScene = 4;
    [field: SerializeField]
    public int GameTime { get; private set; } = 300;
    [field: SerializeField]
    public int ScoreLimit { get; private set; } = 10;
}
