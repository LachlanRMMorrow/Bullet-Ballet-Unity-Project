using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class BulletHitHandler : MonoBehaviour {

    public UnityEvent m_ObjectHit;

    [HideInInspector]
    /// <summary>
    /// reference to the bullet that hit this object
    /// </summary>
    public Transform m_BulletHit;

    public void invoke() {
        m_ObjectHit.Invoke();
    }

    public void tellParent() {
        BulletHitHandler bhh = transform.GetComponentInParent<BulletHitHandler>();
        if(bhh != null) {
            bhh.m_BulletHit = m_BulletHit;
            bhh.m_ObjectHit.Invoke();
        }
    }
}
