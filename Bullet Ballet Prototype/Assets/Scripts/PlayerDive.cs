using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Will be a Dash if slow mo is inactive
/// Will be a Dive if slow mo is active
/// </summary>
public class PlayerDive : MonoBehaviour {

    /// <summary>
    /// distance to move
    /// </summary>
    [Range(0,10)]
    public float m_Distance = 4;

    /// <summary>
    /// how long does it take to dash
    /// </summary>
    [Range(0.01f,10)]
    public float m_TimeTakenToDash = 2;

    /// <summary>
    /// cool down between dashes
    /// </summary>
    [Range(0,10)]
    public float m_DashCooldown;

    /// <summary>
    /// is the player currently dashing or diving
    /// </summary>
    private bool m_IsDiving = false;

    /// <summary>
    /// has the player dashed or dived on this button press
    /// </summary>
    private bool m_HasUsedOnButtonPress = false;


    /// <summary>
    /// time the dash/dive started
    /// </summary>
    private float m_StartTime;


    //references to components that are on the player
    private PlayerArms m_PlayerArms;
    private UnityEngine.AI.NavMeshAgent m_NavMesh;
    private Rigidbody m_Rigidbody;

    void Start() {
        m_PlayerArms = GetComponent<PlayerArms>();
        m_NavMesh = GetComponent<UnityEngine.AI.NavMeshAgent>();
        m_Rigidbody = GetComponent<Rigidbody>();
        
        m_StartTime = -1000;
    }

    void Update() {
        if (m_IsDiving) {
            runDive();
        } else {
            checkDive();
        }
    }

    private void runDive() {
        if (m_StartTime + m_TimeTakenToDash < Time.time) {
            m_IsDiving = false;
            m_Rigidbody.velocity = Vector3.zero;
        }
    }

    private void checkDive() {
        if (SlowMoManager.m_isPaused) {
            return;
        }
        if(GameStateManager.currentState != GameStates.Action) {
            return;
        }
        JInput.Controller controller = JInput.CurrentController.currentController;
        if (controller == null) {
            return;
        }
        bool isAboutToDive = controller.IsButtonDown(Keys.singleton.m_DiveDashButton);

        //if the dive is on cool down then pretend the player didn't press the Dive/Dash button
        if(m_StartTime + m_TimeTakenToDash + m_DashCooldown > Time.time) {
            isAboutToDive = false;
        }

        if (m_HasUsedOnButtonPress) {
            m_PlayerArms.m_CanMoveArms = m_NavMesh.enabled = m_Rigidbody.isKinematic = isAboutToDive;
        } else {
            m_PlayerArms.m_CanMoveArms = m_NavMesh.enabled = m_Rigidbody.isKinematic = !isAboutToDive;
        }

        if (isAboutToDive) {

            Vector2 leftStick = controller.getAxisValue(JInput.ControllerVec2Axes.LStick);

            //if the stick has moved more then 90% of the way
            if (leftStick.magnitude > 0.9f) {
                //if we havent dashed on this press before
                if (!m_HasUsedOnButtonPress) {
                    m_HasUsedOnButtonPress = true;

                    //set up variables
                    m_IsDiving = true;
                    m_StartTime = Time.time;
                    m_PlayerArms.m_CanMoveArms = true;

                    //calc direction
                    Vector3 direction = leftStick.normalized;
                    direction = new Vector3(direction.x, 0, -direction.y);

                    //apply force
                    float moveSpeed = m_Distance / m_TimeTakenToDash;
                    m_Rigidbody.AddForce(direction * moveSpeed, ForceMode.Impulse);
                }
            }
        } else {
            m_HasUsedOnButtonPress = false;
        }

    }

}
