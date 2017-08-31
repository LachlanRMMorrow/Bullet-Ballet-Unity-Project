using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletSpawnParticleOnHit : BulletHitHandlerBase {

    public GameObject m_ObjectToSpwan;
    public float m_DeleteAfterTime = 1.0f;

    protected override void hit() {
        base.hit();

        //create object at position and rotation of bullet
        GameObject go = Instantiate(m_ObjectToSpwan, m_BulletHitHandler.m_BulletHit.transform.position, m_BulletHitHandler.m_BulletHit.transform.rotation);

        //then destroy it
        Destroy(go, m_DeleteAfterTime);
    }

}
