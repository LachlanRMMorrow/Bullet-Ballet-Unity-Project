using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class LaserSight : MonoBehaviour {

    /// <summary>
    /// reference to the line renderer
    /// </summary>
    public LineRenderer m_LineRender;

    /// <summary>
    /// Distance for the laser
    /// </summary>
    [Range(0, 20)]
    public float m_Distance = 10;

    public LayerMask m_LayerMask;

    // Use this for initialization
    void Awake() {
        m_LineRender = GetComponent<LineRenderer>();
        
    }

    // Update is called once per frame
    void Update() {
        //set the starting position to be the transforms position
        m_LineRender.SetPosition(0, transform.position);

        Vector3 forward = transform.forward;

        Vector3 position = transform.position;

        //if the arm is closer then -0.5 or above of the way to being flat/facing up
        //then run the ray-cast
        //this is because when the arm is facing down it has a dot of less then -0.5 and we don't want to do a ray-cast thats facing the floor, instead just having no ray-cast
        //this might be changed later, maybe when the laserSight is lined up better, this wont be a problem
        if (Vector3.Dot(Vector3.up, forward) > -0.5f) {

            //raycast only hitting the walls, cover and default object
            RaycastHit hit;
            if (Physics.Raycast(transform.position, forward, out hit, m_Distance, m_LayerMask.value)) {
                //and object is not the floor
                position = hit.point;

            } else {
                //if the raycast did not hit then just set it to be the max distance
                position = transform.position + (forward * m_Distance);
            }
        }

        //apply the 2nd position
        m_LineRender.SetPosition(1, position);


    }

    void OnValidate() {
        if(m_LineRender == null) {
            m_LineRender = GetComponent<LineRenderer>();
            m_LineRender.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            m_LineRender.receiveShadows = false;
            m_LineRender.widthMultiplier = 0.1f;
            m_LayerMask.value = (1 << LayerMask.NameToLayer("Walls") | 1 << LayerMask.NameToLayer("Cover") | 1 << LayerMask.NameToLayer("Default"));
        }
    }
}
