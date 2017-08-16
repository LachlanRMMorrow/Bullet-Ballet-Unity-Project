using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPathing : MonoBehaviour {

    public GameObject m_PathMarker;
    public float m_MoveSpeed;

    private PlayerMovement m_Player;

    private JInput.Controller m_Controller;

    // Use this for initialization
    void Awake() {
        m_Player = FindObjectOfType<PlayerMovement>();
        GameStateManager.singleton.m_StateChanged.AddListener(stateChanged);

        m_PathMarker.transform.position = m_Player.transform.position;

        if (m_PathMarker == null) {
            Debug.LogError("There is no WaypointMarker!");
        }

    }
	
	// Update is called once per frame
	void Update () {
        m_Controller = JInput.CurrentController.currentController;
        if (m_Controller == null) {
            return;
        }

        if (m_Controller.WasButtonPressed(Keys.singleton.m_PlanningModeApply)) {
            applyWaypoint();
        }

        //move point
        Vector3 leftStick = new Vector3(
            m_Controller.getAxisValue(Keys.singleton.m_PlanningWayPointMovementX),
            0,
            -m_Controller.getAxisValue(Keys.singleton.m_PlanningWayPointMovementY));

        Vector3 pos = transform.position;
        pos += leftStick * m_MoveSpeed * Time.unscaledDeltaTime;
        transform.position = pos;

    }

    private void applyWaypoint() {
        if(m_PathMarker != null) {
            m_PathMarker.transform.position = transform.position;
            if (m_Player != null) {
                m_Player.pathUpdated(m_PathMarker.transform.position);
            }
        }
    }

    private void stateChanged(GameStates a_NewState) {
        switch (a_NewState) {
            case GameStates.Action:
                gameObject.SetActive(false);

                break;
            case GameStates.Planning:
                gameObject.SetActive(true);
                break;
        }
        
    }

}
