using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
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
    private int masterVolume = 16, BGMVolume = 16, SFXVolume = 16;
    [SerializeField]
    private bool bloomValue = true;
    [SerializeField]
    private AudioMixer BGMMixer, SFXMixer;
    [SerializeField]
    private VolumeProfile volume;

    private GameObject returnSelection;
    private Player controllingPlayer;

    private void LoadOptionValues()
    {
        masterSlider.SetValueWithoutNotify(masterVolume);
        SetMasterVolume(masterVolume);
        BGMSlider.SetValueWithoutNotify(BGMVolume);
        SetBGMVolume(BGMVolume);
        SFXSlider.SetValueWithoutNotify(SFXVolume);
        SetSFXVolume(SFXVolume);
        bloomToggle.SetIsOnWithoutNotify(bloomValue);
        SetBloom(bloomValue);
    }

    public void OpenOptions(GameObject returnSelection, Player controllingPlayer)
    {
        this.returnSelection = returnSelection;
        gameObject.SetActive(true);
        firstSelected.Select();
        LoadOptionValues();

        this.controllingPlayer = controllingPlayer;
        this.controllingPlayer.PlayerInput.actions["Cancel"].performed += CloseMenuCallback;
    }

    public void SelectSetting(Selectable selected)
    {
        selectedSetting = selected;
    }

    private void CloseMenuCallback(InputAction.CallbackContext ctx)
    {
        if (selectedSetting != null)
        {
            selectedSetting.Select();
            selectedSetting = null;
        }
        else
        {
            CloseOptionsMenu();
        }
    }

    public void CloseOptionsMenu()
    {
        //Close Options Menu
        controllingPlayer.PlayerInput.actions["Cancel"].performed -= CloseMenuCallback;
        EventSystem.current.SetSelectedGameObject(returnSelection);
        gameObject.SetActive(false);

    }

    public void SetBloom(bool bloomEnabled)
    {
        if (volume.TryGet(out Bloom bloomVolume))
        {
            bloomVolume.active = bloomEnabled;
            bloomValue = bloomEnabled;
        }
    }

    public void SetMasterVolume(float newVolume)
    {
        AudioListener.volume = Mathf.Clamp01(newVolume / 20);
        masterValue.text = (newVolume * 5).ToString();
        masterVolume = (int)newVolume;
    }

    public void SetBGMVolume(float newVolume)
    {
        BGMMixer.SetFloat("volume", Mathf.Clamp((newVolume * 5) - 80f, -80f, 20f));
        BGMValue.text = (newVolume * 5).ToString();
        BGMVolume = (int)newVolume;
    }

    public void SetSFXVolume(float newVolume)
    {
        SFXMixer.SetFloat("volume", Mathf.Clamp((newVolume * 5) - 80f, -80f, 20f));
        SFXValue.text = (newVolume * 5).ToString();
        SFXVolume = (int)newVolume;
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
