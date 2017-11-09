using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomHolder : MonoBehaviour {

    /// <summary>
    /// is the player in this room at the moment
    /// </summary>
    private bool m_InRoom = false;


    public static int m_PlayersCurrentRoom = 0;

    #region Materials

    //reference to the room fog
    public Material m_RoomFogMaterial;

    //this is what we modify as it will effect only this room
    private Material m_ThisRoomsFogMaterial;

    #endregion

    #region RoomID's
    /// <summary>
    /// total number of rooms
    /// incremented every time a new room is created
    /// </summary>
    private static int m_NumOfRooms;
    /// <summary>
    /// this rooms id, taken from m_NumOfRooms on startup
    /// </summary>
    public int m_RoomID;

    #endregion

    /// <summary>
    /// number of colliders the player is currently in, for this room
    /// this could be higher then 1 when the player is in a room
    /// will be 0 when the player is not in the room
    /// </summary>
    private int m_InColliders = 0;

    /// <summary>
    /// Has this room been interacted with.
    /// will be true until the fog fade is done
    /// </summary>
    private bool m_RoomInteractedWith = false;
    /// <summary>
    /// Last time this room was interacted with, was it entered or exited
    /// </summary>
    public bool m_Entered;
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
    public AnimationCurve m_FadeCurve = AnimationCurve.Linear(0, 0, 1, 1);

    //neighbors 
    public List<RoomHolder> m_Neighbors = new List<RoomHolder>();

    public static List<RoomHolder> m_ListOfRooms = new List<RoomHolder>();

    void Awake() {
        //set room ID while also incrementing the number of rooms
        m_RoomID = 1 << m_NumOfRooms++;

        //create a copy of this material
        m_ThisRoomsFogMaterial = new Material(m_RoomFogMaterial);

        m_ListOfRooms.Add(this);
    }

    // Use this for initialization
    void Start() {
        setupRoom(transform);
    }

    // Update is called once per frame
    void Update() {
        //set unscaled time within the shader
        m_ThisRoomsFogMaterial.SetFloat("_UnscaledTime", Time.unscaledTime);
        if (m_RoomInteractedWith) {
            fogAnimation();
        }
    }

    /// <summary>
    /// runs the fog animation using time and the m_FadeCurve
    /// </summary>
    private void fogAnimation() {

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
        if (time < 0 || time >= 1) {
            m_RoomInteractedWith = false;
        }


    }

    public void startFade(bool a_Entered) {
        //if room is already the same as what we want to do, then return
        if (m_Entered == a_Entered) {
            return;
        }
        if (m_RoomInteractedWith) {
            //get how far through the current animation we are, and reverse it
            float percentage = (Time.time - m_TimeInteractedWith) / m_FadeTime;
            m_TimeInteractedWith = Time.time - (1 - percentage * m_FadeTime);
        } else {
            m_TimeInteractedWith = Time.time;
        }
        m_RoomInteractedWith = true;
        m_Entered = a_Entered;
    }

    public void roomInteracted(bool a_Entered, RoomScript a_Script) {
        bool runFade = false;
        int previousCurrentRoom = m_PlayersCurrentRoom;

        m_InRoom = a_Entered;
        if (m_InRoom) {
            m_InColliders++;

            if (m_InColliders == 1) {
                m_PlayersCurrentRoom |= m_RoomID;
                runFade = true;
            }
        } else {
            m_InColliders--;
            if (m_InColliders == 0) {
                m_PlayersCurrentRoom &= ~m_RoomID;
                runFade = true;
            }
        }

        if (runFade) {
            currentRoomUpdated(previousCurrentRoom);
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

    public static bool isPlayerInRoom(int a_RoomID) {
        return (m_PlayersCurrentRoom & a_RoomID) != 0;
    }

    private static void currentRoomUpdated(int a_PreviousCurrentRooms) {
        //print("currentRoomUpdated: current: " + m_PlayersCurrentRoom + " previous: " + a_PreviousCurrentRooms);

        for (int i = 0; i < m_ListOfRooms.Count; i++) {
            RoomHolder rh = m_ListOfRooms[i];
            int currentBitMask = m_PlayersCurrentRoom & rh.m_RoomID;
            int previousBitMask = a_PreviousCurrentRooms & rh.m_RoomID;
            int bitMaskOr = currentBitMask | previousBitMask;
            bool interacted = bitMaskOr != 0;
            if (interacted) {
                //print("Interacted with " + rh.m_RoomID + " - " + bitMaskOr);
                if (currentBitMask == previousBitMask) {
                    //print("nothing happened to this room");
                    continue;
                }
                bool entered = currentBitMask > previousBitMask;
                //print(entered ? "Entered" : "Exited");
                bool shouldFade = true;
                for (int q = 0; q < rh.m_Neighbors.Count; q++) {
                    RoomHolder neighbor = rh.m_Neighbors[q];

                    int neigboursBitMask = m_PlayersCurrentRoom & neighbor.m_RoomID;
                    if(neigboursBitMask == 0) {
                        neighbor.startFade(entered);
                    }else {
                        shouldFade = false;
                    }
                }
                if (shouldFade) {
                    rh.startFade(entered);
                }
            }

        }
        //startFade(enteredRoom);
        //for (int i = 0; i < m_Neighbors.Count; i++) {
        //    if (m_Neighbors[i].m_InRoom) {
        //        continue;
        //    }
        //    m_Neighbors[i].startFade(enteredRoom);
        //}
    }
}
