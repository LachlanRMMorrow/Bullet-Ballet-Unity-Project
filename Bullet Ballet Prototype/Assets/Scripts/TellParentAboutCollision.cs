using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class UnityEventTrigger : UnityEvent<Collider> { }

public class TellParentAboutCollision : MonoBehaviour {
    public UnityEventTrigger m_TriggerEnter;

    public void OnTriggerEnter(Collider other) {
        m_TriggerEnter.Invoke(other);
    }

}
