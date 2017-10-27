using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerMovement))]
[RequireComponent(typeof(PlayerArms))]
[RequireComponent(typeof(PlayerDive))]
[RequireComponent(typeof(Health))]
public class Player : MonoBehaviour {

    public static bool m_PlayerAlive = true;

    public static bool m_HasPlayerDoneAnything = false;

    void Awake() {
        //add listener
        Health health = GetComponent<Health>();
        health.m_ObjectDiedEvent.AddListener(playerKilled);
        health.m_ObjectHitEvent.AddListener(playerHit);

        m_PlayerAlive = true;

        m_HasPlayerDoneAnything = false;
    }

    /// <summary>
    /// event thats called after the player runs out of health
    /// </summary>
    void playerKilled() {
        //if tutorial
        if (false) {
            //move player back to a point
        } else {
            //else if it's anything but the tutorial
            //then die normally

            DeathScreen.runDeathScreen();
            //todo: swap this out with another object?
            Destroy(gameObject);

            m_PlayerAlive = false;
        }
    }

    void playerHit() {
        LevelEmissionFlash.m_Singleton.startFlash(true);
    }
}
