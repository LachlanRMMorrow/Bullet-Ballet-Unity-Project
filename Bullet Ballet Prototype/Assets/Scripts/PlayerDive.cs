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
    //[Range(0,10)]
    //public float m_DashCooldown;

    /// <summary>
    /// ammount of dash charges
    /// </summary>
    //[Range(0, 10)]
    public int m_DashChargesMax;

    public int m_DashChargesCurrent;

    /// <summary>
    /// dash charge timer
    /// </summary>
    //[Range(0, 10)]
    public float m_DashChargeTimerMax;

    public float m_DashChargeTimerCurrent;

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

    private Vector3 m_Direction;

    private Vector3 m_StartPos;


    //references to components that are on the player
    private PlayerArms m_PlayerArms;
    private UnityEngine.AI.NavMeshAgent m_NavMesh;
    private Rigidbody m_Rigidbody;
    private PlayerMovement m_Movement;

    private Animator m_Animator;
    public RuntimeAnimatorController m_Controller;
    private RuntimeAnimatorController m_StartingController;

    void Start() {
        
        m_PlayerArms = GetComponent<PlayerArms>();
        m_Movement = GetComponent<PlayerMovement>();
        m_NavMesh = GetComponent<UnityEngine.AI.NavMeshAgent>();
        m_Rigidbody = GetComponent<Rigidbody>();
        
        m_StartTime = -1000;

        GameStateManager.singleton.m_StateChanged.AddListener(stateChanged);

        m_Rigidbody.isKinematic = true;

        m_DashChargesCurrent = m_DashChargesMax;
        m_DashChargeTimerCurrent = m_DashChargeTimerMax;

        m_Animator = transform.GetChild(2).GetChild(0).GetComponent<Animator>();
        m_StartingController = m_Animator.runtimeAnimatorController;
    }

    void Update() {
        if (m_DashChargesCurrent < m_DashChargesMax)
        {
            m_DashChargeTimerCurrent -= Time.deltaTime;
            if (m_DashChargeTimerCurrent <= 0)
            {
                m_DashChargeTimerCurrent = m_DashChargeTimerMax;
                m_DashChargesCurrent += 1;

            }
        }        

        if (m_IsDiving) {
            runDive();
        } else {
            checkDive();
        }
    }

    private void runDive() {

        GetComponent<Health>().m_IsInvincible = true;
            bool isDiveOver = m_StartTime + m_TimeTakenToDash < Time.time;
            m_PlayerArms.m_CanMoveArms = isDiveOver;
            //if the time is up then stop the dive
            if (isDiveOver)
            {
                m_Animator.runtimeAnimatorController = m_StartingController;

                m_IsDiving = false;
                m_Rigidbody.velocity = Vector3.zero;

                float distScale = Vector3.Distance(m_StartPos, transform.position) / m_Distance;

                m_Movement.modifyPath(m_Direction * m_Distance * distScale);

                m_PlayerArms.m_CanMoveArms = m_NavMesh.enabled = m_Rigidbody.isKinematic = true;

                m_DashChargesCurrent--;
            GetComponent<Health>().m_IsInvincible = false;

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

        if (m_DashChargesCurrent <= 0) {
            return;
        }

        //if the dive is on cool down then pretend the player didn't press the Dive/Dash button
        //if(m_StartTime + m_TimeTakenToDash + m_DashCooldown > Time.time) {
        //    isAboutToDive = false;
        //}

        if (isAboutToDive) {

            Vector2 leftStick = controller.getAxisValue(JInput.ControllerVec2Axes.LStick);

            //if the stick has moved more then 90% of the way
            if (leftStick.magnitude > 0.9f) {
                //if we havent dashed on this press before
                if (!m_HasUsedOnButtonPress) {
                    m_HasUsedOnButtonPress = true;
                    
                    m_PlayerArms.m_CanMoveArms = m_NavMesh.enabled = m_Rigidbody.isKinematic = false;

                    //set up variables
                    m_IsDiving = true;
                    m_StartTime = Time.time;
                    m_PlayerArms.m_CanMoveArms = true;

                    //calc direction
                    m_Direction = leftStick.normalized;
                    m_Direction = new Vector3(m_Direction.x, 0, -m_Direction.y);

                    //apply force
                    float moveSpeed = m_Distance / m_TimeTakenToDash;
                    m_Rigidbody.AddForce(m_Direction * moveSpeed, ForceMode.Impulse);

                    Player.m_HasPlayerDoneAnything = true;

                    m_StartPos = transform.position;

                    m_Animator.runtimeAnimatorController = m_Controller;

                    //calc resulting angle
                    float angle = Mathf.Atan2(leftStick.x, -leftStick.y) * Mathf.Rad2Deg;

                    //get a quaternion version of angle
                    Quaternion endRotation = Quaternion.Euler(new Vector3(0, angle, 0));

                    transform.rotation = endRotation;
                }
            }
        } else {
            m_HasUsedOnButtonPress = false;
        }

    }

    private void stateChanged(GameStates a_NewState) {
        switch (a_NewState) {
            case GameStates.Planning:
                if (m_IsDiving) {
                    quickendDash();
                }
                break;
            case GameStates.Action:
                break;
            default:
                break;
        }
    }

    private void quickendDash() {
        m_IsDiving = false;
        m_StartTime -= 1000;
        runDive();

        transform.position = transform.position + new Vector3(0,-1,0);
    }

    private void startDiveAnimation() {
        //m_Animator.SetTrigger("RunRoll");
       
    }


}
