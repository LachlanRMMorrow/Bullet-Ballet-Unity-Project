using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GUIManager : MonoBehaviour {

    public UnityEngine.PostProcessing.PostProcessingProfile screenBlur;

    int scene;

    public GameObject pauseMenu;

    public GameObject graphicsMenu;
    public GameObject soundMenu;
    public GameObject optionsMainMenu;

    public Button newGame;
    public Button resume;
    public Button options;
    public Button exit;

    // Use this for initialization
    void Start ()
    {

        screenBlur = Resources.Load("Core Post Processing") as UnityEngine.PostProcessing.PostProcessingProfile;

        //remove screen blur on start
        ScreenBlur(false);
    }

    void OnApplicationQuit() {
        //remove screen blur on application finish (also called when stopping game in inspector)
        ScreenBlur(false);
    }

	//*************************** Main Menu *************************************
	public void NewGame()
    {
        
        SceneManager.LoadScene(1);
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
        scene = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(scene + 1);
    }

    public void OptionsMainMenu()
    {
        SceneManager.LoadScene(2);
    }

    public void OptionsPauseMenu()
    {

    }

    public void ExitToMenu()
    {
        SoundManager.StopBGM(false, 0);
        SceneManager.LoadScene(0);
    }

    public void Apply()
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
        SceneManager.LoadScene(3);
    }

    public void Level2()
    {
        SceneManager.LoadScene(4);
    }

    public void Level3()
    {
        SceneManager.LoadScene(5);
    }

    public void Level4()
    {
        SceneManager.LoadScene(6);
    }

    public void Level5()
    {
        SceneManager.LoadScene(7);
    }

    public void Level6()
    {
        SceneManager.LoadScene(8);
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

    public void ScreenBlur(bool acitve)
    {
        if (acitve == true)
        {
            screenBlur.depthOfField.enabled = true;
        }
        else
        {
            screenBlur.depthOfField.enabled = false;
        }
    }
}
