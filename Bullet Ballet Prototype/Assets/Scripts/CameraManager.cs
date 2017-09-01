using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour {

    class MaxDistance {
        public Vector3 m_Max;
        public float m_Distance;
        private Vector3 m_Center;

        public MaxDistance(Vector3 a_Pos) {
            m_Max = Vector3.zero;
            m_Center = Vector3.Scale(new Vector3(1, 0, 1), a_Pos);
            m_Distance = 0;
        }

        /// <summary>
        /// checks if a_Pos is larger then the max distance
        /// if it is, then this will update to use that value
        /// </summary>
        /// <param name="a_Pos">position you want to test</param>
        /// <returns>a_Pos</returns>
        public Vector3 AddValue(Vector3 a_Pos) {
            float distance = Vector3.Distance(m_Center, Vector3.Scale(new Vector3(1, 0, 1), a_Pos));
            if (distance > m_Distance) {
                m_Max = a_Pos;
                m_Distance = distance;
            }
            return a_Pos;
        }
    }

    /// <summary>
    /// transform for the camera
    /// </summary>
    public Transform m_CameraTransform;


    /// <summary>
    /// When used with orthographic this is the min zoom for the orthograpic size.
    /// 
    /// When used with perspective this will affect the min height for the camera,
    /// </summary>
    [Header("Action Mode")]
    public float m_MinZoomDist = 20.0f;
    /// <summary>
    /// When used with orthographic this is the max zoom for the orthograpic size.
    /// 
    /// When used with perspective this will affect the max height for the camera,
    /// </summary>
    public float m_MaxZoomDist = 50.0f;
    /// <summary>
    /// how much is multiplyed to the zoom scale when working out it's size/height
    /// </summary>
    public float m_AddZoomSizeScale = 4.0f;
    /// <summary>
    /// scale to change how quick the camera will move
    /// </summary>
    public float m_MoveSpeedScale = 0.25f;
    /// <summary>
    /// scale to change how quick the camera will scale in ortho size or height
    /// </summary>
    public float m_ZoomSpeedScale = 0.25f;
    /// <summary>
    /// reference to the player
    /// </summary>
    public Transform m_PlayerTransform;
    /// <summary>
    /// reference to the waypoint marker
    /// </summary>
    public Transform m_WaypointMarkerTransform;
    /// <summary>
    /// reference to the players left arm
    /// </summary>
    public Transform m_PlayerArmLeft;
    /// <summary>
    /// reference to the players right arm
    /// </summary>
    public Transform m_PlayerArmRight;

    /// <summary>
    /// position for the camera while the game is in planning mode
    /// </summary>
    [Header("Planning Mode")]
    public Transform m_PlanningModeCamPos;
    /// <summary>
    /// starting ortho size for camera
    /// </summary>
    private float m_StartingOrthoSize;
    /// <summary>
    /// starting position for the camera
    /// </summary>
    private Vector3 m_StartingCamPos;

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

    /// <summary>
    /// reference to the main camera
    /// </summary>
    private Camera m_Camera;

    // Use this for initialization
    void Start() {
        m_CameraTransform.position = m_StartingCamPos = m_PlanningModeCamPos.position;
        m_Camera = Camera.main;
        m_StartingOrthoSize = m_Camera.orthographicSize;
        if (m_CameraShakeHolderTransform != null) {
            m_ShakeStartPoint = m_CameraShakeHolderTransform.position;
        }
        if (m_ShakeUIHolder != null) {
            m_ShakeUIStartPoint = m_ShakeUIHolder.position;
        }
    }

    // Update is called once per frame
    void Update() {
        if (SlowMoManager.m_isPaused) {
            return;
        }
        runScreenShake();

        setCamPos();
    }

    private void runScreenShake() {
        //prevent shake in anything other then Action mode
        if(GameStateManager.currentState != GameStates.Action) {
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

    public void setCamPos() {
        if (m_PlayerTransform == null) {
            return;
        }
        Vector3 moveToPos = Vector3.zero;
        //change what the camera will do depending the on the game state
        switch (GameStateManager.currentState) {
            //action:
            case GameStates.Action: {
                    //create a pointer(class) to a MaxDistance object
                    //this is used to find the furthest point that is found during the calcActionMovePos function
                    MaxDistance minMax = new MaxDistance(m_CameraTransform.position);
                    //get average pos and max dist
                    moveToPos = calcActionModePos(minMax);
                    //scale that distance
                    float dist = minMax.m_Distance * m_AddZoomSizeScale;
                    //clamp the distance
                    dist = Mathf.Clamp(dist, m_MinZoomDist, m_MaxZoomDist);
                    //apply a lerp to it
                    lerpCameraOrtho(dist, m_ZoomSpeedScale);
                    break;
                }
            case GameStates.Planning:
                moveToPos = m_PlanningModeCamPos.position;
                if (m_Camera.orthographic) {
                    lerpCameraOrtho(m_StartingOrthoSize, 1.0f);
                } else {
                    lerpCameraOrtho(m_StartingCamPos.y, m_ZoomSpeedScale);
                }
                break;
            case GameStates.Paused:
                return;
        }
        m_CameraTransform.position = Vector3.Lerp(
            Vector3.Scale(new Vector3(1, 0, 1), m_CameraTransform.position),
            Vector3.Scale(new Vector3(1, 0, 1), moveToPos),
            Time.unscaledDeltaTime * m_MoveSpeedScale)
            + new Vector3(0, m_CameraTransform.position.y, 0);
    }

    /// <summary>
    /// Works out the average point and the furtest point that are considerd interesting
    /// a_MinMax will contain the max distance and the point of that object of those 'interesting objects'
    /// </summary>
    /// <param name="a_MinMax">Reference to a MinMax Class</param>
    /// <returns>Average point</returns>
    private Vector3 calcActionModePos(MaxDistance a_MinMax) {

        Vector3 pos = m_PlayerTransform.position;
        if (Vector3.Distance(m_PlayerTransform.position, m_WaypointMarkerTransform.position) > 2) {
            pos += a_MinMax.AddValue(m_WaypointMarkerTransform.position);
            pos /= 2;
        }

        Vector3 armRaycastAverage = Vector3.zero;
        int valuesAdded = 0;

        RaycastHit hit;
        //check against everything that is not a Bullets/Eneimes or Player
        //int layerMask = ~(1 << LayerMask.NameToLayer("Bullets") | 1 << LayerMask.NameToLayer("Enemies") | 1 << LayerMask.NameToLayer("Player"));
        int layerMask = (1 << LayerMask.NameToLayer("Walls") | 1 << LayerMask.NameToLayer("Default"));
        //add left arm
        if (m_PlayerArmLeft.gameObject.activeInHierarchy) {
            //check AI for LayerMask explination
            //raycast from AI in the player direction for 1000 units, excluding anything on the Bullets Layer
            if (Physics.Raycast(m_PlayerArmLeft.position, m_PlayerArmLeft.forward, out hit, 1000, layerMask)) {
                armRaycastAverage += a_MinMax.AddValue(hit.point);
                valuesAdded++;
            }
        }
        //add right arm
        if (m_PlayerArmRight.gameObject.activeInHierarchy) {
            //check AI for LayerMask explination
            //raycast from AI in the player direction for 1000 units, excluding anything on the Bullets Layer
            if (Physics.Raycast(m_PlayerArmRight.position, m_PlayerArmRight.forward, out hit, 1000, layerMask)) {
                armRaycastAverage += a_MinMax.AddValue(hit.point);
                valuesAdded++;
            }
        }

        if (valuesAdded != 0) {
            //average out the movement
            armRaycastAverage /= valuesAdded;

            pos += armRaycastAverage;
            pos /= 2;

        }

        //and return
        return pos;
    }

    private void lerpCameraOrtho(float a_DesiredSize, float a_Scale) {
        if (m_Camera.orthographic) {
            m_Camera.orthographicSize = Mathf.Lerp(m_Camera.orthographicSize, a_DesiredSize, Time.unscaledDeltaTime * a_Scale);
        } else {
            Vector3 pos = m_CameraTransform.position;
            pos.y = a_DesiredSize;
            m_CameraTransform.position = Vector3.Lerp(m_CameraTransform.position, pos, Time.unscaledDeltaTime * a_Scale);
        }
    }

    public void startScreenShake() {
        m_ShakeStartTime = Time.time;
        m_RunningShake = true;
    }

    //void OnDrawGizmos() {
    //
    //    if (!Application.isPlaying) {
    //        return;
    //    }
    //    Gizmos.color = Color.red;
    //
    //    MaxDistance minMax = new MaxDistance(m_CameraTransform.position);
    //    //get average pos and max dist
    //    Vector3 moveToPos = calcActionModePos(minMax);
    //    //scale that distance
    //    float dist = minMax.m_Distance * m_AddZoomSizeScale;
    //    //clamp the distance
    //    dist = Mathf.Clamp(dist, m_MinZoomDist, m_MaxZoomDist);
    //
    //    Gizmos.DrawSphere(moveToPos, 2);
    //}
}
