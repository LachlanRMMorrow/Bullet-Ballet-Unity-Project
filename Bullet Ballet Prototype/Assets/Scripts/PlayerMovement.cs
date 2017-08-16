using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class PlayerMovement : MonoBehaviour {

    private NavMeshAgent m_NavMesh;

	// Use this for initialization
	void Start () {
        m_NavMesh = GetComponent<NavMeshAgent>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void pathUpdated(Vector3 a_WaypointPos) {
        m_NavMesh.SetDestination(a_WaypointPos);
    }
}
