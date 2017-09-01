﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour {

    public float m_BulletSpeed = 300.0f;

    public GameObject m_BulletHitEnemyPrefab;
    public float m_BulletHitEnemyTimeToDelete = 1.0f;

    public float m_StickAroundAfterDeathTime = 5.0f;

    public float m_BulletDamage = 1.0f;

    // Use this for initialization
    protected virtual void Awake() {
        //set force of bullet, in the forward direction
        ConstantForce force = gameObject.AddComponent<ConstantForce>();
        force.force = transform.rotation * Vector3.forward * m_BulletSpeed;
    }
    /// <summary>
    /// collision with enemy
    /// </summary>
    /// <param name="collision"></param>
    void OnCollisionEnter(Collision collision) {
        checkBulletHitHandler(collision.gameObject);
        bulletHit(collision.gameObject);
    }

    /// <summary>
    ///collision with player
    /// </summary>
    /// <param name="collision"></param>
    void OnTriggerEnter(Collider collision) {
        checkBulletHitHandler(collision.gameObject);
        bulletHit(collision.gameObject);
    }


    private void checkBulletHitHandler(GameObject a_Object) {
        BulletHitHandler bhh = a_Object.GetComponent<BulletHitHandler>();
        if(bhh != null) {
            bhh.m_BulletHit = transform;
            bhh.m_ObjectHit.Invoke();
        }
    }

    virtual protected void bulletHit(GameObject a_Object) {
        bulletCollision();

        //ok well it seems to have a issue where the enemy bullets are using this function
        if (a_Object.layer == LayerMask.NameToLayer("Enemies")|| a_Object.layer == LayerMask.NameToLayer("Player")) {
            //todo: leave body?

            spawnBlood(a_Object.transform.position, transform.rotation);
            dealDamage(a_Object);
        }
    }

    protected void bulletCollision() {
        Destroy(gameObject, m_StickAroundAfterDeathTime);//destroy self


        GetComponent<Rigidbody>().isKinematic = true;//remove movement
        GetComponent<MeshRenderer>().enabled = false;//remove renderer
        GetComponent<Collider>().enabled = false;//remove collider
        GetComponent<ConstantForce>().enabled = false;//stop the force
    }

    protected void spawnBlood(Vector3 a_Position, Quaternion a_Rotation) {
        if (m_BulletHitEnemyPrefab != null) {
            GameObject blood = GameObject.Instantiate(m_BulletHitEnemyPrefab, a_Position, a_Rotation);
            Destroy(blood, m_BulletHitEnemyTimeToDelete);
        }
    }

    protected void dealDamage(GameObject a_Object) {
        //get health script
        Health health = a_Object.GetComponent<Health>();
        if (health == null) {
            health = a_Object.GetComponentInParent<Health>();
        }



        if (health != null) {//if there is a health script attacked
            health.dealDamage(m_BulletDamage);
        } else {
            //todo update when the enemys are nolonger probuilder objects
            a_Object.SetActive(false);//not Destroy because with probuilder it destroys the map aswell
        }
    }


}