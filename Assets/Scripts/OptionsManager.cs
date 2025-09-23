using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class OptionsManager : MonoBehaviour
{
    [SerializeField]
    private Selectable firstSelected = null;
    [SerializeField]
    private TMPro.TextMeshProUGUI masterValue, BGMValue, SFXValue;
    [SerializeField]
    private Toggle bloomToggle;
    [SerializeField]
    private Slider masterSlider, BGMSlider, SFXSlider;
    private Selectable selectedSetting = null;
    [SerializeField]
    private float SelectDelay = 0.1f;
    [SerializeField]
    private int startVolume = 16;
    [SerializeField]
    private AudioMixer BGMMixer, SFXMixer;

    private GameObject returnSelection;
    private Player controllingPlayer;

    private void LoadOptionValues()
    {
        masterSlider.SetValueWithoutNotify(startVolume);
        SetMasterVolume(startVolume);
        BGMSlider.SetValueWithoutNotify(startVolume);
        SetBGMVolume(startVolume);
        SFXSlider.SetValueWithoutNotify(startVolume);
        SetSFXVolume(startVolume);
        bloomToggle.SetIsOnWithoutNotify(true);
        SetBloom(true);
    }

    public void OpenOptions(GameObject returnSelection, Player controllingPlayer)
    {
        this.returnSelection = returnSelection;
        gameObject.SetActive(true);
        firstSelected.Select();
        LoadOptionValues();

        this.controllingPlayer = controllingPlayer;
        this.controllingPlayer.PlayerInput.actions["Cancel"].performed += CloseMenu;
    }

    public void SelectSetting(Selectable selected)
    {
        selectedSetting = selected;
    }

    private void CloseMenu(InputAction.CallbackContext ctx)
    {
        if (selectedSetting != null)
        {
            selectedSetting.Select();
            selectedSetting = null;
        }
        else
        {
            //Close Options Menu
            controllingPlayer.PlayerInput.actions["Cancel"].performed -= CloseMenu;
            EventSystem.current.SetSelectedGameObject(returnSelection);
            gameObject.SetActive(false);
        }
    }

    public void SetBloom(bool bloom)
    {

    }

    public void SetMasterVolume(float newVolume)
    {
        AudioListener.volume = Mathf.Clamp01(newVolume / 20);
        masterValue.text = (newVolume * 5).ToString();
    }

    public void SetBGMVolume(float newVolume)
    {
        BGMMixer.SetFloat("volume", Mathf.Clamp((newVolume * 5) - 80f, -80f, 20f));
        BGMValue.text = (newVolume * 5).ToString();
    }

    public void SetSFXVolume(float newVolume)
    {
        SFXMixer.SetFloat("volume", Mathf.Clamp((newVolume * 5) - 80f, -80f, 20f));
        SFXValue.text = (newVolume * 5).ToString();
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
}
