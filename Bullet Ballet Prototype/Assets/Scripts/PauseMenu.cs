using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour {

    public GameObject pauseMenu;

    public Button resume;
    public Button options;
    public Button exitToMenu;
    public Button exitToDesktop;

	void Start ()
    { 
        //resume.onClick.AddListener(Resume);
        options.onClick.AddListener(Options);
        exitToMenu.onClick.AddListener(ExitToMenu);
        exitToDesktop.onClick.AddListener(ExitToDesktop);
	}

    void OnValidate()
    {
        if(resume == null)
        {
            resume = GameObject.Find("Resume").GetComponent<Button>();
        }

        if (options == null)
        {
            options = GameObject.Find("Options").GetComponent<Button>();
        }
        
        if (exitToMenu == null)
        {
            exitToMenu = GameObject.Find("Exit To Menu").GetComponent<Button>();
        }

        if (exitToDesktop == null)
        {
            exitToDesktop = GameObject.Find("Exit To Desktop").GetComponent<Button>();
        }
        
    }

    void Resume()
    {
        pauseMenu.SetActive(false);
        Time.timeScale = 1;
    }

    void Options()
    {

    }

    void ExitToMenu()
    {
        SceneManager.LoadScene(0);
    }

    void ExitToDesktop()
    {
        Application.Quit();
    }
	
}
