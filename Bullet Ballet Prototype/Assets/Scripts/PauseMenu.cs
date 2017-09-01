using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour {

     GameObject pauseMenu;

     public Button resume;
     public Button options;
     public Button exitToMenu;
     public Button exitToDesktop;

	void Start ()
    { 
        //resume.onClick.AddListener(Resume);
        //options.onClick.AddListener(Options);
        //exitToMenu.onClick.AddListener(ExitToMenu);
        //exitToDesktop.onClick.AddListener(ExitToDesktop);
	}

    public void PauseActive()
    {
        if (pauseMenu ==  null)
        {
            pauseMenu = GameObject.Find("Pause Menu");
        }

        if (resume == null)
        {
            resume = GameObject.Find("Resume").GetComponent<Button>();
            resume.onClick.AddListener(Resume);
        }

        //if (options == null)
        //{
        //    options = GameObject.Find("Options").GetComponent<Button>();
        //    options.onClick.AddListener(Options);
        //}

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
    }


    

    void Resume()
    {
        SlowMoManager.m_isPaused = false;
        pauseMenu.SetActive(false);
        Time.timeScale = 1;
    }

    void Options()
    {

    }

    void ExitToMenu()
    {
        SoundManager.StopBGM(false, 0);
        SceneManager.LoadScene(0);
    }

    void ExitToDesktop()
    {
        Application.Quit();
    }
	
}
