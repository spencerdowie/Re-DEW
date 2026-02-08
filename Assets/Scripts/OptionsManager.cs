using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class OptionsManager : MonoBehaviour
{
    [SerializeField]
    private PlayerDataSO playerData;
    [SerializeField]
    Settings settings = new Settings(16, 16, 16, true);
    [SerializeField]
    private AudioMixer BGMMixer, SFXMixer;
    [SerializeField]
    private VolumeProfile vfxVolume;

    private void Start()
    {
        if (PlayerPrefs.HasKey("Settings"))
        {
            playerData.settings = Settings.FromJSON(PlayerPrefs.GetString("Settings"));
        }

        if (!playerData.LastGameSettings.IsInitialized)
        {
            playerData.LastGameSettings = playerData.DefaultGameSettings();
        }
        //Debug.Log(playerData.LastGameSettings.ToString());
        LoadOptionValues();
    }

    private void LoadOptionValues()
    {
        settings = playerData.settings;
        SetMasterVolume(settings.MasterVolume);
        SetBGMVolume(settings.BGMVolume);
        SetSFXVolume(settings.SFXVolume);
        SetBloom(settings.BloomValue);
    }

    public void SetBloom(bool bloomEnabled)
    {
        if (vfxVolume.TryGet(out Bloom bloomVolume))
        {
            bloomVolume.active = bloomEnabled;
            settings.BloomValue = bloomEnabled;
        }
    }

    public void SetMasterVolume(float newVolume)
    {
        AudioListener.volume = Mathf.Clamp01(newVolume / 20);
        settings.MasterVolume = (int)newVolume;
    }

    public void SetBGMVolume(float newVolume)
    {
        //70-100
        float rescaled = Mathf.Lerp(0, 1, newVolume / 20);
        BGMMixer.SetFloat("volume", Mathf.Clamp((rescaled * 20), -10f, 10f));
        settings.BGMVolume = (int)newVolume;
    }

    public void SetSFXVolume(float newVolume)
    {
        SFXMixer.SetFloat("volume", Mathf.Clamp((newVolume * 5) - 80f, -80f, 20f));
        settings.SFXVolume = (int)newVolume;
    }
}
