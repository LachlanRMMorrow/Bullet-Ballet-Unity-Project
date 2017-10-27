using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BackGroundMusic : MonoBehaviour
{

    public AudioClip clip;
    SoundManager soundMan;

    // Use this for initialization
    void Start()
    {
        soundMan = SoundManager.GetInstance();
        //if scene is main menu
        if (SceneManager.GetActiveScene().buildIndex == 0)
        {
            // if current background music isnt the main menu music then start playing main menu music
            if (soundMan.bgmSource.clip != clip)
            {
                SoundManager.PlayBGM(clip, false, 2.0f, 0);
            }
        }
        else
        {
            SoundManager.PlayBGM(clip, false, 2.0f, 0);
        }
        


    }
}
