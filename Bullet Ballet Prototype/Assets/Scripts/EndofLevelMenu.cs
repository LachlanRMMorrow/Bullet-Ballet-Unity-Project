using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class EndofLevelMenu : MonoBehaviour
{
    int scene;

     Button nextLevel;
     Button exitToMenu;
     Button exitToDesktop;


	void Start ()
    {
        nextLevel.onClick.AddListener(NextLevel);
        exitToMenu.onClick.AddListener(ExitToMenu);
        exitToDesktop.onClick.AddListener(ExitToDesktop);
	}

    public void EndOfLeveActive()
    {
        if (nextLevel == null)
        {
nextLevel = GameObject.Find("Next Level ES").GetComponent<Button>();
            nextLevel.onClick.AddListener(NextLevel);
        }
        

        if (exitToMenu == null)
        {
exitToMenu = GameObject.Find("Exit To Menu ES").GetComponent<Button>();
            exitToMenu.onClick.AddListener(ExitToMenu);
        }
            

        if (exitToDesktop == null)
        {
exitToDesktop = GameObject.Find("Exit To Desktop ES").GetComponent<Button>();
            exitToDesktop.onClick.AddListener(ExitToDesktop);
        }
            

    }

    void NextLevel()
    {
        scene = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(scene + 1);
    }

    void ExitToMenu()
    {
        SoundManager.StopBGM(false, 0);
        SceneManager.LoadScene(0);
    }

    void ExitToDesktop()
    {
        Application.Quit();
    }
}
