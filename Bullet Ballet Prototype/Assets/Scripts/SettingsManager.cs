using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using UnityEngine.PostProcessing;

public class SettingsManager : MonoBehaviour
{
    public GameObject graphicsMenu;
    public GameObject soundsMenu;
    public GameObject inGameOptionsMenu;


    public Toggle fullscreenToggle;
    public Toggle vSyncToggle;
    public Toggle motionBlueToggle;
    public Toggle postProcessingToggle;
    public Dropdown resolutionDropdown;
    public Dropdown textureQualityDropdown;
    public Dropdown antialiasingDropdwon;
    public Slider gammaSlider;
    public Slider masterVolumeSlider;
    public Slider bgmMusicVolumeSlider;
    public Slider sfxVolumeSlider;
    public Toggle masterVolumeMuteToggle;
    public Toggle bgmMusicMuteToggle;
    public Toggle sfxMuteToggle;
    public Button applyButton;


    public bool test;

    //********************************************************* Post Processing variables *********************************************

    public PostProcessingProfile postProcessing;


    //*******************************************************************************************************************************

    public Resolution[] resolutions;

    public GameSettings gameSettings;

    SoundManager soundMan;

    void Awake()
    {

        gameSettings = new GameSettings();

        if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex == 2)
        {
            GetUiElements();
        }
        else
        {
            inGameOptionsMenu = GameObject.Find("Canvas").transform.Find("Options Menu").gameObject;
            inGameOptionsMenu.SetActive(true);
            GetUiElements();
            inGameOptionsMenu.SetActive(false);
        }


        resolutions = Screen.resolutions;
        foreach (Resolution resolution in resolutions)
        {
            resolutionDropdown.options.Add(new Dropdown.OptionData(resolution.ToString()));
        }

        soundMan = SoundManager.GetInstance();

        LoadSettings();

    }

    void GetUiElements()
    {
        fullscreenToggle = GameObject.Find("Fullscreen").GetComponent<Toggle>();
        vSyncToggle = GameObject.Find("Verticle Sync").GetComponent<Toggle>();
        motionBlueToggle = GameObject.Find("Motion Blur").GetComponent<Toggle>();
        postProcessingToggle = GameObject.Find("Post Processing").GetComponent<Toggle>();
        resolutionDropdown = GameObject.Find("Resolution").GetComponent<Dropdown>();
        textureQualityDropdown = GameObject.Find("Texture Quality").GetComponent<Dropdown>();
        antialiasingDropdwon = GameObject.Find("Antialiasing").GetComponent<Dropdown>();
        gammaSlider = GameObject.Find("Gamma Slider").GetComponent<Slider>();
        masterVolumeSlider = GameObject.Find("Master Volume Slider").GetComponent<Slider>();
        bgmMusicVolumeSlider = GameObject.Find("Music Volume Slider").GetComponent<Slider>();
        sfxVolumeSlider = GameObject.Find("SFX Volume Slider").GetComponent<Slider>();
        masterVolumeMuteToggle = GameObject.Find("Master Volume Mute").GetComponent<Toggle>();
        bgmMusicMuteToggle = GameObject.Find("SFX Volume Mute").GetComponent<Toggle>();
        sfxMuteToggle = GameObject.Find("Music Volume Mute").GetComponent<Toggle>();
        applyButton = GameObject.Find("Apply").GetComponent<Button>();

        fullscreenToggle.onValueChanged.AddListener(delegate { OnFullscreenToggle(); });
        vSyncToggle.onValueChanged.AddListener(delegate { OnVSyncChange(); });
        motionBlueToggle.onValueChanged.AddListener(delegate { OnMotionBlurChange(); });
        postProcessingToggle.onValueChanged.AddListener(delegate { OnPostProcessingChange(); });
        resolutionDropdown.onValueChanged.AddListener(delegate { OnResolutionChange(); });
        textureQualityDropdown.onValueChanged.AddListener(delegate { OnTextureChange(); });
        antialiasingDropdwon.onValueChanged.AddListener(delegate { OnAntialiasingChange(); });
        gammaSlider.onValueChanged.AddListener(delegate { OnGammaChange(); });
        masterVolumeSlider.onValueChanged.AddListener(delegate { OnMasterVolumeChange(); });
        bgmMusicVolumeSlider.onValueChanged.AddListener(delegate { OnMusicVolumeChange(); });
        sfxVolumeSlider.onValueChanged.AddListener(delegate { OnSFXVolumeChange(); });
        masterVolumeMuteToggle.onValueChanged.AddListener(delegate { OnMasterVolumeMute(); });
        bgmMusicMuteToggle.onValueChanged.AddListener(delegate { OnMusicMute(); });
        sfxMuteToggle.onValueChanged.AddListener(delegate { OnSFXMute(); });
        applyButton.onClick.AddListener(OnApplyButtonClick);
    }

    public void OnFullscreenToggle()
    {
        gameSettings.fullscreen = Screen.fullScreen = fullscreenToggle.isOn;
    }

    public void OnVSyncChange()
    {
        if (vSyncToggle.isOn)
        {
            QualitySettings.vSyncCount = gameSettings.vSync = 1;
        }
        else
        {
            QualitySettings.vSyncCount = gameSettings.vSync = 0;
        }
    }

    public void OnMotionBlurChange()
    {
        if (motionBlueToggle.isOn)
        {
            gameSettings.motionBlur = postProcessing.motionBlur.enabled = true;
        }
        else
        {
            gameSettings.motionBlur = postProcessing.motionBlur.enabled = false;
        }
    }

    public void OnPostProcessingChange()
    {
        if (postProcessingToggle.isOn)
        { 
            gameSettings.postProcessingBool = postProcessing.bloom.enabled = true;
            postProcessing.vignette.enabled = true;
        }
        else
        {
            gameSettings.postProcessingBool = postProcessing.bloom.enabled = false;
            postProcessing.vignette.enabled = false;
        }
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

    public void OnGammaChange()
    {

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
        //graphicsMenu.SetActive(true);
        //soundsMenu.SetActive(true);
        string jsonData = JsonUtility.ToJson(gameSettings, true);
        File.WriteAllText(Application.persistentDataPath + "/gamesettings.json", jsonData);
        //soundsMenu.SetActive(false);
    }

    public void LoadSettings()
    {
        File.ReadAllText(Application.persistentDataPath + "/gamesettings.json");
        gameSettings = JsonUtility.FromJson<GameSettings>(File.ReadAllText(Application.persistentDataPath + "/gamesettings.json"));
        fullscreenToggle.isOn = gameSettings.fullscreen;
        Screen.fullScreen = gameSettings.fullscreen;
        //******************** Magic **************************
        vSyncToggle.isOn = gameSettings.vSync != 0;
        //******************** Magic **************************
        motionBlueToggle.isOn = gameSettings.motionBlur;
        postProcessingToggle.isOn = gameSettings.postProcessingBool;
        resolutionDropdown.value = gameSettings.resolutionIndex;
        textureQualityDropdown.value = gameSettings.textureQuality;
        antialiasingDropdwon.value = gameSettings.antialiasing;
        gammaSlider.value = gameSettings.gamma;
        masterVolumeSlider.value = gameSettings.masterVolume;
        bgmMusicVolumeSlider.value = gameSettings.bgmMusicVolume;
        sfxVolumeSlider.value = gameSettings.sfxVolume;
        masterVolumeMuteToggle.isOn = gameSettings.masterVolumeMute;
        bgmMusicMuteToggle.isOn = gameSettings.bgmMusicMute;
        sfxMuteToggle.isOn = gameSettings.sfxMute;

    }

}
