using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DashCharges : MonoBehaviour {

    private int currentCharges;
    int previousCharges;

    GameObject chargesUI;

    public Texture chargeUsed;
    public Texture chargeReady;

    private PlayerDive m_PlayerDive;
    private Transform m_DashTransform;

    public Slider m_DashSlider;

    void Start() {
        m_PlayerDive = GameObject.Find("Player (Rigged)").GetComponent<PlayerDive>();
        m_DashTransform = transform;

        currentCharges = m_PlayerDive.m_DashChargesCurrent;
        previousCharges = currentCharges;
    }

    void Update() {
        float percentage = 1 - (m_PlayerDive.m_DashChargeTimerCurrent / m_PlayerDive.m_DashChargeTimerMax);
        float value = 1 - ((percentage / m_PlayerDive.m_DashChargesMax) + m_PlayerDive.m_DashChargesCurrent / (float)m_PlayerDive.m_DashChargesMax);
        //print(value);
        m_DashSlider.value = value;


        currentCharges = m_PlayerDive.m_DashChargesCurrent;

        chargesUI = m_DashTransform.GetChild(currentCharges + 1).transform.gameObject;

        //Make charge show as used on UI
        if (currentCharges < previousCharges) {
            if (previousCharges == m_PlayerDive.m_DashChargesMax) {
                chargesUI.SetActive(false);
            } else {
                chargesUI.GetComponent<RawImage>().texture = chargeUsed;
            }
        } else if (currentCharges > previousCharges) {
            chargesUI = m_DashTransform.GetChild(previousCharges + 1).gameObject;
            if (currentCharges == m_PlayerDive.m_DashChargesMax) {
                chargesUI.SetActive(true);
            } else {
                chargesUI.GetComponent<RawImage>().texture = chargeReady;
            }
        }

        previousCharges = currentCharges;
    }

}
