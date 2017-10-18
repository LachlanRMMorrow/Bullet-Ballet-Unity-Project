using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// deals with getting the weapon and shooting/reloading using the weapons reference times
/// </summary>
public class WeaponShooter : MonoBehaviour {

    public WeaponTypes m_WeaponType;

    private int m_CurrentAmmo;   

    public int currentAmmo { get { return m_CurrentAmmo; } }

    public Transform m_MuzzleFlashSpawnPoint;

    /// <summary>
    /// last time of player shooting or time the reload started
    /// </summary>
    private float m_LastTime = -100;

    public UnityEngine.UI.Text m_DebugText;

    public GameObject m_MuzzleFlashPrefab;

    /// <summary>
    /// Time this component was disabled
    /// </summary>
    private float m_DisableTime = 0;

    /// <summary>
    /// is this weapon reloading?
    /// </summary>
    private bool m_IsReloading = false;

    /// <summary>
    /// for Camera Shake
    /// </summary>
    private CameraManager m_CameraManager;

    /// <summary>
    /// 0 for only exactly horizontal
    /// 1 to allow everything
    /// 0.5 to only allow shooting when the gun is between horizontal and half way to facing up
    /// </summary>
    [Range(0,1)]
    public float m_DontShootWhenPointIsFacingUp = 0.5f;

    void Awake() {
        //set up default values for weapon
        changeWeapon(m_WeaponType);

        m_CameraManager = FindObjectOfType<CameraManager>();
    }

    // Use this for initialization
    void Start () {
        //set up callback for game state changes
        GameStateManager.singleton.m_StateChanged.AddListener(stateChanged);
        
    }
	
	// Update is called once per frame
	void Update () {
        //code to remove the reloading effect
        if (m_IsReloading) {
            //if the time difference between reload start and now is larger then how long it takes to reload
            if (Time.unscaledTime - m_LastTime >= getWeapon().m_TimeToReload * SlowMoManager.m_PlayerSpeedScale) {
                m_IsReloading = false;
                //reset ammo
                m_CurrentAmmo = getWeapon().m_MaxAmmo;
                m_LastTime = -100;
            }
        }
        updateText();
    }

    /// <summary>
    /// will check if this weapon can shoot a bullet,
    /// if it can it will spawn a bullet using m_SpawnPoint
    /// </summary>
    /// <param name="m_SpawnPoint">Position and rotation the bullet will use, if null then this will be the transform</param>
    /// <returns>The Bullet Object, will be null if no bullet was spawned</returns>
    public GameObject shoot(Transform m_SpawnPoint) {

        //dont shoot if currently reloading
        if (m_IsReloading) {
            return null;
        }
        //dont shoot if no ammo, reload instead
        if (m_CurrentAmmo == 0) {
            reload();
            return null;
        }

        if(Mathf.Abs(Vector3.Dot(m_SpawnPoint.forward,Vector3.up)) > m_DontShootWhenPointIsFacingUp) {
            return null;
        }

        //can this weapon shoot again?
        //if the time difference between last shot start and now is larger then how long it takes to shoot again
        if (Time.unscaledTime - m_LastTime >= getWeapon().m_WeaponShootCooldown * SlowMoManager.m_PlayerSpeedScale) {
            //update last shoot time
            m_LastTime = Time.unscaledTime;

            //remove ammo from counter
            m_CurrentAmmo--;

            //run screenshake
            m_CameraManager.startScreenShake();

            //Spawn Muzzle flash
            GameObject muzzleFlash = Object.Instantiate(m_MuzzleFlashPrefab, m_MuzzleFlashSpawnPoint.position,(m_MuzzleFlashSpawnPoint.rotation * Quaternion.Euler(0, -90, 0)));

            //fire bullet
            return getWeapon().fireProjectile(m_SpawnPoint);
            


        }
        return null;
    }

    
    public void reload() {
        //if not reloading
        if (!m_IsReloading) {
            //dont reload if we are already at max ammo
            if (m_CurrentAmmo == getWeapon().m_MaxAmmo) {
                return;
            }
            getWeapon().PlayReloadSound();
            //set up last time to current time, and set reloading to true
            m_LastTime = Time.unscaledTime;
            m_IsReloading = true;
        }
    }

    /// <summary>
    /// debug logging of weapon information
    /// this should be removed later on for a better ui
    /// </summary>
    private void updateText() {

        if (m_DebugText == null) {
            return;
        }
        string text = "";
        float value = 0;
        if (m_IsReloading) {//if reloading
            value = (getWeapon().m_TimeToReload * SlowMoManager.m_PlayerSpeedScale) - (Time.unscaledTime - m_LastTime);
            text = "Reloading... ";
        } else {//if not reloading
            if (Time.unscaledTime - m_LastTime >= getWeapon().m_WeaponShootCooldown * SlowMoManager.m_PlayerSpeedScale || m_CurrentAmmo == 0) {//if can shoot
                text = m_CurrentAmmo + " / ";
                value = getWeapon().m_MaxAmmo;
            } else {//if cant shoot
                text = "Cooldown... ";
                value = (getWeapon().m_WeaponShootCooldown * SlowMoManager.m_PlayerSpeedScale) - (Time.unscaledTime - m_LastTime);
            }
        }
        //limit value to 3 decimal points
        int IntValue = (int)(value * 1000.0f);
        value = IntValue / 1000.0f;

        //combine and set up the text
        m_DebugText.text = text + value.ToString();
    }

    void OnDisable() {
        //remove debug text
        if (m_DebugText == null) {
            return;
        }
        m_DebugText.text = "";
    }

    private void stateChanged(GameStates a_NewState) {
        //what this does:
        //gets the time for when the game was paused
        //then when it's action mode again
        //it added the time between now and when it paused
        //so it should be the same amount of time left between pause and play
        //since the time will still count up, so without this, it will still count down even while paused
        switch (a_NewState) {
            case GameStates.Planning:
            case GameStates.Paused:
            default:
                m_DisableTime = Time.unscaledTime;
                break;
            case GameStates.Action:
                m_LastTime += Time.unscaledTime - m_DisableTime;
                break;
        }
    }

    public WeaponReference getWeapon() {
        return WeaponHolder.getWeapon(m_WeaponType);
    }

    public void changeWeapon(WeaponTypes a_NewWeapon) {
        m_WeaponType = a_NewWeapon;
        //reset all values to default
        m_CurrentAmmo = getWeapon().m_MaxAmmo;
        m_LastTime = -100;
        m_DisableTime = -100;
        m_IsReloading = false;
    }
}
