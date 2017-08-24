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
        resume.onClick.AddListener(Resume);
        options.onClick.AddListener(Options);
        exitToMenu.onClick.AddListener(ExitToMenu);
        exitToDesktop.onClick.AddListener(ExitToDesktop);
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
        SceneManager.LoadScene("Main Menu");
    }

    void ExitToDesktop()
    {
        Application.Quit();
    }
	
}
