using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class DebugMenu : MonoBehaviour
{
    [SerializeField]
    private PlayerDataSO playerData;
    private GameObject debugMenu;

    private void Awake()
    {
        debugMenu = transform.GetChild(0).gameObject;
        debugMenu.SetActive(false);
        InputSystem.actions["Debug"].started += ToggleDebug;
    }

    private void ToggleDebug(InputAction.CallbackContext ctx)
    {
        if (debugMenu.activeSelf)
        {
            debugMenu.SetActive(false);
        }
        else
        {
            debugMenu.SetActive(true);
        }
    }
}
