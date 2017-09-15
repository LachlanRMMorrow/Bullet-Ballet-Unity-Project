using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WeaponSelectMenu : MonoBehaviour {

    WeaponTypes weapon1;
    WeaponTypes weapon2;


	void Start ()
    {
        weapon1 = WeaponTypes.AI_BerettaM9;
        weapon2 = WeaponTypes.AI_ColtPhthon357;
	}
	
    //void Weapon1()
    //{
    //    WeaponShooter.m_WeaponType = weapon1;
    //}

    //void Weapon2()
    //{
    //    WeaponShooter.m_WeaponType = weapon2;
    //}
}
