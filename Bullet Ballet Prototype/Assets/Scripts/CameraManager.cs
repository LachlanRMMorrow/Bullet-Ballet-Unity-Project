using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour {

    #region Variables

    /// <summary>
    /// transform for the camera
    /// </summary>
    public Transform m_CameraTransform;
    public GameObject m_EventSystem;

    #region Static Common Variables
    static readonly Vector3 YMASK = new Vector3(1, 0, 1);
    #endregion

    #region Dynamic Camera Variables
    [Header("Dynamic Camera")]

    public float m_PlanningModeMinHeight = 10.0f;
    [Range(1,20)]
    public float m_PlanningModeYDistanceScale = 15.0f;

    [Range(10,100)]
    public float m_ActionModeHeight_TEMP = 20.0f;

    [Range(0, 50)]
    public float m_CameraMoveSpeed = 2.0f;
    [Range(0, 50)]
    public float m_DistanceForMaxSpeed = 10.0f;
    [Range(0, 1)]
    public float m_MinSpeedScale = 0.01f;
    [Range(10,150)]
    public float m_MinYHeight = 20.0f;

    private Transform m_WayPointTransform;

    private Transform m_PlayerTransform;
    private Transform m_PlayerLeftArmTransform;
    private Transform m_PlayerRightArmTransform;

    private float m_MaxDistancePathHasBeen = 0.0f;
    #endregion

    #region Camera Shake Variables
    [Header("Camera Shake")]
    /// <summary>
    /// transform for object that holds the camera
    /// </summary>
    public Transform m_CameraShakeHolderTransform;
    private Vector3 m_ShakeStartPoint;
    public Transform m_ShakeUIHolder;
    private Vector3 m_ShakeUIStartPoint;
    public AnimationCurve m_CameraShakeCurve = AnimationCurve.Linear(0, 1, 1, 1);
    public float m_ShakeAmountScale = 0.5f;
    public AnimationCurve m_UIShakeCurve = AnimationCurve.Linear(0, 1, 1, 1);
    public float m_UIShakeAmountScale = 10.0f;
    public float m_ShakeTime = 0.1f;
    private bool m_RunningShake;
    private float m_ShakeStartTime = 0;
    #endregion


    #region Camera Intro Variables
    [Header("Intro")]
    public float m_IntroLength = 5.0f;
    /// <summary>
    /// Curve for the camera intro so calculate how much rotation the camera should have during the animation
    /// </summary>
    public AnimationCurve m_CameraIntroCurve = AnimationCurve.Linear(0, 0, 1, 1);
    /// <summary>
    /// is the intro running
    /// the intro rotates the camera down
    /// </summary>
    private bool m_RunningIntroCamera = true;

    private float m_StartingTime;
    private bool m_HasStartedIntro = false;
    #endregion

    #endregion

    #region Unity Functions

    // Use this for initialization
    void Start() {

        m_EventSystem = GameObject.Find("EventSystem");

        checkScreenShakeVariables();

        getDynCameraVariables();

        GameStateManager.singleton.m_StateChanged.AddListener(stateChanged);

        m_CameraTransform.localRotation = Quaternion.Euler(new Vector3(0, 0, 0));

    }

    // Update is called once per frame
    void Update() {

        //do the camera intro first
        if (m_RunningIntroCamera) {
            runIntroCamera();
            return;
        }

        //dont do anything if the player hasent done anything
        if (!Player.m_HasPlayerDoneAnything) {
            return;
        }

        //dont do anything if the game is paused or the player is dead
        if (SlowMoManager.m_isPaused || !Player.m_PlayerAlive) {
            return;
        }

        //do screen shake first
        runScreenShake();

        //do main camera update
        dynamicCameraUpdate();

    }
    #endregion

    #region Camera Functions

    #region Camera Intro Functions

    private void runIntroCamera() {
        if (!m_HasStartedIntro) {
            m_HasStartedIntro = true;
            m_StartingTime = Time.time;
        }
        //pause the game until the camera animation is done
        SlowMoManager.m_isPaused = true;
        //calculate percentage through the intro
        float amount = (Time.time - m_StartingTime) / m_IntroLength;

        //limit to a max of 1, and finish the intro
        if (amount >= 1) {
            amount = 1;
            m_RunningIntroCamera = false;
            //camera animation done, unpause the game to let the player play
            //SlowMoManager.m_isPaused = false;

            GameObject.Find("Canvas").transform.Find("Weapon Select Menu").gameObject.SetActive(true);
            m_EventSystem.GetComponent<WeaponSelectMenu>().WeaponMenuActive();


        }


        float curveAmount = m_CameraIntroCurve.Evaluate(amount);

        //calculate rotation and apply
        Quaternion newRot = Quaternion.Euler(new Vector3(curveAmount * 90, 0, 0));
        m_CameraTransform.localRotation = newRot;

    }

    #endregion


    #region Dynamic Camera Functions

    private void getDynCameraVariables() {
        m_PlayerTransform = FindObjectOfType<Player>().transform;

        m_WayPointTransform = GameObject.Find("Waypoint System").transform.GetChild(0);
    }

    private void dynamicCameraUpdate() {
        switch (GameStateManager.currentState) {
            case GameStates.Action:
                dynamicCameraAction();
                break;
            case GameStates.Planning:
                dynamicCameraPlanning();
                break;
        }
    }

    private void dynamicCameraPlanning() {
        
        float distance = Vector3.Distance(m_WayPointTransform.position, m_PlayerTransform.position);

        if(distance > m_MaxDistancePathHasBeen) {
            //set max distance to the current, distance will be larger then the last largest value
            m_MaxDistancePathHasBeen = distance;
        }else {
            //max distance is larger so just set distance to the largest value
            distance = m_MaxDistancePathHasBeen;
        }        

        Vector3 centerPoint = m_WayPointTransform.position - m_CameraTransform.position;

        float calcYPos = Mathf.Sqrt(distance) * m_PlanningModeYDistanceScale;

        lerpToPoint(centerPoint);
        lerpYpos(calcYPos);
    }

    private void dynamicCameraAction() {

        //todo write a real action camera here

        lerpToPoint(m_PlayerTransform.position);
        lerpYpos(m_ActionModeHeight_TEMP);
    }

    private void lerpToPoint(Vector3 a_TargetPosition) {
        Vector3 newCamPos;

        Vector3 startingPos = Vector3.Scale(YMASK, m_CameraTransform.position);
        Vector3 targetPos = Vector3.Scale(YMASK, a_TargetPosition);

        float distance = Vector3.Distance(startingPos, targetPos);

        float speedScale = Mathf.Clamp01((distance / m_DistanceForMaxSpeed) + m_MinSpeedScale);

        newCamPos = Vector3.MoveTowards(startingPos, targetPos, Time.unscaledDeltaTime * m_CameraMoveSpeed * speedScale);

        newCamPos.y = m_CameraTransform.position.y;

        m_CameraTransform.position = newCamPos;
    }

    private void lerpYpos(float a_TargetY) {
        Vector3 newCamPos = m_CameraTransform.position;

        //print(a_TargetY / newCamPos.y);
        a_TargetY = Mathf.Clamp(a_TargetY, m_MinYHeight, 9999);

        newCamPos.y = Mathf.Lerp(newCamPos.y, a_TargetY, Time.unscaledDeltaTime * 1.0f);


        m_CameraTransform.position = newCamPos;
    }

    #endregion

    #region Screen Shake

    private void checkScreenShakeVariables() {
        if (m_CameraShakeHolderTransform == null) {
            Debug.LogWarning("Camera manager is missing reference to the Camera Shake Transform");
        }
        if (m_ShakeUIHolder == null) {
            Debug.LogWarning("Camera manager is missing reference to the UI Camera Shake Transform");
        }

        if (m_CameraShakeHolderTransform != null) {
            m_ShakeStartPoint = m_CameraShakeHolderTransform.position;
        }
        if (m_ShakeUIHolder != null) {
            m_ShakeUIStartPoint = m_ShakeUIHolder.position;
        }
    }

    private void runScreenShake() {
        //prevent shake in anything other then Action mode
        if (GameStateManager.currentState != GameStates.Action) {
            return;
        }
        if (m_RunningShake) {
            float shakeDuration = (Time.time - m_ShakeStartTime) / m_ShakeTime;
            if (shakeDuration > 1.0f) {
                m_RunningShake = false;
            }
            if (m_CameraShakeHolderTransform != null) {
                float shakeAmount = m_CameraShakeCurve.Evaluate(shakeDuration);
                Vector3 shakeDir = Random.insideUnitSphere * m_ShakeAmountScale * shakeAmount;
                m_CameraShakeHolderTransform.transform.position = m_ShakeStartPoint + shakeDir;
            }
            if (m_ShakeUIHolder != null) {
                float shakeAmount = m_UIShakeCurve.Evaluate(shakeDuration);
                Vector3 shakeDir = Random.insideUnitSphere * m_UIShakeAmountScale * shakeAmount;
                m_ShakeUIHolder.transform.position = m_ShakeUIStartPoint + shakeDir;
            }
        } else {
            if (m_CameraShakeHolderTransform != null) {
                m_CameraShakeHolderTransform.transform.position = Vector3.Lerp(m_CameraShakeHolderTransform.transform.position, m_ShakeStartPoint, Time.deltaTime * 2);
            }
            if (m_ShakeUIHolder != null) {
                m_ShakeUIHolder.transform.position = Vector3.Lerp(m_ShakeUIHolder.transform.position, m_ShakeUIStartPoint, Time.deltaTime * 2);
            }
        }
    }


    public void startScreenShake() {
        m_ShakeStartTime = Time.time;
        m_RunningShake = true;
    }

    #endregion


    #region Other Functions

    private void stateChanged(GameStates a_NewState) {
        switch (a_NewState) {
            case GameStates.Planning:
                //when going into planning mode, reset the last max distance
                m_MaxDistancePathHasBeen = 0.0f;
                break;
            case GameStates.Action:
                break;
        }
    }

    #endregion

    #endregion
}
