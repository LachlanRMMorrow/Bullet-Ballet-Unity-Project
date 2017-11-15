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

    Transform test;
    Transform test2;

    public Button weapon1Button;
    public Button weapon2Button;
    public Button weapon3Button;
    //public Button continueButton;

    public GameObject m_ShakeHolder;
    public UnityEngine.PostProcessing.PostProcessingProfile screenBlur;

    private Transform m_FogCamera;

    void Awake()
    {
        //gets the fog camera, it is the main camera, probably due to it's depth
        m_FogCamera = Camera.main.transform;

        m_ShakeHolder = GameObject.Find("Canvas").transform.GetChild(0).gameObject;
        m_ShakeHolder.SetActive(false);

        weaponMenu = GameObject.Find("Canvas").transform.Find("Weapon Select Menu").gameObject;
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

        screenBlur.depthOfField.enabled = true;
//        m_FogCamera.gameObject.SetActive(false);

        weapon2Button.Select();
        weapon1Button.Select();
    }

    public void EquipWeapon1()
    {
        currentEquippedWeaponRight.changeWeapon(weapon1);
        currentEquippedWeaponLeft.changeWeapon(weapon1);

        WeaponSwitch(0);


    }

    public void EquipWeapon2()
    {
        currentEquippedWeaponRight.changeWeapon(weapon2);
        currentEquippedWeaponLeft.changeWeapon(weapon2);

        WeaponSwitch(180);
    }

    public void EquipWeapon3()
    {
        currentEquippedWeaponRight.changeWeapon(weapon3);
        currentEquippedWeaponLeft.changeWeapon(weapon3);

        WeaponSwitch(0);
    }

    public void StartGame()
    {
        weaponMenu.SetActive(false);
        SlowMoManager.m_isPaused = false;
        m_ShakeHolder.SetActive(true);
        GameObject manager = GameObject.Find("MANAGER");
        manager.GetComponent<SlowMoManager>().updateTimeScale(true);
        screenBlur.depthOfField.enabled = false;
        m_FogCamera.gameObject.SetActive(true);

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
    }
}
