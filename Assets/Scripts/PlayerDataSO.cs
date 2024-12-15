using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDataSO : ScriptableObject
{
    [SerializeField]
    public Color[] playerColours;
    [SerializeField]
    private float respawnTime;
    public float RespawnTime { get => respawnTime; }
    public GameObject playerCharacterPrefab;
}
