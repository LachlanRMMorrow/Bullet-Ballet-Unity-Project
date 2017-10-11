using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(BoxCollider))]
[RequireComponent(typeof(Health))]
public class AI : MonoBehaviour {

    /// <summary>
    /// which weapon should this AI be using
    /// </summary>
    public WeaponTypes m_WeaponType;

    //visible 
    public GameObject m_VisibleObject;
    public GameObject m_LastKnownPositionObject;

    /// <summary>
    /// reference to the player's transform
    /// used for getting the direction of the player
    /// </summary>
    protected Transform m_PlayerTransform;

    /// <summary>
    /// where the Ai should shoot from
    /// so it doesnt shoot itself
    /// </summary>
    public Transform m_ShootPoint;

    /// <summary>
    /// current ammo left
    /// </summary>
    private int m_Ammo;
    /// <summary>
    /// last time the ai left
    /// </summary>
    private float m_LastShootTime;

    /// <summary>
    /// is the ai currently reloading
    /// </summary>
    private bool m_IsReloading;
    /// <summary>
    /// when the ai started reloading
    /// </summary>
    private float m_LastReloadTime;

    /// <summary>
    /// how quick should this ai turn around
    /// </summary>
    public float m_TurnSpeed = 10.0f;

    /// <summary>
    /// how far the player has to be infront of this ai for it to start shooting
    /// </summary>
    [Range(0.0f, 1.0f)]
    public float m_PlayerInfrontForShootRadius = 0.75f;

    /// <summary>
    /// starting rotation of this AI + a random 90 degree offset thats also applied to the transform
    /// </summary>
    private Quaternion m_StartRotation;

    /// <summary>
    /// for Camera Shake
    /// </summary>
    private CameraManager m_CameraManager;

    /// <summary>
    /// last time we have seen the player
    /// </summary>
    protected Vector3 m_LastPlayerPos = Vector3.zero;
    /// <summary>
    /// a flag to check if the player has been seen
    /// </summary>
    protected bool m_SeenPlayer;

    /// <summary>
    /// navmesh for the AI
    /// </summary>
    protected NavMeshAgent m_NavMesh;

    private static int m_RoomLayer = -1;
    public int m_CurrentRoomIndex = -1;
    private RoomHolder m_CurrentRoomHolder;
    /// <summary>
    /// has this AI been in the same room as the player?
    /// </summary>
    private bool m_HasBeenInTheSameRoom = false;

    // Use this for initialization
    protected virtual void Start() {
        //set the static room layer variable
        if (m_RoomLayer == -1) {
            m_RoomLayer = LayerMask.NameToLayer("RoomFog");
        }

        //add a random 90 degree rotation to the starting 
        //and store the starting position
        float rotateAmount = 90 * UnityEngine.Random.Range(0, 4);//can be 0,90,180,270
        m_StartRotation = transform.rotation;//get rotation
        m_StartRotation *= Quaternion.Euler(new Vector3(0, rotateAmount, 0));//add rotation
        transform.rotation = m_StartRotation;//apply new rotation

        //find the player
        //todo, better way for doing this
        m_PlayerTransform = FindObjectOfType<PlayerMovement>().transform;

        //set up ammo
        m_Ammo = getWeapon().m_MaxAmmo;

        //get navmesh agent
        m_NavMesh = transform.GetComponentInChildren<NavMeshAgent>();
        //old code, that causes problems
        //if(m_NavMesh == null) {
        //	m_NavMesh = m_VisibleObject.AddComponent<NavMeshAgent>();
        //	Debug.LogWarning(transform.name + " AI's did not have a nav mesh on it's visible object");
        //}

        m_CameraManager = FindObjectOfType<CameraManager>();

        Health health = GetComponent<Health>();
        health.m_ObjectDiedEvent.AddListener(AiKilled);
        health.m_ObjectHitEvent.AddListener(AiHit);

        //get TellParentAboutCollision from collider within the model
        TellParentAboutCollision tpac = GetComponentInChildren<TellParentAboutCollision>();
        tpac.m_TriggerEnter.AddListener(OnTriggerEnter);
    }



    // Update is called once per frame
    protected virtual void Update() {
        //player can be destroyed, this prevents errors
        if (m_PlayerTransform == null) {
            return;
        }

        updateLastKnownPosition();

        //if we can see the player, then return
        if (checkForPlayer()) {
            return;
        }

        //if the player has been seen before, then we just want to go to where the player was
        if (m_SeenPlayer) {
            //if we have no path and the distance between the players last pos and us is greater then 1
            if (!m_NavMesh.hasPath && Vector3.Distance(m_LastPlayerPos, m_PlayerTransform.position) > 1) {
                //set path
                m_NavMesh.SetDestination(m_LastPlayerPos);
                return;
            }

            //else just go back to looking in your starting direction
            turnBackToStart();

            return;
        }
    }

    /// <summary>
    /// checks if this ai can see the player,
    /// if it can it will shoot and look the player
    /// and update it's last known player position
    /// </summary>
    protected bool checkForPlayer() {
        if (canSeePlayer()) {
            if (m_NavMesh.hasPath) {
                m_NavMesh.ResetPath();
            }
            //look and shoot at the player
            turnToPlayer();
            shoot();

            m_SeenPlayer = true;
            m_LastPlayerPos = m_PlayerTransform.position;
            return true;
        }
        return false;
    }
  
