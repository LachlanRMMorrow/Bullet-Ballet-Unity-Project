using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(MeshRenderer))]
public class RoomScript : MonoBehaviour {

    /// <summary>
    /// the room manager
    /// </summary>
    [HideInInspector]
    public RoomHolder m_ThisRoomsManager;

    /// <summary>
    /// id of this room
    /// </summary>
    [HideInInspector]
    public int m_RoomID;

    void Awake() {
        GetComponent<Rigidbody>().isKinematic = true;
        GetComponent<Collider>().isTrigger = true;
    }

    void OnTriggerExit(Collider other) {
        //only run if the player goes through it
        if (other.gameObject.layer != LayerMask.NameToLayer("Player")) {
            return;
        }
        m_ThisRoomsManager.roomInteracted(false, this);
    }

    void OnTriggerEnter(Collider other) {
        //only run if the player goes through it
        if (other.gameObject.layer != LayerMask.NameToLayer("Player")) {
            return;
        }

        m_ThisRoomsManager.roomInteracted(true, this);

    }

}