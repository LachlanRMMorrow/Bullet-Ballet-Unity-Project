using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BulletHitHandler))]
public class BulletHitHandlerBase : MonoBehaviour {

    [HideInInspector]
    public BulletHitHandler m_BulletHitHandler;

    public virtual void Awake() {
        m_BulletHitHandler = GetComponent<BulletHitHandler>();
        m_BulletHitHandler.m_ObjectHit.AddListener(hit);
    }

    virtual protected void hit() { }

}
