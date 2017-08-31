using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Health))]
public class Pillar : BulletHitHandlerBase {

    public Transform m_PillarTransforms;
    public Transform m_PillarCollider;

    Health m_Health;

    public override void Awake() {
        base.Awake();
        
        swapPillars(true);

        m_Health = GetComponent<Health>();

        //small hack to get the colliders bullet info.. since when passing them through to this class's version
        m_BulletHitHandler = m_PillarCollider.GetComponent<BulletHitHandler>();
        
    }

    protected override void hit() {
        base.hit();

        m_Health.dealDamage(1.0f);

        if (m_Health.isDead()) {

            m_PillarCollider.gameObject.SetActive(false);
            swapPillars(false);

            Vector3 direction = m_BulletHitHandler.m_BulletHit.rotation * Vector3.forward;

            for (int i = 0; i < 4; i++) {
                for (int q = 0; q < 8; q++) {
                    int randomChild = Random.Range(0, 21);
                    Rigidbody middlePeice = m_PillarTransforms.GetChild(i).GetChild(1).GetChild(0).GetChild(randomChild).GetComponent<Rigidbody>();
                    middlePeice.AddForce(direction * 20, ForceMode.Impulse);
                }
            }
        }
    }

    private void swapPillars(bool a_NormalActiveState) {
        for (int i = 0; i < 4; i++) {
            Transform currTransform = m_PillarTransforms.GetChild(i);

            //set the normal pillar to be inactive
            currTransform.GetChild(0).gameObject.SetActive(a_NormalActiveState);

            //set the destroyed pillar to be active
            currTransform.GetChild(1).gameObject.SetActive(!a_NormalActiveState);
        }
    }

}
