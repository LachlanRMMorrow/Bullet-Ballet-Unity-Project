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

    // Use this for initialization
    void Start() {
        m_NavMesh = GetComponent<NavMeshAgent>();

        SoundManager.PlayBGM(clip, true, 2.0f);
    }

    // Update is called once per frame
    void Update() {
        if (SlowMoManager.m_isPaused) {
            return;
        }
        if (m_PathOver || m_Positions.Length == 0) {
            return;
        }
        //print(m_CurrentIndex + " / " + m_Positions.Length);
        //print(Vector3.Distance(m_Positions[m_CurrentIndex], transform.position));
        if (Vector3.Distance(m_Positions[m_CurrentIndex], transform.position) < 1.0f) {
            m_CurrentIndex++;
            if (m_CurrentIndex >= m_Positions.Length - 1) {
                m_PathOver = true;
            } else {
                m_NavMesh.SetDestination(m_Positions[m_CurrentIndex]);
            }
        }
    }

    public void pathUpdated(Vector3[] a_Waypoints) {
        if(a_Waypoints.Length == 0) {
            m_NavMesh.SetDestination(transform.position);
            return;
        }
        m_CurrentIndex = 2;
        m_PathOver = false;
        m_Positions = a_Waypoints;
        m_NavMesh.SetDestination(m_Positions[m_CurrentIndex]);
        //m_NavMesh.SetDestination(a_WaypointPos);
    }

}

