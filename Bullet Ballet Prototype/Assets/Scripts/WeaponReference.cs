using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// reference for the weapon constants, that are the same for each weapon of the same type
/// </summary>
[System.Serializable]
public class WeaponReference {

    [HideInInspector]
    public WeaponTypes m_WeaponType;

    public int m_MaxAmmo = 5;

    public float m_WeaponShootCooldown = 0.5f;

    public float m_TimeToReload = 1.5f;

    public GameObject m_BulletPrefab;

    public Sprite m_WeaponSprite;
    public Sprite m_BulletUIImage;
    public Sprite m_BulletShotUIImage;

    public AudioClip m_ShotSound;
    public AudioClip m_BulletCasingSound;
    public AudioClip m_ReloadSound;

    //shoots a bullet from m_SpawnPoint
    //gives it a constantForce forward
    //then deletes after 5 seconds
    public GameObject fireProjectile(Transform m_SpawnPoint) {
        //spawns a object using it's prefab at m_SpwanPoint
        GameObject newBullet = Object.Instantiate(m_BulletPrefab, m_SpawnPoint.position, m_SpawnPoint.rotation);



        //destroy after 5 seconds
        Object.Destroy(newBullet, 5.0f);
        return newBullet;
    }


   


}
