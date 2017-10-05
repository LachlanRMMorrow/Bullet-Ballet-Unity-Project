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

    WeaponReference wepRef;

    public GameObject rightHandRef;
    public GameObject leftHandRef;

    Vector3 rightHandWeaponPosition;
    Quaternion rightHandWeaponRotation;

    Vector3 leftHandWeaponPosition;
    Quaternion leftHandWeaponRotation;

    Transform test;
    Transform test2;

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

        rightHandRef = GameObject.Find("Player (Rigged)").transform.GetChild(2).GetChild(0).GetChild(0).GetChild(2).GetChild(0).GetChild(0).GetChild(2).GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(2).GetChild(0).gameObject;
        leftHandRef = GameObject.Find("Player (Rigged)").transform.GetChild(2).GetChild(0).GetChild(0).GetChild(2).GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(2).GetChild(0).gameObject;

        rightHandWeaponPosition = rightHandRef.transform.GetChild(0).transform.position;
        rightHandWeaponRotation = rightHandRef.transform.GetChild(0).transform.rotation;

        leftHandWeaponPosition = leftHandRef.transform.GetChild(0).transform.position;
        leftHandWeaponRotation = leftHandRef.transform.GetChild(0).transform.rotation;

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

        weapon2Button.Select();
        weapon1Button.Select();
    }

    public void EquipWeapon1()
    {
        currentEquippedWeaponRight.m_WeaponType = weapon1;
        currentEquippedWeaponLeft.m_WeaponType = weapon1;

        WeaponSwitch(0);


    }

    public void EquipWeapon2()
    {
        currentEquippedWeaponRight.m_WeaponType = weapon2;
        currentEquippedWeaponLeft.m_WeaponType = weapon2;

        WeaponSwitch(180);
    }

    public void EquipWeapon3()
    {
        currentEquippedWeaponRight.m_WeaponType = weapon3;
        currentEquippedWeaponLeft.m_WeaponType = weapon3;

        WeaponSwitch(0);
    }

    public void StartGame()
    {
        weaponMenu.SetActive(false);
        SlowMoManager.m_isPaused = false;
        GameObject manager = GameObject.Find("MANAGER");
        manager.GetComponent<SlowMoManager>().updateTimeScale(true);
    }

    private void WeaponSwitch(float rotationOffset)
    {

        wepRef = WeaponHolder.getWeapon(currentEquippedWeaponRight.m_WeaponType);

        foreach (Transform child in leftHandRef.transform)
        {
            GameObject.Destroy(child.gameObject);
        }

        foreach (Transform child in rightHandRef.transform)
        {
            GameObject.Destroy(child.gameObject);
        }

        Instantiate(wepRef.m_WeaponPrefab, rightHandRef.transform.position, (rightHandRef.transform.rotation * Quaternion.Euler(0, rotationOffset, 0)), rightHandRef.transform);
        Instantiate(wepRef.m_WeaponPrefab, leftHandRef.transform.position, (leftHandRef.transform.rotation * Quaternion.Euler(0, rotationOffset, 0)), leftHandRef.transform);


        currentEquippedWeaponRight.reload();
        currentEquippedWeaponLeft.reload();
    }
}
