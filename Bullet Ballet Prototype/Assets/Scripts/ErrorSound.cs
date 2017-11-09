using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ErrorSound : MonoBehaviour {

    public AudioClip errorSound;

    public void PlayClip()
    {
        SoundManager.PlaySFX(errorSound);
    }
}
