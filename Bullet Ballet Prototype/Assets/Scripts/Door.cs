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

    /// <summary>
    /// amount of entities that are currently in this trigger,
    /// </summary>
    private int m_EntitiesInTransform = 0;

    private float m_OpenLength = 1.0f;
    private float m_OpenStartTime;
    private bool m_IsInteractedWith = false;
    /// <summary>
    /// is this door meant to open or close?
    /// </summary>
    private bool m_Open = false;

    public bool m_ForceDoorInteraction = false;

    void Start() {
        if (m_LeftDoor.m_DoorTransform != null) {
            m_LeftDoor.m_StartingPos = m_LeftDoor.m_DoorTransform.position;
        }
        if (m_RightDoor.m_DoorTransform != null) {
            m_RightDoor.m_StartingPos = m_RightDoor.m_DoorTransform.position;
        }
    }

    // Update is called once per frame
    void Update() { }
        if (m_ForceDoorInteraction) {
            m_ForceDoorInteraction = false;
            startDoorMovement(!m_Open);
        }
        if (m_IsInteractedWith) {
            float percentage = (Time.time - m_OpenStartTime) / m_OpenLength;

            if (percentage > 1) {
                m_IsInteractedWith = false;
                percentage = 1;
            }

            //if closing
            if (!m_Open) {
                percentage = 1 - percentage;
            }

            if (m_LeftDoor.m_DoorTransform != null) {
                m_LeftDoor.m_DoorTransform.position = Vector3.Lerp(m_LeftDoor.m_StartingPos, m_LeftDoor.m_StartingPos + transform.rotation * new Vector3(-m_MovementAmount, 0, 0), percentage);
            }
            if (m_RightDoor.m_DoorTransform != null) {
                m_RightDoor.m_DoorTransform.position = Vector3.Lerp(m_RightDoor.m_StartingPos, m_RightDoor.m_StartingPos + transform.rotation * new Vector3(m_MovementAmount, 0, 0), percentage);
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="a_DoorMovement">true will mean open, false will be closed</param>
    void startDoorMovement(bool a_DoorMovement) {
        //if the rooms are already opening/closing then we should update the time to have it seem like the doors havent changed
        if (m_IsInteractedWith) {
            float percentage = (Time.time - m_OpenStartTime) / m_OpenLength;
            m_OpenStartTime = Time.time - (1-percentage * m_OpenLength);

        } else {
            m_OpenStartTime = Time.time;
            m_IsInteractedWith = true;
        }
        m_Open = a_DoorMovement;
    }

    void OnTriggerEnter(Collider collision) {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Player") || collision.gameObject.layer == LayerMask.NameToLayer("Enemies")) {
            m_EntitiesInTransform++;
            if (m_EntitiesInTransform == 1) {
                startDoorMovement(true);
            }
        }
    }

    void OnTriggerExit(Collider collision) {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Player") || collision.gameObject.layer == LayerMask.NameToLayer("Enemies")) {
            m_EntitiesInTransform--;
            if (m_EntitiesInTransform == 0) {
                startDoorMovement(false);
            }
        }
    }
}
