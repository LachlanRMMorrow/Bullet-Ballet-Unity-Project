using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class Weapon2Button : MonoBehaviour, ISelectHandler
{

    GameObject eS;

	public void OnSelect(BaseEventData eventData)
    {
        eS = GameObject.Find("EventSystem");
        eS.GetComponent<WeaponSelectMenu>().EquipWeapon2();

    }
}
