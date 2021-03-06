﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class EndofLevelMenu : MonoBehaviour
{
    int scene;

     Button nextLevel;
     Button restartLevel;
     Button exitToMenu;
     Button exitToDesktop;

    public GameObject m_ShakeHolder;

    GUIManager manager;

	void Start ()
    {
        manager = GetComponent<GUIManager>();
        //nextLevel.onClick.AddListener(NextLevel);
        //exitToMenu.onClick.AddListener(ExitToMenu);
        //exitToDesktop.onClick.AddListener(ExitToDesktop);
	}

    public void EndOfLeveActive()
    {

        m_ShakeHolder = GameObject.Find("Canvas").transform.GetChild(0).gameObject;
        m_ShakeHolder.SetActive(false);
        manager.ScreenBlur(true);
        if (nextLevel == null)
        {
            nextLevel = GameObject.Find("Next Level ES").GetComponent<Button>();
            nextLevel.onClick.AddListener(NextLevel);
        }
        

        if (restartLevel == null)
        {
            restartLevel = GameObject.Find("Restart ES").GetComponent<Button>();
            restartLevel.onClick.AddListener(RestartLevel);
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
        manager.ScreenBlur(false);
        scene = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(scene + 1);
        
    }

    void RestartLevel()
    {
        manager.ScreenBlur(false);
        scene = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(scene);

    }

    void ExitToMenu()
    {
        manager.ScreenBlur(false);
        SoundManager.StopBGM(false, 0);
        SceneManager.LoadScene(0);
    }

    void ExitToDesktop()
    {
        Application.Quit();
    }
}
