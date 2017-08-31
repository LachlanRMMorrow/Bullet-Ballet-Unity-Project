using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class WeaponHolder : MonoBehaviour {
    [System.Serializable]
    public class WeaponData {
        /// <summary>
        /// name for it to appear
        /// </summary>
        [HideInInspector]
        public string m_Name;
        //This is the base weapon, it holds the capacity of the weapon, how it shoots and how it reloads
        [UnityEngine.Serialization.FormerlySerializedAs("m_Weapon")]
        public WeaponReference m_WeaponRef;
    }

    public WeaponData[] m_Weapons = new WeaponData[0];

    public static WeaponHolder m_Singleton;

    // Use this for initialization
    void Awake() {
        m_Singleton = this;
    }
    void OnEnable() {
        m_Singleton = this;
    }

    /// <summary>
    /// This makes sure the array size is always the size of the WeaponTypes enum
    /// </summary>
    void OnValidate() {
        int numOfWeapons = (int)WeaponTypes.WEAPON_TYPES_SIZE;
        //update size of array if it's different
        if (m_Weapons.Length != numOfWeapons) {
            WeaponData[] copy = new WeaponData[numOfWeapons];
            for (int i = 0; i < numOfWeapons; i++) {
                if (i >= m_Weapons.Length) {
                    copy[i] = new WeaponData();
                } else if (m_Weapons[i] == null) {
                    copy[i] = new WeaponData();
                } else {
                    copy[i] = m_Weapons[i];
                }
            }
            m_Weapons = copy;
        }

        //update array data, and set up default values
        for (int i = 0; i < numOfWeapons; i++) {

            m_Weapons[i].m_Name = ((WeaponTypes)i).ToString();//set name 
            if(m_Weapons[i].m_WeaponRef == null) {
                m_Weapons[i].m_WeaponRef = new WeaponReference();
            }
            m_Weapons[i].m_WeaponRef.m_WeaponType = ((WeaponTypes)i);

            if(m_Weapons[i].m_WeaponRef.m_BulletPrefab == null) {
                Debug.LogError("Weapon " + m_Weapons[i].m_Name + " is missing it's bullet prefab");
            }
        }
        
    }

    public static WeaponReference getWeapon(WeaponTypes a_Type) {
        return m_Singleton.m_Weapons[(int)a_Type].m_WeaponRef;
    }

}
