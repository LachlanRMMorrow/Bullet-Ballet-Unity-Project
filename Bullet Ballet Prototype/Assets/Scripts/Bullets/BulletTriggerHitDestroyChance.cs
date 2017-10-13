using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletTriggerHitDestroyChance : BulletHitHandlerBase {

    [Range(0,1)]
    public float m_ChanceToDestroy = 0.5f;

    protected override void hit() {
        base.hit();

        if (Random.value >= m_ChanceToDestroy) {
            Destroy(m_BulletHitHandler.m_BulletHit.gameObject);
        }else {
            m_BulletHitHandler.m_BulletHit.GetComponent<Bullet>().m_ShouldStopAfterCollision = false;
        }
    }

}
