using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class DeathScreen : MonoBehaviour
{
    int scene;

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
        scene = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(scene);
    }

    void ExitToDesktop()
    {
        Application.Quit();
    }

    void ExitToMenu()
    {
        SceneManager.LoadScene(0);
    }


}
