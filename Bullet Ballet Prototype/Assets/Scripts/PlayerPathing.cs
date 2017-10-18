using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPathing : MonoBehaviour {

    public GameObject m_PathMarker;
    public float m_MoveSpeed;

    private PlayerMovement m_Player;

    private JInput.Controller m_Controller;

    /// <summary>
    /// list of all points across the path
    /// the last position will always hold the current position
    /// </summary>
    private List<Vector3> m_Positions = new List<Vector3>();

    public LineRenderer m_Line;

    private Vector3 m_LastPos;

    // Use this for initialization
    void Awake() {
        m_Player = FindObjectOfType<PlayerMovement>();

        m_Player.setLineRenderer(m_Line);

        GameStateManager.singleton.m_StateChanged.AddListener(stateChanged);

        m_PathMarker.transform.position = m_Player.transform.position;

        if (m_PathMarker == null) {
            Debug.LogError("There is no WaypointMarker!");
        }

        //update position of this object to the players starting pos
        Vector3 pos = m_Player.transform.position;
        pos.y = transform.position.y;
        transform.position = pos;

    }

    // Update is called once per frame
    void FixedUpdate() {
        m_Controller = JInput.CurrentController.currentController;
        if (m_Controller == null) {
            return;
        }
        if (m_Player == null) {
            return;
        }

        //move point
        Vector3 leftStick = new Vector3(
            m_Controller.getAxisValue(Keys.singleton.m_PlanningWayPointMovementX),
            0,
            -m_Controller.getAxisValue(Keys.singleton.m_PlanningWayPointMovementY));

        if(leftStick.magnitude >= 0.1f) {
            Player.m_HasPlayerDoneAnything = true;
        }

        //update position of object
        Vector3 pos = transform.position;
        Vector3 movement = leftStick * m_MoveSpeed * Time.unscaledDeltaTime;

        //RaycastHit hit;
        //if (GetComponent<Rigidbody>().SweepTest(movement.normalized / 2, out hit, 0.25f)) {
        //    pos += movement/8;
        //    print(hit.distance);
        //} else {
        pos += movement;
        //}
        //transform.position = pos;
        GetComponent<Rigidbody>().MovePosition(pos);
        GetComponent<Rigidbody>().velocity = Vector3.zero;

        //todo: a better way to do this would be to use a dot, and use the difference in angle
        if (Vector3.Distance(pos, m_LastPos) > 1) {
            //m_Positions.Insert(m_Positions.Count-1,pos);
            m_Positions.Add(Vector3.zero);
            m_LastPos = pos;
        }
        //apply the current position to the last element in the array
        m_Positions[m_Positions.Count - 1] = transform.position;




        //update line
        m_Line.positionCount = m_Positions.Count;
        m_Line.SetPositions(m_Positions.ToArray());
    }

    private void applyWaypoint() {

        if (m_PathMarker != null) {
            m_PathMarker.transform.position = transform.position;
            if (m_Player != null) {
                if (m_Positions.Count >= 3) {

                    m_Player.pathUpdated(m_Positions.ToArray());
                }
            }
        }
    }

    private void stateChanged(GameStates a_NewState) {
        if (m_Player == null) {
            return;
        }
        switch (a_NewState) {
            case GameStates.Action:
                applyWaypoint();
                gameObject.SetActive(false);
                break;
            case GameStates.Planning:
                gameObject.SetActive(true);
                m_Positions.Clear();
                m_Line.positionCount = 0;
                transform.position = Vector3.Scale(new Vector3(1, 0, 1), m_Player.transform.position) + new Vector3(0,0.5f,0);
                m_LastPos = transform.position;
                m_Positions.Add(transform.position);
                m_Positions.Add(transform.position);
                break;
        }

    }



}
