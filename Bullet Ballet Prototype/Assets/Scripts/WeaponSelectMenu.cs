using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WeaponSelectMenu : MonoBehaviour {

    GameObject weaponMenu;

    WeaponTypes weapon1;
    WeaponTypes weapon2;
    WeaponTypes weapon3;
    WeaponShooter currentEquippedWeaponRight;
    WeaponShooter currentEquippedWeaponLeft;

    Button weapon1Button;
    Button weapon2Button;
    Button weapon3Button;
    Button backButton;

	public void WeaponMenuActive()
    {
        weapon1 = WeaponTypes.BerettaM9;
        weapon2 = WeaponTypes.ColtPhthon357;
        weapon3 = WeaponTypes.M1911;
        currentEquippedWeaponRight = GameObject.Find("Player").transform.GetChild(0).GetChild(0).GetChild(1).GetComponent<WeaponShooter>();
        //transform.Find("Right Arm Shooting Arm").GetComponent<WeaponShooter>().m_WeaponType;
        currentEquippedWeaponLeft = GameObject.Find("Player").transform.GetChild(0).GetChild(1).GetChild(1).GetComponent<WeaponShooter>();
            //Find("Left Arm Shooting Arm").GetComponent<WeaponShooter>().m_WeaponType;


        if (weaponMenu == null)
        {
            weaponMenu = GameObject.Find("Canvas").transform.Find("Weapon Select Menu").gameObject;
        }

        if (weapon1Button == null)
        {
            weapon1Button = weaponMenu.transform.Find("Weapon 1 Button").GetComponent<Button>();
            weapon1Button.onClick.AddListener(EquipWeapon1);
        }

        if (weapon2Button == null)
        {
            weapon2Button = weaponMenu.transform.Find("Weapon 2 Button").GetComponent<Button>();
            weapon2Button.onClick.AddListener(EquipWeapon2);
        }

        if (weapon3Button == null)
        {
            weapon3Button = weaponMenu.transform.Find("Weapon 3 Button").GetComponent<Button>();
            weapon3Button.onClick.AddListener(EquipWeapon3);
        }

        

        weapon2Button.Select();
        weapon1Button.Select();
    }

    public void EquipWeapon1()
    {
        currentEquippedWeaponRight.m_WeaponType = weapon1;
        currentEquippedWeaponLeft.m_WeaponType = weapon1;
    }

    public void EquipWeapon2()
    {
        currentEquippedWeaponRight.m_WeaponType = weapon2;
        currentEquippedWeaponLeft.m_WeaponType = weapon2;
    }

    public void EquipWeapon3()
    {
        currentEquippedWeaponRight.m_WeaponType = weapon3;
        currentEquippedWeaponLeft.m_WeaponType = weapon3;
    }
}
