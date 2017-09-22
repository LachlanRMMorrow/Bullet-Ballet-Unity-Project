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

    public Button weapon1Button;
    public Button weapon2Button;
    public Button weapon3Button;
    //public Button continueButton;

    void Awake()
    {
        weaponMenu = GameObject.Find("Canvas").transform.Find("Weapon Select Menu").gameObject;
        weaponMenu.SetActive(true);
        WeaponMenuActive();
    }

	public void WeaponMenuActive()
    {
        weapon1 = WeaponTypes.BerettaM9;
        weapon2 = WeaponTypes.ColtPhthon357;
        weapon3 = WeaponTypes.M1911;
        currentEquippedWeaponRight = GameObject.Find("Player (Rigged)").transform.GetChild(0).GetChild(0).GetChild(1).GetComponent<WeaponShooter>();

        currentEquippedWeaponLeft = GameObject.Find("Player (Rigged)").transform.GetChild(0).GetChild(1).GetChild(1).GetComponent<WeaponShooter>();



        if (weaponMenu == null)
        {
            weaponMenu = GameObject.Find("Canvas").transform.Find("Weapon Select Menu").gameObject;
        }

        if (weapon1Button == null)
        {
            weapon1Button = weaponMenu.transform.Find("Weapon 1 Button").GetComponent<Button>();
            weapon1Button.onClick.AddListener(StartGame);
        }

        if (weapon2Button == null)
        {
            weapon2Button = weaponMenu.transform.Find("Weapon 2 Button").GetComponent<Button>();
            weapon2Button.onClick.AddListener(StartGame);
        }

        if (weapon3Button == null)
        {
            weapon3Button = weaponMenu.transform.Find("Weapon 3 Button").GetComponent<Button>();
            weapon3Button.onClick.AddListener(StartGame);
        }

        //if (continueButton == null)
        //{
        //    continueButton = weaponMenu.transform.Find("Continue Weapon Menu").GetComponent<Button>();
        //    continueButton.onClick.AddListener(startGame);
        //}


        weapon2Button.Select();
        weapon1Button.Select();
    }

    public void EquipWeapon1()
    {
        currentEquippedWeaponRight.m_WeaponType = weapon1;
        currentEquippedWeaponLeft.m_WeaponType = weapon1;
        currentEquippedWeaponRight.reload();
        currentEquippedWeaponLeft.reload();
        
    }

    public void EquipWeapon2()
    {
        currentEquippedWeaponRight.m_WeaponType = weapon2;
        currentEquippedWeaponLeft.m_WeaponType = weapon2;
        currentEquippedWeaponRight.reload();
        currentEquippedWeaponLeft.reload();
    }

    public void EquipWeapon3()
    {
        currentEquippedWeaponRight.m_WeaponType = weapon3;
        currentEquippedWeaponLeft.m_WeaponType = weapon3;
        currentEquippedWeaponRight.reload();
        currentEquippedWeaponLeft.reload();
    }

    public void StartGame()
    {
        weaponMenu.SetActive(false);
        SlowMoManager.m_isPaused = false;
        GameObject manager = GameObject.Find("MANAGER");
        manager.GetComponent<SlowMoManager>().updateTimeScale(true);
        Debug.Log(Time.timeScale);
    }
}
