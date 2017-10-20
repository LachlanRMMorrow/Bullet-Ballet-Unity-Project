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

    public GameObject m_DeathScreenHolder;

    GUIManager manager;

	void Start ()
    {
        manager = GetComponent<GUIManager>();
	}
	
    public void DeathScreenActive()
    {
        manager.ScreenBlur(true);
        m_DeathScreenHolder.SetActive(true);
        Time.timeScale = 0;

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
        manager.ScreenBlur(false);
        scene = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(scene);
    }

    void ExitToDesktop()
    {
        manager.ScreenBlur(false);
        Application.Quit();
    }

    void ExitToMenu()
    {
        SoundManager.StopBGM(false, 0);
        SceneManager.LoadScene(0);
    }

    void OnValidate()
    {
        if (m_DeathScreenHolder == null)
        {
            //gets death Screen, using the 2nd child
            m_DeathScreenHolder = GameObject.Find("Canvas").transform.GetChild(3).gameObject;

        }
    }

    public static void runDeathScreen() {
        //finds EventSystem, gets this script and calls DeathScreenActive
        DeathScreen ds = GameObject.Find("EventSystem").GetComponent<DeathScreen>();ds.DeathScreenActive();
        ds.exitToDesktop.Select();
        ds.restart.Select();
    }

}
