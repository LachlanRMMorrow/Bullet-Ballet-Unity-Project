using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class DeathScreen : MonoBehaviour
{

    public Button restart;
    public Button exitToDesktop;
    public Button exitToMenu;

	void Start ()
    {
        restart.onClick.AddListener(Restart);
        exitToDesktop.onClick.AddListener(ExitToDesktop);
        exitToMenu.onClick.AddListener(ExitToMenu);
	}
	
    void Restart()
    {
        SceneManager.LoadScene("L1F1 - Addison");
    }

    void ExitToDesktop()
    {
        Application.Quit();
    }

    void ExitToMenu()
    {
        SceneManager.LoadScene("Main Menu");
    }


}
