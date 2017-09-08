using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerMovement))]
[RequireComponent(typeof(PlayerArms))]
[RequireComponent(typeof(Health))]
public class Player : MonoBehaviour {

    void Awake() {
        //add listener
        Health health = GetComponent<Health>();
        health.m_ObjectDiedEvent.AddListener(playerKilled);
    }

    /// <summary>
    /// event thats called after the player runs out of health
    /// </summary>
    void playerKilled() {
        DeathScreen.runDeathScreen();
        //todo: swap this out with another object?
        Destroy(gameObject);
    }

}
