using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelRestart : MonoBehaviour {

    public JInput.ControllerButtons m_RestartControllerButton = JInput.ControllerButtons.Select;
    public KeyCode m_RestartKeyButton = KeyCode.R;
    public KeyCode m_BGMSoundButton = KeyCode.B;

    public AudioClip clip;

	// Use this for initialization
	void Start ()
    {
        clip = GameObject.Find("MANAGER").GetComponent<BackGroundMusic>().clip;
	}
	
	// Update is called once per frame
	void Update ()
    {
        JInput.Controller controller = JInput.CurrentController.currentController;

        if(controller == null) {
            return;
        }

        if(controller.WasButtonPressed(m_RestartControllerButton) || Input.GetKeyDown(m_RestartKeyButton)) {
            Scene scene = SceneManager.GetActiveScene();            
            SceneManager.LoadScene(scene.buildIndex);
        }

        if (Input.GetKeyDown(m_BGMSoundButton))
        {
            Debug.Log("Hello");
            SoundManager.PlayBGM(clip, false, 2.0f, 0);
        }
	}

}
