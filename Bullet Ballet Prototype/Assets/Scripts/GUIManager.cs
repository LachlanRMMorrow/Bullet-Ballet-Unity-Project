using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GUIManager : MonoBehaviour {

    int scene;

    public GameObject pauseMenu;

    public GameObject graphicsMenu;
    public GameObject soundMenu;

    public Button newGame;
    public Button resume;
    public Button options;
    public Button exit;

	// Use this for initialization
	void Start ()
    {

        //newGame.onClick.AddListener(NewGame);
        //resume.onClick.AddListener(Resume);
        //options.onClick.AddListener(Options);
        //exit.onClick.AddListener(Exit);

    }
	//*************************** Main Menu *************************************
	public void NewGame()
    {
        
        SceneManager.LoadScene(2);
    }

    public void Resume()
    {
        pauseMenu.SetActive(false);
        Time.timeScale = 1;
    }

    public void Restart()
    {
        scene = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(scene);
    }

    public void NextLevel()
    {
        SceneManager.LoadScene(1);
    }

    public void OptionsMainMenu()
    {
        SceneManager.LoadScene(3);
    }

    public void OptionsPauseMenu()
    {

    }

    public void ExitToMenu()
    {
        SceneManager.LoadScene(0);
    }

    public void ExitToDesktop()
    {
        Application.Quit();
    }
    //*************************** Level Select *************************************


    public void Level1()
    {
        SceneManager.LoadScene(4);
    }

    public void Level2()
    {
        SceneManager.LoadScene(1);
    }


    //*********************************** Main Menu Options **********************************

    public void Graphics()
    {
        graphicsMenu.SetActive(true);
        soundMenu.SetActive(false);
    }

    public void Sounds()
    {
        graphicsMenu.SetActive(false);
        soundMenu.SetActive(true);
    }

    public void Back()
    {
        SceneManager.LoadScene(0);
    }
}
