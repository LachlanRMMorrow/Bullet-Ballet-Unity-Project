using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour {

    [System.Serializable]
    public struct DoorStruct {
        public Transform m_DoorTransform;
        [HideInInspector]
        public Vector3 m_StartingPos;
    };

    public DoorStruct m_LeftDoor;
    public DoorStruct m_RightDoor;

    public float m_MovementAmount = 5.0f;

    private int m_EntitiesInTransform = 0;

    private float m_OpenLength = 1.0f;
    private float m_OpenStartTime;
    private bool m_IsInteractedWith = false;
    /// <summary>
    /// is this door meant to open or close?
    /// </summary>
    private bool m_Open = false;

    void Start() {

    }
	
	// Update is called once per frame
	void Update () {
        if (m_IsInteractedWith) {

        }
	}

    /// <summary>
    /// 
    /// </summary>
    /// <param name="a_DoorMovement">true will mean open, false will be closed</param>
    void startDoorMovement(bool a_DoorMovement) {
        m_OpenStartTime = Time.time;
        m_IsInteractedWith = true;
        m_Open = a_DoorMovement;
    }
    
    void OnTriggerEnter(Collider collision) {
        m_EntitiesInTransform++;
        if (m_EntitiesInTransform == 1) {
            startDoorMovement(true);
        }
    }

    void OnTriggerExit(Collider collision) {
        m_EntitiesInTransform--;
        if (m_EntitiesInTransform == 0) {
            startDoorMovement(false);
        }
    }
}
