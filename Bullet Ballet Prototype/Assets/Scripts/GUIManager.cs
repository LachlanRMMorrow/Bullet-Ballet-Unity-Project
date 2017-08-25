using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GUIManager : MonoBehaviour {

    public Button newGame;
    public Button resume;
    public Button options;
    public Button exit;

	// Use this for initialization
	void Start ()
    {

        newGame.onClick.AddListener(NewGame);
        resume.onClick.AddListener(Resume);
        options.onClick.AddListener(Options);
        exit.onClick.AddListener(Exit);

    }
	//*************************** Main Menu *************************************
	void NewGame()
    {
        
        SceneManager.LoadScene(2);
    }

    void Resume()
    {

    }

    void Options()
    {
        SceneManager.LoadScene(3);
    }

    void Exit()
    {
        Application.Quit();
    }

    //*************************** Level Select *************************************

}
