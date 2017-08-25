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
        SceneManager.LoadScene(1);
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
