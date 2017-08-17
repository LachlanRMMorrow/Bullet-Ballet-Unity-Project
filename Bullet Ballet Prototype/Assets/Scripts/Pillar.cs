using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pillar : MonoBehaviour {

    public Transform m_PillarTransforms;
    public Transform m_PillarCollider;

    public int m_MaxHealth = 2;
    private int m_CurrentHealth = 0;

    void Awake() {
        m_CurrentHealth = m_MaxHealth;
        swapPillars(true);
    }

    public void pillarHit(Transform m_BulletObject) {
        m_CurrentHealth--;
        if(m_CurrentHealth != 0) {
            return;
        }
        m_PillarCollider.gameObject.SetActive(false);
        swapPillars(false);
        
        Vector3 direction = m_BulletObject.rotation * Vector3.forward;

        for (int i = 0; i < 4; i++) {
            for (int q = 0; q < 8; q++) {
                int randomChild = Random.Range(0, 21);
                Rigidbody middlePeice = m_PillarTransforms.GetChild(i).GetChild(1).GetChild(0).GetChild(randomChild).GetComponent<Rigidbody>();
                middlePeice.AddForce(direction * 20, ForceMode.Impulse);
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
