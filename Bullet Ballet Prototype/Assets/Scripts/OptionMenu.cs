using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class OptionMenu : MonoBehaviour {

    public Button graphics;
    public Button sounds;
    public Button back;

    GameObject graphicsMenu;
    GameObject soundMenu;

	void Start ()
    {
        graphicsMenu = GameObject.Find("Graphics");
        soundMenu = GameObject.Find("Sounds");

        graphicsMenu.SetActive(true);
        soundMenu.SetActive(false);

        graphics.onClick.AddListener(Graphics);
        sounds.onClick.AddListener(Sounds);
        back.onClick.AddListener(Back);
	}
	
	void Graphics()
    {
        graphicsMenu.SetActive(true);
        soundMenu.SetActive(false);
    }

    void Sounds()
    {
        graphicsMenu.SetActive(false);
        soundMenu.SetActive(true);
    }

    void Back()
    {
        SceneManager.LoadScene(0);
    }
}
