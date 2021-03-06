﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(BoxCollider))]
[RequireComponent(typeof(Rigidbody))]
public class ToggleSlowMoTrigger : MonoBehaviour {

    /// <summary>
    /// how long does this slow mo last
    /// </summary>
    public float m_SlowMoTime;

    /// <summary>
    /// simple flag to check if this trigger should be deleted after it triggers the slow mo
    /// </summary>
    public bool m_DeleteAfterTrigger = false;

    void OnTriggerEnter(Collider a_Other) {
        //check if this object is a player
        if (a_Other.gameObject.layer == LayerMask.NameToLayer("Player")) {
            SlowMoManager smm = FindObjectOfType<SlowMoManager>();
            //start the trigger
            smm.startSlowmo(m_SlowMoTime);

            if (m_DeleteAfterTrigger) {
                //delete gameobject
                Destroy(gameObject);
            }
        }
    }

    void OnValidate() {
        Rigidbody rb = GetComponent<Rigidbody>();
        BoxCollider bc = GetComponent<BoxCollider>();

        //these two values need to be set to allow the trigger to work
        if (!rb.isKinematic || !bc.isTrigger) {

            //friendly warning :)
            Debug.LogWarning("Rigidbody or Box Collider was not kinematic or a trigger in object " + transform.name + ", I have updated them to be correct...");
            Debug.LogWarning("If there is a issue if with, talk to one of the programmers");

            //update values
            rb.isKinematic = true;
            bc.isTrigger = true;
        }
    }

}
