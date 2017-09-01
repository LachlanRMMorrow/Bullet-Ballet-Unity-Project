﻿using System.Collections;
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
        //restart.onClick.AddListener(Restart);
        //exitToDesktop.onClick.AddListener(ExitToDesktop);
        //exitToMenu.onClick.AddListener(ExitToMenu);
	}
	
    public void DeathScreenActive()
    {
        if (restart == null)
        {
restart = GameObject.Find("Restart DS").GetComponent<Button>();
            restart.onClick.AddListener(Restart);
        }
        

        if (exitToDesktop == null)
        {
            exitToDesktop = GameObject.Find("Exit to Desktop DS").GetComponent<Button>();
            exitToDesktop.onClick.AddListener(ExitToDesktop);
        }


        if (exitToMenu == null)
        {
exitToMenu = GameObject.Find("Exit to Menu DS").GetComponent<Button>();
            exitToMenu.onClick.AddListener(ExitToMenu);
        }
            
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
        SoundManager.StopBGM(false, 0);
        SceneManager.LoadScene(0);
    }


}
