using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DashCharges : MonoBehaviour
{

    public int currentCharges;
    int previousCharges;

    GameObject chargesUI;

    public Texture chargeUsed;
    public Texture chargeReady;

	void Start ()
    {

        currentCharges = GameObject.Find("Player (Rigged)").GetComponent<PlayerDive>().m_DashChargesCurrent;
        previousCharges = currentCharges;
	}
	
	void Update ()
    {
        currentCharges = GameObject.Find("Player (Rigged)").GetComponent<PlayerDive>().m_DashChargesCurrent;

        chargesUI = GameObject.Find("Canvas").transform.GetChild(0).GetChild(3).GetChild(currentCharges).transform.gameObject;

        
        //Make charge show as used on UI
        if (currentCharges < previousCharges)
        {
            chargesUI.GetComponent<RawImage>().texture = chargeUsed;
        }
        else if (currentCharges > previousCharges)
        {
            chargesUI = GameObject.Find("Canvas").transform.GetChild(0).GetChild(3).GetChild(previousCharges).transform.gameObject;
            chargesUI.GetComponent<RawImage>().texture = chargeReady;
        }

        previousCharges = currentCharges;
    }

    void test()
    {

        
        
    }
}
