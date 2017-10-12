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

        if (SceneManager.GetActiveScene().buildIndex == 0)
        {
            if (soundMan.bgmSource.clip != clip)
            {
                SoundManager.PlayBGM(clip, false, 2.0f);
            }
        }
        else
        {
            SoundManager.PlayBGM(clip, false, 2.0f);
        }
        


    }
}
