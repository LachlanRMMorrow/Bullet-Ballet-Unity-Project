﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// reference for the weapon constants, that are the same for each weapon of the same type
/// </summary>
[System.Serializable]
public class WeaponReference {

    [System.Serializable]
    public class WeaponUIHolderOffsets {
        public Vector2 m_PositionOffset;
        //values defaulted to how they were in the inspector 1/11/17
        public float m_XOffset = 50.0f;

        public int m_MaxBulletsPerRow = 10;

        public Vector2 m_BulletSize = new Vector2(14, 30);
        public Vector2 m_BulletOffset = new Vector2(13.2f, 30);
    }

    [HideInInspector]
    public WeaponTypes m_WeaponType;

    public int m_MaxAmmo = 5;

    public float m_WeaponShootCooldown = 0.5f;
    public float m_BulletCasingSoundDelay = 0.5f;

    public float m_TimeToReload = 1.5f;

    public GameObject m_BulletPrefab;
    public GameObject m_WeaponPrefab;

    public Sprite m_WeaponSprite;
    public Sprite m_BulletUIImage;
    public Sprite m_BulletShotUIImage;

    public AudioClip m_ShotSound;
    public AudioClip m_BulletCasingSound;
    public AudioClip m_ReloadSound;

    public WeaponUIHolderOffsets m_UIBulletOffsets;

    //shoots a bullet from m_SpawnPoint
    //gives it a constantForce forward
    //then deletes after 5 seconds
    public GameObject fireProjectile(Transform m_SpawnPoint) {

        //get rot
        Vector3 pos = m_SpawnPoint.rotation.eulerAngles;
        //only keep the y rotation
        pos.x = pos.z = 0;
        //put back into quaternion form
        Quaternion rot = Quaternion.Euler(pos);


        //spawns a object using it's prefab at m_SpwanPoint
        GameObject newBullet = Object.Instantiate(m_BulletPrefab, m_SpawnPoint.position, rot);


        SoundManager.PlaySFXRandomized(m_ShotSound);
        PlayShellCasingSound();


        //destroy after 5 seconds
        Object.Destroy(newBullet, 5.0f);
        return newBullet;
    }

    public void PlayReloadSound()
    {
        SoundManager.PlaySFX(m_ReloadSound);
    }

    public void PlayShellCasingSound()
    {
        SoundManager.PlaySFXRandomizedDelayed(m_BulletCasingSound, m_BulletCasingSoundDelay);
    }




   


}
