using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pillar : MonoBehaviour {

    public Transform m_PillarTransforms;
    public Transform m_PillarCollider;

    void Awake() {
        swapPillars(true);
    }

    public void pillarHit(Transform m_BulletObject) {
        m_PillarCollider.gameObject.SetActive(false);
        swapPillars(false);

        Vector3 direction = Vector3.Scale(new Vector3(1,0,1),m_BulletObject.position - m_PillarTransforms.position).normalized;

        for (int i = 0; i < 4; i++) {
            Rigidbody middlePeice = m_PillarTransforms.GetChild(i).GetChild(1).GetChild(0).GetChild(11).GetComponent<Rigidbody>();
            middlePeice.AddForce(direction * 50, ForceMode.Impulse);
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
