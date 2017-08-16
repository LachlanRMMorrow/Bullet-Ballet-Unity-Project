using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class PatrolAI : AI {

    public List<Transform> m_PatrolPoints;
    public int m_CurrentPath = 0;

    protected override void Start() {
        base.Start();
        
    }

    // Update is called once per frame
    protected override void Update() {

        //if we havent seen the player
        //and we have a patrol point
        //then look for the player and do pathing
        //else run the normal update
        if(!m_SeenPlayer && m_PatrolPoints.Count != 0) {
			updateLastKnownPosition();
			if (checkForPlayer()) {
                return;
            }

            //if we havent seen the player yet
            doPathing();
        } else {
            base.Update();
        }




    }

    private void doPathing() {
        //if the nav mesh doesnt have a path atm
        if (!m_NavMesh.hasPath) {
            //lerp from m_CurrentPath to m_CurrentPath % m_PatrolPoints.Count
            m_NavMesh.SetDestination(m_PatrolPoints[m_CurrentPath].position);
            m_CurrentPath = (m_CurrentPath + 1) % m_PatrolPoints.Count;
        }
    }
}