    /// <summary>
    /// can the AI see the player
    /// </summary>
    /// <returns></returns>
    protected bool canSeePlayer() {
        RaycastHit hit;
        //eg. get the layer of the Bullets, which is 9
        //convert to a bitmask (000100000000)
        int layerMask = (1 << LayerMask.NameToLayer("Default") | 1 << LayerMask.NameToLayer("Walls") | 1 << LayerMask.NameToLayer("Player") | 1 << LayerMask.NameToLayer("Enemies") | 1 << LayerMask.NameToLayer("Cover"));
        //raycast from AI in the player direction for 1000 units, checking for walls, players and enemies
        if (Physics.Raycast(m_VisibleObject.transform.position + (Vector3.up*1.2f), normPlayerDir(), out hit, 1000, layerMask)) {
            //is this object the player
            if (hit.transform.gameObject.layer == LayerMask.NameToLayer("Player")) {
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// move AI back to starting rotation
    /// </summary>
    protected void turnBackToStart() {
        //basic turn right using m_TurnSpeed and deltatime
        m_VisibleObject.transform.rotation = Quaternion.RotateTowards(m_VisibleObject.transform.rotation, m_StartRotation, m_TurnSpeed * Time.deltaTime);
    }

    /// <summary>
    /// turn AI to face player
    /// </summary>
    protected void turnToPlayer() {
        //gets how far to the left or right the player is from us
        //positive is right
        float turn = Vector3.Dot(m_VisibleObject.transform.right, normPlayerDir());

        //if positive, make it 1, if negative make it -1
        //simular to a clamp with -1,1, although it will only be -1 and 1, not 0.2 or -0.664
        turn = turn > 0 ? 1 : -1;

        //make sure it moves with deltatime and with the added turnSpeed
        turn *= m_TurnSpeed * Time.deltaTime;

        m_VisibleObject.transform.rotation = m_VisibleObject.transform.rotation * Quaternion.Euler(new Vector3(0, turn, 0));
    }

    /// <summary>
    /// runs a shoot test for the AI
    /// check if the AI is reloading and if it's gun is not on cooldown
    /// will shoot a bullet and cause camera shake if the gun passes all the criteria 
    /// </summary>
    protected void shoot() {
        if (m_IsReloading) {
            if (Time.time - m_LastReloadTime > getWeapon().m_TimeToReload) {
                m_Ammo = getWeapon().m_MaxAmmo;
                m_IsReloading = false;
            }
            return;
        }
        //gets how close the player is to being directly infront of the ai
        //1 is infront, -1 is behind, 0 is to the side
        float isInfrount = Vector3.Dot(m_VisibleObject.transform.forward, normPlayerDir());

        //check if the player is infront by m_PlayerInfrontForShootRadius
        if (isInfrount > m_PlayerInfrontForShootRadius) {
            if (Time.time - m_LastShootTime > getWeapon().m_WeaponShootCooldown) {
                if (m_Ammo <= 0) {
                    startReload();
                    return;
                }
                getWeapon().fireProjectile(m_ShootPoint);
                m_LastShootTime = Time.time;

                m_Ammo--;

                //run screenshake
                m_CameraManager.startScreenShake();
            }
        }
    }

    /// <summary>
    /// gets a normalized vector with the direction of the player
    /// </summary>
    /// <returns>Normailzed direction vector</returns>
    protected Vector3 normPlayerDir() {
        //predict where player will be??
        //useing the navmesh angent
        return Vector3.Normalize(m_PlayerTransform.position - m_VisibleObject.transform.position);
    }

    private void startReload() {
        m_IsReloading = true;
        m_LastReloadTime = Time.time;
    }

    public WeaponReference getWeapon() {
        return WeaponHolder.getWeapon(m_WeaponType);
    }

    protected void updateLastKnownPosition() {
        //IT WOULD BE BETTER TO NOT RUN THIS EVERY FRAME, BUT GOOD AS A EXAMPLE/TEST
        bool isRoomHolderEntered = false;
        if(m_CurrentRoomHolder != null) {
            isRoomHolderEntered = m_CurrentRoomHolder.m_Entered;
        }
        if (m_CurrentRoomIndex == RoomHolder.m_PlayersCurrentRoom || m_CurrentRoomIndex == -1 || isRoomHolderEntered) {
        //if (m_CurrentRoomHolder.m_Entered || m_CurrentRoomIndex == -1) {
        m_HasBeenInTheSameRoom = true;
            m_VisibleObject.SetActive(true);
            m_LastKnownPositionObject.SetActive(false);
            //update position and rotation
            m_LastKnownPositionObject.transform.position = m_VisibleObject.transform.position;
            m_LastKnownPositionObject.transform.rotation = m_VisibleObject.transform.rotation;
        } else {
            m_VisibleObject.SetActive(false);
            if (m_HasBeenInTheSameRoom) {
                m_LastKnownPositionObject.SetActive(true);
            } else {
                m_LastKnownPositionObject.SetActive(false);
            }
        }

    }

    public void OnTriggerEnter(Collider other) {
        //update room layer to new room
        if (m_RoomLayer == other.gameObject.layer) {
            m_CurrentRoomHolder = other.GetComponent<RoomScript>().m_ThisRoomsManager;
            m_CurrentRoomIndex = m_CurrentRoomHolder.m_RoomID;
        }
    }

    /// <summary>
    /// called using unity event system when this object is out of health
    /// </summary>
    private void AiKilled() {
        Destroy(gameObject);
    }

    /// <summary>
    /// called using unity event system when this object is hit
    /// </summary>
    private void AiHit() {
        LevelEmissionFlash.m_Singleton.startFlash(false);
    }

}
