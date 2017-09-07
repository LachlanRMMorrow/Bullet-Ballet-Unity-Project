using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class SettingsManager : MonoBehaviour
{
    public GameObject graphicsMenu;
    public GameObject soundsMenu;

    public Toggle fullscreenToggle;
    public Dropdown resolutionDropdown;
    public Dropdown textureQualityDropdown;
    public Dropdown antialiasingDropdwon;
    public Slider masterVolumeSlider;
    public Slider bgmMusicVolumeSlider;
    public Slider sfxVolumeSlider;
    public Toggle masterVolumeMuteToggle;
    public Toggle bgmMusicMuteToggle;
    public Toggle sfxMuteToggle;
    public Button applyButton;

    public Resolution[] resolutions;

    public GameSettings gameSettings;

    SoundManager soundMan;

    void Start()
    {

        graphicsMenu = GameObject.Find("Graphics");
        soundsMenu = GameObject.Find("Sounds");

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
        applyButton = GameObject.Find("Apply").GetComponent<Button>();

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
        applyButton.onClick.AddListener(delegate { OnApplyButtonClick(); });

        resolutions = Screen.resolutions;
        foreach(Resolution resolution in resolutions)
        {
            resolutionDropdown.options.Add(new Dropdown.OptionData(resolution.ToString()));
        }

        soundMan = SoundManager.GetInstance();

        LoadSettings();

        soundsMenu.SetActive(false);

    }

    public void OnFullscreenToggle()
    {
        gameSettings.fullscreen = Screen.fullScreen = fullscreenToggle.isOn;
    }

    public void OnResolutionChange()
    {
        Screen.SetResolution(resolutions[resolutionDropdown.value].width, resolutions[resolutionDropdown.value].height, Screen.fullScreen);
        gameSettings.resolutionIndex = resolutionDropdown.value;
    }

    public void OnTextureChange()
    {
        QualitySettings.masterTextureLimit = gameSettings.textureQuality = textureQualityDropdown.value;
    }

    public void OnAntialiasingChange()
    {
        QualitySettings.antiAliasing = (int)Mathf.Pow(2.0f, antialiasingDropdwon.value);
        gameSettings.antialiasing = antialiasingDropdwon.value;
    }

    public void OnMasterVolumeChange()
    {
        gameSettings.bgmMusicVolume = bgmMusicVolumeSlider.value;
        gameSettings.sfxVolume = sfxVolumeSlider.value;
        SoundManager.SetMusicVolume(masterVolumeSlider.value);
        SoundManager.SetSFXVolume(masterVolumeSlider.value);

    }

    public void OnMusicVolumeChange()
    {
        gameSettings.bgmMusicVolume = bgmMusicVolumeSlider.value;
        SoundManager.SetMusicVolume(bgmMusicVolumeSlider.value);
    }

    public void OnSFXVolumeChange()
    {
        gameSettings.sfxVolume = sfxVolumeSlider.value;
        SoundManager.SetSFXVolume(sfxVolumeSlider.value);
    }

    public void OnMasterVolumeMute()
    {
        gameSettings.masterVolumeMute = masterVolumeMuteToggle.isOn;
        if (masterVolumeMuteToggle.isOn == true)
        {
            SoundManager.SetGlobalVolume(0);
        }
        else
        {
            SoundManager.SetMusicVolume(bgmMusicVolumeSlider.value);
            SoundManager.SetSFXVolume(sfxVolumeSlider.value);
        }
    }

    public void OnMusicMute()
    {
        gameSettings.bgmMusicMute = bgmMusicMuteToggle.isOn;
        if (bgmMusicMuteToggle.isOn == true)
        {
            SoundManager.SetMusicVolume(0);
        }
        else
        {
            SoundManager.SetMusicVolume(bgmMusicVolumeSlider.value);
        }
    }

    public void OnSFXMute()
    {
        gameSettings.sfxMute = sfxMuteToggle.isOn;
        if (sfxMuteToggle.isOn == true)
        {
            SoundManager.SetSFXVolume(0);
        }
        else
        {
            SoundManager.SetSFXVolume(sfxVolumeSlider.value);
        }
    }

    public void OnApplyButtonClick()
    {
        SaveSettings();
    }

    public void SaveSettings()
    {
        graphicsMenu.SetActive(true);
        soundsMenu.SetActive(true);
        string jsonData = JsonUtility.ToJson(gameSettings, true);
        File.WriteAllText(Application.persistentDataPath + "/gamesettings.json", jsonData);
        soundsMenu.SetActive(false);
    }

    public void LoadSettings()
    {
        File.ReadAllText(Application.persistentDataPath + "/gamesettings.json");
        gameSettings = JsonUtility.FromJson<GameSettings>(File.ReadAllText(Application.persistentDataPath + "/gamesettings.json"));
        fullscreenToggle.isOn = gameSettings.fullscreen;
        Screen.fullScreen = gameSettings.fullscreen;
        resolutionDropdown.value = gameSettings.resolutionIndex;
        textureQualityDropdown.value = gameSettings.textureQuality;
        antialiasingDropdwon.value = gameSettings.antialiasing;
        masterVolumeSlider.value = gameSettings.masterVolume;
        bgmMusicVolumeSlider.value = gameSettings.bgmMusicVolume;
        sfxVolumeSlider.value = gameSettings.sfxVolume;
        masterVolumeMuteToggle.isOn = gameSettings.masterVolumeMute;
        bgmMusicMuteToggle.isOn = gameSettings.bgmMusicMute;
        sfxMuteToggle.isOn = gameSettings.sfxMute;

    }

}
