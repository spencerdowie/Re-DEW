using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

public class OptionsUI : MonoBehaviour
{
    [SerializeField]
    private PlayerDataSO playerData;
    [SerializeField]
    private Selectable firstSelected = null, returnSelection = null;
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
    Settings settings = new Settings(16, 16, 16, true);
    [SerializeField]
    private AudioMixer BGMMixer, SFXMixer;
    [SerializeField]
    private VolumeProfile volume;

    private Player controllingPlayer;

    private UnityEvent onOptionClose = new UnityEvent();

    private void LoadOptionValues()
    {
        settings = playerData.settings;
        masterSlider.SetValueWithoutNotify(settings.MasterVolume);
        SetMasterVolume(settings.MasterVolume);
        BGMSlider.SetValueWithoutNotify(settings.BGMVolume);
        SetBGMVolume(settings.BGMVolume);
        SFXSlider.SetValueWithoutNotify(settings.SFXVolume);
        SetSFXVolume(settings.SFXVolume);
        bloomToggle.SetIsOnWithoutNotify(settings.BloomValue);
        SetBloom(settings.BloomValue);
    }

    public void OpenOptionsMenu(Player controllingPlayer, UnityAction optionCloseCallback = null)
    {
        gameObject.SetActive(true);
        firstSelected.Select();
        LoadOptionValues();

        this.controllingPlayer = controllingPlayer;
        this.controllingPlayer.PlayerInput.actions["Cancel"].performed += CloseMenuCallback;

        if (optionCloseCallback != null)
            onOptionClose.AddListener(optionCloseCallback);
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
        playerData.settings = settings;
        PlayerPrefs.SetString("Settings", settings.ToJSON());
        controllingPlayer.PlayerInput.actions["Cancel"].performed -= CloseMenuCallback;
        returnSelection.Select();
        onOptionClose?.Invoke();
        gameObject.SetActive(false);
        onOptionClose.RemoveAllListeners();
    }

    public void SetBloom(bool bloomEnabled)
    {
        if (volume.TryGet(out Bloom bloomVolume))
        {
            bloomVolume.active = bloomEnabled;
            settings.BloomValue = bloomEnabled;
        }
    }

    public void SetMasterVolume(float newVolume)
    {
        AudioListener.volume = Mathf.Clamp01(newVolume / 20);
        masterValue.text = (newVolume * 5).ToString();
        settings.MasterVolume = (int)newVolume;
    }

    public void SetBGMVolume(float newVolumeRaw)
    {
        float newVolume = -80f;
        if (newVolumeRaw > 0)
        {
            float rescaledVolume = Mathf.Lerp(0, 1, newVolumeRaw / 20);
            newVolume = Mathf.Clamp(rescaledVolume * 20, -10f, 10f);
        }
        BGMMixer.SetFloat("volume", newVolume);
        BGMValue.text = (newVolumeRaw * 5).ToString();
        settings.BGMVolume = (int)newVolumeRaw;
    }

    public void SetSFXVolume(float newVolumeRaw)
    {
        float newVolume = -80f;
        if (newVolumeRaw > 0)
        {
            float rescaledVolume = Mathf.Lerp(0, 1, newVolumeRaw / 20);
            newVolume = Mathf.Clamp(rescaledVolume * 20, -10f, 10f);
        }
        SFXMixer.SetFloat("volume", newVolume);
        SFXValue.text = (newVolumeRaw * 5).ToString();
        settings.SFXVolume = (int)newVolumeRaw;
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
