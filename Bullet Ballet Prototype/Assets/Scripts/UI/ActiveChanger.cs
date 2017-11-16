using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActiveChanger : MonoBehaviour {

    public GameObject m_ObjectToCopyTo;

    private void OnDisable() {
        m_ObjectToCopyTo.SetActive(false);
    }

    private void OnEnable() {
        m_ObjectToCopyTo.SetActive(true);
    }


}
