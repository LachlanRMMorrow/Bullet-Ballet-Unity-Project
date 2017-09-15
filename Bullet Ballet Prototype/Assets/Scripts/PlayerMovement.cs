using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class PlayerMovement : MonoBehaviour {
    public AudioClip clip;

    private NavMeshAgent m_NavMesh;

    private Vector3[] m_Positions;

    private int m_CurrentIndex = 1;

    private bool m_PathOver = true;

    private float m_LastDistance;

    public float m_NextNodeDistance = 1.0f;

    public int m_NumOfNodesModifyedEAfterDash = 15;

    // Use this for initialization
    void Start() {
        m_NavMesh = GetComponent<NavMeshAgent>();

        SoundManager.PlayBGM(clip, false, 2.0f);
    }

    // Update is called once per frame
    void Update() {
        if (SlowMoManager.m_isPaused) {
            return;
        }
        if (m_PathOver || m_Positions.Length == 0) {
            return;
        }
        if (!m_NavMesh.enabled) {
            return;
        }
        //print(m_CurrentIndex + " / " + m_Positions.Length);
        //print(Vector3.Distance(m_Positions[m_CurrentIndex], transform.position));

        float distance = Vector3.Distance(m_Positions[m_CurrentIndex], transform.position);
        //there seems to be a bug that caused the player to be outside the m_NextNodeDistance and also not moving
        //this will fix that, because if it's not moving then the distance will be equal to last distance
        if (distance == m_LastDistance) {
            m_LastDistance = 0;
            if (m_CurrentIndex >= m_Positions.Length - 1) {
                m_PathOver = true;
            } else {
                m_NavMesh.SetDestination(m_Positions[m_CurrentIndex]);
            }
            return;
        }
        m_LastDistance = distance;

        if (distance < m_NextNodeDistance) {
            //print("NEXT");
            m_CurrentIndex++;
            if (m_CurrentIndex >= m_Positions.Length - 1) {
                m_PathOver = true;
            } else {
                m_NavMesh.SetDestination(m_Positions[m_CurrentIndex]);
            }
        }
    }

    public void pathUpdated(Vector3[] a_Waypoints) {
        if (a_Waypoints.Length == 0) {
            m_NavMesh.SetDestination(transform.position);
            return;
        }
        m_CurrentIndex = 2;
        m_PathOver = false;
        m_Positions = a_Waypoints;
        m_NavMesh.SetDestination(m_Positions[m_CurrentIndex]);
        //m_NavMesh.SetDestination(a_WaypointPos);
    }

    public void modifyPath(Vector3 a_Direction) {
        if(m_Positions == null) {
            return;
        }
        Vector3 modifyedRatio = a_Direction;

        //how many positions do we want to change
        int numOfPositions = Mathf.Min(m_NumOfNodesModifyedEAfterDash, m_Positions.Length);
        for (int i = 0; i < numOfPositions; i++) {
            //apply offset of position
            m_Positions[i] += modifyedRatio;
            //calc how far we are through the positions
            //it's from 0 to 1, we want 1 to 0
            float scale = 1 - ((i + 1) / (float)numOfPositions);
            //update the modified right
            modifyedRatio = a_Direction * scale;
        }

    }

}

