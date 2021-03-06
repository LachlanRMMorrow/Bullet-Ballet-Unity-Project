﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WeaponUI : MonoBehaviour {

    public WeaponShooter m_Holder;

    public Image m_WeaponImageObject;

    private WeaponTypes m_CurrentRef;
    private int m_LastBulletCount = 0;

    private List<GameObject> m_UIBullets = new List<GameObject>();

    private bool m_IsOnRightSide = false;

    public bool m_RegenerateBullets = false;

    private Vector2 m_LocalStartingPosition;

    // Use this for initialization
    void Start() {
        //remove all transforms which are currently under this weapon ui
        for (int i = transform.childCount - 1; i > 0; i--) {
            Destroy(transform.GetChild(i).gameObject);
        }

        //dirty way to find out if we are the right Bullet Holder
        //might be better having this be a public boolean and setting it in the inspector
        if (transform.name.ToLower().Contains("right")) {
            m_IsOnRightSide = true;
        }

        m_LocalStartingPosition = transform.localPosition;

        runWeaponUpdate();
    }

    // Update is called once per frame
    void Update() {
        if (m_RegenerateBullets) {
            m_RegenerateBullets = false;
            createBulletUI();
        }

        //if the player has changed weapon
        if (m_CurrentRef != m_Holder.m_WeaponType) {
            runWeaponUpdate();
        }
        if (m_LastBulletCount != m_Holder.currentAmmo) {
            m_LastBulletCount = m_Holder.currentAmmo;
            updateBulletUI();
        }
    }

    private void updateBulletUI() {
        //get current weapon
        WeaponReference wepRef = WeaponHolder.getWeapon(m_CurrentRef);
        //loop through each bullet
        for (int i = 0; i < wepRef.m_MaxAmmo; i++) {
            GameObject go = m_UIBullets[i];
            //if i is larger then the current ammo
            //then the bullet is shot
            if (i >= m_Holder.currentAmmo) {
                go.transform.GetChild(0).gameObject.SetActive(false);
                go.transform.GetChild(1).gameObject.SetActive(true);
            } else {//else  show the bullet
                go.transform.GetChild(0).gameObject.SetActive(true);
                go.transform.GetChild(1).gameObject.SetActive(false);
            }
        }
    }

    private void runWeaponUpdate() {
        m_CurrentRef = m_Holder.m_WeaponType;
        createBulletUI();
        updateWeaponUI();
    }

    /// <summary>
    /// updates the weapon ui, should only need to be updated when a weapon is changed
    /// </summary>
    private void updateWeaponUI() {
        if (m_WeaponImageObject == null) {
            return;
        }
        //get current weapon
        WeaponReference wepRef = WeaponHolder.getWeapon(m_CurrentRef);

        //apply sprite, sprite can be null
        m_WeaponImageObject.sprite = wepRef.m_WeaponSprite;
    }

    /// <summary>
    /// updated the bullets displayed on the screen to the new amount of max bullets
    /// </summary>
    private void createBulletUI() {
        //delete current bullet images
        for (int i = 0; i < m_UIBullets.Count; i++) {
            Destroy(m_UIBullets[i]);
        }
        m_UIBullets.Clear();

        //get current weapon
        WeaponReference wepRef = WeaponHolder.getWeapon(m_CurrentRef);

        transform.localPosition = m_LocalStartingPosition + Vector2.Scale(new Vector2(m_IsOnRightSide ? -1 : 1, 1), wepRef.m_UIBulletOffsets.m_PositionOffset);

        //create bullet objects
        for (int i = 0; i < wepRef.m_MaxAmmo; i++) {
            //create object of type RectTransform
            GameObject go = new GameObject("Bullet Holder", typeof(RectTransform));
            go.transform.SetParent(transform);

            //unshot bullet image
            GameObject unshotBullet = new GameObject("Unshot bullet", typeof(RectTransform));
            unshotBullet.transform.SetParent(go.transform);

            //shot bullet image
            GameObject shotBullet = new GameObject("Shot bullet", typeof(RectTransform));
            shotBullet.transform.SetParent(go.transform);

            (unshotBullet.transform as RectTransform).sizeDelta = wepRef.m_UIBulletOffsets.m_BulletSize;
            (shotBullet.transform as RectTransform).sizeDelta = wepRef.m_UIBulletOffsets.m_BulletSize;

            //add images to game objects
            if (wepRef.m_BulletUIImage != null) {
                Image img = unshotBullet.AddComponent<Image>();
                img.sprite = wepRef.m_BulletUIImage;
            }
            if (wepRef.m_BulletShotUIImage != null) {
                Image img = shotBullet.AddComponent<Image>();
                img.sprite = wepRef.m_BulletShotUIImage;
            }

            //hide the shot bullet texture
            shotBullet.SetActive(false);

            //todo: move the bullets to the side
            RectTransform goTransform = go.transform as RectTransform;
            goTransform.position = (transform as RectTransform).position;
            goTransform.localScale = new Vector3(1, 1, 1);
            Vector2 pos = goTransform.anchoredPosition;
            pos.x += (i % wepRef.m_UIBulletOffsets.m_MaxBulletsPerRow) * (wepRef.m_UIBulletOffsets.m_BulletOffset.x * (m_IsOnRightSide ? -1 : 1));
            pos.y += Mathf.CeilToInt(i / wepRef.m_UIBulletOffsets.m_MaxBulletsPerRow) * wepRef.m_UIBulletOffsets.m_BulletOffset.y;
            pos.x += Mathf.CeilToInt(i / wepRef.m_UIBulletOffsets.m_MaxBulletsPerRow) * (wepRef.m_UIBulletOffsets.m_XOffset * (m_IsOnRightSide ? -1 : 1));
            goTransform.anchoredPosition = pos;

            //add reference to array list
            m_UIBullets.Add(go);
        }


    }
}
