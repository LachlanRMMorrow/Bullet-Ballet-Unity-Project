using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour {

    public float m_BulletSpeed = 300.0f;

    public GameObject m_BulletHitEnemyPrefab;
    public float m_BulletHitEnemyTimeToDelete = 1.0f;

    public float m_StickAroundAfterDeathTime = 5.0f;

    public float m_BulletDamage = 1.0f;

    /// <summary>
    /// flag to check if this object has been run through a collision check
    /// if true, then it wont run any other collision or trigger enter events
    /// </summary>
    private bool m_HasHit = false;

    [HideInInspector]
    public bool m_ShouldStopAfterCollision = true;

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
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player") && gameObject.CompareTag("Player Bullet"))
        {

        }
        else
        {
            if (m_HasHit)
            {
                return;
            }
            checkBulletHitHandler(collision.gameObject);
            if (m_ShouldStopAfterCollision) {
                bulletHit(collision.gameObject);
                m_HasHit = true;
            }
        }
            
        
    }

    /// <summary>
    ///collision with player
    /// </summary>
    /// <param name="collision"></param>
    void OnTriggerEnter(Collider collision) {
        if (collision.CompareTag("PlayerCollider") && gameObject.CompareTag("Player Bullet"))
        {
            Debug.Log("Player hit Player");
        }
        else
        {
            if (m_HasHit)
            {
                return;
            }

            PlayerDive pd = collision.GetComponentInParent<PlayerDive>();
            if (pd != null) {
                if (pd.isDiving) {
                    m_HasHit = true;
                    return;
                }
            }
            Ragdoll rd = collision.GetComponentInParent<Ragdoll>();
            if(rd != null) {
                if (rd.enabled) {
                    m_HasHit = true;
                    return;
                }
            }
            checkBulletHitHandler(collision.gameObject);
            if (m_ShouldStopAfterCollision) {
                bulletHit(collision.gameObject);
                m_HasHit = true;
            }

        }
        
    }


    private void checkBulletHitHandler(GameObject a_Object) {
        m_ShouldStopAfterCollision = true;
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
        if (a_Object.CompareTag("PlayerCollider") && gameObject.CompareTag("Player Bullet"))
        {
        }
        else
        {
            Health health = a_Object.GetComponent<Health>();
            if (health == null)
            {
                health = a_Object.GetComponentInParent<Health>();
            }



            if (health != null)
            {//if there is a health script attacked
                health.dealDamage(m_BulletDamage);
            }
            else
            {
                //todo update when the enemys are nolonger probuilder objects
                a_Object.SetActive(false);//not Destroy because with probuilder it destroys the map aswell
            }
        }
        
    }


}
