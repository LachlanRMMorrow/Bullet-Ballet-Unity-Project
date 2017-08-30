using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletEnemy : Bullet {

    public float m_RandomDeviationAngle = 4.0f;

    protected override void Awake() {
        //apply m_RandomDeviationAngle offset to rotation
        Vector3 rot = transform.rotation.eulerAngles;
        rot.y += Random.Range(-1.0f, 1.0f) * m_RandomDeviationAngle;
        transform.rotation = Quaternion.Euler(rot);

        //call base bullet awake which then sets the force for the bullet
        base.Awake();
    }
    protected override void bulletHit(GameObject a_Object) {
        base.bulletHit(a_Object);
        //bulletCollision();
        //
        ////did hit the player?
        //if (a_Object.layer == LayerMask.NameToLayer("Player")) {
        //    //todo: leave body?
        //
        //    spawnBlood(a_Object.transform.position, transform.rotation);
        //    dealDamage(a_Object);
        //}
    }

}
