using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class EndofLevelMenu : MonoBehaviour
{
    public Button nextLevel;
    public Button exitToMenu;
    public Button exitToDesktop;


	void Start ()
    {
        nextLevel.onClick.AddListener(NextLevel);
        exitToMenu.onClick.AddListener(ExitToMenu);
        exitToDesktop.onClick.AddListener(ExitToDesktop);
	}

    void NextLevel()
    {
        SceneManager.LoadScene("L1F1 - Addison");
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
