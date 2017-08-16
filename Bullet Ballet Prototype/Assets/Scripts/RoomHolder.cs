using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomHolder : MonoBehaviour {

    private bool m_InRoom = false;

    //reference to the room fog
    public Material m_RoomFogMaterial;

    //this is what we modify as it will effect only this room
    private Material m_ThisRoomsFogMaterial;

    /// <summary>
    /// total number of rooms
    /// incremented every time a new room is created
    /// </summary>
    private static int m_NumOfRooms;
    /// <summary>
    /// this rooms id, taken from m_NumOfRooms on startup
    /// </summary>
    public int m_RoomID;

    /// <summary>
    /// number of colliders the player is currently in, for this room
    /// </summary>
    private int m_InColliders;

    /// <summary>
    /// Has this room been interacted with.
    /// will be true until the fog fade is done
    /// </summary>
    private bool m_RoomInteractedWith = false;
    /// <summary>
    /// Last time this room was interacted with, was it entered or exited
    /// </summary>
    private bool m_Entered;
    /// <summary>
    /// time of the interaction
    /// </summary>
    private float m_TimeInteractedWith;
    /// <summary>
    /// how many seconds it takes to fade in and out after it has been interacted with
    /// </summary>
    public float m_FadeTime = 2.0f;
    /// <summary>
    /// Curve for the Fading,
    /// defaults to a straight line
    /// </summary>
    public AnimationCurve m_FadeCurve = AnimationCurve.Linear(0,0,1,1);

	public static int m_PlayersCurrentRoom = -1;

    // Use this for initialization
    void Start() {
        //set room ID while also incrementing the number of rooms
        m_RoomID = m_NumOfRooms++;

        //create a copy of this material
        m_ThisRoomsFogMaterial = new Material(m_RoomFogMaterial);

        setupRoom(transform);
    }

    // Update is called once per frame
    void Update() {
        //set unscaled time within the shader
        m_ThisRoomsFogMaterial.SetFloat("_UnscaledTime", Time.unscaledTime);

        fogAnimation();
    }

    /// <summary>
    /// runs the fog animation using time and the m_FadeCurve
    /// </summary>
    private void fogAnimation() {
        if (m_RoomInteractedWith) {
            //get a value between 0 and 1 showing how far we are through the fade
            float time = (Time.time - m_TimeInteractedWith) / m_FadeTime;

            //if exiting the room, do it backwards
            if (!m_Entered) {
                time = 1 - time;
            }

            //getting the fade curve
            float evaluation = m_FadeCurve.Evaluate(time);

            //apply curve
            m_ThisRoomsFogMaterial.SetFloat("_InRoom", evaluation);

            //if the time is over the curve, then we finish it
            if(time <= 0 || time >= 1) {
                m_RoomInteractedWith = false;
            }

        }
    }

    private void startFade(bool a_Entered) {
        m_RoomInteractedWith = true;
        m_Entered = a_Entered;
        m_TimeInteractedWith = Time.time;
    }

    public void roomInteracted(bool a_Entered,RoomScript a_Script) {
            m_InRoom = a_Entered;
        if (m_InRoom) {
            m_InColliders++;

            //if entered room then set _InRoom to 1, else set _InRoom to 0
            //m_ThisRoomsFogMaterial.SetFloat("_InRoom", 1);
            if(m_InColliders == 1) {
				m_PlayersCurrentRoom = m_RoomID;
                startFade(true);
            }
        } else {
            m_InColliders--;
            if (m_InColliders == 0) {
                startFade(false);
                // m_ThisRoomsFogMaterial.SetFloat("_InRoom", 0);
            }
        }


    }

    void setupRoom(Transform a_Base) {
        for (int i = 0; i < a_Base.childCount; i++) {
            Transform child = a_Base.GetChild(i);
            setupRoom(child);

            //set up room script
            RoomScript room = child.gameObject.AddComponent<RoomScript>();
            room.m_ThisRoomsManager = this;
            room.m_RoomID = m_RoomID;//apply this rooms id to the scipt
            //set material
            child.gameObject.GetComponent<Renderer>().material = m_ThisRoomsFogMaterial;
        }
    }


   ///* GIZMOS DRAW */
   //void OnDrawGizmos() {
   //    Color col = Color.red;
   //    col.a = 0.2f;
   //    Gizmos.color = col;
   //
   //    drawChildren(transform);
   //}
   //
   //void drawChildren(Transform a_Base) {
   //    for (int i = 0; i < a_Base.childCount; i++) {
   //        Transform child = a_Base.GetChild(i);
   //        drawChildren(child);
   //        //draw children
   //        //this means that it wont draw the base transform
   //        Gizmos.DrawCube(child.position, child.localScale);
   //    }
   //}
}
