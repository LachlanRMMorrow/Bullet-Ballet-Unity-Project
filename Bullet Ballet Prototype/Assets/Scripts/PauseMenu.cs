using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour {



    GameObject pauseMenu;
    public GameObject optionsMenu;

     public Button resume;
     public Button options;
     public Button exitToMenu;
     public Button exitToDesktop;

    GUIManager manager;

    void Start ()
    {
       
        manager = GetComponent<GUIManager>();
        //resume.onClick.AddListener(Resume);
        //options.onClick.AddListener(Options);
        //exitToMenu.onClick.AddListener(ExitToMenu);
        //exitToDesktop.onClick.AddListener(ExitToDesktop);
	}

    public void PauseActive()
    {
        manager.ScreenBlur(true);
        if (pauseMenu ==  null)
        {
            pauseMenu = GameObject.Find("Pause Menu");
        }

        if (optionsMenu == null)
        {
            optionsMenu = GameObject.Find("Canvas").transform.Find("Options Menu").gameObject;
        }

        if (resume == null)
        {
            resume = GameObject.Find("Resume").GetComponent<Button>();
            resume.onClick.AddListener(Resume);
        }

        if (options == null)
        {
            options = GameObject.Find("Options").GetComponent<Button>();
            options.onClick.AddListener(Options);
        }

        if (exitToMenu == null)
        {
            exitToMenu = GameObject.Find("Exit To Menu").GetComponent<Button>();
            exitToMenu.onClick.AddListener(ExitToMenu);
        }

        if (exitToDesktop == null)
        {
            exitToDesktop = GameObject.Find("Exit To Desktop").GetComponent<Button>();
            exitToDesktop.onClick.AddListener(ExitToDesktop);
        }
        exitToMenu.Select();
        resume.Select();
    }

    
    

    public void Resume()
    {
        SlowMoManager.m_isPaused = false;
        manager.ScreenBlur(false);
        pauseMenu.SetActive(false);
        Debug.Log("dasda");
        Time.timeScale = 1;
    }

    void Options()
    {
        pauseMenu.SetActive(false);
        optionsMenu.SetActive(true);
        Toggle fullScreen = optionsMenu.transform.Find("Fullscreen").GetComponent<Toggle>();
        Toggle vSync = optionsMenu.transform.Find("Verticle Sync").GetComponent<Toggle>();

        vSync.Select();
        fullScreen.Select();

       
    }

    void ExitToMenu()
    {
        Time.timeScale = 1;
        manager.ScreenBlur(false);
        SoundManager.StopBGM(false, 0);
        SceneManager.LoadScene(0);
        
    }

    void ExitToDesktop()
    {
        Application.Quit();
    }

    

}
