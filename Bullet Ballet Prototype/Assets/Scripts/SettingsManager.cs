using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsManager : MonoBehaviour
{
    public Toggle fullscreenToggle;
    public Dropdown resolutionDropdown;
    public Dropdown textureQualityDropdown;
    public Dropdown antialiasingDropdwon;
    public Dropdown vSyncDropdown;
    public Slider masterVolumeSlider;
    public Slider bgmMusicVolumeSlider;
    public Slider sfxVolumeSlider;
    public Toggle masterVolumeMuteToggle;
    public Toggle bgmMusicMuteToggle;
    public Toggle sfxMuteToggle;

    public Resolution[] resolutions;

    public GameSettings gameSettings;

    SoundManager soundMan;

    void Start()
    {

        gameSettings = new GameSettings();

        fullscreenToggle = GameObject.Find("Fullscreen").GetComponent<Toggle>();
        resolutionDropdown = GameObject.Find("Resolution").GetComponent<Dropdown>();
        textureQualityDropdown = GameObject.Find("Texture Quality").GetComponent<Dropdown>();
        antialiasingDropdwon = GameObject.Find("Antialiasing").GetComponent<Dropdown>();
        masterVolumeSlider = GameObject.Find("Master Volume Slider").GetComponent<Slider>();
        bgmMusicVolumeSlider = GameObject.Find("Music Volume Slider").GetComponent<Slider>();
        sfxVolumeSlider = GameObject.Find("SFX Volume Slider").GetComponent<Slider>();
        masterVolumeMuteToggle = GameObject.Find("Master Volume Mute").GetComponent<Toggle>();
        bgmMusicMuteToggle = GameObject.Find("SFX Volume Mute").GetComponent<Toggle>();
        sfxMuteToggle = GameObject.Find("Music Volume Mute").GetComponent<Toggle>();

        fullscreenToggle.onValueChanged.AddListener(delegate { OnFullscreenToggle(); });
        resolutionDropdown.onValueChanged.AddListener(delegate { OnResolutionChange(); });
        textureQualityDropdown.onValueChanged.AddListener(delegate { OnTextureChange(); });
        antialiasingDropdwon.onValueChanged.AddListener(delegate { OnAntialiasingChange(); });
        masterVolumeSlider.onValueChanged.AddListener(delegate { OnMasterVolumeChange(); });
        bgmMusicVolumeSlider.onValueChanged.AddListener(delegate { OnMusicVolumeChange(); });
        sfxVolumeSlider.onValueChanged.AddListener(delegate { OnSFXVolumeChange(); });
        masterVolumeMuteToggle.onValueChanged.AddListener(delegate { OnMasterVolumeMute(); });
        bgmMusicMuteToggle.onValueChanged.AddListener(delegate { OnMusicMute(); });
        sfxMuteToggle.onValueChanged.AddListener(delegate { OnSFXMute(); });

        resolutions = Screen.resolutions;

        soundMan = SoundManager.GetInstance();

    }

    void OnFullscreenToggle()
    {
        gameSettings.fullscreen = Screen.fullScreen = fullscreenToggle.isOn;
    }

    void OnResolutionChange()
    {
        
    }

    void OnTextureChange()
    {
        QualitySettings.masterTextureLimit = gameSettings.textureQuality = textureQualityDropdown.value;
    }

    void OnAntialiasingChange()
    {

    }

    void OnMasterVolumeChange()
    {
        
    }

    void OnMusicVolumeChange()
    {

    }

    void OnSFXVolumeChange()
    {

    }

    void OnMasterVolumeMute()
    {

    }

    void OnMusicMute()
    {

    }

    void OnSFXMute()
    {

    }

}
