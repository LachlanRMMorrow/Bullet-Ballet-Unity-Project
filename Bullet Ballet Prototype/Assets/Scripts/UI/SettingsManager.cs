﻿using System.Collections;
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
    public GameObject pauseMenu;


    public Toggle fullscreenToggle;
    public Toggle vSyncToggle;
    public Toggle motionBlueToggle;
    public Toggle postProcessingToggle;
    public Dropdown resolutionDropdown;
    public Dropdown textureQualityDropdown;
    public Dropdown antialiasingDropdwon;
    public Dropdown shadowsDropdown;
    public Slider masterVolumeSlider;
    public Slider bgmMusicVolumeSlider;
    public Slider sfxVolumeSlider;
    public Toggle masterVolumeMuteToggle;
    public Toggle bgmMusicMuteToggle;
    public Toggle sfxMuteToggle;
    public Button applyButton;
    public Toggle ragdollToggle;
    public Toggle aimSystemToggle;
    public Toggle bulletTimeToggle;

    public Button applyButtonIG;
    public Button backButtonIG;

    //********************************************************* Post Processing variables *********************************************

    public PostProcessingProfile postProcessing;

    //*******************************************************************************************************************************

    //********************************************************* Prefab Variables ******************************************

    public GameObject enemyPrefab;
    public Object playerPrefab;

    //*********************************************************************************************************************



    public Resolution[] resolutions;

    public GameSettings gameSettings;

    SoundManager soundMan;

    void Awake()
    {

        if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex == 0)
        {
            CreateSettingsOnFirstLoad();
        }

        else
        {
            if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex == 2)
            {
                applyButton = GameObject.Find("Apply").GetComponent<Button>();
                GetUiElements();
                antialiasingDropdwon = GameObject.Find("Canvas Options").transform.Find("Antialiasing").GetComponent<Dropdown>();
                gameSettings = JsonUtility.FromJson<GameSettings>(File.ReadAllText(Application.persistentDataPath + "/gamesettings.json"));
                SetUiElements();
            }
            else
            {
                pauseMenu = GameObject.Find("Canvas").transform.Find("Pause Menu").gameObject;
                inGameOptionsMenu = GameObject.Find("Canvas").transform.Find("Options Menu").gameObject;
                inGameOptionsMenu.SetActive(true);
                applyButtonIG = GameObject.Find("ApplyIG").GetComponent<Button>();
                backButtonIG = GameObject.Find("BackButtonIG").GetComponent<Button>();
                antialiasingDropdwon = GameObject.Find("Canvas").transform.GetChild(2).GetChild(9).GetComponent<Dropdown>();
                GetUiElements();
                gameSettings = JsonUtility.FromJson<GameSettings>(File.ReadAllText(Application.persistentDataPath + "/gamesettings.json"));
                SetUiElements();
                inGameOptionsMenu.SetActive(false);
            }
            

            SetUiElements();
        }

        

        soundMan = SoundManager.GetInstance();

        LoadSettings();

    }

    public void CreateSettingsOnFirstLoad()
    {
        if (!File.Exists(Application.persistentDataPath + "/gamesettings.json"))
        {
            gameSettings = new GameSettings();

            gameSettings.fullscreen = true;
            gameSettings.vSync = 1;
            gameSettings.motionBlur = true;
            gameSettings.postProcessingBool = true;
            gameSettings.textureQuality = 0;
            gameSettings.antialiasing = 0;
            gameSettings.shadows = 0;
            gameSettings.masterVolume = 1;
            gameSettings.bgmMusicVolume = 1;
            gameSettings.sfxVolume = 1;
            gameSettings.masterVolumeMute = false;
            gameSettings.bgmMusicMute = false;
            gameSettings.sfxMute = false;
            gameSettings.ragdoll = true;
            gameSettings.aimSystem = false;
            gameSettings.manualBulletTime = false;

            SaveSettings();
        }
    }

    void GetUiElements()
    {

        

        fullscreenToggle = GameObject.Find("Fullscreen").GetComponent<Toggle>();
        vSyncToggle = GameObject.Find("Verticle Sync").GetComponent<Toggle>();
        motionBlueToggle = GameObject.Find("Motion Blur").GetComponent<Toggle>();
        postProcessingToggle = GameObject.Find("Post Processing").GetComponent<Toggle>();
        resolutionDropdown = GameObject.Find("Resolution").GetComponent<Dropdown>();
        textureQualityDropdown = GameObject.Find("Texture Quality").GetComponent<Dropdown>();
        
        shadowsDropdown = GameObject.Find("Shadows").GetComponent<Dropdown>();
        masterVolumeSlider = GameObject.Find("Master Volume Slider").GetComponent<Slider>();
        bgmMusicVolumeSlider = GameObject.Find("Music Volume Slider").GetComponent<Slider>();
        sfxVolumeSlider = GameObject.Find("SFX Volume Slider").GetComponent<Slider>();
        masterVolumeMuteToggle = GameObject.Find("Master Volume Mute").GetComponent<Toggle>();
        bgmMusicMuteToggle = GameObject.Find("Music Volume Mute").GetComponent<Toggle>();
        sfxMuteToggle = GameObject.Find("SFX Volume Mute").GetComponent<Toggle>();
        ragdollToggle = GameObject.Find("Ragdoll").GetComponent<Toggle>();
        aimSystemToggle = GameObject.Find("Alternate Aiming System").GetComponent<Toggle>();
        bulletTimeToggle = GameObject.Find("Manual Bullet Time").GetComponent<Toggle>();
        if (applyButton != null)
        {
            applyButton.onClick.AddListener(delegate { OnApplyButtonClick(); });
        }

        if (applyButtonIG != null)
        {
            applyButtonIG.onClick.AddListener(delegate { OnApplyButtonClickIG(); });
        }

        if (backButtonIG != null)
        {
            backButtonIG.onClick.AddListener(delegate { OnBackButtonClickIG(); });
        }

        fullscreenToggle.onValueChanged.AddListener(delegate { OnFullscreenToggle(); });
        vSyncToggle.onValueChanged.AddListener(delegate { OnVSyncChange(); });
        motionBlueToggle.onValueChanged.AddListener(delegate { OnMotionBlurChange(); });
        postProcessingToggle.onValueChanged.AddListener(delegate { OnPostProcessingChange(); });
        resolutionDropdown.onValueChanged.AddListener(delegate { OnResolutionChange(); });
        textureQualityDropdown.onValueChanged.AddListener(delegate { OnTextureChange(); });
        antialiasingDropdwon.onValueChanged.AddListener(delegate { OnAntialiasingChange(); });
        shadowsDropdown.onValueChanged.AddListener(delegate { OnShadowChange(); });
        masterVolumeSlider.onValueChanged.AddListener(delegate { OnMasterVolumeChange(); });
        bgmMusicVolumeSlider.onValueChanged.AddListener(delegate { OnMusicVolumeChange(); });
        sfxVolumeSlider.onValueChanged.AddListener(delegate { OnSFXVolumeChange(); });
        masterVolumeMuteToggle.onValueChanged.AddListener(delegate { OnMasterVolumeMute(); });
        bgmMusicMuteToggle.onValueChanged.AddListener(delegate { OnMusicMute(); });
        sfxMuteToggle.onValueChanged.AddListener(delegate { OnSFXMute(); });
        ragdollToggle.onValueChanged.AddListener(delegate { OnRagdollToggle(); });
        aimSystemToggle.onValueChanged.AddListener(delegate { OnAimSystemChange(); });
        bulletTimeToggle.onValueChanged.AddListener(delegate { OnBulletTimeToggleChange(); });
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
            if (postProcessing == null)
            {
                
            }
            else
            {
                gameSettings.motionBlur = postProcessing.motionBlur.enabled = false;
            }
            
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
		//this may make the last resultion unselectable, might need to fix?
		if (resolutionDropdown.value >= resolutions.Length) {
			resolutionDropdown.value = resolutions.Length - 1;
		}
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

    public void OnShadowChange()
    {

    }

    public void OnMasterVolumeChange()
    {
        gameSettings.masterVolume = masterVolumeSlider.value;
        SoundManager.SetMasterVolume(masterVolumeSlider.value);
        SoundManager.AdjustSoundImmediate();
    }

    public void OnMusicVolumeChange()
    {
        gameSettings.bgmMusicVolume = bgmMusicVolumeSlider.value;
        SoundManager.SetMusicVolume(bgmMusicVolumeSlider.value);
        SoundManager.AdjustSoundImmediate();
    }

    public void OnSFXVolumeChange()
    {
        gameSettings.sfxVolume = sfxVolumeSlider.value;
        SoundManager.SetSFXVolume(sfxVolumeSlider.value);
        SoundManager.AdjustSoundImmediate();
    }

    public void OnMasterVolumeMute()
    {
        gameSettings.masterVolumeMute = masterVolumeMuteToggle.isOn;
        if (masterVolumeMuteToggle.isOn == true)
        {
            SoundManager.SetMasterVolume(0);
            SoundManager.AdjustSoundImmediate();
        }
        else
        {
            SoundManager.SetMusicVolume(bgmMusicVolumeSlider.value);
            SoundManager.SetSFXVolume(sfxVolumeSlider.value);
            SoundManager.AdjustSoundImmediate();
        }
    }

    public void OnMusicMute()
    {
        gameSettings.bgmMusicMute = bgmMusicMuteToggle.isOn;
        if (bgmMusicMuteToggle.isOn == true)
        {
            SoundManager.SetMusicVolume(0);
            SoundManager.AdjustSoundImmediate();
        }
        else
        {
            SoundManager.SetMusicVolume(bgmMusicVolumeSlider.value);
            SoundManager.AdjustSoundImmediate();
        }
    }

    public void OnSFXMute()
    {
        gameSettings.sfxMute = sfxMuteToggle.isOn;
        if (sfxMuteToggle.isOn == true)
        {
            SoundManager.SetSFXVolume(0);
            SoundManager.AdjustSoundImmediate();
        }
        else
        {
            SoundManager.SetSFXVolume(sfxVolumeSlider.value);
            SoundManager.AdjustSoundImmediate();
        }
    }

    public void OnRagdollToggle()
    {
        gameSettings.ragdoll = enemyPrefab.GetComponent<Ragdoll>().enabled = ragdollToggle.isOn;
    }

    public void OnAimSystemChange()
    {
        gameSettings.aimSystem = aimSystemToggle.isOn;
    }

    public void OnBulletTimeToggleChange()
    {
        gameSettings.manualBulletTime = bulletTimeToggle.isOn;
    }

    public void OnApplyButtonClick()
    {
        SaveSettings();
    }

    public void OnApplyButtonClickIG()
    {
        SaveSettings();
        GameObject optionsMenu = GameObject.Find("Canvas").transform.Find("Options Menu").gameObject;
        optionsMenu.SetActive(false);
        pauseMenu.SetActive(true);
        pauseMenu.transform.Find("Options").GetComponent<Button>().Select();
        pauseMenu.transform.Find("Resume").GetComponent<Button>().Select();
    }

    public void OnBackButtonClickIG()
    {
        GameObject optionsMenu = GameObject.Find("Canvas").transform.Find("Options Menu").gameObject;
        optionsMenu.SetActive(false);
        pauseMenu.SetActive(true);
        pauseMenu.transform.Find("Options").GetComponent<Button>().Select();
        pauseMenu.transform.Find("Resume").GetComponent<Button>().Select();
    }

    public void SaveSettings()
    {
        string jsonData = JsonUtility.ToJson(gameSettings, true);
        File.WriteAllText(Application.persistentDataPath + "/gamesettings.json", jsonData);
    }

    public void LoadSettings()
    {
        File.ReadAllText(Application.persistentDataPath + "/gamesettings.json");
        gameSettings = JsonUtility.FromJson<GameSettings>(File.ReadAllText(Application.persistentDataPath + "/gamesettings.json"));
        if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex != 0)
        {
            SetUiElements();
        }

    }

    void SetUiElements()
    {
        postProcessing = Resources.Load("Core Post Processing") as UnityEngine.PostProcessing.PostProcessingProfile;

        fullscreenToggle.isOn = gameSettings.fullscreen;
        Screen.fullScreen = gameSettings.fullscreen;
        vSyncToggle.isOn = gameSettings.vSync != 0;
        motionBlueToggle.isOn = gameSettings.motionBlur;
        postProcessingToggle.isOn = gameSettings.postProcessingBool;
        resolutionDropdown.value = gameSettings.resolutionIndex;
        textureQualityDropdown.value = gameSettings.textureQuality;
        antialiasingDropdwon.value = gameSettings.antialiasing;
        shadowsDropdown.value = gameSettings.shadows;
        masterVolumeSlider.value = gameSettings.masterVolume;
        bgmMusicVolumeSlider.value = gameSettings.bgmMusicVolume;
        sfxVolumeSlider.value = gameSettings.sfxVolume;
        masterVolumeMuteToggle.isOn = gameSettings.masterVolumeMute;
        bgmMusicMuteToggle.isOn = gameSettings.bgmMusicMute;
        sfxMuteToggle.isOn = gameSettings.sfxMute;
        ragdollToggle.isOn = gameSettings.ragdoll;
        aimSystemToggle.isOn = gameSettings.aimSystem;
        bulletTimeToggle.isOn = gameSettings.manualBulletTime;

        resolutions = Screen.resolutions;
        foreach (Resolution resolution in resolutions)
        {
            resolutionDropdown.options.Add(new Dropdown.OptionData(resolution.ToString()));
        }
    }

}
