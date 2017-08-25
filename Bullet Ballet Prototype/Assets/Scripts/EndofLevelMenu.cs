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

    void OnValidate()
    {
        if (nextLevel == null)
        nextLevel = GameObject.Find("Next Level ES").GetComponent<Button>();

        if (exitToMenu == null)
            exitToMenu = GameObject.Find("Exit To Menu ES").GetComponent<Button>();

        if (exitToDesktop == null)
            exitToDesktop = GameObject.Find("Exit To Desktop ES").GetComponent<Button>();

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
