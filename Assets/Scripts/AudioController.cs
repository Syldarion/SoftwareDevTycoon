using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class AudioController : Singleton<AudioController>
{
    public enum Channel
    {
        Master,
        SFX,
        Music
    }

    public InputField MasterVolumeInput;
    public InputField SFXVolumeInput;
    public InputField MusicVolumeInput;

    public AudioMixer MainMixer;

    private readonly string[] exposedVolumeParameters = {
        "MasterVolume",
        "SFXVolume",
        "MusicVolume"
    };
    private int internalMasterVol;
    private int internalSFXVol;
    private int internalMusicVol;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        SetMasterVolume("50");
        SetSFXVolume("50");
        SetMusicVolume("50");
    }

    void Update()
    {
        
    }

    public void SetMasterVolume(string value)
    {
        int new_value;
        if(!int.TryParse(value, out new_value)) new_value = 50;
        new_value = Mathf.Clamp(new_value, 0, 100);
        internalMasterVol = new_value;
        SetVolume(Channel.Master, new_value / 2.0f - 30.0f);
        MasterVolumeInput.text = new_value.ToString();
    }

    public void SetSFXVolume(string value)
    {
        int new_value;
        if (!int.TryParse(value, out new_value)) new_value = 50;
        new_value = Mathf.Clamp(new_value, 0, 100);
        internalSFXVol = new_value;
        SetVolume(Channel.SFX, new_value / 2.0f - 30.0f);
        SFXVolumeInput.text = new_value.ToString();
    }

    public void SetMusicVolume(string value)
    {
        int new_value;
        if (!int.TryParse(value, out new_value)) new_value = 50;
        new_value = Mathf.Clamp(new_value, 0, 100);
        internalMusicVol = new_value;
        SetVolume(Channel.Music, new_value / 2.0f - 30.0f);
        MusicVolumeInput.text = new_value.ToString();
    }

    public void SetVolume(Channel channel, float value)
    {
        MainMixer.SetFloat(exposedVolumeParameters[(int)channel], value);
    }

    public float GetVolume(Channel channel)
    {
        float value;
        MainMixer.GetFloat(exposedVolumeParameters[(int)channel], out value);
        return value;
    }

    public void LoadAudioSettings()
    {
        SetMasterVolume(PlayerPrefs.GetInt("MasterVolume", 50).ToString());
        SetSFXVolume(PlayerPrefs.GetInt("SFXVolume", 50).ToString());
        SetMusicVolume(PlayerPrefs.GetInt("MusicVolume", 50).ToString());
    }

    public void SaveAudioSettings()
    {
        PlayerPrefs.SetInt("MasterVolume", internalMasterVol);
        PlayerPrefs.SetInt("SFXVolume", internalSFXVol);
        PlayerPrefs.SetInt("MusicVolume", internalMusicVol);
    }
}
